using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDisplay : MonoBehaviour
{

    public Pendulum lastTrigger;
    private Material triggerDisplayMat;

    private void Start()
    {
        triggerDisplayMat = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (lastTrigger != null)
        {
            triggerDisplayMat.SetColor("_EmissionColor", lastTrigger.GetColour());
        }
    }
}
