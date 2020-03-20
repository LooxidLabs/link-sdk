using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterOculusVR : MonoBehaviour
{
    protected OVRInput.Controller activeController = OVRInput.Controller.RTrackedRemote;
    public UnityEngine.EventSystems.OVRInputModule inputModule = null;
    public Transform leftRayAnchor;
    public Transform rightRayAnchor;
    public GameObject[] teleportPoint;

    public OVRInput.Button teleportButton = OVRInput.Button.PrimaryIndexTrigger;
    public OVRInput.Button teleportSecondaryButton = OVRInput.Button.PrimaryIndexTrigger;
    public Transform player;
    public LayerMask includeLayers;
    public LineRenderer lineRenderer;
    public Transform trackingSpace;
    public OVRLaserPointer laserPointer;

    public Transform activeIndicator;
    public Transform inactiveIndicator;
    public Color activeColor;
    public Color inactiveColor;

    private bool bottomContact;
    private bool teleportContact;
    private Vector3 curveEnd;
    private Transform hitTransform;
    private Vector3 hitPoint;
    private Vector3 hitNormal;
    private int segments = 20;
    private float distance = 15.0f;
    private float dropHeight = 5.0f;
    private bool isButtonDown;

    void Awake()
    {
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

        lineRenderer.positionCount = segments;
    }

    void DetermineActiveController()
    {
        OVRInput.Controller controller = OVRInput.GetConnectedControllers();

        if (controller == OVRInput.Controller.LTouch || controller == OVRInput.Controller.LTrackedRemote)
        {
            inputModule.rayTransform = leftRayAnchor;
        }
        else
        {
            inputModule.rayTransform = rightRayAnchor;
        }
    }

    void Update()
    {
        DetermineActiveController();

        if (OVRInput.GetDown(teleportButton) || OVRInput.GetDown(teleportSecondaryButton))
        {
            isButtonDown = true;
            laserPointer.HidePointer();
            
            for( int i = 0; i < teleportPoint.Length; i++)
            {
                teleportPoint[i].SetActive(true);
            }
        }

        if (!isButtonDown)
        {
            lineRenderer.enabled = false;
            activeIndicator.gameObject.SetActive(false);
            inactiveIndicator.gameObject.SetActive(false);
            laserPointer.ShowPointer();

            for (int i = 0; i < teleportPoint.Length; i++)
            {
                teleportPoint[i].SetActive(false);
            }

            return;
        }

        lineRenderer.enabled = true;
        Vector3 controllerPosition = inputModule.rayTransform.position;
        Quaternion orientation = OVRInput.GetLocalControllerRotation(activeController);
        Vector3 ControllerUp = trackingSpace.localToWorldMatrix.MultiplyVector(orientation * Vector3.up);
        Vector3 conrollerForward = trackingSpace.localToWorldMatrix.MultiplyVector(orientation * Vector3.forward);
        curveEnd = hitPoint = controllerPosition + conrollerForward * distance + (ControllerUp * -1.0f) * dropHeight;

        Vector3 midPoint = controllerPosition + (curveEnd - controllerPosition) * 0.5f;
        Vector3 control = midPoint + ControllerUp * player.transform.position.y;

        bottomContact = false;
        teleportContact = false;
        RaycastHit bottomHit;
        Vector3 last = controllerPosition;
        float recip = 1.0f / (float)(segments - 1);

        for (int i = 1; i < segments; ++i)
        {
            float t = (float)i * recip;
            Vector3 sample = SampleCurve(controllerPosition, curveEnd, control, Mathf.Clamp01(t));

            if (Physics.Linecast(last, sample, out bottomHit))
            {
                float angle = Vector3.Angle(Vector3.up, bottomHit.normal);
                if (angle < 5.0f)
                {
                    hitPoint = bottomHit.point;
                    hitNormal = bottomHit.normal;
                    bottomContact = true;
                }
            }

            RaycastHit teleportHit;
            if (Physics.Linecast(last, sample, out teleportHit, includeLayers))
            {
                teleportContact = true;
                hitTransform = teleportHit.transform;
            }

            last = sample;
        }
        
        for (int i = 0; i < segments; ++i)
        {
            float t = (float)i * recip;
            Vector3 sample = SampleCurve(controllerPosition, curveEnd, control, Mathf.Clamp01(t));
            lineRenderer.SetPosition(i, sample);
        }

        if (bottomContact && teleportContact)
        {
            lineRenderer.startColor = activeColor;
            activeIndicator.gameObject.SetActive(true);
            inactiveIndicator.gameObject.SetActive(false);
            activeIndicator.position = hitTransform.position;
            activeIndicator.rotation = Quaternion.LookRotation(Vector3.forward, hitNormal);
        }
        else if (bottomContact && !teleportContact)
        {
            lineRenderer.startColor = inactiveColor;
            activeIndicator.gameObject.SetActive(false);
            inactiveIndicator.gameObject.SetActive(true);
            inactiveIndicator.position = hitPoint;
            inactiveIndicator.rotation = Quaternion.LookRotation(Vector3.forward, hitNormal);
        }
        else
        {
            lineRenderer.startColor = inactiveColor;
            activeIndicator.gameObject.SetActive(false);
            inactiveIndicator.gameObject.SetActive(false);
        }

        if (OVRInput.GetUp(teleportButton))
        {
            if (bottomContact && teleportContact)
            {
                player.position = new Vector3(hitTransform.position.x, player.position.y, hitTransform.position.z);
            }
            isButtonDown = false;
        }
    }

    Vector3 SampleCurve(Vector3 start, Vector3 end, Vector3 control, float time)
    {
        return Vector3.Lerp(Vector3.Lerp(start, control, time), Vector3.Lerp(control, end, time), time);
    }
}
