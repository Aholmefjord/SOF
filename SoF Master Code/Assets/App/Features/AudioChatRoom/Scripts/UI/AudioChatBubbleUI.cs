using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace JULESTech.AudioChat.UI {
    /// <summary>
    /// Chat Bubble for AudioChat with Buddy Avatar in buddyChat.unity3d
    /// </summary>
    public class AudioChatBubbleUI : MonoBehaviour {

        GameObject VisualObject {
            get {
                return gameObject;
            }
        }
        public void Show()
        {
            DOTween.Sequence()
                .Append(VisualObject.transform.DOScaleY(0, 0.0f))
                .Append(VisualObject.transform.DOScaleY(1.2f, 0.2f))
                .Append(VisualObject.transform.DOScaleY(1.0f, 0.05f));

            DOTween.Sequence()
                .Append(VisualObject.GetComponentInChildren<CanvasGroup>().DOFade(1, 0.2f));
        }
        public void Hide()
        {
            DOTween.Sequence()
                .Append(VisualObject.transform.DOScaleY(1.0f, 0.0f))
                .Append(VisualObject.transform.DOScaleY(1.05f, 0.1f))
                .Append(VisualObject.transform.DOScaleY(0.0f, 0.2f));
            DOTween.Sequence()
                .Append(VisualObject.GetComponentInChildren<CanvasGroup>().DOFade(0, 0.2f));
        }

        private void Start()
        {
            Hide();
        }
    }
}