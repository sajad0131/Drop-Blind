using UnityEngine;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "DropBlind/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Difficulty Curves")]
    [Tooltip("X-axis is time/distance, Y-axis is the speed multiplier")]
    public AnimationCurve speedOverTime = AnimationCurve.Linear(0, 1, 300, 3); // Scales from 1x to 3x over 5 mins

    [Tooltip("X-axis is time/distance, Y-axis is the noise multiplier")]
    public AnimationCurve noiseScalingOverTime = AnimationCurve.Linear(0, 1, 300, 2);

    [Header("Spawning")]
    public float baseSpawnInterval = 1.5f;
    public float spawnDistanceY = 20f;

    [Header("Base Settings")]
    public float baseGlobalSpeed = 10f;
}