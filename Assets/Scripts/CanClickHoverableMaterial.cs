using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanClickHoverableMaterial : MonoBehaviour, MouseSelector.Hoverable
{
    Material normalMaterial;
    [SerializeField] Material hoverMaterial, nextMaterial;
    bool hovering = false, next = false;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] MeshRenderer[] otherRenderers;
    Material[] normalMaterials;
    void Awake()
    {
        if (hoverMaterial == null)
            hoverMaterial = GameControllerState.defaultHoverMaterial;
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                Debug.LogWarning("CanClickHoverable " + gameObject + " could not find a MeshRenderer attached in Awake().");
            }
        }
        normalMaterial = meshRenderer.material;
        normalMaterials = new Material[otherRenderers.Length];
        for (int i = 0; i < otherRenderers.Length; i++)
        {
            normalMaterials[i] = otherRenderers[i].material;
        }
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
        if (hovering || next)
        {
            Material m = hovering ? hoverMaterial : nextMaterial;
            meshRenderer.material = m;
            foreach (MeshRenderer r in otherRenderers)
            {
                r.material = m;
            }
        }
        else
        {
            meshRenderer.material = normalMaterial;
            for (int i = 0; i < otherRenderers.Length; i++)
            {
                otherRenderers[i].material = normalMaterials[i];
            }
        }
    }
}
