using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RulerState : MonoBehaviour
{
    public Ruler2dCornerMouseDraggable[] corners;
    public Ruler2dMouseDraggable[] bars;
    public Ruler2dReticle[] reticles;
    [SerializeField] Transform cellScale;
    public float tileWidth
    {
        get => cellScale.lossyScale.x;
    }

    public Ruler2dGrabbable primaryIntent, secondaryIntent;
    public void Grab(MyGrabber grabber, Ruler2dGrabbable segment)
    {
        if (primaryIntent == null)
        {
            primaryIntent = segment;
            Fix();
            return;
        }

        // Since we have only two hands, assume secondaryIntent must be null
        // primaryIntent != null && secondaryIntent == null.
        secondaryIntent = segment;
        Fix();
    }

    public void Ungrab(Ruler2dGrabbable segment)
    {
        // Assume primaryIntent != null && (segment == primaryIntent ^ segment == secondaryIntent)
        if (segment == primaryIntent)
            primaryIntent = secondaryIntent;
        secondaryIntent = null;
        Fix();
    }

    public Transform originalParent;
    void Awake()
    {
        originalParent = transform.parent;
        enabled = false;
    }

    Ruler2dMouseDraggable _primaryTarget, _secondaryTarget;
    Ruler2dMouseDraggable primaryTarget
    {
        get => _primaryTarget;
        set
        {
            _primaryTarget?.transform.SetParent(primaryOriginalParent);
            _primaryTarget = value;
            primaryOriginalParent = _primaryTarget?.transform.parent;
        }
    }
    Ruler2dMouseDraggable secondaryTarget
    {
        get => _secondaryTarget;
        set
        {
            _secondaryTarget?.transform.SetParent(secondaryOriginalParent);
            _secondaryTarget = value;
            secondaryOriginalParent = _secondaryTarget?.transform.parent;
        }
    }

    Ruler2dMouseDraggable GetNearestBar(Vector3 position)
    {
        float bestDistance = float.PositiveInfinity;
        Ruler2dMouseDraggable best = null;
        foreach (Ruler2dMouseDraggable candidate in bars)
        {
            float distance = (candidate.transform.position - position).magnitude;
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = candidate;
            }
        }
        return best;
    }

    (Ruler2dMouseDraggable, Ruler2dMouseDraggable) AssignOpposingBars(Vector3 p1, Vector3 p2)
    {
        Ruler2dMouseDraggable p1Close = GetNearestBar(p1);
        Ruler2dMouseDraggable p2Close = GetNearestBar(p2);
        // Imagine an x in our ruler square from corner to corner.
        // p1 and p2 can be in the same triangle, adjacent triangles, or opposite triangles.
        // We need to return opposite triangles sensibly.
        if (p1Close.oppositeBar == p2Close)
        {
            // Primary and secondary are already opposites. Convenient!
        }
        else if (p1Close == p2Close)
        {
            // Primary and secondary are in same barycenter.
            // Select the "adjacent" bars and assign based on who's closer.
            p1Close = p1Close.frontBar;
            if ((p2 - p1Close.transform.position).magnitude <
                (  p1 - p1Close.transform.position).magnitude
                )
            {
                p1Close = p1Close.oppositeBar;
            }
        }
        else
        {
            // Primary and secondary are diagonal.
            // idk; give up.
            // TODO something about giving priority to whichever pinch point is closer to its bar?
        }
        return (p1Close, p1Close.oppositeBar);
    }

    Transform primaryOriginalParent, secondaryOriginalParent;
    public void Fix()
    {
        if (primaryIntent == null || secondaryIntent == null)
        {
            primaryTarget = null;
            secondaryTarget = null;
            enabled = false;
            if (primaryIntent == null)
            {
                transform.SetParent(originalParent);
            }
            else if (primaryIntent != null && secondaryIntent == null)
            {
                transform.SetParent(primaryIntent.grabber.transform);
            }
        }
        else // primaryIntent != null && secondaryIntent != null
        {
            transform.SetParent(originalParent);

            Vector3   primaryPosition =   primaryIntent.grabber.pinchPoint.transform.position;
            Vector3 secondaryPosition = secondaryIntent.grabber.pinchPoint.transform.position;

            (primaryTarget, secondaryTarget) = AssignOpposingBars(primaryPosition, secondaryPosition);
            
              primaryTarget.transform.SetParent(  primaryIntent.grabber.transform);
            secondaryTarget.transform.SetParent(secondaryIntent.grabber.transform);

            enabled = true;
        }
    }

    void Update()
    {
        primaryTarget.XRFixPrimary();
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
}
