using UnityEngine;

public class Ball : MonoBehaviour
{
    [Tooltip("Initial force magnitude to launch the ball")]
    public float speed = 15f;

    [SerializeField] AudioClip hitPaddle;
    [SerializeField] AudioClip hitBlock;

    private Rigidbody2D rb;
    private Vector3 initialPosition;
    private bool isBouncing = false;

    // Shared cooldown timer for playing any hit SFX
    private static float lastSFXTime = -Mathf.Infinity;
    private const float sfxCooldown = 0.05f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    public void StartBounce()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        isBouncing = true;
    }

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
            GameManager.Instance.TriggerGameLost();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Handle paddle hits
        if (collision.gameObject.CompareTag("Paddle") && isBouncing)
        {
            // Play paddle SFX if cooldown has passed
            if (Time.time - lastSFXTime >= sfxCooldown)
            {
                AudioManager.Instance.PlaySFX(hitPaddle);
                lastSFXTime = Time.time;
            }

            // Get contact info
            ContactPoint2D contact = collision.contacts[0];
            float paddleCenterY = collision.collider.bounds.center.y;

            // If contact is on the bottom half of the paddle, skip custom redirect
            if (contact.point.y < paddleCenterY)
                return;

            // Otherwise compute custom bounce angle
            float paddleWidth = collision.collider.bounds.size.x;
            float offset = contact.point.x - collision.collider.transform.position.x;
            float normOffset = Mathf.Clamp(offset / (paddleWidth * 0.5f), -1f, 1f);

            Vector2 newDir = new Vector2(normOffset, 1f).normalized;

            // Prevent straight vertical loops by adding small random angle if near center
            if (Mathf.Abs(normOffset) < 0.1f)
            {
                float randomAngle = Random.Range(-10f, 10f);
                newDir = (Quaternion.Euler(0, 0, randomAngle) * newDir).normalized;
            }

            rb.linearVelocity = newDir * speed;
        }
        // Handle block hits
        else if (collision.gameObject.CompareTag("Block"))
        {
            if (Time.time - lastSFXTime >= sfxCooldown)
            {
                AudioManager.Instance.PlaySFX(hitBlock);
                lastSFXTime = Time.time;
            }
        }
    }
}
