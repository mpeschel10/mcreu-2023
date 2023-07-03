using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerBody : MonoBehaviour
{
    [SerializeField] GameObject rulerStart, rulerEnd, rulerMiddle, rulerStick;
    // Update is called once per frame
    void Update()
    {
        Vector3 offset = (rulerEnd.transform.position - rulerStart.transform.position);
        Vector3 oldScale = rulerStick.transform.localScale;
        rulerStick.transform.localScale = new Vector3(oldScale.x, offset.magnitude / 2, oldScale.z);

        transform.position = rulerStart.transform.position + offset / 2f;
        transform.rotation = Quaternion.LookRotation(offset) * Quaternion.Euler(90, 0, 0);
    }
}
