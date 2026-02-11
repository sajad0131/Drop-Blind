using UnityEngine;

public class WorldScroller : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("How fast the player 'falls' (World moves up)")]
    [SerializeField] private float fallSpeed = 10f;

    // Singleton pattern for easy access to speed (useful for difficulty scaling later)
    public static WorldScroller Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    private void Update()
    {
        // Move the entire world object UP on the Y axis
        // This simulates the player falling DOWN
        transform.Translate(Vector3.up * fallSpeed * Time.deltaTime);
    }

    public void SetSpeed(float newSpeed)
    {
        fallSpeed = newSpeed;
    }
}