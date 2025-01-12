using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Unity.VisualScripting;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipText; // Text to display in the tooltip
    public TooltipDisplay display;

    public void OnPointerEnter(PointerEventData eventData)
    {
        display.SetText(tooltipText);
    }

    public void OnPointer(PointerEventData eventData)
    {
        if (display.IsEmpty())
            display.SetText(tooltipText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        display.ClearText();
    }

}
