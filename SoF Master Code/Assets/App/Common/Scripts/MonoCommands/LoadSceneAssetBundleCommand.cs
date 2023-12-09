using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSceneAssetBundleCommand : LoadSceneCommand {
    [SerializeField]
    string assetbundleID = "";

    public override void Execute()
    {
        MainNavigationController.DoAssetBundleLoadLevel(assetbundleID, targetSceneName);
    }
}
