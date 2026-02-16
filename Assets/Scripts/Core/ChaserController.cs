using UnityEngine;

public class ChaserController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTarget;

    [Header("Movement Settings")]
    [Tooltip("Y Position offset relative to player when Noise is 0% (Safe)")]
    [SerializeField] private float startOffset = 15f;

    [Tooltip("Y Position offset relative to player when Noise is 100% (Dead)" +
        "Usually 0 or slightly negative to overlap the player")]
    
    [SerializeField] private float killOffset = 0f;

    [Tooltip("How smoothly the monster moves towards the target position")]
    [SerializeField] private float smoothTime = 2f; // Slower is creepier

    [Header("Visuals (Optional)")]
    [SerializeField] private ParticleSystem presenceParticles;

    private float _currentNoiseNormalized = 0f;
    private float _velocity; // Reference for SmoothDamp

    private void Start()
    {
        // Auto-find player if not assigned
        if (playerTarget == null)
        {
            var player = FindFirstObjectByType<PlayerController>();
            if (player != null) playerTarget = player.transform;
        }

        // Subscribe to Noise Manager
        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.OnNoiseLevelChanged.AddListener(OnNoiseChanged);
        }

        // Initialize position off-screen
        UpdatePosition(true);
    }

    private void OnDestroy()
    {
        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.OnNoiseLevelChanged.RemoveListener(OnNoiseChanged);
        }
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        UpdatePosition(false);
        CheckKillCondition();
    }

    public void OnNoiseChanged(float normalizedNoise)
    {
        _currentNoiseNormalized = normalizedNoise;
    }

    private void UpdatePosition(bool snap)
    {
        if (playerTarget == null) return;

        // Calculate where the monster SHOULD be based on noise
        // Lerp between the Safe Offset and the Kill Offset
        float targetOffsetY = Mathf.Lerp(startOffset, killOffset, _currentNoiseNormalized);

        Vector3 targetPosition = playerTarget.position;
        targetPosition.y += targetOffsetY;
        // Keep the Z aligned with player or fixed background
        targetPosition.z = transform.position.z;
        // Optional: Match Player X with a delay (lag) to make it feel like it's hunting
        targetPosition.x = Mathf.Lerp(transform.position.x, playerTarget.position.x, Time.deltaTime * 1.0f);

        if (snap)
        {
            transform.position = targetPosition;
        }
        else
        {
            // Smoothly move on Y axis
            float newY = Mathf.SmoothDamp(transform.position.y, targetPosition.y, ref _velocity, smoothTime);
            transform.position = new Vector3(targetPosition.x, newY, targetPosition.z);
        }
    }

    private void CheckKillCondition()
    {
        // Simple distance check or rely on NoiseManager hitting 100%
        // Using distance is safer to ensure visual overlap
        if (transform.position.y <= playerTarget.position.y + killOffset + 0.5f)
        {
            // Double check noise is actually high to prevent accidental kills at start
            if (_currentNoiseNormalized >= 0.95f)
            {
                Attack();
            }
        }
    }

    private void Attack()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }

        // TODO: Play Jumpscare Animation/Sound here
        Debug.Log("CHASER CAUGHT PLAYER!");
    }
}