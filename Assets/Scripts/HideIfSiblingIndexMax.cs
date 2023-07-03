using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfSiblingIndexMax : MonoBehaviour
{
    void Awake()
    {
        Transform parent = transform.parent;
        int siblingIndex = parent.GetSiblingIndex();
        int maxIndexExclusive = parent.parent.childCount - 1;
        gameObject.SetActive(siblingIndex < maxIndexExclusive);
    }
}
