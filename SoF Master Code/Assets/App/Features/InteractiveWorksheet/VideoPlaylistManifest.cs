using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;

[System.Serializable]
public class VideoPlaylistManifest {
    #region Member Variables
    /// <summary>
    /// list of name of videoplaylist's json files
    /// </summary>]
    public List<string> m_paths = new List<string>();
    #endregion
    /*
    public int Version {
        get {
            return m_config["version"].AsInt;
        }
    }//*/
    public int PathCount { get {
            return m_paths.Count;
        }
    }

    #region initializing
    public VideoPlaylistManifest() { }
    /// <summary>
    /// get manifest from the path, can be local or url
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public VideoPlaylistManifest(string stringData)
    {
        JSONClass config = JSON.Parse(stringData).AsObject;
        
        JSONArray list = config["playlists"].AsArray; // this just contains the names only
        for(int i = 0; i < list.Count; ++i) {
            m_paths.Add(list[i].Value);
        }
    }
    public SimpleJSON.JSONNode Serialize()
    {
        JSONClass obj = new JSONClass();
        JSONArray arr = new JSONArray();

        obj.Add("playlists", arr);
        foreach(var path in m_paths) {
            arr.Add(path);
        }
        return obj;
    }
    #endregion
    /*
    public IEnumerator DownloadAllPlaylistConfig(string serverUrl, string cacheUrl)
    {
        for (int i = 0; i < m_paths.Count; ++i) {
            string textData = "";

            // download server config
            // load cached config
            // if no cache, no server: utter failure
            // if no cache, yes server: cache server config & mark CHANGED(means need reDL/check playlist cache)
            // if yes cache, no server: continue with cached
            // if yes cache, yes server: 
            //  if server is newer version: cache server config & mark CHANGED(means need reDL/check playlist cache)
            //      else continue with cached

            VideoConfigLoadOptions options = new VideoConfigLoadOptions() {
                serverURL = serverUrl,
                cacheURL = cacheUrl,
                filename = m_paths[i]
            };
            yield return VideoConfigComparer.GetConfig (options, (resultType, resultStringData) => {
                textData = resultStringData;
            });

            JSONNode node = JSON.Parse(textData);
            VideoPlaylist pl = VideoPlaylist.Deserialize(node.AsObject);
            m_playlists.Add(pl.m_playlistName, pl);
        }
    }
    //*/
}//*/