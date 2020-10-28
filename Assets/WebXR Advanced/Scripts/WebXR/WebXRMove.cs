//==================================================================================================================
//
// WebXRMove.cs
// Move the camera in the environment using the sticks
// Read the sticks using the WebXRInputManager
// Look if we are using the Unity Editor or a browser (with/without 3d glases)
//
//==================================================================================================================

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;
using WebXR;

public class WebXRMove : MonoBehaviour
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

    private IEnumerator Start()
    {
        myTransform = transform;
        myCamera = cameraMainTransform.GetComponent<Camera>();
        originalRotation = transform.localRotation;

        #if UNITY_EDITOR
        cameraTransform = cameraMainTransform;
        //EnableXRMove(true);
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
        /*if (!XRDevice.isPresent && XRMoveEnabled)
        {
            EnableXRMove(false);
        }*/

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
        // Traslation (Only translation forward, no sideways)
        if (inputManagerLeftHand != null)
        {
            float x = canStrafe ? inputManagerLeftHand.stick.x * Time.deltaTime * speed : 0;
            float z = -inputManagerLeftHand.stick.y * Time.deltaTime * speed;

            Vector3 dirFordward = cameraTransform.forward;
            dirFordward.Normalize();
            Vector3 dirRight = cameraTransform.right;
            dirRight.Normalize();
            cameraText.text = "Cam->fwd: " + dirFordward + "right: " + dirRight;
            //myTransform.position += dirFordward * z + cameraMainTransform.right * x; // OLD
            Vector3 newPos = myTransform.position + dirFordward * z + dirRight * x;
            newPos.y = myTransform.position.y;
            myTransform.position = newPos;
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
        rotationX += inputManagerRightHand.stick.x * mouseSensitivity;
        //rotationY += inputManagerRightHand.stick.y * mouseSensitivity;

        rotationX = ClampAngle(rotationX, minimumX, maximumX);
        //rotationY = ClampAngle(rotationY, minimumY, maximumY);

        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        //Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

        myTransform.localRotation = originalRotation * xQuaternion; // * yQuaternion;
    }

    private void MoveNonXR()
    {
        // Traslation
        float x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        myTransform.Translate(x, 0, z);

        // Rotation
        if (Input.GetMouseButton(0))
        {
            rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
            rotationY += Input.GetAxis("Mouse Y") * mouseSensitivity;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

            myTransform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
    }


    private void EnableXRMove(bool _enable)
    {
        XRMoveEnabled = _enable;
        NonXRMoveEnabled = !XRMoveEnabled;
        Debug.Log("EnableXRMove:: XRMoveEnabled: " + XRMoveEnabled);
        //myCamera.fieldOfView = 60;
        //inputManagerLeftHand.gameObject.SetActive(false);
        //inputManagerRightHand.gameObject.SetActive(false);
    }

    /// Enables rotation and translation control for desktop environments.
    /// For mobile environments, it enables rotation or translation according to
    /// the device capabilities.
    private void EnableAccordingToPlatform()
    {
        XRMoveEnabled = capabilities.supportsImmersiveVR;
        NonXRMoveEnabled = !XRMoveEnabled;
        Debug.Log("EnableAccordingToPlatform:: XRMoveEnabled: " + XRMoveEnabled);
        /*if (XRMoveEnabled)
        {
            //myCamera.fieldOfView = 80;
            //inputManagerLeftHand.gameObject.SetActive(true);
            //inputManagerRightHand.gameObject.SetActive(true);
        }*/
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
