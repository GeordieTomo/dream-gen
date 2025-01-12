using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Audio;
using UnityEngine.Networking;
using System;
using Unity.VisualScripting;
using static UnsavedChangesMenu;
using System.Runtime.CompilerServices;

public class PendulumGenerator : MonoBehaviour
{

    public static PendulumGenerator instance;

    private int numberOfPendulums;
    private float spacingMultiplier;
    private float spacingAddition;

    public float globalPitchMultiplier = 1f;
    public float hiddenPitchMultiplier = 0.5f; // samples come in at C3 - transpose these to C2 by default

    private int numberOfChords;
    private double chordBPM;

    private double minBPM;
    private double maxBPM;

    private double timeOffset;

    public TMP_Dropdown presetDropdown;
    public TMP_Dropdown soundDropdown;

    public string audioFolderPath = "Sounds";
    public string presetsFolderPath = "Presets";
    public AudioClip selectedAudioClip;
    public string selectedAudioClipName;

    public TMP_InputField presetNameInputField;

    public InputFieldParser numPendulumsInputField;
    public InputFieldParser spacingMultiplierInputField;
    public InputFieldParser spacingAdditionInputField;
    public InputFieldParser repeatPitchPatternInputField;
    public InputFieldParser repeatPitchShiftInputField;
    public InputFieldParser repeatBPMPatternInputField;
    public InputFieldParser numChordsInputField;
    public double rootBPM;
    public InputFieldParser chordBPMInputField;
    public InputFieldParser timeOffsetInputField;

    public InputFieldParser noteOctaveInputField;
    public InputFieldParser bassOctaveInputField;

    public InputFieldParser midiStartTimeInputField;
    public InputFieldParser midiEndTimeInputField;
    public InputFieldParser midiFilenameInputField;
    public TMP_Text exportMidiCompletionText;
    MidiGenerator midiGen;
    public List<Button> midiFreezeButtonsWhileExport;

    public Slider exportProgressBar;

    public Button hideNotesButton;
    public Button hideChordsButton;
    public Button hideVisualsButton;
    public Button hideAudioFXButton;
    public Toggle loopEnableToggle;
    public Toggle bpmLoopEnableToggle;

    public Slider reverbSlider;
    public Slider filterSlider;
    public Slider bloomSlider;
    public Slider particlesSlider;

    [SerializeField] public Slider pitchMultiplierSlider;

    public EffectsController effectsController;

    public ColourPicker colorPickerMenu;

    public ColourPickerButton pendulumColourButton;
    public ColourPickerButton bassColourButton;
    public ColourPickerButton backgroundColourButton;

    public ScalePicker scalePickerMenu;

    public Button startButton;

    public TextBoxArrayController notePitchDefinitions;
    public BPMTextBoxArrayController noteBPMDefinitions;

    public TextBoxArrayController chordPitchDefinitions;

    public TabNavigation tabNavigation;

    public Toggle semitonesToggle;
    public Toggle scaleDegreeToggle;
    public Toggle justIntonationToggle;

    private PitchType pitchType;

    public Toggle circularMotionToggle;
    public Toggle linearMotionToggle;
    public Toggle pendulumMotionToggle;
    public Toggle blackholeMotionToggle;

    public Toggle rectangleShapeToggle;
    public Toggle circularShapeToggle;

    public Toggle bounceMotionToggle;

    public GameObject pendulumPrefab;

    public GameObject linePrefab;
    public GameObject circlePrefab;

    public AudioMixer audioMixer;

    public List<Pendulum> pendulums = new List<Pendulum>();
    public List<Pendulum> chordPendulums = new List<Pendulum>();

    public List<int> pendulumScaleDegrees = new List<int>();
    public List<int> chordScaleDegrees = new List<int>();
    public List<double> pendulumBPMs = new List<double>();

    private bool simulationRunning = false;
    private bool previewRunning = false;

    private GameObject triggerDisplay;
    public TriggerDisplay triggerDisplayControl;

    private bool presetUnsaved = false;
    public SaveListener saveListener;

    private PresetMode selectedPresetMode;
    private int curPresetIndex;

    public UnsavedChangesMenu unsavedChangesMenu;
    private bool newPreset = false;

    [SerializeField]
    private Button saveAsButton;
    [SerializeField]
    private AudioClip defaultAudioClip;

    public GameObject reloadPresetWarning;
    public Button reloadPresetButton;
    public Button pauseButton;

    public GameObject confirmMidiOverwriteMenu;

    public SubMenuListener[] subMenus;

    private bool submenuOpen = false;

    private bool minimalMode = true;

    public GameObject advancedModeMenu;
    public MinimalUIController minimalModeMenu;

    private bool playState = false;

    public GameObject mainMenu;

    private IOrderedEnumerable<String> sortedFiles;

    MotionType curMotion = MotionType.LinearMotion;
    MotionType prevMotion = MotionType.LinearMotion;
    private bool bouncePrevState = false;
         
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        advancedModeMenu.gameObject.SetActive(true);

        Application.wantsToQuit += CheckUnsavedChangesBeforeQuit;

        PopulateSoundDropdown();
        soundDropdown.onValueChanged.AddListener(SoundDropdownValueChanged);

        PopulateDropdown();
        presetDropdown.onValueChanged.AddListener(OnDropdownValueChanged);

        if (hideNotesButton.IsActive()) hideNotesButton.onClick.Invoke();
        if (hideChordsButton.IsActive()) hideChordsButton.onClick.Invoke();
        if (hideAudioFXButton.IsActive()) hideAudioFXButton.onClick.Invoke();
        if (hideVisualsButton.IsActive()) hideVisualsButton.onClick.Invoke();
        noteBPMDefinitions.gameObject.SetActive(false);

        startButton.onClick.AddListener(Play);
        numPendulumsInputField.AddValueChangedListener(FillNoteTextBoxes);
        numPendulumsInputField.AddValueChangedListener(FillBPMTextBoxes);
        numPendulumsInputField.AddValueChangedListener(PreviewSimulation);
        repeatPitchPatternInputField.AddValueChangedListener(FillNoteTextBoxes);
        repeatBPMPatternInputField.AddValueChangedListener(FillBPMTextBoxes);
        repeatPitchShiftInputField.AddValueChangedListener(FillNoteTextBoxes);
        loopEnableToggle.onValueChanged.AddListener(FillNoteTextBoxes);
        bpmLoopEnableToggle.onValueChanged.AddListener(FillBPMTextBoxes);
        numChordsInputField.AddValueChangedListener(FillChordTextBoxes);
        numChordsInputField.AddValueChangedListener(PreviewSimulation);
        noteOctaveInputField.AddValueChangedListener(FillNoteTextBoxes);
        bassOctaveInputField.AddValueChangedListener(FillChordTextBoxes);


