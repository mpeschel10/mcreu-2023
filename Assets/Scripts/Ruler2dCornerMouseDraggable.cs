using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MouseDraggable))]
public class Ruler2dCornerMouseDraggable : MonoBehaviour, Fixable
{
    [SerializeField] Ruler2dCornerMouseDraggable clockwiseCorner, ccwCorner;
    Ruler2dCornerMouseDraggable oppositeCorner;
    RulerState rulerState;
    [System.NonSerialized] public int index = -1;

    void Awake()
    {
        rulerState = GetComponentInParent<RulerState>();
        oppositeCorner = rulerState.GetOpposite(this);
    }
    
    public void Fix()
    {
        clockwiseCorner.Project(transform);
        ccwCorner.ProjectCCW(transform);
        oppositeCorner.ProjectAdjacentIntersection();

        foreach (Ruler2dMouseDraggable bar in rulerState.bars)
            bar.FixBar();

        // Debug.Log("Fixing " + gameObject);
        foreach (Ruler2dReticle reticle in rulerState.reticles)
        {
            reticle.Fix();
            // Debug.Log("Fixing reticle " + reticle.gameObject);
        }
    }

    public void Project(Transform ray)
    {
        Vector3 idealOffset = transform.position - ray.position;
        Vector3 newOffset = Vector3.Project(idealOffset, ray.right);
        transform.position = ray.position + newOffset;

        transform.rotation = Quaternion.LookRotation(forward: ray.right, upwards: ray.up);
    }
    public void ProjectCCW(Transform ray)
    {
        Vector3 idealOffset = transform.position - ray.position;
        Vector3 newOffset = Vector3.Project(idealOffset, ray.forward * -1);
        transform.position = ray.position + newOffset;

        transform.rotation = Quaternion.LookRotation(forward: ray.right * -1, upwards: ray.up);
    }

    public void ProjectAdjacentIntersection()
    {
        Vector3 parallelOffset =   ccwCorner.transform.position - oppositeCorner.transform.position;
        transform.position = clockwiseCorner.transform.position + parallelOffset;
        transform.rotation = oppositeCorner.transform.rotation * Quaternion.Euler(0, 180, 0);
    }
}
