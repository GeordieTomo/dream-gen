using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TextBoxArrayController : MonoBehaviour
{

    public bool BPMArrayController = false;

    public InputFieldParser textBoxPrefab;
    public int numberOfTextBoxes;
    public int loopPoint = 25;

    public float verticalSpacing = 40.5f;
    public float horizontalSpacing = 44f;

    private List<int> arrayContents = new List<int>();
    public List<InputFieldParser> textBoxes = new List<InputFieldParser>();

    public int displayedLength = 0;

    private bool firstTimeSetup = true;

    public bool currentlySettingValues = false;

    public int repeatPattern;
    private int pitchShift;
    private List<int> notePattern = new List<int>();

    public delegate void ValueChangedHandler(string newValue);
    public event ValueChangedHandler onValueChanged;

    public PendulumGenerator pendulumGen;

    public bool chords = false;

    private bool manual = false;

    void Awake()
    {
        for (int i = 0; i < numberOfTextBoxes; i++)
        {
            InputFieldParser inputField = Instantiate(textBoxPrefab, transform);
            textBoxes.Add(inputField);
            int loopNumber = Mathf.RoundToInt(i / loopPoint);
            textBoxes.Last().GetComponent<RectTransform>().anchoredPosition = new Vector2((i%loopPoint) * horizontalSpacing, - loopNumber * verticalSpacing) + textBoxes.Last().GetComponent<RectTransform>().anchoredPosition;

            textBoxes.Last().AddValueChangedListener(InputChanged);
            textBoxes.Last().isChord = chords;

            SetArrayContents(100, 7, 100, notePattern, false);
        }
    }

    public List<int> CalculatePattern(List<int> notePattern, int repeatPoint, int pitchShift, int arrayLength, bool manual)
    {
        this.manual = manual;
        List<int> output = new List<int>();

        if (chords)
        {
            output.AddRange(notePattern);
        }
        else if (arrayLength > repeatPoint && notePattern.Count > 0)
        {
            for (int i = 0; i < arrayLength; i++)
            {
                int pitchShiftAddition = (Mathf.RoundToInt((i / repeatPattern)) * (int)pitchShift) % 28;
                output.Add(notePattern[i % repeatPoint] + pitchShiftAddition);
            }
            if (!manual)
                output.Sort();
        }
        else
        {
            output.AddRange(notePattern);
        }
        return output;
    }

    public void SetArrayContents(int repeatPattern, int pitchShift, int arrayLength, List<int> notePattern, bool manual)
    {
        this.notePattern.Clear();
        this.notePattern.AddRange(notePattern);
        currentlySettingValues = true;
        this.repeatPattern = repeatPattern;
        this.pitchShift = pitchShift;
        arrayContents.Clear();


        List<int> realPattern = CalculatePattern(notePattern, repeatPattern, pitchShift, arrayLength, manual);


        for (int i = 0; i < textBoxes.Count; i++)
        {
            if (i < realPattern.Count)
            {
                arrayContents.Add(realPattern[i]);
                textBoxes[i].gameObject.SetActive(true);
                textBoxes[i].SetIntValue(realPattern[i]);
                textBoxes[i].SetBoxEnabled(true);
            }
            else
            {
                arrayContents.Add(textBoxes[i].GetIntValue());
                textBoxes[i].gameObject.SetActive(false);
            }
        }

        SetArrayLength(arrayLength);
        currentlySettingValues = false;

    }

    public void SetArrayLength(int length)
    {
        displayedLength = length;
        for (int i = 0; i < textBoxes.Count; i++)
        {
            if (i < length)
            {
                textBoxes[i].gameObject.SetActive(true);
            }
            else
            {
                textBoxes[i].gameObject.SetActive(false);
            }
        }
    }

    public int GetArrayLength()
    {
        return displayedLength;
    }

    public void InputChanged(int repeatPattern, int pitchShift, int arrayLength)
    {
        if (!currentlySettingValues)
        {
            GetArrayContents();
            List<int> arrayContentsCopy = new List<int>();
            arrayContentsCopy.AddRange(new List<int>(notePattern));
            SetArrayContents(repeatPattern, pitchShift, arrayLength, arrayContentsCopy, manual);
        }
    }
    public void InputChanged(string value)
    {
        InputChanged(repeatPattern, pitchShift, displayedLength);
        onValueChanged?.Invoke(value.ToString());
    }

    public void OnValueChangedListener(string value)
    {
        onValueChanged?.Invoke(value.ToString());
    }

    public void InputChanged()
    {
        InputChanged(string.Empty);
    }


    public void AddValueChangedListener(ValueChangedHandler listener)
    {
        onValueChanged += listener;
    }

    public List<int> GetArrayContents()
    {

        for (int i = 0; i < displayedLength; i++)
        {
            arrayContents[i] = textBoxes[i].GetIntValue();
        }

        List<int> result = new List<int>();
        result.AddRange(new List<int>(arrayContents.GetRange(0, displayedLength)));

        return result;
    }

    public int GetArrayContents(int index)
    {
        return textBoxes[index].GetIntValue();
    }

    public int GetNotePattern(int index)
    {
        return arrayContents[index];
    }

    public bool FirstTimeSetup ()
    {
        if (firstTimeSetup)
        {
            firstTimeSetup = false;
            return true;
        }

        return false;
    }

}
