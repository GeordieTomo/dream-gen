using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;

public class TimeScaleTracker : MonoBehaviour
{

    public bool trackTime = false;

    TMP_Text textDisplay;
    PendulumGenerator pendulumGenerator;

    // Start is called before the first frame update
    void Start()
    {
        textDisplay = GetComponent<TMP_Text>();
        pendulumGenerator = FindFirstObjectByType<PendulumGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (trackTime)
        {
            textDisplay.text = (pendulumGenerator.GetTimeOffset()).ToString("F2");
        }
        else
        {
            textDisplay.text = Time.timeScale.ToString("F2");
        }
    }
}
