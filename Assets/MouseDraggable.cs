using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDraggable : MonoBehaviour, MouseSelector.Draggable
{
    Vector2 grabOffset;
    float distanceFromScreen;
    Fixable updateCallback;
    [SerializeField] Transform targetTransform;
    void Awake()
    {
        if (targetTransform == null)
            targetTransform = transform;
        updateCallback = GetComponent<Fixable>();
        enabled = false;
    }

    public void Grab(Transform _)
    {
        // Debug.Log("Grab ruler");
        Vector2 myPosition = Camera.main.WorldToScreenPoint(targetTransform.position);
        Vector2 mousePosition = Input.mousePosition;
        grabOffset = myPosition - mousePosition;

        // Do not use Camera.main.ScreenToWorldPoint instead of ray.origin;
        //  Screen does not take into account the camera's clipping plane,
        //  but ScreenPointToRay does,
        //  so you will get a discrepancy in the supposed camera position between this function and Update.
        Ray ray = Camera.main.ScreenPointToRay(myPosition);
        distanceFromScreen = (ray.origin - targetTransform.position).magnitude;
        enabled = true;
        // Debug.Log("Click ray origin: " + ray.origin + ", distance: " + distanceFromScreen);

        firstUpdate = true;
    }

    bool firstUpdate = false;
    void Update()
    {
        Vector2 myPosition = (Vector2) Input.mousePosition + grabOffset;
        // Debug.Log(myPosition);
        Ray ray = Camera.main.ScreenPointToRay(myPosition);
        targetTransform.position = ray.GetPoint(distanceFromScreen);
        if (firstUpdate)
        {
            // Debug.Log("Drag ray origin: " + ray.origin + ", distance: " + distanceFromScreen);
            firstUpdate = false;
        }
        updateCallback?.Fix();
    }
    
    public void Ungrab()
    {
        Debug.Log("Ungrab ruler part " + gameObject);
        enabled = false;
    }
}
