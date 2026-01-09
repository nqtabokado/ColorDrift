using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public float speed = 0.5f;
    public Vector2 direction = Vector2.right; // trái = Vector2.left

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D rb = other.attachedRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(
                direction.x * speed,
                rb.linearVelocity.y
            );
        }
    }
}
