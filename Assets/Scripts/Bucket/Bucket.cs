using UnityEngine;

public class Bucket : MonoBehaviour
{
    public float speed = 0.5f;

    private Rigidbody2D rb;
    private int direction = 1; // 1 = sang phải, -1 = sang trái

    public float attractForce = 10f;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Sand")) return;

        Sand sand = other.GetComponent<Sand>();
        if (sand != null)
        {
            sand.StartAttract(transform);
        }
    }
}
