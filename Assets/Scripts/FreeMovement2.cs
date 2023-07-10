using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FreeMovement2 : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] float speed = 8;
    [SerializeField] float acceleration = 1.3f, minimumSpeed = 1;
    [SerializeField] float lookSpeedX = 0.17f, lookSpeedY = 0.12f;
    InputAction move, look;
    private Transform grabTransform;

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

    void OnDestroy()
    {
        move.performed -= OnMove;
        look.started   -= OnLookStarted;
        look.performed -= OnLookPerformed;
        look.canceled  -= OnLookCanceled;
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
    }

    void DoMovement()
    {
        if (movementPlayerWants != Vector3.zero)
        {
            if (currentSpeed < minimumSpeed)
                currentSpeed = minimumSpeed;
            currentSpeed += speed * Time.deltaTime;
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


}
