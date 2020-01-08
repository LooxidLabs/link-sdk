using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Looxid.Link
{
    public enum LooxidLinkMessageType
    {
        CoreDisconnected = 0,
        HubDisconnected = 1,
        SensorOff = 2,
        NoiseSignal = 3
    }

    public class LooxidLinkMessage : MonoBehaviour
    {
        public CanvasGroup disconnectedPanel;
        public CanvasGroup noiseSingalPanel;
        public CanvasGroup sensorOffPanel;

        private float disconnectedWindowAlpha = 0.0f;
        private float disconnectedWindowTargetAlpha = 0.0f;
        private float noiseSingalWindowAlpha = 0.0f;
        private float noiseSingalWindowTargetAlpha = 0.0f;
        private float sensorOffWindowAlpha = 0.0f;
        private float sensorOffWindowTargetAlpha = 0.0f;

        private LooxidLinkMessageType messageType;

        public void ShowMessage(LooxidLinkMessageType messageType)
        {
            this.messageType = messageType;
            if (messageType == LooxidLinkMessageType.CoreDisconnected || messageType == LooxidLinkMessageType.HubDisconnected)
            {
                if (disconnectedWindowAlpha <= 0.0f)
                {
                    disconnectedWindowAlpha = 0.0f;
                    disconnectedWindowTargetAlpha = 0.82f;
                }
            }
            else if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (noiseSingalWindowAlpha <= 0.0f && sensorOffWindowAlpha <= 0.0f)
                {
                    noiseSingalWindowAlpha = 0.0f;
                    noiseSingalWindowTargetAlpha = 0.82f;
                }
            }
            else if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (sensorOffWindowAlpha <= 0.0f)
                {
                    sensorOffWindowAlpha = 0.0f;
                    sensorOffWindowTargetAlpha = 0.82f;
                    noiseSingalWindowTargetAlpha = -0.02f;
                }
            }
        }

        public void HideMessage(LooxidLinkMessageType messageType)
        {
            this.messageType = messageType;
            if (messageType == LooxidLinkMessageType.CoreDisconnected || messageType == LooxidLinkMessageType.HubDisconnected)
            {
                if (disconnectedWindowAlpha > 0.0f)
                {
                    disconnectedWindowTargetAlpha = -0.02f;
                }
            }
            else if (messageType == LooxidLinkMessageType.NoiseSignal)
            {
                if (noiseSingalWindowAlpha > 0.0f)
                {
                    noiseSingalWindowTargetAlpha = -0.02f;
                }
            }
            else if (messageType == LooxidLinkMessageType.SensorOff)
            {
                if (sensorOffWindowAlpha > 0.0f)
                {
                    sensorOffWindowTargetAlpha = -0.02f;
                }
            }
        }

        void Update()
        {
            disconnectedWindowAlpha = Mathf.Lerp(disconnectedWindowAlpha, disconnectedWindowTargetAlpha, 0.2f);
            noiseSingalWindowAlpha = Mathf.Lerp(noiseSingalWindowAlpha, noiseSingalWindowTargetAlpha, 0.2f);
            sensorOffWindowAlpha = Mathf.Lerp(sensorOffWindowAlpha, sensorOffWindowTargetAlpha, 0.2f);

            disconnectedPanel.alpha = disconnectedWindowAlpha;
            noiseSingalPanel.alpha = noiseSingalWindowAlpha;
            sensorOffPanel.alpha = sensorOffWindowAlpha;
        }
    }
}