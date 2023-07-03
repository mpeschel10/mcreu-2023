using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscapeToggle : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] InputActionReference actionReference;
    void Start()
    {
        actionReference.action.performed += OnAction;
        actionReference.action.Enable();
    }
    void OnAction(InputAction.CallbackContext context) { Toggle(); }
    public void Toggle() { Debug.Log("Toggling"); target?.SetActive(!target.activeSelf); }
}
