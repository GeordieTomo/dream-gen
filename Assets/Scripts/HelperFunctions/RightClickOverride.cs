using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RightClickOverride : MonoBehaviour
{

    [SerializeField] public UnityEvent onRightClick;
    CircularSlider slider;

    private void Awake()
    {
        slider = GetComponent<CircularSlider>();
    }

    public void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(1))
        {
            onRightClick.Invoke();
            slider.CancelDrag();
        }
    }
    
}
