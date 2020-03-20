using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OVRLaserPointer : MonoBehaviour
{
    public LayerMask excludeLayers;
    public Transform trackingSpace = null;
    public UnityEngine.EventSystems.OVRInputModule inputModule = null;
    public OVRGazePointer cursor;
    public Transform leftRayAnchor;
    public Transform rightRayAnchor;

    protected OVRInput.Controller activeController = OVRInput.Controller.RTrackedRemote;
    public OVRInput.Button joyPadClickButton = OVRInput.Button.PrimaryIndexTrigger;

    private LineRenderer lineRenderer = null;
    public float rayLength = 500;
    public Color rayColor = Color.white;
    public Color clickColor = Color.white;

    protected Transform lastHit = null;
    protected Transform triggerDown = null;
    private bool isShowPointer = false;

    public static System.Action<GameObject> PointerIn;
    public static System.Action<GameObject> PointerOut;
    public static System.Action<GameObject> PointerDown;
    public static System.Action<GameObject> PointerUp;

    void Awake()
    {
        if (inputModule != null)
        {
            joyPadClickButton = inputModule.joyPadClickButton;
        }

        if ((activeController & OVRInput.Controller.RTouch) == OVRInput.Controller.RTouch)
        {
            activeController = OVRInput.Controller.RTouch;
        }

        if ((activeController & OVRInput.Controller.LTouch) == OVRInput.Controller.LTouch)
        {
            activeController = OVRInput.Controller.LTouch;
        }

        if ((activeController & OVRInput.Controller.RTrackedRemote) == OVRInput.Controller.RTrackedRemote)
        {
            activeController = OVRInput.Controller.RTrackedRemote;
        }

        if ((activeController & OVRInput.Controller.LTrackedRemote) == OVRInput.Controller.LTrackedRemote)
        {
            activeController = OVRInput.Controller.LTrackedRemote;
        }

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material.SetColor("_Color", rayColor);

        ShowPointer();
    }

    public void ShowPointer()
    {
        isShowPointer = true;
        cursor.gameObject.SetActive(true);
    }
    public void HidePointer()
    {
        isShowPointer = false;
        cursor.gameObject.SetActive(false);
    }

    void RayHitSomething(Vector3 hitPosition, Vector3 hitNormal)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(1, hitPosition);
        }
    }

    void DetermineActiveController()
    {
        OVRInput.Controller controller = OVRInput.GetConnectedControllers();

        if (controller == OVRInput.Controller.LTouch || controller == OVRInput.Controller.LTrackedRemote)
        {
            inputModule.rayTransform = leftRayAnchor;
            cursor.rayTransform = leftRayAnchor;
        }
        if (controller == OVRInput.Controller.Touch || controller == OVRInput.Controller.RTouch || controller == OVRInput.Controller.RTrackedRemote)
        {
            inputModule.rayTransform = rightRayAnchor;
            cursor.rayTransform = rightRayAnchor;
        }
    }

    void DisableLineRendererIfNeeded()
    {
        if (lineRenderer != null)
        {
            lineRenderer.enabled = trackingSpace != null && activeController != OVRInput.Controller.None;
        }
        lineRenderer.enabled = isShowPointer;
    }

    Ray UpdateCastRayIfPossible()
    {
        if (trackingSpace != null && activeController != OVRInput.Controller.None)
        {
            Quaternion orientation = OVRInput.GetLocalControllerRotation(activeController);
            Vector3 localStartPoint = OVRInput.GetLocalControllerPosition(activeController);

            Matrix4x4 localToWorld = trackingSpace.localToWorldMatrix;
            Vector3 worldStartPoint = localToWorld.MultiplyPoint(localStartPoint);
            Vector3 worldOrientation = localToWorld.MultiplyVector(orientation * Vector3.forward);

            if (lineRenderer != null)
            {
                lineRenderer.SetPosition(0, worldStartPoint);
                lineRenderer.SetPosition(1, worldStartPoint + worldOrientation * rayLength);
            }

            if (inputModule != null)
            {
                return new Ray(worldStartPoint, worldOrientation);
            }
        }

        return new Ray();
    }

    void Update()
    {
        DetermineActiveController();
        DisableLineRendererIfNeeded();
        Ray selectionRay = UpdateCastRayIfPossible();
        ProcessInteractions(selectionRay);

        if (OVRInput.GetDown(joyPadClickButton))
        {
            lineRenderer.material.SetColor("_Color", clickColor);
        }
        else if (OVRInput.GetUp(joyPadClickButton))
        {
            lineRenderer.material.SetColor("_Color", rayColor);
        }
    }

    void ProcessInteractions(Ray pointer)
    {
        RaycastHit hit;
        if (Physics.Raycast(pointer, out hit, rayLength, ~excludeLayers))
        {
            if (lastHit != null && lastHit != hit.transform)
            {
                if (PointerOut != null)
                {
                    PointerOut.Invoke(lastHit.transform.gameObject);
                }
                lastHit = null;
            }

            if (lastHit == null)
            {
                if (PointerIn != null)
                {
                    PointerIn.Invoke(hit.transform.gameObject);
                }
            }

            lastHit = hit.transform;

            if (activeController != OVRInput.Controller.None)
            {
                if (OVRInput.GetDown(joyPadClickButton))
                {
                    triggerDown = lastHit;
                    if (PointerDown != null)
                    {
                        PointerDown.Invoke(hit.transform.gameObject);
                    }
                }
                else if (OVRInput.GetUp(joyPadClickButton))
                {
                    if (triggerDown != null && triggerDown == lastHit)
                    {
                        if (PointerUp != null)
                        {
                            PointerUp.Invoke(hit.transform.gameObject);
                        }
                    }
                }
                if (!OVRInput.Get(joyPadClickButton))
                {
                    triggerDown = null;
                }
            }
        }

        else if (lastHit != null)
        {
            if (PointerOut != null)
            {
                PointerOut.Invoke(lastHit.transform.gameObject);
            }
            lastHit = null;
        }
    }
}
