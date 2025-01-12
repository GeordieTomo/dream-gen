using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;

public class InputFieldParser : MonoBehaviour
{

    TMP_InputField inputField;

    public bool integerInput = false;
    public bool floatInput = true;
    public bool doubleInput = false;

    public bool limitRange = true;
    public float minInput = 1.0f;
    public float maxInput = 4.0f;

    private double value;
    private string strVal;

    public TMP_Text outScaleText;

    public PendulumGenerator pendulumGen;

    public bool isChord = false;

    public delegate void ValueChangedHandler(string newValue);
    public event ValueChangedHandler onValueChanged;

    bool isCommaUsed;

    private void Awake()
    {
        pendulumGen = FindObjectOfType<PendulumGenerator>();

        inputField = GetComponent<TMP_InputField>();

        inputField.onValueChanged.AddListener(InputChanged);
        inputField.onEndEdit.AddListener(InputComplete);

        InputChanged(inputField.text);
        InputComplete(inputField.text);



        CultureInfo cultureInfo = CultureInfo.CurrentCulture;
        isCommaUsed = cultureInfo.NumberFormat.NumberDecimalSeparator == ",";
    }

    private void InputChanged(string text)
    {
        if (integerInput)
        {
            string numericText = new string(text.Where(char.IsDigit).ToArray());

            // Convert the cleaned numeric text to an integer and store it (or use it as needed).
            if (int.TryParse(numericText, out int result))
            {
                if (limitRange)
                {
                    value = Mathf.Clamp(result, minInput, maxInput);
                }
                else
                {
                    value = result;
                }

                inputField.text = value.ToString();
            }
            else if (numericText == "")
            {
                value = minInput;
            }
            else
            {
                Debug.LogWarning("Input cannot be parsed as an integer.");
            }
        }
        else if (floatInput || doubleInput)
        {
            // Replace commas with dots for proper float parsing
            if (isCommaUsed)
            {
                text = text.Replace('.', ',');
            }
            else
            {
                text = text.Replace(',', '.');
            }

            string numericText = new string(text.Where(c => char.IsDigit(c) || c == '.' || c == ',' || c == '-').ToArray());

            // Convert the cleaned numeric text to an integer and store it (or use it as needed).
            if (double.TryParse(numericText.ToArray(), out double result))
            {
                if (limitRange)
                {
                    value = System.Math.Clamp(result, minInput, maxInput);
                }
                else
                {
                    value = result;
                }

                inputField.text = numericText;
            }
            else if (numericText == "")
            {
                value = minInput;
            }
            else
            {
                Debug.LogWarning("Input cannot be parsed as an float.");
            }
        }
        else
        {
            strVal = text; 
        }
        
        //onValueChanged?.Invoke(value.ToString());
        if (outScaleText != null)
        {
            outScaleText.text = NoteName(GetIntValue());
        }
    }
    private void InputComplete(string text)
    {
        if (floatInput || doubleInput)
        {
            // Replace commas with dots for proper float parsing
            if (isCommaUsed)
            {
                text = text.Replace('.', ',');
            }
            else
            {
                text = text.Replace(',', '.');
            }

            string numericText = new string(text.Where(c => char.IsDigit(c) || c == '.' || c == ',' || c == '-').ToArray());

            // Convert the cleaned numeric text to an integer and store it (or use it as needed).
            if (double.TryParse(numericText, out double result))
            {
                if (limitRange)
                {
                    value = System.Math.Clamp(result, minInput, maxInput);
                }
                else
                {
                    value = result;
                }

                inputField.text = value.ToString();
            }
            else if (numericText == "")
            {
                value = minInput;
            }
            else
            {
                Debug.LogWarning("Input cannot be parsed as an float.");
            }
        }
        onValueChanged?.Invoke(value.ToString());
    }

    public void SetBoxEnabled(bool enabled)
    {
        GetComponent<TMP_InputField>().interactable = enabled;
    }


    public void AddValueChangedListener(ValueChangedHandler listener)
    {
        onValueChanged += listener;
    }


    public float GetFloatValue()
    {
        return System.Convert.ToSingle(value);
    }

    public void SetFloatValue(float newValue)
    {
        if (limitRange)
            value = Mathf.Clamp(newValue, minInput, maxInput);
        else
            value = newValue;

        inputField.text = value.ToString();

    }

    public string GetStringValue()
    {
        return strVal;
    }

    public double GetDoubleValue()
    {
        return value;
    }

    public void SetDoubleValue(double newValue)
    {
        if (limitRange)
            value = System.Math.Clamp(newValue, minInput, maxInput);
        else
            value = newValue;

        inputField.text = value.ToString();
    }

    public int GetIntValue()
    {
        return (int)value;
    }

    public void SetIntValue(int newValue)
    {
        if (limitRange)
            value = Mathf.Clamp(newValue, minInput, maxInput);
        else
            value = newValue;
        
        inputField.text = value.ToString();

        if (outScaleText != null)
        {
            outScaleText.text = NoteName(GetIntValue());
        }

    }

    public string NoteName(int value)
    {
        if (!isChord)
            return pendulumGen.scalePickerMenu.NoteName(value, pendulumGen.noteOctaveInputField.GetIntValue());
        else
            return pendulumGen.scalePickerMenu.NoteName(value, pendulumGen.bassOctaveInputField.GetIntValue());
    }

}
