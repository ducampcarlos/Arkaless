using TMPro;
using UnityEngine;

public class Ball : MonoBehaviour
{
    Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartBounce();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StartBounce()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        rb.AddForce(randomDirection * 10f, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FallOffBoundary"))
        {
            GameManager.Instance.Restart();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Paddle"))
        {
            GameManager.Instance.ScoreUp();
        }
    }
}
