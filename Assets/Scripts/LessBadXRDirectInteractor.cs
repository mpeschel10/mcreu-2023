using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LessBadXRDirectInteractor : MonoBehaviour
{
    [SerializeField] InputActionReference selectReference, actionReference;
    void Awake()
    {
        selectReference.action.performed += OnSelectPerformed;
        selectReference.action.canceled += OnSelectCanceled;
        actionReference.action.performed += OnActionPerformed;
        actionReference.action.canceled += OnActionCanceled;
    }

    void OnEnable() { selectReference.action.Enable(); actionReference.action.Enable(); }
    void OnDisable() { selectReference.action.Disable(); actionReference.action.Disable(); }

    HashSet<Collider> activeColliders = new HashSet<Collider>();
    void OnTriggerEnter(Collider other) {
        Debug.Log("Trigger enter");
        activeColliders.Add(other); }
    void OnTriggerExit(Collider other) { activeColliders.Remove(other); }

    Collider GetNearest(HashSet<Collider> colliders)
    {
        float bestDistance = float.PositiveInfinity;
        Collider bestCollider = null;
        foreach(Collider collider in colliders)
        {
            Vector3 offset = collider.ClosestPoint(transform.position) - transform.position;
            if (offset.magnitude < bestDistance)
            {
                bestDistance = offset.magnitude;
                bestCollider = collider;
            }
        }
        return bestCollider;
    }

    public interface Grabbable
    {
        public void Grab(Transform grabberTransform);
        public void Ungrab();
    }
    Grabbable grabbed;
    public int select, action = 0;
    void OnSelectPerformed(InputAction.CallbackContext context) {
        // Debug.Log("Selecting");
        int oldTotal = select + action;
        select = 1;
        if (oldTotal == 0) OnGrab();
    }
    void OnActionPerformed(InputAction.CallbackContext context) {
        // Debug.Log("Actioning");
        int oldTotal = select + action;
        action = 1;
        if (oldTotal == 0) OnGrab();
    }
    void OnSelectCanceled(InputAction.CallbackContext context) {
        // Debug.Log("unselecting");
        int oldTotal = select + action;
        select = 0;
        if (oldTotal == 1) OnUngrab();
    }
    void OnActionCanceled(InputAction.CallbackContext context) {
        // Debug.Log("Unactioning");
        int oldTotal = select + action;
        action = 0;
        if (oldTotal == 1) OnUngrab();
    }

    void OnGrab()
    {
        Debug.Log("Grabbing");
        Collider grabbedCollider = GetNearest(activeColliders);
        if (grabbedCollider == null) return;
        grabbed = grabbedCollider.GetComponentInParent<Grabbable>();
        grabbed.Grab(transform);
        Debug.Log("Grab happened");
    }

    void OnUngrab()
    {
        Debug.Log("Ungrabbing");
        if (grabbed != null) grabbed.Ungrab();
    }

}
