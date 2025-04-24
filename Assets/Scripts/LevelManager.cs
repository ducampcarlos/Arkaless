using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Tooltip("All your levels in order")] public LevelData[] levels;
    [Tooltip("Block prefab with Block.cs")] public GameObject blockPrefab;
    [Tooltip("World-space spacing between blocks")] public Vector2 blockSpacing = new Vector2(1f, 0.5f);

    [Header("Sound Effects")]
    public AudioClip startClip;
    public AudioClip levelCompleteClip;
    public AudioClip loseClip;

    private AudioSource audioSource;
    private int currentLevelIndex = 0;
    private int remainingBlocks;
    private List<Block> registeredBlocks = new List<Block>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Suscribirse a eventos del GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted += HandleGameStarted;
            GameManager.Instance.OnGameLost += HandleGameLost;
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted -= HandleGameStarted;
            GameManager.Instance.OnGameLost -= HandleGameLost;
        }
    }

    // Se dispara tras StartGame()
    private void HandleGameStarted()
    {
        StartCoroutine(StartLevelRoutine());
    }

    private IEnumerator StartLevelRoutine()
    {
        if (startClip != null)
        {
            audioSource.PlayOneShot(startClip);
            yield return new WaitForSeconds(startClip.length);
        }
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
            StartCoroutine(LevelCompleteRoutine());
    }

    private IEnumerator LevelCompleteRoutine()
    {
        if (levelCompleteClip != null)
        {
            audioSource.PlayOneShot(levelCompleteClip);
            yield return new WaitForSeconds(levelCompleteClip.length);
        }
        AdvanceToNextLevel();
    }

    private void AdvanceToNextLevel()
    {
        // Limpiar bloques antiguos
        foreach (var b in registeredBlocks)
            if (b != null) Destroy(b.gameObject);
        registeredBlocks.Clear();

        currentLevelIndex++;
        if (currentLevelIndex >= levels.Length)
        {
            // Fin de juego: reinicio inmediato
            GameManager.Instance.RestartImmediate();
        }
        else
        {
            LoadLevel(currentLevelIndex);
            FindAnyObjectByType<Ball>().ResetPosition();
            FindAnyObjectByType<Paddle>().ResetPosition();
        }
    }

    private void LoadLevel(int index)
    {
        registeredBlocks.Clear();
        remainingBlocks = 0;

        var lvl = levels[index];
        int w = lvl.width, h = lvl.height;

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int durability = lvl.cellDurabilities[y * w + x];
                if (durability <= 0) continue;

                Vector3 pos = new Vector3(
                    (x - w / 2f) * blockSpacing.x,
                    (y + 1) * blockSpacing.y,
                    0f
                );
                var go = Instantiate(blockPrefab, pos, Quaternion.identity, transform);
                var blk = go.GetComponent<Block>();
                blk.durability = durability;
                registeredBlocks.Add(blk);
                remainingBlocks++;
            }
        }
    }

    // Se dispara tras pérdida de partida
    private void HandleGameLost()
    {
        StartCoroutine(LoseRoutine());
    }

    private IEnumerator LoseRoutine()
    {
        if (loseClip != null)
        {
            audioSource.PlayOneShot(loseClip);
            yield return new WaitForSeconds(loseClip.length);
        }
        GameManager.Instance.RestartImmediate();
    }
}