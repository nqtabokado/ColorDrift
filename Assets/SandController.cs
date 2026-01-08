using UnityEngine;

public class SandController : MonoBehaviour
{
    public int cols = 50;
    public int rows = 50;
    Texture2D texture;

    public float sandStepTime = 0.05f; // 0.05s = 20 lần / giây
    float sandTimer;

// 0 = rỗng
// 1 = cát
    int[,] grid;

    void Start()
    {
        grid = new int[cols, rows];
        texture = new Texture2D(cols, rows);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        for (int x = 0; x < cols; x++) {
            for (int y = 0; y < rows; y++)
            {

                grid[x, y] = Random.value < 0.5f ? 0 : 1;
                // grid[x, y] = 0;
                Color color = grid[x, y] == 1
                    ? Color.blue
                    : Color.white;
                texture.SetPixel(x, y, color);
            }
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

    // Update is called once per frame
    void Update()
    {
        sandTimer += Time.deltaTime;

        if (sandTimer >= sandStepTime) {
            sandTimer = 0f;
            UpdateSand();
            texture.Apply();
        }
    }

    void MoveSand(int fromX, int fromY, int toX, int toY)
    {
        grid[fromX, fromY] = 0;
        grid[toX, toY] = 1;

        texture.SetPixel(fromX, fromY, Color.white);   // rỗng
        texture.SetPixel(toX, toY, Color.blue);    // cát
    }

    void UpdateSand()
    {
        for (int x = 0; x < cols; x++) // bắt đầu từ y=1
        {
            for (int y = 1; y < rows; y++)
            {

                if (grid[x, y] != 1) continue;
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
}
