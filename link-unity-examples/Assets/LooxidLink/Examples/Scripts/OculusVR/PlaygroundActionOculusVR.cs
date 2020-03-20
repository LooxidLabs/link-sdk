using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Looxid.Link
{
    public class PlaygroundActionOculusVR : MonoBehaviour
    {
        public OVRInput.Button primaryButton = OVRInput.Button.PrimaryIndexTrigger;
        public OVRInput.Button secondaryButton = OVRInput.Button.PrimaryIndexTrigger;

        public UnityEvent OnCreateCubeButtonClick;

        void Update()
        {
            if (OVRInput.GetDown(primaryButton) || OVRInput.GetDown(secondaryButton))
            {
                OnCreateCubeButtonClick.Invoke();
            }
        }
    }
}