using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using JULESTech.AWS.S3;
using DG.Tweening;
using UnityEngine.Networking;
using JULESTech.AudioChat.UI;

namespace JULESTech.AudioChat {
    public class ChatroomManager : MonoBehaviour {
        [SerializeField, Tooltip("Id of the chatroom to enter")]
        int mChatRoomID = 1;

        [SerializeField]
        GameObject chatBubblePrefab;
        [SerializeField]
        GameObject usernameLabelPrefab;

        GameObject mUserAvatar = null;
        Dictionary<string, GameObject> mAvatars = new Dictionary<string, GameObject>();
        Dictionary<string, GameObject> mChatBubbles = new Dictionary<string, GameObject>();

        int ChatRoomID {
            get {
                return mChatRoomID;
                //PlayerPrefs.GetInt("CurrentChatroomID");
            }
        }
        private void Awake()
        {
            HackChatWindow.isService = true;
        }

        // Use this for initialization
        IEnumerator Start()
        {
            OnLoadingStart();
            #region Check if ChatGroup is around
            /*
            var reqState = ChatroomValidityRequest.MakeRequest(mChatRoomID);
            yield return reqState;
            if (reqState.isError) {
                OnLoadingFailure (reqState.message);
                yield break;
            }
            //*/
            #endregion

            string url = Constants.AppURL + "SOFGetChatGroupParticipants.php";
            WWWForm postData = new WWWForm();
            postData.AddField("chatgroup_id", ChatRoomID);

            UnityWebRequest req = UnityWebRequest.Post(url, postData);
            yield return req.Send();

            if (req.isError == false) {
                // process data
                string[] data = System.Text.Encoding.UTF8.GetString(req.downloadHandler.data).Split('\t');

                if (data[0] == "OK") {
                    JSONNode entries = JSON.Parse(StringHelper.Base64Decode(data[1]));

                    SpawnAllParticipantAvatars(entries);

                    OnLoadingSuccess();
                } else {
                    OnLoadingFailure("Error: " + data[1]);
                }
            } else {
                OnLoadingFailure("load participants failed: " + req.error);
            }
        }

        private void OnDestroy()
        {
            HackChatWindow.Instance.OnMessageEntryReceived -= OnMessagePayloadReceived;
            //HackChatWindow.Instance.CloseChat();
        }

        void OnLoadingStart()
        {
            // show input blocking ui
        }
        void OnLoadingSuccess()
        {
            // hide input blocking ui
            HackChatWindow.Instance.SetChatGroup(mChatRoomID);
            HackChatWindow.Instance.OnMessageEntryReceived += OnMessagePayloadReceived;

            // TOM: TODO: add a coroutine to poll for current participants, in event when ppl leave or join
        }
        void OnLoadingFailure(string message = null)
        {
            // show error prompt, press button to goback to mapNew
            // Create error dialog prompt object
            UnityEngine.UI.Button button = null;
            button.onClick.AddListener(() => {
                MainNavigationController.GoToMap();
            });
            UnityEngine.UI.Text label = null;
            label.text = message;
            Debug.LogError(message);
        }

        // can be used for updating new participants
        void SpawnAllParticipantAvatars(JSONNode entries)
        {
            int entryCount = entries["count"].AsInt;
            for (int i = 0; i < entryCount; ++i) {
                //Debug.LogFormat("name: {0}, animal: {1}, skin: {2}", entries[i]["username"], entries[i]["userAvatarAnimal"], entries[i]["userAvatarSkin"]);
                var obj = MakeChatAvatar(entries[i]["username"], entries[i]["userAvatarAnimal"], entries[i]["userAvatarSkin"]);
                obj.transform.position = new Vector3((i - 2) * 5, 0, 0);

                mAvatars.Add(entries[i]["username"], obj);

                if (GameState.me != null && entries[i]["username"] == GameState.me.username) {
                    mUserAvatar = obj;
                }
            }
        }

        // catching and processing message for each user
        void OnMessagePayloadReceived(JSONClass entry)
        {
            string username = entry["username"];
            int time = entry["time"].AsInt;
            int msgIndex = entry["index"].AsInt;

            GameObject avatarSrc = mAvatars[username];

            if (avatarSrc == null) {
                // avatar of message owner does not exist in this client instance yet
            }
            // this data is just the filename of the base64 .wav stored as a string on aws s3;
            if (entry["data_type"].Value == ChatWindow.WaveAudioDataReferenceType) {
                ChatAudioPlayer audPlayer = avatarSrc.GetComponent<ChatAudioPlayer>();
                audPlayer.PlayData(entry["data"].Value);
            }
            if (entry["data_type"].Value == "gesture") {
            }
            if (entry["data_type"].Value == "emoticon") {
            }
            //Debug.Log(entry.ToString());
        }

