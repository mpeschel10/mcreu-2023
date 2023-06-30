using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokeHighlighter : MonoBehaviour
{
    [SerializeField] AnimateHands state;
    [SerializeField] Transform pokeFocus;
    [SerializeField] float radius = 0.1f;

    Collider GetNearestCollider()
    {
        float bestDistance = float.PositiveInfinity;
        Collider bestCollider = null;
        Collider[] colliders = Physics.OverlapSphere(pokeFocus.position, radius);
        foreach (Collider c in colliders)
        {
            if (c.gameObject.TryGetComponent(out FreeMovement.Hoverable _))
            {
                float distance = (c.transform.position - transform.position).magnitude;
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    bestCollider = c;
                }
            }
        }
        return bestCollider;
    }
    FreeMovement.Hoverable hoverable;
    void Update()
    {
        FreeMovement.Hoverable newHoverable;
        if (state.grip && !state.trigger)
        {
            Collider newCollider = GetNearestCollider();
            newHoverable = newCollider?.GetComponent<FreeMovement.Hoverable>();
        } else {
            newHoverable = null;
        }
        
        if (hoverable == newHoverable) return;
        hoverable?.Unhover();
        newHoverable?.Hover();
        hoverable = newHoverable;
    }
}
