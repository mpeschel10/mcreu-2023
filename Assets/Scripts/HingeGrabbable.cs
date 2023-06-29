using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingeGrabbable : MonoBehaviour, LessBadXRDirectInteractor.Grabbable
{
    Transform grabberTransform;
    Transform parentTransform;
    Vector3 grabberOffset;
    Quaternion rotationFromFacingToIntersection;
    public void Awake() { parentTransform = transform.parent; }
    public void Grab(Transform grabberTransform)
    {
        this.grabberTransform = grabberTransform;
        Vector3 intersection = GetComponent<Collider>().ClosestPoint(grabberTransform.position);
        intersection.y = transform.position.y;
        grabberOffset = intersection - grabberTransform.position;

        Vector3 intersectionOffset = intersection - transform.position;
        Quaternion rotationFromReferenceToIntersection = Quaternion.LookRotation(intersectionOffset);
        Quaternion rotationFromReferenceToFacing = parentTransform.rotation;
        rotationFromFacingToIntersection = rotationFromReferenceToIntersection * Quaternion.Inverse(rotationFromReferenceToFacing);
    }

    public void Ungrab() { this.grabberTransform = null; }

    Quaternion initialOffset;
    public void Update()
    {
        if (grabberTransform == null) return;
        Vector3 intersection = grabberTransform.position + grabberOffset;
        intersection.y = transform.position.y;
        Vector3 intersectionOffset = intersection - transform.position;

        Quaternion rotationFromReferenceToIntersection = Quaternion.LookRotation(intersectionOffset);
        parentTransform.rotation = rotationFromReferenceToIntersection * Quaternion.Inverse(rotationFromFacingToIntersection);
    }
}
