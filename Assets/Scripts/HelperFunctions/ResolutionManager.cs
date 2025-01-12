using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown windowedDropdown;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;

    private RefreshRate currentRefreshRate;
    private int currentResolutionIndex = 0;
    private bool currentlyFullscreen = true;


    private void Awake()
    {
        resolutionDropdown = GetComponent<TMP_Dropdown>();
    }

    // Start is called before the first frame update
    void Start()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio;

        for (int i = 0; i < resolutions.Length; i++) {
            if (resolutions[i].refreshRateRatio.ToString() == currentRefreshRate.ToString())
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height;
            options.Add(resolutionOption);
            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, currentlyFullscreen);
    }

    public void SetWindowed(int windowedIndex)
    {
        Resolution resolution = filteredResolutions[currentResolutionIndex];
        currentlyFullscreen = windowedIndex == 1;
        Screen.SetResolution(resolution.width, resolution.height, currentlyFullscreen);
    }

}
