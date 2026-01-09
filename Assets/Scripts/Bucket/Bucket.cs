using UnityEngine;

public class Bucket : MonoBehaviour
{
    public int capacity = 100;
    public int currentSand = 0;

    public int bucketColor = 1;

    CircleCollider2D col;

    void Awake()
    {
        col = GetComponent<CircleCollider2D>();
    }

    void Start()
    {
    }


    public bool IsFull()
    {
        return currentSand >= capacity;
    }

    public bool IsAvailableColor(int color)
    {
        return color == bucketColor;
    }

    public void AddSand()
    {
        if (!IsFull())
        {   
            currentSand++;
        }
    }

    public void DeleteBucket()
    {
        Destroy(gameObject);
    }

    public bool ContainsPoint(Vector2 worldPos)
    {
        return col.OverlapPoint(worldPos);
    }
}
