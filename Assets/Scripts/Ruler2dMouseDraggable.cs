using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BarGeometry))]
[RequireComponent(typeof(MouseDraggable))]
public class Ruler2dMouseDraggable : MonoBehaviour, Fixable
{
    [SerializeField] public GameObject frontCorner, backCorner;
    [System.NonSerialized] public BarGeometry barGeometry;
    [SerializeField] public Ruler2dMouseDraggable frontBar, backBar, oppositeBar;
    RulerState rulerState;

    void Awake()
    {
        barGeometry = GetComponent<BarGeometry>();
        if (barGeometry == null)
            Debug.LogError("Cannot find barGeometry component on GameObject " + gameObject);
        rulerState = GetComponentInParent<RulerState>();
    }
    
    public void Fix()
    {
        frontCorner.transform.position = barGeometry.GetPrimaryEnd();
        backCorner.transform.position = barGeometry.GetSecondaryEnd();
        frontCorner.transform.rotation = transform.rotation;
        backCorner.transform.rotation = Quaternion.LookRotation(transform.right * -1, transform.up);
        frontBar.barGeometry.AlignBackCorner(this);
        frontBar.FixFrontCorner();
        backBar.barGeometry.AlignFrontCorner(this);
        backBar.FixBackCorner();
        oppositeBar.barGeometry.AlignToEndpoints(frontBar.frontCorner, backBar.backCorner);
        foreach (Ruler2dReticle reticle in rulerState.reticles)
        {
            reticle.Fix();
        }
    }

    public void FixFrontCorner()
    {
        frontCorner.transform.position = barGeometry.GetFrontEnd();
    }
    public void FixBackCorner()
    {
        backCorner.transform.position = barGeometry.GetBackEnd();
    }

    public void FixBar()
    {
        barGeometry.AlignToEndpoints(frontCorner, backCorner);
        transform.rotation = frontCorner.transform.rotation;
    }
}
