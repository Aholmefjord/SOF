using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

// singleton but 
public class VideoPlaylistManager: MonoBehaviour {
    #region Singleton
    static VideoPlaylistManager _inst;
    public static VideoPlaylistManager Instance {
        get {
            if (_inst == null) {
                GameObject obj = new GameObject("VideoPlaylistManager.Singleton");
                _inst = obj.AddComponent<VideoPlaylistManager>();
                obj.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(obj);
            }
            return _inst;
        }
    }
    #endregion
    static TextAsset serverFile;
    public static string ServerURL {
        get {
            if (serverFile == null) {
                serverFile = Resources.Load<TextAsset>("assetConfigs/videoPlaylistServerURL");
            }
            return serverFile.text;
        }
    }

    ManagerState m_CurrState = ManagerState.Uninitialize;
    VideoPlaylistManifest m_Manifest;
    Dictionary<string, VideoPlaylist> m_allPlaylist = new Dictionary<string, VideoPlaylist>();
    
    #region ManifestFilename Generation
    static readonly string filenamePrefix = "videoPlaylistManifest";
    static readonly string filenamePostfix = ".json";
    public static string DefaultManifestFilename {
        get {
            return string.Format("{0}/{1}", filenamePrefix, filenamePostfix);
        }
    }
    public static string ManifestByLanguage(string langName = "")
    {
        return string.Format("{0}{1}{2}", filenamePrefix, langName, filenamePostfix);
    }
    #endregion

    #region Initializing code
    static GenericCoroutineRequest _request;
    public static IAsyncRequest Initialize()
    {
        if (_request == null) {
            SimpleEventListener ls = new SimpleEventListener();
            GenericAsyncState state = new GenericAsyncState(ls);
            _request = new GenericCoroutineRequest(state, Instance.LoadConfigs);
            Action<bool, string> finishCallback = (bool success, string msg) => {
                VideoPlaylistManager._request.Stop();
                VideoPlaylistManager._request = null;
            };
            ls.Register("OnFinish", finishCallback);
        }
        return _request;
    }
    IEnumerator LoadConfigs(IAsyncRequest _req)
    {
        GenericCoroutineRequest greq = _req as GenericCoroutineRequest;
        GenericAsyncState state = _req.CurrentState as GenericAsyncState;
        Action<string> Print = (msg) => {
            if (_req.EventListener.Contains("OnPrintMessage"))
                _req.EventListener.Call<Action<string>>("OnPrintMessage")(msg);
        };

        bool catastrophe = false;
        string manifestJsonString = "";
        string resultMessage = "";

        if (m_CurrState == ManagerState.Loading || m_CurrState == ManagerState.Initialized) {
            // do not load when system is still loading or loaded successfully;
            Debug.Log("Manager has either been initialized or still in the process doing so.");
            yield break;
        }
        m_CurrState = ManagerState.Loading;

        #region Initializing of Manifest
        VideoConfigLoadOptions loadOptions = new VideoConfigLoadOptions() {
            cacheURL = VideoPlaylistConstants.Cache.CACHE_LOCATION,
            serverURL = ServerURL,
            filename = ManifestByLanguage(string.Empty)
        };
        // yield return GetConfigOperation;
        // if( GetConfigOperation.isCatastrophe ){ yield break; }else{ //do normal stuff }
        yield return VideoConfigComparer.GetConfig(loadOptions, (resultType, resultStringData) => {
            manifestJsonString = resultStringData;
            if (resultType == VersionCheckStatus.FAIL) {
                // should stop the coroutine;
                catastrophe = true;
            } else {
                m_Manifest = new VideoPlaylistManifest(manifestJsonString);
            }
        });
        // ERROR HANDLING
        if (catastrophe) {
            resultMessage = "Failed to retrieve VideoPlaylist Manifest";
            m_CurrState = ManagerState.Error;
            state.OnEnd(false, resultMessage);
            yield break;
        }
        state.SetProgress(0.1f);
        #endregion

        #region Initializing all videoplaylist configs listed in Manifest
        bool isConfigDownloadSuccessFul = false;
        yield return DownloadAllPlaylistConfigs((progress) => {
            state.SetProgress(0.1f + (0.9f * progress));
        }, (success, msg) => {
            isConfigDownloadSuccessFul = success;
            resultMessage = msg;
        });
        #endregion

        if (isConfigDownloadSuccessFul)
            m_CurrState = ManagerState.Initialized;
        else
            m_CurrState = ManagerState.Error;

        state.OnEnd(isConfigDownloadSuccessFul, resultMessage);
    }
    #endregion

