using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MyGrabber : MonoBehaviour
{
    [SerializeField] InputActionReference selectReference, actionReference;
    [SerializeField] Transform trigger;
    [SerializeField] public Transform pinchPoint;
    [SerializeField] LayerMask pinchables;
    void Awake()
    {
        selectReference.action.performed += OnSelectPerformed;
        selectReference.action.canceled  += OnSelectCanceled;
        actionReference.action.performed += OnActionPerformed;
        actionReference.action.canceled  += OnActionCanceled;
    }

    void OnDestroy()
    {
        selectReference.action.performed -= OnSelectPerformed;
        selectReference.action.canceled  -= OnSelectCanceled;
        actionReference.action.performed -= OnActionPerformed;
        actionReference.action.canceled  -= OnActionCanceled;
    }

    void OnEnable() { selectReference.action.Enable(); actionReference.action.Enable(); }
    void OnDisable() { selectReference.action.Disable(); actionReference.action.Disable(); }

    Collider GetNearest()
    {
        float bestDistance = float.PositiveInfinity;
        int bestPriority = int.MaxValue;
        Collider bestCollider = null;
        Collider[] colliders = Physics.OverlapBox(trigger.position, trigger.localScale / 2, trigger.rotation, pinchables);
        // Debug.Log("Finding closest grabbable among " + colliders.Length + " colldiers");
        foreach(Collider collider in colliders)
        {
            int priority = 10;
            if (collider.gameObject.TryGetComponent(out GrabPriority p))
                priority = p.Main();
            if (priority > bestPriority) continue;

            Vector3 offset = collider.ClosestPoint(pinchPoint.position) - pinchPoint.position;
            if (offset.magnitude > bestDistance) continue;
            
            bestPriority = priority;
            bestDistance = offset.magnitude;
            bestCollider = collider;
        }
        return bestCollider;
    }

    public interface Grabbable
    {
        public void Grab(MyGrabber grabber);
        public void Ungrab(MyGrabber grabber);
    }

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
    
    Grabbable grabbed;
    public void OnGrab()
    {
        OnUngrab();
        // Debug.Log("Grab started");
        Collider grabbedCollider = GetNearest();
        if (grabbedCollider == null)
        {
            // Debug.Log("Grab fail since no grabbable colliders found.");
            return;
        }
        
        // Debug.Log("Trying to grab collider " + grabbedCollider);
        grabbed = grabbedCollider.GetComponentInParent<Grabbable>();
        if (grabbed == null)
        {
            Debug.LogError(gameObject + " tried to grab object " + grabbedCollider.gameObject + " but could not find Grabbable in object or parent.");
            return;
        }
        
        grabbed.Grab(this);
    }

    public void OnUngrab() // Idempotent
    {
        // Debug.Log("MyGrabber " + gameObject + " ungrab '" + grabbed + "'");
        if (grabbed != null)
        {
            grabbed.Ungrab(this);
            grabbed = null;
        }
    }
}
