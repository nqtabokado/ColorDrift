using UnityEngine;

public class Sand : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float wakeRadius = 0.2f;

    private Transform target;
    private bool isAttracting = false;

    public void StartAttract(Transform center)
    {
        target = center;
        isAttracting = true;

        WakeNearbySand();

        // Tắt collider → xuyên
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Không cần physics nữa
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.simulated = false;
        }
    }

    void Update()
    {
        if (!isAttracting || target == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, target.position) < 0.05f)
        {
            Destroy(gameObject);
        }
    }

    void WakeNearbySand()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            wakeRadius
        );

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Sand")) continue;

            Rigidbody2D rb = hit.attachedRigidbody;
            if (rb != null)
            {
                rb.WakeUp();
            }
        }
    }
}
