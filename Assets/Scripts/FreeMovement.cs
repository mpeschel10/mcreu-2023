using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FreeMovement : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float speed = 8;
    [SerializeField] float acceleration = 1.3f;
    [SerializeField] float lookSpeedX = 0.17f, lookSpeedY = 0.12f;
    InputAction move, look;
    private Transform grabTransform;
    // [SerializeField] float selectionRange = 50f;
    [SerializeField] LayerMask selectableMask = 64;

    void Start()
    {
        if (cameraTransform == null)
        {
            GameObject camera = GetComponentInChildren<Camera>().gameObject;
            if(camera == null)
            {
                throw new System.Exception(this + "cannot find camera component in its children or self.");
            }
            cameraTransform = camera.transform;
        }
        
        GameObject grabGameObject = new GameObject();
        grabTransform = grabGameObject.transform;
        grabTransform.SetParent(cameraTransform);

        if (layerNames.Length != layerColors.Length)
            throw new System.Exception("OutlineLayerColors: length of names " + layerNames.Length + " does not match length of colors " + layerColors.Length + ".");
        
        layerArray = new OutlineLayerColor[layerNames.Length];
        layerDictionary = new Dictionary<string, OutlineLayerColor>();
        for (int i = 0; i < layerNames.Length; i++)
        {
            string name = layerNames[i];
            if (layerDictionary.ContainsKey(name)) throw new System.Exception("OutlineLayerColors: duplicate name " + name + ".");
            
            OutlineLayerColor layer = new OutlineLayerColor(i, name, layerColors[i]);
            layerArray[i] = layer;
            layerDictionary[name] = layer;
        }

    }

    void Awake()
    {
        move = new InputAction(name: "move", type: InputActionType.Value);
        move.AddCompositeBinding("3DVector")
            .With("Up", "<Keyboard>/e")
            .With("Down", "<Keyboard>/q")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d")
            .With("Forward", "<Keyboard>/w")
            .With("Backward", "<Keyboard>/s");
        move.performed += OnMove;

        look = new InputAction(name: "look", type: InputActionType.Value);
        look.AddCompositeBinding("OneModifier")
            .With("Modifier", "<Mouse>/rightButton")
            .With("Binding", "<Mouse>/position");

        look.started   += OnLookStarted;
        look.performed += OnLookPerformed;
        look.canceled  += OnLookCanceled;
    }

    void OnEnable()
    {
        move.Enable();
        look.Enable();
    }
    void OnDisable()
    {
        move.Disable();
        look.Disable();
    }

    Vector3 movementPlayerWants;
    void OnMove(InputAction.CallbackContext context)
    {
        movementPlayerWants = context.ReadValue<Vector3>();
    }

    Vector2 clickLocation, lookPlayerWants;
    Vector3 clickRotation;
    void OnLookStarted(InputAction.CallbackContext context)
    {
        clickRotation = transform.rotation.eulerAngles;
        clickLocation = context.action.ReadValue<Vector2>();
    }
    void OnLookPerformed(InputAction.CallbackContext context)
    { lookPlayerWants = context.action.ReadValue<Vector2>() - clickLocation; }
    void OnLookCanceled(InputAction.CallbackContext context)
    { lookPlayerWants = Vector2.zero; }
    
    float currentSpeed = 0;
    // Update is called once per frame
    void Update()
    {
        DoMovement();
        
        if (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Physics.Raycast(ray, out RaycastHit raycastHit, float.PositiveInfinity, selectableMask);
            
            DoOutlines(raycastHit);
            DoClicks(raycastHit);
            // DoDrags(raycastHit); // Will need rewrite since transform offset is no longer fixed to camera
        } else {
            // Putting the outline removal code here is hacky and bad. I don't know how I can fix this.
            if (hoverable != null)
            {
                try {
                    hoverable.Unhover();
                } catch (System.Exception e) {
                    Debug.LogWarning(e);
                }
                hoverable = null;
            }
        }
    }

    void DoMovement()
    {
        if (movementPlayerWants != Vector3.zero)
        {
            if (currentSpeed < 1)
                currentSpeed = 1;
            currentSpeed *= (float) System.Math.Pow(acceleration, Time.deltaTime);
            
            Transform t = cameraTransform;
            Vector3 movementPlayerGets = (t.right   * movementPlayerWants.x +
                                          t.up      * movementPlayerWants.y +
                                          t.forward * movementPlayerWants.z);
            movementPlayerGets *= Time.deltaTime * speed * currentSpeed;
            transform.Translate(movementPlayerGets, Space.World);
        } else {
            currentSpeed = 0;
        }

        if (lookPlayerWants != Vector2.zero)
        {
            float xRotation = lookPlayerWants.y * -lookSpeedY;
            float yRotation = lookPlayerWants.x * lookSpeedX;
            Quaternion newRotation = Quaternion.Euler(clickRotation.x + xRotation, clickRotation.y + yRotation, clickRotation.z);
            transform.rotation = newRotation;
        }
    }

    public interface Hoverable {
        public void Hover(); public void Unhover();
        public GameObject GetGameObject(); // Interfaces cannot expose instance fields...
    }

    public interface Draggable {
        public void Grab(Transform transform); public void Ungrab();
    }

    public interface Clickable {
        public void Click();
    }

    Hoverable hoverable;
    void DoOutlines(RaycastHit hitInfo)
    {
        if ((hoverable == null && hitInfo.collider == null) ||
            (hitInfo.collider != null && hoverable != null && hitInfo.collider.gameObject == hoverable.GetGameObject()))
            return; // Nothing has changed, so don't flip-flop the outline.

        if (hoverable != null)
        {
            try {
                hoverable.Unhover();
            } catch (System.Exception e) {
                Debug.LogWarning(e);
            }
            hoverable = null;
        }

        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out hoverable))
            {
                hoverable.Hover();
            } else  {
                Debug.LogError("gameObject " + hitInfo.collider.gameObject + " on selectable layer has no Hoverable");
            }
        }
    }

    void DoClicks(RaycastHit hitInfo)
    {
        if (hitInfo.collider != null && Input.GetMouseButtonDown(0))
        {
            GameObject gameObject = hitInfo.collider.gameObject;
            if (gameObject.TryGetComponent(out Clickable clickable))
            {
                clickable.Click();
            } else {
                return;
            }
        }
    }

    Draggable dragging;
    void DoDrags(RaycastHit hitInfo)
    {
        if (hitInfo.collider != null && Input.GetMouseButtonDown(0))
        {
            if (dragging != null) // We missed a GetMouseButtonUp() somewhere; normalize.
            {
                dragging.Ungrab();
                dragging = null;
            }
            dragging = hitInfo.collider.gameObject.GetComponentInParent<Draggable>();
            if (dragging != null)
            {
                // grabTransform is child of this.transform.
                // Fix the grab relative to the camera.
                grabTransform.position = hitInfo.point;
                dragging.Grab(grabTransform);
            }
        }
        if (dragging != null && Input.GetMouseButtonUp(0))
        {
            dragging.Ungrab();
            dragging = null;
        }
    }

    public string[] layerNames = { "can-drag", "can-click" };
    public Color[] layerColors = { Color.blue, Color.green };

    public OutlineLayerColor[] layerArray;
    public Dictionary<string, OutlineLayerColor> layerDictionary;

    public class OutlineLayerColor
    {
        public int index; public string name; public Color color;
        public OutlineLayerColor(int index, string name, Color color)
        {
            this.index = index; this.name = name; this.color = color;
        }
    }

}
