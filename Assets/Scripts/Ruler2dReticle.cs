using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruler2dReticle : MonoBehaviour, Fixable
{
    [SerializeField] Ruler2dMouseDraggable nearBar, farBar, cwBar, ccwBar;
    GameObject cube;
    RulerState rulerState;

    void Awake()
    {
        cube = transform.GetChild(0).gameObject;
        rulerState = GetComponentInParent<RulerState>();
    }

    public float height
    {
        get { return cube.transform.localScale.z; }
        set
        {
            Vector3 o = cube.transform.localScale;
            cube.transform.localScale = new Vector3(o.x, o.y, value);
        }
    }

    public float width
    {
        get => cube.transform.localScale.x;
        set
        {
            Vector3 o = cube.transform.localScale;
            cube.transform.localScale = new Vector3(value, o.y, o.z);
        }
    }

    public void Fix()
    {
        height = (cwBar.transform.position - ccwBar.transform.position).magnitude -
                 (cwBar.barGeometry.width + ccwBar.barGeometry.width) / 2f;
        
        float horizontalSpan = (nearBar.transform.position - farBar.transform.position).magnitude -
                               (nearBar.barGeometry.width + farBar.barGeometry.width) / 2f;
        float filledSpan = horizontalSpan - rulerState.tileWidth;
        width = filledSpan / 2f;
        
        transform.position = nearBar.transform.position + nearBar.transform.right * (width + nearBar.barGeometry.width) / 2f;
        transform.rotation = nearBar.transform.rotation;
    }
}
