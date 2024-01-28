using System;
using System.Collections;
using System.Reflection;
using Unity.Cinemachine;
using Unity.Sentis.Layers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchManager : MonoBehaviour
{
    // serialized
    [SerializeField]
    private ConstantForce cf;
    [SerializeField]
    private GameObject cameraBrain;
    [SerializeField]
    private float maxAngularVelocity;
    [SerializeField]
    private float spinForce;
    [SerializeField]
    private float airSpinMultiplier;
    [SerializeField]
    private float timeScale;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private Material highlightMaterial;

    // private
    public GameObject gravityPoint;
    private Rigidbody rb;
    private Camera mainCamera;
    private CinemachineInputAxisController cameraInputProvider;
    private bool grounded;
    private Vector2 screenCenter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
        EnhancedTouchSupport.Enable();
        mainCamera = Camera.main;
        cameraInputProvider = cameraBrain.GetComponent<CinemachineInputAxisController>();
        screenCenter = new Vector2(Screen.width/2, Screen.height/2);
    }


    private void OnEnable()
    {
        Touch.onFingerUp += TouchReleased;
        Touch.onFingerMove += TouchHeld;
    }

    private void OnDisable()
    {
        Touch.onFingerUp -= TouchReleased;
        Touch.onFingerMove -= TouchHeld;
    }

    private void TouchHeld(Finger finger)
    {
        if (Touch.activeFingers.Count < 2)
        {
            // get vectors
            Vector2 swipeVector = screenCenter - finger.currentTouch.screenPosition;
            Vector3 forwardRelativeInput = swipeVector.x * mainCamera.transform.forward;     //  left and right
            Vector3 rightRelativeInput = swipeVector.y * mainCamera.transform.right;    //  up and down
            Vector3 upRelativeInput = swipeVector.y * mainCamera.transform.up;    //  jump
            Vector3 cameraRelativeSpin = -rightRelativeInput + forwardRelativeInput;
            Vector3 cameraRelativeMovement = upRelativeInput + rightRelativeInput;

            float distance = Vector3.Distance(rb.position, mainCamera.ScreenToWorldPoint(finger.currentTouch.screenPosition));


            rb.AddTorque(cameraRelativeSpin.normalized * distance * spinForce, ForceMode.Impulse);
            //rb.MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(rb.position, finger.currentTouch.screenPosition, rotationSpeed * Time.deltaTime, 0f)));

        }

        if (Touch.activeFingers.Count >= 2)
        {
            cameraInputProvider.enabled = true;
        }
    }

    private void TouchReleased(Finger finger)
    {
        cameraInputProvider.enabled = false;


        //rb.MoveRotation(Quaternion.LookRotation(Vector3.RotateTowards(rb.position, finger.currentTouch.screenPosition, rotationSpeed * Time.deltaTime, 0f)));

        if (!grounded && Touch.activeFingers.Count < 2)
        {
            // get vectors
            Vector2 swipeVector = screenCenter - finger.currentTouch.screenPosition;
            Vector3 forwardRelativeInput = swipeVector.x * mainCamera.transform.forward;     //  left and right
            Vector3 rightRelativeInput = swipeVector.y * mainCamera.transform.right;    //  up and down
            Vector3 upRelativeInput = swipeVector.y * mainCamera.transform.up;    //  jump
            Vector3 cameraRelativeSpin = -rightRelativeInput + forwardRelativeInput;
            Vector3 cameraRelativeMovement = upRelativeInput + rightRelativeInput;

            cameraRelativeMovement *= airSpinMultiplier; // only for one spin
            rb.AddTorque(cameraRelativeSpin.normalized * spinForce, ForceMode.Impulse);
        }


        if (Touch.activeFingers.Count >= 2)
        {
            // Gravity
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100) && hit.transform.tag == "hand")
            {
                gravityPoint = hit.transform.gameObject;
                gravityPoint.GetComponent<MeshRenderer>().material = highlightMaterial;
            }
        }
        // PullDownTowardsRealGravity(down);
    }

    private void OnCollisionExit(Collision collision)
    {
        //grounded = false;
    }

    void FixedUpdate()
    {
        if (gravityPoint)
        {
            Vector3 gravityUp = (transform.position - gravityPoint.transform.position).normalized;
            cf.force = (-gravityUp * gravityPoint.GetComponent<GravityPoint>().gravity) * rb.mass;
        }
    }
}

/**
 * TODO:
 *
 * BUGS:
 *
*/