        GameObject MakeChatAvatar(string username, string avatarPrefab, string avatarSkin)
        {
            GameObject avatar = AvatarLoader.MakeAvatarObject(Constants.AVATAR_BASE_PATH + avatarPrefab, Constants.AVATAR_SKIN_BASE_PATH + avatarSkin);
            avatar.transform.localScale = Vector3.one * 1.5f;
            avatar.transform.forward = new Vector3(0, 0, -1);

            avatar.AddComponent<AudioSource>();
            ChatAudioPlayer audPlayer = avatar.AddComponent<ChatAudioPlayer>();
            audPlayer.OnFailToDownloadAudio += PrintErrorLog;

            WalkingAvatar walker = avatar.AddComponent<WalkingAvatar>();
            walker.MoveSpeed = 8;
            walker.RotateSpeed = 720;
            walker.thresholdDistance = 0.01f;
            walker.IsChatBuddy = true;
            avatar.AddComponent<AIPathFinder>();

            StartCoroutine(walker.StartRunningAround());

            GameObject label = GameObject.Instantiate(usernameLabelPrefab);
            FollowGameObjectPosition follower = label.AddComponent<FollowGameObjectPosition>();
            follower.FollowTarget = avatar;
            UnityEngine.UI.Text tLabel = label.GetComponentInChildren<UnityEngine.UI.Text>();
            if (tLabel != null) {
                if (GameState.me != null && username == GameState.me.username) {
                    tLabel.text = "YOU";
                } else {
                    tLabel.text = username;
                }
            }

            GameObject chatBubbleInst = GameObject.Instantiate(chatBubblePrefab);
            follower = chatBubbleInst.AddComponent<FollowGameObjectPosition>();
            follower.FollowTarget = avatar;
            AudioChatBubbleUI uiBubble = chatBubbleInst.AddComponent<AudioChatBubbleUI>();
            audPlayer.OnStartPlaying += uiBubble.Show;
            audPlayer.OnStopPlaying += uiBubble.Hide;
            mChatBubbles.Add(username, chatBubbleInst);

            return avatar;
        }

        void PrintErrorLog(string msg)
        {
            Debug.LogError(msg);
        }
    }

    public class ChatAudioPlayer : MonoBehaviour {
        public event System.Action OnStartPlaying;
        public event System.Action OnStopPlaying;
        public event System.Action<string> OnFailToDownloadAudio;

        QuickComponentAccess<AudioSource> mAudioSrc;
        QuickComponentAccess<AudioSource> AudioSrc {
            get {
                if (mAudioSrc == null) {
                    mAudioSrc = new QuickComponentAccess<AudioSource>(gameObject);
                    mAudioSrc.Value.playOnAwake = false;
                    mAudioSrc.Value.spatialBlend = 0.0f;
                }
                return mAudioSrc;
            }
        }
        Coroutine _playing;
        Coroutine _audioEndPlaybackPoll = null;

        public void PlayData(string audioData)
        {
            ForceStop ();
            _playing = StartCoroutine(DownloadAudio(audioData));
        }

        IEnumerator DownloadAudio(string wavDataRef)
        {
            yield return null;

            SimpleS3Bucket s3Bucket = SimpleS3Bucket.LoadFromResources("awss3/s3bucket_audiochatlog");
            s3Bucket.DownloadObject(wavDataRef, (responseObj) => {
                Debug.Log("[PlayCurrentAudioMessage] return from aws");
                if (responseObj.Exception == null) {
                    string base64stringWavData = "";
                    using (var reader = new System.IO.StreamReader(responseObj.Response.ResponseStream)) {
                        base64stringWavData = reader.ReadToEnd();
                    }

                    Debug.Log("[ChatAudioPlayer::PlayCheck] AudioClip on msgData has been download");
                    _audioEndPlaybackPoll = StartCoroutine (PlaybackAudio(base64stringWavData));
                } else {
                    string errorMsg = string.Format("ERROR: Failed to download audio, {0}", responseObj.Exception.Message);
                    if (OnFailToDownloadAudio != null)
                        OnFailToDownloadAudio(errorMsg);
                }
            });
        }

        IEnumerator PlaybackAudio(string wavDataBase64Encoded)
        {
            AudioClip clip = AudioRecorder.ConvertBase64String2AudioClip(wavDataBase64Encoded);
            AudioSrc.Value.clip = clip;
            AudioSrc.Value.Play();

            if (OnStartPlaying != null) OnStartPlaying();

            while (AudioSrc.Value.isPlaying) {
                yield return null;
            }

            if (OnStopPlaying != null) OnStopPlaying();

            _playing = null;
            _audioEndPlaybackPoll = null;
        }

        public void ForceStop()
        {
            if (_playing != null) {
                StopCoroutine(_playing);

                if (OnStopPlaying != null) OnStopPlaying();

                _playing = null;
                _audioEndPlaybackPoll = null;
            }
        }
    }

    public class FollowGameObjectPosition : MonoBehaviour {

        public GameObject FollowTarget {
            get; set;
        }

        private void Update()
        {
            transform.position = FollowTarget.transform.position;
        }
    }

    public sealed class ChatroomValidityRequest {
        int mRoomID = -1;

        ChatroomValidityRequest(int chatroomID)
        {
            mRoomID = chatroomID;
        }

        public static AsyncState MakeRequest(int id)
        {
            var roomObj = new ChatroomValidityRequest(id);
            SimpleEventListener el = new SimpleEventListener();
            GenericAsyncState state = new GenericAsyncState(el);
            IAsyncRequest req = new GenericCoroutineRequest(state, roomObj.IsChatroomValid);
            return req.Execute();
        }

        IEnumerator IsChatroomValid(IAsyncRequest req)
        {
            //var onFinish = req.EventListener.Call<System.Action<bool, string>>("OnFinish");
            //var onPrint = req.EventListener.Call<System.Action<string>>("OnPrintMessage");
            GenericAsyncState state = req.CurrentState as GenericAsyncState;
            yield return null;
            string url = Constants.AppURL + "SOFIsChatroomAvalable.php";

            WWWForm postData = new WWWForm();
            postData.AddField("chatroom_id", mRoomID);

            UnityWebRequest wReq = UnityWebRequest.Post(url, postData);
            yield return wReq.Send();

            // cannot connect to server
            if (wReq.isError) {
                state.OnEnd(false, wReq.error);
                yield break;
            }
            string[] data = System.Text.Encoding.UTF8.GetString(wReq.downloadHandler.data).Split('\t');
            if (data[0] == "OK") {
                // chatroom valid
                state.OnEnd(true, string.Format("Chatroom of id [{0}] is avaliable.", mRoomID));
            } else {
                // chatroom not found
                state.OnEnd(false, data[1]);
            }
        }
    }
}