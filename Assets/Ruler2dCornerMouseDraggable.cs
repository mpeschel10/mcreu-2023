using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MouseDraggable))]
public class Ruler2dCornerMouseDraggable : MonoBehaviour, Fixable
{
    [SerializeField] Ruler2dCornerMouseDraggable clockwiseCorner, ccwCorner;
    RulerState rulerState;

    void Awake()
    {
        rulerState = GetComponentInParent<RulerState>();
    }
    
    public void Fix()
    {
        clockwiseCorner.Project(transform);
        ccwCorner.ProjectCCW(transform);

        foreach (Ruler2dMouseDraggable bar in rulerState.bars)
            bar.FixBar();

        foreach (Ruler2dReticle reticle in rulerState.reticles)
            reticle.Fix();
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
}
