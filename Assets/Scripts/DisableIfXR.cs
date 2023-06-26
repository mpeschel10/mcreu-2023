using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DisableIfXR : MonoBehaviour
{
    static bool? isXR;
    [SerializeField] Object target;
    void Start()
    {
        if (target == null) target = gameObject;
        if (IsXR())
        {
            Debug.Log("Disabling " + target + " since XR is available.");
            if (target is Collider collider)          { collider.enabled = false;         }
            else if (target is Behaviour behaviour)   { behaviour.enabled = false;        }
            else if (target is GameObject gameObject) { gameObject.SetActive(false);      }
            else
            {
                throw new System.Exception("Unknown type to disable " + target + "; supported types are Collider, Behaviour, and GameObject.");
            }
        } else
        {
            Debug.Log(target + " is enabled since XR is not available.");
        }
    }

    public static bool IsNotXR() { return !IsXR(); }
    public static bool IsXR()
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
