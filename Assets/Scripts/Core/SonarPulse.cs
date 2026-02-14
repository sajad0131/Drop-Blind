using UnityEngine;
using System.Collections;

public class SonarPulse : MonoBehaviour
{
    [Header("Pulse Settings")]
    [SerializeField] private float expansionSpeed = 10f;
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private float fadeDuration = 0.5f;


    [Header("Visuals")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private string alphaProperty = "_Alpha"; // Check your Shader Graph for the exact property name!

    private Material materialInstance;
    private float currentRadius;
    private Coroutine pulseRoutine;


    private System.Action<SonarPulse> returnToPoolAction;

    public void Initialize(System.Action<SonarPulse> returnAction)
    {
        returnToPoolAction = returnAction;
    }

    private void Awake()
    {
        // Cache the material to modify properties efficiently
        if (meshRenderer != null)
        {
            materialInstance = meshRenderer.material;
            
        }
    }

    public void StartPulse(Vector3 origin)
    {
        transform.position = origin;
        transform.localScale = Vector3.zero;
        currentRadius = 0f;

        // Reset Alpha/Opacity
        if (materialInstance != null) materialInstance.SetFloat(alphaProperty, 1f);

        gameObject.SetActive(true);

        if (pulseRoutine != null) StopCoroutine(pulseRoutine);
        pulseRoutine = StartCoroutine(PulseRoutine());
    }

    private IEnumerator PulseRoutine()
    {
        // 1. Expand
        while (currentRadius < maxRange)
        {
            currentRadius += expansionSpeed * Time.deltaTime;
            transform.localScale = Vector3.one * currentRadius * 2f; // Scale is diameter
            yield return null;
        }

        // 2. Optional: Fade out at max range before disappearing
        float elapsed = 0f;
        float startAlpha = materialInstance != null ? materialInstance.GetFloat(alphaProperty) : 1f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            if (materialInstance != null)
            {
                float newAlpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeDuration);
                materialInstance.SetFloat(alphaProperty, newAlpha);
            }
            yield return null;
        }

        // 3. Return to Pool
        //gameObject.SetActive(false);
    }
}