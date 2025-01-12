using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAllButton : MonoBehaviour
{

    [SerializeField] private GameObject[] buttons;

    [SerializeField] private bool showButton = false;

    // Update is called once per frame
    void Update()
    {
        UpdateViewState();
    }

    public void UpdateViewState( )
    {
        bool allHidden = true;

        foreach (GameObject button in buttons)
        {
            if (!button.activeInHierarchy)
            {
                allHidden = false;
            }
        }
        if (allHidden)
        {
            if (showButton)
            {
                gameObject.SetActive(true);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
