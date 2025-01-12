using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColourPickerButton : MonoBehaviour
{

    private Image buttonDisplay;
    private TMP_Text textDisplay;

    public ColourPicker colourPicker;

    private void Awake()
    {
        buttonDisplay = GetComponent<Image>();
        textDisplay = GetComponentInChildren<TMP_Text>();
    }

    public void SetColour(Color colour)
    {
        buttonDisplay.color = colour;
        Color.RGBToHSV(colour, out _, out float sat, out float val);
        if (val > 0.5 && sat < 0.5)
        {
            val = 0;
        }
        else
        {
            val = 1;
        }
        textDisplay.color = Color.HSVToRGB(0, 0, val);
    }

    public Color GetColour()
    {
        return buttonDisplay.color;
    }

    public void OnButtonClick()
    {
        colourPicker.gameObject.SetActive(true);
        colourPicker.colourPickerButton = this;
        colourPicker.SetColour(buttonDisplay.color);
    }
}
