using UnityEngine;
using System.Collections;

public static class LevelDesignData
{
    //****************************************
    //  LEVEL LOADING
    //****************************************

    static SimpleJSON.JSONArray LevelListing = null;
    static SimpleJSON.JSONNode CurrentLevel = null;
    public static int CurrentLevelID = -99;

    /*
    public static void LoadFile (string filename)
    {
        string leveljson = Cleanbox.LoadTextFile("TsumTsumLevelDesign");
        LevelListing = SimpleJSON.JSON.Parse(leveljson).AsArray;   
    }
    //*/

    private static void Clear()
    {
        LevelListing = null;
        CurrentLevel = null;
        CurrentLevelID = -99;
    }

    public static void LoadFileFromAssetBundle(string assetBundleName, string filename, System.Action onComplete=null)
    {
        Clear();

        JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(assetBundleName, filename, (loadedAsset) => {
            LevelListing = SimpleJSON.JSON.Parse(loadedAsset.text).AsArray;
            if (onComplete != null) {
                onComplete();
            }
        });
    }

    public static SimpleJSON.JSONNode LoadLevel(int level)
    {
        if (LevelListing == null) {
            Debug.Log("No file loaded!!");
            throw new System.IO.FileNotFoundException("No data found, file is not loaded");
        }

        if (CurrentLevelID != level) {
            CurrentLevelID = level;
            CurrentLevel = LevelListing[level];
        }

        return LevelListing[level];
    }

    public static SimpleJSON.JSONNode GetData(string parametername)
    {
        if (LevelListing == null)
        {
            //string leveljson = Cleanbox.LoadTextFile("TsumTsumLevelDesign");
            //LevelListing = SimpleJSON.JSON.Parse(leveljson).AsArray;
            throw new System.IO.FileNotFoundException("No data found, file is not loaded");
        }

        SimpleJSON.JSONNode result = CurrentLevel[parametername];
        if (result == null) Debug.Log(parametername + " not found in JSON.");
        return result;
    }

}
