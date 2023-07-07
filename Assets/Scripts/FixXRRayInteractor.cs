using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FixXRRayInteractor : MonoBehaviour, Fixable
{
    public static HashSet<FixXRRayInteractor> interactors = new HashSet<FixXRRayInteractor>();
    public static void FixAll() { foreach ( FixXRRayInteractor i in interactors) i.Fix(); }
    [SerializeField] MenuLocation thisLocation;
    void Start()
    {
        interactors.Add(this);
        Fix();
    }

    void OnDestroy()
    {
        interactors.Remove(this);
    }

    public void Fix()
    {
        bool shouldShow = (thisLocation == MenuLocation.RightHand && GameControllerState.menuLocation == MenuLocation.LeftHand ) ||
                          (thisLocation == MenuLocation.LeftHand  && GameControllerState.menuLocation == MenuLocation.RightHand);
        GetComponent<XRRayInteractor>().enabled = shouldShow;
    }
}
