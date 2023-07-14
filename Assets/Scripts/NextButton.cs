using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButton : MonoBehaviour, MouseSelector.Clickable
{
    PillarControllerState pillarController;
    void Awake()
    {
        pillarController = GameObject.FindGameObjectWithTag("PillarController").GetComponent<PillarControllerState>();
    }

    public void Click()
    {
        Debug.Log("Next click");
        pillarController.OnNext();
    }
}
