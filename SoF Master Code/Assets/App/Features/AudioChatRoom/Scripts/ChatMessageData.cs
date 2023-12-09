using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JULESTech.AudioChat
{
    /// <summary>
    /// data struct of ChatMessage
    /// </summary>
    public class ChatMessageData : ScriptableObject
    {
        // retrieve from db
        public int roomID;
        public string username;
        public int messageID;
        public int timestampEPOCH;
        public string wavdataS3Ref;
        
        public string base64stringWavData;
        public AudioClip clip;

        #region Accessors
        // actualPath
        private string cachePath { get { return Application.persistentDataPath + "/audiochatcache/audiochatcache_" + roomID.ToString() + username.ToString() + messageID.ToString() + ".txt"; } }
        // debugPath
        //string cachePath { get { return Application.persistentDataPath + "/audiochatcache_.txt"; } }

        /// <summary>
        /// Return if the current chat audio cache exist: true==exist, false==!exist;
        /// </summary>
        public bool CacheExist
        {
            get
            {
                return System.IO.File.Exists(cachePath);
            }
        }
        #endregion
        
        /// <summary>
        /// cache base64stringWavData data onto local disc drive
        /// </summary>
        public void CacheDataToLocalDrive()
        {
            if (System.String.IsNullOrEmpty(base64stringWavData) == false)
            {
                if (System.IO.File.Exists(cachePath))
                {
                    System.IO.File.Delete(cachePath);
                }
                var stream = System.IO.File.CreateText(cachePath);
                stream.Write(base64stringWavData);
                stream.Close();
                Debug.Log("[AudioClip cache complete]");
            }
        }
        /// <summary>
        /// Load from cached data on local drive; loads the AudioClip at the end;
        /// </summary>
        /// <returns></returns>
        public bool LoadCacheDataFromLocalDrive()
        {
            /*// Do we want to stop loading if the clip is already loaded?
            if(clip!=null)
            {
                Debug.Log("[ChatMessageData]: Already have clip.");
                return true;
            }
            //*/

            string key = roomID.ToString() + username.ToString() + messageID.ToString();
            clip = null;/*
            if (s_cachedAudio.ContainsKey(key))
            {
                // cache found in memory! quick load
                s_cachedAudio.TryGetValue(key, out clip);
            }
            else//*/
            {
                if (CacheExist)
                {
                    // file exist
                    Debug.Log("[ChatMessageData]: AudioClip cache exist");
                    var sr = System.IO.File.OpenText(cachePath);
                    base64stringWavData = sr.ReadToEnd();
                }
                else
                {
                    // loading of cache failed
                    Debug.LogError("[ChatMessageData]: Local cache not found at " + cachePath);

                    // download from server?
                }
                // convert from base64string string data to AudioClip;
                clip = AudioRecorder.ConvertBase64String2AudioClip(base64stringWavData);
            }
            /*
            if (clip != null)
            {
                //s_cachedAudio.Add(key, clip);
                s_cachedAudio.Add(clip);
            }
            //*/
            return clip != null;
        }

        // TOM: not implemented yet
        public IEnumerator DownloadFromServer()
        {
            yield return null;
            /*
            WWWForm postdata = new WWWForm();
            postdata.AddField("chatgroup_id", this.roomID);
            postdata.AddField("userID", this.userID);
            postdata.AddField("messageID", this.messageID);

            yield return JulesNet.Instance.SendPOSTRequestCoroutine("SOFGetChatMessage.php", postdata,
                // OnSendSuccess callback, server got the audio data
                (byte[] receivedData) =>
                {
                    this.base64stringWavData = ""; // replace with data;
                    this.CacheDataToLocalDrive();
                }, DownloadRequestFailed);
            //*/
        }

        void DownloadRequestFailed(string error)
        {
            Debug.Log("Download audioclip from server failed: " + error);
        }

        public static ChatMessageData MakeSendData(int chatroomID, string wavString)
        {
            ChatMessageData data = ScriptableObject.CreateInstance<ChatMessageData>();
            data.roomID = chatroomID;
            data.base64stringWavData = wavString;
            data.wavdataS3Ref = MakeUniqueFilename();
            return data;
        }
        public static ChatMessageData MakeReceiveData(int chatroomID, string name, int msgID, string wavS3Ref, int timeEPOCH)
        {
            ChatMessageData data = ScriptableObject.CreateInstance<ChatMessageData>();
            data.roomID = chatroomID;
            data.username = name;
            data.messageID = msgID;
            data.wavdataS3Ref = wavS3Ref;
            data.timestampEPOCH = timeEPOCH;
            return data;
        }

        public static string MakeUniqueFilename()
        {
            System.Text.StringBuilder sbuilder = new System.Text.StringBuilder();
            sbuilder.Append(GameState.me.id);
            sbuilder.Append(TimeHelper.CurrentEpochTime);
            return sbuilder.ToString();
        }

        public static void CacheAudioClipInMemory(AudioClip clip)
        {
            if (s_cachedAudio.Contains(clip) == false)
            {
                s_cachedAudio.Add(clip);
            }
        }
        // store audioclip loaded into memory here
        static List<AudioClip> s_cachedAudio = new List<AudioClip>(); 
    }
}