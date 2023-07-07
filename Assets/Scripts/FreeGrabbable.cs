using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeGrabbable : MonoBehaviour, MyGrabber.Grabbable, Fixable
{
    Transform originalParent;
    MyGrabber grabber;
    void Start()
    {
        originalParent = transform.parent;
    }
    public void Grab(MyGrabber grabber) {
        Ungrab();
        this.grabber = grabber;
        Fix();
    }

    public void Fix()
    {
        transform.SetParent(grabber != null ? grabber.transform : originalParent);
    }

    public void Ungrab()
    {
        if (grabber != null)
        {
            grabber.UngrabCleanup();
            grabber = null;
        }
        Fix();
    }
}
