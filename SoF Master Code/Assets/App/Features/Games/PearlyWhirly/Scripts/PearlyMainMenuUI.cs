using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PearlyMainMenuUI : BaseUI 
{
    
    public static PearlyMainMenuUI Current { get; private set; }
    public static bool openui=false;

    private void Awake()
    {
        PearlyLevelDesignHolder.LoadDesigns();
    }

    public static void Open()
    {
        if (Current != null)
        {
            Current.Close();
        }
        //GameObject go = CachedResources.Spawn("ui/Pearly Main Menu Screen", false);
        CachedResources.Spawn(Constants.PEARLY_SHARED, "Pearly Main Menu Screen", (loadedAsset) => {
            GameObject go = GameObject.Instantiate(loadedAsset as GameObject, UIRoot.transform);
            Current = go.GetComponent<PearlyMainMenuUI>();
        });
    }

    protected override void Start()
    {
     if(PearlyMainMenuUI.openui==true)
        {
            PearlyLevelSelectUI.Open();
            openui = false;

        }
		AudioSystem.PlayBGM("bgm_pw_what-goes-up");
		base.Start();
        canvasGroup.alpha = 0f;

        uiManager["Back Button"].OnClick(() => {
            Close();
            DOTween.Sequence()
                .AppendInterval(2f)
                .AppendCallback(() => MainNavigationController.GotoMainMenu())
                .SetUpdate(true);
        });

        uiManager["Play Button"].OnClick(() => {
            Close();
            DOTween.Sequence()
                .AppendInterval(1f)
                .AppendCallback(() => PearlyLevelSelectUI.Open())
                .SetUpdate(true);
        });

        var currentStars = 0;
        
        GameSys.GameState gameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();

        //for (var i = gameState.startLevel; i <= gameState.endLevel; ++i)
        //{
        //    var currentLevel = GameState.pearlyProg.GetLevel(i);
        //    currentStars += currentLevel.starEarned;
        //}
        for (var i = 0; i < gameState.levelCount; ++i)
        {
            var currentLevel = GameState.pearlyProg.GetLevel(gameState.startLevel + i - 1);
            currentStars += currentLevel.starEarned;
        }

        uiManager["Current Star Level"].SetText(currentStars.ToString());
        uiManager["Max Star Level"].SetText((gameState.levelCount * 3).ToString());
        
        // Screen animations
        uiManager["Ray Left"].GetComponent<Image>().DOFade(0.5f, 1.6f).SetLoops(-1, LoopType.Yoyo);
        uiManager["Ray Mid"].GetComponent<Image>().DOFade(0.6f, 1.1f).SetLoops(-1, LoopType.Yoyo).SetDelay(1.1f);
        uiManager["Ray Right"].GetComponent<Image>().DOFade(0.6f, 1.8f).SetLoops(-1, LoopType.Yoyo).SetDelay(2.2f);

        SetupUI();

        // UI animations
        //uiManager["Game Title"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Play Button"].transform.DOLocalMoveY (-1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Info Button"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Back Button"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
    }

    public void SetupUI()
    {
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Game Title").GetComponent<Image>(), "pearly_main_menu_title");
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Play Button").GetComponent<Image>(), "gui_go_button");
    }

    protected override Sequence TweenIn
    {
        get
        {
            return DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, 1f))
                .Join(GameObject.Find("AnimatedGrass").GetComponent<MeshRenderer>().material.DOColor(Color.white, 1f));
        }
    }

    protected override Sequence TweenOut
    {
        get
        {

            return DOTween.Sequence()
                .Append(canvasGroup.DOFade(0f, 1f))
                .Join(GameObject.Find("AnimatedGrass").GetComponent<MeshRenderer>().material.DOColor(Color.black, 1f));
        }
    }


}
