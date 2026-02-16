using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance { get; private set; }

    [SerializeField] private LevelData levelData;

    private float _elapsedTime;
    public float CurrentSpeedMultiplier { get; private set; } = 1f;
    public float CurrentNoiseMultiplier { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameOver) return;

        _elapsedTime += Time.deltaTime;

        // Evaluate curves based on time
        CurrentSpeedMultiplier = levelData.speedOverTime.Evaluate(_elapsedTime);
        CurrentNoiseMultiplier = levelData.noiseScalingOverTime.Evaluate(_elapsedTime);

        // Update World Scroller speed in real-time
        if (WorldScroller.Instance != null)
        {
            float targetSpeed = levelData.baseGlobalSpeed * CurrentSpeedMultiplier;

            WorldScroller.Instance.SetSpeed(targetSpeed);
        }
    }
}