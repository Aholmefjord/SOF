using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJSON;

namespace JULESTech {
    public class Buildscript {
        const string SIGNING_KEYSTORE_LOCATION = @".\Submission\Android\jules.keystore";
        const string SIGNING_KEYSTORE_PASSWORD = "7E4Zkgmz7Bquae5wrsTp";
        const string SIGNING_KEYALIAS = "jules";
        const string SIGNING_KEYALIAS_PASSWORD = "7E4Zkgmz7Bquae5wrsTp";
        static void BuildAndroid()
        {
            // load things from jsonfile
            JSONClass settings = LoadSettingFile();

            // setup the android keystore signing values
            AndroidKeystoreSigning();

            string error = BuildPipeline.BuildPlayer( MakePlayerOptions(settings) );

            if (string.IsNullOrEmpty(error) == false) {
                Debug.LogError("Failed to build, " + error);
                //return;
            }
            // build assetbundles associated to the current build
            BuildAndroidAssetBundles(settings);
        }

        #region Android Helper
        static AssetBundleManifest BuildAndroidAssetBundles(JSONClass settings)
        {
            PrimeOutputFolder(settings["assetbundleOutputPath"].Value);
            
            BuildAssetBundleOptions abOptions = (BuildAssetBundleOptions)settings["assetbundleCompression"].AsInt;
            AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(settings["assetbundleOutputPath"].Value,abOptions,BuildTarget.Android);
            if(manifest==null) {
                //error;
                Debug.LogError("Failed to build AssetBundles.");
            }
            return manifest;
        }
        static BuildPlayerOptions MakePlayerOptions(JSONClass settings)
        {
            PrimeOutputFolder(settings["apkOutputPath"].Value);
            //BuildPlayerOptions asd;asd.
            return new BuildPlayerOptions() {
                locationPathName = settings["apkOutputPath"].Value+"/sof.apk",
                target = BuildTarget.Android,
                //targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.CompressWithLz4 | BuildOptions.ShowBuiltPlayer,
                scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
                assetBundleManifestPath = settings["assetbundleOutputPath"]
            };
        }
        /// <summary>
        /// Tom: making this public is a hack
        /// </summary>
        public static void AndroidKeystoreSigning()
        {
            PlayerSettings.Android.keystoreName = SIGNING_KEYSTORE_LOCATION;
            PlayerSettings.Android.keystorePass = SIGNING_KEYSTORE_PASSWORD;
            PlayerSettings.Android.keyaliasName = SIGNING_KEYALIAS;
            PlayerSettings.Android.keyaliasPass = SIGNING_KEYALIAS_PASSWORD;
        }
        #endregion

        #region helper functions
        static JSONClass LoadSettingFile()
        {
            string jsonData = "";
            //@"{apkOutputPath: Build/Android/, assetbundleOutputPath: Build/Android/AssetBundles/}";
            /*
            TextAsset asset = UnityEngine.Resources.Load<TextAsset>("buildSetting");
            if (asset == null) {
                Debug.LogError("buildSetting.txt file is missing.");
                throw new FileNotFoundException("buildSetting.txt file is missing.");
            }
            jsonData = asset.text;
            //*/

            jsonData = File.ReadAllText(string.Format("{0}/Automation/buildSetting.txt", System.Environment.CurrentDirectory));

            JSONClass settings = JSON.Parse(jsonData).AsObject;
            if (settings == null) {
                Debug.LogError("loading of buildSetting.txt as JSON file failed.");
                throw new FileNotFoundException("loading of buildSetting.txt as JSON file failed.");
            }
            return settings;
        }
        static void PrimeOutputFolder(string dir)
        {
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
        }
        #endregion
    }
}