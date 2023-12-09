using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

#if UNITY_EDITOR
public class DownloadFromS3Helper : MonoBehaviour {
    [SerializeField, Tooltip("root is in project folder")]
    string destinationSubPath = "VideoDownload";
    [SerializeField, Tooltip("please provide full download link")]
    string[] urls;
    
	// Use this for initialization
	IEnumerator Start () {
        string dir = string.Format("{0}/{1}", System.Environment.CurrentDirectory, destinationSubPath);
        if (System.IO.Directory.Exists(dir) == false) {
            System.IO.Directory.CreateDirectory(dir);
        }
        for(int i = 0; i < urls.Length; ++i) {
            yield return DownloadAndSave(urls[i]);
        }
        yield return null;
        Debug.Log("Download Ended");
        UnityEditor.EditorApplication.isPlaying = false;
    }
    public IEnumerator DownloadAndSave(string dlPath)
    {
        UnityWebRequest req = UnityWebRequest.Get(dlPath);
        yield return req.Send();

        if (req.isError == false) {
            // no error
            byte[] video = req.downloadHandler.data;
            int index = dlPath.LastIndexOf('/') + 1;
            string filename = dlPath.Substring(index, dlPath.Length - index);
            Debug.LogFormat("Download Success: [{0},{1}]", filename, video.ToString());
            // this saves the video to local
            string path = string.Format("{0}/{1}/{2}", System.Environment.CurrentDirectory, destinationSubPath, NameHashPair.RemoveSpecial(filename));
            DataHelper.SaveFileLocal(video, path);
        } else
            Debug.LogErrorFormat("[{0}] download failure", dlPath);
    }
}
#endif