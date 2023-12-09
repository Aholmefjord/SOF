using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JULESTech.AudioChat
{
    /// <summary>
    /// A message bubble used in a ChatWindow; UI 
    /// // plays audio
    /// // will need to pool or cache
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class ChatMessageBubble : MonoBehaviour
    {
        static ChatMessageBubble lastPlayedBubble;

        public ChatMessageData data;
        [SerializeField]
        Text username;
        [SerializeField]
        Text timestamp;
        [SerializeField]
        Button playPauseButton;
        [SerializeField]
        Image playPauseButtonGylph;
        [SerializeField]
        Slider playbackProgressSlider;
        [SerializeField]
        Image backgroundPanel;
        [SerializeField]
        Image loadingVisual;

        public float playbackWidth;
        public float idleWidth;

        public Color ownMessageColor = Color.green;
        public Color friendMessageColor = Color.yellow;
        bool ownMsg = true;

        AudioSource audioPlayback;
        private void OnEnable()
        {
            audioPlayback = GetComponent<AudioSource>();
        }

        //*
        void Update()
        {
            if (playPauseButton != null)
            {

            }

            // animate slider when audio is playing
            if (playbackProgressSlider != null && audioPlayback != null)
            {
                playbackProgressSlider.maxValue = audioPlayback.clip.length;
                playbackProgressSlider.minValue = 0;
                playbackProgressSlider.value = audioPlayback.time;
            }
        }

        public bool IsOwnMessage
        {
            get { return ownMsg; }
            set
            {
                if (ownMsg != value)
                {
                    ownMsg = value;
                    // update colors and formatting
                    if (ownMsg)
                    {
                        backgroundPanel.color = ownMessageColor;
                        // play button position
                        // slider position
                    }
                    else
                    {
                        backgroundPanel.color = friendMessageColor;
                    }
                }
            }
        }

        //*/
        #region Audio playback
        public void PlayAudio()
        {
            // stop current playing instance if it exist;
            if (lastPlayedBubble != null)
            {
                lastPlayedBubble.StopAudio();
            }

            // is clip loaded?
            if(audioPlayback.clip == null)
            {
                //  not loaded, look for cached

                // if no cached, download from source
                StartCoroutine(RetrieveAudioClipFromServer(data));
            }
            else
            {
                // loaded
            }
            lastPlayedBubble = this;
            audioPlayback.Play();
        }

        public void StopAudio()
        {
            audioPlayback.Stop();
            playbackProgressSlider.value = 0;
        }
        #endregion

        #region Download AudioClip
        // load data from cached location
        bool LoadFromCachedLocation()
        {
            return false;
        }

        IEnumerator RetrieveAudioClipFromServer(ChatMessageData data)
        {
            /*
            // load from ChatMessageData
            string base64stringWavData = "";
            // on downloading finish, cache it as file
            //
            data.base64stringWavData = base64stringWavData;
            //*/
            yield return null;
        }
        #endregion
    }
}