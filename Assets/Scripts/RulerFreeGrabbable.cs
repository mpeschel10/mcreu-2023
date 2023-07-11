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
        if (this.grabber != null)
        {
            this.grabber.OnUngrab();
        }
        this.grabber = grabber;
        Fix();
    }
    
    void Update() {
        mainDraggable.Fix();
    }

    public void Ungrab() // Idempotent
    {
        grabber = null;
        Fix();
    }
    
    void Fix() // Idempotent
    {
        transform.SetParent(grabber != null ? grabber.transform : tableTransform);
        enabled = grabber != null;
    }
    
}
