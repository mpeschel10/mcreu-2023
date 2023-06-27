using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EscapeToggle : MonoBehaviour
{
    [SerializeField] GameObject target;
    InputAction escapeAction;
    void Awake()
    {
        escapeAction = new InputAction(name: "menuToggle", type: InputActionType.Button);
        escapeAction.AddBinding("<Keyboard>/escape");
        escapeAction.performed += OnEscape;
    }

    void OnEnable() { escapeAction.Enable(); }
    void OnDisable() { escapeAction.Disable(); }
    void OnEscape(InputAction.CallbackContext context) { target.SetActive(!target.activeSelf); }
}
