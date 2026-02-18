using UnityEngine;

// MARKETING TIP: This "Endless Descent" feel is key for your trailers. 
// The smooth movement of the background vs the jagged movement of the monster creates contrast.

public class BackgroundScroller : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Reference to the Background Renderer (Quad/Plane)")]
    [SerializeField] private Renderer _renderer;

    [Tooltip("Base speed at which the texture scrolls. Should match your World Move Speed.")]
    [SerializeField] private float _scrollSpeed = 5.0f;

    [Tooltip("Multiplier to create depth (Parallax). < 1.0 means it looks further away.")]
    [SerializeField] private float _parallaxFactor = 0.1f;

    [Header("Texture Configuration")]
    [Tooltip("The property name in the shader. For URP Lit/Unlit, it is usually '_BaseMap' or '_MainTex'.")]
    [SerializeField] private string _texturePropertyName = "_BaseMap";

    // Internal state
    private MaterialPropertyBlock _propBlock;
    private float _currentOffsetY;
    private int _propertyID;

    private void Awake()
    {
        // Cache the Property ID for performance (Strings are slow in Update)
        _propertyID = Shader.PropertyToID(_texturePropertyName + "_ST");

        _propBlock = new MaterialPropertyBlock();

        if (_renderer == null)
        {
            _renderer = GetComponent<Renderer>();
        }
    }

    private void Update()
    {
        HandleTextureScrolling();
    }

    private void HandleTextureScrolling()
    {
        // Calculate the new offset
        // We move the offset POSITIVE Y to simulate the texture moving UP (falling down sensation)
        float moveDistance = _scrollSpeed * _parallaxFactor * Time.deltaTime;

        // Use Mathf.Repeat to keep the value between 0 and 1 to avoid huge numbers (Floating Point errors)
        _currentOffsetY = Mathf.Repeat(_currentOffsetY + moveDistance, 1.0f);
        Debug.Log("move distance: " + moveDistance);
        // Apply to the renderer using PropertyBlock
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetVector(_propertyID, new Vector4(1, 1, 0, _currentOffsetY)); // Z and W are tiling (1,1)
        _renderer.SetPropertyBlock(_propBlock);
    }

    // Call this from your GameManager or PlayerController when the falling speed changes
    public void SetSpeed(float newSpeed)
    {
        _scrollSpeed = newSpeed;
    }
}