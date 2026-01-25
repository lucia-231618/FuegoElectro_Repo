using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public float speed = 2f;
    public float distance = 5f;
    private Vector3 startPosition;
    private Rigidbody2D rb;

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float offset = Mathf.PingPong(Time.time * speed, distance);
        Vector3 newPosition = startPosition + new Vector3(offset, 0f, 0f);

        rb.MovePosition(newPosition);
    }
}