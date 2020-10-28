
//==================================================================================================================
//
// WebXRMovev2
// Same controller than WebXRMove.cs but using a CharacterController so it has collisions.
//
//==================================================================================================================

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using WebXR;


public class WebXRMovev2 : MonoBehaviour
{
    [Tooltip("Enable/disable rotation control. For use in Unity editor only.")]
    public bool XRMoveEnabled = false;

    [Tooltip("Enable/disable rotation control. For use in Unity editor only.")]
    public bool NonXRMoveEnabled = false;

    public WebXRInputManager inputManagerLeftHand;
    public WebXRInputManager inputManagerRightHand;
    public Transform cameraMainTransform;
    public Transform cameraLeftTransform;
    private Camera myCamera;

    private WebXRDisplayCapabilities capabilities;

    [Tooltip("Mouse sensitivity")]
    public float mouseSensitivity = 1f;

    [Tooltip("Movement Speed")]
    public float speed = 5f;
    public bool canStrafe = true;

    [Tooltip("Straffe Speed")]
    public float rotationAngle = 15f;


    public Text stickText;
    public Text cameraText;

    private float minimumX = -360f;
    private float maximumX = 360f;

    private float minimumY = -90f;
    private float maximumY = 90f;

    private float rotationX = 0f;
    private float rotationY = 0f;

    private Quaternion originalRotation;
    private WebXRInputManager inputManager;
    private Transform myTransform;
    private Transform cameraTransform;  // Camera used to move. Used in MoveXR()
    private CharacterController controller;
    private CollisionFlags flags;

    private IEnumerator Start()
    {
        myTransform = transform;
        myCamera = cameraMainTransform.GetComponent<Camera>();
        originalRotation = transform.localRotation;
        controller = GetComponent<CharacterController> (); 

        #if UNITY_EDITOR
        cameraTransform = cameraMainTransform;
        //EnableXRMove(true);   // In the editor we decide the control to use (in Inspector)
        #elif UNITY_WEBGL
        cameraTransform = cameraLeftTransform;
        EnableXRMove(false);
        #endif
        XRMoveEnabledPrev = !XRMoveEnabled;

        yield return new WaitForSeconds(0.5f);

        capabilities = WebXRManager.Instance.GetWebXRDisplayCapabilities();
        //Debug.Log("WebXRMove->Start:: vrCapabilities VR: " + capabilities.supportsImmersiveVR.ToString());
    }

    void OnEnable()
    {
        WebXRManager.Instance.OnXRChange += onXRChange;
        WebXRManager.Instance.OnXRCapabilitiesUpdate += onXRCapabilitiesUpdate;
    }

    void OnDisable()
    {
        WebXRManager.Instance.OnXRChange -= onXRChange;
        WebXRManager.Instance.OnXRCapabilitiesUpdate -= onXRCapabilitiesUpdate;
    }


    private void onXRChange(WebXRState state)
    {
        Debug.Log("onXRChange:: state: " + state.ToString());
        if (state == WebXRState.ENABLED)
        {
            EnableXRMove(true);
        }
        else
        {
            EnableXRMove(false);
        }
    }

    private void onXRCapabilitiesUpdate(WebXRDisplayCapabilities vrCapabilities)
    {
        Debug.Log("onXRCapabilitiesUpdate:: vrCapabilities VR: " + vrCapabilities.supportsImmersiveVR.ToString());
        capabilities = vrCapabilities;
        EnableAccordingToPlatform();
    }

