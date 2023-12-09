using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace JULESTech.Resources {
    public sealed class AssetBundleManager : MonoBehaviour {
        #region Constants
        /// final bundle location should be AndroidAssetBundleServer/BuildBranch/GetPlatform()/
        //const string LocalServerAddress = "http://192.168.1.3:2468";
        const string AssetBundleManifestLabel = "assetbundlemanifest";
        const string StartupAssetBundleNameFilterList = "assetbundleLoadFilter.txt";
        public const string urlConfigFilename = "assetbundleServerURL";
        public static string ConfigFileFullPath { get { return "Assets/App/Resources/assetConfigs/" + urlConfigFilename; } }
        #endregion
        #region Singleton
        static AssetBundleManager _inst;
        public static AssetBundleManager Instance {
            get {
                if (_inst == null) {
                    GameObject obj = new GameObject("JULESTech.AssetBundleManager");
                    _inst = obj.AddComponent<AssetBundleManager>();

                    //obj.hideFlags = HideFlags.HideAndDontSave;
                    DontDestroyOnLoad(obj);
                }
                return _inst;
            }
        }
        /*
        private void Awake()
        {
            if (_inst == null) {
                _inst = this;
                DontDestroyOnLoad(gameObject);
            } else {
                return;
            }
        }
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if (_inst == null) {
                GameObject obj = new GameObject("JULESTech.AssetBundleManager");
                _inst = obj.AddComponent<AssetBundleManager>();
                DontDestroyOnLoad(obj);
            }
        }
        //*/
        #endregion
        #region Simulation Mode
#if UNITY_EDITOR
        // These values are only around in editor mode; 
        public static bool IsSimulationMode {
            get {
                return UnityEditor.EditorPrefs.GetBool("AssetBundleSimulationMode", true);
            }
            set {
                UnityEditor.EditorPrefs.SetBool("AssetBundleSimulationMode", value);
            }
        }
#endif // UNITY_EDITOR
        #endregion;
        #region ServerURL
        static TextAsset serverURLConfigFile = null;
        public static string ServerURL {
            get {
                if(serverURLConfigFile == null) {
                    serverURLConfigFile = UnityEngine.Resources.Load<TextAsset>("assetConfigs/assetbundleServerURL");
                }
                return serverURLConfigFile.text +'/'+ AssetBundles.Utility.GetPlatformName();
            }
        }
        #endregion
        public bool ShowLogText = false;

        AssetBundleManifest m_Manifest = null;
        Dictionary<string, AssetBundle> m_LoadedAssetBundles = new Dictionary<string, AssetBundle>();
        
        #region Callback Events
        /// <summary>
        /// event triggered by LoadAssetBundle at before loading starts
        /// </summary>
        public event System.Action<string> OnLoadAssetBundleBegin; // <bundleName>
        /// <summary>
        /// event triggered by LoadAssetBundle when loading ends, giving the message
        /// </summary>
        public event System.Action<bool, string, string> OnLoadAssetBundleFinish; // <isSuccess, bundleName, message>
        #endregion

        #region System initializing
        static GenericCoroutineRequest _request;
        /// <summary>
        /// IAsyncRequest returns the following events:
        ///  - OnPrintMessage(string msg), 
        ///  - OnProgressUpdate(float progress), 
        ///  - OnFinish(bool isSuccess, string msg)
        /// </summary>
        /// <returns></returns>
        public static IAsyncRequest Initialize()
        {
            if (_request == null) {
                SimpleEventListener ls = new SimpleEventListener();
                GenericAsyncState state = new GenericAsyncState(ls);
                _request = new GenericCoroutineRequest(state, Instance.InitializeWithManifest);

                Action<bool, string> finishCallback = (bool success, string msg) => {
                    AssetBundleManager._request.Stop();
                    AssetBundleManager._request = null;
                };
                ls.Register("OnFinish", finishCallback);
            }
            return _request;
        }
        IEnumerator InitializeWithManifest(IAsyncRequest _iReq)
        {
            //GenericCoroutineRequest greq = _iReq as GenericCoroutineRequest;
            GenericAsyncState state = _iReq.CurrentState as GenericAsyncState;
            
            #if UNITY_EDITOR
            if (IsSimulationMode) {
                Debug.Log("[JULES's AssetBundleManager] Simulate mode");
                state.OnEnd(true, "Simulate assetbundle usage");
                yield break;
            } else
            #endif
            {
                //string temp = ServerURL;
                //hack for windows"
                //serverURL = Application.streamingAssetsPath;
                if (ShowLogText && state.EventListener.Contains("OnPrintMessage")) {
                    state.EventListener.Call<Action<string>>("OnPrintMessage")(string.Format("[JULES's AssetBundleManager] Connecting to assetbundle server at: {0}", ServerURL));
                }
            }

            #region Unity's Assetbundle Manifest download
            bool isError = false;
            string msg = "AssetBundle Manifest loaded.";
            yield return GetAssetBundleManifest (
                (res) => {
                    m_Manifest = res;
                }, 
                (errorMsg) => {
                    isError = true;
                    msg = string.Format("Download of assetbundle manifest failed, {0}", errorMsg);
                }
            );

            // ERROR HANDLING - Manifest download failed
            state.OnEnd(!isError, msg);
            #endregion
        }        
        #endregion

        // TOM: TODO: convert to IAsyncRequest and AsyncState 
        #region Loading functionalities
        public AsyncOperation LoadSceneAsync (string bundleName, string sceneName)
        {
            throw new NotImplementedException();
            return null;
        }
        /// <summary>
        /// Load a scene asynchronously from scenestreaming assetbundle
        /// </summary>
        /// <param name="bundleName"></param>
        /// <param name="sceneName"></param>
        /// <param name="isAdditive"></param>
        /// <param name="allowSceneActivation"></param>
        /// <param name="on_startLoading"></param>
        /// <returns></returns>
        public static Coroutine LoadSceneAsync(string bundleName, string sceneName, bool isAdditive, bool allowSceneActivation = true, System.Action<AsyncOperation> on_startLoading = null)
        {
            return Instance.StartCoroutine(Instance.loadSceneAsync(bundleName, sceneName, isAdditive, allowSceneActivation, on_startLoading));
        }
        IEnumerator loadSceneAsync(string bundleName, string sceneName, bool isAdditive, bool allowSceneActivation = true, System.Action<AsyncOperation> on_startLoading = null)
        {
#if UNITY_EDITOR
            if (IsSimulationMode) {
                AsyncOperation m_Operation = null;
                string[] levelPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(bundleName, sceneName);
                if (levelPaths.Length == 0) {
                    ///@TODO: The error needs to differentiate that an asset bundle name doesn't exist
                    //        from that there right scene does not exist in the asset bundle...

                    Debug.LogError("There is no scene with name \"" + sceneName + "\" in " + bundleName);
                    yield break;
                }

                if (isAdditive)
                    m_Operation = UnityEditor.EditorApplication.LoadLevelAdditiveAsyncInPlayMode(levelPaths[0]);
                else
                    m_Operation = UnityEditor.EditorApplication.LoadLevelAsyncInPlayMode(levelPaths[0]);
                if (on_startLoading != null)
                    on_startLoading(m_Operation);
            } else
#endif
            {
                AssetBundle assetBundle = null;
                if (Instance.m_LoadedAssetBundles.ContainsKey(bundleName)) {
                    assetBundle = Instance.m_LoadedAssetBundles[bundleName];
                } else {
                    yield return loadAssetBundle(bundleName, (res) => { assetBundle = res; }, null);
                }

                if (assetBundle == null) {
                    Debug.LogError(string.Format("assetbundle [{0}] not found", bundleName));
                    yield break;
                }

                var ops = SceneManager.LoadSceneAsync(sceneName, ((isAdditive) ? LoadSceneMode.Additive : LoadSceneMode.Single));
                ops.allowSceneActivation = allowSceneActivation;

                if (on_startLoading != null) on_startLoading(ops);
            }
        }

        /// <summary>
        /// Load an asset from designated assetbundle
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="name"></param>
        /// <param name="on_complete"></param>
        /// <returns></returns>
        public static Coroutine LoadAsset(string assetBundleName, string name, System.Action<UnityEngine.Object> on_complete)
        {
            return Instance.StartCoroutine(loadAsset(assetBundleName, name, on_complete));
        }
        static IEnumerator loadAsset(string assetBundleName, string name, System.Action<UnityEngine.Object> on_complete)
        {
#if UNITY_EDITOR
            // simulate bundle mode
            if (IsSimulationMode) {
                string[] assetPaths = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundleAndAssetName(assetBundleName, name);
                if (assetPaths.Length == 0) {
                    Debug.LogErrorFormat("There is no asset with name \"{0}\" in {1}", name, assetBundleName);
                    //return null;
                    yield break;
                }
                UnityEngine.Object target = UnityEditor.AssetDatabase.LoadMainAssetAtPath(assetPaths[0]);
                // return object
                if (on_complete != null) {
                    on_complete(target);
                }
            } else
#endif
            {
                // get bundle
                AssetBundle assetBundle = null;
                // wait for bundle loading
                yield return LoadAssetBundle(assetBundleName, (result) => { assetBundle = result; }, null);

                if (assetBundle == null) {
                    // not found in any bundle
                    Debug.LogErrorFormat("assetbundle [{0}] not found", assetBundleName);
                    yield break;
                }

                if (assetBundle.isStreamedSceneAssetBundle) {
                    Debug.LogErrorFormat("Cannot load normal asset from Streamed Scene AssetBundle [{0}].", assetBundleName);
                    yield break;
                }

                AssetBundleRequest ops = assetBundle.LoadAssetAsync(name);
                //yield return ops;
                while (ops.isDone == false) {
                    if (Instance.ShowLogText)
                        Debug.LogFormat("Loading [{0}/{1}], {2}%", assetBundleName, name, ops.progress);
                    yield return null;
                }
                if (on_complete != null)
                    on_complete(ops.asset);
            }
        }

        public static Coroutine LoadAssetTextAsset(string assetBundleName, string name, System.Action<TextAsset> on_complete)
        {
            return Instance.StartCoroutine(loadAsset(assetBundleName, name, (loadedAsset) => {
                if (on_complete != null) on_complete(loadedAsset as TextAsset);
            }));
        }
        /// <summary>
        /// Load assetbundle, given the name
        /// </summary>
        /// <param name="assetBundleName"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static Coroutine LoadAssetBundle(string assetBundleName, Action<AssetBundle> result, Action<float> progressUpdate = null, bool downloadDependancies = true)
        {
            return Instance.StartCoroutine(Instance.loadAssetBundle(assetBundleName, result, progressUpdate, downloadDependancies));
        }
        IEnumerator loadAssetBundle(string assetBundleName, Action<AssetBundle> result, Action<float> progressUpdate, bool downloadDependancies = true)
        {
            if (OnLoadAssetBundleBegin != null)
                OnLoadAssetBundleBegin(assetBundleName);

            #if UNITY_EDITOR
            if(IsSimulationMode) {
                yield break;
            }
            #endif
            // check for loaded in memory
            if (m_LoadedAssetBundles.ContainsKey(assetBundleName)) {
                if (result != null) {
                    result(m_LoadedAssetBundles[assetBundleName]);
                }
                string printLoadedAlready = string.Format("AssetBundle [{0}] has already been loaded.", assetBundleName);
                if (ShowLogText)
                    Debug.Log(printLoadedAlready);
                if (OnLoadAssetBundleFinish != null)
                    OnLoadAssetBundleFinish(true, assetBundleName, printLoadedAlready);
                yield break;
            }
            // requested assetbundle not loaded yet
            // need to retry
            UnityWebRequest req = null;
            int currRetryCount = 0;
            int maxRetryCount = 5;
            #region Download main bundle
            while (currRetryCount < maxRetryCount) {
                req = MakeAssetBundleRequest(assetBundleName);
                var ops = req.Send();
                //*
                while (ops.isDone == false) {
                    if (progressUpdate != null) progressUpdate(ops.progress);
                    yield return null;
                }
                //*/
                yield return ops;
                //yield return req.Send();
                if (progressUpdate != null) progressUpdate(req.downloadProgress);
                if (req.isError) {
                    if (result != null) {
                        result(null);
                    }
                    Debug.LogError(req.error);
                    if (OnLoadAssetBundleFinish != null) {
                        OnLoadAssetBundleFinish(false, assetBundleName, assetBundleName + " download failed " + req.error);
                    }
                    currRetryCount++;
                    if (currRetryCount >= maxRetryCount)
                        yield break;
                }else {
                    currRetryCount = maxRetryCount;
                }
            }
            #endregion

            #region Downloading of assetbundle successful
            if(ShowLogText)
                Debug.Log(string.Format("AssetBundle [{0}] download success", assetBundleName));
            DownloadHandlerAssetBundle handler = req.downloadHandler as DownloadHandlerAssetBundle;
            // hold in 
            m_LoadedAssetBundles.Add(assetBundleName, handler.assetBundle);
            #endregion

            #region Download dependencies
            if (downloadDependancies) {
                string[] dep = m_Manifest.GetAllDependencies(assetBundleName);
                if (dep.Length > 0) {
                    for (int i = 0; i < dep.Length; ++i) {
                        if (m_LoadedAssetBundles.ContainsKey(dep[i])) {
                            //dependency has already been loaded
                            if (ShowLogText)
                                Debug.Log(string.Format("AssetBundle dependency [{0}] has already been loaded.", dep[i]));
                            continue;
                        }
                        req = MakeAssetBundleRequest(dep[i]);
                        yield return req.Send();

                        if (req.isError) {
                            if (result != null) {
                                result(null);
                            }
                            Debug.LogError(req.error);
                            if (OnLoadAssetBundleFinish != null) {
                                OnLoadAssetBundleFinish(false, assetBundleName, dep[i] + " download of dependency failed," + req.error);
                            }
                            yield break;
                        }
                    }
                }
            }
            #endregion

            #region requested bundle and dependencies successfully downloaded
            if (result != null) {
                result(handler.assetBundle);
            }
            if (OnLoadAssetBundleFinish != null) {
                OnLoadAssetBundleFinish(true, assetBundleName, string.Format("Requested assetbundle [{0}] and dependencies successfully downloaded.", assetBundleName));
            }
            #endregion
        }
        
        IEnumerator GetAssetBundleManifest(System.Action<AssetBundleManifest> on_success, System.Action<string> on_error = null)
        {
            UnityWebRequest webReq = UnityWebRequest.GetAssetBundle(ServerURL + '/' + AssetBundles.Utility.GetPlatformName());
            webReq.timeout = 10000;
            bool isError = false;

            yield return webReq.Send();

            if (webReq.isError) {
                if (on_error != null) {
                    on_error(webReq.error);
                }
            } else {
                DownloadHandlerAssetBundle dlHandler = webReq.downloadHandler as DownloadHandlerAssetBundle;
                AssetBundle manifestBundle = dlHandler.assetBundle;

                if (manifestBundle != null) {
                    AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>(AssetBundleManifestLabel);
                    Debug.Log("Manifest: [" + manifest + "] downloaded successfully");

                    if (ShowLogText) {
                        System.Text.StringBuilder build = new System.Text.StringBuilder();
                        build.AppendLine("[AssetBundles]:");
                        var str = manifest.GetAllAssetBundles();
                        foreach (var name in str) {
                            build.Append("\t");
                            build.AppendLine(name);
                        }
                        Debug.Log(build.ToString());
                    }
                    if (manifest != null) {
                        if (on_success != null) { on_success(manifest); }
                    } else {
                        isError = true;
                    }
                } else {
                    // manifestBundle == null
                    isError = true;
                }
            }

            if(isError && on_error != null) {
                on_error("failed to download manifest.");
            }
        }
        #endregion

        #region Non-async stuff
        public static bool AssetBundleIsLoaded(string assetBundleName)
        {
            return Instance.m_LoadedAssetBundles.ContainsKey(assetBundleName);
        }

        public static AssetBundle GetBundle(string name)
        {
            if (Instance.m_LoadedAssetBundles.ContainsKey(name)) {
                return Instance.m_LoadedAssetBundles[name];
            }
            return null;
        }
        public static string[] GetAllAssetBundleName()
        {
            return Instance.m_Manifest.GetAllAssetBundles();
        }
        #endregion

        #region helper functions
        UnityWebRequest MakeAssetBundleRequest(string bundleName)
        {
            uint crc = 0;
            return UnityWebRequest.GetAssetBundle(string.Format("{0}/{1}", ServerURL, bundleName), m_Manifest.GetAssetBundleHash(bundleName), crc);
        }
        /// <summary>
        /// Helper to get sprite out of a LoadAsset return value
        /// In editor, texture2d is returned, so will need to make a new sprite
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static Sprite GetSprite(UnityEngine.Object asset)
        {
            Texture2D tex = asset as Texture2D;
            Sprite sp = asset as Sprite;
            if (tex == null && sp != null)
                return sp;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        }
        #endregion
    }

    public class LoadAssetBundleRequest : GenericCoroutineRequest {
        public LoadAssetBundleRequest(AsyncState state, Func<IAsyncRequest, IEnumerator> job) : base(state, job)
        {
        }
    }
}