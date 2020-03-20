using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OVRInputItem : MonoBehaviour
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
    public UnityEvent OnPointerIn;
    public UnityEvent OnPointerOut;

    void Awake()
    {
        OVRLaserPointer.PointerIn += PointerIn;
        OVRLaserPointer.PointerOut += PointerOut;
        OVRLaserPointer.PointerDown += PointerDown;
        OVRLaserPointer.PointerUp += PointerUp;
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

    public void PointerIn(GameObject hitObj)
    {
        if (hitObj == null) return;
        if (!hitObj.Equals(this.gameObject)) return;

        if (TargetGraphic != null)
        {
            TargetGraphic.color = HighlitedColor;
        }
        if (TargetText != null)
        {
            TargetText.color = TextHighlitedColor;
        }

        if (OnPointerIn != null) OnPointerIn.Invoke();
    }

    public void PointerOut(GameObject hitObj)
    {
        if (hitObj == null) return;
        if (!hitObj.Equals(this.gameObject)) return;

        if (TargetGraphic != null)
        {
            TargetGraphic.color = NormalColor;
        }
        if (TargetText != null)
        {
            TargetText.color = TextNormalColor;
        }

        if (OnPointerOut != null) OnPointerOut.Invoke();
    }

    public void PointerDown(GameObject hitObj)
    {
        if (hitObj == null) return;
        if (!hitObj.Equals(this.gameObject)) return;

        if (TargetGraphic != null)
        {
            TargetGraphic.color = PressedColor;
        }
        if (TargetText != null)
        {
            TargetText.color = TextPressedColor;
        }
    }

    public void PointerUp(GameObject hitObj)
    {
        if (hitObj == null) return;
        if (!hitObj.Equals(this.gameObject)) return;

        if (TargetGraphic != null)
        {
            TargetGraphic.color = HighlitedColor;
        }
        if (TargetText != null)
        {
            TargetText.color = TextHighlitedColor;
        }

        if (OnPointerClick != null) OnPointerClick.Invoke();
    }
}
