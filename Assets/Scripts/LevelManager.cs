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
    [Tooltip("Sound that plays only once when the game is first started")] public AudioClip gameStartClip;
    [Tooltip("Sound that plays before each level (round) starts")] public AudioClip roundStartClip;
    [Tooltip("Sound that plays when the player loses")] public AudioClip loseClip;

    [Header("Pooling")]
    [Tooltip("Initial number of blocks to pool")] public int initialPoolSize = 20;
    private List<GameObject> blockPool = new List<GameObject>();

    private int currentLevelIndex = 0;
    private int remainingBlocks;
    private List<Block> registeredBlocks = new List<Block>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted += HandleGameStarted;
            GameManager.Instance.OnGameLost += HandleGameLost;
        }
        InitBlockPool();

    }

    void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStarted -= HandleGameStarted;
            GameManager.Instance.OnGameLost -= HandleGameLost;
        }
    }

    private void InitBlockPool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            GameObject block = Instantiate(blockPrefab);
            block.SetActive(false);
            block.transform.parent = transform;
            blockPool.Add(block);
        }
    }

    private GameObject GetBlockFromPool()
    {
        foreach (var block in blockPool)
        {
            if (!block.activeInHierarchy)
                return block;
        }

        GameObject newBlock = Instantiate(blockPrefab);
        newBlock.transform.parent = transform;
        newBlock.SetActive(false);
        blockPool.Add(newBlock);
        return newBlock;
    }


    private void HandleGameStarted()
    {
        StartCoroutine(GameStartRoutine());
    }

    private IEnumerator GameStartRoutine()
    {
        if (gameStartClip != null)
        {
            AudioManager.Instance.PlaySFX(gameStartClip);
            yield return new WaitForSeconds(gameStartClip.length);
        }

        yield return StartCoroutine(RoundStartRoutine());
    }

    private IEnumerator RoundStartRoutine()
    {
        var paddle = FindAnyObjectByType<Paddle>();
        var ball = FindAnyObjectByType<Ball>();
        if (paddle != null) { paddle.isAllowedToMove = false; paddle.ResetPosition(); }
        if (ball != null) ball.ResetPosition();

        LoadLevel(currentLevelIndex);

        if (roundStartClip != null)
        {
            AudioManager.Instance.PlaySFX(roundStartClip);
            yield return new WaitForSeconds(roundStartClip.length);
        }

        AudioManager.Instance.PlayMusicFromStart();

        if (ball != null) ball.StartBounce();
        if (paddle != null) paddle.isAllowedToMove = true;
    }

    public void RegisterBlock(Block block)
    {
        registeredBlocks.Add(block);
    }

    public void NotifyBlockDestroyed(Block block)
    {
        remainingBlocks--;
        if (remainingBlocks <= 0)
            StartCoroutine(HandleRoundComplete());
    }

    private IEnumerator HandleRoundComplete()
    {
        var paddle = FindAnyObjectByType<Paddle>();
        if (paddle != null) paddle.isAllowedToMove = false;
        AudioManager.Instance.StopMusic();

        currentLevelIndex++;

        if (currentLevelIndex >= levels.Length)
        {
            if (loseClip != null)
            {
                AudioManager.Instance.PlaySFX(loseClip);
                yield return new WaitForSeconds(loseClip.length);
            }
            GameManager.Instance.RestartImmediate();
            yield break;
        }

        yield return StartCoroutine(RoundStartRoutine());
    }

    private void LoadLevel(int index)
    {
        foreach (var b in registeredBlocks)
            if (b != null) b.gameObject.SetActive(false);
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
                    (x - w / 2f) * blockSpacing.x+1,
                    (y + 1) * blockSpacing.y,
                    0f
                );
                var go = GetBlockFromPool();
                go.transform.position = pos;
                go.transform.rotation = Quaternion.identity;
                go.SetActive(true);
                var blk = go.GetComponent<Block>();
                blk.durability = durability;
                blk.UpdateVisual();
                registeredBlocks.Add(blk);
                remainingBlocks++;
            }
        }
    }

    private void HandleGameLost()
    {
        StartCoroutine(LoseRoutine());
        AudioManager.Instance.StopMusic();
    }

    private IEnumerator LoseRoutine()
    {
        if (loseClip != null)
        {
            AudioManager.Instance.PlaySFX(loseClip);
            yield return new WaitForSeconds(loseClip.length);
        }
        GameManager.Instance.RestartImmediate();
    }
}