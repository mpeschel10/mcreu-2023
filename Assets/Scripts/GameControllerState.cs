using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GameControllerState : MonoBehaviour
{
    static MenuLocation _menuLocation; // Set in Awake() for some reason
    public static MenuLocation menuLocation
    {
        get => _menuLocation;
        set
        {
            if (value == MenuLocation.LeftHand || value == MenuLocation.RightHand)
                _lastMenuHand = value;
            _menuLocation = value;
        }
    }
    static MenuLocation _lastMenuHand = MenuLocation.LeftHand;
    public static MenuLocation lastMenuHand
    {
        get => _lastMenuHand;
    }
    public static ActiveCamera activeCamera;
    
    static Material _defaultHoverMaterial;
    public static Material defaultHoverMaterial
    {
        get
        {
            if (_defaultHoverMaterial == null)
            {
                _defaultHoverMaterial = GameObject.FindGameObjectWithTag("GameController")
                                                  .GetComponent<GameControllerState>()
                                                  .defaultHoverMaterialInstance;
            }
            return _defaultHoverMaterial;
        }
    }

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
            // I've moved the load order around and this is problem should be impossible now.
            // Also, I'm not sure that it ever threw an error; just returned an empty list.
            Debug.LogError("Something went wrong fetching IsXR. This may be because you accessed isXR during Awake() before Start() when XR system is not initialized.");
            throw e;
        }
    }

    // The frickin' tag system doesn't let you search for inactive objects,
    // so keep a reference to the inactive positionMarker convenient. I guess I'm supposed to use a prefab instead.
    public static GameObject positionMarker;
    public static GameControllerState oneTrueInstance;
    void Fix()
    {
        GameObject candidateObject = GameObject.FindGameObjectWithTag("PositionMarker");
        if (candidateObject != null)
        {
            positionMarker = candidateObject;
            positionMarker.SetActive(false);
            Debug.Log("Set position marker " + positionMarker);
        }
        FixIsXR();
    }

    [SerializeField] Material defaultHoverMaterialInstance;
    [SerializeField] GameObject defaultPositionMarkerInstance;
    void Awake()
    {
        if (oneTrueInstance != null)
        {
            oneTrueInstance.Fix();
            Destroy(gameObject);
            return;
        }
        Debug.Log("!!---------------!! Beginning of application log. !!---------------!!");
        if (isXR)
        {
            // Debug.Log("Gamecontroller awake says isXR");
            menuLocation = MenuLocation.LeftHand;
        }
        else
        {
            // Debug.Log("Gamecontroller awake says not isXR");
            menuLocation = MenuLocation.FullScreen;
        }
        oneTrueInstance = this;
        DontDestroyOnLoad(gameObject);
        Fix();
    }

    void Start()
    {
        FixIsXR(); // Just in case I messed it up by calling isXR in Awake
        // Debug.Log("As of gamecontroller state start , menuLocation is " + menuLocation);
    }

    public static void Mark(Vector3 position)
    {
        GameObject.Instantiate(positionMarker, position, positionMarker.transform.rotation).SetActive(true);
    }
}
