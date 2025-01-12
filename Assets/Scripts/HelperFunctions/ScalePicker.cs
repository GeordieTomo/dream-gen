using Melanchall.DryWetMidi.MusicTheory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScalePicker : MonoBehaviour
{

    public List<Toggle> keyToggles;

    bool[] scaleSelection = new bool[12];
    int scaleLength = 12;

    public delegate void ValueChangedHandler();
    public event ValueChangedHandler OnValueChanged;

    public PendulumGenerator pendulumGen;

    public string[] noteNames = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

    public void CompleteSelection()
    {
        scaleLength = 0;
        for (int i = 0; i < scaleSelection.Length; i++)
        {
            scaleSelection[i] = keyToggles[i].isOn;
            if (scaleSelection[i]) scaleLength++;
        }

        if (pendulumGen == null)
        {
            pendulumGen = FindObjectOfType<PendulumGenerator>();
        }

        pendulumGen.RecalculateNotePitchNames();

        OnValueChanged?.Invoke();
        gameObject.SetActive(false);

    }

    public void SetScale(bool[] keyValues) 
    { 
        keyValues.CopyTo(scaleSelection, 0);
        scaleLength = 0;

        for (int i = 0; i < scaleSelection.Length; i++) 
        {
            keyToggles[i].isOn = scaleSelection[i];
            if (scaleSelection[i]) scaleLength++; 
        }
    }

    public bool[] GetScale() { return scaleSelection; }

    public float ScaleDegreeToFrequency(int scaleDegree, int rootNote, int octave)
    {
        int semitone = ScaleDegreeToSemitone(scaleDegree, rootNote, octave);
        return FrequencyFromSemitones(semitone);
    }

    // converts scale degree to semitone
    public (int note, int octave) ScaleDegreeToSemitone(int scaleDegree, int rootNote, int octave, bool returnOctave)
    {

        int[] scaleSemitones = new int[scaleLength];
        for (int i = 0,j = 0; i < 12; i++)
        {
            if (scaleSelection[i] == true)
            {
                scaleSemitones[j] = i + 1;
                j++;
            }
        }

        int index = scaleDegree + rootNote - 2;
        octave = octave + Mathf.FloorToInt(index / scaleLength);

        int note = octave * 12 + scaleSemitones[(index + scaleLength) % scaleLength];
        return (note, octave);
    }

    public int ScaleDegreeToSemitone(int scaleDegree, int rootNote, int octave)
    {
        (int tmp, _) = ScaleDegreeToSemitone(scaleDegree, rootNote, octave, true);
        return tmp;
    }

    // converts semitone degree to pitch
    public float FrequencyFromSemitones(int note)
    {
        // Freq = 1 | C3
        // 0.5 C2 0.25 C1 0.125 C0
        float frequency = Mathf.Pow(2f, (note - 1) / 12f) * 0.25f; // 0 = 1
        return frequency;
    }

    public string NoteName(int scaleDegree, int octave)
    {
        (int tmp, int octresult) = ScaleDegreeToSemitone(scaleDegree, 1, octave, true);
        int index = (tmp - 1) % (noteNames.Length);
        string str = "";
        while (index < 0)
            index += noteNames.Length;

        str = noteNames[index];
        str += (octresult).ToString();
        return str;
    }

}
