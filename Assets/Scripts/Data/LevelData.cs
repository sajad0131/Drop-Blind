using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "DropBlind/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Spawning")]
    [Tooltip("Time in seconds between obstacle spawns")]
    public float spawnInterval = 1.5f;

    [Tooltip("How far below the player obstacles spawn (Y-axis)")]
    public float spawnDistanceY = 20f;

    [Header("Difficulty")]
    [Tooltip("Speed multiplier for the WorldScroller")]
    public float globalSpeed = 10f;
}