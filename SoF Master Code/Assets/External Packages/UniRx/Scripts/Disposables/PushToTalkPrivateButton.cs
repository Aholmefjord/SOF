﻿namespace ExitGames.Demos.DemoPunVoice
{
    using Client.Photon.LoadBalancing;
    using UnityEngine;
    using UnityEngine.UI;

//    [RequireComponent(typeof(Button))]
    public class PushToTalkPrivateButton : MonoBehaviour
    {
        [SerializeField]
        private Button pushToTalkPrivateButton;
        [SerializeField]
        public PushToTalkScript pttScript;
        public Image registerImage;
        public byte AudioGroup;
        public bool Subscribed;

        public Sprite OnSprite;
        public Sprite OffSprite;

        // Use this for initialization
        private void Start()
        {
//            pttScript = FindObjectOfType<PushToTalkScript>();
            PhotonVoiceNetwork.Client.OnStateChangeAction += OnVoiceClientStateChanged;
        }


        /// <summary>Callback by the Voice Chat Client.</summary>
        /// <remarks>
        /// Unlike callbacks from PUN, this only updates you on the state of Voice.
        /// Voice will by default automatically enter a Voice room, when PUN joined one. That's why Joined state will happen.
        /// </remarks>
        /// <param name="state">The new state.</param>
        private void OnVoiceClientStateChanged(ClientState state)
        {
            //Debug.LogFormat("VoiceClientState={0}", state);
            if (pushToTalkPrivateButton != null)
            {
                switch (state)
                {
                    case ClientState.Joined:
                        pushToTalkPrivateButton.gameObject.SetActive(true);
                        Subscribed = Subscribed || PhotonVoiceNetwork.Client.ChangeAudioGroups(null, new byte[1] { AudioGroup });
                        break;
                    default:
                        pushToTalkPrivateButton.gameObject.SetActive(false);
                        break;
                }
            }
        }
        public void Update()
        {
            
        }
        public void SetAudioGroup(PhotonPlayer player)
        {
            if (!Subscribed)
            {
                int targetActorNr = player.ID;
                if (PhotonNetwork.player.ID < targetActorNr)
                {
                    AudioGroup = (byte) (targetActorNr + PhotonNetwork.player.ID*10);
                }
                else if (PhotonNetwork.player.ID > targetActorNr)
                {
                    AudioGroup = (byte) (PhotonNetwork.player.ID + targetActorNr*10);
                }
                else
                {
                    return;
                }
                if (PhotonVoiceNetwork.ClientState == ClientState.Joined)
                {
                    Subscribed = PhotonVoiceNetwork.Client.ChangeAudioGroups(null, new byte[1] { AudioGroup });
                }
            }
        }

        public void PushToTalkOn()
        {
            if (Subscribed)
            {
                PhotonVoiceNetwork.Client.GlobalAudioGroup = AudioGroup;
                pttScript.PushToTalkOn();
                registerImage.color = Color.green;
                registerImage.sprite = OnSprite;
            }
        }

        public void PushToTalkOff()
        {
            pttScript.PushToTalkOff();
            registerImage.color = Color.red;
            registerImage.sprite = OffSprite;
        }
    }
}
