using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

public class LaserPointerUIItem : MonoBehaviour
{
    void OnEnable()
    {
        LaserPointerSteamVR.PointerClick += PointerClick;
        LaserPointerSteamVR.PointerIn += PointerIn;
        LaserPointerSteamVR.PointerOut += PointerOut;
        LaserPointerSteamVR.PointerDown += PointerDown;
        LaserPointerSteamVR.PointerUp += PointerUp;
    }
    void OnDisable()
    {
        LaserPointerSteamVR.PointerClick += PointerClick;
        LaserPointerSteamVR.PointerIn += PointerIn;
        LaserPointerSteamVR.PointerOut += PointerOut;
        LaserPointerSteamVR.PointerDown += PointerDown;
        LaserPointerSteamVR.PointerUp += PointerUp;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        if (!e.target.gameObject.Equals(this.gameObject)) return;

        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerClickHandler);
    }

    public void PointerIn(object sender, PointerEventArgs e)
    {
        if (e.target == null) return;
        if (!e.target.gameObject.Equals(this.gameObject)) return;

        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerEnterHandler);
    }

    public void PointerOut(object sender, PointerEventArgs e)
    {
        if (e.target == null) return;
        if (!e.target.gameObject.Equals(this.gameObject)) return;

        var pointer = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(gameObject, pointer, ExecuteEvents.pointerExitHandler);
    }

    public void PointerDown(object sender, PointerEventArgs e)
    {
        if (e.target == null) return;
        if (!e.target.gameObject.Equals(this.gameObject)) return;

    }

    public void PointerUp(object sender, PointerEventArgs e)
    {
        if (e.target == null) return;
        if (!e.target.gameObject.Equals(this.gameObject)) return;

    }
}
