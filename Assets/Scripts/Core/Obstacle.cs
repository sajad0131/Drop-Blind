using UnityEngine;
using System.Collections;
public class Obstacle : MonoBehaviour
{
    private System.Action<Obstacle> returnToPoolAction;
    private float destructionY; // Y position where the obstacle is "safe" to remove (above player)

    [Header("Visual Settings")]
    [SerializeField] private Renderer _renderer;
    [SerializeField] private float revealDuration = 0.5f;
    [SerializeField] private float fadeSpeed = 0.2f;    // Time to reveal (1 -> 0)
    [SerializeField] private float visibleTime = 1.0f;  // Time to stay visible
    [SerializeField] private float hideSpeed = 0.5f;

    // Shader Property ID (Micro-optimization for mobile)
    private static readonly int DissolvePropertyID = Shader.PropertyToID("_Dissolve");

    private MaterialPropertyBlock _propBlock;
    private Coroutine _revealCoroutine;



    public void Initialize(System.Action<Obstacle> returnAction, float topYThreshold)
    {
        returnToPoolAction = returnAction;
        destructionY = topYThreshold;
    }

    private void Update()
    {
        // Check if we have moved past the player and gone off-screen (Top)
        // Since the World moves UP, the obstacle's World Position Y increases.
        if (transform.position.y > destructionY)
        {
            returnToPoolAction?.Invoke(this);
        }
    }


    private void Awake()
    {
        if (_renderer == null) _renderer = GetComponent<Renderer>();

        _propBlock = new MaterialPropertyBlock();

        // Ensure obstacle starts invisible (Dissolve = 1)
        SetDissolveValue(1f);
    }

    public void Reveal()
    {
        // Restart timer if already revealed
        if (_revealCoroutine != null) StopCoroutine(_revealCoroutine);
        _revealCoroutine = StartCoroutine(RevealRoutine());
        Debug.Log("revealed");
    }

    private IEnumerator RevealRoutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeSpeed)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(1f, 0f, elapsedTime / fadeSpeed);
            SetDissolveValue(currentValue);
            yield return null;
        }
        SetDissolveValue(0f); // Ensure it's fully visible

        // 2. Wait
        yield return new WaitForSeconds(visibleTime);

        // 3. Smooth Hide (Dissolve 0 -> 1)
        elapsedTime = 0f;
        while (elapsedTime < hideSpeed)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(0f, 1f, elapsedTime / hideSpeed);
            SetDissolveValue(currentValue);
            yield return null;
        }
        SetDissolveValue(1f); // Ensure it's fully invisible
    }

    private void SetDissolveValue(float value)
    {
        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(DissolvePropertyID, value);
        _renderer.SetPropertyBlock(_propBlock);
    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("GAME OVER! Hit Obstacle.");
            GameManager.Instance.TriggerGameOver();
            // TODO: Trigger Game Over Event
        }
    }
}