using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFacing : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            transform.rotation *= Quaternion.Euler(0, 45, 0);
        }
    }
}
