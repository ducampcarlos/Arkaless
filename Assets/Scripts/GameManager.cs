using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] GameObject gameMenuUI;
    bool gameStarted = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!gameStarted) DetectGameStart();
    }

    void DetectGameStart()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (Mouse.current.leftButton.wasPressedThisFrame || Keyboard.current.anyKey.wasPressedThisFrame)
            StartGame();
    }

    void StartGame()
    {
        gameStarted = true;
        gameMenuUI.SetActive(false);
        FindAnyObjectByType<Ball>().StartBounce();
    }

    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
}
