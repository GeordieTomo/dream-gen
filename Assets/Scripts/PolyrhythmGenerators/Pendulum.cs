using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public enum MotionType
{
    PendulumMotion,
    LinearMotion,
    CircularMotion,
    BlackHoleMotion
}

public enum ShapeType
{
    Circle,
    Rectangle
}

public enum PitchType {
    Semitone,
    ScaleDegree,
    JustIntonation
}

public class Pendulum : MonoBehaviour
{

    public double curBPM = 4.5f;
    private double curPeriod;
    public float pitch = 1.0f;
    public int scaleDegreeNote = 0;
    private int index = 0;
    private int numberOfPendulums = 0;
    private int numberOfChords = 0;
    private int totalPendulums = 0;
    private float scale;

    public float maxAngle = 60f;

    public float startingPosition = -1;
    public float startingPosRadians;

    private double triggerPosition = 0f;
    public double triggerOffset = 0f;

    private float particleEffectAmount = 30f;

    private double currentPosition;
    private double currentPositionSinusoidal;
    private double currentPositionCosinusoidal;

    private AudioSource[] audioSources;
    private int curAudioSource = 0;
    private float initVolume;

    private Renderer pendulum;

    public GameObject spherePrefab;
    public GameObject cubePrefab;
    public GameObject particlesPrefab;

    private Color pendulumColour;

    public Color pendulumGlow = new Color(0,0,0);

    private float currentEmission = 1f;

    MotionType motionType = MotionType.PendulumMotion;
    MotionType prevMotionType = MotionType.PendulumMotion;
    ShapeType shapeType = ShapeType.Circle;
    ShapeType prevShape = ShapeType.Circle;

    public bool bounceMotion = false;

    public bool isChord = false;

    private double minBPM = 1f;
    private double maxBPM = 1f;

    private double timeOffset = 0f;

    private bool rainbowHue = false;

    Transform child;

    public PendulumGenerator pendulumGenerator;

    private bool destroy = false;

    private bool preview = false;
    private bool cancelBloom = false;

    public void Initialise(double BPM, float pitch, int scaleDegree, double minBPM, double maxBPM, bool bounce, bool linear, bool rectangle, bool circularMotion, bool blackholeMotion, double timeOffset, int index, int numberOfPendulums, int numberOfChords, bool isChord, Color colour, float particleEffectsAmount,bool preview,bool isNewPendulum, PendulumGenerator pendulumGenerator)
    {
        curBPM = BPM;
        this.pitch = pitch;
        scaleDegreeNote = scaleDegree;
        this.minBPM = minBPM;
        this.maxBPM = maxBPM;
        bounceMotion = bounce;

        if (linear)
            this.motionType = MotionType.LinearMotion;
        else if (circularMotion)
            this.motionType = MotionType.CircularMotion;
        else if (blackholeMotion)
            this.motionType = MotionType.BlackHoleMotion;
        else
            motionType = MotionType.PendulumMotion;

        if (rectangle)
            shapeType = ShapeType.Rectangle;
        else
            shapeType = ShapeType.Circle;

        this.timeOffset = timeOffset;
        this.index = index;
        SetNumberOfPendulums(numberOfPendulums, numberOfChords);
        this.isChord = isChord;
        this.pendulumGenerator = pendulumGenerator;
        this.pendulumGlow = colour;
        this.particleEffectAmount = particleEffectsAmount;

        if (colour.r == 0 && colour.g == 0 && colour.b == 1f / 255f)
        {
            rainbowHue = true;
        }

        this.preview = preview;

        if (preview )
            Reset();
        else
        {
            LiveUpdate();
            if (isNewPendulum || colour != pendulumColour / currentEmission)
                ResetColour();
        }

    }

