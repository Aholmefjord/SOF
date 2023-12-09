using UnityEngine;
using UnityEngine.UI;

namespace JULESTech.AudioChat.UI
{
    [RequireComponent(typeof(Text))]
    public class UILastReceivedMessageEventHook : MonoBehaviour
    {
        QuickComponentAccess<Text> Label = new QuickComponentAccess<Text>();
        Text display;
        
        private void OnEnable()
        {
            HackChatWindow inst = HackChatWindow.Instance;
            inst.OnNewMessageReceived += MessageReceviedTimestamp;
        }
        private void OnDisable()
        {
            HackChatWindow inst = HackChatWindow.Instance;
            inst.OnNewMessageReceived -= MessageReceviedTimestamp;
        }

        void MessageReceviedTimestamp(int timeEPOCH)
        {
            System.DateTime timestamp = TimeHelper.DateTime(timeEPOCH);
            timestamp = timestamp.ToLocalTime();
            System.Text.StringBuilder builder = new System.Text.StringBuilder();
            builder.Append("Last message received at: ");
            builder.Append(timestamp.ToShortDateString());
            builder.Append(", ");
            builder.Append(timestamp.ToShortTimeString());

            Label.ReferenceObject = gameObject;
            Label.Value.text = builder.ToString();
        }
    }
}