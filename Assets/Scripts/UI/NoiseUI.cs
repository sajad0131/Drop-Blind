using UnityEngine;
using UnityEngine.UI;

public class NoiseUI : MonoBehaviour
{
    [SerializeField] private Slider noiseSlider;

    private void Awake()
    {
        EnsureSliderReference();
    }

    private void Start()
    {
        EnsureSliderReference();

        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.OnNoiseLevelChanged.AddListener(UpdateVisuals);
            UpdateVisuals(NoiseManager.Instance.CurrentNoiseNormalized);
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
        EnsureSliderReference();
        if (noiseSlider == null) return;

        noiseSlider.value = normalizedNoise;

        if (noiseSlider.fillRect != null)
        {
            var fillImage = noiseSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                Color barColor = Color.Lerp(Color.cyan, Color.red, normalizedNoise);
                fillImage.color = barColor;
            }
        }
    }

    private void EnsureSliderReference()
    {
        if (noiseSlider != null) return;

        var sliderObj = GameObject.FindGameObjectWithTag("NoiseSlider");
        if (sliderObj != null)
        {
            noiseSlider = sliderObj.GetComponent<Slider>();
        }
    }
}
