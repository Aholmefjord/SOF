using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JULESTech.Resources;

/// <summary>
/// This is a coroutine command, 
/// </summary>
public class InitializeAssetBundleCommand : ChainableMonoCommand {
    const string StartupAssetBundleNameFilterList = "assetbundleLoadFilter.txt";

    [SerializeField]
    GenericProgressGUI loadingGUI;

    protected override IEnumerator Run()
    {
        #if UNITY_EDITOR
        if (AssetBundleManager.IsSimulationMode) {
            /*
            yield return new WaitForSeconds(5);
            yield return Instance.LoadFont("DISTROB");
            yield return Instance.LoadFont("PORKYS");
            yield return Instance.LoadFont("Cone");
            yield return Instance.LoadFont("PoplarStd");
            yield return Instance.LoadFont("Animated");
            yield return Instance.LoadFont("STHeitiSC-Light");
            yield return Instance.LoadFont("take_off_good_luck_new_kids");
            //*/
            loadingGUI.ShowInfo("Simulating Assetbundles");
            yield return new WaitForSeconds(5);
            ExecuteNextCommand();
            yield break;
        }
        #endif

        float starttime = Time.time;

        #region Manifest dl
        string serverURL = AssetBundleManager.ServerURL;
        var abInitRequest = AssetBundleManager.Initialize();
        abInitRequest.EventListener.Register("OnProgressUpdate", new Action<float>(loadingGUI.SetProgress));
        abInitRequest.EventListener.Register("OnPrintMessage", new Action<string>(loadingGUI.ShowInfo));
        abInitRequest.EventListener.Register("OnFinish", new Action<bool, string>((success, msg) => {
            if (success) {
                Debug.Log("AssetBundle Manifest loaded.");
                loadingGUI.ClearInfo();
            }else {
                Debug.LogError(msg);
                CreateErrorOverlay(msg);
            }
        }));

        var reqState = abInitRequest.Execute();
        yield return reqState;

        if (reqState.isError) {
            // display critical error overlay
            yield break;
        }
        #endregion

        loadingGUI.ShowInfo("Start downloading assetbundles");
        string[] allBundleNames = AssetBundleManager.GetAllAssetBundleName();
        List<string> bundlesToDownload = new List<string>();

        #region filtering to avoid downloading certain assetbundles
        TextFilter filter = null;
        yield return TextFilter.LoadFrom (
            string.Format("{0}/{1}", serverURL, StartupAssetBundleNameFilterList),
            (filterObj) => filter = filterObj, 
            null
        );

        for (int i = 0; i < allBundleNames.Length; ++i) {
            if (filter != null) {
                if (filter.IsFiltered(allBundleNames[i]) == false) {
                    // bundle not filtered, add to dl list
                    bundlesToDownload.Add(allBundleNames[i]);
                }
            } else {
                // no filtering, just include as no filter found
                bundlesToDownload.Add(allBundleNames[i]);
            }
        }
        #endregion

        float[] allProgress = new float[bundlesToDownload.Count];
        for (int i=0;i<bundlesToDownload.Count; ++i) {
            allProgress[i] = 0;
        }
        Func<float> SumOfProgress = () => {
            float sum = 0;
            for (int i = 0; i < allProgress.Length; ++i) {
                sum += allProgress[i];
            }
            return sum / (float)allProgress.Length;
        };

        BundleJumbledList dlList = new BundleJumbledList(bundlesToDownload);
        for (int i = 0; i < bundlesToDownload.Count; ++i) {
            //var request = AssetBundleManager.LoadAssetBundle();
            //var requestState = requestState.Execute();
            //yield return requestState;
            int currIndex = i;
            loadingGUI.ShowInfo(string.Format("Loading assetbundle [{0}]", dlList[currIndex]));
            yield return AssetBundleManager.LoadAssetBundle (
                dlList[currIndex], 
                (bundle) => {
                    loadingGUI.ShowInfo(string.Format("Assetbundle [{0}] has been loaded", bundle.name));
                }, 
                (progress) => {
                    allProgress[currIndex] = progress;
                    loadingGUI.SetProgress(SumOfProgress());
                },
                downloadDependancies: false
            );
        }

        #region fake simulate pause for splashintro to finish playing
        // check if everything downloaded, else, restart app
        float minWaitTime = 6;
        float duration = Time.time - starttime;
        if (duration < minWaitTime) {
            Debug.Log("time left: " + (minWaitTime - duration));
            yield return new WaitForSeconds(minWaitTime - duration);
        }
        #endregion

        ExecuteNextCommand();
    }

    struct BundleJumbledList {
        List<string> mBundlesToDownload;
        int[] mJumbledIndices;

        public BundleJumbledList(List<string> names)
        {
            mBundlesToDownload = names;
            mJumbledIndices = new int[mBundlesToDownload.Count];

            // offsetting the order in which clients download the AB to avoid 
            // everyone getting stuck on the same file, reducing dl speed;
            // this is not fullproof as this will only support as many "lanes" as bundles being dl-ed
            int index = TimeHelper.CurrentEpochTime % mBundlesToDownload.Count;
            for (int i = 0; i < mBundlesToDownload.Count; ++i) {
                mJumbledIndices[i] = index;
                index += 1;
                // wrap around back to the start
                if (index >= mBundlesToDownload.Count)
                    index = 0;
            }
        }
        public string this[int index] {
            get {
                return mBundlesToDownload[mJumbledIndices[index]];
            }
        }
        public int JumbledIndex(int index)
        {
            return mJumbledIndices[index];
        }
    }
    /// <summary>
    /// displays message for critical errors;
    /// </summary>
    /// <param name="errmessage"></param>
    /// <returns></returns>
    GameObject CreateErrorOverlay(string errmessage = "")
    {
        GameObject obj = (UnityEngine.Resources.Load("AssetBundleManagerErrorOverlay")) as GameObject;
        obj = GameObject.Instantiate(obj);
        obj.name = "AssetBundleManagerErrorOverlay";
        //LoadingScreenInstance loadingInst = obj.GetComponent<LoadingScreenInstance>();
        GameObject confirmButton = obj.FindChild("ConfirmButton");
        confirmButton.OnClick(() => {
            // restart splashscreen
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        });
        GameObject msglabel = obj.FindChild("ErrorMessageLabel");
        if (string.IsNullOrEmpty(errmessage)) {
            msglabel.SetText("An error has occured");
        }
        msglabel.SetText(errmessage);
        return obj;
    }
}