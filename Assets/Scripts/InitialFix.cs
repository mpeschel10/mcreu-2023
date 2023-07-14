using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialFix : MonoBehaviour
{
    void Start()
    {
        Fixable fixable = GetComponent<Fixable>();
        // Debug.Log("Calling initial fix on " + ((MonoBehaviour) fixable).gameObject);
        fixable.Fix();
    }
}
