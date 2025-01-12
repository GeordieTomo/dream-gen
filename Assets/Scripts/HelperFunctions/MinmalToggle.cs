using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinmalToggle : MonoBehaviour
{

    public static MinmalToggle instance;

    public GameObject minimalMode;
    public GameObject advancedMode;

    public bool minimalModeEnabled = true;

    Toggle minimalToggle;

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
        minimalToggle = GetComponent<Toggle>();
    }

    public void SwitchMode(bool minimal)
    {
        minimalModeEnabled = minimal;
        if (minimal)
        {
            minimalMode.SetActive(true); 
            advancedMode.SetActive(false);
        }
        else
        {
            minimalMode.SetActive(false); 
            advancedMode.SetActive(true);
        }
        minimalToggle.isOn = minimal;
    }

    public bool MinimalActive()
    {
        return minimalModeEnabled;
    }

}
