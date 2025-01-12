using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpdateBuildVersion : MonoBehaviour
{
    private TMP_Text versionText;

    void Start()
    {
        versionText = GetComponent<TMP_Text>();

        if (versionText != null)
        {
            versionText.text = "v" + Application.version;
        }
        else
        {
            Debug.LogError("Please assign a TextMeshPro Text component to the 'versionText' field in the inspector.");
        }
    }
}

