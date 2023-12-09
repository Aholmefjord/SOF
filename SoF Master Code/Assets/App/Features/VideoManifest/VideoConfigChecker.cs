using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class VideoConfigComparer {
    /// <summary>
    /// 
    /// Will cache the version from server if its the newer version
    /// </summary>
    /// <param name="loadOptions"></param>
    /// <param name="resultCallback"></param>
    /// <returns></returns>
    public static IEnumerator GetConfig(VideoConfigLoadOptions loadOptions, ConfigFileCheckResult resultCallback)
    {
        bool cacheHit = true;
        bool serverHit = false;
        string serverManifestString = "";
        string cachedManifestString = "";

        #region try to get files from cache and server
        // dl from server
        UnityWebRequest req = UnityWebRequest.Get(loadOptions.SERVER_FILE_URL);
        yield return req.Send();
        if (req.isError) {
            Debug.LogError(req.error);
            yield break;
        }
        serverManifestString = req.downloadHandler.text;
        // try set manifest into XML(aws s3 return message if there is an error)
        System.Xml.XmlDocument awsResult = new System.Xml.XmlDocument();
        try {
            awsResult.LoadXml(serverManifestString);
        } catch (System.Exception) {
            // downloaded is not an XML, success!
            serverHit = true;
        }

        // open cache
        try {
            cachedManifestString = SimpleCache.OpenTextFromCacheStatic(loadOptions.CACHE_FILE_URL);
        } catch (System.Exception) {
            cacheHit = false;
        }
        #endregion

        string resultString = "";
        VersionCheckStatus resultType = VersionCheckStatus.FAIL;

        #region Version Checking resolution
        if (cacheHit) {
            if (serverHit) {
                // TOM: do i really need to hash here?
                string cacheHash = StringHelper.GetMD5Hash(cachedManifestString);
                string serverHash = StringHelper.GetMD5Hash(serverManifestString);
                if (cacheHash != serverHash) {
                    // use server's change
                    resultString = serverManifestString;
                    resultType = VersionCheckStatus.USE_SERVER;
                } else {
                    // cached version is same as server version, use cached version
                    resultString = cachedManifestString;
                    resultType = VersionCheckStatus.USE_CACHE;
                }
            } else {
                // got cache no server, continue with cache
                resultString = cachedManifestString;
                resultType = VersionCheckStatus.USE_CACHE;
            }
        } else {
            if (serverHit) {
                // no cache version but got server
                resultString = serverManifestString;
                resultType = VersionCheckStatus.USE_SERVER;
                if (loadOptions.overrideCache)
                    SimpleCache.WriteTextToCacheStatic(loadOptions.CACHE_FILE_URL, serverManifestString);
            } else {
                // everything missing
                Debug.LogError("Unable to load video playlists, no manifest found");
            }
        }
        #endregion

        if (resultCallback != null) {
            resultCallback(resultType, resultString);
        }
    }

}

/// <summary>
/// Download/Load options for video json manifest
/// </summary>
public class VideoConfigLoadOptions {
    /// <summary>
    /// cache location to load cached version from
    /// </summary>
    public string cacheURL;
    /// <summary>
    /// server location to download config from
    /// </summary>
    public string serverURL;
    /// <summary>
    /// filename of config; eg videoPlaylistManifest.json
    /// file to check should have the same filename on server and cache
    /// </summary>
    public string filename;
    /// <summary>
    /// will override the version in cache if the server is different
    /// true by default
    /// </summary>
    public bool overrideCache = true;

    /// <summary>
    /// property that provides fullpath to the file in cache
    /// </summary>
    public string CACHE_FILE_URL { get { return cacheURL + '/' + filename; } }
    /// <summary>
    /// property that provides fullpath to the file on server;
    /// </summary>
    public string SERVER_FILE_URL { get { return serverURL + '/' + filename; } }
}

public enum VersionCheckStatus : int {
    FAIL = 0,       // no cache found, cannot connect to server;
    USE_CACHE = 1,  // using cache version
    USE_SERVER = 2  // using server version;
}
public delegate void ConfigFileCheckResult(VersionCheckStatus status, string data);