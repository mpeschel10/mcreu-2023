using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FixXRRayInteractor : MonoBehaviour, Fixable
{
    XRRayInteractor interactor;
    [SerializeField] MenuLocation thisLocation;
    void Start()
    {
        interactor = GetComponent<XRRayInteractor>();
        Fix();
    }

    public void Fix()
    {
        bool shouldShow = (thisLocation == MenuLocation.RightHand && GameControllerState.menuLocation == MenuLocation.LeftHand) ||
                          (thisLocation == MenuLocation.LeftHand && GameControllerState.menuLocation == MenuLocation.RightHand);
        interactor.enabled = shouldShow;
    }
}
