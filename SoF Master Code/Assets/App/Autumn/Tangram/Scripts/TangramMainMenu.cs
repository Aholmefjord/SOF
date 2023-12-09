using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TangramMainMenu : BaseUI {

    public static TangramMainMenu Current { get; private set; }

    private void Awake()
    {
        mantaMatchDesign.Load();
    }

    public static void Open () {
        if (Current != null) {
            Current.Close ();
        }
        //GameObject go = CachedResources.Spawn ("ui/Tangram MainMenu", false);
        GameObject go = null;
        CachedResources.Spawn(Constants.MANTA_MATCH_SHARED, "Stage Clear UI", (loadedAsset) => {
            go = loadedAsset;
            go.transform.SetParent(UIRoot.transform);
            Current = go.GetComponent<TangramMainMenu>();
        });
    }

    protected override void Start () {
		AudioSystem.PlayBGM("bgm_m3_built-with-springs");
		base.Start ();
        canvasGroup.alpha = 0f;

        uiManager["Back Button"].OnClick (() => {
            Close ();
            DOTween.Sequence()
                .AppendInterval(2f)
                .AppendCallback(() => MainNavigationController.GotoMainMenu())
                .SetUpdate (true);
        });

        uiManager["Play Button"].OnClick (() => {
            Close ();
            DOTween.Sequence ()
                .AppendInterval (1f)
                .AppendCallback (() => TangramLevelSelectUI.Open ())
                .SetUpdate (true);
        });

        GameSys.GameState gameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();

        var currentStars = 0;
        for (var i = 0; i < gameState.levelCount; ++i) {
            var currentLevel = GameState.tangramProg.GetLevel (gameState.startLevel + i - 1);
            currentStars += currentLevel.starEarned;
        }
        uiManager["Current Star Level"].SetText (currentStars.ToString ());
        uiManager["Max Star Level"].SetText ((gameState.levelCount * 3).ToString ());


        // Screen animations
        uiManager["Ray Left"].GetComponent<Image> ().DOFade (0.5f, 1.6f).SetLoops (-1, LoopType.Yoyo);
        uiManager["Ray Mid"].GetComponent<Image> ().DOFade (0.6f, 1.1f).SetLoops (-1, LoopType.Yoyo).SetDelay (1.1f);
        uiManager["Ray Right"].GetComponent<Image> ().DOFade (0.6f, 1.8f).SetLoops (-1, LoopType.Yoyo).SetDelay (2.2f);

        // UI animations
        //uiManager["Game Title"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Play Button"].transform.DOLocalMoveY (-1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Info Button"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Back Button"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);

        SetupUI();
    }

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(gameObject.FindChild("Play Text").FindChild("Text"), "manta_main_menu_go_button");
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Game Title").GetComponent<Image>(), "manta_main_menu_title");
    }

    protected override Sequence TweenIn {
        get {
            return DOTween.Sequence ()
                .Append (canvasGroup.DOFade (1f, 1f));
        }
    }

    protected override Sequence TweenOut {
        get {

            return DOTween.Sequence ()
                .Append (canvasGroup.DOFade (0f, 1f));
        }
    }


}
