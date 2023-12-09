using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// VideoPlaylist Download test.
/// Downloads One VideoPlaylist
/// </summary>
public class VideoPlaylistTestScript : MonoBehaviour {
    public string serverURL = "http://localhost:4444";
    public string fileName = "Youtube7NaturalConservation.json";
    SimpleCache cacheHelper = SimpleCache.Load("video");
    int currGUIQueueOffset = -1;
    float progress = 0;

    static int queueOffset = 0;
    static int GetQueue() { return queueOffset++; }

	// Use this for initialization
	IEnumerator Start ()
    {
        currGUIQueueOffset = GetQueue();
        bool failed = false;
        string jsonData = "";

        VideoConfigLoadOptions options = new VideoConfigLoadOptions() {
            serverURL = serverURL,
            cacheURL = cacheHelper.CACHE_LOCATION,
            filename = fileName
        };
        yield return VideoConfigComparer.GetConfig(options, (resultStatus, resultData) => {
            if(resultStatus == VersionCheckStatus.FAIL) {
                Debug.LogErrorFormat("failed to download file: {0}", fileName);
                failed = true;
            } else {
                jsonData = resultData;
            }
        });

        // failed to get data, breakoff
        if (failed) {
            Debug.LogError("failed to dl config files");
            yield break;
        }

        VideoPlaylist playlist = VideoPlaylist.Deserialize( SimpleJSON.JSON.Parse(jsonData).AsObject );
        VideoAsyncTaskEvents events = new VideoAsyncTaskEvents();
        events.OnTaskEnd += (success, msg) => {
            if (success) {
                Debug.LogFormat("dl success: {0}/{1}", serverURL, fileName);
            } else {
                failed = true;
            }
        };
        events.OnProgressUpdate += (_progress) => { progress = _progress; };

        yield return playlist.Download(serverURL, events);
        if (failed) {
            Debug.LogErrorFormat("dl failure: {0}/{1}",serverURL,fileName);
        }
        //UnityEditor.EditorApplication.isPlaying = false;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 22*currGUIQueueOffset, 400, 20), string.Format("{0} Progress: {1}", fileName, progress.ToString("p1")));
    }
}
#endif
