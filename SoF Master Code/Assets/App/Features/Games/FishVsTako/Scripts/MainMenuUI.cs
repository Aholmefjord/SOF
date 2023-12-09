using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using DG.Tweening;

namespace FvTGame
{
    public class MainMenuUI : BaseUI
    {

        public static FvTLevelSelectUI Current { get; private set; }
        private static GameObject thisGO;
        public static void Open()
        {
            if (Current != null)
            {
                Current.Close();
            }
            BlankScreenUI.Open();
            thisGO = CachedResources.Spawn("ui/Tako Level Select", false);
            thisGO.transform.SetParent(UIRoot.transform);
            Current = thisGO.GetComponent<FvTLevelSelectUI>();
        }

        private int displayLevels = -1;
        private int displayCurrentStars = -1;
        private int currentPage = 0, displayPage = -1;

        private List<int> displayPoints = new List<int>();
        private List<int> displayStarEarns = new List<int>();

        private GameSys.GameState mGameState;

        protected override void Start()
        {
            base.Start();
            canvasGroup.alpha = 0f;

            uiManager["Back Button"].OnClick(() => {
                Close();
                DOTween.Sequence()
                    .AppendInterval(0.1f)
                    .AppendCallback(() => MainNavigationController.GotoMainMenu())
                    .SetUpdate(true);
            });

            uiManager["Play Button"].OnClick(() => {
                //SceneManager.UnloadScene("Tsum");
               // SceneManager.LoadScene("Level Complete");
                Close();
                //SceneManager.LoadScene("FvTLevelSelect");
                MainNavigationController.DoAssetBundleLoadLevel(Constants.FVT_SCENES, "FvTLevelSelect");
                //DOTween.Sequence()
                //.AppendInterval(1f)
                //.AppendCallback(() => TakoLevelSelectUI.Open())
                //.SetUpdate(true);
            });

            mGameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();

            SetupUI();
        }

        private void SetupUI()
        {
            /*
            MultiLanguage.getInstance().apply(thisGO.FindChild("Locked Stage Number Text"), "game_stage_select_locked_stage");
            //MultiLanguage.getInstance().apply(thisGO.FindChild("Locked Text"), "game_stage_select_locked");
            MultiLanguage.getInstance().apply(thisGO.FindChild("New Stage Number Text"), "game_stage_select_new_stage");
            MultiLanguage.getInstance().apply(thisGO.FindChild("New Text"), "game_stage_select_new");
            MultiLanguage.getInstance().apply(thisGO.FindChild("Cleared Stage Number Text"), "game_stage_select_cleared_stage");
            MultiLanguage.getInstance().apply(thisGO.FindChild("Cleared Text"), "game_stage_select_cleared");

            MultiLanguage.getInstance().apply(thisGO.FindChild("Level Selection Text"), "game_stage_select_level_select");
            //*/
        }
        bool hasFinished = false;
        /*
        private void Update()
        {

            // Spawn the stage
            if (displayLevels == -1)
            {
                displayLevels = mGameState.levelCount;
            }
            else if (displayLevels != mGameState.levelCount)
            {
                displayLevels = mGameState.levelCount;
                uiManager["Vert Layout"].Clear();
                currentPage = 0;
            }

            var totalPage = (mGameState.levelCount - 1) / 10;
            var pageBookmarks = new float[totalPage + 1];

            var currentStars = 0;
            if (!hasFinished)
            {
                hasFinished = true;
                for (var i = 0; i < mGameState.levelCount; ++i)
                {
                    var currentLevel = GameState.takoProg.GetLevel(mGameState.startLevel + i - 1);

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
                        var go = uiManager.CreateFromTemplate("Stage Container", rowGo);
                        go.tag = "Untagged";
                    }
                    var stageGo = rowGo.transform.GetChild(column).gameObject;

                    var stageUI = new UIManager(stageGo);

                    var stageText = MultiLanguage.getInstance().getString("game_stage_select_new_stage") + " " + currentIndex.ToString();
                    stageUI["Locked Stage Number Text"].SetText(stageText);
                    stageUI["New Stage Number Text"].SetText(stageText);
                    stageUI["Cleared Stage Number Text"].SetText(stageText);

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

                    if (currentLevel.status == TakoPlayerProgression.ELevelStatus.Locked)
                    {
                        stageUI["Locked"].SetActive(true);
                        stageUI["New"].SetActive(false);
                        stageUI["Cleared"].SetActive(false);


                    }
                    else if (currentLevel.status == TakoPlayerProgression.ELevelStatus.Available)
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
                        //intersting effects
                        if (displayStarEarns[i] == -1)
                        {
                            displayStarEarns[i] = currentLevel.starEarned;
                            for (var o = 0; o < stageUI["Stars"].transform.childCount; ++o)
                            {
                                stageUI["Stars"].transform.GetChild(o).gameObject.SetActive(o < currentLevel.starEarned);

                            }


                        }

                        for (var o = 0; o < stageUI["Stars"].transform.childCount; ++o)
                        {
                            stageUI["Pearls"].transform.GetChild(o).gameObject.SetActive(o < currentLevel.starEarned);

                        }


                        /// intersting effects
                        /// 
                        //Debug.Log(stageUI["Stars"].transform.childCount);


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
            }
        }
        //*/
        public void SelectLevel(int levelId)
        {
            //GameState.takoProg.selectedLevel = levelId;
            //SceneManager.LoadScene("Tako");
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
                    .Append(canvasGroup.DOFade(0f, 1f))
                    .AppendCallback(() => BlankScreenUI.Current.CloseImmediately());
            }
        }

    }
}