    IEnumerator DownloadAllPlaylistConfigs(Action<float> onUpdate = null, Action<bool, string> onFinish = null)
    {
        // TODO: condition to enter check ( Initialized/Loading )
        int playlistCount = m_Manifest.m_paths.Count;
        string textData = "";

        for (int i = 0; i < playlistCount; ++i) {
            textData = "";
            VideoConfigLoadOptions options = new VideoConfigLoadOptions() {
                serverURL = ServerURL,
                cacheURL = VideoPlaylistConstants.Cache.CACHE_LOCATION,
                filename = m_Manifest.m_paths[i]
            };
            bool noCacheNoServerVersion = false;
            yield return VideoConfigComparer.GetConfig(options, 
                (resultType, resultStringData) => {
                    textData = resultStringData;
                    if (resultType == VersionCheckStatus.FAIL) {
                        noCacheNoServerVersion = true;
                    }
                });

            if (noCacheNoServerVersion) {
                string errorMsg = "failed to download: " + options.filename;
                if (onFinish != null) {
                    onFinish(false, errorMsg);
                }
                m_CurrState = ManagerState.Error;
                Debug.LogError(errorMsg);
                yield break;
            }

            JSONNode node = JSON.Parse(textData);
            VideoPlaylist pl = VideoPlaylist.Deserialize(node.AsObject);
            m_allPlaylist.Add(pl.m_playlistName, pl);
            if (onUpdate != null) {
                onUpdate((float)(i+1) / (float)playlistCount);
            }
        }

        if (onFinish != null) {
            onFinish(true, "All playlist configs successfully downloaded.");
        }
        yield return null;
    }
    
    /*
    public IEnumerator GetPlaylist(string name, ResultCallback<VideoPlaylist> resultCallback)
    {
        if(g_CurrState == ManagerState.Uninitialize) {
            StartCoroutine(Initialize(null));
        }
        while(g_CurrState != ManagerState.Initialized) {
            if(g_CurrState == ManagerState.Error) {
                yield break;
            }
            yield return null;
        }
        // if playlist not loaded, load and wait for it;
        // if playlist is loading in progress, wait for it;
        // playlist loaded, return result;
        VideoPlaylist list = s_allPlaylist[name];

        yield return null;
    }
    //*/

    public static VideoPlaylist GetPlaylist(string name)
    {
        if(Instance.m_CurrState == ManagerState.Uninitialize || Instance.m_CurrState == ManagerState.Error) {
            throw new System.AccessViolationException("VideoPlaylistManager not initialized yet.");
        }
        return Instance.m_allPlaylist[name]; ;
    }
    public static string[] GetAllPlaylistNames()
    {
        string[] allNames = new string[Instance.m_allPlaylist.Count];
        int i = 0;
        foreach (var pair in Instance.m_allPlaylist) {
            allNames[i++] = pair.Value.m_playlistName;
        }
        return allNames;
    }
    
    public static IAsyncRequest DownloadAllVideos()
    {
        SimpleEventListener el = new SimpleEventListener();
        GenericAsyncState state = new GenericAsyncState(el);
        GenericCoroutineRequest req = new GenericCoroutineRequest(state, DownloadPlaylists);

        Action<bool, string> stopCall = (success, msg) => { req.Stop(); };
        el.Register("OnFinish", stopCall);

        return req;
    }
    static IEnumerator DownloadPlaylists(IAsyncRequest _req)
    {
        GenericCoroutineRequest greq = _req as GenericCoroutineRequest;
        GenericAsyncState state = _req.CurrentState as GenericAsyncState;

        bool isError = false;
        float currPlaylistLoaded = 0;
        float totalPlaylistCount = (float)Instance.m_allPlaylist.Count;
        
        VideoAsyncTaskEvents localEvents = new VideoAsyncTaskEvents();
        localEvents.PrintText += (msg) => {
            if (_req.EventListener.Contains("OnPrintMessage"))
                _req.EventListener.Call<Action<string>>("OnPrintMessage")(msg);
        }; ;
        localEvents.OnProgressUpdate += (singleListProgress) => {
            state.SetProgress(Progress(currPlaylistLoaded + singleListProgress, totalPlaylistCount));
        };
        localEvents.OnTaskEnd += (playlistLoadSuccess, msg) => {
            if (playlistLoadSuccess) {
                currPlaylistLoaded += 1;
                state.SetProgress(Progress(currPlaylistLoaded, totalPlaylistCount));
            } else {
                isError = true;
                state.OnEnd(isError, msg);
            }
        };

        foreach (var item in Instance.m_allPlaylist) {
            yield return item.Value.Download(ServerURL, localEvents);

            if (isError) {
                state.OnEnd(false, "VideoPlaylist failed to download: "+item.Value.m_playlistName);
                yield break;
            }
        }
        state.OnEnd(true, "Loading of video is successful");
    }

    // download progress helper
    static float Progress(float curr, float total)
    {
        return curr / total;
    }
    public enum ManagerState : int {
        Uninitialize,
        Error,
        Loading,
        Initialized
    }
}
