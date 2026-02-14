using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float sensitivity = 0.01f;
    [SerializeField] private float xClamp = 4.5f; // Limit how far left/right player can go
    [SerializeField] private float smoothness = 20f; // For smoothing the input

    [Header("References")]
    [SerializeField] private SonarManager sonarManager;

    private Vector2 _moveInput;
    private float _targetX;


    // specific reference to your uploaded Input Action Asset
    private InputAction _moveAction;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();

        // Setup initial target to current position
        _targetX = transform.position.x;
    }

    private void OnEnable()
    {
        // We look for the "Look" action because your Input file binds <Pointer>/delta to it.
        // Usually "Move" is for Joysticks (Value), "Look" is for Delta.
        _moveAction = _playerInput.actions["Look"];
    }

    private void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        // 1. Read the Delta (How much the finger/mouse moved this frame)
        // We only care about X axis for this game
        float deltaX = 0f;

        // Check if we are touching/clicking
        // Note: For a "Drop" game, usually ANY touch counts as control
        if (Mouse.current.leftButton.isPressed || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed))
        {
            Vector2 delta = _moveAction.ReadValue<Vector2>();
            deltaX = delta.x;
        }

        // 2. Apply sensitivity and calculate target
        // We assume 60FPS reference for sensitivity, hence Time.deltaTime isn't strictly multiplied 
        // by delta here if using the new Input System's raw delta, but for consistency:
        _targetX += deltaX * sensitivity;

        // 3. Clamp the position so player doesn't fly off screen
        _targetX = Mathf.Clamp(_targetX, -xClamp, xClamp);

        // 4. Smoothly move the visual player to the target X
        Vector3 newPos = transform.position;
        newPos.x = Mathf.Lerp(newPos.x, _targetX, Time.deltaTime * smoothness);

        transform.position = newPos;
    }

    public void OnTap()
    {

        sonarManager.TriggerSonar(transform.position);
    }


}