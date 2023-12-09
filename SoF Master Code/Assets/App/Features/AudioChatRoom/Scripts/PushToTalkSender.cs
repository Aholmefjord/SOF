using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JULESTech.AudioChat
{
    // TOM TODO: disable push to talk when not online;
    public class PushToTalkSender : MonoBehaviour
    {
        [SerializeField]
        GameObject recordingOverlay;

        public event System.Action OnStartRecording;
        public event System.Action OnStopRecording;
        public event System.Action OnCancelRecording;

        #region Singleton
        // TOM: not sure if this class should be a singleton;
        public static PushToTalkSender Instance { get { return _instance; } }
        private static PushToTalkSender _instance;
        private void Awake()
        {
            if(_instance==null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        #endregion

        private void OnApplicationPause(bool pause)
        {
            if (pause && AudioRecorder.IsRecording())
            {
                AudioRecorder.CancelRecording();
                if (OnCancelRecording != null)
                    OnCancelRecording();
                AudioSystem.ResumeBGM();
            }
        }
        /// <summary>
        /// starts a recording session with unity's microphone
        /// </summary>
        public void StartRecording()
        {
            bool result = AudioRecorder.StartRecording();
            if (result)
            {
                if (OnStartRecording != null)
                    OnStartRecording();
                AudioSystem.PauseBGM();
            }
        }
        /// <summary>
        /// stops current recording session
        /// </summary>
        public void StopRecording()
        {
            if(AudioRecorder.IsRecording() == false)
            {
                Debug.LogError("No recording started.");
                return;
            }
            AudioSystem.ResumeBGM();

            AudioClip recording = AudioRecorder.StopRecording();
            Debug.Log("[AudioClip]: " + recording);

            if (recording == null)
            {
                Debug.LogError("no audio clip produced");
                return;
            }

            if (OnStopRecording != null)
                OnStopRecording();
            try {
                // attempt to create data;
                HackChatWindow.Instance.SendAudioMessage(recording);
            } catch (System.Exception e) {
                // error caught, need to show error notification
                Debug.LogError("HackChatWindow.Instance.SendAudioMessage thrown an error.\t"+ e.Message);
            }
        }
        /// <summary>
        /// cancel current recording session if there is one
        /// </summary>
        public void CancelRecording()
        {
            if (AudioRecorder.IsRecording())
            {
                AudioRecorder.CancelRecording();
                if (OnCancelRecording != null)
                    OnCancelRecording();
                AudioSystem.ResumeBGM();
            }
        }
    }
}