using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfXR : MonoBehaviour
{
    [SerializeField] MonoBehaviour script;
    void Awake()
    {
        if (GameControllerState.isXR) Destroy(script);
    }
}
