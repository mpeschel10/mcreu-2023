using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanClickHoverableMaterial : MonoBehaviour, FreeMovement.Hoverable
{
    MeshRenderer meshRenderer;
    Material normalMaterial;
    [SerializeField] Material hoverMaterial;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        normalMaterial = meshRenderer.material;
    }

    public GameObject GetGameObject() { return gameObject; }
    public void Hover() { meshRenderer.material = hoverMaterial; }
    public void Unhover() { meshRenderer.material = normalMaterial; }
}
