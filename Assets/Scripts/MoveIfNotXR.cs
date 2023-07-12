using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIfNotXR : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    void Start()
    {
        if (!GameControllerState.isXR)
        {
            transform.SetParent(targetTransform.parent);
            transform.localPosition = targetTransform.localPosition;
            transform.localRotation = targetTransform.localRotation;
            transform.localScale = targetTransform.localScale;
        }
    }

}
