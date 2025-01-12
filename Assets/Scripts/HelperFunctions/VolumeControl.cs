using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    private Slider volumeSlider;

    private const string PlayerPrefsKey = "GameVolume";

    public GameObject configMenu;

    private void Awake()
    {
        volumeSlider = GetComponent<Slider>();
    }

    void Start()
    {
        // Load the saved volume level from PlayerPrefs
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            float savedVolume = PlayerPrefs.GetFloat(PlayerPrefsKey);
            volumeSlider.value = savedVolume * 100;
            SetVolume(savedVolume);
        }
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        configMenu.SetActive(false);
    }

    public void OnVolumeChanged(float newValue)
    {
        // Called when the slider value changes
        float volume = newValue / 100;
        SetVolume(volume);

        // Save the volume level to PlayerPrefs
        PlayerPrefs.SetFloat(PlayerPrefsKey, volume);
        PlayerPrefs.Save();
    }

    private void SetVolume(float volume)
    {
        // Set the global volume using AudioListener
        AudioListener.volume = volume;
    }
}
