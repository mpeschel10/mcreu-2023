using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeGrabbable : MonoBehaviour, LessBadXRDirectInteractor.Grabbable
{
    Transform tableTransform;
    void Start() { tableTransform = transform.parent; }
    public void Grab(Transform grabber) { transform.SetParent(grabber); }
    public void Ungrab() { transform.SetParent(tableTransform); }
}
