using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class CanClickHoverableMultipleOutlines : MonoBehaviour, MouseSelector.Hoverable
{
    public GameObject GetGameObject() { return gameObject; }
    Outline outline;
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color highlightColor = Color.blue;

    bool highlighted = false;
    void Awake()
    {
        outline = GetComponent<Outline>();
        Fix();
    }

    public void Fix()
    {
        outline.OutlineColor = highlighted ? highlightColor : normalColor;
    }

    public void Hover()
    {
        highlighted = true;
        Fix();
    }

    public void Unhover()
    {
        highlighted = false;
        Fix();
    }
}
