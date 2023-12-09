using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

using AutumnInteractive.SimpleJSON;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TangramGame : MonoBehaviour {
    // for spawning , replacing loading from Resources.Load(in CacheResources) to just normal asset reference
    static TangramGame _inst;

    private static JSONArray _allLevels;
    /*
    public static JSONArray ALL_LEVELS {
        get {
            if (_allLevels == null) {
                _allLevels = JSON.Parse<JSONArray>(CachedResources.Load<TextAsset>("json/tangram/all_levels").text);
            }
            return _allLevels;
        }
    }
    //*/
    public static JSONClass GetLevel(int levelId)
    {
        //return (JSONClass)ALL_LEVELS[levelId];
        return (JSONClass)_allLevels[levelId];
    }

    public bool DebugMode = false;
    public int PuzzleId = 0;

    private TangramGameLogic Logic;
    private UIManager uiManager;

    private bool needUpdate;
    private Vector3 selectedOffset;
    private GameObject selectedPieceGo;
    private TangramGameLogic.TangramPiece selectedPiece;
    private Color selectedColor;
    public int stage = 0;
    public int hearts = 3;
    GameScoreChart gcs;
    private bool playing = false;

    public GameObject pausePanel;

    private int TotalTime, TotalTimeLeft;
    private bool FirstClear = true;

    private CanvasGroup canvasGroup;

    private int stageWon = 0, combo = 0, highestCombo = 0;

    public Text levelText;

    public GameObject readyGoText;

    public Text ProgressText, StageText, StageTimerText;
    private float StageTimer_Red = 255.0f, StageTimer_Green = 255.0f, StageTimer_Blue = 255.0f;
    private bool StageTimerScale = false;
    private bool overlappingPiece = false;
    private int tempListCount = 0;

    public GameObject spriteGlowTemplate;

    [SerializeField]
    Sprite coinSprite;
    [SerializeField]
    List<Sprite> Fishes = new List<Sprite>();
    [SerializeField]
    List<Sprite> FuseFishes = new List<Sprite>();
    [SerializeField]
    GameObject coinExplosionEffectPrefab;
    [SerializeField]
    GameObject tangram_template;
    [SerializeField]
    List<Sprite> mantaParts = new List<Sprite>();
    Dictionary<string, Sprite> m_MantaParts = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (_inst == null) {
            _inst = this;
        }
    }

    IEnumerator Start()
    {
        gcs = gameObject.GetComponent<GameScoreChart>();
        uiManager = new UIManager(gameObject);
        uiManager["Bar Fill"].GetComponent<Slider>().value = 0;

        SetupUI();

        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.DOFade(1f, 1f).OnComplete(() => canvasGroup.interactable = true);

        // preload manta parts;
        for (int i = 0; i < mantaParts.Count; ++i) {
            m_MantaParts.Add(mantaParts[i].texture.name, mantaParts[i]);
        }

        mantaMatchDesign.Load();
        
        yield return JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.MANTA_MATCH_CONFIG, "all_levels", (allLevelsConfig) => {
            _allLevels = JSON.Parse<JSONArray>(allLevelsConfig.text);
        });

        yield return TangramGameLogic.TangramPiece.InitializePieceData();

        yield return JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.MANTA_MATCH_CONFIG, "puzzles", (puzzlesConfig) => {
            TangramGameLogic.allStages = JSON.Parse<JSONArray>(puzzlesConfig.text);
            ClearGame(true);

            DOTween.Sequence()
                .AppendInterval(1f)
                .AppendCallback(StartGame);
        });
    }

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(readyGoText, "manta_game_ready_go_text");

        MultiLanguage.getInstance().apply(pausePanel.FindChild("Paused Text"), "in_game_menu_title");
        MultiLanguage.getInstance().apply(pausePanel.FindChild("CurrentStage"), "in_game_menu_current_stage");

        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Game Title Image").GetComponent<Image>(), "manta_main_menu_title");
        if (pausePanel.FindChild("Game Title Image").GetComponent<Image>().sprite.name == "mantamatchmaniatitle_CN")
            pausePanel.FindChild("Game Title Image").GetComponent<Image>().SetNativeSize();
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Button Home").GetComponent<Image>(), "gui_settings_home");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Button Level Select").GetComponent<Image>(), "gui_settings_levelselect");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Button Resume").GetComponent<Image>(), "gui_settings_resume");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Button Restart").GetComponent<Image>(), "gui_settings_restart");
    }

    private void ClearGame(bool immediately = false)
    {
        playing = false;

        if (levelBg != null) Destroy(levelBg);

        if (immediately) {
            uiManager["Plate Container"].transform.localPosition += Vector3.up * Screen.height;
            StageText.transform.localPosition += Vector3.up * Screen.height;
            StageTimerText.transform.localPosition += Vector3.up * Screen.height;
            //uiManager["Fuse Container"].transform.localPosition += Vector3.up * Screen.height;
            //uiManager["ProgressBar Container"].transform.localPosition += Vector3.down * Screen.height;
            uiManager["Workstation Grid"].GetComponent<CanvasGroup>().alpha = 0;

            uiManager["GreenCarpet"].transform.localPosition += Vector3.right * Screen.width;
            //uiManager["Hearts Container"].transform.localPosition += Vector3.right * Screen.width;

            uiManager["Mouse Blocker"].GetComponent<CanvasGroup>().alpha = 1f;
        } else {
            uiManager["Plate Container"].transform.DOLocalMoveY(Screen.height, 0.2f).SetRelative();
            StageText.transform.DOLocalMoveY(Screen.height, 0.2f).SetRelative();
            StageTimerText.transform.DOLocalMoveY(Screen.height, 0.2f).SetRelative();
            //uiManager["Fuse Container"].transform.DOLocalMoveY (Screen.height, 0.2f).SetRelative ();
            //uiManager["ProgressBar Container"].transform.DOLocalMoveY (-Screen.height, 0.2f).SetRelative ();
            uiManager["Workstation Grid"].GetComponent<CanvasGroup>().DOFade(0.5f, 0.2f);

            uiManager["GreenCarpet"].transform.DOLocalMoveX(Screen.width, 0.2f).SetRelative();
            //uiManager["Hearts Container"].transform.DOLocalMoveX (Screen.width, 0.2f).SetRelative ();

            uiManager["Mouse Blocker"].GetComponent<CanvasGroup>().DOFade(1f, 0.2f);
        }
    }

    public void LevelSelectGameClick()
    {
        //MainNavigationController.GoToScene("Tangram Level Select");
        MainNavigationController.DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram Level Select");
        if (Time.timeScale != 1.0f)
            Time.timeScale = 1.0f;
    }

    public void ExitGameClick()
    {
        Debug.Log("Go to Home - Tangram");
        MainNavigationController.GotoMainMenu();
        //MainNavigationController.GoToHome();
        if (Time.timeScale != 1.0f)
            Time.timeScale = 1.0f;
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
    }

    public void PauseAnimationClick()
    {
        Time.timeScale = 0.0f;
        pausePanel.SetActive(true);
    }

    public void ResumeAnimationClick()
    {
        Time.timeScale = 1.0f;
        pausePanel.SetActive(false);
    }

    public void ReloadGameClick()
    {
        if (Time.timeScale != 1)
            Time.timeScale = 1.0f;

        //SceneManager.UnloadScene("Tangram");
        //SceneManager.LoadScene("Tangram");
        MainNavigationController.DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram");
    }

    private void StartGame()
    {
        gameEnded = false;
        Logic = new TangramGameLogic();

        if (DebugMode) {
            Logic.SetupPuzzle(PuzzleId);
        } else {
            Logic.SetupGame(GameState.tangramProg.selectedLevel, stage);
        }
        loseSFXPlayed = false;

        uiManager["Fish Head Curry"].SetColor(Color.white);
        uiManager["Fish Head Curry"].transform.localScale = Vector3.one;

        uiManager["Bar Line"].Clear();
        for (var i = 0; i < Logic.MaxStage; ++i) {
            var go = uiManager.CreateFromTemplate("shortLine", uiManager["Bar Line"]);
        }

        uiManager["Plate Container"].transform.DOLocalMoveY(-Screen.height, 0.5f).SetRelative().SetEase(Ease.OutBack);
        StageText.transform.transform.DOLocalMoveY(-Screen.height, 0.5f).SetRelative().SetEase(Ease.OutBack);
        StageTimerText.transform.DOLocalMoveY(-Screen.height, 0.5f).SetRelative().SetEase(Ease.OutBack);
        //uiManager["Fuse Container"].transform.DOLocalMoveY (-Screen.height, 0.5f).SetRelative ().SetEase (Ease.OutBack);
        //uiManager["ProgressBar Container"].transform.DOLocalMoveY (Screen.height, 0.5f).SetRelative ().SetEase (Ease.OutBack);
        uiManager["Workstation Grid"].GetComponent<CanvasGroup>().DOFade(0.5f, 0.5f);

        uiManager["GreenCarpet"].transform.DOLocalMoveX(-Screen.width, 0.5f).SetRelative().SetEase(Ease.OutBack);
        //uiManager["Hearts Container"].transform.DOLocalMoveX (-Screen.width, 0.5f).SetRelative ().SetEase (Ease.OutBack);

        uiManager["Mouse Blocker"].GetComponent<CanvasGroup>().DOFade(0f, 0.5f);
        needUpdate = true;
        alreadySelected = false;

        DOTween.Sequence()
            .AppendInterval(1f)
            .AppendCallback(() => {
                readyGoText.SetActive(true);
                readyGoText.transform.DOScale(1.0f, 0.5f).SetEase(Ease.OutSine);
                readyGoText.SetText(MultiLanguage.getInstance().getString("manta_game_ready_go_ready_text"));
            })
            .AppendInterval(1.0f)
            .AppendCallback(() => {
                readyGoText.transform.DOScale(0.25f, 0.1f);
                readyGoText.SetText(MultiLanguage.getInstance().getString("manta_game_ready_go_go_text"));
            })
            .AppendInterval(0.1f)
            .AppendCallback(() => {
                readyGoText.transform.DOScale(1.5f, 0.4f).SetEase(Ease.OutSine);
            })
            .AppendInterval(0.9f)
            .AppendCallback(() => {
                readyGoText.SetActive(false);
                playing = true;
            });
        if (Logic.Stage == 0)
            ProgressText.text = (Logic.Stage) + "/" + Logic.MaxStage + MultiLanguage.getInstance().getString("manta_game_progress_text");
        StageText.text = MultiLanguage.getInstance().getString("manta_game_stage_text") + (GameState.tangramProg.selectedLevel + 1);
    }

    private void WinStage()
    {
        AudioSystem.PlaySFX("buttons/JULES_WIN_SOUND_01");
        gameEnded = true;
        TotalTimeLeft += Mathf.FloorToInt(Logic.Timer);
        TotalTime += Mathf.FloorToInt(Logic.MaxTimer);

        playing = false;
        stage += 1;
        stageWon += 1;
        combo += 1;
        if (combo > highestCombo) highestCombo = combo;

        uiManager["Bar Fill"].GetComponent<Slider>().maxValue = Logic.MaxStage;
        uiManager["Bar Fill"].GetComponent<Slider>().value = stage - 1;

        bool isGameContinue = (hearts != 0 && stage < Logic.MaxStage);

        StageClearUI.Open(true, isGameContinue ? 1.0f : 3.0f, isGameContinue);
        DOTween.Sequence()
            .AppendInterval(isGameContinue ? 1.0f : 3.0f)
            .AppendCallback(() =>
            {
                AudioSystem.PlaySFX("buttons/JULES_MANTA_COINS_03_YIPEE_ONLY");
            })
            .AppendInterval(1f)
            .AppendCallback(CustomerPayCoinsEffect)
            .Append(uiManager["Bar Fill"].GetComponent<Slider>().DOValue(stage, 2f))
            .AppendCallback(() => ClearGame())
            .AppendInterval(1f)
            .AppendCallback(() => {
                if (isGameContinue) {
                    StartGame();
                } else {
                    GameEnd();
                }
            })
            .AppendCallback(() => {
                #region reset workstation cells to grey
                var gridGo = uiManager["Workstation Grid"];
                var workstationGrid = Logic.Workstation;
                var blueprintGrid = Logic.Blueprint;

                for (var y = 0; y < workstationGrid.size.y; ++y) {
                    for (var x = 0; x < workstationGrid.size.x; ++x) {
                        var index = y * (int)workstationGrid.size.x + x;
                        var gridCellGo = gridGo.transform.GetChild(index).gameObject;
                        var cellEmpty = workstationGrid.CheckPositionEmpty(new Vector2(x, y));
                        var blueprintEmpty = blueprintGrid.CheckPositionEmpty(new Vector2(x, y));

                        gridCellGo.SetColor(cellEmpty ? (blueprintEmpty ? new Color(1, 1, 1, 0.5f) : Color.gray) : Color.green);

                        // Disable any active glows
                        if (gridCellGo.GetComponent<Image>().material.name == "SpriteGlow")
                            gridCellGo.GetComponent<Image>().material = readyGoText.GetComponent<Text>().material; // resetting material to nothing
                    }
                }
                #endregion
            });
    }

    private void LoseStage()
    {
        gameEnded = true;
        if (!DebugMode) hearts -= 1;
        playing = false;

        combo = 0;

        var heartGo = uiManager["Hearts Container"].transform.GetChild(uiManager["Hearts Container"].transform.childCount - 1).gameObject;

        StageClearUI.Open(false, 1.0f);
        DOTween.Sequence()
            .AppendInterval(1.5f)
            .Append(heartGo.transform.DOScale(1.2f, 0.5f))
            .Join(heartGo.transform.DOShakeRotation(0.5f, new Vector3(0, 0, 45)).SetEase(Ease.OutCirc))
            .Append(heartGo.transform.DOScale(0.0f, 0.5f).SetEase(Ease.InBounce))
            .AppendCallback(() => Destroy(heartGo))
            .AppendCallback(() => ClearGame())
            .AppendInterval(1f)
            .AppendCallback(() => {
                if (hearts != 0 && stage < Logic.MaxStage) {
                    StartGame();
                } else {
                    GameEnd();
                }
            })
            .AppendCallback(() => {
                #region reset workstation cells to grey
                var gridGo = uiManager["Workstation Grid"];
                var workstationGrid = Logic.Workstation;
                var blueprintGrid = Logic.Blueprint;

                for (var y = 0; y < workstationGrid.size.y; ++y) {
                    for (var x = 0; x < workstationGrid.size.x; ++x) {
                        var index = y * (int)workstationGrid.size.x + x;
                        var gridCellGo = gridGo.transform.GetChild(index).gameObject;
                        var cellEmpty = workstationGrid.CheckPositionEmpty(new Vector2(x, y));
                        var blueprintEmpty = blueprintGrid.CheckPositionEmpty(new Vector2(x, y));

                        gridCellGo.SetColor(cellEmpty ? (blueprintEmpty ? new Color(1, 1, 1, 0.5f) : Color.gray) : Color.green);

                        // Disable any active glows
                        if (gridCellGo.GetComponent<Image>().material.name == "SpriteGlow")
                            gridCellGo.GetComponent<Image>().material = readyGoText.GetComponent<Text>().material; // resetting material to nothing
                    }
                }
                #endregion
            });
    }

    private void GameEnd()
    {
        bool gameWon = hearts != 0;
        if (gameWon) {
            bool loadBuddy = false;
            var currentLevelId = GameState.tangramProg.selectedLevel;
            var currentLevel = GameState.tangramProg.GetLevel(currentLevelId);
            int points_earned, bonus;
            int totalScore = CalculateScore(out points_earned, out bonus);
            var star_earned = TangramWinScreen.ConvertScoresToStars(totalScore);

            var level = GetLevel(GameState.tangramProg.selectedLevel);
            var tangramTrials = PlayerPrefs.GetInt("tangram.playCount.level_" + GameState.tangramProg.selectedLevel, 0);
            ++tangramTrials;
            PlayerPrefs.SetInt("tangram.playCount.level_" + GameState.tangramProg.selectedLevel, tangramTrials);

            int goldEarned = 0, rGemEarned = 0, uGemEarned = 0;
            //if (tangramTrials == 1) {
            //    goldEarned = (star_earned == 3 ? level.GetEntry ("gold_3_stars", 0) : 0) + Random.Range (level.GetEntry ("gold_first_completion_min", 0), level.GetEntry ("gold_first_completion_max", 0));
            //    rGemEarned = (star_earned == 3 ? level.GetEntry ("ruby_3_stars", 0) : 0) + level.GetEntry ("ruby_first_completion", 0);
            //} else if (tangramTrials < 20) {
            //    goldEarned = (star_earned == 3 ? 150 : 0) + Random.Range (level.GetEntry ("gold_subsequent_completion_min", 0), level.GetEntry ("gold_subsequent_completion_max", 0));
            //    rGemEarned = (star_earned == 3 ? level.GetEntry ("ruby_3_stars", 0) : 0) + level.GetEntry ("ruby_subsequent_completion", 0);
            //} else {
            //    goldEarned = (star_earned == 3 ? 150 : 0) + Random.Range (level.GetEntry ("gold_after_20_completion_max", 0), level.GetEntry ("gold_after_20_completion_min", 0));
            //    rGemEarned = (star_earned == 3 ? level.GetEntry ("ruby_3_stars", 0) : 0) + level.GetEntry ("ruby_20_completion", 0);
            //}
            if (GameState.tangramProg.GetLevel(GameState.tangramProg.selectedLevel).status == TangramPlayerProgression.ELevelStatus.Available) {
                goldEarned += Random.Range(mantaMatchDesign.levelSettings[currentLevelId].FirstClearBonusGold_Min, (mantaMatchDesign.levelSettings[currentLevelId].FirstClearBonusGold_Max + 1));
                rGemEarned += mantaMatchDesign.levelSettings[currentLevelId].FirstClearBonus_Jewel;
                uGemEarned += mantaMatchDesign.levelSettings[currentLevelId].FirstClearBonus_Jewel;
                //to send feedback when player first complete this level
                GameState.PostProgress(ActivityFeedIds.FEED_GAME_PROGRESS, currentLevelId + 1, GameNamesFeed.GAME_NAME_MANTA);

                if (star_earned == 3) {
                    goldEarned += mantaMatchDesign.levelSettings[currentLevelId].ThreeStarReward_Gold;
                    rGemEarned += mantaMatchDesign.levelSettings[currentLevelId].ThreeStarReward_Jewel;
                    uGemEarned += mantaMatchDesign.levelSettings[currentLevelId].ThreeStarReward_Jewel;
                }
            }

            goldEarned += Random.Range(mantaMatchDesign.levelSettings[currentLevelId].GoldReward_Min, (mantaMatchDesign.levelSettings[currentLevelId].GoldReward_Max + 1));
            rGemEarned += Random.Range(mantaMatchDesign.levelSettings[currentLevelId].JewelReward_Min, (mantaMatchDesign.levelSettings[currentLevelId].JewelReward_Max + 1));
            uGemEarned += Random.Range(mantaMatchDesign.levelSettings[currentLevelId].JewelReward_Min, (mantaMatchDesign.levelSettings[currentLevelId].JewelReward_Max + 1));

            if ((currentLevelId + 1) % 5 == 0) {
                loadBuddy = true;
            }


            TangramWinScreen.Open(gameWon, points_earned, bonus, goldEarned, rGemEarned, uGemEarned, loadBuddy);
            TangramWinScreen.TwoStarsThreshold = mantaMatchDesign.levelSettings[currentLevelId].ScoreThreshold_2Star;
            TangramWinScreen.ThreeStarsThreshold = mantaMatchDesign.levelSettings[currentLevelId].ScoreThreshold_3Star;



            currentLevel.pointsEarned = Mathf.Max(currentLevel.pointsEarned, totalScore);
            currentLevel.starEarned = star_earned;
            currentLevel.status = TangramPlayerProgression.ELevelStatus.Finished;
            GameState.tangramProg.SetLevel(currentLevelId, currentLevel);


            var nextLevel = GameState.tangramProg.GetLevel(currentLevelId + 1);
            if (nextLevel.status == TangramPlayerProgression.ELevelStatus.Locked) {
                nextLevel.status = TangramPlayerProgression.ELevelStatus.Available;
                GameState.tangramProg.SetLevel(currentLevelId + 1, nextLevel);
            }

        } else {
            TangramWinScreen.Open(gameWon);
        }

        Destroy(gameObject, 2f);
    }
    //end of game end.

    private int CalculateScore(out int baseScore, out int bonus)
    {
        var selectedLevel = GameState.tangramProg.GetLevel(GameState.tangramProg.selectedLevel);
        var firstTime = selectedLevel.status == TangramPlayerProgression.ELevelStatus.Available;
        baseScore = Logic.MaxStage * Logic.ScorePerPuzzle;
        bonus = TotalTimeLeft * mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].PerRemainingSecond_Score
            +
        (firstTime ? mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].StageClearBonus_Score : 0)
            +
        (hearts == 3 ? mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].PerfectGame_Score : 0);
        return baseScore + bonus;
    }

    private int CalculateStars(int score)
    {
        if (score > mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].ScoreThreshold_3Star)
            return 3;
        if (score > mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].ScoreThreshold_2Star)
            return 2;
        return 1;
    }

    bool gameEnded = false;
    bool alreadySelected = false;
    bool loseSFXPlayed = false;

    private void TextUpdate()
    {
        if (Logic == null)
            return;

        StageTimerText.text = ((int)Logic.Timer).ToString() + "s";
        StageTimerText.color = new Color(StageTimer_Red / 255.0f, StageTimer_Green / 255.0f, StageTimer_Blue / 255.0f);

        if (Logic.Timer > 8.0f) {
            if (StageTimer_Blue != 255.0f)
                StageTimer_Blue = 255.0f;
            if (StageTimer_Green != 255.0f)
                StageTimer_Green = 255.0f;

            StageTimerText.GetComponent<DOTweenVisualManager>().enabled = false;
            StageTimerScale = false;
        }

        if (Logic.Timer < 8.0f) {
            #region StageTimer Color update
            if (StageTimer_Blue > 0.0f)
                StageTimer_Blue -= (Logic.MaxTimer - Logic.Timer) * 102 * Time.deltaTime;
            if (StageTimer_Green > 0.0f && Logic.Timer < 6.0f)
                StageTimer_Green -= (40.0f - Logic.Timer) * (102) * Time.deltaTime;
            #endregion
            if (!StageTimerScale) {
                StageTimerText.GetComponent<DOTweenVisualManager>().enabled = true;
                StageTimerScale = true;
            }
        }

        //Debug.Log("StageTimer Color: " + StageTimerText.color + "\nBlue: " + StageTimer_Blue + ", Green: " + StageTimer_Green);
    }

    private void Update()
    {
        TextUpdate();
        if (levelText.text != (GameState.tangramProg.selectedLevel + 1).ToString()) {
            levelText.text = (GameState.tangramProg.selectedLevel + 1).ToString();
        }
        if (playing && DebugMode) {
            if (Input.GetKeyUp(KeyCode.RightArrow)) {
                PuzzleId++;
                PuzzleId = Mathf.Clamp(PuzzleId, 0, TangramGameLogic.allStages.Count - 1);
                DOTween.Sequence()
                    .AppendCallback(() => ClearGame())
                    .AppendInterval(1.2f)
                    .AppendCallback(() => StartGame());
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow)) {
                PuzzleId--;
                PuzzleId = Mathf.Clamp(PuzzleId, 0, TangramGameLogic.allStages.Count - 1);
                DOTween.Sequence()
                    .AppendCallback(() => ClearGame())
                    .AppendInterval(1.2f)
                    .AppendCallback(() => StartGame());
            }
            if (Input.GetKeyDown(KeyCode.P)) {
                CustomerPayCoinsEffect();
            }
        }

        if (Logic != null) {

            if (Logic.Timer < 4.0f && !loseSFXPlayed) {
                Debug.LogError("PlaySFX");
                AudioSystem.PlaySFX("buttons/JULES_MANTA_TIMEOUT_01");
                loseSFXPlayed = true;
            }

            uiManager["FuseBar"].GetComponent<Slider>().maxValue = Logic.MaxTimer;
            uiManager["FuseBar"].GetComponent<Slider>().value = Logic.Timer;
            uiManager["Fuse Fire"].SetActive(playing);

            //var fishState = Logic.Timer > Logic.MaxTimer / 2 ? "Happy" : (Logic.Timer > 5 ? "Semi" : "Angry");

            int fishStateInt = Logic.Timer > Logic.MaxTimer / 2 ? 1 : (Logic.Timer > 5 ? 2 : 0);
            int linearIndex = Logic.FishId * 3 + fishStateInt;  // 3 is the number of faces for each fish

            //var fishSprite = CachedResources.Load<Sprite>(string.Format("customer_fishes/Fish{0}_{1}", Logic.FishId, fishState));
            var fishSprite = Fishes[linearIndex];
            uiManager["Fish Head Curry"].SetImage(fishSprite);

            //var finSprite = CachedResources.Load<Sprite>(string.Format("customer_fishes/fuseFish_fins{0}", Logic.FishId));
            var finSprite = FuseFishes[Logic.FishId];
            uiManager["Fins"].SetImage(finSprite);
        }

        if (needUpdate) {
            needUpdate = false;
            UpdateInventoryGrid();
            UpdateWorkstationGrid();

            if (Logic.Workstation.CompareTo(Logic.Blueprint)) {
                WinStage();
            }
        }

        uiManager["Mouse Blocker"].GetComponent<CanvasGroup>().blocksRaycasts = !playing;

        if (!playing) return;

        if (Input.GetKeyUp(KeyCode.Alpha0)) {
            LoseStage();
        }
        if (Input.GetKeyUp(KeyCode.Alpha1)) {
            WinStage();
        }

        var previousTimer = Logic.Timer;
        Logic.Timer -= Time.deltaTime;

        if (Logic.Timer < 0f) {
            LoseStage();
        }

        if (previousTimer > 5f && Logic.Timer < 5f) {
            var sequence = DOTween.Sequence();
            for (var i = 0; i < 10; ++i) {
                var delta = Vector3.forward * (3 + i * 1.5f);
                sequence
                    .Append(uiManager["Fish Head Curry"].transform.DORotate(-delta, 0.15f).SetEase(Ease.Linear))
                    .Append(uiManager["Fish Head Curry"].transform.DORotate(delta, 0.15f).SetEase(Ease.Linear))
                    .Append(uiManager["Fish Head Curry"].transform.DORotate(Vector3.zero, 0.15f).SetEase(Ease.Linear));
            }
            uiManager["Fish Head Curry"].transform.DOScale(Vector3.one * 1.3f, 5f).SetEase(Ease.InCirc);
            uiManager["Fish Head Curry"].GetComponent<Image>().DOColor(new Color(1f, 0.4f, 0.4f), 5f).SetEase(Ease.InCirc).OnComplete(() =>
            {
                uiManager["Fish Head Curry"].SetColor(Color.clear);
            });
        }



        if (selectedPieceGo != null) {
            selectedPieceGo.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition - selectedOffset);
            selectedPieceGo.GetComponent<CanvasGroup>().alpha = 0.8f;

            #region make it glow
            /*selectedPieceGo.GetComponent<Image>().material = spriteGlowTemplate.GetComponent<Image>().material;
            if (selectedPieceGo.GetComponent<SpriteGlow>() == null)
                selectedPieceGo.AddComponent<SpriteGlow>();
            selectedPieceGo.GetComponent<SpriteGlow>().GlowColor = spriteGlowTemplate.GetComponent<SpriteGlow>().GlowColor;
            selectedPieceGo.GetComponent<SpriteGlow>().OutlineWidth = spriteGlowTemplate.GetComponent<SpriteGlow>().OutlineWidth;*/
            #endregion

            // Check dropable
            var droppable = false;
            var ped = new PointerEventData(EventSystem.current);
            ped.position = Camera.main.WorldToScreenPoint(selectedPieceGo.transform.position);
            var raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped, raycastResults);

            GameObject cellGo = null;
            foreach (var result in raycastResults) {
                if (result.gameObject.transform.IsChildOf(uiManager["Workstation Grid"].transform)) {
                    cellGo = result.gameObject;
                    break;
                }
            }

            var newPiece = selectedPiece;

            if (cellGo != null) {
                // The piece is dropped to the workstation
                var cellIndex = cellGo.transform.GetSiblingIndex();

                newPiece.position = new Vector2(cellIndex % 8, cellIndex / 8);
                droppable = Logic.Blueprint.CheckWithinBlueprint(newPiece);
            } else {
                droppable = false;
            }

            //tint colour of the texture depending if we can drop on this location
            var desiredColor = droppable ? Color.white : new Color(0.8f, 0, 0);
            if (selectedColor == default(Color)) {
                selectedColor = desiredColor;
                selectedPieceGo.GetComponent<Image>().color = selectedColor;
            } else if (selectedColor != desiredColor) {
                selectedColor = desiredColor;
                selectedPieceGo.GetComponent<Image>().DOColor(selectedColor, 0.5f);
            }

            //use the new piece because the position in newPiece has already been updated.
            UpdateWorkStationGridTint(newPiece, droppable, cellGo);
        }
    }

    private void UpdateInventoryGrid()
    {
        var grid = Logic.Inventory;
        var gridGo = uiManager["Inventory Grid"];
        // gridGo.GetComponent<CanvasGroup> ().alpha = 0f;

        for (var j = 0; j < grid.size.y; ++j) {
            for (var i = 0; i < grid.size.x; ++i) {
                var index = j * (int)grid.size.x + i;
                var cellGo = gridGo.transform.GetChild(index).gameObject;
                var cellEmpty = grid.CheckPositionEmpty(new Vector2(i, j));
                var singlePiece = grid.CheckPositionSinglePiece(new Vector2(i, j));

                cellGo.SetColor(cellEmpty ? new Color(1, 1, 1, 0.5f) : singlePiece ? Color.green : Color.green);
            }
        }

        uiManager["Inventory Pieces"].Clear();
        foreach (var piece in Logic.Inventory.GetPieces()) {
            //var prefab = CachedResources.Load ("tangram_prefabs/" + piece.name);
            //if (prefab == null) continue;
            //var pieceGo = CachedResources.Spawn ("tangram_prefabs/" + piece.name, false);
            var pieceGo = SpawnTangramPiece(piece);
            pieceGo.transform.SetParent(uiManager["Inventory Pieces"].transform);
            var index = (int)piece.position.x + (int)piece.position.y * 10;
            pieceGo.transform.position = uiManager["Inventory Grid"].transform.GetChild(index).position;


            var currentPiece = piece;
            pieceGo.AddEventTrigger(EventTriggerType.PointerDown, () => {
                if (!alreadySelected) {
                    alreadySelected = true;
                    //Debug.LogError("PlaySFX");
                    AudioSystem.PlaySFX("buttons/JULES_MENU_CLICK_01");
                    selectedOffset = Input.mousePosition - Camera.main.WorldToScreenPoint(pieceGo.transform.position); ;
                    selectedPiece = currentPiece;
                    selectedPieceGo = pieceGo;
                    pieceGo.transform.SetParent(uiManager["Drag Layer"].transform);
                    pieceGo.ClearEventTrigger(EventTriggerType.PointerDown);
                } else
                    return;

                pieceGo.AddEventTrigger(EventTriggerType.PointerUp, () => {
                    if (gameEnded) {
                        Destroy(selectedPieceGo);
                        return;
                    }
                    alreadySelected = false;
                    var ped = new PointerEventData(EventSystem.current);
                    ped.position = Camera.main.WorldToScreenPoint(pieceGo.transform.position);
                    var raycastResults = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(ped, raycastResults);

                    GameObject cellGo = null;
                    foreach (var result in raycastResults) {
                        if (result.gameObject.transform.IsChildOf(uiManager["Workstation Grid"].transform)) {
                            cellGo = result.gameObject;
                            break;
                        }
                    }

                    if (cellGo != null) {
                        // The piece is dropped to the workstation
                        var cellIndex = cellGo.transform.GetSiblingIndex();
                        var newPiece = selectedPiece;
                        newPiece.position = new Vector2(cellIndex % 8, cellIndex / 8);
                        //OLD LOGIC, just incase we want to revert it.
                        //This logic does not allow dropping a piece ontop of another already placed piece.
                        /*
                        if (Logic.Workstation.CheckPieceFit (newPiece) && Logic.Blueprint.CheckWithinBlueprint (newPiece)) {
							Debug.LogError("PlaySFX");
                            AudioSystem.PlaySFX("buttons/JULES_SOFT_CLICK_PUZZLE_DOWN_01");
                            //place the piece to the location
							Logic.Workstation.AddPiece (newPiece);
                            Destroy (selectedPieceGo);
                            needUpdate = true;
                        } 
                        else {
							//Debug.LogError("PlaySFX");
							AudioSystem.PlaySFX("buttons/JULES_WRONG_SOUND_01");
                            ReturnPieceToOriginalPosition(selectedPiece, selectedPieceGo);
                        }
                        */

                        // Now we want to change the logic, we will allow overlapping of pieces into one that already exist in the box
                        // First we want to know if the piece is inside the blueprint
                        if (Logic.Blueprint.CheckWithinBlueprint(newPiece)) {
                            //This means we can drop it
                            //Now we need to handle any piece that we overlapped with.
                            //Get the list of piece that we are overlapping
                            List<TangramGameLogic.TangramPiece> list = Logic.Workstation.GetListOfOverlap(newPiece);

                            for (int i = 0; i < list.Count; ++i) {
                                TangramGameLogic.TangramPiece overlap_piece = list[i];

                                Debug.LogError("Removing Piece: " + overlap_piece.name);

                                //Remove the piece from work station.
                                Logic.Workstation.RemovePiece(overlap_piece);

                                //reset piece position to inventory position;
                                overlap_piece.position = overlap_piece.inventoryPosition;

                                //Add it into inventory grid
                                Logic.Inventory.AddPiece(overlap_piece);

                            }

                            //Now we have hanlded the overlapping piece, we can drop this piece onto the workstation grid
                            AudioSystem.PlaySFX("buttons/JULES_SOFT_CLICK_PUZZLE_DOWN_01");
                            //place the piece to the location
                            //Debug.LogError("Adding Piece: " + newPiece.name);
                            Logic.Workstation.AddPiece(newPiece);

                            Destroy(selectedPieceGo);
                            needUpdate = true;
                        } else {
                            //Debug.LogError("PlaySFX");
                            AudioSystem.PlaySFX("buttons/JULES_WRONG_SOUND_01");
                            ReturnPieceToOriginalPosition(selectedPiece, selectedPieceGo);
                        }
                    } else {
                        //Debug.LogError("PlaySFX");
                        AudioSystem.PlaySFX("buttons/JULES_WRONG_SOUND_01");
                        ReturnPieceToOriginalPosition(selectedPiece, selectedPieceGo);
                    }

                    selectedPieceGo = null;

                });

                needUpdate = true;
                Logic.Inventory.RemovePiece(currentPiece);
            });
        }

        SortChildren(uiManager["Inventory Pieces"]);
    }

    private void ReturnPieceToOriginalPosition(TangramGameLogic.TangramPiece piece, GameObject pieceGO)
    {
        // Return the piece back to the original position
        Logic.Inventory.AddPiece(piece);
        var inventoryIndex = (int)piece.position.x + (int)piece.position.y * 10;
        pieceGO.transform.SetParent(uiManager["Inventory Pieces"].transform);
        pieceGO.transform.DOMove(uiManager["Inventory Grid"].transform.GetChild(inventoryIndex).transform.position, 0.2f).OnComplete(() => needUpdate = true);

    }

    public void CustomerPayCoinsEffect()
    {
        // Spawn the sparking effect
        //var effect = CachedResources.Spawn("Tangram Coin Explosion");
        var effect = CachedResources.Spawn(coinExplosionEffectPrefab);

        //Reset fish head incase it's running animation
        uiManager["Fish Head Curry"].transform.rotation.Set(0, 0, 0, 0);
        uiManager["Fish Head Curry"].transform.localScale = Vector3.one;

        effect.transform.position = uiManager["Fish Head Curry"].transform.position + Vector3.back * 0.01f;

        // Spawn some coins that fly to the piggy bank
        var sequence = DOTween.Sequence();

        for (var i = 0; i < 20; ++i) {
            sequence
                .AppendInterval(i*0.01f)
                .AppendCallback(() => {
                    DOTween.Sequence()
                        .Append(uiManager["Fish Head Curry"].transform.DOScale(1.3f, 0.1f))
                        .Append(uiManager["Fish Head Curry"].transform.DOScale(1f, 0.2f));

                    var coin = new GameObject("Coin");
                    AudioSystem.PlaySFX("buttons/JULES_MANTA_COINS_02");
                    coin.transform.SetParent(uiManager["Coin Container"].transform);
                    coin.transform.position = uiManager["Fish Head Curry"].transform.position;
                    coin.transform.localRotation = Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.forward);
                    coin.transform.DOLocalRotate(new Vector3(0, 0, 180f), 0.05f).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);

                    var controlPoints = new List<Vector3>();
                    var middlePoint = Vector3.Lerp(coin.transform.position, uiManager["Pig"].transform.position, 0.5f);
                    var perpendicular = Vector3.Cross(coin.transform.position - middlePoint, Vector3.forward);

                    controlPoints.Add(coin.transform.position);
                    controlPoints.Add(coin.transform.position + perpendicular * Random.Range(0f, 1f) + Vector3.up * Random.Range(1f, 3f));
                    controlPoints.Add(middlePoint + perpendicular * Random.Range(0f, 1f));
                    controlPoints.Add(uiManager["Pig"].transform.position);
                    var path = Bezier.GenerateBezierInterpolate(controlPoints);

                    Image coinImage = coin.AddComponent<Image>();
                    coinImage.sprite = coinSprite;
                    coin.transform.position = uiManager["Fish Head Curry"].transform.position + 30f * (Vector3)Random.insideUnitCircle;
                    coin.transform.DOPath(path.ToArray(), Random.Range(0.3f, 0.5f)).OnComplete(() => {
                        Destroy(coin);
                        DOTween.Sequence()
                            .Append(uiManager["Pig"].transform.DOScale(1.3f, 0.1f))
                            .Append(uiManager["Pig"].transform.DOScale(1f, 0.2f));
                    });
                });
            ProgressText.text = (Logic.Stage + 1) + "/" + Logic.MaxStage + MultiLanguage.getInstance().getString("manta_game_progress_text");
        }
    }

    private GameObject levelBg;
    private void UpdateWorkstationGrid()
    {

        if (levelBg == null) {
            var blueprint = Logic.Blueprint.GetPieces()[0];
            levelBg = SpawnTangramPiece(blueprint);
            levelBg.transform.SetParent(uiManager["Workstation"].transform);
            levelBg.SetAlpha(0.5f);
            int index = (int)blueprint.position.y * 8 + (int)blueprint.position.x;
            levelBg.transform.position = uiManager["Workstation Grid"].transform.GetChild(index).position;
            levelBg.transform.SetAsFirstSibling();
        }

        uiManager["Workstation Pieces"].Clear();
        foreach (var piece in Logic.Workstation.GetPieces()) {
            // var prefab = CachedResources.Load ("tangram_prefabs/" + piece.name);
            // if (prefab == null) continue;
            // var pieceGo = CachedResources.Spawn ("tangram_prefabs/" + piece.name, false);
            var pieceGo = SpawnTangramPiece(piece);
            pieceGo.transform.SetParent(uiManager["Workstation Pieces"].transform);
            var index = (int)piece.position.x + (int)piece.position.y * 8;
            pieceGo.transform.position = uiManager["Workstation Grid"].transform.GetChild(index).position;
            var currentPiece = piece;
            pieceGo.AddEventTrigger(EventTriggerType.PointerDown, () => {
                if (!alreadySelected) {
                    alreadySelected = true;
                    Debug.LogError("PlaySFX");
                    AudioSystem.PlaySFX("buttons/JULES_MENU_CLICK_01");
                    selectedOffset = Input.mousePosition - Camera.main.WorldToScreenPoint(pieceGo.transform.position);
                    selectedPiece = currentPiece;

                    // Make sure the selected piece is described in their inventory position
                    selectedPiece.position = selectedPiece.inventoryPosition;

                    selectedPieceGo = pieceGo;
                    pieceGo.transform.SetParent(uiManager["Drag Layer"].transform);

                    pieceGo.ClearEventTrigger(EventTriggerType.PointerDown);
                } else
                    return;


                pieceGo.AddEventTrigger(EventTriggerType.PointerUp, () => {
                    if (gameEnded) {
                        Destroy(selectedPieceGo);
                        return;
                    }
                    alreadySelected = false;
                    var ped = new PointerEventData(EventSystem.current);
                    ped.position = Camera.main.WorldToScreenPoint(pieceGo.transform.position);
                    var raycastResults = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(ped, raycastResults);

                    GameObject cellGo = null;
                    foreach (var result in raycastResults) {
                        if (result.gameObject.transform.IsChildOf(uiManager["Workstation Grid"].transform)) {
                            cellGo = result.gameObject;
                            break;
                        }
                    }
                    if (cellGo != null) {
                        // The piece is dropped to the workstation
                        var cellIndex = cellGo.transform.GetSiblingIndex();
                        var newPiece = selectedPiece;
                        newPiece.position = new Vector2(cellIndex % 8, cellIndex / 8);
                        //Old code, which doesn't allow over lapping an existing piece on the work station board
                        /*
                        if (Logic.Workstation.CheckPieceFit (newPiece) && Logic.Blueprint.CheckWithinBlueprint (newPiece))
						{
							Debug.LogError("PlaySFX");
							AudioSystem.PlaySFX("buttons/JULES_SOFT_CLICK_PUZZLE_DOWN_01");
							Logic.Workstation.AddPiece (newPiece);

                            Destroy (selectedPieceGo);
                            needUpdate = true;
                        } else {
							Debug.LogError("PlaySFX");
							AudioSystem.PlaySFX("buttons/JULES_WRONG_SOUND01");
                            // Return the piece back to the original
                            ReturnPieceToOriginalPosition(selectedPiece, selectedPieceGo);
                        }
                        */

                        // Now we want to change the logic, we will allow overlapping of pieces into one that already exist in the box
                        // First we want to know if the piece is inside the blueprint
                        if (Logic.Blueprint.CheckWithinBlueprint(newPiece)) {
                            //This means we can drop it
                            //Now we need to handle any piece that we overlapped with.
                            //Get the list of piece that we are overlapping
                            List<TangramGameLogic.TangramPiece> list = Logic.Workstation.GetListOfOverlap(newPiece);

                            for (int i = 0; i < list.Count; ++i) {
                                TangramGameLogic.TangramPiece overlap_piece = list[i];

                                //Remove the piece from work station.
                                Logic.Workstation.RemovePiece(overlap_piece);

                                //reset piece position to inventory position;
                                overlap_piece.position = overlap_piece.inventoryPosition;

                                //ReturnPieceToOriginalPosition(overlap_piece, overlap_pieceGo);
                                Logic.Inventory.AddPiece(overlap_piece);
                            }

                            //Now we have hanlded the overlapping piece, we can drop this piece onto the workstation grid
                            AudioSystem.PlaySFX("buttons/JULES_SOFT_CLICK_PUZZLE_DOWN_01");
                            //place the piece to the location
                            Logic.Workstation.AddPiece(newPiece);

                            Destroy(selectedPieceGo);
                            needUpdate = true;
                        } else {
                            Debug.LogError("PlaySFX");
                            AudioSystem.PlaySFX("buttons/JULES_WRONG_SOUND_01");
                            ReturnPieceToOriginalPosition(selectedPiece, selectedPieceGo);
                        }
                    } else {
                        // Return the piece back to the original position
                        ReturnPieceToOriginalPosition(selectedPiece, selectedPieceGo);
                    }
                    selectedPieceGo = null;
                });

                needUpdate = true;
                Logic.Workstation.RemovePiece(currentPiece);
            });
        }

        SortChildren(uiManager["Workstation Pieces"]);

        UpdateWorkStationGridTint(selectedPiece);
    }

    /**
     * Update work station grid tinting colour.
     * 
     * HoveringPiece must be put in because I cannot assign null to it.
     * 
     * If there is a hoveringPiece, and is droppable, the grid that the hovering piece is on will be tinted blue
     * Pieces that is already on the work station grid will also be tinted blue.
     **/
    private void UpdateWorkStationGridTint(TangramGameLogic.TangramPiece hoveringPiece, bool droppable = false, GameObject hoveringSelectedGO = null)
    {
        var gridGo = uiManager["Workstation Grid"];
        var workstationGrid = Logic.Workstation;
        var blueprintGrid = Logic.Blueprint;

        List<TangramGameLogic.TangramPiece> overlappedList = null;

        //if we are checking hovering piece
        //we also want to do an effect that the overlapping piece on the board will be half transparent, and they longer tint the grid to green
        //so that player can clearly see what happens when they overlap that piece on board.
        //because of the possible previous transparent set
        //let's always reset all game object under workstation grid alpha
        foreach (var piece in workstationGrid.GetPieces()) {
            GameObject overlappedGo = uiManager["Workstation Pieces"].FindChild(piece.name);
            overlappedGo.SetAlpha(1);
        }

        //now make sure we are checking for hovering piece
        if (hoveringSelectedGO != null) {
            //now get the full list of overlapping pieces
            overlappedList = workstationGrid.GetListOfOverlap(hoveringPiece);

            for (int i = 0; i < overlappedList.Count; i++) {
                TangramGameLogic.TangramPiece piece = overlappedList[i];


                Debug.Log("Checking piece BEFORE has moved: " + piece.hasMoved);
                //get the game object from uiManager
                GameObject overlappedGo = uiManager["Workstation Pieces"].FindChild(piece.name);
                //set alpha to half
                overlappedGo.SetAlpha(0.5f);
                if (!piece.hasMoved) {
                    piece.hasMoved = true; //(true);

                    Debug.Log("Reassigned Piece to moved");
                    overlappedGo.gameObject.transform.position = new Vector2(overlappedGo.gameObject.transform.position.x - 10, overlappedGo.gameObject.transform.position.y + 10);
                    piece.needResetPosition = true;
                }

                overlappedList[i] = piece;


                Debug.Log("Checking piece AFTER has moved: " + piece.hasMoved);
                //in order for us to easily remove the tint on the workstation for those overlapping piece
                //we will temporatly remove those pieces from the work station
                workstationGrid.RemovePiece(piece);
            }
        }

        //now perform the tinting check.
        for (var j = 0; j < workstationGrid.size.y; ++j) {
            for (var i = 0; i < workstationGrid.size.x; ++i) {
                var index = j * (int)workstationGrid.size.x + i;
                var gridCellGo = gridGo.transform.GetChild(index).gameObject;
                var cellEmpty = workstationGrid.CheckPositionEmpty(new Vector2(i, j));
                var blueprintEmpty = blueprintGrid.CheckPositionEmpty(new Vector2(i, j));

                //Want to tint the blueprint grid that the selectedGO piece is currently hovering
                //So that the player knows where it will drop to
                if (cellEmpty && hoveringSelectedGO != null)
                    cellEmpty = !(droppable && hoveringPiece.CheckCover(new Vector2(i, j)));

                gridCellGo.SetColor(cellEmpty ? (blueprintEmpty ? new Color(1, 1, 1, 0.5f) : Color.gray) : Color.green);

                if (hoveringSelectedGO != null) {
                    if (droppable && hoveringPiece.CheckCover(new Vector2(i, j))) {
                        // Make cells glow when being hovered over
                        gridCellGo.GetComponent<Image>().material = spriteGlowTemplate.GetComponent<Image>().material;
                        if (gridCellGo.GetComponent<SpriteGlow>() == null)
                            gridCellGo.AddComponent<SpriteGlow>();
                        gridCellGo.GetComponent<SpriteGlow>().GlowColor = spriteGlowTemplate.GetComponent<SpriteGlow>().GlowColor;
                        gridCellGo.GetComponent<SpriteGlow>().OutlineWidth = spriteGlowTemplate.GetComponent<SpriteGlow>().OutlineWidth;
                    } else {
                        // Disable glow when no longer hovering over those cells
                        if (gridCellGo.GetComponent<Image>().material.name == "SpriteGlow")
                            gridCellGo.GetComponent<Image>().material = selectedPieceGo.GetComponent<Image>().material; // resetting material to nothing
                    }
                } else
                    gridCellGo.SetColor(cellEmpty ? (blueprintEmpty ? new Color(1, 1, 1, 0.5f) : Color.gray) : Color.green);
            }
        }

        //if our overlap list is not empty
        if (overlappedList != null) {
            foreach (var piece in overlappedList) {
                //add it back to the workstation after tinting check
                workstationGrid.AddPiece(piece);
            }
        } else
            return;

        if (overlappedList.Count < tempListCount) {
            uiManager["Workstation Pieces"].Clear();
            for (int i = 0; i < workstationGrid.GetPieces().Count; i++) {
                TangramGameLogic.TangramPiece piece = workstationGrid.GetPieces()[i];

                var pieceGo = SpawnTangramPiece(piece);
                pieceGo.transform.SetParent(uiManager["Workstation Pieces"].transform);
                var index = (int)piece.position.x + (int)piece.position.y * 8;
                pieceGo.transform.position = uiManager["Workstation Grid"].transform.GetChild(index).position;

                piece.SetHasMoved(false);
                piece.SetNeedResetPosition(false);
                var currentPiece = piece;
                workstationGrid.ReplacePiece(i, currentPiece);
            }
        }

        tempListCount = overlappedList.Count;
    }

    private void SortChildren(GameObject parent)
    {
        var children = new List<GameObject>();
        foreach (Transform child in parent.transform) {
            children.Add(child.gameObject);
        }
        children.Sort((x, y) => {
            var boundX = x.rectTransform().sizeDelta;
            var boundY = y.rectTransform().sizeDelta;
            var sizeX = boundX.x * boundX.y;
            var sizeY = boundY.x * boundY.y;
            return sizeY.CompareTo(sizeX);
        });
        foreach (var child in children) {
            child.transform.SetAsLastSibling();
        }
    }

    public static GameObject SpawnTangramPiece(TangramGameLogic.TangramPiece piece)
    {
        GameObject go = CachedResources.Spawn(_inst.tangram_template);
        go.GetComponent<Image>().sprite = _inst.m_MantaParts[piece.name];
        //var go = CachedResources.Spawn("tangram_prefabs/tangram_template");
        //go.GetComponent<Image>().sprite = CachedResources.Load<Sprite>("mantaparts/" + piece.name);
        go.rectTransform().sizeDelta = piece.bound * TangramGridSpawn.GRID_SIZE.x;
        go.rectTransform().pivot = new Vector2(0.5f / piece.bound.x, 1 - 0.5f / piece.bound.y);
        go.transform.localScale = Vector3.one;
        go.name = piece.name;
        go.layer = LayerMask.NameToLayer("Tangram Pieces");
        go.AddComponent<EnforceScale>();
        return go;
    }

    public static GameObject SpawnTangramPiece(string pieceName)
    {
        var tangram = TangramGameLogic.TangramPiece.CreateNew(pieceName);
        return SpawnTangramPiece(tangram);
    }

    public void ResetWorkstationGrid()
    {
        List<TangramGameLogic.TangramPiece> list = Logic.Workstation.GetPieces();
        var gridGo = uiManager["Workstation Grid"];
        var workstationGrid = Logic.Workstation;
        var blueprintGrid = Logic.Blueprint;

        for (int i = 0; i < list.Count; ++i) {
            TangramGameLogic.TangramPiece piece = list[i];

            Debug.LogError("Removing Piece: " + piece.name);

            //Remove the piece from work station.
            Logic.Workstation.RemovePiece(piece);

            //reset piece position to inventory position;
            piece.position = piece.inventoryPosition;

            //Add it into inventory grid
            Logic.Inventory.AddPiece(piece);

            if (i == (list.Count - 1)) {
                needUpdate = true;
                #region reset workstation cells to grey
                for (var y = 0; y < workstationGrid.size.y; ++y) {
                    for (var x = 0; x < workstationGrid.size.x; ++x) {
                        var index = y * (int)workstationGrid.size.x + x;
                        var gridCellGo = gridGo.transform.GetChild(index).gameObject;
                        var cellEmpty = workstationGrid.CheckPositionEmpty(new Vector2(x, y));
                        var blueprintEmpty = blueprintGrid.CheckPositionEmpty(new Vector2(x, y));

                        gridCellGo.SetColor(cellEmpty ? (blueprintEmpty ? new Color(1, 1, 1, 0.5f) : Color.gray) : Color.green);

                        // Disable any active glows
                        if (gridCellGo.GetComponent<Image>().material.name == "SpriteGlow")
                            gridCellGo.GetComponent<Image>().material = readyGoText.GetComponent<Text>().material; // resetting material to nothing
                    }
                }
                #endregion
            }
        }
    }
}