    private void LiveUpdate()
    {
        // 60BPM = 1perSecond
        // period = 60/BPM
        curPeriod = 60 / curBPM;

        if (isChord)
        {
            double chordStagger = curPeriod / numberOfChords * (index - numberOfPendulums);
            timeOffset -= chordStagger;
            //pendulumGlow = new Color(0.1f, 0, 1);
        }
        currentPosition = (timeOffset);


        // how many double periods fit into the current position (this method is used to avoid integer overflow and keep precision for high values of time)
        int numFit = (int)(currentPosition / 120 * curBPM);
        if (currentPosition < 0)
        {
            numFit--;
        }

        currentPosition -= numFit / curBPM * 120;

        triggerPosition = currentPosition >= 60 / curBPM ? 120 / curBPM : 60 / curBPM;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        scale = System.Convert.ToSingle(Map(1 / curBPM, 1 / maxBPM, 1 / minBPM, 1, 2.25f));

        // if shape changed- generate new shape
        if (pendulum == null || shapeType != prevShape)
        {
            if (pendulum != null)
            {
                Destroy(pendulum);
            }

            switch (shapeType)
            {
                case ShapeType.Rectangle:
                    child = Instantiate<GameObject>(cubePrefab, transform).transform;
                    break;

                case ShapeType.Circle:
                default:
                    child = Instantiate<GameObject>(spherePrefab, transform).transform;
                    break;
            }

            pendulum = child.GetComponent<Renderer>();

            ResetColour();

            prevShape = shapeType;
        }


        switch (shapeType)
        {
            case ShapeType.Rectangle:
                child.position = new Vector3(0, (scale * 2f));
                child.localScale = new Vector3(0.5f, scale, 1);
                break;

            case ShapeType.Circle:
            default:
                child.position = new Vector3(0, (scale * 2));
                child.localScale = new Vector3(scale / 5, scale / 5, scale / 5);
                break;
        }



        //if (prevMotionType != motionType)
        {
            switch (motionType)
            {
                case MotionType.LinearMotion:
                    break;

                case MotionType.CircularMotion:
                    child.position = new Vector3(0, Map(index, 0, totalPendulums - 1, 1, 4.5f));
                    break;

                case MotionType.BlackHoleMotion:
                    child.position = new Vector3(0, 4, 0);
                    transform.rotation = Quaternion.Euler(0f, 0f, Map(index, 0, totalPendulums, 0, 360f));
                    break;

                case MotionType.PendulumMotion:
                default:
                    child.position = new Vector3(0, Map(index, 0, totalPendulums - 1, 1, 9));
                    transform.position = new Vector3(0, 5, 0);
                    break;
            }
            prevMotionType = motionType;
        }

        if (audioSources != null)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.pitch = pitch;
                audioSource.clip = pendulumGenerator.selectedAudioClip;
            }
        }

        //ResetColour();

        if (!preview)
            UpdateMotion(Time.deltaTime);
        else
        {
            UpdateMotion(0);

        }

    }

    private void Reset()
    {

        cancelBloom = true;
        // 60BPM = 1perSecond
        // period = 60/BPM
        curPeriod = 60 / curBPM;

        if (isChord)
        {
            double chordStagger = curPeriod / numberOfChords * (index - numberOfPendulums);
            timeOffset -= chordStagger;
            //pendulumGlow = new Color(0.1f, 0, 1);
        }
        currentPosition = (timeOffset);


        // how many double periods fit into the current position (this method is used to avoid integer overflow and keep precision for high values of time)
        int numFit = (int)(currentPosition / 120 * curBPM);
        if (currentPosition < 0)
        {
            numFit--;
        }

        currentPosition -= numFit / curBPM * 120;

        triggerPosition = currentPosition >= 60 / curBPM ? 120 / curBPM : 60 / curBPM;

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;

        scale = System.Convert.ToSingle(Map(1 / curBPM, 1 / maxBPM, 1 / minBPM, 1, 2.25f));

        // if shape changed- generate new shape
        if (pendulum == null || shapeType != prevShape)
        {
            if (pendulum != null)
            {
                Destroy(pendulum);
            }

            switch (shapeType)
            {
                case ShapeType.Rectangle:
                    child = Instantiate<GameObject>(cubePrefab, transform).transform;
                    break;

                case ShapeType.Circle:
                default:
                    child = Instantiate<GameObject>(spherePrefab, transform).transform;
                    break;
            }

            pendulum = child.GetComponent<Renderer>();

            prevShape = shapeType;
        }


        switch (shapeType)
        {
            case ShapeType.Rectangle:
                child.position = new Vector3(0, (scale * 2f));
                child.localScale = new Vector3(0.5f, scale, 1);
                break;

            case ShapeType.Circle:
            default:
                child.position = new Vector3(0, (scale * 2));
                child.localScale = new Vector3(scale / 5, scale / 5, scale / 5);
                break;
        }



        //if (prevMotionType != motionType)
        {
            switch (motionType)
            {
                case MotionType.LinearMotion:
                    break;

                case MotionType.CircularMotion:
                    child.position = new Vector3(0, Map(index, 0, totalPendulums - 1, 1, 4.5f));
                    break;

                case MotionType.BlackHoleMotion:
                    child.position = new Vector3(0, 4, 0);
                    transform.rotation = Quaternion.Euler(0f, 0f, Map(index, 0, totalPendulums, 0, 360f));
                    break;

                case MotionType.PendulumMotion:
                default:
                    child.position = new Vector3(0, Map(index, 0, totalPendulums - 1, 1, 9));
                    transform.position = new Vector3(0, 5, 0);
                    break;
            }
            prevMotionType = motionType;
        }

        if (audioSources != null)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.pitch = pitch;
                audioSource.clip = pendulumGenerator.selectedAudioClip;
            }
        }

        ResetColour();

        if (!preview)
            UpdateMotion(Time.deltaTime);
        else
        {
            UpdateMotion(0);

        }
    }

    private void Start()
    {

        audioSources = GetComponents<AudioSource>();

        initVolume = audioSources[0].volume;

    }

    void Update()
    {
        if (!destroy && !preview)
        {
            float deltaTime = Time.deltaTime;
            UpdateMotion(deltaTime);

            if (index == 0)
            {
                timeOffset += deltaTime;
            }

            if (currentPosition >= triggerPosition)
            {
                if (triggerPosition == 60 / curBPM)
                {
                    triggerPosition = 120 / curBPM;
                }
                else
                {
                    triggerPosition = 60 / curBPM;
                    currentPosition -= 120 / curBPM;
                }

                PlaySound();

                StartCoroutine(PlayVisual());

            }
        }
    }

    private void UpdateMotion(float deltaTime)
    {
        currentPosition += (deltaTime);
        currentPositionSinusoidal = System.Math.Sin(currentPosition / curPeriod * Mathf.PI);
        currentPositionCosinusoidal = System.Math.Cos(currentPosition / curPeriod * Mathf.PI);

        if (rainbowHue)
        {
            float hue = System.Convert.ToSingle((currentPosition / curPeriod) % 1d);

            // Set the HSV color
            pendulumGlow = Color.HSVToRGB(hue, 1f, 1f);

            pendulumColour = pendulumGlow * currentEmission;
            pendulum.material.SetColor("_EmissionColor", pendulumColour);
        }

        if (bounceMotion)
        {
            currentPositionSinusoidal = Map(System.Math.Abs(currentPositionSinusoidal), 0, 1, -1, 1);
            currentPositionCosinusoidal = Map(System.Math.Abs(currentPositionCosinusoidal), 0, 1, -1, 1);
        }

        switch (motionType)
        {
            case MotionType.LinearMotion:
                child.position = new Vector3(Map(index, 0, totalPendulums - 1, -8, 8), System.Convert.ToSingle(Map(currentPositionSinusoidal, -1, 1, -4.5f + scale / 2, 4.5f - scale / 2)));
                break;

            case MotionType.CircularMotion:
                if (bounceMotion)
                {
                    if (System.Math.Round(currentPosition / curPeriod - 0.5f) % 2 == 0)
                    {
                        currentPositionSinusoidal = currentPosition / curPeriod * 360;
                    }
                    else
                    {
                        currentPositionSinusoidal = -currentPosition / curPeriod * 360;
                    }
                }
                else
                {
                    currentPositionSinusoidal = currentPosition / curPeriod * 180;
                }

                transform.rotation = Quaternion.Euler(0f, 0f, System.Convert.ToSingle(180f + currentPositionSinusoidal));
                break;

            case MotionType.BlackHoleMotion:
                if (bounceMotion)
                    transform.localScale = Mathf.Pow(System.Convert.ToSingle(Map((currentPositionSinusoidal), -1, 1, 1, 0)),3) * Vector3.one;
                else
                    transform.localScale = Mathf.Pow(System.Convert.ToSingle((currentPositionCosinusoidal)),3) * Vector3.one;
                break;

            case MotionType.PendulumMotion:
            default:
                transform.rotation = Quaternion.Euler(0f, 0f, System.Convert.ToSingle(180f + currentPositionSinusoidal * maxAngle));
                break;
        }
    }

    void PlaySound()
    {
        pendulumGenerator.triggerDisplayControl.lastTrigger = this;

        if (isChord)
        {
            pendulumGenerator.SetPitchesFromBassNote(scaleDegreeNote);
        }

        audioSources[curAudioSource].volume = initVolume;
        audioSources[curAudioSource].Play();
        curAudioSource = (curAudioSource + 1) % audioSources.Length;
        StartCoroutine(FadeVolume(audioSources[curAudioSource]));

    }

    IEnumerator FadeVolume(AudioSource audioSource)
    {
        while(audioSource.volume > 0)
        {
            audioSource.volume -= Time.deltaTime * 0.01f;
            yield return new WaitForEndOfFrame();
        }
        audioSource.Stop();
    }

    IEnumerator PlayVisual()
    {

        cancelBloom = false;

        GameObject particlesObj = Instantiate(particlesPrefab, child.transform);
        ParticleSystem particles = particlesObj.GetComponent<ParticleSystem>();

        ShapeModule shapeModule = particles.shape;
        EmissionModule emissionModule = particles.emission;
        MainModule mainModule = particles.main;

        shapeModule.scale = new Vector3(child.localScale.x*2f, child.localScale.y * 2f, 1);
        emissionModule.rateOverTime = particleEffectAmount * 100f;
        //mainModule.startSpeed = new MinMaxCurve(0, 3);
        mainModule.startSize = new MinMaxCurve(0, 0.01f * MathUtil.Map(particleEffectAmount,0,100,0.5f,2));
        mainModule.startLifetime = new MinMaxCurve(5f * MathUtil.Map(particleEffectAmount, 0, 100,0.5f,3));

        ParticleSystemRenderer renderer = particlesObj.GetComponent<ParticleSystemRenderer>();

        // Access the material and modify its emission color property
        Material mat = renderer.material;
        mat.SetColor("_EmissionColor", pendulumGlow);

        particles.Play();   

        Destroy(particles.gameObject, particles.main.startLifetime.constant);

        currentEmission = 5f;
        pendulumColour = pendulumGlow * currentEmission;

        if (isChord)
        {
            pendulumGenerator.triggerDisplayControl.lastTrigger = this;
        }

        while (currentEmission >= 1f)
        {
            yield return new WaitForSeconds(0.01f);
            pendulum.material.SetColor("_EmissionColor", pendulumColour);
            currentEmission -= 0.05f;
            if (cancelBloom)
                currentEmission = 1f;

            pendulumColour = pendulumGlow * currentEmission;
        }

        currentEmission = 1f;
        pendulumColour = pendulumGlow * currentEmission;
        pendulum.material.SetColor("_EmissionColor", pendulumColour);

        yield return null;
    }

    public void ResetColour()
    {
        pendulum.material.SetColor("_EmissionColor", pendulumGlow);
    }

    public void SetMaxBPM(float newBPM)
    {
        maxBPM = newBPM;
    }

    public void SetMinBPM(float newBPM) {  minBPM = newBPM; }

    public void SetTimeOffset(double timeOffset)
    {
        this.timeOffset = timeOffset; 
    }

    public double GetTimeOffset()
    {
        return timeOffset;
    }

    public void SetIndex(int index)
    {
        this.index = index;
    }

    private void SetNumberOfPendulums(int numberOfPendulums, int numberOfChords)
    {
        this.numberOfPendulums = numberOfPendulums;
        this.numberOfChords = numberOfChords;
        totalPendulums = numberOfChords + numberOfPendulums;
    }

    public void SetPitch(float pitch)
    {
        this.pitch = pitch;
        if (audioSources != null)
        {
            foreach (AudioSource audioSource in audioSources)
            {
                audioSource.pitch = pitch;
            }
        }
    }

    public void Destroy()
    {
        destroy = true;
        StartCoroutine(FadeOutAndDestroy());
    }

    private IEnumerator FadeOutAndDestroy()
    {

        float maxVol = initVolume;
        while (maxVol > 0)
        {
            if (pendulum != null)
            {
                pendulum.enabled = false;
            }

            foreach (AudioSource audioSource in audioSources)
            {
                maxVol -= Time.deltaTime * 0.01f;
                audioSource.volume = Mathf.Min(maxVol, audioSource.volume);
                yield return new WaitForEndOfFrame();
            }
        }

        Destroy(this.gameObject);

    }

    public static float Map(float value, float inputMin, float inputMax, float outputMin, float outputMax)
    {
        float mappedValue = (outputMax + outputMin) / 2;
        
        if (inputMin != inputMax)
        {
            // Ensure the input value is clamped within the input range
            float clampedValue = Mathf.Clamp(value, inputMin, inputMax);

            // Map the clamped input value to the output range
            mappedValue = (clampedValue - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin;
        }

        return mappedValue;
    }
    public static double Map(double value, double inputMin, double inputMax, double outputMin, double outputMax)
    {

        double mappedValue = (outputMax + outputMin) / 2;

        if (inputMin < inputMax)
        {
            // Ensure the input value is clamped within the input range
            double clampedValue = System.Math.Clamp(value, inputMin, inputMax);

            // Map the clamped input value to the output range
            mappedValue = (clampedValue - inputMin) / (inputMax - inputMin) * (outputMax - outputMin) + outputMin;
        }

        return mappedValue;
    }

    public Color GetColour()
    {
        return pendulumColour;
    }

}
