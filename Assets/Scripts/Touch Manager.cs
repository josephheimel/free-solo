using Cinemachine;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class TouchManager : MonoBehaviour
{
    // serialized
    [SerializeField]
    private float speed;
    [SerializeField]
    private float explosionForce;
    [SerializeField]
    private GameObject cinemachineCamera;
    [SerializeField]
    private MeshRenderer planeRenderer;

    // private
    private Rigidbody rb;
    private Camera mainCamera;
    private CinemachineInputProvider cameraMovement;
    private CinemachineFreeLook cameraBrain;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        EnhancedTouchSupport.Enable();
        mainCamera = Camera.main;
        cameraMovement = cinemachineCamera.GetComponent<CinemachineInputProvider>();
        cameraBrain = cinemachineCamera.GetComponent<CinemachineFreeLook>();
    }

    private void OnEnable()
    {
        Touch.onFingerUp += TouchReleased;
        Touch.onFingerMove += TouchMoved;
    }

    private void OnDisable()
    {
        Touch.onFingerUp -= TouchReleased;
        Touch.onFingerMove -= TouchMoved;
    }

    private void TouchMoved(Finger finger)
    {

        if (Touch.activeFingers.Count >= 2)
        {
            cameraMovement.enabled = true;
            planeRenderer.enabled = true;

            float cameraHeight = cameraBrain.m_YAxis.Value - 0.5f;
            rb.centerOfMass = new Vector3(0, cameraHeight, 0);
        }

        if (Touch.activeFingers.Count < 2)
        {
            // get vectors
            Vector2 swipeVector = finger.currentTouch.startScreenPosition - finger.currentTouch.screenPosition;
            Vector3 upRelativeInput = swipeVector.x * mainCamera.transform.forward;     //  left and right
            Vector3 rightRelativeInput = swipeVector.y * mainCamera.transform.right;    //  up and down
            Vector3 cameraRelativeMovement = -rightRelativeInput + upRelativeInput;

            rb.AddTorque(cameraRelativeMovement, ForceMode.Impulse);
        }
    }

    private void TouchReleased(Finger finger)
    {
        cameraMovement.enabled = false;
        planeRenderer.enabled = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Input.GetKey("space"))
        {
            rb.AddExplosionForce(explosionForce, collision.contacts[0].point, 1, 1, ForceMode.Impulse);
        }
    }
}

/**
 * TODO:
 *
 * BUGS:
 *
*/