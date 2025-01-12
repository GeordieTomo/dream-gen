using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateDisplay : MonoBehaviour
{
    public Toggle verticalDisplay;

    public void Start()
    {
        verticalDisplay.onValueChanged.AddListener(RotateDisplayVertical);
    }

    private void RotateDisplayVertical(bool vertical)
    {
        if (vertical)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
            Camera.main.orthographicSize = 10;

        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            Camera.main.orthographicSize = 5;
        }
    }
}
