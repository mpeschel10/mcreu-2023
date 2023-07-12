using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeGrabbable : MonoBehaviour, MyGrabber.Grabbable, Fixable
{
    Transform originalParent;
    MyGrabber grabber;
    void Awake()
    {
        originalParent = transform.parent;
    }
    public void Grab(MyGrabber grabber)
    {
        if (this.grabber != null)
        {
            this.grabber.OnUngrab();
        }
        this.grabber = grabber;
        Fix();
    }
    public void Ungrab(MyGrabber grabber) // idempotent
    {
        this.grabber = null;
        Fix();
    }
    public void Fix() // idempotent
    {
        transform.SetParent(grabber != null ? grabber.transform : originalParent);
    }

}
