using UnityEngine;

public class Ball : MonoBehaviour
{
    [Tooltip("Initial force magnitude to launch the ball")]
    public float speed = 15f;

    private Rigidbody2D rb;
    private Vector3 initialPosition;

    bool isBouncing = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    /// <summary>
    /// Launches the ball from rest in a random direction.
    /// </summary>
    public void StartBounce()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(randomDir * speed, ForceMode2D.Impulse);
        isBouncing = true;
    }

    /// <summary>
    /// Resets the ball to its starting position and stops all motion.
    /// </summary>
    public void ResetPosition()
    {
        isBouncing = false;
        rb.linearVelocity = Vector2.zero;
        transform.position = initialPosition;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FallOffBoundary"))
        {
            GameManager.Instance.TriggerGameLost();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle") && isBouncing)
        {
            // Calculate hit point relative to paddle center
            ContactPoint2D contact = collision.contacts[0];
            float paddleWidth = collision.collider.bounds.size.x;
            float offset = contact.point.x - collision.collider.transform.position.x;
            float normalizedOffset = Mathf.Clamp(offset / (paddleWidth * 0.5f), -1f, 1f);

            // New direction: X by offset, Y always positive
            Vector2 newDir = new Vector2(normalizedOffset, 1f).normalized;
            rb.linearVelocity = newDir * speed;
        }
    }
}
