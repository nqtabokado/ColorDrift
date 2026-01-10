using UnityEngine;

public class BucketLoop : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    private Collider2D col;
    private float halfWidth;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        halfWidth = col.bounds.extents.x/ Mathf.Sqrt(2f);
    }

    void FixedUpdate()
    {
        float rightEdge = transform.position.x + halfWidth;
        float spawnPos = startPoint.position.x + halfWidth;

        if (rightEdge > endPoint.position.x)
        {
            Vector3 pos = transform.position;
            pos.x = spawnPos;
            transform.position = pos;
        }
    }
}