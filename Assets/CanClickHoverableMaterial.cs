using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanClickHoverableMaterial : MonoBehaviour, FreeMovement.Hoverable
{
    Material normalMaterial;
    [SerializeField] Material hoverMaterial, nextMaterial;
    bool hovering = false, next = false;
    MeshRenderer meshRenderer;
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        normalMaterial = meshRenderer.material;
    }

    public GameObject GetGameObject() { return gameObject; }
    public void Hover()
    {
        hovering = true;
        Fix();
    }
    public void Unhover()
    {
        hovering = false;
        Fix();
    }

    public void SetNext(bool value)
    {
        next = value;
        Fix();
    }
    
    public void Fix()
    {
        Material m;
        if (hovering)
            m = hoverMaterial;
        else if (next)
            m = nextMaterial;
        else
            m = normalMaterial;
        meshRenderer.material = m;
    }
}
