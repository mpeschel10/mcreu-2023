using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfNotXR : MonoBehaviour
{
    [SerializeField] MonoBehaviour script;
    void Awake()
    {
        if (!GameControllerState.isXR) Destroy(script);
    }
}
