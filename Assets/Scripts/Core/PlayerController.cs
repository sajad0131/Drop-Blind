using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float sensitivity = 0.01f;
    [SerializeField] private float smoothness = 20f; // For smoothing the input

    // REPLACED: Fixed xClamp with dynamic calculation
    [Header("Screen Bounds")]
    [Tooltip("Distance from edge of screen to keep player inside.")]
    [SerializeField] private float horizontalPadding = 0.5f;
    private float _dynamicXClamp;

    [Header("References")]
    [SerializeField] private SonarManager sonarManager;

    [Header("Noise Settings")]
    [SerializeField] private float noisePerTap = 25f;

    private Vector2 _moveInput;
    private float _targetX;
    private Camera _mainCamera;

    // specific reference to your uploaded Input Action Asset
    private InputAction _moveAction;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main;

        // Setup initial target to current position
        _targetX = transform.position.x;

        // Calculate bounds immediately
        RecalculateBounds();
    }

    private void OnEnable()
    {
        _moveAction = _playerInput.actions["Look"];
    }

    private void Update()
    {
        // Optional: If testing on WebGL where window resizes, uncomment this:
        // RecalculateBounds(); 

        HandleMovement();
    }

    private void RecalculateBounds()
    {
        // 1. Get distance from camera to player
        float distanceToCamera = Mathf.Abs(_mainCamera.transform.position.z - transform.position.z);

        // 2. Find the world position of the right edge of the screen (Viewport 1.0)
        // Viewport Coordinates: (0,0) is bottom-left, (1,1) is top-right
        Vector3 rightEdge = _mainCamera.ViewportToWorldPoint(new Vector3(1, 0.5f, distanceToCamera));

        // 3. Set clamp based on that edge minus padding
        _dynamicXClamp = rightEdge.x - horizontalPadding;
    }

    private void HandleMovement()
    {
        float deltaX = 0f;

        if (Mouse.current.leftButton.isPressed || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed))
        {
            Vector2 delta = _moveAction.ReadValue<Vector2>();
            deltaX = delta.x;
        }

        _targetX += deltaX * sensitivity;

        // UPDATED: Use the dynamic clamp
        _targetX = Mathf.Clamp(_targetX, -_dynamicXClamp, _dynamicXClamp);

        Vector3 newPos = transform.position;
        newPos.x = Mathf.Lerp(newPos.x, _targetX, Time.deltaTime * smoothness);

        transform.position = newPos;
    }

    public void OnTap()
    {
        sonarManager.TriggerSonar(transform.position);

        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.AddNoise(noisePerTap);
        }
    }
}