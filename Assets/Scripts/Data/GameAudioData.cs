using UnityEngine;

[CreateAssetMenu(fileName = "GameAudioData", menuName = "DropBlind/Audio Data")]
public class GameAudioData : ScriptableObject
{
    [Header("Player")]
    public AudioClip sonarPing;
    [Range(0.8f, 1.2f)] public float pingPitchVariance = 0.1f;

    public AudioClip footstep;
    public AudioClip deathImpact;

    [Header("Ambience & Music")]
    [Tooltip("Background drone/music")]
    public AudioClip backgroundAmbience;
    [Range(0f, 1f)] public float musicVolume = 0.5f;

    [Header("Threats")]
    public AudioClip chaserGrowl;
    public AudioClip highNoiseWarning; // Played when noise is critical
    public AudioClip obstacleHit;
}