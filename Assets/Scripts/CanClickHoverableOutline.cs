using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanClickHoverableOutline : MonoBehaviour, MouseSelector.Hoverable
{
    public GameObject GetGameObject() { return gameObject; }
    Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void Hover()
    {
        outline.enabled = true;
    }

    public void Unhover()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
