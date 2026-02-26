using UnityEngine;

public class ProceduralBoneSway : MonoBehaviour
{
    [Header("Rig References")]
    [Tooltip("Drag the bones you want to sway here (e.g., Spine, Chest, UpperArms).")]
    [SerializeField] private Transform[] swayBones;

    [Header("Sway Settings")]
    [Tooltip("How drastically the bones rotate based on horizontal speed. Negative values lag behind.")]
    [SerializeField] private float rotationMultiplier = -12f;
    [Tooltip("The maximum angle the bones can bend, preventing broken models.")]
    [SerializeField] private float maxRotationAngle = 35f;
    [Tooltip("How quickly the bones snap back to their default animation pose.")]
    [SerializeField] private float smoothness = 12f;

    [Header("Axis Setup")]
    [Tooltip("The local axis to rotate around. Usually Z (0,0,1) for leaning left/right, but depends on your 3D software export.")]
    [SerializeField] private Vector3 rotationAxis = new Vector3(0, 0, 1);

    private Vector3 _previousWorldPosition;
    private float _currentSwayAngle = 0f;

    private void Start()
    {
        _previousWorldPosition = transform.position;
    }

    // We MUST use LateUpdate so we rotate the bones AFTER the Animator sets their initial frame pose.
    private void LateUpdate()
    {
        if (swayBones == null || swayBones.Length == 0) return;

        // 1. Calculate horizontal velocity
        Vector3 velocity = (transform.position - _previousWorldPosition) / Time.deltaTime;
        _previousWorldPosition = transform.position;

        // 2. Calculate the target lean angle based on movement speed
        float targetAngle = Mathf.Clamp(velocity.x * rotationMultiplier, -maxRotationAngle, maxRotationAngle);

        // 3. Smoothly lerp the current angle to the target angle
        _currentSwayAngle = Mathf.Lerp(_currentSwayAngle, targetAngle, Time.deltaTime * smoothness);

        // 4. Create the quaternion rotation
        Quaternion swayRotation = Quaternion.AngleAxis(_currentSwayAngle, rotationAxis);

        // 5. Apply the rotation offset to each bone in the array
        foreach (Transform bone in swayBones)
        {
            if (bone != null)
            {
                // Multiply the current local rotation (from the Animator) by our procedural sway
                bone.localRotation = bone.localRotation * swayRotation;
            }
        }
    }
}