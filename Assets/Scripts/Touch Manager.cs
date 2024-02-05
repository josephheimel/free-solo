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
    [SerializeField]
    private float staminaDuration;

    // public
    public GameObject gripGravityPoint;

    // private
    private GameObject jumpGravityPoint;
    private Vector3 gravityConstant;
    private Rigidbody rb;
    private Camera mainCamera;
    private CinemachineInputAxisController cameraInputProvider;
    private Vector2 screenCenter;
    private Material originalMaterial;
    private bool stamina;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = maxAngularVelocity;
        EnhancedTouchSupport.Enable();
        mainCamera = Camera.main;
        cameraInputProvider = cameraBrain.GetComponent<CinemachineInputAxisController>();
        screenCenter = new Vector2(Screen.width/2, Screen.height/2);
        gravityConstant = Physics.gravity;
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
        // Gravity
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100) && hit.transform.tag == "hand")
        {
            jumpGravityPoint = hit.transform.gameObject;
            originalMaterial = GetComponent<MeshRenderer>().material;
            jumpGravityPoint.GetComponent<MeshRenderer>().material = highlightMaterial;
        }
    }

    IEnumerator LoseStamina()
    {
        yield return new WaitForSeconds(staminaDuration);
        gripGravityPoint = null;
        jumpGravityPoint = null;
        cf.force = Vector3.zero;
    }

    private void TouchHeld(Finger finger)
    {
        if (Touch.activeFingers.Count == 1)
        {
            // get vectors
            //Vector2 swipeVector = screenCenter - finger.currentTouch.screenPosition;
            Vector2 touchPosition = finger.currentTouch.screenPosition;
            Vector3 forwardRelativeInput = touchPosition.x * mainCamera.transform.forward;     //  left and right
            Vector3 rightRelativeInput = touchPosition.y * mainCamera.transform.right;    //  up and down
            Vector3 upRelativeInput = touchPosition.y * mainCamera.transform.up;    //  jump
            Vector3 cameraRelativeSpin = -rightRelativeInput + forwardRelativeInput;

            float distance = Vector3.Distance(rb.position, mainCamera.ScreenToWorldPoint(finger.currentTouch.screenPosition));

            rb.AddTorque(cameraRelativeSpin.normalized * distance * spinForce, ForceMode.Impulse);
        }

        if (Touch.activeFingers.Count >= 2)
        {
            cameraInputProvider.enabled = true;
        }
    }

    private void TouchReleased(Finger finger)
    {
        cameraInputProvider.enabled = false;

        if (jumpGravityPoint)
        {
            gripGravityPoint = null;
            jumpGravityPoint.GetComponent<MeshRenderer>().material = originalMaterial;
            jumpGravityPoint = null;

            cf.force = Vector3.zero;
        }
    }

    void FixedUpdate()
    {
        if (jumpGravityPoint)
        {
            Vector3 gravityUp = (transform.position - jumpGravityPoint.transform.position).normalized;
            cf.force = (-gravityUp * jumpGravityPoint.GetComponent<GravityPoint>().jumpGravity) * rb.mass;
        }
        else if (gripGravityPoint)
        {
            StartCoroutine(LoseStamina());
            Vector3 gravityUp = (transform.position - gripGravityPoint.transform.position).normalized;
            cf.force = (-gravityUp * gripGravityPoint.GetComponent<GravityPoint>().gripGravity) * rb.mass;
        }
    }
}

/**
 * TODO:
 *
 * BUGS:
 *
*/