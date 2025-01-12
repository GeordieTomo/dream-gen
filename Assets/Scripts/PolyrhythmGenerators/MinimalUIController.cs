using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MinimalUIController : MonoBehaviour
{

    public static MinimalUIController instance;


    [SerializeField] public Slider numberOfNotesSlider;
    [SerializeField] public Slider noteComplexitySlider;
    [SerializeField] public Slider rhythmSpeedSlider;
    [SerializeField] public Slider rhythmDivergeSlider;
    [SerializeField] public Slider rhythmConvergeSlider;
    [SerializeField] public Slider numberOfChordsSlider;
    [SerializeField] public Slider chordsSpeedSlider;
    [SerializeField] public Slider chordComplexitySlider;
    [SerializeField] private Slider bloomSlider;
    [SerializeField] private Slider particlesSlider;
    [SerializeField] private Slider reverbSlider;
    [SerializeField] private Slider pitchSlider;
    [SerializeField] private Slider filterSlider;


    [SerializeField] private Image notePatternImage;
    [SerializeField] private Image divergeSliderImage;
    [SerializeField] private Image convergeSliderImage;
    [SerializeField] private Image bpmSliderImage;

    [SerializeField] private Image chordSliderImage;


    [SerializeField] private TMP_Dropdown sampleDropdown;
    [SerializeField] private TMP_Dropdown motionTypeDropdown;
    [SerializeField] private TMP_Dropdown shapeTypeDropdown;

    [SerializeField] private Toggle reboundToggleButton;

    [SerializeField] private ColourPickerButton pendulumColourButton;
    [SerializeField] private ColourPickerButton bassColourButton;
    [SerializeField] private ColourPickerButton backgroundColourButton;

    [SerializeField] private PendulumGenerator pendulumGenerator;


    private bool customPitchNotes = true;
    private bool customBPMNotes = true;
    private bool customChordProgression = true;

    [SerializeField] ArrayEditController noteEditBox;
    [SerializeField] ArrayEditController bpmEditBox;
    [SerializeField] ArrayEditController chordEditBox;

    public bool convergeMode = true;


    private int[][] progressions =
    {
        new int[] {1,5,1,5,1,5,1,5},
        new int[] {1,4,1,4,1,4,1,4},
        new int[] {1,6,5,4,1,6,5,4},
        new int[] {1,4,5,4,1,4,5,4},
        new int[] {6, 6, 6, 2, 6, 6, 6, 2},
        new int[] {6,5,4,5,6,5,4,5},
        new int[] {2,5,1,2,5,1,2,5},
        new int[] {6,2,5,1,6,2,5,1},
        new int[] { 7, 4, 1, 7, 4, 1, 7, 4}
    };

    private int[][] noteProgressions =
    {
        new int[] { 1, 3, 5},
        new int[] {1,4,5},
        new int[] {5,5,3,3,1,1},
        new int[] {1,3,5,7},
        new int[] {2,3,5,7},
        new int[] {7,7,5,5,3,3},
        new int[] {1,1,1,1,3,3,3,3,5,5,5,5,7,7,7,7},
        new int[] {1,3,5,7,9},
        new int[] { 3, 5, 7, 9, 11}
    };


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

        noteComplexitySlider.onValueChanged.AddListener(NotesChanged);
        numberOfNotesSlider.onValueChanged.AddListener(NotesChanged);

        chordComplexitySlider.onValueChanged.AddListener(ChordsChanged);
        numberOfChordsSlider.onValueChanged.AddListener(ChordsChanged);

        rhythmDivergeSlider.onValueChanged.AddListener(RhythmChanged);
        rhythmConvergeSlider.onValueChanged.AddListener(RhythmChanged);
        rhythmSpeedSlider.onValueChanged.AddListener(RhythmChanged);

        rhythmDivergeSlider.onValueChanged.AddListener(DivergeChanged);
        rhythmConvergeSlider.onValueChanged.AddListener(ConvergeChanged);

    }

    private bool initialised = false;
    public void Update()
    {
        if (initialised)
        {

            SyncMinimalToAdvanced();

        }
    }

    public void NotesChanged(float newVal)
    {
        customPitchNotes = false;
    }
    public void ChordsChanged(float newVal)
    {
        customChordProgression = false;
    }
    public void RhythmChanged(float newVal)
    {
        customBPMNotes = false;
    }

    public void ConvergeChanged(float newVal)
    {
        RhythmChanged(0);
        convergeMode = true;
    }

    public void DivergeChanged(float newVal)
    {
        RhythmChanged(0);
        convergeMode = false;
    }

    public void SyncMinimalToAdvanced()
    {
        pendulumGenerator.numPendulumsInputField.SetIntValue((int)numberOfNotesSlider.value);


        pendulumGenerator.numChordsInputField.SetIntValue((int)numberOfChordsSlider.value);
        pendulumGenerator.chordBPMInputField.SetFloatValue((float)rhythmSpeedSlider.value / 4 / (float)chordsSpeedSlider.value);

        pendulumGenerator.pendulumColourButton.SetColour(pendulumColourButton.GetColour());
        pendulumGenerator.bassColourButton.SetColour(bassColourButton.GetColour());
        pendulumGenerator.backgroundColourButton.SetColour(backgroundColourButton.GetColour());
        pendulumGenerator.UpdateBackgroundColour();

        pendulumGenerator.reverbSlider.value = reverbSlider.value;
        pendulumGenerator.filterSlider.value = filterSlider.value;
        pendulumGenerator.pitchMultiplierSlider.value = pitchSlider.value;
        pendulumGenerator.bloomSlider.value = bloomSlider.value;
        pendulumGenerator.particlesSlider.value = particlesSlider.value;

        pendulumGenerator.soundDropdown.value = sampleDropdown.value;

        switch (motionTypeDropdown.value)
        {

            case 0:
                pendulumGenerator.pendulumMotionToggle.isOn = true;

                pendulumGenerator.linearMotionToggle.isOn = false;
                pendulumGenerator.circularMotionToggle.isOn = false;
                pendulumGenerator.blackholeMotionToggle.isOn = false;
                break;
            case 1:
                pendulumGenerator.linearMotionToggle.isOn = true;
                pendulumGenerator.pendulumMotionToggle.isOn = false;
                pendulumGenerator.circularMotionToggle.isOn = false;
                pendulumGenerator.blackholeMotionToggle.isOn = false;
                break;
            case 2:
                pendulumGenerator.circularMotionToggle.isOn = true;
                pendulumGenerator.pendulumMotionToggle.isOn = false;
                pendulumGenerator.linearMotionToggle.isOn = false;
                pendulumGenerator.blackholeMotionToggle.isOn = false;
                break;
            case 3:
                pendulumGenerator.blackholeMotionToggle.isOn = true;
                pendulumGenerator.pendulumMotionToggle.isOn = false;
                pendulumGenerator.linearMotionToggle.isOn = false;
                pendulumGenerator.circularMotionToggle.isOn = false;
                break;
        }

        switch (shapeTypeDropdown.value)
        {
            case 0:
                pendulumGenerator.circularShapeToggle.isOn = true;
                pendulumGenerator.rectangleShapeToggle.isOn = false;
                break;
            case 1:
                pendulumGenerator.rectangleShapeToggle.isOn = true;
                pendulumGenerator.circularShapeToggle.isOn = false;
                break;
        }

        pendulumGenerator.bounceMotionToggle.isOn = reboundToggleButton.isOn;



        if (customBPMNotes)
        {

            pendulumGenerator.noteBPMDefinitions.SetBPMArrayContents(bpmEditBox.GetArrayValues(), 100, (int)numberOfNotesSlider.value, -1, -1);


            divergeSliderImage.color = new Color(divergeSliderImage.color.r, divergeSliderImage.color.g, divergeSliderImage.color.b, 0.1f);
            convergeSliderImage.color = new Color(convergeSliderImage.color.r, convergeSliderImage.color.g, convergeSliderImage.color.b, 0.1f);
            bpmSliderImage.color = new Color(bpmSliderImage.color.r, bpmSliderImage.color.g, bpmSliderImage.color.b, 0.1f);


        }
        else
        {
            float rootBpm = rhythmSpeedSlider.value / 4;
            float secondBpm = rootBpm * ((float)rhythmConvergeSlider.value / 1000f + 1);

            List<double> bpms = new List<double>
            {
                rootBpm,
                secondBpm
            };

            for (int i = 2; i < numberOfNotesSlider.value; i++)
            {
                bpms.Add(0);
            }

            pendulumGenerator.bpmLoopEnableToggle.isOn = true;
            pendulumGenerator.repeatBPMPatternInputField.SetIntValue(2);

            if (convergeMode)
            {
                pendulumGenerator.noteBPMDefinitions.SetBPMArrayContents(bpms, 2, (int)numberOfNotesSlider.value, (int)rhythmConvergeSlider.value, 0);

                divergeSliderImage.color = new Color(divergeSliderImage.color.r, divergeSliderImage.color.g, divergeSliderImage.color.b, 0.1f);
                convergeSliderImage.color = new Color(convergeSliderImage.color.r, convergeSliderImage.color.g, convergeSliderImage.color.b, 1f);
            }
            else
            {
                pendulumGenerator.noteBPMDefinitions.SetBPMArrayContents(bpms, 2, (int)numberOfNotesSlider.value, 0, (int)rhythmDivergeSlider.value);

                divergeSliderImage.color = new Color(divergeSliderImage.color.r, divergeSliderImage.color.g, divergeSliderImage.color.b, 1f);
                convergeSliderImage.color = new Color(convergeSliderImage.color.r, convergeSliderImage.color.g, convergeSliderImage.color.b, 0.1f);
            }

            //pendulumGenerator.noteBPMDefinitions.BPMInputChanged(2, (int)numberOfNotesSlider.value);

            bpmSliderImage.color = new Color(bpmSliderImage.color.r, bpmSliderImage.color.g, bpmSliderImage.color.b, 1f);


        }

        if (customChordProgression)
        {
            List<int> prog = new List<int>();
            prog.AddRange(chordEditBox.GetArrayValuesInt());
            pendulumGenerator.chordPitchDefinitions.SetArrayContents(prog.Count, 0, (int)numberOfChordsSlider.value, prog, true);

            chordSliderImage.color = new Color(chordSliderImage.color.r, chordSliderImage.color.g, chordSliderImage.color.b, 0.1f);
        }
        else
        {
            List<int> prog = new List<int>();
            prog.AddRange(progressions[(int)chordComplexitySlider.value - 1]);
            pendulumGenerator.chordPitchDefinitions.SetArrayContents(prog.Count, 0, (int)numberOfChordsSlider.value, prog, true);

            chordSliderImage.color = new Color(chordSliderImage.color.r, chordSliderImage.color.g, chordSliderImage.color.b, 1f);
        }

        if (customPitchNotes)
        {
            List<int> notePitches = new List<int>();
            notePitches.AddRange(noteEditBox.GetArrayValuesInt());
            pendulumGenerator.repeatPitchPatternInputField.SetIntValue(notePitches.Count);
            pendulumGenerator.notePitchDefinitions.SetArrayContents(notePitches.Count, notePitches.Count, (int)numberOfNotesSlider.value, notePitches, true);

            notePatternImage.color = new Color(notePatternImage.color.r, notePatternImage.color.g, notePatternImage.color.b, 0.1f);

        }
        else
        {
            List<int> notePitches = new List<int>();
            notePitches.AddRange(noteProgressions[(int)noteComplexitySlider.value - 1]);
            pendulumGenerator.repeatPitchPatternInputField.SetIntValue(notePitches.Count);
            pendulumGenerator.notePitchDefinitions.SetArrayContents(notePitches.Count, 7, (int)numberOfNotesSlider.value, notePitches, false);

            notePatternImage.color = new Color(notePatternImage.color.r, notePatternImage.color.g, notePatternImage.color.b, 1f);

        }

        pendulumGenerator.FillNoteTextBoxes();
        pendulumGenerator.FillChordTextBoxes();
        pendulumGenerator.FillBPMTextBoxes();
    }

    public void SyncAdvancedToMinimal()
    {

        /*
        float difference = (float) ((pendulumGenerator.pendulumBPMs[1] - pendulumGenerator.noteBPMDefinitions.GetArrayContents(0)) / pendulumGenerator.noteBPMDefinitions.GetArrayContents(0)) * 1000;
        rhythmConvergeSlider.value = difference;
        
        float chordBarLength =  pendulumGenerator.chordBPMInputField.GetFloatValue() / rhythmSpeedSlider.value * 4;
        chordsSpeedSlider.value = (int) (chordBarLength);
        */

        SetNotePitchEditMenu();
        SetChordProgEditMenu();
       // SetBPMEditMenu();


        pendulumColourButton.SetColour(pendulumGenerator.pendulumColourButton.GetColour());
        bassColourButton.SetColour(pendulumGenerator.bassColourButton.GetColour());
        backgroundColourButton.SetColour(pendulumGenerator.backgroundColourButton.GetColour());

        reverbSlider.value = pendulumGenerator.reverbSlider.value;
        filterSlider.value = pendulumGenerator.filterSlider.value;
        pitchSlider.value = pendulumGenerator.pitchMultiplierSlider.value;
        bloomSlider.value = pendulumGenerator.bloomSlider.value;
        particlesSlider.value = pendulumGenerator.particlesSlider.value;

        sampleDropdown.options = pendulumGenerator.soundDropdown.options;
        sampleDropdown.value = pendulumGenerator.soundDropdown.value;



        if (pendulumGenerator.pendulumMotionToggle.isOn)
        {
            motionTypeDropdown.value = 0;
        }
        else if (pendulumGenerator.circularMotionToggle.isOn)
        {
            motionTypeDropdown.value = 2;
        }
        else if(pendulumGenerator.linearMotionToggle.isOn)
        {
            motionTypeDropdown.value = 1;
        }
        else if(pendulumGenerator.blackholeMotionToggle.isOn)
        {
            motionTypeDropdown.value = 3;
        }

        if (pendulumGenerator.circularShapeToggle.isOn)
        {
            shapeTypeDropdown.value = 0;
        }
        else
        {
            shapeTypeDropdown.value = 1;
        }

        reboundToggleButton.isOn = pendulumGenerator.bounceMotionToggle.isOn;


        initialised = true;
    }


    public void OpenNotePitchEditMenu()
    {
        if (numberOfNotesSlider.value < 1)
        {
            numberOfNotesSlider.value = 1;
            SyncMinimalToAdvanced();
        }

        noteEditBox.OpenEditMenu((int)numberOfNotesSlider.value, pendulumGenerator.notePitchDefinitions.GetArrayContents());

        customPitchNotes = true;

    }

    public void OpenChordProgEditMenu()
    {
        if(numberOfChordsSlider.value < 1)
        {
            numberOfChordsSlider.value = 1;
            SyncMinimalToAdvanced();
        }

        chordEditBox.OpenEditMenu((int)numberOfChordsSlider.value, pendulumGenerator.chordPitchDefinitions.GetArrayContents());

        customChordProgression = true;
    }

    public void OpenBPMEditMenu()
    {

        bpmEditBox.OpenEditMenu((int)numberOfNotesSlider.value, pendulumGenerator.noteBPMDefinitions.GetBPMArrayContents());

        customBPMNotes = true;

    }


    public void CustomPitchNotes()
    {
        customPitchNotes = true;
    }

    public void CustomChordProgression()
    {
        customChordProgression = true;
    }

    public void CustomBPMNotes()
    {
        customBPMNotes = true;
    }

    public bool GetCustomPitchNotesEnb()
    {
        return customPitchNotes;
    }

    public bool GetCustomChordProgressionEnb()
    {
        return customChordProgression;
    }

    public bool GetCustomBPMNotesEnb()
    {
        return customBPMNotes;
    }


    private void SetNotePitchEditMenu()
    {
        noteEditBox.SetArray((int)numberOfNotesSlider.value, pendulumGenerator.notePitchDefinitions.GetArrayContents());
        noteEditBox.ExitEditMenu();

    }
    private void SetChordProgEditMenu()
    {
        chordEditBox.SetArray((int)numberOfChordsSlider.value, pendulumGenerator.chordPitchDefinitions.GetArrayContents());
        chordEditBox.ExitEditMenu();

    }
    public void SetBPMEditMenu(int numNote, List<double> newBPMS)
    {

        bpmEditBox.SetArray(numNote, newBPMS);
        bpmEditBox.ExitEditMenu();

    }


}
