using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Looxid.Link
{
    public class AutoRotation : MonoBehaviour
    {
        public float speed = 10;
        public Vector3 dir = Vector3.up;

        void Update()
        {
            transform.Rotate(dir * Time.deltaTime * speed);
        }
    }
}