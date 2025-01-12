using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrayEditController : MonoBehaviour
{

    [SerializeField] private InputFieldParser[] inputFields;

    private GridLayoutGroup gridLayoutGroup;

    private int numAlive = 0;
    private int rowWidth = 15;

    private List<double> arrayValues = new List<double>();

    private bool menuOpen = false;

    private void Awake()
    {
        gridLayoutGroup = GetComponent<GridLayoutGroup>();
        rowWidth = gridLayoutGroup.constraintCount;

        foreach (var field in inputFields)
        {
            field.gameObject.SetActive(false);
        }
        transform.parent.parent.gameObject.SetActive(false);
    }

    public void SetArray(int numElements, List<int> arrayValues)
    {
        List<double> array = new List<double>();
        foreach (int element in arrayValues)
        {
            array.Add(element);
        }
        OpenEditMenu(numElements, array);
    }

    public void SetArray(int numElements, List<double> arrayValues)
    {
        OpenEditMenu(numElements, arrayValues);
        /*
        this.arrayValues.Clear();
        this.arrayValues.AddRange(arrayValues);

        for (int i = 0; i < numElements; i++)
        {
            Debug.Log(arrayValues[i]);

            inputFields[i].gameObject.SetActive(true);

            inputFields[i].SetDoubleValue(arrayValues[i]);
            inputFields[i].SetBoxEnabled(true);

        }
        Debug.Log("----" + numElements);

        numAlive = numElements;*/
    }

    public void OpenEditMenu(int numElements, List<int> arrayValues)
    {
        List<double> array = new List<double>();
        foreach(int element in arrayValues)
        {
            array.Add(element);
        }
        OpenEditMenu(numElements, array);
    }

    public void OpenEditMenu(int numElements, List<double> arrayValues)
    {
        menuOpen = true;
        this.arrayValues.Clear();
        this.arrayValues.AddRange(arrayValues);

        transform.parent.parent.gameObject.SetActive(true);

        gameObject.SetActive(false);

        transform.parent.parent.parent.parent.SetAsLastSibling();

        gameObject.SetActive(true);


        numAlive = numElements;

        if (numElements < rowWidth)
        {
            gridLayoutGroup.constraintCount = numElements;
        }
        else
        {
            gridLayoutGroup.constraintCount = rowWidth;
        }

        for (int i = 0; i < inputFields.Length; i++)
        {
            if (i < numElements)
            {
                inputFields[i].gameObject.SetActive(true);

                inputFields[i].SetDoubleValue(arrayValues[i]);
                inputFields[i].SetBoxEnabled(true);
            }
            else
            {
                inputFields[i].gameObject.SetActive(false);
            }
        }

    }

    public void ExitEditMenu()
    {
        for (int i = 0; i < inputFields.Length; i++)
        {
            if (i < numAlive)
            {
                arrayValues[i] = inputFields[i].GetFloatValue();
            }
        }
        menuOpen = false;

        transform.parent.parent.gameObject.SetActive(false);
    }

    public List<double> GetArrayValues()
    {
        return arrayValues.GetRange(0, numAlive);
    }

    public List<int> GetArrayValuesInt()
    {
        List<int> intValues = new List<int>();

        for (int i = 0; i < numAlive;i++)
        {
            intValues.Add((int)arrayValues[i]);
        }
        return intValues;
    }

    private bool MenuIsOpen()
    {
        return menuOpen;
    }

}
