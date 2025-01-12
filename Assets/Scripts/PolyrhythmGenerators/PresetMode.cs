using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PresetMode
{
    public string name = "New Preset";
    public int numPendulums = 15;
    public int spacingDiverge = -1;
    public int spacingConverge = -1;
    public int noteComplexity = -1;
    public int chordComplexity = -1;
    public float timeOffset = -1f;
    public List<int> notes = new List<int> { 1, 3, 5, 7, 8, 10, 12, 14, 15, 17, 19, 21, 22, 24, 26 };
    public bool bpmLoopEnable = true;
    public int noteBPMRepeat = 2;
    public List<double> noteBPMs = new List<double> { 15f, 15.15f, 15.3f, 15.45f, 15.6f, 15.75f, 15.9f, 16.05f, 16.2f, 16.35f, 16.5f, 16.65f, 16.8f, 16.95f, 17.1f };
    public int numOfChords = 2;
    public List<int> chords = new List<int> { 1, 4, 5, 6, 4, 5, 7, 6 };
    public double chordBPM = 15;
    public double rootBPM = 15;
    public float globalPitchMod = 1f;
    public int noteOctave = 2;
    public int bassOctave = 1;
    public bool[] scaleDegrees = { true, false, true, false, true, true, false, true, false, true, false, true };
    public PitchType pitchType = PitchType.ScaleDegree;
    public MotionType motionType = MotionType.CircularMotion;
    public ShapeType shapeType = ShapeType.Circle;
    public bool bounce = false;
    public Color pendulumColour = new Color(0, 0.23f, 0.75f);
    public Color bassColour = new Color(0.1f, 0, 1f);
    public Color backgroundColour = new Color(0.21f, 0.21f, 0.21f);
    public float reverbLevel = 45f;
    public float filterLevel = 60f;
    public float bloomLevel = 15f;
    public float particlesLevel = 30f;
    public string audioClip = "sawtooth";
}