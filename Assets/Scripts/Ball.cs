using UnityEngine;

public class Ball : MonoBehaviour
{
    [Tooltip("Initial force magnitude to launch the ball")]
    public float speed = 15f;

    private Rigidbody2D rb;
    private Vector3 initialPosition;

    bool isBouncing = false;

    [SerializeField] AudioClip hitPaddle;
    [SerializeField] AudioClip hitBlock;

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
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
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

    public void Stop()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
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
            AudioManager.Instance.PlaySFX(hitPaddle);

            // Calculate hit point relative to paddle center
            ContactPoint2D contact = collision.contacts[0];
            float paddleWidth = collision.collider.bounds.size.x;
            float offset = contact.point.x - collision.collider.transform.position.x;
            float normalizedOffset = Mathf.Clamp(offset / (paddleWidth * 0.5f), -1f, 1f);

            // New direction: X by offset, Y always positive
            Vector2 newDir = new Vector2(normalizedOffset, 1f).normalized;

            // If hit too close to center, apply slight random angle to avoid vertical loops
            if (Mathf.Abs(normalizedOffset) < 0.1f)
            {
                float randomAngle = Random.Range(-10f, 10f); // degrees
                newDir = Quaternion.Euler(0, 0, randomAngle) * newDir;
                newDir.Normalize();
            }

            rb.linearVelocity = newDir * speed;
        }
        else if (collision.gameObject.CompareTag("Block"))
        {
            AudioManager.Instance.PlaySFX(hitBlock);
        }
    }


}
