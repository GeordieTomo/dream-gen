using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class CircularSlider : Slider
{

    private Vector2 previousMousePosition;
    public TMP_Text valueText;

    private float curTracking;
    private float prevValue;

    private bool cancelDrag = false;
    
    protected override void Awake()
    {
        base.Awake();
        valueText = GetComponentInChildren<TMP_Text>();
        // Set up any additional initialization code here
        DisplayValueInCenter();
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        cancelDrag = false;

        previousMousePosition = eventData.position;
        curTracking = 0;
        prevValue = value;
        //base.OnPointerDown(eventData);
        UpdateSliderValue(eventData.position);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!cancelDrag)
        {
            //base.OnDrag(eventData);
            UpdateSliderValue(eventData.position);
            DisplayValueInCenter();
        }
    }

    private void UpdateSliderValue(Vector2 inputPosition)
    {
        
        Vector2 inputVector = inputPosition - previousMousePosition;
        float inputAmount = inputVector.x + inputVector.y;

        curTracking += inputAmount;

        value = prevValue + curTracking * (maxValue - minValue) / 300;
       
        // Trigger the value changed event
        onValueChanged.Invoke(value);

        previousMousePosition = inputPosition;

    }

    public void CancelDrag()
    {
        cancelDrag = true;
    }

    private void DisplayValueInCenter()
    {
        if (valueText != null)
        {
            // Display the value in the center of the dial
            valueText.text = value.ToString();
        }
    }

    public override float value
    {
        get { return base.value; }
        set
        {
            base.value = value;
            DisplayValueInCenter();
        }
    }
}
