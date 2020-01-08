using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Looxid.Link
{
    public class SkyboxRotation : MonoBehaviour
    {
        public float speedMultiplier;

        void Update()
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * speedMultiplier);
        }
    }
}