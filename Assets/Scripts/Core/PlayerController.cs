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

    [Header("Animation Settings")]
    [Tooltip("How fast the animation blends from falling to leaning left/right.")]
    [SerializeField] private float animationThreshold = 0.05f;

    // Component References
    private Animator _animator;

    // State Variables
    private float _currentHorizontalInput = 0f;
    private float _currentAnimBlend = 0f;
    private bool isDragging;

    // FIX: Track the actual swipe distance per frame
    private float _currentDeltaX = 0f;

    // MOBILE OPTIMIZATION: Animator Hashes for faster lookups (strings create garbage on mobile)
    private readonly int _horizontalSpeedHash = Animator.StringToHash("HorizontalSpeed");
    private readonly int _movingLeftHash = Animator.StringToHash("MovingLeft");
    private readonly int _movingRightHash = Animator.StringToHash("MovingRight");


    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _mainCamera = Camera.main;
        _animator = GetComponentInChildren<Animator>();
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
        HandleAnimation();
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
        _currentDeltaX = 0f; // Reset delta every frame

        if (Mouse.current.leftButton.isPressed || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed))
        {
            Vector2 delta = _moveAction.ReadValue<Vector2>();
            _currentDeltaX = delta.x; // Store the actual input delta for the animator
            isDragging = true;
        }
        else
        {
            isDragging = false;
        }

        _targetX += _currentDeltaX * sensitivity;

        // UPDATED: Use the dynamic clamp
        _targetX = Mathf.Clamp(_targetX, -_dynamicXClamp, _dynamicXClamp);

        Vector3 newPos = transform.position;
        newPos.x = Mathf.Lerp(newPos.x, _targetX, Time.deltaTime * smoothness);

        transform.position = newPos;
    }

    public void OnTap()
    {
        isDragging = false;
        if (SonarManager.Instance != null)
        {
            SonarManager.Instance.TriggerSonar(transform.position);
        }

        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.AddNoise(noisePerTap);
        }

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySonarPing();
        }
    }

    private void HandleAnimation()
    {
        // FIX: Now we evaluate _currentDeltaX (finger movement) instead of _targetX (world position)
        if (isDragging && _currentDeltaX < -animationThreshold)
        {
            // Moving Left
            _animator.SetBool(_movingLeftHash, true);
            _animator.SetBool(_movingRightHash, false);
        }
        else if (isDragging && _currentDeltaX > animationThreshold)
        {
            // Moving Right
            _animator.SetBool(_movingRightHash, true);
            _animator.SetBool(_movingLeftHash, false);
        }
        else
        {
            // Stopped moving horizontally OR lifted finger -> return to "falling" (idle) state
            _animator.SetBool(_movingLeftHash, false);
            _animator.SetBool(_movingRightHash, false);
        }
    }
}