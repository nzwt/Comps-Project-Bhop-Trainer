using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderNumberSync : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField inputField;

    void Start()
    {
        // Initialize the Input Field with the Slider's value
        inputField.text = slider.value.ToString();

        // Add listeners for changes
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        inputField.onEndEdit.AddListener(OnInputFieldValueChanged);
    }

    void OnSliderValueChanged(float value)
    {
        // Update the Input Field when the Slider value changes
        inputField.text = value.ToString();
    }

    void OnInputFieldValueChanged(string input)
    {
        if (float.TryParse(input, out float value))
        {
            // Update the Slider when the Input Field value changes
            slider.value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
        }
    }
}
