using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
// for use in editor only
public class GenerateVideoPlaylistConfigs : MonoBehaviour {
    static bool GenerateForAWS = true;

    [SerializeField]
    string rootManifestName = "videoPlaylistManifest.json";

    [SerializeField, Tooltip("The folder where the videos are kept")]
    string folder = "VideoBuild";
    
    IEnumerator Start()
    {
        yield return null;
        GenerateFrom(System.Environment.CurrentDirectory + "\\" + folder, rootManifestName);
        Debug.Log("Video Playlist config files generation ended");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    /// <summary>
    /// Recursively search into folders until no child folder is found, that will be the playlist
    /// build all individual playlist, then finalize with a manifest
    /// </summary>
    /// <param name="folderPath"></param>
    public static void GenerateFrom(string folderPath, string manifestFileName)
    {
        //folderPath = folderPath.Replace('\\', '/');
        List<string> playlists = new List<string>();
        RecursiveFolderSearch(folderPath, playlists);

        string playlistFilename = "";
        string playlistPathName = "";
        string subPath = "";
        string[] filenames = null;

        VideoPlaylistManifest manifest = new VideoPlaylistManifest();
        // by now folders should be generated;
        for (int i = 0; i < playlists.Count; ++i) {

            filenames = Directory.GetFiles(playlists[i]);
            subPath = playlists[i].Replace(folderPath + '\\', string.Empty);
            subPath = WWW.UnEscapeURL(subPath);

            playlistFilename = subPath.Replace("\\", string.Empty).Replace(" ", string.Empty);
            playlistPathName = PathName(subPath); 

            // populate
            VideoPlaylist list = VideoPlaylist.Make(playlistFilename, playlistPathName);
            for (int j = 0; j < filenames.Length; ++j) {
                list.m_list.Add(NameHashPair.Make(BuildTrackname(filenames[j]), GetFileHash(filenames[j])));
            }

            System.Text.StringBuilder buildPlaylistFilename = new System.Text.StringBuilder();
            buildPlaylistFilename.Append(playlistFilename);
            buildPlaylistFilename.Append(".json");

            manifest.m_paths.Add(buildPlaylistFilename.ToString());

            File.WriteAllText(folderPath + '\\' + buildPlaylistFilename.ToString(), list.Serialize().ToString());
        }
        // save manifestfile
        File.WriteAllText(folderPath + "/" + manifestFileName, manifest.Serialize().ToString());
    }
    /// <summary>
    /// build the trackname to be saved into the file
    /// changed to that it can be used for AWS S3 link
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static string BuildTrackname(string path)
    {
        //Debug.LogFormat("TrackName: {0}", path);
        int lastSlash = path.LastIndexOf('\\');
        string result = path.Substring(lastSlash+1, path.Length-(lastSlash+1));
        result = NameHashPair.SwapForSpecial(result);
        Debug.LogFormat("TrackNameResult: {0}", result);
        return result;
    }

    static string PathName(string original)
    {
        return NameHashPair.SwapForSpecial(original.Replace('\\', '/'));
    }

    /// <summary>
    /// Get MD5 hash of a file
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>MD5 hash string of the file</returns>
    static string GetFileHash(string filePath)
    {
        byte[] data = File.ReadAllBytes(filePath);
        string hash = StringHelper.GetMD5Hash(data);
        return hash;
    }


    static void RecursiveFolderSearch(string subPath, List<string> playlists)
    {
        if (Directory.Exists(subPath)) {
            string[] dir = Directory.GetDirectories(subPath);
            if (dir.Length == 0) {
                // no more sub dir
                // base case
                playlists.Add(subPath);
                //Debug.Log("no more sub directories: " + subPath);
            } else {
                for(int i = 0; i < dir.Length; ++i) {
                    RecursiveFolderSearch(dir[i], playlists);
                }
            }
        }else {
            // base case
            // do nothing
            Debug.Log("directory dun exist: "+subPath);
        }
    }
}
#endif