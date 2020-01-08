using UnityEngine;
using UnityEngine.UI;

namespace Looxid.Link
{
    [ExecuteInEditMode]
    public class CustomRenderQueue : MonoBehaviour
    {
        public UnityEngine.Rendering.CompareFunction comparison = UnityEngine.Rendering.CompareFunction.Always;

        void OnEnable()
        {
            Image image = GetComponent<Image>();
            Text text = GetComponent<Text>();
            if (image != null)
            {
                Material existingGlobalMat = image.materialForRendering;
                Material updatedMaterial = new Material(existingGlobalMat);
                updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
                image.material = updatedMaterial;
            }
            if (text != null)
            {
                Material existingGlobalMat = text.materialForRendering;
                Material updatedMaterial = new Material(existingGlobalMat);
                updatedMaterial.SetInt("unity_GUIZTestMode", (int)comparison);
                text.material = updatedMaterial;
            }
        }
    }
}