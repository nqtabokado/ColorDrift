using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SandController : MonoBehaviour
{
    public int cols = 150;
    public int rows = 150;
    Texture2D texture;

    public float sandStepTime = 0.05f; // 0.05s = 20 lần / giây
    float sandTimer;

    Bucket[] buckets;

    Dictionary<string, int> nameToId;
    Dictionary<int, string> idToName;

    // 0 = rỗng
    // 1 = cát
    int[,] grid;

    void Start()
    {
        // lấy màu từ json
        Load();
        // lấy map
        LoadMap();

        buckets = FindObjectsByType<Bucket>(FindObjectsSortMode.None);
    }

    void Load()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("color_config");

        RootConfig colorConfig = JsonUtility.FromJson<RootConfig>(jsonFile.text);

        BuildMaps(colorConfig.regions);
    }

    void LoadMap()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("output");

        RootLevel levelConfig = JsonUtility.FromJson<RootLevel>(jsonFile.text);
        cols = levelConfig.width;
        rows = levelConfig.height;
        
        grid = new int[cols, rows];
        texture = new Texture2D(cols, rows);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        
        // tô màu trắng
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                grid[x, y] = 1;
                texture.SetPixel(x, y, Color.white);
            }
        }
        foreach (ColorPosition region in levelConfig.regions)
        {
            grid[region.x, region.y] = nameToId[region.region];
            texture.SetPixel(region.x, region.y, NameToColor(region.region));
        }

        texture.Apply();
        // Gán texture thành sprite
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, cols, rows),
            new Vector2(0.5f, 0.5f), // pivot giữa
            cols                    // 👈 PPU = số pixel
        );

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
    }

    void BuildMaps(RegionConfig[] regions)
    {
        nameToId = new Dictionary<string, int>();
        idToName = new Dictionary<int, string>();

        int id = 2;
        nameToId["other"] = 1;
        idToName[1] = "other";
        foreach (RegionConfig region in regions)
        {
            if (nameToId.ContainsKey(region.name)) continue;
            nameToId[region.name] = id;
            idToName[id] = region.name;
            id++;
        }
    }

    Color NameToColor(string name)
    {
        var prop = typeof(Color).GetProperty(
            name,
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.Static |
            System.Reflection.BindingFlags.IgnoreCase
        );

        if (prop != null)
            return (Color)prop.GetValue(null);

        // màu custom
        return name.ToLower() switch
        {
            "orange" => new Color(1f, 0.5f, 0f),
            "purple" => new Color(0.6f, 0f, 1f),
            _ => Color.white
        };
    }

    Color IdToColor(int id)
    {
        if (!idToName.TryGetValue(id, out var name))
            return Color.white;

        return NameToColor(name);
    }

    // Update is called once per frame
    void Update()
    {
        sandTimer += Time.deltaTime;

        if (sandTimer >= sandStepTime)
        {
            sandTimer = 0f;
            UpdateSand();
            DestroySandInBuckets();
            texture.Apply();
        }
    }

    void MoveSand(int fromX, int fromY, int toX, int toY)
    {
        int tempPosition = grid[fromX, fromY];
        grid[fromX, fromY] = 0;
        grid[toX, toY] = tempPosition;

        Color color = IdToColor(tempPosition);

        texture.SetPixel(fromX, fromY, Color.clear);   // rỗng
        texture.SetPixel(toX, toY, color);    // cát
    }

    void UpdateSand()
    {
        for (int x = 0; x < cols; x++) // bắt đầu từ y=1
        {
            for (int y = 1; y < rows; y++)
            {

                if (grid[x, y] == 0) continue;
                // Rơi thẳng
                if (grid[x, y - 1] == 0)
                {
                    MoveSand(x, y, x, y - 1);
                }
                // Trượt chéo
                else
                {
                    bool leftFirst = Random.value < 0.5f;

                    if (leftFirst)
                    {
                        if (x > 0 && grid[x - 1, y - 1] == 0)
                            MoveSand(x, y, x - 1, y - 1);
                        else if (x < cols - 1 && grid[x + 1, y - 1] == 0)
                            MoveSand(x, y, x + 1, y - 1);
                    }
                    else
                    {
                        if (x < cols - 1 && grid[x + 1, y - 1] == 0)
                            MoveSand(x, y, x + 1, y - 1);
                        else if (x > 0 && grid[x - 1, y - 1] == 0)
                            MoveSand(x, y, x - 1, y - 1);
                    }
                }
            }
        }
    }

    Vector2 GridToWorld(int x, int y)
    {
        float wx = (x / (float)cols) - 0.5f;
        float wy = (y / (float)rows) - 0.5f;
        return transform.TransformPoint(new Vector3(wx, wy));
    }

    void DestroySandInBuckets()
    {
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows; y++)
            {

                if (grid[x, y] == 0) continue;

                Vector2 worldPos = GridToWorld(x, y);

                foreach (var bucket in buckets)
                {
                    if (bucket == null) continue;

                    if (!bucket.IsAvailableColor(grid[x, y])) continue;

                    if (bucket.IsFull())
                    {
                        bucket.DeleteBucket();
                    }

                    if (bucket.ContainsPoint(worldPos))
                    {
                        grid[x, y] = 0;
                        texture.SetPixel(x, y, Color.clear);
                        bucket.AddSand();
                        break; // 1 hạt chỉ vào 1 bucket
                    }
                }
            }
        }
    }
}
