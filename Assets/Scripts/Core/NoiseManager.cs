using UnityEngine;
using UnityEngine.Events;

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager Instance { get; private set; }

    [Header("Noise Settings")]
    [Tooltip("How fast noise reduces per second when idle.")]
    [SerializeField] private float decayRate = 15f;
    [Tooltip("Maximum noise level.")]
    [SerializeField] private float maxNoise = 100f;

    [Header("Debug")]
    [SerializeField] private float currentNoise = 0f;


    [Header("Settings")]
    [SerializeField] private float noiseThreshold = 80f; // Point where monster hears you

    // Event for UI and Monster to listen to (Optimization: Better than checking every frame)
    public UnityEvent<float> OnNoiseLevelChanged;

    private void Awake()
    {
        // Singleton Pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (currentNoise > 0)
        {
            // Decay noise over time
            currentNoise -= decayRate * Time.deltaTime;
            currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);

            // Notify listeners
            OnNoiseLevelChanged?.Invoke(currentNoise / maxNoise); // Return normalized 0-1 value
        }
    }

    /// <summary>
    /// Call this from PlayerController or Obstacles
    /// </summary>
    /// <param name="amount">Raw amount to add (e.g., 20 for a tap)</param>
    public void AddNoise(float amount)
    {

        if (currentNoise >= maxNoise) return;

        currentNoise += amount;
        currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);

        OnNoiseLevelChanged?.Invoke(currentNoise / maxNoise);

        if (currentNoise >= maxNoise)
        {
            // We let ChaserController handle the actual Kill logic 
            // when it physically reaches the player, or trigger it here directly.
            Debug.LogWarning("MAX NOISE REACHED!");
        }

        if (currentNoise >= noiseThreshold)
        {
            // The AudioManager handles the cooldown, so we can safely call this every frame
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayWarning();
            }
        }
    }
}