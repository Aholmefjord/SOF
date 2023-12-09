using UnityEngine;
using UnityEngine.EventSystems;

namespace JULESTech.AudioChat.UI {
    public class UICancelRecording : MonoBehaviour, IPointerEnterHandler {

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (eventData.dragging && eventData.pointerDrag != null) {
                UIPushToTalk pttButton = eventData.pointerDrag.GetComponent<UIPushToTalk>();
                if (pttButton == null) {
                    Debug.Log("not pushtotalk button, ignore");
                    return;
                } else {
                    // cancel recording
                    eventData.Use();
                    pttButton.CancelRecording();
                }
            }
        }
    }
}