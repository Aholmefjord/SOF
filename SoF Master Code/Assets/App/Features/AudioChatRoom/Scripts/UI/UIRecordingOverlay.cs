using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using JULESTech.AudioChat;
namespace JULESTech.AudioChat.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIRecordingOverlay : MonoBehaviour
    {
        [SerializeField]
        PushToTalkSender recordingUI;
        [SerializeField]
        Slider durationBar;
        [SerializeField]
        Text timeLabel;

        CanvasGroup visibilityControl;
        CanvasGroup Visual {
            get{
                if (visibilityControl == null)
                    visibilityControl = GetComponent<CanvasGroup>();
                return visibilityControl;
            }
        }

        float internalTimer = 0;
        bool isShowing = false;

        private void Start()
        {
            _hide();
        }
        /*
        private void OnEnable()
        {
            recordingUI.OnCancelRecording += OnRecordingCanceled;
        }
        private void OnDisable()
        {
            recordingUI.OnCancelRecording -= OnRecordingCanceled;
        }

        void OnRecordingCanceled()
        {
            _hide();
        }
        //*/
        // Update is called once per frame
        void Update()
        {
            if (isShowing)
            {
                if(internalTimer <= 0)
                {
                    // countdown finish
                    recordingUI.StopRecording();
                    _hide();
                }
                else
                {
                    timeLabel.text = internalTimer.ToString("00") + " of " + AudioRecorder.MaxRecordingDuration + "s left";
                    durationBar.value = internalTimer;
                    internalTimer -= Time.deltaTime;
                }
            }
        }

        public void Show()
        {
            if (isShowing)
                return;

            isShowing = true;
            internalTimer = AudioRecorder.MaxRecordingDuration;

            durationBar.maxValue = AudioRecorder.MaxRecordingDuration;
            durationBar.minValue = 0;
            durationBar.value = internalTimer;

            Visual.alpha = 1;
            Visual.blocksRaycasts = true;
        }

        public void Hide()
        {
            if (!isShowing)
                return;
            _hide();
        }
        void _hide()
        {
            isShowing = false;
            Visual.alpha = 0;
            Visual.blocksRaycasts = false;
        }
    }
}