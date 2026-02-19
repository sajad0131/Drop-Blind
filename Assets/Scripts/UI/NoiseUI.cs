using UnityEngine;
using UnityEngine.UI;

public class NoiseUI : MonoBehaviour
{
    [SerializeField] private Slider noiseSlider;

    private void Start()
    {
        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.OnNoiseLevelChanged.AddListener(UpdateVisuals);
        }
    }

    // THIS IS THE CRITICAL FIX FOR YOUR UI ERROR
    private void OnDestroy()
    {
        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.OnNoiseLevelChanged.RemoveListener(UpdateVisuals);
        }
    }

    private void UpdateVisuals(float normalizedNoise)
    {
        if (noiseSlider == null) return;

        noiseSlider.value = normalizedNoise;
        Color barColor = Color.Lerp(Color.cyan, Color.red, normalizedNoise);
        noiseSlider.fillRect.GetComponent<Image>().color = barColor;
    }
}