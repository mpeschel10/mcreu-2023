using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ruler2dGrabbable : MonoBehaviour, MyGrabber.Grabbable
{
    RulerState rulerState;
    public bool isPrimary = false;
    void Awake()
    {
        rulerState = GetComponentInParent<RulerState>();
    }

    [System.NonSerialized] public MyGrabber grabber = null;
    public void Grab(MyGrabber grabber)
    {
        this.grabber?.OnUngrab();
        this.grabber = grabber;
        rulerState.Grab(grabber);
    }

    public void Ungrab(MyGrabber grabber)
    {
        rulerState.Ungrab(grabber);
        this.grabber = null;
    }
}


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Ruler2dGrabbable : MonoBehaviour, MyGrabber.Grabbable
// {
//     RulerState rulerState;
//     public bool isPrimary = false;
//     [System.NonSerialized] public MyGrabber primaryGrabber, secondaryGrabber;
//     void Awake()
//     {
//         rulerState = GetComponentInParent<RulerState>();
//     }

//     // [System.NonSerialized] public MyGrabber grabber = null;
//     public void Grab(MyGrabber grabber)
//     {
//         if (primaryGrabber != null)
//             this.primaryGrabber = grabber;
//         else
//             this.secondaryGrabber = grabber;
//         rulerState.Grab(grabber, this);
//     }

//     public void Ungrab(MyGrabber grabber)
//     {
//         rulerState.Ungrab(grabber, this);
//         if (grabber == primaryGrabber)
//             primaryGrabber = secondaryGrabber;
//         secondaryGrabber = null;
//     }
// }
