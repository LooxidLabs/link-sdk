using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Valve.VR.Extras;

namespace Looxid.Link
{
    public class LaserPointerInputItem : MonoBehaviour
    {
        [Header("Target Graphic")]
        public Image TargetGraphic;
        public Color NormalColor = Color.white;
        public Color HighlitedColor = Color.white;
        public Color PressedColor = Color.white;

        [Header("Target Text")]
        public Text TargetText;
        public Color TextNormalColor = Color.white;
        public Color TextHighlitedColor = Color.white;
        public Color TextPressedColor = Color.white;

        public UnityEvent OnPointerClick;

        void Awake()
        {
            LaserPointerEvent.PointerClick += PointerClick;
            LaserPointerEvent.PointerIn += PointerIn;
            LaserPointerEvent.PointerOut += PointerOut;
            LaserPointerEvent.PointerDown += PointerDown;
            LaserPointerEvent.PointerUp += PointerUp;
        }

        void Start()
        {
            if (TargetGraphic != null)
            {
                TargetGraphic.color = NormalColor;
            }
            if (TargetText != null)
            {
                TargetText.color = TextNormalColor;
            }
        }

        public void SetNormalColor(Color color)
        {
            this.NormalColor = color;
            if (TargetGraphic != null)
            {
                TargetGraphic.color = NormalColor;
            }
        }
        public void SetTextNormalColor(Color color)
        {
            this.TextNormalColor = color;
            if (TargetText != null)
            {
                TargetText.color = TextNormalColor;
            }
        }

        public void PointerClick(object sender, PointerEventArgs e)
        {
            if (!e.target.gameObject.Equals(this.gameObject)) return;

            OnPointerClick.Invoke();
        }

        public void PointerIn(object sender, PointerEventArgs e)
        {
            if (e.target == null) return;
            if (!e.target.gameObject.Equals(this.gameObject)) return;

            if (TargetGraphic != null)
            {
                TargetGraphic.color = HighlitedColor;
            }
            if (TargetText != null)
            {
                TargetText.color = TextHighlitedColor;
            }
        }

        public void PointerOut(object sender, PointerEventArgs e)
        {
            if (e.target == null) return;
            if (!e.target.gameObject.Equals(this.gameObject)) return;

            if (TargetGraphic != null)
            {
                TargetGraphic.color = NormalColor;
            }
            if (TargetText != null)
            {
                TargetText.color = TextNormalColor;
            }
        }

        public void PointerDown(object sender, PointerEventArgs e)
        {
            if (e.target == null) return;
            if (!e.target.gameObject.Equals(this.gameObject)) return;

            if (TargetGraphic != null)
            {
                TargetGraphic.color = PressedColor;
            }
            if (TargetText != null)
            {
                TargetText.color = TextPressedColor;
            }
        }

        public void PointerUp(object sender, PointerEventArgs e)
        {
            if (e.target == null) return;
            if (!e.target.gameObject.Equals(this.gameObject)) return;

            if (TargetGraphic != null)
            {
                TargetGraphic.color = HighlitedColor;
            }
            if (TargetText != null)
            {
                TargetText.color = TextHighlitedColor;
            }
        }
    }
}