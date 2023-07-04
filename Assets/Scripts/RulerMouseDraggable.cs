using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerMouseDraggable : MonoBehaviour, FreeMovement.Draggable
{
    void Start() {
        enabled = false;
        Fix();
    }
    Vector3 grabOffset;
    [SerializeField] LayerMask rulerPlaneLayerMask;
    [SerializeField] Pillars pillars;
    [SerializeField] TMPro.TMP_Text textFront;
    [SerializeField] RulerBody rulerBody;
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

    public int index = 0;
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out RaycastHit raycastHit, float.PositiveInfinity, rulerPlaneLayerMask);
        if (raycastHit.collider != null)
            transform.position = raycastHit.point + grabOffset;
        Fix();
    }

    public void Fix()
    {
        FixIndex();
        rulerBody.Fix();
    }

    void FixIndex()
    {
        index = pillars.PositionToIndex(transform.position);
        textFront.text = index.ToString();
        Debug.Log(index);
    }

    public void Ungrab()
    {
        enabled = false;
    }
}
