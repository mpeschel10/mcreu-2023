using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHands : MonoBehaviour
{
    [SerializeField] GameObject handNormal, handGrip, handPoint, handPinch;
    GameObject current;
    [SerializeField] InputActionReference gripReference, triggerReference;
    void Awake() {
        gripReference.action.performed += OnGrip;
        gripReference.action.canceled += OnGripCancel;
        gripReference.action.Enable();
        triggerReference.action.performed += OnTrigger;
        triggerReference.action.canceled += OnTriggerCancel;
        triggerReference.action.Enable();
    }

    bool grip = false;
    bool trigger = false;

    void OnGrip(InputAction.CallbackContext context) { grip = true; }
    void OnGripCancel(InputAction.CallbackContext context) { grip = false; }
    void OnTrigger(InputAction.CallbackContext context) { trigger = true; }
    void OnTriggerCancel(InputAction.CallbackContext context) { trigger = false; }

    void Update() {
        current = grip ?
            ( trigger ? handGrip : handPoint ) :
            ( trigger ? handPinch : handNormal );
        handNormal.SetActive(handNormal == current);
        handGrip.SetActive(handGrip == current);
        handPoint.SetActive(handPoint == current);
        handPinch.SetActive(handPinch == current);
    }
}