public class TangramGameLogic {

    public int FishId;
    public int MaxStage, Stage, ScorePerPuzzle;
    public float MaxTimer, Timer;
    public TangramGrid Workstation = new TangramGrid(new Vector2(8, 8)); //grid to keep the piece that placed inside
    public TangramGrid Blueprint = new TangramGrid(new Vector2(8, 8)); //grid to keep the answer of the pattern.
    public TangramGrid Inventory = new TangramGrid(new Vector2(10, 10)); //right side, inventory grid.

    public static JSONArray allStages;// = JSON.Parse<JSONArray>(Resources.Load<TextAsset>("json/tangram/puzzles").text);
    private static JSONClass previousLevel = null;

    private static JSONClass GetStageBasedOnDifficulty(int difficulty)
    {
        List<JSONClass> stages = new List<JSONClass>();
        foreach (JSONClass stage in allStages) {
            if (stage.GetEntry("difficulty", 0) == difficulty) {
                stages.Add(stage);
            }
        }
        if (stages.Count > 0) {
            int stageId = Random.Range(0, stages.Count);
            return stages[stageId];
        } else {
            return null;
        }
    }
    private static JSONClass GetStageBasedOnIndex(int index)
    {
        return (JSONClass)allStages[index];
    }

    public void SetupGame(int levelId, int stageId = 0)
    {
        Workstation.Clear();
        Blueprint.Clear();
        Inventory.Clear();

        //TangramPiece.InitializePieceData();

        var levelJson = TangramGame.GetLevel(levelId);
        var stageJson = GetStageBasedOnDifficulty(levelJson["puzzles"][stageId].AsInt);

        while (previousLevel == stageJson) {
            // Debug.LogError("Same level as before, getting new stage");
            stageJson = GetStageBasedOnDifficulty(levelJson["puzzles"][stageId].AsInt);
        }
        // Debug.LogError("DIFFICULTY: " + levelJson["puzzles"][stageId].AsInt);

        //MaxStage = levelJson.GetEntry ("puzzle_num", 1);
        MaxStage = mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].Puzzle_Count;
        Stage = Mathf.Clamp(stageId, 0, MaxStage - 1);

