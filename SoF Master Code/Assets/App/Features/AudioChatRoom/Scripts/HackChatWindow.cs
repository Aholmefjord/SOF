using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JULESTech; // julesnet
using SimpleJSON;
using JULESTech.AudioChat;
using JULESTech.AWS.S3;
using UnityEngine.Networking;
using System.IO;

using UniRx;

namespace JULESTech.AudioChat   // TOM: should move to a sub namespace?
{
    public class HackChatWindow : MonoBehaviour
    {
        public static bool isService = false; // if true, do not handle any UI and playback stuff
        #region Singleton
        static HackChatWindow _instance;
        public static HackChatWindow Instance { get { return _instance; } }
        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
            }
        }
        #endregion

        [SerializeField]
        int chatgroup_ID = 1;

        public Text usernameLabel;
        public Text timestampLabel;
        public AudioSource playbackSrc;

        public event System.Action OnPlaybackStarted;
        public event System.Action OnPlaybackStopped;
        public event System.Action<JSONClass> OnMessageEntryReceived;
        public event System.Action<string> OnPollMessageError;
        /// <summary>
        /// argument int is time in EPOCH
        /// </summary>
        public event System.Action<int> OnNewMessageReceived; // input is epoch time

        ChatMessageData msgdata;
        bool isUpdating = false;
        bool isSendingMessage = false;

        GameObject rootobject;
        QuickComponentAccess<Animator> bubbleAnimator = new QuickComponentAccess<Animator>();
        //* UI code, need to shift out
        [SerializeField]
        GameObject sendingAudioUIBlocker;
        //end of UI code */ 
        Coroutine UpdateCoroutine = null;
        Coroutine PlaybackDelayCoroutine = null;

        SimpleS3Bucket s3Bucket;
        private void Start()
        {
            s3Bucket = SimpleS3Bucket.LoadFromResources("awss3/s3bucket_audiochatlog");
            //if (isService) return;

            if (msgdata == null)
            {
                msgdata = ScriptableObject.CreateInstance<ChatMessageData>();
            }
            msgdata.roomID = chatgroup_ID;
            rootobject = GameObject.Find("ChatCanvas");
        }

        private void OnEnable()
        {
            if (UpdateCoroutine == null)
                UpdateCoroutine = StartCoroutine(PingForUpdates());

            if (isService) return;

            if(usernameLabel) usernameLabel.text = "";
            if(timestampLabel) timestampLabel.text = "";
            if(sendingAudioUIBlocker) sendingAudioUIBlocker.SetActive(false);

            OnPlaybackStarted += ShowBox;
            OnPlaybackStopped += HideBox;
        }

        private void OnDisable()
        {
            if (isService) return;

            OnPlaybackStarted -= ShowBox;
            OnPlaybackStopped -= HideBox;
            CloseChat();
        }

        void ShowBox()
        {
            bubbleAnimator.ValueByRef(gameObject).SetTrigger("Show");
        }
        void HideBox()
        {
            bubbleAnimator.ValueByRef(gameObject).SetTrigger("Hide");
        }

        public void SetChatGroup(int group)
        {
            chatgroup_ID = group;
        }

        //chatwindow's logic
        int debugEPOCHTime = 1499328525; // 6th july 2017, 4.09pm, thrusday
        IEnumerator PingForUpdates()
        {
            yield return new WaitForSeconds(0.2f);

            int lastFulfilledEPOCHtime = TimeHelper.CurrentEpochTime;
            if (OnNewMessageReceived != null)
                OnNewMessageReceived(lastFulfilledEPOCHtime);

            WWWForm postData = null;
            string[] data = null;
            string errorMsg = "";
            int msgReceivedThisPing = 0;

            if(playbackSrc) playbackSrc.Stop();
            // keep pinging for message
            while (true)
            {
                if (!isService) {
                    // don't check when audio is playing, or recording
                    while (playbackSrc.isPlaying || AudioRecorder.IsRecording() || isSendingMessage)
                        yield return new WaitForSeconds(1);
                }

                isUpdating = true;
                data = null;
                msgReceivedThisPing = 0;
                errorMsg = "";
                postData = new WWWForm();

                // sending request for messages that you've missed for the group
                postData.AddField("chatgroup_id", chatgroup_ID);
                postData.AddField("lastcheck", lastFulfilledEPOCHtime);
                //Debug.Log("Send request for chatlog");

                //*
                // request for updates for chatlogs
                yield return JulesNet.Instance.SendPOSTRequestCoroutine("SOFGetChatMessages.php", postData,
                    // OnSuccess
                    (byte[] result) =>
                    {
                        data = System.Text.Encoding.UTF8.GetString(result).Split('\t');
                    //Debug.Log("[PingUpdate] Data Received");
                    },
                    // OnFailure
                    (string error) => { errorMsg = error; }
                );
                //*/

                // after getting reply, create all message bubble, create all message data
                // data !null means data received
                if (data != null && data[0] == "OK")
                {
                    JSONNode converstionJSONNode = JSON.Parse(StringHelper.Base64Decode(data[1]));
                    int size = converstionJSONNode["count"].AsInt;

                    if (size > 0)
                    {
                        //Debug.Log("[PingUpdate] processing latest update...");
                        // get message json node;
                        JSONNode latestEntry = converstionJSONNode[(size - 1).ToString()];

                        if (!isService) {
                            msgdata.base64stringWavData = "";
                            msgdata.clip = null;
                            msgdata.wavdataS3Ref = "";
                            msgdata.username = "";
                            msgdata.roomID = -1;

                            ProcessMessageEntry(latestEntry, msgdata);

                            if (OnNewMessageReceived != null)
                                OnNewMessageReceived(lastFulfilledEPOCHtime);
                            PlayCurrentAudioMessage();
                            msgReceivedThisPing += 1;
                        } else {
                            for (int i = 0; i < size; ++i) {
                                msgReceivedThisPing += 1;
                                if (OnMessageEntryReceived != null)
                                    OnMessageEntryReceived(converstionJSONNode[i.ToString()].AsObject);
                            }
                        }
                        // only update this when you received a successful message, 
                        lastFulfilledEPOCHtime = TimeHelper.CurrentEpochTime;
                    }
                }else{
                    // error
                    Debug.LogError("[PingUpdate]: " + errorMsg);
                    if (OnPollMessageError != null) OnPollMessageError(errorMsg);
                }

                isUpdating = false;

                // only print when a message arrives
                if (msgReceivedThisPing > 0)
                    Debug.Log("[PingUpdate] 1 message received, " + System.DateTime.Now);
                //else
                //Debug.Log("[No new message]");
                //yield return new WaitForSeconds(0.5f);
            }
        }

        void ProcessMessageEntry(JSONNode dataEntry, ChatMessageData receiver)
        {
            // building current mesage's info
            receiver.username = dataEntry["username"];
            receiver.timestampEPOCH = dataEntry["time"].AsInt;
            receiver.messageID = dataEntry["index"].AsInt;

            // this data is just the filename of the base64 .wav stored as a string on aws s3;
            if (dataEntry["data_type"].Value == ChatWindow.WaveAudioDataReferenceType)
            {
                receiver.wavdataS3Ref = dataEntry["data"].Value;
                Destroy(receiver.clip); // this is only if you are reusing
                receiver.clip = null;
                playbackSrc.clip = null; // remove clip loaded in player
            }

            // update UI;
            usernameLabel.text = receiver.username = dataEntry["username"];
            System.DateTime dateTime = TimeHelper.DateTime(dataEntry["time"].AsInt);
            timestampLabel.text = dateTime.ToString("dd-MM-yyyy hh:mm");
        }

        #region Sending audio message to server
        public void SendAudioMessage(AudioClip clip)
        {
            isSendingMessage = true;

            string wavAsBase64string = AudioRecorder.ConvertAudioClip2Base64String(clip);

            // show "Sending..." UI, tat can cancel
            if(sendingAudioUIBlocker) sendingAudioUIBlocker.SetActive(true);

            // Start sending the wav data onto AWS S3 bucket
            // TOM: need to add in retry mode
            
            // TOM: **NOTE** if upload request is stuck, assume failure, if its return with success, deleteonline store

            Debug.Log("[SendAudioMessage] start uploading data");
            ChatMessageData messageData = ChatMessageData.MakeSendData(chatgroup_ID, wavAsBase64string);
            s3Bucket.UploadDataPutMethod(messageData.wavdataS3Ref, messageData.base64stringWavData, (responseobj) =>
            {
                if (responseobj.Exception == null){
                    // no error, aws s3 got data safely
                    Debug.Log("Upload wav success");
                    StartCoroutine(SendAudioMessageCoroutine(messageData));
                    //retryUpload = false;
                }else{
                    //Debug.LogException(responseobj.Exception);
                    Debug.LogError("[HackChatWindow:SendAudioMessage] error sending audio data, " + responseobj.Exception.Message);
                    if(sendingAudioUIBlocker) sendingAudioUIBlocker.SetActive(false);
                }
                EndSendingMessage();
            });
        }

        // called right after recording ended, create an instance of ChatMessageData for ChatMessageBubble usage
        IEnumerator SendAudioMessageCoroutine(ChatMessageData messageData)
        {
            isSendingMessage = true;
            // prepare to send data
            WWWForm postdata = new WWWForm();
            postdata.AddField("chatgroup_id", messageData.roomID);
            postdata.AddField("data", messageData.wavdataS3Ref);
            postdata.AddField("data_hash", StringHelper.GetMD5Hash(messageData.wavdataS3Ref));
            postdata.AddField("data_type", ChatWindow.WaveAudioDataReferenceType);
            postdata.AddField("sender_account_id", GameState.me.id);

            Debug.Log("[SendAudioMessage] sending: " + messageData.wavdataS3Ref);

            yield return JulesNet.Instance.SendPOSTRequestCoroutine("SOFSendMessage.php", postdata,
                // OnSendSuccess callback, server got the audio data
                (byte[] _msg) =>
                {
                    // get the messageID
                    string[] dataMsg = System.Text.Encoding.UTF8.GetString(_msg).Split('\t');
                    if (dataMsg[0] == "OK")
                    {
                        messageData.messageID = int.Parse(dataMsg[1]);
                        Debug.Log("[SendAudioMessage] Send success.");
                        // TODO: force currentChatWindow to update
                    }else{
                        Debug.LogError("CreateMessageFailed: " + dataMsg[0]);
                    // will need to retry again just in case
                    }
                    EndSendingMessage();
                }, null);

            while (isSendingMessage)
                yield return null;

            // can use this to hide "Sending" UI
            sendingAudioUIBlocker.SetActive(false);
            Debug.Log("[SendAudioMessage] Send coroutine ended.");
        }
        void EndSendingMessage()
        {
            isSendingMessage = false;
        }
        #endregion

        #region Audio chat playback
        // chatmessagebubble's stuff
        public void PlayCurrentAudioMessage()
        {
            // prevent multiple wait
            if (PlaybackDelayCoroutine != null)
            {
                Debug.Log("[already queued audio playback]");
                return;
            }

            PlaybackDelayCoroutine = StartCoroutine(PlayCheck());
        }
        IEnumerator PlayCheck()
        {
            // if updating message, wait for new message to come in before playing;
            while (isUpdating)
                yield return null;

            if (playbackSrc.isPlaying)
            {
                playbackSrc.Stop();
            }
            if (msgdata.clip != null)
            {
                PlaybackDelayCoroutine = StartCoroutine(OnDownloadFinish());
                yield break;
            }
            Debug.Log("[Playbutton pressed, attempting to dl clip]");
            if (playbackSrc.clip == null)
            {
                // msgdata == current data
                Debug.Log("[PlayCurrentAudioMessage] Downloading " + msgdata.wavdataS3Ref);
                s3Bucket.DownloadObject(msgdata.wavdataS3Ref, (responseObj) =>
                {
                    Debug.Log("[PlayCurrentAudioMessage] return from aws");
                    if (responseObj.Exception == null)
                    {
                        using (StreamReader reader = new StreamReader(responseObj.Response.ResponseStream))
                        {
                            msgdata.base64stringWavData = reader.ReadToEnd();
                        }
                        msgdata.clip = AudioRecorder.ConvertBase64String2AudioClip(msgdata.base64stringWavData);
                        playbackSrc.clip = msgdata.clip;
                        Debug.Log("[PlayCurrentAudioMessage] AudioClip on msgData has been download");
                        PlaybackDelayCoroutine = StartCoroutine(OnDownloadFinish());
                    }
                    else
                    {
                    // TOM TODO: what if there is an error in downloading audio?
                    Debug.Log("[PlayCurrentAudioMessage] Download failed");
                        Debug.LogException(responseObj.Exception);
                    }
                });
            }
            /*
            // wait for download from s3 bucket to complete before continuing
            while (waitForDownload)
                yield return null;

            Debug.Log("Play Audio Clip!!");
            playbackSrc.Play();

            // dun lock chatwindow's update while current clip is playing
            while (playbackSrc.isPlaying)
                yield return null;
            PlaybackDelayCoroutine = null;
            //*/
        }

        // actual playing downloaded audio
        IEnumerator OnDownloadFinish()
        {
            playbackSrc.Play();
            if (OnPlaybackStarted != null){
                OnPlaybackStarted();
            }

            while (playbackSrc.isPlaying)
                yield return null;

            if (OnPlaybackStopped != null){
                OnPlaybackStopped();
            }

            PlaybackDelayCoroutine = null;
            playbackSrc.clip = null;
            usernameLabel.text = "";
        }
        #endregion

        public void CloseChat()
        {
            // cleanup
            if(playbackSrc!=null)
                playbackSrc.Stop();
            Destroy(msgdata.clip); // this is only if you are reusing
            if (OnPlaybackStopped != null)
            {
                OnPlaybackStopped();
            }

            isUpdating = false;
            isSendingMessage = false;
            
            StopAllCoroutines();
            PlaybackDelayCoroutine = null;
            UpdateCoroutine = null;

            rootobject.SetActive(false);
            Debug.Log("ChatCanvas closed");
        }
    }
}