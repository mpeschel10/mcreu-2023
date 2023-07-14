using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfSiblingIndexMax : MonoBehaviour, Fixable
{
    void Awake()
    {
        Fix();
    }

    public void Fix()
    {

        Transform parent = transform.parent;
        int siblingIndex = parent.GetSiblingIndex();
        int maxIndexExclusive = parent.parent.childCount - 1;
        gameObject.SetActive(siblingIndex < maxIndexExclusive);
        // if (transform.parent.gameObject.name == "Menu Instructions PC")
        // {
        //     Debug.Log("Fixing " + gameObject + " of " + transform.parent.gameObject);
        //     Debug.Log("Parent is " + parent.gameObject);
        //     Debug.Log("Siblingindex is " + siblingIndex);
        //     Debug.Log("Siblingindex is " + maxIndexExclusive);
        // }
    }
}
