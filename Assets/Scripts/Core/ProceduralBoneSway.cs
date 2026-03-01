using UnityEngine;

public class ProceduralArmSway : MonoBehaviour
{
    [Header("Left Arm Settings")]
    [SerializeField] private Transform leftArm;
    [Tooltip("The local axis to bend around. Usually Z (0,0,1) or Y (0,1,0).")]
    [SerializeField] private Vector3 leftRotationAxis = new Vector3(0, 0, 1);
    [Tooltip("Speed multiplier. Negative values usually make it lag behind movement.")]
    [SerializeField] private float leftSwayMultiplier = -15f;

    [Header("Right Arm Settings")]
    [SerializeField] private Transform rightArm;
    [Tooltip("The local axis to bend around. Usually Z (0,0,1) or Y (0,1,0).")]
    [SerializeField] private Vector3 rightRotationAxis = new Vector3(0, 0, 1);
    [Tooltip("Speed multiplier. If bones are mirrored, you might need to flip the sign (e.g., 15f instead of -15f).")]
    [SerializeField] private float rightSwayMultiplier = 15f;

    [Header("Global Sway Dynamics")]
    [Tooltip("Maximum angle the arms can bend to prevent them from breaking into the torso.")]
    [SerializeField] private float maxAngle = 45f;
    [Tooltip("How fast the arms snap back to their default pose.")]
    [SerializeField] private float smoothness = 12f;

    private Vector3 _previousWorldPosition;
    private float _currentLeftAngle = 0f;
    private float _currentRightAngle = 0f;

    private void Start()
    {
        _previousWorldPosition = transform.position;
    }

    private void LateUpdate()
    {
        if (leftArm == null && rightArm == null) return;

        // 1. Calculate horizontal velocity purely from position changes
        Vector3 velocity = (transform.position - _previousWorldPosition) / Time.deltaTime;
        _previousWorldPosition = transform.position;

        // 2. Process LEFT Arm
        if (leftArm != null)
        {
            float targetLeft = Mathf.Clamp(velocity.x * leftSwayMultiplier, -maxAngle, maxAngle);
            _currentLeftAngle = Mathf.Lerp(_currentLeftAngle, targetLeft, Time.deltaTime * smoothness);
            leftArm.localRotation *= Quaternion.AngleAxis(_currentLeftAngle, leftRotationAxis);
        }

        // 3. Process RIGHT Arm
        if (rightArm != null)
        {
            // By separating this, you can make the right arm react entirely differently if needed
            float targetRight = Mathf.Clamp(velocity.x * rightSwayMultiplier, -maxAngle, maxAngle);
            _currentRightAngle = Mathf.Lerp(_currentRightAngle, targetRight, Time.deltaTime * smoothness);
            rightArm.localRotation *= Quaternion.AngleAxis(_currentRightAngle, rightRotationAxis);
        }
    }
}