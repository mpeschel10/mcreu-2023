using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruler2dGrabbable : MonoBehaviour, MyGrabber.Grabbable
{
    RulerState rulerState;
    public bool isPrimary = false;
    void Awake()
    {
        rulerState = GetComponentInParent<RulerState>();
    }

    [System.NonSerialized] public MyGrabber grabber = null;
    public void Grab(MyGrabber grabber)
    {
        this.grabber?.OnUngrab();
        this.grabber = grabber;
        rulerState.Grab(grabber, this);
    }

    public void Ungrab()
    {
        rulerState.Ungrab(this);
        grabber = null;
    }
}
