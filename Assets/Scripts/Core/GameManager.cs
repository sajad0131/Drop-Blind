using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool IsGameOver = false;

    // Reference to the UI Controller
    [SerializeField] private GameUI gameUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        Debug.Log("GAME OVER");

        // 1. Stop the World Scrolling
        if (WorldScroller.Instance != null)
        {
            WorldScroller.Instance.SetSpeed(0);
        }

        // 2. Disable Player Controls
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.enabled = false;

        // 3. Show UI
        if (gameUI != null)
        {
            gameUI.ShowGameOver();
        }
        else
        {
            // Fallback if UI isn't linked
            Debug.LogError("GameUI reference missing in GameManager!");
            Invoke(nameof(RestartLevel), 2f);
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}