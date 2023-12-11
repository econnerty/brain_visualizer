using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChangeDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI selectedText; // The text component where you want to show the selected option

    void Start()
    {
        // Add listener for when the value of the Dropdown changes
        dropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(dropdown);
        });
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        // Update the text to reflect the current selection
        selectedText.text = change.options[change.value].text;
    }
}
