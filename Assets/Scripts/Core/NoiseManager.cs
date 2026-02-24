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
    public float CurrentNoiseNormalized => maxNoise <= 0f ? 0f : currentNoise / maxNoise;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        if (OnNoiseLevelChanged == null)
        {
            OnNoiseLevelChanged = new UnityEvent<float>();
        }

        // Listen for when the player hits "Retry"
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetNoise();
    }

    public void ResetNoise()
    {
        currentNoise = 0f;
        OnNoiseLevelChanged?.Invoke(CurrentNoiseNormalized);
    }

    private void Update()
    {
        if (currentNoise > 0)
        {
            currentNoise -= decayRate * Time.deltaTime;
            currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);

            OnNoiseLevelChanged?.Invoke(CurrentNoiseNormalized);
        }
    }

    public void AddNoise(float amount)
    {
        if (currentNoise >= maxNoise) return;

        currentNoise += amount;
        currentNoise = Mathf.Clamp(currentNoise, 0, maxNoise);

        OnNoiseLevelChanged?.Invoke(CurrentNoiseNormalized);

        if (currentNoise >= noiseThreshold)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayWarning();
        }
    }
}
