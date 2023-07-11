using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MouseSelector : MonoBehaviour
{
    [SerializeField] LayerMask selectableMask = 64 + 128;

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit raycastHit, float.PositiveInfinity, selectableMask);
            
            DoOutlines(raycastHit);
            DoClicks(raycastHit);
            DoDrags(raycastHit);
            DoSmearClicks(raycastHit);
        } else {
            // Putting the outline removal code here is hacky and bad. I don't know how I can fix this.
            if (hoverable != null)
            {
                try {
                    hoverable.Unhover();
                } catch (System.Exception e) {
                    Debug.LogWarning(e);
                }
                hoverable = null;
            }
        }
        if (Input.GetMouseButtonUp(0))
            shouldSmear = false;
    }

    public interface Hoverable {
        public void Hover(); public void Unhover();
        public GameObject GetGameObject(); // Interfaces cannot expose instance fields...
    }

    public interface Draggable {
        public void Grab(Transform transform); public void Ungrab();
    }

    public interface Clickable {
        public void Click();
    }

    public interface SmearClickable {
        public void Click();
    }

    Hoverable hoverable;
    void DoOutlines(RaycastHit hitInfo)
    {
        // Debug.Log("Doing outlines.");
        // Debug.Log("Hit info collider: " + hitInfo.collider);
        if ((hoverable == null && hitInfo.collider == null) ||
            (hitInfo.collider != null && hoverable != null && hitInfo.collider.gameObject == hoverable.GetGameObject()))
            return; // Nothing has changed, so don't flip-flop the outline.

        if (hoverable != null)
        {
            try {
                hoverable.Unhover();
            } catch (System.Exception e) {
                Debug.LogWarning(e);
            }
            hoverable = null;
        }

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out hoverable))
            {
                hoverable.Hover();
            } else  {
                // Debug.LogError("gameObject " + hitInfo.collider.gameObject + " on selectable layer has no Hoverable");
            }
        }
    }

    bool shouldSmear = false;
    void DoClicks(RaycastHit hitInfo)
    {
        if (hitInfo.collider != null && Input.GetMouseButtonDown(0))
        {
            GameObject gameObject = hitInfo.collider.gameObject;
            if (gameObject.TryGetComponent(out Clickable clickable))
            {
                clickable.Click();
                shouldSmear = gameObject.TryGetComponent(out SmearClickable _);
            } else {
                return;
            }
        } else {
        }
    }
    void DoSmearClicks(RaycastHit hitInfo)
    {
        if (hitInfo.collider != null && !Input.GetMouseButtonDown(0) && Input.GetMouseButton(0) && shouldSmear)
        {
            GameObject gameObject = hitInfo.collider.gameObject;
            if (gameObject.TryGetComponent(out SmearClickable clickable))
            {
                clickable.Click();
            } else {
                return;
            }
        }
    }

    Draggable dragging;
    void DoDrags(RaycastHit hitInfo)
    {
        if (hitInfo.collider != null && Input.GetMouseButtonDown(0))
        {
            if (dragging != null) // We missed a GetMouseButtonUp() somewhere; normalize.
            {
                dragging.Ungrab();
                dragging = null;
            }
            dragging = hitInfo.collider.gameObject.GetComponentInParent<Draggable>();
            if (dragging != null)
            {
                dragging.Grab(null);
            }
        }
        if (dragging != null && Input.GetMouseButtonUp(0))
        {
            dragging.Ungrab();
            dragging = null;
        }
    }

}
