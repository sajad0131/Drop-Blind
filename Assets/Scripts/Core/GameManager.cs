using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public bool IsGameOver = false;

    [SerializeField]private GameUI gameUI;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        gameUI = FindObjectOfType<GameUI>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        IsGameOver = false;
        // Notice we removed the search here! The UI will register itself.
    }

    // NEW METHOD: The UI will call this to link itself safely
    public void RegisterUI(GameUI ui)
    {
        gameUI = ui;
    }

    public void TriggerGameOver()
    {
        if (IsGameOver) return;

        IsGameOver = true;
        Debug.Log("GAME OVER");

        if (WorldScroller.Instance != null)
        {
            WorldScroller.Instance.SetSpeed(0);
        }

        var player = FindFirstObjectByType<PlayerController>();
        if (player != null) player.enabled = false;

        if (gameUI != null)
        {
            gameUI.ShowGameOver();
        }
        else
        {
            Invoke(nameof(RestartLevel), 2f);
        }
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}