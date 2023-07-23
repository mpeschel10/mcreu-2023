using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableScoreboard : MonoBehaviour, Fixable
{
    Quaternion forwardQuaternion = Quaternion.Euler(-29.198f, 0, 0);
    [SerializeField] Transform plateTransform;
    [SerializeField] Pillars pillars;
    Vector3 offsetFromCornerToCenter = new Vector3(-0.15f, -0.05f, 0f);

    public void SetTopRight(Vector3 topRight)
    {
        Debug.Log(topRight);
        transform.rotation = forwardQuaternion;
        transform.position = topRight + offsetFromCornerToCenter;
    }

    public void Fix()
    {
        SetTopRight(pillars.topRightPosition);
    }
}
