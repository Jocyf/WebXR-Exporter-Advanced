//==================================================================================================================
//
// WebXRInputManager.cs
// A wrapper to read all input from original code: WebXRController.cs
// So, all the project will detect the input looking at this class instead of doing it from WebXRController
//
//==================================================================================================================

using UnityEngine;
using System.Collections.Generic;
using WebXR;


public enum XRButtonStatus { ButtonDown, ButtonUp, isDown, None }

[RequireComponent(typeof(WebXRController))]
public class WebXRInputManager : MonoBehaviour
{
    //public float stickAxis = 0;
    public Vector2 stick = Vector2.zero;
    public float[] strickFloat;
    public XRButtonStatus triggerStatus = XRButtonStatus.None;
    public XRButtonStatus gripStatus = XRButtonStatus.None;

    public float triggerAxis = 0;
    public float gripAxis = 0;

    [Space (10)]
    public bool showDebug = false;

    private WebXRController controller;
    private string controllerHand;

    public WebXRController GetController() { return controller; }
    public bool IsControllerLeft() { return controller.hand == WebXRControllerHand.LEFT; }
    public bool IsControllerRight() { return controller.hand == WebXRControllerHand.RIGHT; }

    public bool IsStickEnabled() { return stick.x != 0 || stick.y != 0; }

    public bool IsTriggerButtonInactive() { return triggerStatus == XRButtonStatus.None; }
    public bool IsTriggerButtonDown() { return triggerStatus == XRButtonStatus.ButtonDown ; }
    public bool IsTriggerDown() { return triggerStatus == XRButtonStatus.isDown; }
    public bool IsTriggerButtonUp() { return triggerStatus == XRButtonStatus.ButtonUp; }
    public float GetTriggerAxis() { return triggerAxis; }

    public bool IsGripButtonInactive() { return gripStatus == XRButtonStatus.None; }
    public bool IsGripButtonDown() { return gripStatus == XRButtonStatus.ButtonDown; }
    public bool IsGripDown() { return gripStatus == XRButtonStatus.isDown; }
    public bool IsGripButtonUp() { return gripStatus == XRButtonStatus.ButtonUp; }
    public float GetGripAxis() { return gripAxis; }


    void Awake()
    {
        controller = GetComponent<WebXRController>();
        controllerHand = controller.hand.ToString();
    }

    void Update()
    {
        ReadXRInput();


        if (IsTriggerButtonDown()) { if (showDebug) Debug.Log("Controller " + controllerHand + " Trigger ButtonDown"); }
        if (IsGripButtonDown()) { if (showDebug) Debug.Log("Controller " + controllerHand + " Grip ButtonDown"); }

        /*if (controller.GetButton("Trigger")) { if(showDebug) Debug.Log("Controller " + controllerHand + " Trigger is Down"); }
        if (controller.GetButton("Grip")) { if(showDebug) Debug.Log("Controller " + controllerHand + " Grip is Down"); } */

        if (IsTriggerButtonUp()) { if (showDebug) Debug.Log("Controller " + controllerHand + " Trigger ButtonUp"); }
        if (IsGripButtonUp()) { if (showDebug) Debug.Log("Controller " + controllerHand + " Grip ButtonUp"); }

       

        /*stickAxis = controller.GetAxis("Stick");
        if(stickAxis != 0) Debug.Log("Controller " + controllerHand + " Strick Axis: "+ stickAxis.ToString());*/

        if (IsStickEnabled()) { if (showDebug) Debug.Log("Controller " + controllerHand + " Strick Axis: " + stick.ToString()); }
    }

    
    private void ReadXRInput()
    {
        #if UNITY_EDITOR
        stick.x = controller.GetAxis("StickHorizontal");
        stick.y = controller.GetAxis("StickVertical");
        #elif UNITY_WEBGL
        stick = controller.Get2DAxis("Stick");
        #endif

        GetXRButtonStatus("Trigger", ref triggerStatus);
        GetXRButtonStatus("Grip", ref gripStatus);

        triggerAxis = controller.GetAxis("Trigger");
        gripAxis = controller.GetAxis("Grip");
    }

    private void GetXRButtonStatus(string buttonName, ref XRButtonStatus buttonStatus)
    {
        if (controller.GetButtonDown(buttonName))
        {
            buttonStatus = XRButtonStatus.ButtonDown;
        }
        else if (controller.GetButtonUp(buttonName))
        {
            buttonStatus = XRButtonStatus.ButtonUp;
        }
        else if (controller.GetButton(buttonName))
        {
            buttonStatus = XRButtonStatus.isDown;
        }
        else
        {
            buttonStatus = XRButtonStatus.None;
        }
    }
}
