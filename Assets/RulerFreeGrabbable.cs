using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerFreeGrabbable : MonoBehaviour, LessBadXRDirectInteractor.Grabbable
{
    Transform tableTransform;
    RulerMouseDraggable mainDraggable;
    void Awake()
    {
        tableTransform = transform.parent;
        mainDraggable = GetComponent<RulerMouseDraggable>();
    }
    
    public void Grab(Transform grabber) {
        transform.SetParent(grabber);
        enabled = true;
    }
    
    void Update() {
        mainDraggable.Fix();
    }
    
    public void Ungrab()
    {
        transform.SetParent(tableTransform);
        enabled = false;
    }
}