        FishId = stageJson.GetEntry("fish", 1);

        MaxTimer = Timer = mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].PuzzleTime_Limit;
        ScorePerPuzzle = mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].PerPuzzleCleared_Score;
        Blueprint.Initialize(stageJson.GetJson("blueprint", new JSONClass()));
        Inventory.Initialize(stageJson.GetJson("inventory", new JSONClass()));
        previousLevel = stageJson;
    }

    public void SetupPuzzle(int stageId = 0)
    {
        Workstation.Clear();
        Blueprint.Clear();
        Inventory.Clear();

        var stageJson = GetStageBasedOnIndex(stageId);
        MaxStage = 50;
        Stage = Mathf.Clamp(stageId, 0, MaxStage - 1);
        FishId = stageJson.GetEntry("fish", 1);
        //MaxTimer = Timer = stageJson.GetEntry ("timer", 45f);
        MaxTimer = Timer = mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].PuzzleTime_Limit;
        Blueprint.Initialize(stageJson.GetJson("blueprint", new JSONClass()));
        Inventory.Initialize(stageJson.GetJson("inventory", new JSONClass()));
    }


    public class TangramGrid {
        public Vector2 size;
        private List<TangramPiece> pieces = new List<TangramPiece>();

        public TangramGrid(Vector2 size)
        {
            this.size = size;
        }

        public void Initialize(JSONClass json)
        {
            Clear();

            var sizeJson = json.GetJson("size", new JSONClass());
            size.x = sizeJson.GetEntry("x", size.x);
            size.y = sizeJson.GetEntry("y", size.y);

            var piecesJson = json.GetJson("pieces", new JSONArray());
            foreach (JSONClass pieceJson in piecesJson) {
                var piece = TangramPiece.CreateFromJson(pieceJson);
                AddPiece(piece);
            }
        }

        public bool CheckWithinGrid(Vector2 position)
        {
            if (position.x < 0) return false;
            if (position.y < 0) return false;
            if (position.x >= size.x) return false;
            if (position.y >= size.y) return false;
            return true;
        }

        /**
         * Returns a list of TangramPiece that the input piece is overlapping in the grid
         **/
        public List<TangramPiece> GetListOfOverlap(TangramPiece newPiece)
        {
            List<TangramPiece> list = new List<TangramPiece>();

            //check for each position inside new piece location
            foreach (var localposition in newPiece.localPositions) {
                var position = localposition + newPiece.position;
                //aginst all pieces inside this grid
                foreach (var piece in pieces) {
                    if (piece.CheckCover(position)) {
                        //overlapping
                        //make sure we don't already added it in.
                        if (list.Contains(piece))
                            continue;
                        //add piece into list
                        list.Add(piece);

                        //go to next piece
                        continue;
                    }
                }
            }

            return list;
        }

        /**
         * Return true if the position is inside the grid and is not overlapping another piece
         **/
        public bool CheckPositionEmpty(Vector2 position)
        {
            if (!CheckWithinGrid(position)) return false;

            foreach (var piece in pieces) {
                if (piece.CheckCover(position)) return false;
            }

            return true;
        }

        /*
         * Check if this tile object is single piece.
         * Only return true if this position has a piece and is single piece.
         * */
        public bool CheckPositionSinglePiece(Vector2 position)
        {
            if (!CheckPositionEmpty(position)) {
                return GetPieceAtPosition(position).IsSinglePiece();
            }

            //by default false.
            return false;
        }

        public TangramPiece GetPieceAtPosition(Vector2 position)
        {
            if (!CheckWithinGrid(position)) return TangramPiece.INVALID;
            foreach (var piece in pieces) {
                if (piece.CheckCover(position)) return piece;
            }
            return TangramPiece.INVALID;
        }

        /**
         * Return true if the piece is able to fit into the grid without overlapping anything existing
         **/
        public bool CheckPieceFit(TangramPiece newPiece)
        {
            foreach (var localPosition in newPiece.localPositions) {
                var position = localPosition + newPiece.position;
                if (!CheckPositionEmpty(position)) return false;
            }
            return true;
        }

        public void AddPiece(TangramPiece newPiece)
        {
            if (!CheckPieceFit(newPiece)) return;
            pieces.Add(newPiece);
        }

        public List<TangramPiece> GetPieces()
        {
            return new List<TangramPiece>(pieces);
        }


        public void ReplacePiece(int pos, TangramPiece piece)
        {
            if (pieces.Count <= pos) {
                Debug.Log("Tangram Board replace piece invalid, position does not exist");
                return;
            }

            pieces[pos] = piece;
        }


        public void RemovePiece(TangramPiece piece)
        {
            for (var i = 0; i < pieces.Count; ++i) {
                if (piece.Equals(pieces[i])) {
                    pieces.RemoveAt(i);
                    return;
                }
            }
        }

        /**
         * Return true if the  piece is fully overlapping with the piece
         * Ideal for checking if the piece is within the blueprint grid
         **/
        public bool CheckWithinBlueprint(TangramPiece newPiece)
        {
            foreach (var localPosition in newPiece.localPositions) {
                var position = localPosition + newPiece.position;
                if (CheckPositionEmpty(position)) return false;
            }
            return true;
        }

        public void Clear()
        {
            pieces.Clear();
        }

        public bool CompareTo(TangramGrid grid)
        {
            for (var i = 0; i < size.x; ++i) {
                for (var j = 0; j < size.y; ++j) {
                    Vector2 position = new Vector2(i, j);
                    if (CheckPositionEmpty(position) != grid.CheckPositionEmpty(position)) return false;
                }
            }
            return true;
        }
    }

    public struct TangramPiece {

        private static JSONClass _tangramPieceData;
        public static JSONClass TangramPieceData {
            get {
                /*
                if (_tangramPieceData == null) {
                    var jsonText = Resources.Load<TextAsset>("json/tangram/pieces").text;
                    _tangramPieceData = JSON.Parse<JSONClass>(jsonText);
                }
                //*/
                return _tangramPieceData;
            }
        }
        public static IEnumerator InitializePieceData()
        {
            yield return JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.MANTA_MATCH_CONFIG, "pieces", (loadedAsset) => {
                _tangramPieceData = JSON.Parse<JSONClass>(loadedAsset.text);
            });
        }

        public static TangramPiece INVALID {
            get {
                var piece = new TangramPiece("invalid");
                piece.isInvalid = false;
                return piece;
            }
        }

        public static TangramPiece CreateNew(string name)
        {
            var pieceJson = TangramPieceData.GetJson(name, new JSONArray());
            var localPositions = new Vector2[pieceJson.Count];
            for (var i = 0; i < pieceJson.Count; ++i) {
                var positionJson = (JSONArray)pieceJson[i];
                localPositions[i] = new Vector2(positionJson[0].AsInt, -positionJson[1].AsInt);
            }
            return new TangramPiece(name, localPositions);
        }

        public static TangramPiece CreateFromJson(JSONClass json)
        {
            var name = json.GetEntry("name", string.Empty);
            var positionJson = json.GetJson("position", new JSONArray());
            var position = new Vector2(positionJson[0].AsInt, positionJson[1].AsInt);
            var piece = CreateNew(name);
            piece.position = position;
            piece.inventoryPosition = position;
            return piece;
        }

        public string name;
        public Vector2 position;
        public Vector2 inventoryPosition;
        public Vector2[] localPositions;

        public Vector2 bound { get; private set; }
        public Vector2 anchor { get; private set; }

        public bool isInvalid { get; private set; }
        public bool hasMoved { get; set; }
        public bool needResetPosition { get; set; }

        private TangramPiece(string name, params Vector2[] localPositions)
        {
            this.name = name;
            this.localPositions = localPositions;
            position = Vector2.zero;
            inventoryPosition = Vector2.zero;

            var min = Vector2.zero;
            var max = Vector2.zero;
            foreach (var v in localPositions) {
                min.x = Mathf.Min(min.x, v.x);
                min.y = Mathf.Min(min.y, v.y);
                max.x = Mathf.Max(max.x, v.x);
                max.y = Mathf.Max(max.y, v.y);
            }

            bound = new Vector2(max.x - min.x + 1, max.y - min.y + 1);
            anchor = new Vector2(Mathf.Abs(min.x), Mathf.Abs(min.y));

            //Debug.Log("Create new Piece");
            isInvalid = true;
            hasMoved = false;
            needResetPosition = false;
        }

        public void SetHasMoved(bool value)
        {
            this.hasMoved = value;
        }

        public void SetNeedResetPosition(bool value)
        {
            this.needResetPosition = value;
        }

        public bool CheckCover(Vector2 position)
        {
            var localPosition = position - this.position;
            foreach (var v in localPositions) {
                if (v == localPosition) return true;
            }
            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) {
                return false;
            }
            if (!(obj is TangramPiece)) {
                return false;
            }
            var piece = (TangramPiece)obj;
            return piece.name == name && piece.position == position;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool IsSinglePiece()
        {
            return localPositions.Length == 1;
        }
    }
}