        bounceMotionToggle.onValueChanged.AddListener(PreviewSimulation);
        rectangleShapeToggle.onValueChanged.AddListener(PreviewSimulation);
        circularShapeToggle.onValueChanged.AddListener(PreviewSimulation);
        circularMotionToggle.onValueChanged.AddListener(PreviewSimulation);
        pendulumMotionToggle.onValueChanged.AddListener(PreviewSimulation);
        linearMotionToggle.onValueChanged.AddListener(PreviewSimulation);
        blackholeMotionToggle.onValueChanged.AddListener(PreviewSimulation);
        chordBPMInputField.AddValueChangedListener(PreviewSimulation);
        noteBPMDefinitions.AddValueChangedListener(PreviewSimulation);
        timeOffsetInputField.AddValueChangedListener(PreviewSimulation);
        

        reverbSlider.onValueChanged.AddListener(ReverbLevel);
        ReverbLevel(reverbSlider.value);
        filterSlider.onValueChanged.AddListener(FilterLevel);
        FilterLevel(filterSlider.value);
        bloomSlider.onValueChanged.AddListener(BloomLevel);
        BloomLevel(bloomSlider.value);
        particlesSlider.onValueChanged.AddListener(ParticlesLevel);

        pitchMultiplierSlider.onValueChanged.AddListener(SetGlobalPitch);
        SetGlobalPitch(pitchMultiplierSlider.value);

        colorPickerMenu.OnValueChanged += (UpdateBackgroundColour);
        colorPickerMenu.gameObject.SetActive(false);

        scalePickerMenu.gameObject.SetActive(false);
        /*
        if (minimalMode)
        {
            advancedModeMenu.gameObject.SetActive(false);
            minimalModeMenu.gameObject.SetActive(true);
        }
        else
        {
            advancedModeMenu.gameObject.SetActive(true);
            minimalModeMenu.gameObject.SetActive(false);

        }*/

        
        minimalModeMenu.SyncAdvancedToMinimal();
        //
        advancedModeMenu.gameObject.SetActive(false);


