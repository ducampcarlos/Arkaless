using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using System;
using System.Linq;

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
        else { Destroy(gameObject); return; }

        // Activar Enhanced Touch
        EnhancedTouchSupport.Enable();
    }

    void Update()
    {
        if (!gameStarted) DetectGameStart();
    }

    void DetectGameStart()
    {
        bool keyPressed = Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool touchPressed = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        if (keyPressed || mousePressed || touchPressed)
            StartGame();
    }

    void StartGame()
    {
        gameStarted = true;
        if (gameMenuUI != null) gameMenuUI.SetActive(false);
        OnGameStarted?.Invoke();
    }

    public void TriggerGameLost()
    {
        if (!gameStarted) return;
        OnGameLost?.Invoke();
    }

    public void RestartImmediate()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnDestroy()
    {
        // Desactivar Enhanced Touch al salir (opcional)
        EnhancedTouchSupport.Disable();
    }
}
