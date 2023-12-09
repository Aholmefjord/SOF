using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace JULESTech.AudioChat
{
    /// <summary>
    /// A chatgroup instance; holds conversations of it participants
    /// holds instances of chatbubble
    /// </summary>
    ///

    // conversation: a list of messagebubble;
    public class ChatWindow : MonoBehaviour
    {
        #region Singleton
        static ChatWindow _instance;
        public static ChatWindow Instance { get { return _instance; } }
        private void Awake()
        {
            if(_instance!=null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        public const string WaveAudioDataType = "base64wavstring";
        public const string WaveAudioDataReferenceType = "base64wavDataRef";
        public static int currChatroomID;

        public int ChatgroupID { get { return chatgroupID; } }

        int chatgroupID = -1;
        int lastFulfilledEPOCHtime = 0;
        List<ChatMessageBubble> messageUI = new List<ChatMessageBubble>();
        List<ChatMessageData> messageData = new List<ChatMessageData>();

        [SerializeField]
        RectTransform ContentRect; // manipulate the height only

        Coroutine conversationUpdater = null;
        public void LoadConversation(int chatroomID)
        {
            lastFulfilledEPOCHtime = TimeHelper.CurrentEpochTime;
            // load time when last fetch request fufilled by server
            conversationUpdater = StartCoroutine(ConversationUpdate());
            // handle UI
        }
        public void CloseConversation()
        {
            // stop pinging for update
            if(conversationUpdater!=null)
                StopCoroutine(conversationUpdater);
        }
        IEnumerator ConversationUpdate()
        {
            // load all chat messages in this chatgroup

            // request server for chat history;
            WWWForm postData = new WWWForm();
            postData.AddField("chatgroup_id", chatgroupID);
            postData.AddField("lastcheck", lastFulfilledEPOCHtime);
            string[] data = null;
            string errorMsg = "";

            yield return JulesNet.Instance.SendPOSTRequestCoroutine("SOFGetChatMessages.php", postData,
                // OnSuccess
                (byte[] result) =>{
                    if (data[0] == "OK")
                        data = System.Text.Encoding.UTF8.GetString(result).Split('\t');
                },
                // OnFailure
                (string error) => { errorMsg = error; });

            // after getting reply, create all message bubble, create all message data
            // data !null means data received
            if (data != null){
                JSONNode converstionJSONNode = JSON.Parse(data[1]);
                int size = converstionJSONNode["count"].AsInt;

                // create one msg bubble with each entry
                for (int i = size - 1; i > 0; --i){
                    CreateMessageBubble(converstionJSONNode[i.ToString()]);
                }
            }else{
                // error
                Debug.LogError("[Load Convo]: " + errorMsg);
            }
            yield return new WaitForSeconds(1);
        }

        // not fully implemented
        void CreateMessageBubble(JSONNode field)
        {
            // make from pool ChatMessageBubble
            ChatMessageBubble msgBubble = new ChatMessageBubble(); // pool this pls
            ChatMessageData data = ScriptableObject.CreateInstance<ChatMessageData>();  // pool this pls
            data.username = field["username"];
            data.messageID = field["index"].AsInt;
            data.base64stringWavData = field["data"];
            data.timestampEPOCH = field["time"].AsInt;
            
            msgBubble.data = data;
            // load data here
            // insert
        }

        public void SendAudioMessage(string wavAsBase64string)
        {
            // create 
            StartCoroutine(CreateMessage(wavAsBase64string));
        }

        // called right after recording ended, create an instance of ChatMessageData for ChatMessageBubble usage
        IEnumerator CreateMessage(string wavAsBase64string)
        {
            ChatMessageData messageData = ScriptableObject.CreateInstance<ChatMessageData>();
            messageData.roomID = chatgroupID;
            messageData.username = GameState.me.username;
            messageData.base64stringWavData = wavAsBase64string;

            WWWForm postdata = new WWWForm();
            postdata.AddField("chatgroup_id", chatgroupID);
            postdata.AddField("data", wavAsBase64string);
            postdata.AddField("data_hash", StringHelper.GetMD5Hash(wavAsBase64string));
            postdata.AddField("data_type", ChatWindow.WaveAudioDataType);
            postdata.AddField("sender_account_id", GameState.me.id);

            yield return JulesNet.Instance.SendPOSTRequestCoroutine("SOFSendMessage.php", postdata,
                // OnSendSuccess callback, server got the audio data
                (byte[] _msg) => {
                    // get the messageID
                    string[] dataMsg = System.Text.Encoding.UTF8.GetString(_msg).Split('\t');
                    if (dataMsg[0] == "OK")
                    {
                        messageData.messageID = int.Parse(dataMsg[1]);
                        messageData.timestampEPOCH = int.Parse(dataMsg[2]);

                        messageData.CacheDataToLocalDrive();

                        // TODO: force currentChatWindow to update
                    }
                    else
                    {
                        Debug.LogError("CreateMessageFailed: " + dataMsg[0]);
                    }
                }, SendFailure);
        }

        void SendFailure(string errorMsg)
        {
        }

        #region UI updateloops
        // loop through all chat msg bubble
        void UpdateChatViewportInstant()
        {

        }
        #endregion

    }
}