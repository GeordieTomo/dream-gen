using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuListener : MonoBehaviour
{
    public GameObject mainMenu;

    public PendulumGenerator pendulumGenerator;

    private void Update()
    {
        if (!mainMenu.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
            {
                OpenMenu();
            }

            if (Input.GetKeyDown(KeyCode.Space) ||  Input.GetKeyDown(KeyCode.P))
            {
                pendulumGenerator.SetTimeOffset();

                pendulumGenerator.TogglePlayState();
            }
        }
    }

    public void OpenMenu()
    {
        pendulumGenerator.SetTimeOffset();
        mainMenu.SetActive(true);
    }


}
