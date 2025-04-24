using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action OnGameStarted;
    public event Action OnGameLost;

    [SerializeField] private GameObject gameMenuUI;
    private bool gameStarted = false;

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
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

        bool keyPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool touchPressed = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        if (keyPressed || touchPressed)
            StartGame();
    }

    void StartGame()
    {
        gameStarted = true;
        if (gameMenuUI != null) gameMenuUI.SetActive(false);
        FindAnyObjectByType<Ball>().StartBounce();
        OnGameStarted?.Invoke();
    }

    // Llama a este método para notificar pérdida de partida
    public void TriggerGameLost()
    {
        if (!gameStarted) return;
        OnGameLost?.Invoke();
    }

    // Reinicio inmediato de escena
    public void RestartImmediate()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}