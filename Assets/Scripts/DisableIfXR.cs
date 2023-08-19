using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class DisableIfXR : MonoBehaviour
{
    [SerializeField] Object target;
    void Start()
    {
        if (target == null) target = gameObject;
        if (GameControllerState.isXR)
        {
            // Debug.Log("Disabling " + target + " since XR is available.");
            if (target is Collider collider)          { collider.enabled = false;         }
            else if (target is Behaviour behaviour)   { behaviour.enabled = false;        }
            else if (target is GameObject gameObject) { gameObject.SetActive(false);      }
            else
            {
                throw new System.Exception("Unknown type to disable " + target + "; supported types are Collider, Behaviour, and GameObject.");
            }
        } else
        {
            // Debug.Log(target + " is enabled since XR is not available.");
        }
    }
}
