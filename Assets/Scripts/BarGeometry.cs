using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarGeometry : MonoBehaviour
{
    GameObject cube;
    GameObject endMarker;
    GameObject endMarker1, endMarker2;

    void Awake()
    {
        cube = GetComponentInChildren<BoxCollider>().gameObject;
        // endMarker = GameObject.FindGameObjectWithTag("PositionMarker");
        // endMarker1 = Object.Instantiate(endMarker);
        // endMarker2 = Object.Instantiate(endMarker);
        // endMarker1.SetActive(true);
        // endMarker2.SetActive(true);
    }

    public Vector3 GetFrontOffset() // relative to center.
    {
        return transform.forward * (size + width) / 2f;
    }
    public Vector3 GetBackOffset() // relative to center.
    {
        return GetFrontOffset() * -1;
    }
    public Vector3 GetPrimaryEnd()
    {
        // Debug.Log("Size: " + size);
        // Debug.Log("Width: " + width);
        // Debug.Log("Offset: " + (size + (width * 2)) / 2f);
        return GetFrontEnd();
    }
    public Vector3 GetFrontEnd()
    {
        return transform.position + GetFrontOffset();
    }
    public Vector3 GetSecondaryEnd() { return GetBackEnd(); }
    public Vector3 GetBackEnd() { return transform.position + GetBackOffset(); }
    
    public float size
    {
        get { return cube.transform.localScale.z; }
        set {
            Vector3 o = cube.transform.localScale;
            cube.transform.localScale = new Vector3(o.x, o.y, value);
        }
    }
    public float width
    {
        get { return cube.transform.localScale.x; }
    }

    public void AlignBackCorner(Ruler2dMouseDraggable primaryIntent)
    {
        // Debug.Log("Begun alignbackcorner on gameobject " + gameObject);
        Vector3 newBackPosition = primaryIntent.frontCorner.transform.position;
        Vector3 idealOffset = GetFrontEnd() - newBackPosition;
        Vector3 newOffset = Vector3.Project(idealOffset, primaryIntent.transform.right);
        Vector3 newFrontPosition = newBackPosition + newOffset;
        AlignToEndpoints(newFrontPosition, newBackPosition);
    }

    public void AlignFrontCorner(Ruler2dMouseDraggable primaryIntent)
    {
        Vector3 newFrontPosition = primaryIntent.backCorner.transform.position;
        Vector3 idealOffset = GetBackEnd() - newFrontPosition;
        Vector3 newOffset = Vector3.Project(idealOffset, primaryIntent.transform.right);
        Vector3 newBackPosition = newFrontPosition + newOffset;
        AlignToEndpoints(newFrontPosition, newBackPosition);
    }

    public void AlignToEndpoints(Vector3 p1, Vector3 p2)
    {
        transform.position = (p1 + p2) / 2f;
        float newSpan = (p2 - p1).magnitude;
        size = newSpan - width;
    }

    public void AlignToEndpoints(GameObject p1, GameObject p2)
    {
        AlignToEndpoints(p1.transform.position, p2.transform.position);
    }


    // void Update()
    // {
    //     Debug.Log("Placing endpoints for fixablebar " + gameObject);
    //     Debug.Log("Primary end: " + GetPrimaryEnd());
    //     endMarker1.transform.position = GetPrimaryEnd();
    //     endMarker2.transform.position = GetSecondaryEnd();
    // }
}
