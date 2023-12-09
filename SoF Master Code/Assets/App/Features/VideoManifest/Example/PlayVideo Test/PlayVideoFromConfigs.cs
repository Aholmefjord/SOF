using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

#if UNITY_EDITOR
/// <summary>
/// example only, avoid being compiled
/// </summary>
public class PlayVideoFromConfigs : MonoBehaviour {
    public string serverURL = "http://localhost:4444";
    public string playlistName = "Doodleen";

    QuickComponentAccess<VideoPlayer> player;
    VideoPlaylistManifest vpl;

    // Use this for initialization
    void Start () {
        player = new QuickComponentAccess<VideoPlayer>(gameObject);
    }

    public void Play(int index)
    {
        string cacheLocation = VideoPlaylistConstants.Cache.CACHE_LOCATION;

        VideoPlaylist playlist = VideoPlaylistManager.GetPlaylist(playlistName);

        player.Value.url = playlist.GetVideoPath(cacheLocation, index);
        player.Value.Play();
    }

    private void OnDisable()
    {
        if (player.Value.renderMode == VideoRenderMode.RenderTexture)
            player.Value.targetTexture.DiscardContents();
        if (player.Value.renderMode == VideoRenderMode.CameraFarPlane || player.Value.renderMode == VideoRenderMode.CameraNearPlane)
            player.Value.targetCamera.targetTexture.DiscardContents();
    }
}
#endif