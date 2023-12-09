using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Amazon;
using Amazon.S3;

namespace JULESTech.AWS.S3
{
    /// <summary>
    /// this access s3 bucket with accesskey and secret accesskey; based on Amazon's provided S3Example
    /// Contains only client credentials
    /// </summary>
    public class SimpleS3Controller : MonoBehaviour {
        private SimpleS3ControllerSetting setting;

        #region private accessors
        private IAmazonS3 _s3Client;
        public IAmazonS3 Client {
            get{
                if (_s3Client == null) {
                    _s3Client = new AmazonS3Client(setting.AccessKey, setting.SecretAccessKey, setting.S3RegionEndpoint);
                }
                //test comment
                return _s3Client;
            }
        }
        #endregion

        private void Init(){
            UnityInitializer.AttachToGameObject(this.gameObject);

            if (setting == null)
            {
                // try to load setting from resource?
                SimpleS3ControllerSetting obj = UnityEngine.Resources.Load<SimpleS3ControllerSetting>("awss3/s3Credential");
                if (obj != null)
                {
                    setting = obj;
                    Debug.Log("[SimpleS3Controller] settings automatically loaded from Resources folder.");
                } else {
                    Debug.LogError("[SimpleS3Controller] settings not loaded from Resources folder, S3 procedures will not work");
                }
            }
            AWSConfigsS3.UseSignatureVersion4 = true;
            AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
            /*
            AWSConfigs.LoggingConfig.LogTo = LoggingOptions.UnityLogger;
            AWSConfigs.LoggingConfig.LogResponses = ResponseLoggingOption.Always;
            AWSConfigs.LoggingConfig.LogMetrics = true;
            AWSConfigs.CorrectForClockSkew = true;
            //*/
        }

        /* //multiple clients for different regions
        Dictionary<string, IAmazonS3> mClients = new Dictionary<string, IAmazonS3>();
        public IAmazonS3 GetClient (SimpleS3ControllerSetting config)
        {
            IAmazonS3 lClient = null;
            if (mClients.ContainsKey(config.name) == false) {
                lClient = new AmazonS3Client(config.AccessKey, config.SecretAccessKey, config.S3RegionEndpoint);
                mClients.Add(config.name, lClient);
            } else {
                lClient = mClients[config.name];
            }
            return lClient;
        }
        //*/
        // this is assuming only 1set of keys
        Dictionary<RegionEndpoint, IAmazonS3> mClientsCache = new Dictionary<RegionEndpoint, IAmazonS3>();
        public IAmazonS3 GetClientByRegion (RegionEndpoint region) {
            IAmazonS3 lClient = null;
            if (mClientsCache.ContainsKey(region) == false) {
                lClient = new AmazonS3Client(setting.AccessKey, setting.SecretAccessKey, region);
                mClientsCache.Add(region, lClient);
            } else {
                lClient = mClientsCache[region];
            }
            return lClient;
        }

        private void OnApplicationQuit()
        {
            //cleanup when app dies?
            Client.Dispose();
        }
        
        #region Singleton
        static SimpleS3Controller _instance;
        public static SimpleS3Controller Instance {
            get {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("S3Credential");
                    obj.hideFlags = HideFlags.NotEditable;
                    DontDestroyOnLoad(obj.gameObject);
                    _instance = obj.AddComponent<SimpleS3Controller>();
                    _instance.Init();
                }
                return _instance;
            }
        }
        /*
        [RuntimeInitializeOnLoadMethod]
        private void MakeInstance()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                _instance = this;
                hideFlags = HideFlags.NotEditable;
                DontDestroyOnLoad(gameObject);
            }
        }
        //*/
        #endregion
    }
}