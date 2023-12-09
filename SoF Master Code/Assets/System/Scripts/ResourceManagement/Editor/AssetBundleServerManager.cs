using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleJSON;
using System;

namespace JULESTech.Resources {
    public class AssetBundleServerManager : EditorWindow {
        public const string SYSTEM_EDITOR_PATH = "Assets/System/Editor";

        ServerPresetSettings settings;
        Vector2 serverPresetScrollPos = Vector2.zero;
        string currentInputAddress;

        BuildSetting mCurrentBuildSetting;

        GUILayoutOption rowHeight = GUILayout.Height(18);
        GUILayoutOption sidePanelWidth = GUILayout.Width(150);
        
        private void OnEnable()
        {
            settings = ServerPresetSettings.Make();
            mCurrentBuildSetting = new BuildSetting();
            currentInputAddress = mCurrentBuildSetting.IP_Address;
        }
        private void OnDisable()
        {
        }
        
        private void OnGUI()
        {
            if (EditorApplication.isPlaying) {
                DisableGUIState();
            }

            // top toolbar
            using (var buttons = new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                if (GUILayout.Button("Set Android Keystore player settings", EditorStyles.toolbarButton)) {
                    Buildscript.AndroidKeystoreSigning();
                }
                if (GUILayout.Button("Reload", EditorStyles.toolbarButton)) {
                    mCurrentBuildSetting.LoadSetting();
                    AssetDatabase.Refresh();
                }
                if (GUILayout.Button("Clear Cache", EditorStyles.toolbarButton)) {
                    BundleCacheClean.CleanUnityCache();
                }
                GUILayout.FlexibleSpace();
                AssetBundleManager.IsSimulationMode = EditorGUILayout.ToggleLeft("Simulate AssetBundles in Editor", AssetBundleManager.IsSimulationMode);
            }

            if (AssetBundleManager.IsSimulationMode) {
                DisableGUIState();
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("AssetBundle Server Settings", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            DrawServerPresetSidePanel();

            using (var rightPanel = new EditorGUILayout.VerticalScope(EditorStyles.helpBox)) {
                using (var buttons = new EditorGUILayout.HorizontalScope()) {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("Current Server Settings", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                }
                GUILayout.Space(5);
                using (var fields = new EditorGUILayout.HorizontalScope()) {
                    GUILayout.Label("URL: ", GUILayout.Width(40));

                    DisableGUIState();
                    EditorGUILayout.TextField(currentInputAddress, rowHeight);
                    RecoverGUIState();

                    if (GUILayout.Button("Save", rowHeight, GUILayout.Width(80))) {
                        mCurrentBuildSetting.IP_Address = currentInputAddress;
                    }
                }
            } //right panel vertical scope
            EditorGUILayout.EndHorizontal();

            if (AssetBundleManager.IsSimulationMode) {
                RecoverGUIState();
            }
            if (EditorApplication.isPlaying) {
                RecoverGUIState();
            }
        }

        static int guidisableRefcount = 0;
        void DisableGUIState()
        {
            guidisableRefcount++;
            GUI.enabled = (guidisableRefcount <= 0);
        }
        void RecoverGUIState()
        {
            guidisableRefcount--;
            guidisableRefcount = Mathf.Clamp(guidisableRefcount, 0, int.MaxValue);
            GUI.enabled = (guidisableRefcount <= 0);
        }

        void DrawServerPresetSidePanel()
        {
            using (var sidePanel = new EditorGUILayout.VerticalScope(EditorStyles.helpBox, sidePanelWidth)) {
                // Header
                EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                GUILayout.Label("Server Presets", sidePanelWidth);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("R", EditorStyles.toolbarButton)) {
                    settings = ServerPresetSettings.Make();
                }
                if (GUILayout.Button("E", EditorStyles.toolbarButton)) {
                    //UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(AssetBundleManager.ConfigFileFullPath, 0);
                    EditorGUIUtility.PingObject(ServerPresetSettings.GetConfigFileAsset());
                }
                EditorGUILayout.EndHorizontal();

                serverPresetScrollPos = EditorGUILayout.BeginScrollView(serverPresetScrollPos);
                if (settings != null) {
                    for (int i = 0; i < settings.serverList.Count; ++i) {
                        if (GUILayout.Button(settings.serverList[i].id_name, EditorStyles.miniButton)) {
                            mCurrentBuildSetting.IP_Address = currentInputAddress = settings.serverList[i].ip_address;
                            Repaint();
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
        }

        [MenuItem("AssetBundle/AssetBundle Server Control", priority = 2)]
        static void OpenMgrWindow()
        {
            AssetBundleServerManager mgr = EditorWindow.CreateInstance<AssetBundleServerManager>();
            mgr.Show();
        }
    }

    #region Setting objects
    // setting class for quickly swapping environment
    abstract class DeploymentSettingBase {
        public abstract string name { get; }
        protected string ipaddress;
        public string IP_Address {
            get {
                return ipaddress;
            }
            set {
                ipaddress = value;
                SaveSetting();
            }
        }
        public abstract void LoadSetting();
        public abstract void SaveSetting();
    }
    class DevelopmentServerSetting : DeploymentSettingBase {
        public override string name {
            get {
                return "Development Server";
            }
        }

        public override void LoadSetting()
        {
            ipaddress = EditorPrefs.GetString("assetbundle_dev_server", "http://192.168.1.3:2468");
        }

        public override void SaveSetting()
        {
            EditorPrefs.SetString("assetbundle_dev_server", ipaddress);
        }
    }
    class BuildSetting : DeploymentSettingBase {
        public BuildSetting()
        {
            LoadSetting();
        }

        public override string name {
            get {
                return "Build Server";
            }
        }
        public override void LoadSetting()
        {
            string[] result = AssetDatabase.FindAssets(AssetBundleManager.urlConfigFilename);
            if (result.Length > 0) {
                TextAsset settingStr = AssetDatabase.LoadAssetAtPath<TextAsset>(AssetDatabase.GUIDToAssetPath(result[0]));
                ipaddress = settingStr.text;
                //Debug.Log("ip loaded: " + ipaddress);
            } else {
                // no file found, create one;
                var newFile = System.IO.File.CreateText(AssetBundleManager.ConfigFileFullPath + ".txt");
                newFile.Write("http://192.168.1.3:2468");
                Debug.LogError("file not found, create default");
                newFile.Close();
            }
        }

        public override void SaveSetting()
        {
            System.IO.StreamWriter fs = null;
            fs = System.IO.File.CreateText(AssetBundleManager.ConfigFileFullPath + ".txt");
            fs.Write(ipaddress);
            fs.Close();
            //Debug.Log("address saved "+ipaddress);
            AssetDatabase.Refresh();
        }
    }
    #endregion

    class ServerPresetSettings {
        public List<ServerPresetItem> serverList;

        const string ID_TAG = "idname";
        const string IP_TAG = "ipaddress";
        const string LIST_TAG = "serverList";
        const string CONFIG_FILENAME = "assetBundleIP_presets";
        ServerPresetSettings() { }
        /// <summary>
        /// load json data from txt file and make a setting obj
        /// </summary>
        /// <returns></returns>
        public static ServerPresetSettings Make()
        {
            ServerPresetSettings obj = new ServerPresetSettings();
            string[] results = AssetDatabase.FindAssets(CONFIG_FILENAME);
            if (results.Length > 0) {
                string path = AssetDatabase.GUIDToAssetPath(results[0]);
                TextAsset jsonSetting = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                JSONNode root = JSON.Parse(jsonSetting.text);
                JSONArray list = root[LIST_TAG].AsArray;
                obj.serverList = new List<ServerPresetItem>();

                //start populating new list 
                for (int i = 0; i < list.Count; ++i) {
                    obj.serverList.Add(ServerPresetItem.Make(list[i][ID_TAG].Value, list[i][IP_TAG].Value));
                }
            }else {
                Debug.LogError("[Jules AssetBundle Editor Tool]: serverPresets not loaded");
                return null;
            }

            return obj;
        }
        public static UnityEngine.Object GetConfigFileAsset()
        {
            string[] results = AssetDatabase.FindAssets(CONFIG_FILENAME);
            if (results.Length > 0) {
                string path = AssetDatabase.GUIDToAssetPath(results[0]);
                TextAsset jsonSetting = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                return jsonSetting;
            } else {
                Debug.LogError("[Jules AssetBundle Editor Tool]: serverPresets not found");
            }
            return null;
        }
        public void Save()
        {
            JSONArray array = new JSONArray();
            JSONClass itemNode = null;
            for(int i = 0; i < serverList.Count; ++i) {
                itemNode = new JSONClass();
                itemNode[ID_TAG] = serverList[i].id_name;
                itemNode[IP_TAG] = serverList[i].ip_address;
                array.Add(itemNode);
            }
            JSONClass root = new JSONClass();
            root.Add(LIST_TAG, array);

            var fs = System.IO.File.CreateText(FileLocation);
            fs.Write(root.ToString());
            fs.Close();
        }
        public string FileLocation {
            get {
                return string.Format(@"{1)/AssetBundleServerManager/{0}.txt", CONFIG_FILENAME, AssetBundleServerManager.SYSTEM_EDITOR_PATH);
            }
        }
    }
    class ServerPresetItem {
        public string id_name;
        public string ip_address;
        public static ServerPresetItem Make(string idname, string ip)
        {
            ServerPresetItem obj = new ServerPresetItem();
            obj.id_name = idname;
            obj.ip_address = ip;
            return obj;
        }
    }
}