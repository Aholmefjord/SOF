using UnityEngine;
using System.Collections;

public class AudioSystem : MonoBehaviour {
    private static AudioSystem _instance;
	static AudioSource current_bgm_source;
	static GameObject current_bgm_go;
	static string current_bgm_key;

	static AudioSource current_sfx_loop_source;
	static GameObject current_sfx_loop_go;

	public static float bgm_volume = 1.0f;
	public static float sfx_volume = 1.0f;
	public const float DEFAULT_BGM_VOLUME = 0.3f;
	public const float DEFAULT_SFX_VOLUME = 1;

	void Awake()
	{
		if (_instance == null)
		{
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
		{
			//If a Singleton already exists and you find
			//another reference in scene, destroy it!
			if (this != _instance)
				Destroy(this.gameObject);
		}
		bgm_volume = PlayerPrefs.GetFloat("bgm_volume", DEFAULT_BGM_VOLUME);
		sfx_volume = PlayerPrefs.GetFloat("sfx_volume", DEFAULT_SFX_VOLUME);
	}

	public static void InstanceCheck()
	{
		if (!_instance)
		{
			//Create from resources if not already created
			GameObject obj = new GameObject("AudioSystem");
			_instance = obj.AddComponent<AudioSystem>();
            obj.AddComponent<AudioListener>();
		}
	}

    #region Volume Manipulators
    public static void set_sfx_volume(float new_volume)
	{
		if (new_volume > 1)
			new_volume = 1;
		if (new_volume < 0)
			new_volume = 0;

		PlayerPrefs.SetFloat("sfx_volume", new_volume);
		sfx_volume = new_volume;
	}

	public static void set_bgm_volume(float new_volume)
	{
		if (new_volume > 1)
			new_volume = 1;
		if (new_volume < 0)
			new_volume = 0;

		PlayerPrefs.SetFloat("bgm_volume", new_volume);
		bgm_volume = new_volume;

		if (current_bgm_source != null)
		{
			current_bgm_source.volume = new_volume;
		}

		//Debug.Log ("BGM Volume Changed: " + bgm_volume);
	}

    public static float get_bgm_volume()
    {
        return bgm_volume;
    }
    #endregion

    #region SFX playback
    public static void PlaySFX(string sfx, bool loop = false, float volume = 1.0f)
	{
		InstanceCheck();
        _instance.play_sfx(sfx, loop, volume);
    }

	void play_sfx(string sfx, bool loop = false, float volume = 1.0f)
	{
		if (string.IsNullOrEmpty(sfx))
		{
			Debug.Log("Warning: Tried to load empty sfx file");
			return;
		}

		if (sfx_volume <= 0)
		{
#if UNITY_EDITOR
			Debug.Log("sfx_volume: 0");
#endif
			return;
		}

#if UNITY_EDITOR
		//AudioClip clip = Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/ResourcesBundle/SFX/" + sfx + ".wav", typeof(AudioClip))) as AudioClip;
		//PlaySoundClip (clip, loop);
		StartCoroutine(PlaySFXAssetBundle("audio/sfx/" + sfx, loop, volume));
#else
		StartCoroutine(PlaySFXAssetBundle("audio/sfx/" + sfx, loop));
#endif
    }

    public static void stop_sfx_loop()
    {
        if (current_sfx_loop_go != null) {
            Destroy(current_sfx_loop_go);
        }
    }

    public static IEnumerator PlaySFXAssetBundle(string key, bool loop = false, float volume = 1.0f)
    {
        // TOM: this is to not affect the other systems that is using this system before converting to assetbundles
        // TOM: extract the filename only
        StringHelper.TrimFilename(ref key);

        UnityEngine.Object loadedAsset = null;
        yield return JULESTech.Resources.AssetBundleManager.LoadAsset(Constants.SFX_SHARED_BUNDLE, key, (result) => { loadedAsset = result; });
        
        AudioClip clip = loadedAsset as AudioClip;
        //Debug.Log("<color='00FF00'>[PLAY SFX]</color>: " + clip);

        if (clip == null) {
            Debug.LogWarning("Missing SFX: " + key);
            yield break;
        }

        PlaySoundClip(clip, loop, volume);

        yield break;
    }

    static void PlaySoundClip(AudioClip clip, bool loop = false, float volume = 1.0f)
    {
        if (!loop) {
            GameObject go = new GameObject(clip.name);
            AudioSource source = go.AddComponent<AudioSource>();
            go.transform.SetParent(_instance.transform);

            source.clip = clip;
            source.volume = AudioSystem.sfx_volume * volume;
            source.spatialBlend = 0;
            source.Play();
            Destroy(go, source.clip.length + 0.1f); //Add a buffer incase of lag, else sound is cuttoff
        } else {
            if (current_sfx_loop_go == null) {
                current_sfx_loop_go = new GameObject(clip.name);
                current_sfx_loop_source = current_sfx_loop_go.AddComponent<AudioSource>();
            } else {
                current_sfx_loop_source.Stop();
            }
            current_sfx_loop_source.clip = clip;
            current_sfx_loop_source.volume = AudioSystem.sfx_volume * volume;
            print(current_sfx_loop_source.volume);
            current_sfx_loop_source.spatialBlend = 0;
            current_sfx_loop_source.loop = true;
            current_sfx_loop_source.Play();
        }

    }
    #endregion

    #region BGM playback
    public static void PlayBGM(string bgm, bool loop = true)
	{
		InstanceCheck();
		_instance.play_bgm(bgm, loop);
    }

    void play_bgm(string bgm, bool loop = true)
    {
        if (bgm == null || bgm == "") {
            return;
        }

        if (bgm_volume <= 0) {
            //return;
        }

#if UNITY_EDITOR
        //AudioClip clip = Instantiate (UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/ResourcesBundle/SFX/" + sfx + ".wav", typeof(AudioClip))) as AudioClip;
        //PlaySoundClip (clip, loop);
        StartCoroutine(PlayBGMAssetBundle("audio/bgm/" + bgm, loop));
#else
		StartCoroutine (PlayBGMAssetBundle ("audio/bgm/" + bgm, loop));
#endif
    }

    public static void stop_bgm()
    {
        if (current_bgm_source != null) {
            current_bgm_source.Stop();
        }

        //set bgm to null, so we can play the next OR the same again.
        current_bgm_source = null;
        current_bgm_key = "";

    }

    public static void PauseBGM()
    {
        if (current_bgm_source != null)
        {
            current_bgm_source.Pause();
        }
        else
        {
            Debug.LogError("Audio System ERROR: Pause BGM that doesn't exist");
        }
    }

    public static void ResumeBGM()
    {
        if(current_bgm_source != null)
        {
            current_bgm_source.UnPause();
        }
        else
        {
            Debug.LogError("Audio System ERROR: Resume BGM that doesn't exist");
        }
    }

    static IEnumerator PlayBGMAssetBundle(string key, bool loop = true)
    {
        if (key == current_bgm_key) {
            yield break;
        }

        // TOM: this is to not affect the other systems that is using this system before converting to assetbundles
        // trims 
        string original = key;
        StringHelper.TrimFilename(ref key);
        /*
        var ops = AssetBundles.AssetBundleManager.LoadAssetAsync(BGMBundleName, key, typeof(AudioClip));
        if (ops == null) {
            Debug.LogError("[BGM] "+key + " cannot be loaded");
            yield break;
        }
        //*/
        
        UnityEngine.Object loadedAsset = null;
        yield return JULESTech.Resources.AssetBundleManager.LoadAsset(Constants.BGM_SHARED_BUNDLE, key, (result) => { loadedAsset = result; });
        
        AudioClip clip = loadedAsset as AudioClip;
        //Debug.Log("[PlayBGM]["+clip+"]");

        if (clip == null) {
            Debug.LogError(string.Format("[PlayBGMAssetBundle] {0} is not a valid file in {1}", key, Constants.BGM_SHARED_BUNDLE));
            yield break;
        }
        current_bgm_key = key;
        PlayBGMClip(clip, loop);

        //Resources.UnloadUnusedAssets();
    }

    static void PlayBGMClip(AudioClip clip, bool loop = true)
    {
        if (current_bgm_source == null) {
            current_bgm_go = new GameObject("Audio_BGM");
            current_bgm_source = current_bgm_go.AddComponent<AudioSource>();
            current_bgm_go.transform.SetParent(_instance.transform);
        } else {
            current_bgm_source.Stop();
        }

        current_bgm_source.clip = clip;
        current_bgm_source.volume = AudioSystem.bgm_volume;
        current_bgm_source.spatialBlend = 0;
        if (!loop) {
            current_bgm_source.loop = false;
            current_bgm_source.Play();
            Destroy(current_bgm_go, current_bgm_source.clip.length);
        } else {
            current_bgm_source.loop = true;
            current_bgm_source.Play();
            DontDestroyOnLoad(current_bgm_go);
        }

    }
    #endregion

    #region Cutscene BGM
    public static void PlayBGMCutscene(string bgm, bool loop = true)
    {
        InstanceCheck();
        _instance.play_bgm(bgm, loop);
    }
    #endregion

    public static void StartLoad(string url)
    {
        _instance.StarCoroutineLoad(url);
    }

    public void StarCoroutineLoad(string url)
    {
        StartCoroutine(LoadMusic(url));
    }
    
    private IEnumerator LoadMusic(string url)
    {
        WWW www = new WWW(url);

        yield return www;

        PlayBGMClip(www.GetAudioClip());
    }
}


