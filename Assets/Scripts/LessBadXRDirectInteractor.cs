using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LessBadXRDirectInteractor : MonoBehaviour
{
    [SerializeField] InputActionReference selectReference;
    [SerializeField] Collider pinchPoint;
    void Awake()
    {
        selectReference.action.performed += OnSelectPerformed;
        selectReference.action.canceled += OnSelectCanceled;
    }

    void OnEnable() { selectReference.action.Enable(); }
    void OnDisable() { selectReference.action.Disable(); }

    HashSet<Collider> activeColliders = new HashSet<Collider>();
    void OnTriggerEnter(Collider other) { activeColliders.Add(other); }
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
    void OnSelectPerformed(InputAction.CallbackContext context)
    {
        Collider grabbedCollider = GetNearest(activeColliders);
        if (grabbedCollider == null) return;
        grabbed = grabbedCollider.GetComponentInParent<Grabbable>();
        grabbed.Grab(transform);
    }

    void OnSelectCanceled(InputAction.CallbackContext context)
    {
        if (grabbed != null) grabbed.Ungrab();
    }
}
