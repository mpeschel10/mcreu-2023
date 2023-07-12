using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeYIfNotXR : MonoBehaviour
{
    float frozenY;
    void Start()
    {
        if (!GameControllerState.isXR)
        {
            frozenY = transform.position.y;
        }
        else
        {
            enabled = false;
        }
    }

    void Update()
    {
        // Awful solution to the isometric movement problem
        Vector3 o = transform.position;
        transform.position = new Vector3(o.x, frozenY, o.z);
    }
}
