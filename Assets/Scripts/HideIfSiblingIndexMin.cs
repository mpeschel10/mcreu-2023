using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfSiblingIndexMin : MonoBehaviour, Fixable
{
    [SerializeField] int minimumIndexInclusive = 1;
    void Awake()
    {
        Fix();
    }

    public void Fix()
    {
        int siblingIndex = transform.parent.GetSiblingIndex();
        gameObject.SetActive(siblingIndex >= minimumIndexInclusive);
    }
}
