using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement; // NEEDED FOR RELOADS

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager Instance { get; private set; }

    [Header("Noise Settings")]
    [SerializeField] private float decayRate = 15f;
    [SerializeField] private float maxNoise = 100f;
    [SerializeField] private float noiseThreshold = 80f;

    [Header("Debug")]
    [SerializeField] private float currentNoise = 0f;

    public UnityEvent<float> OnNoiseLevelChanged;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        // Listen for when the player hits "Retry"
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // THIS FIXES YOUR BUG: 
        // Wipes all connections to the old destroyed Sliders
        OnNoiseLevelChanged.RemoveAllListeners();

        // Reset the noise for the new round
        currentNoise = 0f;
    }

    private void Update()
    {
        if (currentNoise > 0)
        {
            currentNoise -= decayRate * Time.deltaTime;
            currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);

            OnNoiseLevelChanged?.Invoke(currentNoise / maxNoise);
        }
    }

    public void AddNoise(float amount)
    {
        if (currentNoise >= maxNoise) return;

        currentNoise += amount;
        currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);

        OnNoiseLevelChanged?.Invoke(currentNoise / maxNoise);

        if (currentNoise >= noiseThreshold)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayWarning();
        }
    }
}