using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfSiblingIndexMin : MonoBehaviour
{
    [SerializeField] int minimumIndexInclusive = 1;
    void Awake()
    {
        int siblingIndex = transform.parent.GetSiblingIndex();
        gameObject.SetActive(siblingIndex >= minimumIndexInclusive);
    }
}
