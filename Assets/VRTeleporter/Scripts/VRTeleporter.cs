//===================================================================================
//
// IJEMIN original code
//
// https://codeload.github.com/IJEMIN/Simple-Unity-VR-Teleporter/zip/master
//
//===================================================================================


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRTeleporter : MonoBehaviour
{

    public GameObject positionMarker; // marker for display ground position

    public Transform bodyTransforn; // target transferred by teleport

    public LayerMask excludeLayers; // excluding for performance

    public float angle = 45f; // Arc take off angle

    public float strength = 10f; // Increasing this value will increase overall arc length


    int maxVertexcount = 100; // limitation of vertices for performance. 

    private float vertexDelta = 0.08f; // Delta between each Vertex on arc. Decresing this value may cause performance problem.

    private LineRenderer arcRenderer;

    private Vector3 velocity; // Velocity of latest vertex

    private Vector3 groundPos; // detected ground position

    private Vector3 lastNormal; // detected surface normal

    private bool groundDetected = false;

    private List<Vector3> vertexList = new List<Vector3>(); // vertex on arc

    [SerializeField]
    [Disable]
    private bool displayActive = false; // don't update path when it's false.


    // Teleport target transform to ground position
    public void Teleport()
    {
        if (groundDetected)
        {
            //Vector3 dest = groundPos + lastNormal * 0.1f + Vector3.up * 1.05f;
            //Debug.Log("Teleport completed: origin: "+ bodyTransforn.position + " Dest: " + dest);

            // bodyTransforn.position = groundPos + lastNormal * 0.1f; // This works for the "camera Walker" (WebXRCameraSetv1 prefab)
            bodyTransforn.position = groundPos +  Vector3.up * 1.05f; // This works for the "character controller" walker (WebXRCameraSetv2 prefab)
            // The controller has to be positioned always depending on the CharacterController.height value. (Vector3.up * controller.height + 0.05f;)
            // for now i'm doing it just adding a fixed value by hand.
        }
        else
        {
            Debug.Log("Ground wasn't detected");
        }
    }

    // Active Teleporter Arc Path
    public void ToggleDisplay(bool active)
    {
        arcRenderer.enabled = active;
        positionMarker.SetActive(active);
        displayActive = active;
        if (!displayActive) { ClearLineRendererPoints(); }
    }


    private void Awake()
    {
        arcRenderer = GetComponent<LineRenderer>();
        arcRenderer.enabled = false;
        positionMarker.SetActive(false);
        ClearLineRendererPoints();
    }

    private void FixedUpdate()
    {
        if (displayActive)
        {
            UpdatePath();
        }
    }

    private void OnDisable()
    {
        ClearLineRendererPoints();
    }

    private void ClearLineRendererPoints()
    {
        vertexList.Clear();
        arcRenderer.positionCount = vertexList.Count;
        arcRenderer.SetPositions(vertexList.ToArray());
    }

    private void UpdatePath()
    {
        RaycastHit hit;

        groundDetected = false;
        vertexList.Clear(); // delete all previouse vertices
        velocity = Quaternion.AngleAxis(-angle, transform.right) * transform.forward * strength;

        Vector3 pos = transform.position; // take off position
        vertexList.Add(pos);

        while (!groundDetected && vertexList.Count < maxVertexcount)
        {
            Vector3 newPos = pos + velocity * vertexDelta
                            + 0.5f * Physics.gravity * vertexDelta * vertexDelta;

            velocity += Physics.gravity * vertexDelta;

            vertexList.Add(newPos); // add new calculated vertex

            // linecast between last vertex and current vertex
            if (Physics.Linecast(pos, newPos, out hit, ~excludeLayers))
            {
                groundDetected = true;
                groundPos = hit.point;
                lastNormal = hit.normal;
            }
            pos = newPos; // update current vertex as last vertex
        }


        positionMarker.SetActive(groundDetected);

        if (groundDetected)
        {
            positionMarker.transform.position = groundPos + lastNormal * 0.015f;
            positionMarker.transform.LookAt(groundPos);
        }

        // Update Line Renderer
        arcRenderer.positionCount = vertexList.Count;
        arcRenderer.SetPositions(vertexList.ToArray());
    }


}