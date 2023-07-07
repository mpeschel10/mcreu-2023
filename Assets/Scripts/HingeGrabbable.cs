using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeGrabbable : MonoBehaviour, MyGrabber.Grabbable
{
    MyGrabber grabber;
    Transform grabberTransform;
    Transform parentTransform;
    Vector3 grabberOffset;
    Quaternion rotationFromFacingToIntersection;
    public void Awake() {
        parentTransform = transform.parent;
        Fix();
    }
    public void Grab(MyGrabber grabber)
    {
        Debug.Log("HingeGrabbable grab");
        Ungrab();
        this.grabber = grabber;
        Fix();
        Vector3 intersection = GetComponent<Collider>().ClosestPoint(grabberTransform.position);
        intersection.y = transform.position.y;
        grabberOffset = intersection - grabberTransform.position;

        Vector3 intersectionOffset = intersection - transform.position;
        Quaternion rotationFromReferenceToIntersection = Quaternion.LookRotation(intersectionOffset);
        Quaternion rotationFromReferenceToFacing = parentTransform.rotation;
        rotationFromFacingToIntersection = rotationFromReferenceToIntersection * Quaternion.Inverse(rotationFromReferenceToFacing);
    }

    public void Ungrab() {
        Debug.Log("HingeGrabbable ungrab");
        if (grabber != null)
        {
            grabber.UngrabCleanup();
            grabber = null;
        }
        Fix();
    }

    public void Fix()
    {
        grabberTransform = grabber?.transform;
        enabled = grabber != null;
    }

    Quaternion initialOffset;
    public void Update()
    {
        // Debug.Log("HingeGrabbable update");
        Vector3 intersection = grabberTransform.position + grabberOffset;
        intersection.y = transform.position.y;
        Vector3 intersectionOffset = intersection - transform.position;

        Quaternion rotationFromReferenceToIntersection = Quaternion.LookRotation(intersectionOffset);
        parentTransform.rotation = rotationFromReferenceToIntersection * Quaternion.Inverse(rotationFromFacingToIntersection);
    }
}
