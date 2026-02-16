using UnityEngine;
using UnityEngine.UI;
using TMPro; // Standard for Unity 6.x
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] private TextMeshProUGUI depthText;
    [SerializeField] private GameObject noiseBarPanel; // The parent of your existing NoiseUI

    [Header("Game Over References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Button restartButton;

    private void Start()
    {
        // Ensure correct initial state
        gameOverPanel.SetActive(false);
        noiseBarPanel.SetActive(true);

        // Hook up button
        restartButton.onClick.AddListener(OnRestartClicked);

        // Start Score Tracking
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.StartTracking();
    }

    private void Update()
    {
        // Update HUD
        if (ScoreManager.Instance != null && !GameManager.Instance.IsGameOver)
        {
            // Format: "125m"
            depthText.text = $"{Mathf.FloorToInt(ScoreManager.Instance.CurrentDepth)}";
        }
    }

    /// <summary>
    /// Called by GameManager when player dies
    /// </summary>
    public void ShowGameOver()
    {
        // 1. Hide HUD elements if desired
        noiseBarPanel.SetActive(false);
        depthText.gameObject.SetActive(false);

        // 2. Finalize Score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.StopTracking();
            finalScoreText.text = $"Depth: {Mathf.FloorToInt(ScoreManager.Instance.CurrentDepth)}m";
            highScoreText.text = $"Best: {Mathf.FloorToInt(ScoreManager.Instance.HighScore)}m";
        }

        // 3. Show Panel with simple animation
        gameOverPanel.SetActive(true);
    }

    private void OnRestartClicked()
    {
        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}