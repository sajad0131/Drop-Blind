using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] private TextMeshProUGUI depthText;
    [SerializeField] private GameObject noiseBarPanel;

    [Header("Game Over References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;

    private void Awake()
    {
        // THIS IS THE FIX: The new UI registers itself directly to the manager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterUI(this);
        }
    }

    private void Start()
    {
        // Ensure correct initial state
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (noiseBarPanel != null) noiseBarPanel.SetActive(true);

        // Hook up button
        if (restartButton != null) restartButton.onClick.AddListener(OnRestartClicked);

        // Start Score Tracking
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StartTracking();
    }

    private void Update()
    {
        // Update HUD
        if (ScoreManager.Instance != null && GameManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            if (depthText != null) depthText.text = $"{Mathf.FloorToInt(ScoreManager.Instance.CurrentDepth)}";
        }
    }

    public void ShowGameOver()
    {
        // Added safety null checks to completely prevent missing reference errors
        if (noiseBarPanel != null) noiseBarPanel.SetActive(false);
        if (depthText != null) depthText.gameObject.SetActive(false);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StopTracking();
            if (finalScoreText != null) finalScoreText.text = $"Depth: {Mathf.FloorToInt(ScoreManager.Instance.CurrentDepth)}m";
            if (highScoreText != null) highScoreText.text = $"Best: {Mathf.FloorToInt(ScoreManager.Instance.HighScore)}m";
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    private void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}