using UnityEngine;
using UnityEngine.UI; // For UI elements
using TMPro;

public class UIHandler : MonoBehaviour
{
    public BrainConnectivityVisualizer brainVisualizer; // Assign in Inspector
    public Slider connectionsSlider; // Assign in Inspector
    public TMP_Dropdown dropdown;

    void Start()
    {
        // Add a listener to the slider to call a method whenever its value changes
        connectionsSlider.onValueChanged.AddListener(HandleSliderValueChanged);
        dropdown.onValueChanged.AddListener(HandleDropdownValueChanged);
    }

    private void HandleSliderValueChanged(float value)
    {
        // Call the UpdateVisualization method with the new slider value
        brainVisualizer.UpdateVisualization((int)value);
    }

    private void HandleDropdownValueChanged(int value)
    {
       //the dropdown values correspond to different CSV file paths
        string selectedFilePath = GetFilePathFromDropdownIndex(value);
        bool isDirected = value > 3;
        brainVisualizer.UpdateVisualization(selectedFilePath, isDirected);
    }
    string GetFilePathFromDropdownIndex(int index)
    {
        // Map the dropdown index to a specific file path
        // Example:
        switch (index)
        {
            case 0: return "coh_ec_normalized";
            case 1: return "coh_eo_normalized";
            case 2: return "ciplv_eo_normalized";
            case 3: return "ciplv_ec_normalized";
            case 4: return "ddtf_eo_normalized";
            case 5: return "ddtf_ec_normalized";
            case 6: return "gpdc_eo_normalized";
            case 7: return "gpdc_ec_normalized";
            case 8: return "psgp_eo_normalized";
            case 9: return "psgp_ec_normalized";
            default: return "coh_ec_normalized";
        }
    }
}
