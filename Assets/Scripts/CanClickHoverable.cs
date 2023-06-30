using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LayeredOutline))]
public class CanClickHoverable : MonoBehaviour, FreeMovement.Hoverable
{
    public GameObject GetGameObject() { return gameObject; }
    public void Hover() { layeredOutline.AddLayer("can-click"); }
    public void Unhover() { layeredOutline.SubtractLayer("can-click"); }

    LayeredOutline layeredOutline;
    void Start() {
        layeredOutline = GetComponent<LayeredOutline>();
    }
}
