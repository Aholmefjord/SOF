using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;

namespace JULESTech.AudioChat.UI {
    public class UIPushToTalk : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler, IEndDragHandler {
        [SerializeField]
        UIRecordingOverlay recordingOverlay;
        [SerializeField]
        CanvasGroup trashIcon;

        Vector2 startPos;
        bool isDragging = false;

        private void Start()
        {
            startPos = transform.position;
            ResetEvent();
        }

        public void StartEvent()
        {
            isDragging = true;
            transform.position = startPos;
            recordingOverlay.Show();
            trashIcon.alpha = 1;
            trashIcon.transform.DOPunchScale(Vector3.one * 0.4f, 0.4f);

            transform.localScale = Vector3.one;
            DOTween.Sequence()
                .Append(transform.DOPunchScale(Vector3.one * 0.3f, 0.4f))
                .Append(transform.DOScale(1, 0));
        }

        public void ResetEvent()
        {
            // hide trash icon;
            isDragging = false;
            transform.position = startPos;
            recordingOverlay.Hide();
            trashIcon.alpha = 0;

            transform.localScale = Vector3.one;
            DOTween.Sequence()
                .Append(transform.DOPunchScale(Vector3.one * 0.3f, 0.4f))
                .Append(transform.DOScale(1, 0));
        }
        public void CancelRecording()
        {
            if (AudioRecorder.IsRecording()) {
                AudioRecorder.CancelRecording();
                AudioSystem.ResumeBGM();
                Handheld.Vibrate();
                ResetEvent();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            // start recording
            var success = AudioRecorder.StartRecording();
            if (success) {
                StartEvent();
                Handheld.Vibrate();
                AudioSystem.PauseBGM();
            } else {
                Debug.LogError("failed to start recording");
            }
        }
        
        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // stop recording
            if (AudioRecorder.IsRecording() == false) {
                Debug.LogError("No recording started.");
                ResetEvent();
                return;
            }
            AudioSystem.ResumeBGM();

            AudioClip recording = AudioRecorder.StopRecording();
            Debug.Log("[AudioClip]: " + recording);

            if (recording == null) {
                Debug.LogError("no audio clip produced");
                return;
            }

            if (recording.length < 1) {
                // reject anything less than 1sec
                Debug.LogError("audioclip lessthan 1sec, reject");
                return;
            }

            try {
                // attempt to create data;
                HackChatWindow.Instance.SendAudioMessage(recording);
                Handheld.Vibrate();
                transform.DOPunchScale(Vector3.one * 0.2f, 0.2f);
            } catch (System.Exception e) {
                // error caught, need to show error notification
                Debug.LogError("HackChatWindow.Instance.SendAudioMessage thrown an error.\t" + e.Message);
            }
            ResetEvent();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!isDragging) return;
            OnEndDrag(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CancelRecording();
        }
    }
}