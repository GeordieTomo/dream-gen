using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PauseButtonTrigger : MonoBehaviour
{
    private Button pauseButton;

    private void Start()
    {
        pauseButton = GetComponent<Button>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.P))
        {
            pauseButton.onClick.Invoke();
        }
    }
}
