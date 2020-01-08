using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Looxid.Link
{
    public class BarIndicator : MonoBehaviour
    {
        public Image BackgroundImage;
        public Image FilledImage;
        public Text ValueText;

        public void SetValue(float value)
        {
            if (BackgroundImage != null)
            {
                BackgroundImage.fillAmount = 1.0f - value;
            }
            if (FilledImage != null)
            {
                FilledImage.fillAmount = value;
            }
            if (ValueText != null)
            {
                ValueText.text = string.Format("{0}%", Mathf.RoundToInt(value * 100.0f));
            }
        }
    }
}
