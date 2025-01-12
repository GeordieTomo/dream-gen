using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MovablePanel : MonoBehaviour
{

    public RectTransform rectTransform;

    private Vector2 prevMousePos;
    private Vector2 initPosition;

    [SerializeField] private bool anchorBottomRight = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            prevMousePos = Input.mousePosition;
        }
    }

    public void OnMouseDown()
    {
        transform.parent.SetAsLastSibling();
        prevMousePos = Input.mousePosition;

    }


    public void OnMouseDrag()
    {


        Vector2 mouseDelta = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - prevMousePos;
        Vector2 newPos = rectTransform.anchoredPosition + mouseDelta;

        float screenHeight = 1080f;
        float screenWidth = 1920f;
        float ratio = (float)Screen.width / (float)Screen.height;

        if (ratio > screenWidth / screenHeight)
        {
            screenWidth = ratio * screenHeight;
        }
        else
        {
            screenHeight = screenWidth / ratio;
        }

        Debug.Log(screenWidth);

        if (anchorBottomRight)
        {
            newPos.x = Mathf.Clamp(newPos.x, -screenWidth + rectTransform.rect.width, 0);
            newPos.y = Mathf.Clamp(newPos.y, 0, screenHeight - rectTransform.rect.height - 40);
        }
        else
        {
            newPos.x = Mathf.Clamp(newPos.x, 0, screenWidth - rectTransform.rect.width);
            newPos.y = Mathf.Clamp(newPos.y, 0, screenHeight - rectTransform.rect.height - 40);
        }


        rectTransform.anchoredPosition = newPos;
        prevMousePos = Input.mousePosition;
    }

    public void ResetPosition() 
    { 
        // allow panels to stay where left by user
    }
}
