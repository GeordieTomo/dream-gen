using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmenuAwakeLoader : MonoBehaviour
{

    [SerializeField] GameObject[] submenus; 

    void Awake()
    {
        foreach (var submenu in submenus)
        {
            submenu.SetActive(true);
        }
    }
}
