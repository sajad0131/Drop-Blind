using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Settings")]
    [Tooltip("Score multiplier per unit of distance fallen.")]
    [SerializeField] private float scoreMultiplier = 1f;

    public float CurrentDepth { get; private set; }
    public float HighScore { get; private set; }

    private bool _isTracking = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Load saved high score (works on WebGL/Mobile)
        HighScore = PlayerPrefs.GetFloat("HighScore", 0);
    }

    public void StartTracking()
    {
        CurrentDepth = 0;
        _isTracking = true;
    }

    public void StopTracking()
    {
        _isTracking = false;

        // Check for new high score
        if (CurrentDepth > HighScore)
        {
            HighScore = CurrentDepth;
            PlayerPrefs.SetFloat("HighScore", HighScore);
            PlayerPrefs.Save();
        }
    }

    private void Update()
    {
        if (!_isTracking || GameManager.Instance.IsGameOver) return;

        // Calculate distance based on WorldScroller speed
        // Since WorldScroller moves the world UP, we are technically falling that distance.
        if (WorldScroller.Instance != null)
        {
            // Accessing the private 'fallSpeed' would require a getter in WorldScroller, 
            // OR we just accumulate time * speed. 
            // Let's assume constant calculation or add a getter to WorldScroller later.
            // For now, let's use a public property or estimation.
            // Ideally: CurrentDepth += WorldScroller.Instance.CurrentSpeed * Time.deltaTime;
        }

        // Simpler fallback if WorldScroller speed isn't public yet:
        // We will add a getter to WorldScroller in the step below, or just use Time for now.
        CurrentDepth += 10f * Time.deltaTime * scoreMultiplier; // Placeholder speed
    }
}