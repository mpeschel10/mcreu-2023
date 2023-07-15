using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextButton : MonoBehaviour, MouseSelector.Clickable
{
    Pillar2dControllerState pillarController;
    void Awake()
    {
        pillarController = GameObject.FindGameObjectWithTag("Pillar2dController").GetComponent<Pillar2dControllerState>();
    }

    public void Click()
    {
        Debug.Log("Next click");
        pillarController.OnNext();
    }
}
