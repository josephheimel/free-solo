using Cinemachine;
using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchManager : MonoBehaviour
{
    // serialized
    [SerializeField]
    private GameObject cinemachineCamera;
    [SerializeField]
    private MeshRenderer planeRenderer;
    [SerializeField]
    private float maxAngularVelocity;
    [SerializeField]
    private float clingForce;
    [SerializeField]
    private float spinForce;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float airSpinMultiplier;


    // private
    private Rigidbody rb;
    private Camera mainCamera;
    private CinemachineInputProvider cameraMovement;
    private CinemachineFreeLook cameraBrain;
    private bool grounded;
    private Vector2 screenCenter;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
        EnhancedTouchSupport.Enable();
        mainCamera = Camera.main;
        cameraMovement = cinemachineCamera.GetComponent<CinemachineInputProvider>();
        cameraBrain = cinemachineCamera.GetComponent<CinemachineFreeLook>();
        screenCenter = new Vector2(Screen.width/2, Screen.height/2);
    }

    private void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.velocity);
        localVelocity.z = 0;

        rb.velocity = transform.TransformDirection(localVelocity);
    }

    private void OnEnable()
    {
        Touch.onFingerDown += TouchPressed;
        Touch.onFingerUp += TouchReleased;
        Touch.onFingerMove += TouchMoved;
    }

    private void OnDisable()
    {
        Touch.onFingerDown -= TouchPressed;
        Touch.onFingerUp -= TouchReleased;
        Touch.onFingerMove -= TouchMoved;
    }

    private void TouchPressed(Finger finger) // fix to be only at specific time of collision -> tap in mid air to spin and sssssuuuuper jump
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
            rb.maxAngularVelocity = maxAngularVelocity;
            cameraRelativeMovement *= airSpinMultiplier; // only for one spin
        }

        rb.AddTorque(cameraRelativeSpin.normalized * spinForce, ForceMode.Impulse); // limit forward and back based on camera
    }

    private void TouchMoved(Finger finger)
    {
        if (Touch.activeFingers.Count >= 2)
        {
            cameraMovement.enabled = true;
            // planeRenderer.enabled = true;

            float cameraHeight = cameraBrain.m_YAxis.Value - 0.5f;
            rb.centerOfMass = new Vector3(0, cameraHeight/10, 0);
        }
    }

    private void TouchReleased(Finger finger)
    {
        cameraMovement.enabled = false;
        // planeRenderer.enabled = false;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "hand") // AND holding and has stamina
        {
            foreach (ContactPoint c in collision.contacts) {
                rb.AddForce(-c.normal * clingForce, ForceMode.Impulse);
            }
            //rb.useGravity = false;
        }

        grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        grounded = false;
        //rb.useGravity = true;   // may cause errors with exiting other collisions
    }
}

/**
 * TODO:
 *
 * BUGS:
 *
*/