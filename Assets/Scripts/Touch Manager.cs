using Cinemachine;
using System;
using System.Collections;
using System.Reflection;
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
    private CinemachineInputProvider cameraInputProvider;
    private bool grounded;
    private Vector2 screenCenter;
    private bool held;
    //private Vector3 ;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
        EnhancedTouchSupport.Enable();
        mainCamera = Camera.main;
        cameraInputProvider = cameraBrain.GetComponent<CinemachineInputProvider>();
        screenCenter = new Vector2(Screen.width/2, Screen.height/2);

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



            //float cameraHeight = cameraBrain.m_YAxis.Value - 0.5f;
            //rb.centerOfMass = new Vector3(0, cameraHeight/10, 0);

            // Gravity
            Vector3 down = Vector3.Reflect(mainCamera.transform.up.normalized * 9.81f, Vector3.up);

            Debug.DrawLine(transform.position, mainCamera.transform.up.normalized * 9.81f);
            Debug.DrawLine(transform.position, down);
            // PullDownTowardsRealGravity(down);
            Physics.gravity = down;
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
        }
    }

    /*
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "hand") // AND holding and has stamina
        {
            foreach (ContactPoint c in collision.contacts) {
                rb.AddForce(-c.normal * clingForce, ForceMode.Impulse);
            }
        }

        grounded = true;
    }
    */

    private void OnCollisionExit(Collision collision)
    {
        //grounded = false;
    }
}

/**
 * TODO:
 *
 * BUGS:
 *
*/