using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerBody : MonoBehaviour
{
    [SerializeField] GameObject rulerStart, rulerEnd, rulerMiddle, rulerStick;
    [SerializeField] TMPro.TMP_Text middleText;
    RulerMouseDraggable rulerStartDraggable, rulerEndDraggable;

    void Awake()
    {
        rulerStartDraggable = rulerStart.GetComponent<RulerMouseDraggable>();
        rulerEndDraggable = rulerEnd.GetComponent<RulerMouseDraggable>();
    }

    public void Fix()
    {
        FixOrientation();
        FixText();
    }

    public void FixOrientation()
    {
        Vector3 offset = (rulerEnd.transform.position - rulerStart.transform.position);
        Vector3 oldScale = rulerStick.transform.localScale;
        rulerStick.transform.localScale = new Vector3(oldScale.x, offset.magnitude / 2, oldScale.z);

        transform.position = rulerStart.transform.position + offset / 2f;
        transform.rotation = Quaternion.LookRotation(offset) * Quaternion.Euler(90, 0, 0);
    }

    public void FixText()
    {
        float average = (rulerStartDraggable.index + rulerEndDraggable.index) / 2f;
        middleText.text = average.ToString();
    }
}
