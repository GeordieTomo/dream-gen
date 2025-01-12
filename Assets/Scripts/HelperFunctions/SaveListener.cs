using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveListener : MonoBehaviour
{

    [SerializeField] private Button saveButton;
    public Button resetButton;
    private bool presetUnsaved = false;


    private void Update()
    {
        if (PendulumGenerator.instance.HotkeysActive() && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                // Perform the save operation or call the function you want
                SaveFunction();
            }
        }
    }

    public void SaveFunction()
    {
        if (presetUnsaved) {

            saveButton.onClick.Invoke();
        }
    }

    public void PresetUnsaved(bool presetUnsaved)
    {
        this.presetUnsaved = presetUnsaved;

        saveButton.interactable = presetUnsaved;
        resetButton.interactable = presetUnsaved;
    }
}
