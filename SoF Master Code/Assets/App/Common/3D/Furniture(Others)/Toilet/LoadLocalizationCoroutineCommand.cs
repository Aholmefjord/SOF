using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// This is a coroutine command, 
/// </summary>
public class LoadLocalizationCoroutineCommand : ChainableMonoCommand {
    [SerializeField]
    GenericProgressGUI loadingGUI;

    protected override IEnumerator Run()
    {
        /*
        MultiLanguage.getInstance();
        ExecuteNextCommand();
        yield break;
        //*/

        // TODO: to download specific language only
        var req = MultiLanguage.Initialize();
        req.EventListener.Register("OnPrintMessage", new Action<string>(loadingGUI.ShowInfo));
        req.EventListener.Register("OnProgressUpdate", new Action<float>(loadingGUI.SetProgress));
        req.EventListener.Register("OnFinish", new Action<bool, string>((success, msg) => {
            if (success) {
                loadingGUI.ClearInfo();
            } else {
                loadingGUI.ShowInfo(msg);
                Debug.LogError(msg);
            }
        }));
        var ops = req.Execute();
        yield return ops;

        if (ops.isError) { yield break; }

        ExecuteNextCommand();
    }
}
