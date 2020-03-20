//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;

namespace Valve.VR.Extras
{
    public class LaserPointerSteamVR : MonoBehaviour
    {
        public SteamVR_Behaviour_Pose pose;

        public SteamVR_Action_Boolean interactWithUI = SteamVR_Input.GetBooleanAction("InteractUI");

        public bool active = true;
        public Color color;
        public float thickness = 0.002f;
        public Color clickColor = Color.green;
        public GameObject holder;
        public GameObject pointer;
        public bool addRigidBody = false;
        public Transform reference;

        private InteractionSystem.RenderModel hand;

        public static event PointerEventHandler PointerIn;
        public static event PointerEventHandler PointerOut;
        public static event PointerEventHandler PointerClick;
        public static event PointerEventHandler PointerDown;
        public static event PointerEventHandler PointerUp;

        Transform previousContact = null;


        private void Start()
        {
            if (pose == null)
                pose = this.GetComponent<SteamVR_Behaviour_Pose>();
            if (pose == null)
                Debug.LogError("No SteamVR_Behaviour_Pose component found on this object");
            
            if (interactWithUI == null)
                Debug.LogError("No ui interaction action has been set on this component.");
            

            holder = new GameObject();
            holder.transform.parent = this.transform;
            holder.transform.localPosition = Vector3.zero;
            holder.transform.localRotation = Quaternion.identity;

            pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pointer.transform.parent = holder.transform;
            pointer.transform.localScale = new Vector3(thickness, thickness, 100f);
            pointer.transform.localPosition = new Vector3(0f, 0f, 50f);
            pointer.transform.localRotation = Quaternion.identity;
            BoxCollider collider = pointer.GetComponent<BoxCollider>();
            if (addRigidBody)
            {
                if (collider)
                {
                    collider.isTrigger = true;
                }
                Rigidbody rigidBody = pointer.AddComponent<Rigidbody>();
                rigidBody.isKinematic = true;
            }
            else
            {
                if (collider)
                {
                    Object.Destroy(collider);
                }
            }
            Material newMaterial = new Material(Shader.Find("Unlit/Color"));
            newMaterial.SetColor("_Color", color);
            pointer.GetComponent<MeshRenderer>().material = newMaterial;
        }
        
        public virtual void OnPointerIn(PointerEventArgs e)
        {
            if (PointerIn != null)
                PointerIn(this, e);
        }

        public virtual void OnPointerClick(PointerEventArgs e)
        {
            if (PointerClick != null)
                PointerClick(this, e);
        }

        public virtual void OnPointerOut(PointerEventArgs e)
        {
            if (PointerOut != null)
                PointerOut(this, e);
        }

        public virtual void OnPointerDown(PointerEventArgs e)
        {
            if (PointerDown != null)
                PointerDown(this, e);
        }

        public virtual void OnPointerUp(PointerEventArgs e)
        {
            if (PointerUp != null)
                PointerUp(this, e);
        }


        private void LateUpdate()
        {
            if (hand == null)
            {
                hand = GetComponentInChildren<InteractionSystem.RenderModel>();
                if (hand != null)
                {
                    holder.SetActive(true);
                }
                else
                {
                    holder.SetActive(false);
                    return;
                }
            }
            else
            {
                holder.SetActive(true);
            }

            // laserpoint distance
            float dist = 500f;

            Ray raycast = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit);

            if (previousContact && previousContact != hit.transform)
            {
                PointerEventArgs args = new PointerEventArgs();
                args.fromInputSource = pose.inputSource;
                args.distance = 0f;
                args.flags = 0;
                args.target = previousContact;
                OnPointerOut(args);
                previousContact = null;
            }
            if (bHit && previousContact != hit.transform)
            {
                PointerEventArgs argsIn = new PointerEventArgs();
                argsIn.fromInputSource = pose.inputSource;
                argsIn.distance = hit.distance;
                argsIn.flags = 0;
                argsIn.target = hit.transform;
                OnPointerIn(argsIn);
                previousContact = hit.transform;
            }
            if (!bHit)
            {
                previousContact = null;
            }
           
            if (bHit && hit.distance < 500f)
            {
                dist = hit.distance;
            }

            if (bHit && interactWithUI.GetStateUp(pose.inputSource))
            {
                PointerEventArgs argsClick = new PointerEventArgs();
                argsClick.fromInputSource = pose.inputSource;
                argsClick.distance = hit.distance;
                argsClick.flags = 0;
                argsClick.target = hit.transform;
                OnPointerClick(argsClick);
            }

            if (interactWithUI != null && interactWithUI.GetState(pose.inputSource))
            {
                pointer.transform.localScale = new Vector3(thickness * 5f, thickness * 5f, dist);
                pointer.GetComponent<MeshRenderer>().material.color = clickColor;
                PointerEventArgs argsDown = new PointerEventArgs();
                argsDown.fromInputSource = pose.inputSource;
                argsDown.distance = hit.distance;
                argsDown.flags = 0;
                argsDown.target = hit.transform;
                OnPointerDown(argsDown);
            }
            else
            {
                pointer.transform.localScale = new Vector3(thickness, thickness, dist);
                pointer.GetComponent<MeshRenderer>().material.color = color;
                PointerEventArgs argsUp = new PointerEventArgs();
                argsUp.fromInputSource = pose.inputSource;
                argsUp.distance = hit.distance;
                argsUp.flags = 0;
                argsUp.target = hit.transform;
                OnPointerUp(argsUp);
            }
            pointer.transform.localPosition = new Vector3(0f, 0f, dist / 2f);
        }
    }
}