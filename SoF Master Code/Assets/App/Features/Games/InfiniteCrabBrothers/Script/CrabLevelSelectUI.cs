using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.UI;

public class CrabLevelSelectUI : BaseUI
{
    List<GameObject> unearned_Stars = new List<GameObject>();
    public static CrabLevelSelectUI Current { get; private set; }
    public static void Open()
    {
        if (Current != null)
        {
            Current.Close();
        }

        //GameObject go = CachedResources.Spawn("ui/Crab Level Select", false);
        CachedResources.Spawn(Constants.CRAB_BROS_SHARED, "Crab Level Select", (result) => {
            GameObject go = result;
            go.transform.SetParent(UIRoot.transform);
            Current = go.GetComponent<CrabLevelSelectUI>();
            Debug.Log("loaded");
        });
    }

    private int displayLevels = -1;
    private int displayCurrentStars = -1;
    private int currentPage = 0, displayPage = -1;

    private GameSys.GameState mGameState;

    private List<int> displayPoints = new List<int>();
    private List<int> displayStarEarns = new List<int>();

    protected override void Start()
    {
        AudioSystem.PlayBGM("bgm_icb_circus-sneak-peak");
        base.Start();

        mGameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();

        SetupUI();

        canvasGroup.alpha = 0f;

        uiManager["Back Button"].OnClick(() => {
            Close();
            DOTween.Sequence()
                .AppendInterval(1f)
                .AppendCallback(() => MainNavigationController.GotoMainMenu())
                .SetUpdate(true);
        });

        uiManager["Critter Level Image"].OnClick(() => {
            //SceneManager.UnloadScene("Tsum");
            SceneManager.LoadScene("Level Complete");
            Close();
            /*DOTween.Sequence()
                .AppendInterval(1f)
                .AppendCallback(() => SceneManager.LoadScene("Level Complete"));
               */
        });
    }


