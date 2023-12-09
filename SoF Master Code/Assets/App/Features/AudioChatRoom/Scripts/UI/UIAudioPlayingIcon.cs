using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JULESTech.AudioChat.UI
{
    [RequireComponent(typeof(Image))]
    public class UIAudioPlayingIcon : MonoBehaviour
    {
        Image cache;
        Image Target
        {
            get
            {
                if (cache == null) { cache = GetComponent<Image>(); }
                return cache;
            }
        }

        private void OnEnable()
        {
            Target.enabled = false;
            HackChatWindow chatWin = HackChatWindow.Instance;
            chatWin.OnPlaybackStarted += OnStartPlayingAudio;
            chatWin.OnPlaybackStopped += OnStopPlayingAudio;
        }
        private void OnDisable()
        {
            Target.enabled = false;
            HackChatWindow chatWin = HackChatWindow.Instance;
            chatWin.OnPlaybackStarted -= OnStartPlayingAudio;
            chatWin.OnPlaybackStopped -= OnStopPlayingAudio;
        }

        void OnStartPlayingAudio()
        {
            Debug.Log("show");
            Target.enabled = true;
        }
        void OnStopPlayingAudio()
        {
            Debug.Log("hide");
            Target.enabled = false;
        }
    }
}