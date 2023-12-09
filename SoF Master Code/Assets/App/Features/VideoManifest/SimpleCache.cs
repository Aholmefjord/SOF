using UnityEngine;
using System.IO;

/// <summary>
/// use of Application.persistantDataPath
/// improvement to add: hash check
/// </summary>
public sealed class SimpleCache {
    string cachePath;
    string rootLocation = string.Empty;

    public string CACHE_LOCATION {
        get {
            string path = Root + cachePath;
            EnsureFolderExistStatic(path);
            return path;
        }
    }
    public string Root {
        get {
            if (string.IsNullOrEmpty(rootLocation)) {
                return Application.persistentDataPath;   
            } else {
                return rootLocation;
            }
        }
        set {
            rootLocation = value;
            if (string.IsNullOrEmpty(rootLocation) == false) {
                Directory.CreateDirectory(Root + cachePath);
            }
        }
    }

    #region init
    public static SimpleCache Load(string path)
    {
        SimpleCache c = new SimpleCache(path);
        return c;
    }
    SimpleCache() { }
    SimpleCache(string path)
    {
        cachePath = path;
        pathStartCheck(ref cachePath);
    }
    #endregion

    #region Simple Saving and Reading
    /// <summary>
    /// simepl file check
    /// </summary>
    /// <param name="path">starts with /</param>
    /// <returns></returns>
    public bool CacheExist(string path)
    {
        pathStartCheck(ref path);
        return System.IO.File.Exists(CACHE_LOCATION + path);
    }
    public static void WriteTextToCacheStatic(string fullPath, string data)
    {
        File.WriteAllText(fullPath, data);
    }
    public void WriteTextToCache(string filePath, string data)
    {
        pathStartCheck(ref filePath);
        WriteTextToCacheStatic(CACHE_LOCATION + filePath, data);
    }
    public void OpenTextFromCache(string filePath, ref string container)
    {
        pathStartCheck(ref filePath);
        container = OpenTextFromCacheStatic(CACHE_LOCATION + filePath);
    }
    /// <summary>
    /// static version of OpenTextFromCache
    /// </summary>
    /// <param name="fullPath"></param>
    /// <returns></returns>
    public static string OpenTextFromCacheStatic(string fullPath)
    {
        var strm = System.IO.File.OpenText(fullPath);
        string container = strm.ReadToEnd();
        strm.Close();
        return container;
    }
    public string OpenTextFromCache(string filePath)
    {
        pathStartCheck(ref filePath);
        return OpenTextFromCacheStatic(CACHE_LOCATION + filePath);
    }
    /// <summary>
    /// using path that is in this instance
    /// </summary>
    /// <param name="filePath">subpath of the location in the cache folder</param>
    /// <param name="data"></param>
    public void WriteByteToCache(string filePath, byte[] data)
    {
        pathStartCheck(ref filePath);
        System.IO.File.WriteAllBytes(CACHE_LOCATION + filePath, data);
    }
    public byte[] ReadByteFromCache(string filePath)
    {
        pathStartCheck(ref filePath);
        return File.ReadAllBytes(CACHE_LOCATION + filePath);
    }
    #endregion

    #region Mimicking Unity's cache structure
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">the path to save file, do not include filename</param>
    /// <param name="fileName">filename of file</param>
    /// <param name="data">the data to write</param>
    public void WriteByteCacheEntry(string path, string fileName, byte[] data)
    {
        pathStartCheck(ref path);
        string hashedFilename = StringHelper.GetMD5Hash(fileName);
        string folderName = CACHE_LOCATION + path + "/" + hashedFilename;

        string dataHash = StringHelper.GetMD5Hash(data);
        if (CacheEntryExist(path, fileName) == false) {
            // cache does not exist
            WriteCacheEntry(folderName, data, dataHash);
        } else {
            // cache exist, should check
            if (IsSameAsCacheEntry(path, fileName, dataHash) == false) {
                WriteCacheEntry(folderName, data, dataHash);
            } else {
                // don't do anything, same as cache 
            }
        }
    }
    void WriteCacheEntry(string folderName, byte[] data, string dataHash)
    {
        EnsureFolderExist(folderName);
        File.WriteAllBytes(folderName + "/data", data);
        File.WriteAllText(folderName + "/hash", dataHash);
    }
    void WriteCacheEntry(string folderName, string data, string dataHash)
    {
        EnsureFolderExist(folderName);
        File.WriteAllText(folderName + "/data", data);
        File.WriteAllText(folderName + "/hash", dataHash);
    }
    bool CacheEntryExist(string path, string fileName)
    {
        pathStartCheck(ref path);
        string hashedFilename = StringHelper.GetMD5Hash(fileName);
        string folderName = CACHE_LOCATION + path + "/" + hashedFilename;
        if (Directory.Exists(folderName) == false) {
            return false;
        } else {
            // cache folder exist
            //
            return true;
        }
    }
    public bool IsSameAsCacheEntry(string path, string fileName, string md5hash)
    {
        pathStartCheck(ref path);
        string hashedFilename = StringHelper.GetMD5Hash(fileName);
        string folderName = CACHE_LOCATION + path + "/" + hashedFilename;

        if (File.Exists(folderName + "/hash") == false) {
            // no hash exist
            return false;
        }

        string hash = File.ReadAllText(folderName + "/hash");
        return hash.Equals(md5hash);
    }
    public byte[] ReadByteCacheEntry(string path, string fileName)
    {
        pathStartCheck(ref path);
        if (CacheEntryExist(path, fileName) == false) {
            return null;
        }

        string hashedFilename = StringHelper.GetMD5Hash(fileName);
        string folderName = CACHE_LOCATION + path + "/" + hashedFilename;

        return File.ReadAllBytes(folderName + "/data");
    }
    #endregion

    public void EnsureFolderExist(string folderPath)
    {
        pathStartCheck(ref folderPath);
        if (System.IO.Directory.Exists(CACHE_LOCATION + folderPath) == false) {
            System.IO.Directory.CreateDirectory(CACHE_LOCATION + folderPath);
        }
    }
    public static void EnsureFolderExistStatic(string fullPath)
    {
        if (System.IO.Directory.Exists(fullPath) == false) {
            System.IO.Directory.CreateDirectory(fullPath);
        }
    }
    void pathStartCheck(ref string original)
    {
        if (original.Length > 0) {
            if (original[0] != '/' && original[0] != '\\') {
                original = "/" + original;
            }
        }
    }
}