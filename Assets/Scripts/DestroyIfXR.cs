using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyIfXR : MonoBehaviour
{
    [SerializeField] MonoBehaviour script;
    void Start()
    {
        if (GameControllerState.isXR) Destroy(script);
    }
}
