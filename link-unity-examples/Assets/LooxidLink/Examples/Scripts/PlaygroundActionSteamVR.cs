using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR.InteractionSystem;

namespace Looxid.Link
{
    public class PlaygroundActionSteamVR : MonoBehaviour
    {
        public Valve.VR.SteamVR_Action_Boolean menuButtonAction = Valve.VR.SteamVR_Input.GetAction<Valve.VR.SteamVR_Action_Boolean>("InteractionMenu");
        public UnityEvent OnMenuButtonClick;

        private Player player = null;

        void Start()
        {
            player = Player.instance;

            if (player == null)
            {
                Debug.LogError("<b>[SteamVR Interaction]</b> No Player instance found in map.");
                Destroy(this.gameObject);
                return;
            }
        }

        void Update()
        {
            foreach (Hand hand in player.hands)
            {
                if (IsEligibleForTeleport(hand))
                {
                    bool isMenuButtonClick = menuButtonAction.GetStateUp(hand.handType);

                    if (isMenuButtonClick)
                    {
                        OnMenuButtonClick.Invoke();
                    }
                }
            }
        }

        public bool IsEligibleForTeleport(Hand hand)
        {
            if (hand == null)
            {
                return false;
            }
            if (!hand.gameObject.activeInHierarchy)
            {
                return false;
            }
            if (hand.hoveringInteractable != null)
            {
                return false;
            }
            return true;
        }
    }
}