    private void SetupUI()
    {
        MultiLanguageApplicator textProc = new MultiLanguageApplicator(gameObject);
        textProc.ApplyText("Stage Container 1/Locked Text"      , "crab_level_select_locked");
        textProc.ApplyText("Stage Container 1/New Text"         , "crab_level_select_new");
        textProc.ApplyText("Stage Container 1/Cleared Text"     , "crab_level_select_cleared");
        textProc.ApplyText("Stage Container 1/Cleared/LevelText", "crab_level_select_level_text");

        textProc.ApplyText("Stage Container 2/Locked Text"      , "crab_level_select_locked");
        textProc.ApplyText("Stage Container 2/New Text"         , "crab_level_select_new");
        textProc.ApplyText("Stage Container 2/Cleared Text"     , "crab_level_select_cleared");
        textProc.ApplyText("Stage Container 2/Cleared/LevelText", "crab_level_select_level_text");

        textProc.ApplyText("Level Selection Text", "game_stage_select_level_select");
    }
    bool hasFinished = false;
    private void Update()
    {
        if (!hasFinished)
        {
            hasFinished = true;

            // Spawn the stage
            if (displayLevels == -1)
            {
                displayLevels = mGameState.levelCount;
            }
            else if (displayLevels != mGameState.levelCount)
            {
                displayLevels = GameState.infiniteProg.levelCount;
                uiManager["Vert Layout"].Clear();
                currentPage = 0;
            }

            var totalPage = (mGameState.levelCount - 1) / 10;
            var pageBookmarks = new float[totalPage + 1];

            var currentStars = 0;
            for (var i = 0; i < mGameState.levelCount; ++i)
            {
                var currentLevel = GameState.infiniteProg.GetLevel(mGameState.startLevel + i - 1);

                currentStars += currentLevel.starEarned;

                var currentIndex = mGameState.startLevel + i;
                var page = i / 10;
                var column = i % 5 + page * 5;
                var row = i % 10 < 5 ? 0 : 1;

                while (row >= uiManager["Vert Layout"].transform.childCount)
                {
                    var go = uiManager.CreateFromTemplate("Hor Layout", uiManager["Vert Layout"]);
                    go.tag = "Untagged";
                }
                var rowGo = uiManager["Vert Layout"].transform.GetChild(row).gameObject;

                while (column >= rowGo.transform.childCount)
                {
                    if (i % 2 == 0)
                    {
                        var go = uiManager.CreateFromTemplate("Stage Container 1", rowGo);
                        go.tag = "Untagged";
                    }
                    else
                    {
                        var go = uiManager.CreateFromTemplate("Stage Container 2", rowGo);
                        go.tag = "Untagged";
                    }
                }
                var stageGo = rowGo.transform.GetChild(column).gameObject;

                var stageUI = new UIManager(stageGo);

                var stageText = (currentIndex).ToString();
                stageUI["Locked Stage Number Text"].SetText(stageText);
                stageUI["New Stage Number Text"].SetText(stageText);
                stageUI["Cleared Stage Number Text"].SetText(stageText);

                stageUI["Locked Stage Number Text"].transform.parent.Find("LevelText").GetComponent<Text>().text = MultiLanguage.getInstance().getString("game_stage_select_new_stage");
                stageUI["New Stage Number Text"].transform.parent.Find("LevelText").GetComponent<Text>().text = MultiLanguage.getInstance().getString("game_stage_select_new_stage");

                while (i >= displayPoints.Count)
                {      
                    displayPoints.Add(-1);
                }
                if (displayPoints[i] == -1)
                {
                    displayPoints[i] = currentLevel.pointsEarned;
                    stageUI["Points"].SetText(string.Format("{0:n0}", currentLevel.pointsEarned));
                }
                else if (displayPoints[i] != currentLevel.pointsEarned)
                {
                    var temp = displayPoints[i];
                    displayPoints[i] = currentLevel.pointsEarned;
                    DOTween.To(() => temp, x => temp = x, displayPoints[i], 1f).SetUpdate(true).OnUpdate(() => stageUI["Points"].SetText(string.Format("{0:n0}", temp)));
                }

                if (currentLevel.status == InfiniteBrosProgression.ELevelStatus.Locked)
                {
                    stageUI["Locked"].SetActive(true);
                    stageUI["New"].SetActive(false);
                    stageUI["Cleared"].SetActive(false);


                }
                else if (currentLevel.status == InfiniteBrosProgression.ELevelStatus.Available)
                {
                    stageUI["Locked"].SetActive(false);
                    if (!stageUI["New"].activeSelf)
                    {
                        stageUI["New"].SetActive(true);
                        stageUI["New"].transform.DOScale(Vector3.one * 1.1f, 0.2f).SetLoops(2, LoopType.Yoyo);
                    }
                    stageUI["Cleared"].SetActive(false);

                    stageUI["New"].OnClick(() => SelectLevel(currentIndex - 1));
                }
                else
                {
                    stageUI["Locked"].SetActive(false);
                    stageUI["New"].SetActive(false);
                    if (!stageUI["Cleared"].activeSelf)
                    {
                        stageUI["Cleared"].SetActive(true);
                        stageUI["Cleared"].transform.DOScale(Vector3.one * 1.1f, 0.2f).SetLoops(2, LoopType.Yoyo);
                    }

                    stageUI["Cleared"].OnClick(() => SelectLevel(currentIndex - 1));

                    while (i >= displayStarEarns.Count)
                    {
                        displayStarEarns.Add(-1);
                    }
                    if (displayStarEarns[i] == -1)
                    {
                        displayStarEarns[i] = currentLevel.starEarned;
                        for (var o = 0; o < stageUI["Stars"].transform.childCount; ++o)
                        {
                            stageUI["Stars"].transform.GetChild(o).gameObject.SetActive(o < currentLevel.starEarned);
                            if (!stageUI["Stars"].transform.GetChild(o).gameObject.GetActive())
                                unearned_Stars.Add(stageUI["Stars"].transform.GetChild(o).gameObject);
                        }
                    }
                    if (displayStarEarns[i] != currentLevel.starEarned)
                    {
                        displayStarEarns[i] = currentLevel.starEarned;
                        for (var o = 0; o < stageUI["Stars"].transform.childCount; ++o)
                        {
                            stageUI["Stars"].transform.GetChild(o).gameObject.SetActive(false);
                        }
                        Sequence starSequence = DOTween.Sequence().SetUpdate(true);
                        for (var o = 0; o < currentLevel.starEarned; ++o)
                        {
                            var star = stageUI["Stars"].transform.GetChild(o).gameObject;
                            starSequence.Append(
                                DOTween.Sequence()
                                    .AppendCallback(() => star.SetActive(true))
                                    .Append(star.transform.DOScale(Vector3.one * 1.1f, 0.3f).SetLoops(2, LoopType.Yoyo))
                                    .Join(star.transform.DOShakePosition(0.2f))
                                    .Join(star.transform.DORotate(new Vector3(0, 0, -180), 0.3f).SetLoops(2, LoopType.Yoyo))
                                    .AppendInterval(0.2f)
                            );
                        }
                    }
                    foreach (GameObject star in unearned_Stars)
                    {
                        star.SetActive(true);
                        star.GetComponent<Image>().color = new Color(50.0f / 255.0f, 50.0f / 255.0f, 50.0f / 255.0f);
                    }
                }

                if (i % 10 == 0)
                {
                    pageBookmarks[page] = stageGo.transform.localPosition.x;
                }

            }

            if (displayPage == -1)
            {
                displayPage = currentPage = 0;
            }
            else if (displayPage != currentPage)
            {
                displayPage = currentPage;
                uiManager["Vert Layout"].transform.DOLocalMoveX(-pageBookmarks[displayPage], 1f).SetUpdate(true);
            }

            // Set current and total stars
            if (displayCurrentStars == -1)
            {
                displayCurrentStars = currentStars;
                uiManager["Current Star Level"].SetText(displayCurrentStars.ToString());
            }
            else if (displayCurrentStars != currentStars)
            {
                var temp = displayCurrentStars;
                displayCurrentStars = currentStars;
                DOTween.To(() => temp, x => temp = x, displayCurrentStars, 1f).SetUpdate(true).OnUpdate(() => uiManager["Current Star Level"].SetText(temp.ToString()));
            }

            uiManager["Max Star Level"].SetText((mGameState.levelCount * 3).ToString());

            uiManager["Left Arrow"].SetActive(currentPage != 0);
            uiManager["Left Arrow"].OnClick(() => { --currentPage; hasFinished = false; });

            uiManager["Right Arrow"].SetActive(currentPage < totalPage);
            uiManager["Right Arrow"].OnClick(() => { ++currentPage; hasFinished = false; });
            int starTotal = 0;
            for (int i = 0; i < mGameState.levelCount; i++)
            {
                if (GameState.infiniteProg.GetLevel(mGameState.startLevel + i - 1).status == InfiniteBrosProgression.ELevelStatus.Finished)
                    starTotal += GameState.infiniteProg.GetLevel(mGameState.startLevel + i - 1).starEarned;
            }
            uiManager["Current Star Level"].SetText(starTotal.ToString());
        }
    }

    public void SelectLevel(int levelId)
    {
        GameState.infiniteProg.selectedLevel = levelId;
        PlayerPrefs.SetInt("CrabbyLevel", levelId);
        //SceneManager.LoadScene("inifinite_crab_brothers_game");
        JULESTech.Resources.AssetBundleManager.LoadSceneAsync(Constants.CRAB_BROS_SCENES, "inifinite_crab_brothers_game", false);
    }

    protected override Sequence TweenIn
    {
        get
        {
            return DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, 1f));
        }
    }

    protected override Sequence TweenOut
    {
        get
        {
            return DOTween.Sequence()
                .Append(canvasGroup.DOFade(0f, 1f));
        }
    }

}
