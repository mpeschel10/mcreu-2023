using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuToggle : MonoBehaviour, Fixable
{
    [SerializeField] GameObject menu;
    [SerializeField] InputActionReference toggleAction;
    [SerializeField] MenuLocation thisLocation = MenuLocation.FullScreen;
    void Start()
    {
        toggleAction.action.performed += Toggle;
        Fix();
    }
    void OnDisable() { toggleAction.action.Disable(); }
    void OnEnable() { toggleAction.action.Enable(); }

    void Toggle(InputAction.CallbackContext context) { Toggle(); }
    public void Toggle()
    {
        GameControllerState.menuLocation = GameControllerState.menuLocation == thisLocation ?
                                           MenuLocation.Hidden : thisLocation;
        Fix();
    }

    public void Fix()
    {
        if (GameControllerState.menuLocation == thisLocation)
            menu.transform.SetParent(transform);
        menu.GetComponent<Fixable>().Fix();
    }
}
