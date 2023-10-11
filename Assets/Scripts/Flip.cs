using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using System;

public class Flip : MonoBehaviour
{
    // private
    private Rigidbody rb;
    private Renderer renderer;
    private Vector2 centerOfMass;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        renderer = GetComponent<Renderer>();
        centerOfMass = Vector3.zero;
        EnhancedTouchSupport.Enable();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        Touch.onFingerUp += TouchPressed;
    }

    private void OnDisable()
    {
        Touch.onFingerUp -= TouchPressed;
    }

    private void TouchPressed(Finger finger)
    {
        Debug.DrawLine(transform.position, centerOfMass);


        // get vector
        Vector2 swipeVector = finger.currentTouch.startScreenPosition - finger.currentTouch.screenPosition;

        // get magnitude and normal
        float magnitude = swipeVector.magnitude;
        Vector2 normalVector = swipeVector.normalized;

        Vector2 radius = transform.position + renderer.bounds.size / 4;
        centerOfMass = normalVector * radius.magnitude;

        // invert x direction (does not match swipeDirection)
        normalVector.x = -normalVector.x;

        // set rotation point
        rb.centerOfMass = centerOfMass;
        rb.AddRelativeForce(normalVector, ForceMode.Impulse);

        //       Try This.                   rb.AddForceAtPosition()

        // rb.AddRelativeForce(normalVector, ForceMode.Impulse);
        //rb.MoveRotation(transform.rotation * Quaternion.AngleAxis(180, normalVector));

        if (Math.Sign(normalVector.y) == -1)
        {
            //rb.AddRelativeTorque(Vector2.up, ForceMode.Impulse);
        }

        // reset
        rb.ResetCenterOfMass();
    }
}

/**
 * TODO:
 * - ring prefab
 *
 * BUGS:
 *
 *
 *
 *
 *
 *void FixedUpdate()
{
    Vector3 movement;

    float moveHorizontal = Input.GetAxis("Horizontal");
    float moveVertical = Input.GetAxis("Vertical");

    GameObject cameraCompass = new GameObject("Camera Compass");
    // "Correct" the compass so that it lies flat in the game plane
    cameraCompass.transform.eulerAngles = new Vector3(0, GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0f);

    Vector3 movementX = cameraCompass.transform.right * moveHorizontal;
    Vector3 movementZ = cameraCompass.transform.forward * moveVertical;
    movement = movementX + movementZ;

    rb.AddForce(movement * speed);

    Destroy(cameraCompass); // We don't need the Compass anymore!
}
*/
