using UnityEngine;
using UnityEngine.UI;

public class NoiseUI : MonoBehaviour
{
    [SerializeField] private Slider noiseSlider;

    private void Start()
    {
        // Subscribe to the event
        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.OnNoiseLevelChanged.AddListener(UpdateVisuals);
        }
    }

    private void UpdateVisuals(float normalizedNoise)
    {
        // Lerp for smooth UI effect (optional optimization)
        noiseSlider.value = normalizedNoise;

        // Visual feedback: Turn Red when high
        Color barColor = Color.Lerp(Color.cyan, Color.red, normalizedNoise);
        noiseSlider.fillRect.GetComponent<Image>().color = barColor;
    }
}