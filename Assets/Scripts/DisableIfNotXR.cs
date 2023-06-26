using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DisableIfNotXR : MonoBehaviour
{
    static bool? isXR;
    [SerializeField] Object target;
    void Start()
    {
        if (target == null) target = gameObject;
        if (IsThisJustFantasy())
        {
            Debug.Log(target + " is enabled since XR is available.");
        } else
        {
            Debug.Log("Disabling " + target + " since XR is not available.");
            if (target is Collider collider)          { collider.enabled = false;         }
            else if (target is Behaviour behaviour)   { behaviour.enabled = false;        }
            else if (target is GameObject gameObject) { gameObject.SetActive(false);      }
            else
            {
                throw new System.Exception("Unknown type to disable " + target + "; supported types are Collider, Behaviour, and GameObject.");
            }
        }
    }

    public static bool IsThisTheRealLife() { return !IsThisJustFantasy(); }
    public static bool IsThisJustFantasy()
    {
        if (isXR == null)
        {
            List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            isXR = xrDisplaySubsystems.Count != 0;
        }
        return isXR.Value;
    }
}
