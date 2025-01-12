using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using System.Collections.Generic;
using System;

public class TabNavigation : MonoBehaviour
{

    private Selectable[] inputFieldsAuto;

    private float repeatTimer = 0f;

    private void Start()
    {
        FindSelectableUIElements();
    }

    public void FindSelectableUIElements()
    {
        inputFieldsAuto = FindObjectsByType<Selectable>(FindObjectsSortMode.None);

        inputFieldsAuto = inputFieldsAuto.OrderBy(field => GetScreenPosition(field.transform), new InputFieldPositionComparer()).ToArray();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SelectNextItem();
            repeatTimer = Time.time + 0.3f;
        } 
        else if (Input.GetKey(KeyCode.Tab) && Time.time >= repeatTimer)
        {
            SelectNextItem();
            repeatTimer = Time.time + 0.1f;
        }
    }

    private Vector2 GetScreenPosition(Transform transform)
    {
        // Calculate the screen position based on the RectTransform
        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        Vector2 center = (corners[0] + corners[2]) / 2f;

        return center;
    }

    private class InputFieldPositionComparer : IComparer<Vector2>
    {
        public int Compare(Vector2 p1, Vector2 p2)
        {
            // Compare the positions: left to right, top to bottom
            if (p1.y > p2.y || (p1.y == p2.y && p1.x < p2.x))
                return -1;
            if (p1.y < p2.y || (p1.y == p2.y && p1.x > p2.x))
                return 1;
            return 0;
        }
    }

    private void SelectNextItem()
    {
        int currentIndex = -1;

        if (EventSystem.current.currentSelectedGameObject != null)
        {

            Selectable current = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

            currentIndex = Array.IndexOf(inputFieldsAuto, current);


        }

        if (currentIndex >= -1)
        {

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                // Calculate the index of the next Input Field to select
                int nextIndex = (currentIndex == -1) ? (inputFieldsAuto.Length - 1) : (inputFieldsAuto.Length + currentIndex - 1) % inputFieldsAuto.Length;

                while (!inputFieldsAuto[nextIndex].interactable)
                {
                    nextIndex = (nextIndex - 1) % inputFieldsAuto.Length;
                }

                // Select the next Input Field
                inputFieldsAuto[nextIndex].Select();

            }
            else
            {
                // Calculate the index of the next Input Field to select
                int nextIndex = (currentIndex + 1) % inputFieldsAuto.Length;
                
                while(!inputFieldsAuto[nextIndex].interactable)
                {
                    nextIndex = (nextIndex + 1) % inputFieldsAuto.Length;
                }

                // Select the next Input Field
                inputFieldsAuto[nextIndex].Select();

            }
        }
        else
        {
            inputFieldsAuto[0].Select();
        }
    }

}
