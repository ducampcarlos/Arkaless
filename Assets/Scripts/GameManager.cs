using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    int score = 0;
    [SerializeField] TextMeshProUGUI scoreText;

    [SerializeField] GameObject gameMenuUI;

    bool gameStarted = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        // Initialize the score text
        scoreText.text = score.ToString();
        scoreText.gameObject.SetActive(false);

        // Set the game menu UI to active
        gameMenuUI.SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        if(!gameStarted)
        {
            DetectGameStart();
        }
    }

    private void DetectGameStart()
    {
        if (gameStarted)
        {
            return;
        }

        // Check if the game is started
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
#if UNITY_ANDROID || UNITY_EDITOR
        // Android Controls (Touch)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            StartGame();
        }
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        // PC Controls
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartGame();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartGame();
        }
#endif
    }

    void StartGame()
    {
        gameStarted = true;
        FindAnyObjectByType<Ball>().StartBounce();
        gameMenuUI.SetActive(false);
        scoreText.gameObject.SetActive(true);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ScoreUp()
    {
        score++;
        scoreText.text = score.ToString();
    }
}