        //LoadPreset(selectedPresetMode);

    }

    public void Update()
    {
        
        if (HotkeysActive())
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.P)))
            {
                TogglePlayState();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {

                ToggleMenu();

            }

            if (Input.GetKeyDown(KeyCode.R) && reloadPresetButton.interactable)
            {
                ReloadPresetWithCheck();
                //ReloadPreset();
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                ReloadPresetTime();
            }
        }

        presetUnsaved = CheckPresetUnsaved();
        bool timeUnsaved = CheckTimeUnsaved();
        saveListener.PresetUnsaved(presetUnsaved || timeUnsaved);

        if (playState)
        {
            UpdateTimeOffset();
        }
    }

    public bool HotkeysActive()
    {
        submenuOpen = false;
        foreach (SubMenuListener submenu in subMenus)
        {
            if (submenu.IsActive())
            {
                submenuOpen = true;
            }
        }


        return !IsTextInputFocused() && !submenuOpen;
    }

    public void ReloadPresetWithCheck()
    {

        if (CheckPresetUnsaved())
        {
            reloadPresetWarning.SetActive(true);
        }
        else
        {
            ReloadPreset();
        }
    }

    public void ToggleMenu()
    {
        if (transform.parent.gameObject.activeInHierarchy)
        {
            if (!playState)
            {
                startButton.onClick.Invoke();
            }

            transform.parent.gameObject.SetActive(false);
        }

        else
        {
            transform.parent.gameObject.SetActive(true);
        }
    }

    public void TogglePlayState()
    {
        if (playState)
        {
            pauseButton.onClick.Invoke();
        }
        else
        {
            startButton.onClick.Invoke();
        }

        UpdateSimulation(playState);
    }

    public void ReverbLevel(float newVal) {

        // Map the input value to a linear scale between 0 and 1
        float linearScale = MathUtil.Map(newVal, 0, 100, 0, 1);

        // Convert linear to logarithmic scale using base 10
        float reverbInDecibels = -Mathf.Pow(100, 1 - linearScale) + 1;
        float dryInDecibels = -Mathf.Pow(100, linearScale) + 1;

        // Map the logarithmic scale back to the desired range between -2500 and 0
        reverbInDecibels = MathUtil.Map(reverbInDecibels, -99, 0, -10000, 0);
        dryInDecibels = MathUtil.Map(dryInDecibels, -99, 0, -10000, 0);

        // Set the reverb levels in the audio mixer
        audioMixer.SetFloat("DryLevel", dryInDecibels);
        audioMixer.SetFloat("Room", reverbInDecibels);

    }

    public void FilterLevel(float newVal)
    {
        // Map the input value to a linear scale between 0 and 1
        float linearScale = MathUtil.Map(newVal, 0, 100, 0, 1);

        // Convert linear to logarithmic scale using base 10
        float filterInHz = Mathf.Pow(100, linearScale) - 1;

        // Map the logarithmic scale back to the desired range between -2500 and 0
        filterInHz = MathUtil.Map(filterInHz, 0, 99, 200, 22000);

        // Set the reverb levels in the audio mixer
        audioMixer.SetFloat("Filter", filterInHz);
    }

    public void BloomLevel(float newVal)
    {
        // Map the input value to a linear scale between 0 and 1
        float linearScale = MathUtil.Map(newVal, 0, 100, 0, 1);

        // Convert linear to logarithmic scale using base 10
        float bloom = Mathf.Pow(10, linearScale) - 1;

        // Map the logarithmic scale back to the desired range between -2500 and 0
        bloom = MathUtil.Map(bloom, 0, 9, 0, 25);

        // Set the bloom on the volume
        effectsController.SetBloom(bloom);
    }

    public void ParticlesLevel(float newVal)
    {
        // Map the input value to a linear scale between 0 and 1
        float linearScale = MathUtil.Map(newVal, 0, 100, 0, 1);

        // Convert linear to logarithmic scale using base 10
        float filterInHz = Mathf.Pow(100, linearScale) - 1;

        // Map the logarithmic scale back to the desired range between -2500 and 0
        filterInHz = MathUtil.Map(filterInHz, 0, 99, 200, 22000);

        // Set the reverb levels in the audio mixer
        //audioMixer.SetFloat("Filter", filterInHz);
    }

    private void SetGlobalPitch(float newVal)
    {
        globalPitchMultiplier = newVal;
    }

    public void GenerateMidiFile()
    {
        var filePath = Path.Combine("MIDI", midiFilenameInputField.GetStringValue() + ".mid");

        if (File.Exists(filePath))
        {
            ConfirmMidiOverwrite();
        }
        else
        {
            GenerateMidiFile(true);
        }

    }

    public void ConfirmMidiOverwrite()
    {
        confirmMidiOverwriteMenu.SetActive(true);

    }

    public void GenerateMidiFile(bool overwrite)
    {
        rootBPM = noteBPMDefinitions.GetBPMArrayContents(0);

        var filePath = Path.Combine("MIDI", midiFilenameInputField.GetStringValue() + ".mid");

        var bpms = new List<double>();   // Example timings in milliseconds

        for (int i = 0; i < numberOfPendulums; i++)
        {
            double pendulumBPM = (Mathf.Pow(spacingMultiplier, numberOfPendulums - 1 - i) + spacingAddition * (numberOfPendulums - 1 - i)) * rootBPM;
            bpms.Add(pendulumBPM);
        }

        chordBPM = chordBPMInputField.GetDoubleValue();

        if (midiGen == null)
            midiGen = this.AddComponent<MidiGenerator>();

        StartCoroutine(midiGen.GenerateMidi(filePath, pendulumScaleDegrees, pendulumBPMs, chordScaleDegrees, chordBPM, midiStartTimeInputField.GetDoubleValue(), midiEndTimeInputField.GetDoubleValue(), exportProgressBar, exportMidiCompletionText, this));

        foreach (Button button in midiFreezeButtonsWhileExport)
        {
            button.interactable = false;
        }
    }

    public void MidiFileComplete()
    {
        foreach (Button button in midiFreezeButtonsWhileExport)
        {
            button.interactable = true;
        }
    }

    public void CancelMidiGeneration()
    {
        if (midiGen != null)
            midiGen.Cancel(exportProgressBar);
    }

    public void PreviewSimulation(string _)
    {
        UpdateSimulation(playState);
    }

    public void PreviewSimulation(bool _)
    {
        PreviewSimulation("");
    }

    private void Play()
    {
        playState = true;

        UpdateSimulation(playState);

    }

    public void Pause()
    {
        playState = false;
    }

    private void UpdateSimulation(bool playState)
    {
        numberOfPendulums = numPendulumsInputField.GetIntValue();
        numberOfChords = numChordsInputField.GetIntValue();

        ReadPitchInputBoxes();


        int currentChord;
        if (numberOfChords > 0)
        {
            chordBPM = chordBPMInputField.GetDoubleValue();
            int currentChordIndex = Mathf.FloorToInt(Convert.ToSingle(timeOffsetInputField.GetDoubleValue() / 60f * chordBPM)) % numberOfChords;
            if (currentChordIndex < 0)
            {
                currentChordIndex += numberOfChords;
            }
            currentChord = chordScaleDegrees[currentChordIndex];
        }
        else
        {
            currentChord = 1;
        }

        //globalPitchMultiplier = pitchMultiplierInputField.GetFloatValue();

        minBPM = 120;
        maxBPM = 0;
        foreach (double bpm in pendulumBPMs)
        {
            if (bpm < minBPM)
            {
                minBPM = bpm;
            }
            if (bpm > maxBPM)
            {
                maxBPM = bpm;
            }
        }

        timeOffset = timeOffsetInputField.GetDoubleValue();

        for (int i = 0; i < numberOfPendulums; i++)
        {
            double pendulumBPM = pendulumBPMs[numberOfPendulums - 1 - i];

            if (i < pendulums.Count)
            {
                pendulums[i].Initialise(pendulumBPM, ScaleDegreesToFrequency(pendulumScaleDegrees[numberOfPendulums - 1 - i], currentChord, noteOctaveInputField.GetIntValue()) * globalPitchMultiplier * hiddenPitchMultiplier, 0,
                minBPM, maxBPM, bounceMotionToggle.isOn, linearMotionToggle.isOn, rectangleShapeToggle.isOn, circularMotionToggle.isOn, blackholeMotionToggle.isOn, timeOffset, i, numberOfPendulums, numberOfChords, false, pendulumColourButton.GetColour(), particlesSlider.value, !playState, false, this);
            }
            else
            {
                pendulums.Add(Instantiate<GameObject>(pendulumPrefab).GetComponent<Pendulum>());
                pendulums.Last().Initialise(pendulumBPM, ScaleDegreesToFrequency(pendulumScaleDegrees[numberOfPendulums - 1 - i], currentChord, noteOctaveInputField.GetIntValue()) * globalPitchMultiplier * hiddenPitchMultiplier, 0,
                    minBPM, maxBPM, bounceMotionToggle.isOn, linearMotionToggle.isOn, rectangleShapeToggle.isOn, circularMotionToggle.isOn, blackholeMotionToggle.isOn, timeOffset, i, numberOfPendulums, numberOfChords, false, pendulumColourButton.GetColour(), particlesSlider.value, !playState, true, this);

            }
        }
        
        if (pendulums.Count > numberOfPendulums)
        {
            for (int i = numberOfPendulums; i < pendulums.Count; i++)
            {
                Destroy(pendulums[i].gameObject);
                pendulums.RemoveAt(i);
            }
        }


        for (int i = 0; i < numberOfChords; i++)
        {

            if (i < chordPendulums.Count)
            {

                chordPendulums[i].Initialise(chordBPM / numberOfChords, ScaleDegreesToFrequency(chordScaleDegrees[i], 1, bassOctaveInputField.GetIntValue()) * globalPitchMultiplier * hiddenPitchMultiplier, chordScaleDegrees[i],
                    minBPM, maxBPM, bounceMotionToggle.isOn, linearMotionToggle.isOn, rectangleShapeToggle.isOn, circularMotionToggle.isOn, blackholeMotionToggle.isOn, timeOffset, numberOfPendulums + i, numberOfPendulums, numberOfChords, true, bassColourButton.GetColour(), particlesSlider.value, !playState, false, this);

            }
            else
            {
                chordPendulums.Add(Instantiate<GameObject>(pendulumPrefab).GetComponent<Pendulum>());
                chordPendulums.Last().Initialise(chordBPM / numberOfChords, ScaleDegreesToFrequency(chordScaleDegrees[i], 1, bassOctaveInputField.GetIntValue()) * globalPitchMultiplier * hiddenPitchMultiplier, chordScaleDegrees[i],
                    minBPM, maxBPM, bounceMotionToggle.isOn, linearMotionToggle.isOn, rectangleShapeToggle.isOn, circularMotionToggle.isOn, blackholeMotionToggle.isOn, timeOffset, numberOfPendulums + i, numberOfPendulums, numberOfChords, true, bassColourButton.GetColour(), particlesSlider.value, !playState, true, this);

            }
        }


        if (chordPendulums.Count > numberOfChords)
        {
            for (int i = numberOfChords; i < chordPendulums.Count; i++)
            {
                Destroy(chordPendulums[i].gameObject);
                chordPendulums.RemoveAt(i);
            }
        }

        if (linearMotionToggle.isOn)
        {
            curMotion = MotionType.LinearMotion;
        }
        else if (circularMotionToggle.isOn)
        {
            curMotion = MotionType.CircularMotion;
        }
        else if (pendulumMotionToggle.isOn)
        {
            curMotion = MotionType.PendulumMotion;
        }
        else if (blackholeMotionToggle.isOn)
        {
            curMotion = MotionType.BlackHoleMotion;
        }

        if (prevMotion != curMotion || bouncePrevState != bounceMotionToggle.isOn)
        {
            Destroy(triggerDisplay);

            // motion control
            if (linearMotionToggle.isOn)
            {
                // generate horizontal line
                triggerDisplay = Instantiate(linePrefab);
                if (bounceMotionToggle.isOn)
                {
                    // move line down to bottom
                    triggerDisplay.transform.position = new Vector3(-32, -4.5f, 0);
                }
            }
            else if (circularMotionToggle.isOn)
            {
                // generate vertical line
                triggerDisplay = Instantiate(linePrefab);
                triggerDisplay.transform.position = new Vector3(0, 35, 0);
                triggerDisplay.transform.rotation = Quaternion.Euler(0, 0, -90f);

                if (bounceMotionToggle.isOn)
                {
                    // move line down to bottom
                    triggerDisplay.transform.position = new Vector3(0, 0, 0);
                }
            }
            else if (pendulumMotionToggle.isOn)
            {
                // generate vertical line
                triggerDisplay = Instantiate(linePrefab);
                triggerDisplay.transform.position = new Vector3(0, 35, 0);
                triggerDisplay.transform.rotation = Quaternion.Euler(0, 0, -90f);

                if (bounceMotionToggle.isOn)
                {
                    triggerDisplay.transform.position = new Vector3(30f, 22.5f, 0);

                    // rotate line to side
                    triggerDisplay.transform.rotation = Quaternion.Euler(0, 0, 210f);
                }

            }
            else if (blackholeMotionToggle.isOn)
            {
                // generate circular line
                triggerDisplay = Instantiate(circlePrefab);
            }
            triggerDisplayControl = triggerDisplay.GetComponentInChildren<TriggerDisplay>();

        }
        prevMotion = curMotion;
        bouncePrevState = bounceMotionToggle.isOn;
        //UpdateTimeOffset();
    }

    public void OpenMenu()
    {
        if (pendulums.Count > 0)
            timeOffset = pendulums[0].GetTimeOffset();
        else if (chordPendulums.Count > 0)
            timeOffset = chordPendulums[0].GetTimeOffset();

        timeOffsetInputField.SetDoubleValue(timeOffset);
    }
    
    public void SetTimeOffset()
    {
        if (pendulums.Count > 0)
            timeOffset = pendulums[0].GetTimeOffset();
        else if (chordPendulums.Count > 0)
            timeOffset = chordPendulums[0].GetTimeOffset();
        timeOffsetInputField.SetDoubleValue(timeOffset);
    }

    public void UpdateTimeOffset()
    {
        /*
        if (pendulums.Count > 0)
            timeOffset = pendulums[0].GetTimeOffset();
        else if (chordPendulums.Count > 0)
            timeOffset = chordPendulums[0].GetTimeOffset();*/
        timeOffset += Time.deltaTime;
        timeOffsetInputField.SetDoubleValue(timeOffset);

    }

    public void SetPitchesFromBassNote(int rootNote)
    {
        for (int i = 0; i < numberOfPendulums; i++)
        {
            float newPitch = ScaleDegreesToFrequency(pendulumScaleDegrees[numberOfPendulums - 1 - i], rootNote, noteOctaveInputField.GetIntValue()) * globalPitchMultiplier * hiddenPitchMultiplier;
            pendulums[i].SetPitch(newPitch);
        }
    }

    private float ScaleDegreesToFrequency(int scaleDegree, int rootNote, int octave)
    {
        return scalePickerMenu.ScaleDegreeToFrequency(scaleDegree, rootNote, octave);
    }

    public int ScaleDegreesToSemitones(int scaleDegree, int chordDegree, bool chord)
    {
        if (!chord)
            return scalePickerMenu.ScaleDegreeToSemitone(scaleDegree, chordDegree, noteOctaveInputField.GetIntValue());
        else
            return scalePickerMenu.ScaleDegreeToSemitone(scaleDegree, chordDegree, bassOctaveInputField.GetIntValue());
    }

    public void PopulateSoundDropdown() {

        string[] wavFiles;
        if (Directory.Exists(audioFolderPath))
        {

            wavFiles = Directory.GetFiles(audioFolderPath, "*.wav");

        }
        else
        {
            Debug.LogError("No Audio File Directory");
            wavFiles = new string[0];
        }

        List<string> wavFileNames = new List<string>();

        if (wavFiles != null)
        {
            foreach (string file in wavFiles)
            {
                // Get only the filename without the path and extension
                string fileName = Path.GetFileNameWithoutExtension(file);
                wavFileNames.Add(fileName);
            }
        }

        string curName = soundDropdown.captionText.text;

        // Clear the current options in the dropdown
        soundDropdown.ClearOptions();

        // Add the WAV file names to the dropdown options
        soundDropdown.AddOptions(wavFileNames);

        if (curName != "default" && curName != "")
            SetSoundDropdown(curName);

    }

    public void OpenDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        System.Diagnostics.Process.Start("explorer.exe", directory);
    }

    public void OpenSoundDirectory()
    {
        if (!Directory.Exists(audioFolderPath))
        {
            Directory.CreateDirectory(audioFolderPath);
        }

        System.Diagnostics.Process.Start("explorer.exe", audioFolderPath);
    }

    public void OpenPresetsDirectory()
    {
        if (!Directory.Exists(presetsFolderPath))
        {
            Directory.CreateDirectory(presetsFolderPath);
        }

        System.Diagnostics.Process.Start("explorer.exe", presetsFolderPath);
    }

    public void SoundDropdownValueChanged(int index)
    {
        if (index >= 0)
        {
            selectedAudioClipName = soundDropdown.options[index].text;

            string filePath = Path.Combine(audioFolderPath, selectedAudioClipName + ".wav");
            StartCoroutine(LoadAudioClip(filePath));
        }
    }

    public void SetSoundDropdown(string name)
    {
        string filePath = audioFolderPath + "/" + name + ".wav";

        int index = -1;

        for (int i = 0; i < soundDropdown.options.Count; i++)
        {
            if (soundDropdown.options[i].text == name)
            {
                index = i;
            }
        }

        if (File.Exists(filePath))
        {
            StartCoroutine(LoadAudioClip(filePath));

            if (index != -1)
            {
                soundDropdown.value = index;
                soundDropdown.RefreshShownValue();
            }
            else
            {
                Debug.LogWarning("String not found in dropdown options!");
            }
        }
        else
        {
            soundDropdown.value = -1;
            soundDropdown.RefreshShownValue();
            selectedAudioClip = defaultAudioClip;
            Debug.LogError("Sound File Not In Directory");
        }
    }

    private IEnumerator LoadAudioClip(string filePath)
    {
        UnityWebRequest req = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath, AudioType.WAV);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            selectedAudioClip = DownloadHandlerAudioClip.GetContent(req);
            // Audio clip loaded successfully, do something with selectedAudioClip
        }
        else
        {
            // Log the error if the request was not successful
            Debug.LogError("Failed to load audio clip. Error: " + req.error);
        }
    }


    public void PopulateDropdownNoChange()
    {
        PopulateDropdown(selectedPresetMode.name, false);
    }

    private void PopulateDropdown()
    {
        PopulateDropdown("");
    }

    private void PopulateDropdown(string presetName)
    {
        PopulateDropdown(presetName, true);
    }

    private void PopulateDropdown(string presetName, bool refresh)
    {
        presetDropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> dropdownOptions = new List<TMP_Dropdown.OptionData>();

        List<PresetMode> presets = LoadPresets(); // Use the LoadPresets function

        foreach (PresetMode presetMode in presets)
        {
            string presetModeName = presetMode.name;
            dropdownOptions.Add(new TMP_Dropdown.OptionData(presetModeName));
        }

        presetDropdown.AddOptions(dropdownOptions);

        // Find the index of the preset by its name
        int indexToSelect = presets.FindIndex(preset => preset.name == presetName);

        if (presetName == "" && indexToSelect == -1)
        {
            indexToSelect = 0;
        }

        curPresetIndex = indexToSelect;


        //if (refresh)
        {
            // The preset name was found, select it in the dropdown
            presetDropdown.value = indexToSelect;
            presetDropdown.RefreshShownValue();
            OnDropdownValueChanged(indexToSelect); // Trigger any necessary actions upon selecting the preset
        }
        
        
    }

    public void OnDropdownValueChanged(int index)
    {
        List<PresetMode> presets = LoadPresets();

        // Handle the selection here
        // You can access the selected PresetMode asset and use it as needed

        if (selectedPresetMode == null)
        {
            if (index >= 0 && index < presets.Count)
            {
                newPreset = false;
                    selectedPresetMode = presets[index];

                    // Use the selected preset as needed
                    LoadPreset(selectedPresetMode);
                curPresetIndex = index;

            }
            else if (index == -1 || presets.Count == 0)
            {
                newPreset = true;
                selectedPresetMode = new PresetMode();
                LoadPreset(selectedPresetMode);
            }
        }
            
        else 
        {
            if (presetUnsaved && index == -1)
            {
                if (curPresetIndex != index)
                {
                    StartCoroutine(OnDropdownValueChangedUnsaved(index));

                }
                else
                {

                }
            }
            else if (presetUnsaved && selectedPresetMode.name != presets[index].name)
            {
                StartCoroutine(OnDropdownValueChangedUnsaved(index));
            }
            else
            {
                if (index >= 0 && index < presets.Count)
                {
                    newPreset = false;
                    if (selectedPresetMode.name != presets[index].name)
                    {
                        selectedPresetMode = presets[index];

                        // Use the selected preset as needed
                        LoadPreset(selectedPresetMode);
                    }
                    curPresetIndex = index;

                }
                else if (curPresetIndex != index && (index == -1 || presets.Count == 0))
                {

                    Debug.Log(curPresetIndex);
                    Debug.Log(index);

                    newPreset = true;
                    selectedPresetMode = new PresetMode();
                    selectedPresetMode.spacingConverge = 10;
                    selectedPresetMode.noteComplexity = 4;
                    selectedPresetMode.chordComplexity = 4;
                    LoadPreset(selectedPresetMode);
                    curPresetIndex = -1;
                }
            }
        }
    }

    private IEnumerator OnDropdownValueChangedUnsaved(int index) 
    {
        if (!unsavedChangesMenu.gameObject.activeInHierarchy)
        {
            unsavedChangesMenu.gameObject.SetActive(true);
            unsavedChangesMenu.Open();

            UnsavedMenuState tmpState = unsavedChangesMenu.State();
            while (tmpState == UnsavedMenuState.MenuOpen)
            {

                tmpState = unsavedChangesMenu.State();

                yield return new WaitForEndOfFrame();
            }
            if (tmpState == UnsavedMenuState.Save)
            {
                saveListener.SaveFunction();
                presetUnsaved = false;
                OnDropdownValueChanged(index);
            }
            else if (tmpState == UnsavedMenuState.DontSave)
            {
                presetUnsaved = false;
                OnDropdownValueChanged(index);
            }
            // Cancel button
            else
            {
                presetDropdown.value = curPresetIndex;
            }
            unsavedChangesMenu.gameObject.SetActive(false);
        }
    }

    private bool CheckUnsavedChangesBeforeQuit()
    {
        if (presetUnsaved)
        {
            StartCoroutine(QuitGameUnsaved());
            return false;
        }
        
        return true;
    }

    public void QuitGame()
    {

        if (presetUnsaved)
        {
            StartCoroutine(QuitGameUnsaved()); 
        }
        else
        {
            Application.Quit();
        }
    }
    private IEnumerator QuitGameUnsaved()
    {
        if (!unsavedChangesMenu.gameObject.activeInHierarchy)
        {
            unsavedChangesMenu.gameObject.SetActive(true);
            unsavedChangesMenu.Open();

            UnsavedChangesMenu.UnsavedMenuState tmpState = unsavedChangesMenu.State();
            while (tmpState == UnsavedChangesMenu.UnsavedMenuState.MenuOpen)
            {
                tmpState = unsavedChangesMenu.State();
                yield return new WaitForEndOfFrame();
            }
            if (tmpState == UnsavedChangesMenu.UnsavedMenuState.Save)
            {
                saveListener.SaveFunction();
                presetUnsaved = false;
                QuitGame();
            }
            else if (tmpState == UnsavedChangesMenu.UnsavedMenuState.DontSave)
            {
                presetUnsaved = false;
                QuitGame();
            }

            unsavedChangesMenu.gameObject.SetActive(false);
        }
    }




    public void DeleteCurrentPreset()
    {
        string[] jsonFiles = Directory.GetFiles(presetsFolderPath, "*.json");
        //sortedFiles = jsonFiles.OrderByDescending(f => new FileInfo(f).LastWriteTime);

        if (jsonFiles.Length > 0)
        {
            int selectedIndex = presetDropdown.value;
            // Assuming the selected index corresponds to the preset to delete
            if (selectedIndex >= 0 && selectedIndex < jsonFiles.Length)
            {
                string fileToDelete = sortedFiles.ElementAt(selectedIndex);
                File.Delete(fileToDelete);
                Debug.Log("Preset deleted: " + fileToDelete);
            }
            else
            {
                Debug.LogWarning("Invalid selection to delete.");
            }
        }
        else
        {
            Debug.LogWarning("No presets found to delete.");
        }

        PopulateDropdown(); // Refresh dropdown after deletion
    }

    public void ReloadPreset()
    {

        if (true || newPreset)
        {
            LoadPreset(selectedPresetMode);
        }
        else
        {
            List<PresetMode> presets = LoadPresets();
            if (curPresetIndex >= 0 && curPresetIndex < presets.Count)
            {

                selectedPresetMode = presets[curPresetIndex];

                // Use the selected preset as needed
                LoadPreset(selectedPresetMode);
            }
        }
    }

    public void ReloadPresetTime()
    {
        pauseButton.gameObject.SetActive(false);
        startButton.gameObject.SetActive(true);

        playState = false;
        if (true || newPreset)
        {
            timeOffsetInputField.SetDoubleValue(selectedPresetMode.timeOffset);
            PreviewSimulation("");
        }
        else
        {
            List<PresetMode> presets = LoadPresets();
            if (curPresetIndex >= 0 && curPresetIndex < presets.Count)
            {

                selectedPresetMode = presets[curPresetIndex];

                // Use the selected preset as needed
                timeOffsetInputField.SetDoubleValue(selectedPresetMode.timeOffset);
                PreviewSimulation("");
            }
        }
    }


    public void LoadPreset(PresetMode presetMode)
    {
        semitonesToggle.isOn = presetMode.pitchType == PitchType.Semitone;
        scaleDegreeToggle.isOn = presetMode.pitchType == PitchType.ScaleDegree;
        justIntonationToggle.isOn = presetMode.pitchType == PitchType.JustIntonation;
        bpmLoopEnableToggle.isOn = presetMode.bpmLoopEnable;
        pitchType = presetMode.pitchType;
        scalePickerMenu.SetScale(presetMode.scaleDegrees);
        numPendulumsInputField.SetIntValue(presetMode.numPendulums);
        bool custom = presetMode.noteComplexity == -1;
        notePitchDefinitions.SetArrayContents(presetMode.notes.Count, presetMode.notes.Count, presetMode.numPendulums, presetMode.notes, custom);
        noteBPMDefinitions.SetBPMArrayContents(presetMode.noteBPMs, 100, presetMode.numPendulums, presetMode.spacingConverge, presetMode.spacingDiverge);
        repeatPitchPatternInputField.SetIntValue(100);
        repeatPitchShiftInputField.SetFloatValue(7);
        repeatBPMPatternInputField.SetIntValue(presetMode.noteBPMRepeat);
        numChordsInputField.SetIntValue(presetMode.numOfChords);
        chordPitchDefinitions.SetArrayContents(8, 8, presetMode.numOfChords, presetMode.chords, true);
        chordBPMInputField.SetDoubleValue(presetMode.chordBPM);
        pitchMultiplierSlider.value = (presetMode.globalPitchMod);
        noteOctaveInputField.SetIntValue(presetMode.noteOctave);
        bassOctaveInputField.SetIntValue(presetMode.bassOctave);
        pendulumMotionToggle.isOn = presetMode.motionType == MotionType.PendulumMotion;
        linearMotionToggle.isOn = presetMode.motionType == MotionType.LinearMotion;
        blackholeMotionToggle.isOn = presetMode.motionType == MotionType.BlackHoleMotion;
        circularMotionToggle.isOn = presetMode.motionType == MotionType.CircularMotion;
        circularShapeToggle.isOn = presetMode.shapeType == ShapeType.Circle;
        rectangleShapeToggle.isOn = presetMode.shapeType == ShapeType.Rectangle;
        bounceMotionToggle.isOn = presetMode.bounce;
        pendulumColourButton.SetColour(presetMode.pendulumColour);
        bassColourButton.SetColour(presetMode.bassColour);
        backgroundColourButton.SetColour(presetMode.backgroundColour);
        reverbSlider.value = presetMode.reverbLevel;
        filterSlider.value = presetMode.filterLevel;
        bloomSlider.value = presetMode.bloomLevel;
        BloomLevel(presetMode.bloomLevel);
        particlesSlider.value = presetMode.particlesLevel;
        SetSoundDropdown(presetMode.audioClip);
        UpdateBackgroundColour();
        FillNoteTextBoxes();
        FillBPMTextBoxes();
        timeOffsetInputField.SetFloatValue(presetMode.timeOffset);

        minimalModeMenu.rhythmSpeedSlider.value = noteBPMDefinitions.GetArrayContents(0) * 4;

        minimalModeMenu.chordsSpeedSlider.value = (int)Mathf.Clamp(noteBPMDefinitions.GetArrayContents(0) / (float)presetMode.chordBPM, 1,8);



        minimalModeMenu.numberOfNotesSlider.value = numPendulumsInputField.GetIntValue();
        minimalModeMenu.numberOfChordsSlider.value = numChordsInputField.GetIntValue();


        if (presetMode.spacingConverge == -1 && presetMode.spacingDiverge == -1)
        {
            minimalModeMenu.SetBPMEditMenu(presetMode.numPendulums, presetMode.noteBPMs);
            minimalModeMenu.CustomBPMNotes();
        }
        else
        {
            minimalModeMenu.rhythmConvergeSlider.value = presetMode.spacingConverge;
            minimalModeMenu.rhythmDivergeSlider.value = presetMode.spacingDiverge;
            if (presetMode.spacingConverge == 0)
            {
                minimalModeMenu.DivergeChanged(0);
            }
            else
            {
                minimalModeMenu.ConvergeChanged(0);
            }

        }

        if (presetMode.noteComplexity == -1)
        {
            minimalModeMenu.CustomPitchNotes();
        }
        else
        {
            minimalModeMenu.noteComplexitySlider.value = presetMode.noteComplexity;
            minimalModeMenu.NotesChanged(0);

        }

        if (presetMode.chordComplexity == -1)
        {
            minimalModeMenu.CustomChordProgression();
        }
        else
        {
            minimalModeMenu.chordComplexitySlider.value = presetMode.chordComplexity;
            minimalModeMenu.ChordsChanged(0);

        }

        presetUnsaved = false;
        saveListener.PresetUnsaved(presetUnsaved);
        PreviewSimulation("");
        minimalModeMenu.SyncAdvancedToMinimal();

    }

    public void NewPreset()
    {
        presetDropdown.value = -1;
    }

    public void SavePreset(bool saveAs)
    {
        if (newPreset && !saveAs)
        {
            saveAsButton.onClick.Invoke();
            return;
        }
        // Create a new instance of PresetMode
        PresetMode presetModeInstance = new PresetMode();

        string presetName = presetNameInputField.text;
        if (!saveAs)
        {
            presetName = presetDropdown.options[curPresetIndex].text;
        }

        presetModeInstance.name = presetName;
        // Set the values for the fields
        presetModeInstance.numPendulums = numPendulumsInputField.GetIntValue();
        presetModeInstance.timeOffset = timeOffsetInputField.GetFloatValue();
        presetModeInstance.bpmLoopEnable = bpmLoopEnableToggle.isOn;
        presetModeInstance.noteBPMRepeat = repeatBPMPatternInputField.GetIntValue();

        presetModeInstance.notes = notePitchDefinitions.GetArrayContents();
        presetModeInstance.noteBPMs = noteBPMDefinitions.GetBPMArrayContents();
        presetModeInstance.numOfChords = numChordsInputField.GetIntValue();
        presetModeInstance.chordBPM = chordBPMInputField.GetFloatValue();
        presetModeInstance.chords = chordPitchDefinitions.GetArrayContents();
        presetModeInstance.rootBPM = noteBPMDefinitions.GetBPMArrayContents(0);
        presetModeInstance.noteOctave = noteOctaveInputField.GetIntValue();
        presetModeInstance.bassOctave = bassOctaveInputField.GetIntValue();
        presetModeInstance.globalPitchMod = pitchMultiplierSlider.value;
        presetModeInstance.pendulumColour = pendulumColourButton.GetColour();
        presetModeInstance.bassColour = bassColourButton.GetColour();
        presetModeInstance.backgroundColour = backgroundColourButton.GetColour();
        presetModeInstance.scaleDegrees = scalePickerMenu.GetScale();

        presetModeInstance.pitchType = pitchType;

        if (pendulumMotionToggle.isOn) presetModeInstance.motionType = MotionType.PendulumMotion;
        else if (linearMotionToggle.isOn) presetModeInstance.motionType = MotionType.LinearMotion;
        else if (circularMotionToggle.isOn) presetModeInstance.motionType = MotionType.CircularMotion;
        else presetModeInstance.motionType = MotionType.BlackHoleMotion;

        if (circularShapeToggle.isOn) presetModeInstance.shapeType = ShapeType.Circle;
        else presetModeInstance.shapeType = ShapeType.Rectangle;

        presetModeInstance.bounce = bounceMotionToggle.isOn;


        presetModeInstance.reverbLevel = reverbSlider.value;
        presetModeInstance.filterLevel = filterSlider.value;
        presetModeInstance.bloomLevel = bloomSlider.value;
        presetModeInstance.particlesLevel = particlesSlider.value;
        presetModeInstance.audioClip = selectedAudioClipName;

        if (MinimalUIController.instance.GetCustomPitchNotesEnb())
        {
            presetModeInstance.noteComplexity = -1;
        }
        else
        {
            presetModeInstance.noteComplexity = (int)MinimalUIController.instance.noteComplexitySlider.value;
        }

        if (MinimalUIController.instance.GetCustomChordProgressionEnb())
        {
            presetModeInstance.chordComplexity = -1;
        }
        else
        {
            presetModeInstance.chordComplexity = (int)MinimalUIController.instance.chordComplexitySlider.value;
        }

        if (MinimalUIController.instance.GetCustomBPMNotesEnb())
        {
            presetModeInstance.spacingConverge = -1;
            presetModeInstance.spacingDiverge = -1;
        }
        else
        { 
            if (MinimalUIController.instance.convergeMode)
            {
                presetModeInstance.spacingConverge = (int)MinimalUIController.instance.rhythmConvergeSlider.value;
                presetModeInstance.spacingDiverge = 0;

            }
            else
            {
                presetModeInstance.spacingConverge = (int)0;
                presetModeInstance.spacingDiverge = (int)MinimalUIController.instance.rhythmDivergeSlider.value;

            }
        }

        // You can optionally save the asset to the project
        // This is useful if you want to create and use it as a permanent asset
        //AssetDatabase.CreateAsset(presetModeInstance, "Assets/Presets/" + presetName + ".asset");

        ExportPreset(presetModeInstance);

        presetUnsaved = false;


        // Don't forget to call AssetDatabase.SaveAssets(); after creating assets if needed
        if (saveAs)
        {
            PopulateDropdown(presetName);
        }

        selectedPresetMode = presetModeInstance;

        saveListener.PresetUnsaved(presetUnsaved);

    }

    public bool CheckTimeUnsaved( )
    {

        if (timeOffsetInputField.GetFloatValue() != (selectedPresetMode.timeOffset))
        {
            return true;
        }
        return false;
    }

    public bool CheckPresetUnsaved()
    {


        if (pitchType != selectedPresetMode.pitchType ||
        numPendulumsInputField.GetIntValue() != (selectedPresetMode.numPendulums) ||
        //timeOffsetInputField.GetFloatValue() != (selectedPresetMode.timeOffset) ||
        numChordsInputField.GetIntValue() != (selectedPresetMode.numOfChords) ||
        chordBPMInputField.GetFloatValue() != (selectedPresetMode.chordBPM) ||
        noteOctaveInputField.GetIntValue() != (selectedPresetMode.noteOctave) ||
        bassOctaveInputField.GetIntValue() != (selectedPresetMode.bassOctave) ||
        pitchMultiplierSlider.value != (selectedPresetMode.globalPitchMod) ||
        pendulumMotionToggle.isOn != (selectedPresetMode.motionType == MotionType.PendulumMotion) ||
        linearMotionToggle.isOn != (selectedPresetMode.motionType == MotionType.LinearMotion) ||
        blackholeMotionToggle.isOn != (selectedPresetMode.motionType == MotionType.BlackHoleMotion) ||
        circularMotionToggle.isOn != (selectedPresetMode.motionType == MotionType.CircularMotion) ||
        circularShapeToggle.isOn != (selectedPresetMode.shapeType == ShapeType.Circle) ||
        rectangleShapeToggle.isOn != (selectedPresetMode.shapeType == ShapeType.Rectangle) ||
        bounceMotionToggle.isOn != (selectedPresetMode.bounce) ||
        pendulumColourButton.GetColour() != (selectedPresetMode.pendulumColour) ||
        bassColourButton.GetColour() != (selectedPresetMode.bassColour) ||
        backgroundColourButton.GetColour() != (selectedPresetMode.backgroundColour) ||
        reverbSlider.value != selectedPresetMode.reverbLevel ||
        filterSlider.value != selectedPresetMode.filterLevel || 
        particlesSlider.value != selectedPresetMode.particlesLevel ||
        bloomSlider.value != selectedPresetMode.bloomLevel ||
        selectedAudioClipName != selectedPresetMode.audioClip)
        {
            Debug.Log("params unsaved");
            return true;
        }

        int loopPoint = notePitchDefinitions.GetArrayLength();
        for (int i = 0; i < loopPoint; i++)
        {
            if (notePitchDefinitions.GetArrayContents(i) != selectedPresetMode.notes[i])
            {
                Debug.Log("note pitch unsaved");

                return true;
            }
        }
        loopPoint = noteBPMDefinitions.GetArrayLength();
        for (int i = 0; i < loopPoint; i++)
        {
            if (Math.Abs(noteBPMDefinitions.GetBPMArrayContents(i) - selectedPresetMode.noteBPMs[i]) > 0.0001d)
            {
                Debug.Log("bpms unsaved   " + noteBPMDefinitions.GetBPMArrayContents(i)+"    "+ (double)selectedPresetMode.noteBPMs[i]);

                return true;
            }
        }

        for (int i = 0; i < chordPitchDefinitions.GetArrayLength(); i++)
        {
            if (chordPitchDefinitions.GetArrayContents(i) != selectedPresetMode.chords[i])
            {
                Debug.Log("chords unsaved   " + chordPitchDefinitions.GetArrayContents(i) + "    " + selectedPresetMode.chords[i]);

                return true;
            }
        }

        bool[] presetScalePicker = scalePickerMenu.GetScale();
        for (int i = 0; i < presetScalePicker.Length; i++)
        {
            if (presetScalePicker[i] != selectedPresetMode.scaleDegrees[i])
            {
                return true;
            }
        }


        return false;

    }

    public void SavePreset()
    {
        SavePreset(true);
    }

    private void FillNoteTextBoxes(string newValue)
    {
        FillNoteTextBoxes();
    }
    private void FillNoteTextBoxes(bool newValue)
    {
        FillNoteTextBoxes();
    }

    private void FillBPMTextBoxes(string newValue)
    {
        FillBPMTextBoxes();
    }
    private void FillBPMTextBoxes(bool newValue)
    {
        FillBPMTextBoxes();
    }

    public void FillNoteTextBoxes()
    {
        int repeatPattern = repeatPitchPatternInputField.GetIntValue();
        if (!loopEnableToggle.isOn)
        {
            repeatPattern = 100;
        }
        int pitchShift = repeatPitchShiftInputField.GetIntValue();
        numberOfPendulums = numPendulumsInputField.GetIntValue();

        ReadPitchInputBoxes();

        notePitchDefinitions.InputChanged(repeatPattern, pitchShift, numberOfPendulums);

        tabNavigation.FindSelectableUIElements();
    }

    public void FillBPMTextBoxes()
    {
        int repeatPattern = repeatBPMPatternInputField.GetIntValue();
        if (!bpmLoopEnableToggle.isOn)
        {
            repeatPattern = 100;
        }
        numberOfPendulums = numPendulumsInputField.GetIntValue();
        
        ReadPitchInputBoxes();

        noteBPMDefinitions.BPMInputChanged(repeatPattern, numberOfPendulums);

        tabNavigation.FindSelectableUIElements();

        PreviewSimulation("");
    }

    private void FillChordTextBoxes(string newValue)
    {
        FillChordTextBoxes();
    }

    public void FillChordTextBoxes()
    {
        
        numberOfChords = numChordsInputField.GetIntValue();
        chordPitchDefinitions.InputChanged(8, 0, numberOfChords);

        tabNavigation.FindSelectableUIElements();
    }

    public void RecalculateNotePitchNames()
    {
        FillNoteTextBoxes();
        FillChordTextBoxes();
    }

    private void ReadPitchInputBoxes()
    {
        pendulumScaleDegrees = notePitchDefinitions.GetArrayContents();
        chordScaleDegrees = chordPitchDefinitions.GetArrayContents();
        pendulumBPMs = noteBPMDefinitions.GetBPMArrayContents();
    }

    public void UpdateBackgroundColour()
    {
        Camera.main.backgroundColor = (backgroundColourButton.GetColour());
    }

    public double GetTimeOffset()
    {
        if (gameObject.activeInHierarchy)
        {
            timeOffset = timeOffsetInputField.GetDoubleValue();
        }
        else
        {
            if (pendulums.Count > 0)
            {
                timeOffset = pendulums[0].GetTimeOffset();
            }
            else if (chordPendulums.Count > 0)
            {
                timeOffset = chordPendulums[0].GetTimeOffset();
            }
        }
        return timeOffset;
    }

    public void SetTimeOffset(double timeOffset)
    {
        this.timeOffset = timeOffset;
    }

    private bool IsTextInputFocused()
    {
        // Check if there is a current selected object (focused UI element).
        GameObject selectedObject = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;

        // If there is a selected object, check if it's an InputField.
        if (selectedObject != null)
        {
            TMP_InputField selectedInputField = selectedObject.GetComponent<TMP_InputField>();
            
            return selectedInputField != null;
        }

        // No selected object or not an InputField.
        return false;
    }

    public void ExportPreset(PresetMode preset)
    {
        if (!Directory.Exists(presetsFolderPath))
        {
            Directory.CreateDirectory(presetsFolderPath);
        }

        string filePath = presetsFolderPath + "/" + preset.name + ".json";

        string json = JsonUtility.ToJson(preset);
        File.WriteAllText(filePath, json);

        Debug.Log("Preset exported to: " + filePath);

    }

    public List<PresetMode> LoadPresets()
    {
        List<PresetMode> loadedPresets = new List<PresetMode>();

        if (!Directory.Exists(presetsFolderPath))
        {
            Debug.LogError("Presets folder does not exist.");
            return loadedPresets;
        }

        string[] jsonFiles = Directory.GetFiles(presetsFolderPath, "*.json");
        sortedFiles = jsonFiles.OrderByDescending(f => new FileInfo(f).LastWriteTime);

        foreach (string file in sortedFiles)
        {
            string jsonData = File.ReadAllText(file);
            PresetMode loadedPreset = JsonUtility.FromJson<PresetMode>(jsonData);
            string fileName = Path.GetFileNameWithoutExtension(file);
            loadedPreset.name = fileName;

            loadedPresets.Add(loadedPreset);
        }

        return loadedPresets;
    }

}
