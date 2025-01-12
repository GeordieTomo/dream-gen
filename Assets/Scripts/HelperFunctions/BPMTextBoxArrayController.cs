using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BPMTextBoxArrayController : TextBoxArrayController
{

    private List<double> bpmArrayContents = new List<double>();

    private int complexity = 0;
    private int converge = 0;

    void Awake()
    {
        for (int i = 0; i < numberOfTextBoxes; i++)
        {
            InputFieldParser inputField = Instantiate(textBoxPrefab, transform);
            textBoxes.Add(inputField);
            int loopNumber = Mathf.RoundToInt(i / loopPoint);
            textBoxes.Last().GetComponent<RectTransform>().anchoredPosition = new Vector2((i % loopPoint) * horizontalSpacing, -loopNumber * verticalSpacing) + textBoxes.Last().GetComponent<RectTransform>().anchoredPosition;

            textBoxes.Last().AddValueChangedListener(BPMInputChanged);
            textBoxes.Last().isChord = chords;

            SetBPMArrayContents(bpmArrayContents, 100, 100);
        }
    }

    public void SetBPMArrayContents(List<double> arrayContents, int repeatPattern, int arrayLength)
    {
        SetBPMArrayContents(arrayContents, repeatPattern, arrayLength, converge, complexity);
    }

    public void SetBPMArrayContents(List<double> arrayContents, int repeatPattern, int arrayLength, int converge, int complexity)
    {
        this.complexity = complexity;
        this.converge = converge;
        currentlySettingValues = true;
        this.repeatPattern = repeatPattern;
        this.bpmArrayContents.Clear();

        if (converge >= 0 || complexity >= 0)
        {

            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (i < arrayContents.Count)
                {
                    if (i == 0)
                    {
                        this.bpmArrayContents.Add(arrayContents[i] + arrayContents[0] * (((float)converge / 1000f) * i + Mathf.Pow(1f + ((float)complexity) / 10000f, i) - 1));
                        textBoxes[i].gameObject.SetActive(true);
                        textBoxes[i].SetDoubleValue(this.bpmArrayContents[i]);
                        textBoxes[i].SetBoxEnabled(true);
                    }
                    else
                    {
                        //                   prev
                        double newBPM = bpmArrayContents[(i - 1)];
                        newBPM += arrayContents[0] * ((converge / 1000f) + Mathf.Pow(1f + complexity / 10000f, i) - 1);
                        this.bpmArrayContents.Add(newBPM);
                        textBoxes[i].gameObject.SetActive(true);
                        textBoxes[i].SetDoubleValue(this.bpmArrayContents[i]);
                        textBoxes[i].SetBoxEnabled(true);
                    }
                }
                else
                {
                    this.bpmArrayContents.Add(textBoxes[i].GetDoubleValue());
                    textBoxes[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            for (int i = 0; i < textBoxes.Count; i++)
            {
                if (i < arrayContents.Count)
                {
                   
                    this.bpmArrayContents.Add(arrayContents[i]);
                    textBoxes[i].gameObject.SetActive(true);
                    textBoxes[i].SetDoubleValue(this.bpmArrayContents[i]);
                    textBoxes[i].SetBoxEnabled(true);
                    
                }
                else
                {
                    this.bpmArrayContents.Add(textBoxes[i].GetDoubleValue());
                    textBoxes[i].gameObject.SetActive(false);
                }
            }
        }


        SetArrayLength(arrayLength);
        currentlySettingValues = false;

    }

    public void  BPMInputChanged(int repeatPattern, int arrayLength)
    {
        if (!currentlySettingValues)
        {
            GetBPMArrayContents();
            List<double> arrayContentsCopy = new List<double>();
            arrayContentsCopy.AddRange(new List<double>(bpmArrayContents));
            SetBPMArrayContents(arrayContentsCopy, repeatPattern, arrayLength);
        }
    }

    public void BPMInputChanged(string value)
    {
        BPMInputChanged(repeatPattern, displayedLength);
        OnValueChangedListener(value);
    }

    public void BPMInputChanged()
    {
        InputChanged(string.Empty);
    }

    public List<double> GetBPMArrayContents()
    {

        for (int i = 0; i < displayedLength; i++)
        {
            bpmArrayContents[i] = textBoxes[i].GetDoubleValue();
        }

        List<double> result = new List<double>();
        result.AddRange(new List<double>(bpmArrayContents.GetRange(0, displayedLength)));

        return result;
    }

    public double GetBPMArrayContents(int index)
    {
        return textBoxes[index].GetDoubleValue();
    }

}
