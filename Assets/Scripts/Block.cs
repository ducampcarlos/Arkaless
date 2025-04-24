using UnityEngine;

public class Block : MonoBehaviour
{
    [Tooltip("Starting durability for this block instance")]
    public int durability = 1;
    [Tooltip("Colors per durability level; index 0 = durability 1")]
    public Color[] durabilityColors;

    private SpriteRenderer sr;
    private LevelManager levelManager;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // Actualizar color inicial
        UpdateColor();
        levelManager = FindAnyObjectByType<LevelManager>();
        levelManager.RegisterBlock(this);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ball")) return;

        durability--;
        if (durability > 0)
        {
            UpdateColor();
        }
        else
        {
            levelManager.NotifyBlockDestroyed(this);
            Destroy(gameObject);
        }
    }

    void UpdateColor()
    {
        int idx = Mathf.Clamp(durability - 1, 0, durabilityColors.Length - 1);
        sr.color = durabilityColors[idx];
    }
}
