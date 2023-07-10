using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruler2dRotate : MonoBehaviour, MouseSelector.Clickable
{
    Transform rulerTransform;
    RulerState rulerState;
    void Awake()
    {
        rulerState = GetComponentInParent<RulerState>();
        rulerTransform = rulerState.transform;
    }

    void CenterRuler()
    {
        MonoBehaviour northBar = rulerState.bars[0];
        MonoBehaviour southBar = rulerState.bars[2];
        
        Vector3 trueCenter = (northBar.transform.position + southBar.transform.position) / 2f;
        // Debug.Log("True center: " + trueCenter);
        // Object.Instantiate(GameControllerState.positionMarker, trueCenter, new Quaternion()).SetActive(true);
        Vector3 centerOffset = rulerState.transform.position - trueCenter;
        foreach (GameObject o in rulerState.rulerParts)
        {
            o.transform.position += centerOffset;
        }
        rulerState.transform.position -= centerOffset;
    }

    public void Click()
    {
        CenterRuler();
        rulerTransform.rotation *= Quaternion.Euler(0, 90, 0);
    }
}
