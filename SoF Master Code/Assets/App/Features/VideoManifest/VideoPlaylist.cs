using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using UnityEngine.Networking;
using System;

[System.Serializable]
public sealed class VideoPlaylist {
    public string m_playlistName;
    public string m_playlistPath;
    public List<NameHashPair> m_list;

    public string SubPath {
        get {
            return m_playlistPath;
        }
    }

    #region instance functions
    /// <summary>
    /// get url with percentage characters %20/+ for whitespace.
    /// ONLY selected onces were replaced.
    /// go look at NameHashPair.SwapForSpecial.
    ///  for aws s3 url
    /// </summary>
    /// <param name="server">provide either file server url, or cache location</param>
    /// <param name="i"></param>
    /// <returns></returns>
    public string GetVideoPath(string server, int i)
    {
        string url = string.Format("{0}/{1}/{2}", server, SubPath, m_list[i].filename);
        return url;
    }

    /// <summary>
    /// download/load videos listed in playlist from cache.
    /// Will only download from server if there is a cache miss
    /// </summary>
    /// <param name="serverURL"></param>
    /// <param name="cacheURL"></param>
    /// <returns></returns>
    public IEnumerator Download(string serverURL, VideoAsyncTaskEvents events)
    {
        float currProgress = 0;
        float fileCount = (float)m_list.Count;

        bool error = false;
        string errorMsg = "";
        VideoAsyncTaskEvents localEvents = new VideoAsyncTaskEvents();
        localEvents.PrintText += events.Print;
        localEvents.OnProgressUpdate += (fileDownloadProgress) => {
            events.Update((currProgress + fileDownloadProgress) / fileCount);
        };
        localEvents.OnTaskEnd += (isSuccess, message) => {
            if (isSuccess) {
                currProgress += 1;
                events.Update(currProgress / fileCount);
                //Debug.LogFormat("{0} downloaded, {1}", m_list[i].filename,message);
            } else {
                //error
                error = true;
                errorMsg = message;
                //Debug.LogErrorFormat("{0} failed: {1}", m_list[i].filename, message);
            }
        };

        for (int i = 0; i < m_list.Count; ++i) {
            error = false;
            events.Print("Checking " + m_list[i].filename);
            yield return DownloadVideoByIndex(serverURL, i, localEvents);

            if (error) {
                events.End(false, errorMsg);
                yield break;
            }
        }
        events.End(true, "VideoPlaylist [" + m_playlistName + "] successfully downloaded.");
    }
    /// <summary>
    /// download the video to cache
    /// </summary>
    /// <param name="serverUrl"></param>
    /// <param name="index"></param>
    /// <param name="c"></param>
    /// <param name="_progressUpdate"></param>
    /// <param name="_onFinish"></param>
    /// <returns></returns>
    private IEnumerator DownloadVideoByIndex(string serverUrl, int index, VideoAsyncTaskEvents events)
    {
        SimpleCache c = VideoPlaylistConstants.Cache;
        if (SameAsCache(index)) {
            events.Update(1);
            events.End(true, "No change in filehash, using cached version.");
            yield break;
        }

        string downloadPath = GetVideoPath(serverUrl, index);
        //Debug.Log("Start download: "+downloadPath);
        events.Print("Downloading: " + m_list[index].filename);
        UnityWebRequest req = UnityWebRequest.Get(downloadPath);

        var ops = req.Send();
        while (ops.isDone == false) {
            events.Update(ops.progress);
            yield return null;
        }
        
        string msg = "";
        string filePath = "";
        if (req.isError == false) {
            if (HandleAWSErrorValue(req.downloadHandler.text, events.End) == false) {
                //Debug.Log(req.downloadHandler.text);
                filePath = FilePath(index);
                c.EnsureFolderExist (SubPath);
                c.WriteByteToCache (filePath, req.downloadHandler.data);
                c.WriteTextToCache (filePath + ".md5hash", StringHelper.GetMD5Hash(req.downloadHandler.data));
                //Debug.LogFormat("Downloading complete: {0}", downloadPath);
                msg = m_list[index].filename + " download success.";
            }
        } else {
            msg = req.error;
        }
        events.End(!req.isError, msg);
        req.Dispose();
    }
    /// <summary>
    /// checks if the hash loaded from config file is the same as 
    /// the copy in cache
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private bool SameAsCache(int i)
    {
        SimpleCache c = VideoPlaylistConstants.Cache;
        string cacheFileUrl = FilePath(i);
        if (c.CacheExist(cacheFileUrl) == false) {
            //Debug.LogFormat("no cache for: {0}", cacheFileUrl);
            return false;
        }
        string cacheHash = "";
        if (c.CacheExist(cacheFileUrl + ".md5hash")) {
            cacheHash = c.OpenTextFromCache(cacheFileUrl + ".md5hash");
        } else {
            byte[] data = VideoPlaylistConstants.Cache.ReadByteFromCache(cacheFileUrl);
            cacheHash = StringHelper.GetMD5Hash(data);
        }

        return cacheHash == m_list[i].filehash;
    }
    /// <summary>
    /// returns true if an error is returned by AWS
    /// </summary>
    /// <param name="text"></param>
    /// <param name="_onFinish"></param>
    /// <returns></returns>
    private bool HandleAWSErrorValue(string text, Action<bool, string> _onFinish)
    {
        bool isAWSError = true;
        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();

        try {
            doc.LoadXml(text);
        } catch (Exception) {
            // normal
            isAWSError = false;
        }

        if (isAWSError) {
            System.Xml.XmlNode root = doc.LastChild;
            System.Xml.XmlNode first = root.FirstChild;
            string msg = string.Format("AWS Error: {0}", first.InnerText);
            if (_onFinish != null) {
                _onFinish(false, msg);
            }
        }

        return isAWSError;
    }

