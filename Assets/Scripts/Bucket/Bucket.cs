using UnityEngine;

public class Bucket : MonoBehaviour
{
    public float speed = 0.5f;

    private Rigidbody2D rb;
    private int direction = 1; // 1 = sang phải, -1 = sang trái

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
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
    }

    void OnMouseDown()
    {
        direction *= -1; // đảo hướng
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
