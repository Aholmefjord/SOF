using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LoadingScreenInstance : MonoBehaviour
{
	public GameObject sceneLoaderPanel;
	public Slider progressSlider;
	public Animator sceneAnimator;
	private AsyncOperation asyncOperation;
	private const int DELAY_BUFFER_SECONDS = 1;
	private bool playingSlideOut = false;
	public GameObject reloadPanel;
    public GameObject titlePanel;
    public bool isLoad = false;

    private string sceneName;

    private bool isWaitingForItemToLoad = false;
    private int totalWaitItemRequired;
    private int currentWaitItemRequired;

    void Awake()
	{
        AudioSystem.stop_bgm();
		AudioSystem.PlaySFX("buttons/JULES_LOADING_02");
        SetupUI();
    }

	public void Init(string scene, int totalItemToLoad = 0)
	{
        sceneName = scene;

        isWaitingForItemToLoad = totalItemToLoad > 0;
        totalWaitItemRequired = totalItemToLoad;
        currentWaitItemRequired = 0;

        if(!isWaitingForItemToLoad)
        { 
            StartCoroutine(StartLoader(scene));
            Invoke("DisplayReloadPanel", 30.0f);
        }

        DisableInteractableButton(); //pausing button appears on loading screen when exiting chomp chomp..

        SetupUI();
    }

    private void SetupUI()
    {
        MultiLanguage.getInstance().apply(titlePanel.FindChild("Title Text"), "loading_screen_title");
        MultiLanguage.getInstance().apply(reloadPanel.FindChild("ReloadCanvas").FindChild("Text"), "loading_screen_reload");
    }

    public void DisplayReloadPanel()
	{
		reloadPanel.SetActive(true);
	}

	void Update()
	{
		if (asyncOperation == null) 
		{
			return;
		}
		
		progressSlider.value = asyncOperation.progress;

		if (asyncOperation.progress >= 0.9f && !playingSlideOut)
		{
			playingSlideOut = true;
            sceneAnimator.Play("SlideOut");
			Invoke("HideSceneLoader", 0.4f);
        }
	}

    public void UpdateLoadingItem(int _count)
    {
        if (!isWaitingForItemToLoad)
            return;

        currentWaitItemRequired += _count;
        if (currentWaitItemRequired >= totalWaitItemRequired)
        {
            SceneManager.LoadScene(sceneName);
        }
        
    }
	public void LoadTimeOut()//Cause unity to fail loading for testing
	{
		Debug.LogError("go to main map");
		this.gameObject.SetActive(false);
		MainNavigationController.GoToMap();
		MainNavigationController.GoToMap();
		MainNavigationController.GoToMap();
		MainNavigationController.GoToMap();
	}

	public void Reload()
	{
		SceneManager.LoadScene(0);
	}

	IEnumerator StartLoader(string scene)
	{
		if (!isLoad)
		{
			isLoad = true;
			progressSlider.value = 0.08f; //base value for ui look
										  //yield return new WaitForSeconds(DELAY_BUFFER_SECONDS);
			yield return new WaitForSeconds(1.0f);
			Debug.Log(scene);
			SceneManager.LoadScene(scene);
            DG.Tweening.DOTween.CompleteAll();
	//		asyncOperation.allowSceneActivation = false;
//			yield return asyncOperation;
#if UNITY_EDITOR
			Debug.Log("Loading complete");
//            Destroy(this.gameObject);
#endif
		}
	}

    public IEnumerator LoadFromAssetBundle(string bundleName, string sceneName)
    {
        SetupUI();
        DG.Tweening.DOTween.CompleteAll();
        Invoke("DisplayReloadPanel", 30.0f);
        DisableInteractableButton(); //pausing button appears on loading screen when exiting chomp chomp..

        MultiLanguage.getInstance().apply(titlePanel.FindChild("Title Text"), "loading_screen_title");
        MultiLanguage.getInstance().apply(reloadPanel.FindChild("ReloadCanvas").FindChild("Text"), "loading_screen_reload");
        
        AsyncOperation ops = null;
        Coroutine coroutineInst = JULESTech.Resources.AssetBundleManager.LoadSceneAsync(bundleName, sceneName, false, true, (result)=> { ops = result; });

        yield return coroutineInst;
        Debug.Log("load scene finish");
    }

	public void HideSceneLoader()
	{
		asyncOperation.allowSceneActivation = true;
	}

	public void ShowLoaderProgress(float progress, bool changeTips = false)
	{
		if (!sceneLoaderPanel || !sceneLoaderPanel.activeInHierarchy)
		{
			sceneLoaderPanel.SetActive(true);
		}
	}
	public void DisableInteractableButton()
	{
		GameObject[] go = GameObject.FindGameObjectsWithTag("InteractableButton");
		if (go != null)
		{
			for (int i = 0; i < go.Length; i++)
			{
				go[i].gameObject.SetActive(false);
			}
		}
	}
}