using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerMouseDraggable : MonoBehaviour, FreeMovement.Draggable
{
    void Awake() { enabled = false; }
    Vector3 grabOffset;
    [SerializeField] LayerMask rulerPlaneLayerMask;
    public void Grab(Transform grabTransform)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.PositiveInfinity, rulerPlaneLayerMask);
        if (raycastHit.collider == null)
            grabOffset = Vector3.zero;
        else
            grabOffset = transform.position - raycastHit.point;
        enabled = true;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.PositiveInfinity, rulerPlaneLayerMask);
        if (raycastHit.collider != null)
            transform.position = raycastHit.point + grabOffset;
    }

    public void Ungrab()
    {
        enabled = false;
    }
}
