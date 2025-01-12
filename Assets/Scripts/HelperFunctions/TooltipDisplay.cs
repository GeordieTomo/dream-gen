using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TooltipDisplay : MonoBehaviour
{
    public TMP_Text textDisplay;
    private bool textEmpty = true;

    public void Awake()
    {
        ClearText();
    }

    public void SetText(string text)
    {
        textDisplay.text = text;
        textEmpty = false;
    }

    public void ClearText()
    {
        textDisplay.text = "Info View";
        textEmpty = true;
    }

    public bool IsEmpty()
    {
        return textEmpty;
    }
}
