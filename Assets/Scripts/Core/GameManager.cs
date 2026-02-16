using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool IsGameOver = false;

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

        // 3. TODO: Show UI / Restart Button
        // For now, simple reload after delay for testing
        Invoke(nameof(RestartLevel), 2f);
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}