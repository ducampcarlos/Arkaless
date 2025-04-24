using UnityEngine;

public class Block : MonoBehaviour
{
    [Tooltip("Starting durability for this block instance")]
    public int durability = 1;
    [Tooltip("Sprites per durability level; index 0 = durability 1")]
    public Sprite[] durabilitySprites;

    private SpriteRenderer sr;
    private LevelManager levelManager;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Update sprite based on initial durability
        UpdateVisual();
        levelManager = FindAnyObjectByType<LevelManager>();
        //levelManager.RegisterBlock(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        durability--;
        if (durability > 0)
        {
            UpdateVisual();
        }
        else
        {
            gameObject.SetActive(false);
            levelManager.NotifyBlockDestroyed(this);
        }
    }

    public void UpdateVisual()
    {
        int idx = Mathf.Clamp(durability - 1, 0, durabilitySprites.Length - 1);
        sr.sprite = durabilitySprites[idx];
    }
}
