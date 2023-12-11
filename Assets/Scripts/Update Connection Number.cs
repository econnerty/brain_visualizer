using UnityEngine;
using UnityEngine.UI; // For UI elements
using TMPro;
// If you're using TextMeshPro, also add:
// using TMPro;

public class SliderValueDisplay : MonoBehaviour
{
    public Slider numberConnectionsSlider; // Assign in Inspector
    public TextMeshProUGUI valueDisplayText;
    // If using TextMeshPro:
    // public TextMeshProUGUI valueDisplayText;

    void Start()
    {
        // Optionally set an initial value
        valueDisplayText.text = numberConnectionsSlider.value.ToString();
        
        // Add a listener to the slider
        numberConnectionsSlider.onValueChanged.AddListener(UpdateTextWithSliderValue);
    }

    void UpdateTextWithSliderValue(float value)
    {
        valueDisplayText.text = value.ToString();
    }
}
