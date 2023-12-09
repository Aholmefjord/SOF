using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// VideoPlaylistManager initializer; Initialize and checks video cache
/// </summary>
public class VideoPlaylistLoadCommand : ChainableMonoCommand
{
    [SerializeField] // a update ui object here, to catch progress
    GenericProgressGUI progressUI;

    protected override IEnumerator Run()
    {
        //Debug.Log("ServerURL: " + VideoPlaylistManager.ServerURL);
        progressUI.ShowInfo("Start Loading Videoplaylists");

        #region Downloading all configs
        progressUI.Reset();
        var req = VideoPlaylistManager.Initialize();
        req.EventListener.Register("OnFinish", new Action<bool, string>((success, msg)=> {
            if (success) {
                progressUI.ShowInfo(msg);
                Debug.Log(msg);
            }else {
                Debug.LogError(msg);
            }
        }));
        var state = req.Execute();
        yield return state;

        if (state.isError) { yield break; }
        yield return new WaitForSeconds(1);
        #endregion

        #region Download Video files
        progressUI.Reset();
        Debug.Log("Checking video files.");
        progressUI.ShowInfo("Checking videos");
        
        var dlvideoReq = VideoPlaylistManager.DownloadAllVideos();
        dlvideoReq.EventListener.Register("OnPrintMessage", new Action<string>(progressUI.ShowInfo));
        dlvideoReq.EventListener.Register("OnProgressUpdate", new Action<float>(progressUI.SetProgress));
        dlvideoReq.EventListener.Register("OnFinish", new Action<bool, string>((success, msg)=> {
            progressUI.ShowInfo(msg);
        }));

        var dlProgressState = dlvideoReq.Execute();
        yield return dlProgressState;

        if(dlProgressState.isError) { yield break; }
        #endregion

        Debug.Log("Done");
        if (nextCommand != null)
            nextCommand.Execute();
    }
}