    private bool XRMoveEnabledPrev = false;
    void Update()
    {
        // if (!XRDevice.isPresent && XRMoveEnabled) { EnableXRMove(false); }

        if (NonXRMoveEnabled) { MoveNonXR(); }
        else if(XRMoveEnabled) { MoveXR(); }

        if(XRMoveEnabledPrev != XRMoveEnabled)
        {
            Debug.Log("Update -> XRMoveEnabled: " + XRMoveEnabled);
            XRMoveEnabledPrev = XRMoveEnabled;
        }
        //Debug.Log("Update -> Left Stick: " + inputManagerLeftHand.stick + " - Right Stick: " + inputManagerRightHand.stick);
    }

    
    private void MoveXR()
    {
        if (XRTeleporterController.Instance.IsTeleporterActive()) return;

        // Traslation (Only translation forward, no sideways)
        if (inputManagerLeftHand != null)
        {

            float x = canStrafe ? inputManagerLeftHand.stick.x : 0;
            float z = -inputManagerLeftHand.stick.y;

            Vector3 dir = new Vector3(x, 0, z);
            MoveCharacterController(dir);
        }

        // Rotation (No rotation on Y -> Up/Down)
        if (inputManagerRightHand != null)
        {
            TickXRRotation();
        }

        // Write the values in the UI text in game screen
        if(stickText != null)
            stickText.text = "Left Stick: " + inputManagerLeftHand.stick + " - Right Stick: " + inputManagerRightHand.stick;

        /*if (inputManagerLeftHand.IsStickEnabled() || inputManagerRightHand.IsStickEnabled())
            Debug.Log("MoveXR -> Left Stick: " + inputManagerLeftHand.stick + " - Right Stick: " + inputManagerRightHand.stick);*/
    }

    private bool hasRotated = false;
    private float strickXPreb = 0;
    private void TickXRRotation()
    {
        if (XRTeleporterController.Instance.IsTeleporterActive()) return;

        if (strickXPreb < 0.1f && Mathf.Abs(inputManagerRightHand.stick.x) > 0.1f)
        {
            float angle = rotationAngle;
            if(inputManagerRightHand.stick.x < 0) { angle = -angle; }
            rotationX += angle;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            myTransform.localRotation = originalRotation * xQuaternion;
        }

        strickXPreb = Mathf.Abs(inputManagerRightHand.stick.x);
    }

    private void PlainXRRotation()
    {
        if (XRTeleporterController.Instance.IsTeleporterActive()) return;

        rotationX += inputManagerRightHand.stick.x * mouseSensitivity;
        rotationX = ClampAngle(rotationX, minimumX, maximumX);
        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);

        myTransform.localRotation = originalRotation * xQuaternion; // * yQuaternion;
    }

    private void MoveNonXR()
    {
        Vector3 dir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        MoveCharacterController(dir);

        // Rotation
        if (Input.GetMouseButton(0))
        {
            rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
    }


    private void EnableXRMove(bool _enable)
    {
        XRMoveEnabled = _enable;
        NonXRMoveEnabled = !XRMoveEnabled;
        Debug.Log("EnableXRMove:: XRMoveEnabled: " + XRMoveEnabled);
    }

    /// Enables rotation and translation control for desktop environments.
    /// For mobile environments, it enables rotation or translation according to
    /// the device capabilities.
    private void EnableAccordingToPlatform()
    {
        XRMoveEnabled = capabilities.supportsImmersiveVR;
        NonXRMoveEnabled = !XRMoveEnabled;
        Debug.Log("EnableAccordingToPlatform:: XRMoveEnabled: " + XRMoveEnabled);
    }

    private Vector3 moveDirection = Vector3.zero;
    private bool grounded = false;
    private float gravity = 20.0f;
    private float jumpSpeed = 6.0f;
    void MoveCharacterController(Vector3 direction)
    {
        if (grounded)
        {
            // We are grounded, so recalculate movedirection directly from axes
            //moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection = myTransform.TransformDirection(direction);
            moveDirection *= speed;

            if (Input.GetButton("Jump")) { moveDirection.y = jumpSpeed; }
        }

        // Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        flags = controller.Move(moveDirection * Time.deltaTime);
        grounded = (flags & CollisionFlags.CollidedBelow) != 0;
    }

    public static float ClampAngle (float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp (angle, min, max);
    }
}
