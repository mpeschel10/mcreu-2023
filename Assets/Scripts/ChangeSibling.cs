using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSibling : MonoBehaviour
{
    public void Main(int direction)
    {
        Transform parent = transform.parent;
        int siblingIndex = parent.GetSiblingIndex();
        Transform aunt = parent.parent.GetChild(siblingIndex + direction);
        parent.gameObject.SetActive(false);
        aunt.gameObject.SetActive(true);
    }
}
