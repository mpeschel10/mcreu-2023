using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeHighlighter : MonoBehaviour
{
    [SerializeField] AnimateHands state;
    [SerializeField] Transform pokeFocus;
    [SerializeField] float radius = 0.1f;
    [SerializeField] LayerMask layerMask = 64;

    Collider GetNearestCollider()
    {
        float bestDistance = float.PositiveInfinity;
        Collider bestCollider = null;
        Collider[] colliders = Physics.OverlapSphere(pokeFocus.position, radius, layerMask);
        foreach (Collider c in colliders)
        {
            float distance = (c.transform.position - transform.position).magnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestCollider = c;
            }
        }
        return bestCollider;
    }
    MouseSelector.Hoverable hoverable;
    void Update()
    {
        MouseSelector.Hoverable newHoverable;
        if (state.grip && !state.trigger)
        {
            Collider newCollider = GetNearestCollider();
            newHoverable = newCollider?.GetComponent<MouseSelector.Hoverable>();
        } else {
            newHoverable = null;
        }
        
        if (hoverable == newHoverable) return;
        hoverable?.Unhover();
        newHoverable?.Hover();
        hoverable = newHoverable;
    }
}
