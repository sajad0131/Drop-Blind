using UnityEngine;
using UnityEngine.UI;

public class NoiseUI : MonoBehaviour
{
    public Slider noiseSlider;

    private void Start()
    {
        if (noiseSlider == null)
        {
            noiseSlider = GameObject.FindGameObjectWithTag("NoiseSlider").GetComponent<Slider>();
        }
        if (NoiseManager.Instance != null)
        {
            NoiseManager.Instance.OnNoiseLevelChanged.AddListener(UpdateVisuals);
        }
    }

    private void Awake()
    {
        noiseSlider = GameObject.FindGameObjectWithTag("NoiseSlider").GetComponent<Slider>();
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
        if (noiseSlider == null)
        {
            noiseSlider = GameObject.FindGameObjectWithTag("NoiseSlider").GetComponent<Slider>();
        }

        noiseSlider.value = normalizedNoise;
        Color barColor = Color.Lerp(Color.cyan, Color.red, normalizedNoise);
        noiseSlider.fillRect.GetComponent<Image>().color = barColor;
    }
}