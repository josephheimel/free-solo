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
    private GameObject cameraBrain;
    [SerializeField]
    private float maxAngularVelocity;
    [SerializeField]
    private float spinForce;
    [SerializeField]
    private float airSpinMultiplier;
    [SerializeField]
    private float timeScale;


    // private
    private Rigidbody rb;
    private Camera mainCamera;
    private CinemachineInputAxisController cameraInputProvider;
    private bool grounded;
    private Vector2 screenCenter;
    private bool held;
    private Vector3 gravityCenter;
    private bool dynamicGravity;

    //private Vector3 ;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
        EnhancedTouchSupport.Enable();
        mainCamera = Camera.main;
        cameraInputProvider = cameraBrain.GetComponent<CinemachineInputAxisController>();
        screenCenter = new Vector2(Screen.width/2, Screen.height/2);
        gravityCenter = Vector3.down * 9.81f;
        dynamicGravity = false;
    }


    private void OnEnable()
    {
        Touch.onFingerDown += TouchPressed;
        Touch.onFingerUp += TouchReleased;
        Touch.onFingerMove += TouchHeld;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= TouchPressed;
        Touch.onFingerUp -= TouchReleased;
        Touch.onFingerMove -= TouchHeld;
    }

    private void TouchPressed(Finger finger)
    {
        StartCoroutine(holdTimer());

        held = false;
    }


    private IEnumerator holdTimer()
    {
        yield return new WaitForSeconds(timeScale);

        held = true;
    }

    private void TouchHeld(Finger finger)
    {
        if (held)
        {
            cameraInputProvider.enabled = true;
        }
    }

    private void TouchReleased(Finger finger)
    {
        cameraInputProvider.enabled = false;

        if (!held)
        {
            // get vectors
            Vector2 swipeVector = screenCenter - finger.currentTouch.screenPosition;
            Vector3 forwardRelativeInput = swipeVector.x * mainCamera.transform.forward;     //  left and right
            Vector3 rightRelativeInput = swipeVector.y * mainCamera.transform.right;    //  up and down
            Vector3 upRelativeInput = swipeVector.y * mainCamera.transform.up;    //  jump
            Vector3 cameraRelativeSpin = -rightRelativeInput + forwardRelativeInput;
            Vector3 cameraRelativeMovement = upRelativeInput + rightRelativeInput;

            if (!grounded)
            {
                cameraRelativeMovement *= airSpinMultiplier; // only for one spin
            }

            rb.AddTorque(cameraRelativeSpin.normalized * spinForce, ForceMode.Impulse);


            if (Touch.activeFingers.Count >= 2)
            {
                // Gravity
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100) && hit.transform.tag == "hand")
                {
                    gravityCenter = (hit.transform.position - transform.position).normalized * 9.81f;
                    dynamicGravity = true;
                }
            }

            // PullDownTowardsRealGravity(down);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        //grounded = false;
    }

    private void Update()
    {
        if (dynamicGravity)
        {
            Vector3 down = (gravityCenter - transform.position).normalized * 9.81f;
            Physics.gravity = down;
        }
    }
}

/**
 * TODO:
 *
 * BUGS:
 *
*/