using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuUI : BaseUI {

    public static MainMenuUI Current { get; private set; }
    public static bool opentsumui = false;

    private void Awake()
    {
        tumbleTroubleDesign.Load();
    }

    public static void Open () {
        if (Current != null) {
            Current.Close ();
        }
        //GameObject go = CachedResources.Spawn ("ui/Main Menu Screen", false);
        JULESTech.Resources.AssetBundleManager.LoadAsset(Constants.TUMBLE_TROUBLE_SHARED, "Main Menu Screen", (loadedAsset)=>{
            GameObject go = loadedAsset as GameObject;
            go.transform.SetParent(UIRoot.transform);
            Current = go.GetComponent<MainMenuUI>();
        });
    }

    protected override void Start ()
	{
        if (MainMenuUI.opentsumui == true)
        {
            LevelSelectUI.Open();
            opentsumui = false;

        }
        AudioSystem.PlayBGM("bgm_chasing-my-tail_M1YjuyHO");
		base.Start ();
        canvasGroup.alpha = 0f;
        var currentStars = 0;

        GameSys.GameState gameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();
        
        for (var i = 0; i < gameState.levelCount; ++i) {
            var currentLevel = GameState.tsumProg.GetLevel (gameState.startLevel + i - 1);
            currentStars += currentLevel.starEarned;
        }
        uiManager["Current Star Level"].SetText (currentStars.ToString ());
        uiManager["Max Star Level"].SetText ((gameState.levelCount * 3).ToString ());


        // Screen animations
        uiManager["Ray Left"].GetComponent<Image> ().DOFade (0.5f, 1.6f).SetLoops (-1, LoopType.Yoyo);
        uiManager["Ray Mid"].GetComponent<Image> ().DOFade (0.6f, 1.1f).SetLoops (-1, LoopType.Yoyo).SetDelay (1.1f);
        uiManager["Ray Right"].GetComponent<Image> ().DOFade (0.6f, 1.8f).SetLoops (-1, LoopType.Yoyo).SetDelay (2.2f);

        SetupUI();

        // UI animations
        //uiManager["Game Title"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Play Button"].transform.DOLocalMoveY (-1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Info Button"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Back Button"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
    }

    public void SetupUI()
    {
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Game Title").GetComponent<Image>(), "tsum_tsum_main_menu_title");
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Play Button").GetComponent<Image>(), "gui_go_button");
    }

    public void PlayButtonPressed()
    {
        Close();
        DOTween.Sequence()
            .AppendInterval(1f)
            .AppendCallback(() => {
                TsumGame.GoToTsumTsumLevelSelect();
                }) //LevelSelectUI.Open())

            .SetUpdate(true);
    }
    public void BackButtonPressed()
    {
        Close();
        DOTween.Sequence()
            .AppendInterval(2f)
            .AppendCallback(() => MainNavigationController.GotoMainMenu())
            .SetUpdate(true);
    }
    protected override Sequence TweenIn {
        get {
            return DOTween.Sequence ()
				.Append (canvasGroup.DOFade (1f, 1f));
                //.Join(GameObject.Find("AnimatedGrass").GetComponent<MeshRenderer>().material.DOColor(Color.white, 1f));
        }
    }

    protected override Sequence TweenOut {
        get {
            
            return DOTween.Sequence()
				.Append(canvasGroup.DOFade(0f, 1f));
                //.Join(GameObject.Find("AnimatedGrass").GetComponent<MeshRenderer>().material.DOColor(Color.black, 1f));
        }
    }


}
