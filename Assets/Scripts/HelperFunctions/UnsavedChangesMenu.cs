using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UnsavedChangesMenu : MonoBehaviour
{
    public Button[] cancelButtons;
    public Button saveButton;
    public Button dontSaveButton;

    public enum UnsavedMenuState
    {
        MenuOpen,
        Save,
        DontSave,
        Cancel
    }

    public UnsavedMenuState state = UnsavedMenuState.MenuOpen;

    public void Start()
    {
        cancelButtons[0].onClick.AddListener(Cancel);
        cancelButtons[1].onClick.AddListener(Cancel);
        saveButton.onClick.AddListener(Save);
        dontSaveButton.onClick.AddListener(DontSave);
    }

    private void Cancel()
    {
        state = UnsavedMenuState.Cancel;
    }

    private void Save()
    {
        state = UnsavedMenuState.Save;
    }

    private void DontSave()
    {
        state = UnsavedMenuState.DontSave;
    }

    public void Open()
    {
        state = UnsavedMenuState.MenuOpen;
    }

    public UnsavedMenuState State()
    {
        return state;
    }
}