    //Misc
    string FilePath(int index)
    {
        return SubPath + '/' + m_list[index].filename;
    }
    #endregion

    #region Serialization/Deserialization
    public static VideoPlaylist Deserialize(JSONClass node)
    {
        VideoPlaylist pl = Make(node["playlistName"].Value, node["playlistPath"].Value);
        JSONArray arr = node["playlistList"].AsArray;
        for(int i=0;i<arr.Count;++i) {
            pl.m_list.Add(NameHashPair.Deserialize(arr[i].AsObject));
        }
        //if (pl != null) Debug.LogFormat("VideoPlaylist [{0}] Deserialize Success",pl.m_playlistName);
        return pl;
    }

    /// <summary>
    /// this is for making a new empty playlist object
    /// </summary>
    /// <param name="name"></param>
    /// <param name="subpath"></param>
    /// <returns></returns>
    public static VideoPlaylist Make(string name, string subpath)
    {
        VideoPlaylist pl = new VideoPlaylist();
        pl.m_playlistName = name;
        pl.m_playlistPath = subpath;
        pl.m_list = new List<NameHashPair>();
        return pl;
    }

    public JSONClass Serialize()
    {
        JSONClass playlistObj = new JSONClass();
        //arrayOfPlaylist.Add(playlistObj);
        playlistObj["playlistName"] = m_playlistName;
        playlistObj["playlistPath"] = m_playlistPath;

        JSONArray playlistList = new JSONArray();
        playlistObj.Add("playlistList", playlistList);

        foreach (var videoData in m_list) {
            playlistList.Add(videoData.Serialize());
        }
        return playlistObj;
    }
    #endregion
}
[System.Serializable]
public sealed class NameHashPair {
    public string filename;
    public string filehash;

    /// <summary>
    /// convert to SimpleJSON.JSONClass
    /// </summary>
    /// <returns></returns>
    public JSONClass Serialize()
    {
        JSONClass obj = new JSONClass();
        obj["filename"] = filename;
        obj["filehash"] = filehash;
        return obj;
    }

    public static NameHashPair Make(string name, string hash)
    {
        NameHashPair f = new NameHashPair();
        f.filename = name;
        f.filehash = hash;
        return f;
    }
    /// <summary>
    /// deserialize from a SimpleJSON.JSONClass
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static NameHashPair Deserialize(JSONClass node)
    {
        return Make(node["filename"].Value, node["filehash"].Value);
    }
    /// <summary>
    /// remove some special characters inserted into filenames to conform to URL standards
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string RemoveSpecial(string input)
    {
        string temp = input;
        temp = temp.Replace('+', ' ');
        temp = temp.Replace("%E2%80%93", "–");
        temp = temp.Replace("%26", "&");
        return temp;
    }
    /// <summary>
    /// re-insert the special characters or aws s3 links
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string SwapForSpecial(string input)
    {
        string temp = input;
        temp = temp.Replace(' ', '+');
        temp = temp.Replace("–", "%E2%80%93");
        temp = temp.Replace("&", "%26");
        return temp;
    }
}
public class DataHelper {
    /// <summary>
    ///  for use on windows only
    /// </summary>
    /// <param name="data"></param>
    /// <param name="path"></param>
    public static void SaveFileLocal(byte[] data, string path)
    {
        System.IO.File.WriteAllBytes(path, data);
    }
}


