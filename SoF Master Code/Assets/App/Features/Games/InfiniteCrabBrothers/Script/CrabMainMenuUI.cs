using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CrabMainMenuUI : BaseUI
{

    public static CrabMainMenuUI Current { get; private set; }
    public static void Open()
    {
        if (Current != null)
        {
            Current.Close();
        }
        //GameObject go = CachedResources.Spawn("ui/Crab Main Menu Screen", false);
        CachedResources.Spawn(Constants.CRAB_BROS_SHARED, "Main Menu Screen Crab", (result) => {
            GameObject go = result;
            go.transform.SetParent(UIRoot.transform);
            Current = go.GetComponent<CrabMainMenuUI>();
        });
    }

    protected override void Start()
    {
		AudioSystem.PlayBGM("bgm_icb_circus-sneak-peak");
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
                .AppendCallback(() => CrabLevelSelectUI.Open())
                .SetUpdate(true);
        });

        GameSys.GameState gameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();

        int starTotal = 0;
        //for (int i = 0; i < gameState.levelCount; i++)
        //{
        //    if (GameState.infiniteProg.GetLevel(gameState.startLevel + i).status == InfiniteBrosProgression.ELevelStatus.Finished)
        //        starTotal += GameState.infiniteProg.GetLevel(gameState.startLevel + i).starEarned;
        //}
        for (var i = 0; i < gameState.levelCount; ++i)
        {
            var currentLevel = GameState.infiniteProg.GetLevel(gameState.startLevel + i - 1);
            starTotal += currentLevel.starEarned;
        }
        uiManager["Current Star Level"].SetText(starTotal.ToString());
        uiManager["Max Star Level"].SetText((gameState.levelCount * 3).ToString());

        SetupUI();
        // Screen animations


        // UI animations
        //uiManager["Game Title"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Play Button"].transform.DOLocalMoveY (-1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Info Button"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);
        //uiManager["Back Button"].transform.DOLocalMoveY (1500f, 1f).From ().SetDelay (0.5f);

        // Tom: preload crab assets
        StartCoroutine(CrabResources.InitResourcesAsync());
    }

    public void SetupUI()
    {
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Game Title").GetComponent<Image>(), "crabby_main_menu_title");
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Play Button").GetComponent<Image>(), "crabby_main_menu_start");
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
