//==================================================================================================================
//
// VRShowOutlineByClick.cs
// Helper class to attach to the VR hand that raycasts where the user.
// It shows a line dependding on user input.
// Will show an Outline when detecting an object tagged as: "InteractiveObjects"
//
//==================================================================================================================

using UnityEngine;
using OutlineUtils;

public class VRShowOutlineByClick : MonoBehaviour
{
    public LaserPointer lp;
    public Transform eyeTransform;
    public LayerMask layers;
    public WebXRInputManager inputManager;
    public Vector3 offset = Vector3.zero;

    public const float INPUT_RATE_CHANGE = 20.0f;

    private Material LineMaterial;
    private Color lineColorOrig;
    public Color lineColorHit;

    private RaycastHit hit;
    private Vector3 _startPoint;
    private Vector3 _endPoint;
    private Vector3 _normal;

    private OutlineUtils.Outline lastOutline;

    private void Start()
    {
        LineMaterial = lp.GetComponent<LineRenderer>().material;
        lineColorOrig = LineMaterial.color;
    }

    private float m_pointBlend;
    void Update()
    {
        if (XRTeleporterController.Instance.IsTeleporterActive()) return;

        //m_pointBlend = inputManager.GetTriggerAxis();
        //if (m_pointBlend > 0.01f && m_pointBlend < 0.9f)
        if (inputManager.IsTriggerDown())
        {
            lp.OnInputFocusAcquired();
            _startPoint = eyeTransform.position + eyeTransform.transform.right * offset.x + eyeTransform.transform.up * offset.y + eyeTransform.transform.forward * offset.z;
            _endPoint = eyeTransform.transform.forward * 1000f;
            _normal = Vector3.one;

            // do a forward raycast to see if we hit a Button
            if (Physics.Raycast(_startPoint, eyeTransform.forward, out hit, 100f, layers))
            {
                _endPoint = hit.point;
                _normal = hit.normal;

                Outline OutlineAux = hit.transform.GetComponent<Outline>();
                if (lastOutline == null)
                {
                    lastOutline = OutlineAux;
                }
                else if (lastOutline != OutlineAux)
                {
                    HideOutline();
                    lastOutline = OutlineAux;
                }

                ShowOutline();
                //Debug.Log("VRShowLineRendererByClick: RayCast Hit Detected -> Normal: " + _normal);
            }
            else
            {
                HideOutline();
            }

            //lp.SetCursorRay(eyeTransform);
            /*if (!myOVRInputModule.isUIDetected) /**/
            {
                lp.SetCursorStartDest(_startPoint, _endPoint, _normal);
            }
        }
        else
        {
            lp.OnInputFocusLost();
            HideOutline();
        }
    }


    private void ShowOutline()
    {
        LineMaterial.color = lineColorHit;
        if (lastOutline != null) lastOutline.enabled = true;
    }

    private void HideOutline()
    {
        LineMaterial.color = lineColorOrig;
        if (lastOutline != null) lastOutline.enabled = false;
        lastOutline = null;
    }
}
