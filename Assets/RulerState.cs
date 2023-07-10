using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerState : MonoBehaviour
{
    public Ruler2dCornerMouseDraggable[] corners;
    public Ruler2dMouseDraggable[] bars;
    public Ruler2dReticle[] reticles;
    public float tileWidth
    {
        get => cellScale.lossyScale.x;
    }

    GameObject[] _rulerParts;
    public GameObject[] rulerParts
    {
        get
        {
            if (_rulerParts == null)
            {
                _rulerParts = new GameObject[corners.Length + bars.Length + reticles.Length];
                int i = 0;
                foreach (MonoBehaviour[] scripts in new MonoBehaviour[][] {corners, bars, reticles})
                {
                    foreach (MonoBehaviour script in scripts)
                    {
                        _rulerParts[i++] = script.gameObject;
                    }
                }
            }
            return _rulerParts;
        }
    }
    [SerializeField] Transform cellScale;
}
