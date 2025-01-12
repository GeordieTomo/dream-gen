using UnityEngine;
using System.IO;
//using AnotherFileBrowser.Windows;

public class SampleLoader : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void LoadAudioFile()
    {
        /*var browserProperties = new BrowserProperties();
        browserProperties.filter = "Audio Files|*.wav;*.mp3";

        new FileBrowser().OpenFileBrowser(browserProperties, path =>
        {
            //Do something with path(string)
            Debug.Log(path);
        });*/
    }

    private void LoadAudioClip(string filePath)
    {
        if (File.Exists(filePath))
        {
            // Load the audio clip and assign it to the AudioSource
            //AudioClip audioClip = WavUtility.ToAudioClip(filePath); // You'll need a utility to convert WAV/MP3 to AudioClip
            //audioSource.clip = audioClip;
        }
        else
        {
            Debug.LogError("File does not exist: " + filePath);
        }
    }
}

