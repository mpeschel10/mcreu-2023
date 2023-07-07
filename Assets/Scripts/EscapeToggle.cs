using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscapeToggle : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] InputActionReference actionReference;
    [SerializeField] Fixable leftXRRayInteractor, rightXRRayInteractor;
    void Start()
    {
        actionReference.action.performed += OnAction;
        actionReference.action.Enable();
        Fix();
    }

    void OnDestroy()
    {
        actionReference.action.performed -= OnAction;
    }
    void OnAction(InputAction.CallbackContext context) { Toggle(); }
    public void Toggle()
    {
        Debug.Log("Toggling on escape key; probably still for menu");
        // GameControllerState.menuVisible = !GameControllerState.menuVisible;
        Fix();
    }

    public void Fix()
    {
        // target?.SetActive(GameControllerState.menuVisible);
    }
}
