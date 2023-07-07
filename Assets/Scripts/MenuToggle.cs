using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuToggle : MonoBehaviour, Fixable
{
    [SerializeField] GameObject menu;
    [SerializeField] InputActionReference toggleAction;
    [SerializeField] MenuLocation thisLocation = MenuLocation.FullScreen;
    [SerializeField] GameObject[] gameObjectsThatHaveFixables;
    Fixable[] callbacks;
    void Start()
    {
        callbacks = new Fixable[gameObjectsThatHaveFixables.Length];
        for (int i = 0; i < callbacks.Length; i++)
        {
            GameObject g = gameObjectsThatHaveFixables[i];
            callbacks[i] = g.GetComponent<Fixable>();
        }
        // Debug.Log("Start of menu toggle stuff");
        toggleAction.action.performed += Toggle;
        Fix();
    }
    void OnDestroy()
    {
        toggleAction.action.performed -= Toggle;
    }
    void OnDisable() { toggleAction.action.Disable(); }
    void OnEnable() { toggleAction.action.Enable(); }

    void Toggle(InputAction.CallbackContext context) { Toggle(); }
    public void Toggle()
    {
        // Debug.Log("Toggling " + thisLocation);
        GameControllerState.menuLocation = GameControllerState.menuLocation == thisLocation ?
                                           MenuLocation.Hidden : thisLocation;
        Fix();
    }

    public void Fix()
    {
        if (GameControllerState.menuLocation == thisLocation)
        {
            menu.transform.SetParent(transform, false);
            // Debug.Log("Set menu transform");
        }
        menu.GetComponent<Fixable>().Fix();
        foreach (Fixable f in callbacks)
        {
            // Debug.Log("Menu toggle is calling fixable : " + f);
            f.Fix();
        }
    }
}
