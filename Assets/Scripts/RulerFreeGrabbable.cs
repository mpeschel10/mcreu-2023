using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerFreeGrabbable : MonoBehaviour, MyGrabber.Grabbable
{
    MyGrabber grabber;
    Transform tableTransform;
    RulerMouseDraggable mainDraggable;
    void Awake()
    {
        tableTransform = transform.parent;
        mainDraggable = GetComponent<RulerMouseDraggable>();
    }
    
    public void Grab(MyGrabber grabber) {
        Ungrab();
        this.grabber = grabber;
        Fix();
    }
    
    void Update() {
        mainDraggable.Fix();
    }

    void Fix()
    {
        transform.SetParent(grabber != null ? grabber.transform : tableTransform);
        enabled = grabber != null;
    }
    
    public void Ungrab()
    {
        if (this.grabber != null)
        {
            grabber.UngrabCleanup();
            grabber = null;
        }
        Fix();
    }
}
