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
        FixCorners();
        frontBar.FixFromBackCornerAndFrontBarPlane();
        backBar.FixFromFrontCornerAndBackBarPlane();
        oppositeBar.FixBar();
        foreach (Ruler2dReticle reticle in rulerState.reticles)
        {
            reticle.Fix();
        }
    }

    public void FixFromBackCornerAndFrontBarPlane()
    {
        Vector3 newBackPosition = backCorner.transform.position;
        Vector3 idealOffset = frontBar.transform.position - newBackPosition;
        Vector3 newOffset = Vector3.Project(idealOffset, backCorner.transform.right);
        Vector3 newFrontPosition = newBackPosition + newOffset;
        
        frontCorner.transform.position = newFrontPosition;
        frontCorner.transform.rotation = Quaternion.LookRotation(forward: backCorner.transform.right, upwards: backCorner.transform.up);
        FixBar();
    }
    public void FixFromFrontCornerAndBackBarPlane()
    {
        Vector3 newFrontPosition = frontCorner.transform.position;
        Vector3 idealOffset = backBar.transform.position - newFrontPosition;
        Vector3 newOffset = Vector3.Project(idealOffset, frontCorner.transform.forward * -1);
        Vector3 newBackPosition = newFrontPosition + newOffset;
        
        backCorner.transform.position = newBackPosition;
        backCorner.transform.rotation = Quaternion.LookRotation(forward: frontCorner.transform.right * -1, upwards: frontCorner.transform.up);
        FixBar();
    }

    public void XRFixPrimary()
    {
        Vector3 secondaryOffset = oppositeBar.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(forward: secondaryOffset, upwards: transform.up) * Quaternion.Euler(0, 270, 0);
        FixCorners();
        Vector3 myBack = transform.forward * -1;
        oppositeBar.transform.rotation = Quaternion.LookRotation(forward: myBack, upwards: transform.up);
        oppositeBar.FixCorners();
        frontBar.FixBar();
        backBar.FixBar();
        foreach (Ruler2dReticle reticle in rulerState.reticles)
        {
            reticle.Fix();
        }
    }

    public void FixCorners()
    {
        FixFrontCorner();
        FixBackCorner();
    }

    public void FixFrontCorner()
    {
        frontCorner.transform.position = barGeometry.GetFrontEnd();
        frontCorner.transform.rotation = transform.rotation;
    }
    public void FixBackCorner()
    {
        backCorner.transform.position = barGeometry.GetBackEnd();
        backCorner.transform.rotation = Quaternion.LookRotation(transform.right * -1, transform.up);
    }

    public void FixBar()
    {
        barGeometry.AlignToEndpoints(frontCorner, backCorner);
        transform.rotation = frontCorner.transform.rotation;
    }
}
