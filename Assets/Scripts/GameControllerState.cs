using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameControllerState : MonoBehaviour
{
    // private static MenuLocation _menuLocation;

    // public static MenuLocation menuLocation
    // {
    //     get { return _menuLocation; }
    //     set
    //     {
    //         Debug.Log("Changing _menuLocation from " + _menuLocation + " to " + value);
    //         _menuLocation = value;
    //     }
    // }
    public static MenuLocation menuLocation;

    private static bool _isXR_initialized = false;
    private static bool _isXR = false;
    public static bool isXR
    {
        get {
            if (!_isXR_initialized)
                FixIsXR();
            return _isXR;
        }
        set {
            _isXR = value;
        }
    }
    public static void FixIsXR()
    {
        try
        {
            List<XRDisplaySubsystem> xrDisplaySubsystems = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances<XRDisplaySubsystem>(xrDisplaySubsystems);
            _isXR = xrDisplaySubsystems.Count != 0;
            _isXR_initialized = true;
        } catch (System.Exception e) {
            Debug.LogError("Something went wrong fetching IsXR. This may be because you accessed isXR during Awake() before Start() when XR system is NOT initialized.");
            throw e;
        }
    }

    // This feels unclean, but it is apparently the canonical way to implement having one unique object.
    private static bool isFirst = true;
    void Awake()
    {
        if (!isFirst)
        {
            Destroy(gameObject);
            return;
        }
        isFirst = false;
        DontDestroyOnLoad(gameObject);
    }
}
