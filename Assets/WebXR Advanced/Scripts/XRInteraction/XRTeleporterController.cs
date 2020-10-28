//==================================================================================================================
//
// XRTeleporterController.cs
// Helper class to teleport based calling original teleport code.
// It will read WebXRInputManager stick value to draw the teleport curve.
// Will show an Outline when detecting an object tagged as: "InteractiveObjects"
//
//==================================================================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRTeleporterController : MonoBehaviour
{
    public VRTeleporter teleporter;
    public WebXRInputManager inputManager;

    [Space(10)]
    public float minStickValue = 0.3f;
    public int distanceMult = 10;

    [Header("Runtime Watchers")]
    [SerializeField]
    [Disable]
    private bool isActive = false;
    [SerializeField]
    [Disable]
    private float stickvalue = 0;
    [SerializeField]
    [Disable]
    private float strength = 0;

    private float stickvaluePrev = 0;


    #region Singleton
    public static XRTeleporterController Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public bool IsTeleporterActive() { return isActive; }

    private void Update ()
    {
        stickvalue = -inputManager.stick.y;

        if (stickvalue > minStickValue)
        {
            if (Mathf.Abs(stickvaluePrev) < minStickValue && !isActive)
            {
                teleporter.ToggleDisplay(true);
                isActive = true;
                //Debug.Log("XRTeleporterController:: Enabled");
            }
            strength = stickvalue;
            strength = Mathf.Clamp(strength, 0.1f, 1f);
            teleporter.strength = strength * distanceMult;
        }
        else if(isActive)
        {
            teleporter.ToggleDisplay(false);
            isActive = false;
            //Debug.Log("XRTeleporterController:: Disabled");
        }

        if (isActive && inputManager.IsTriggerButtonUp())
        {
            teleporter.Teleport();
            teleporter.ToggleDisplay(false);
            StartCoroutine("_DisableTeleportTimed");
            //Debug.Log("XRTeleporterController:: Disabled");
        }

        stickvaluePrev = stickvalue;
    }


    private IEnumerator _DisableTeleportTimed()
    {
        yield return new WaitForSeconds(0.2f);
        isActive = false;
    }
}