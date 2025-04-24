using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Tooltip("All your levels in order")]
    public LevelData[] levels;
    [Tooltip("Block prefab with Block.cs")]
    public GameObject blockPrefab;
    [Tooltip("World-space spacing between blocks")]
    public Vector2 blockSpacing = new Vector2(1f, 0.5f);

    private int currentLevelIndex = 0;
    private int remainingBlocks;
    private List<Block> registeredBlocks = new();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void RegisterBlock(Block block)
    {
        registeredBlocks.Add(block);
    }

    public void NotifyBlockDestroyed(Block block)
    {
        remainingBlocks--;
        if (remainingBlocks <= 0)
        {
            NextLevel();
        }
    }

    void LoadLevel(int index)
    {
        // Limpiar lista de bloques previa
        registeredBlocks.Clear();

        var lvl = levels[index];
        int w = lvl.width, h = lvl.height;
        remainingBlocks = 0;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int durability = lvl.cellDurabilities[y * w + x];
                if (durability <= 0) continue;

                // Instanciar bloque
                Vector3 pos = new Vector3(
                    (x - w / 2f) * blockSpacing.x,
                    (y + 1) * blockSpacing.y,
                    0f
                );
                var go = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                var blk = go.GetComponent<Block>();
                blk.durability = durability;
                remainingBlocks++;
            }
        }
    }

    void NextLevel()
    {
        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
        {
            // Fin del juego o reinicio
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            // Destruir bloques sobrantes y cargar siguiente
            foreach (var b in registeredBlocks)
                if (b != null) Destroy(b.gameObject);

            LoadLevel(currentLevelIndex);
            // Reset bola y paleta según tu lógica de GameManager
            FindAnyObjectByType<Ball>().ResetPosition();
            FindAnyObjectByType<Paddle>().ResetPosition();
        }
    }
}
