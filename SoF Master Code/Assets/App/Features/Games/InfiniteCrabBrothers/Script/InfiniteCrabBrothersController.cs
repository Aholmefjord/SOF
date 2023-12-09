using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
public class InfiniteCrabBrothersController : MonoBehaviour {
    public Animator answerIndicator;
    public bool TriggerReset;
    public CrabUIManager uiManager;
    public List<Animator> buttonAnimators;
    public int health = 3;
    public int TimeTaken;
    public int puzzleAt = 0;

    public List<Crab> crabGO;
    public List<Crab> crabGoSix;
    public List<Crab> buttonCrabs;
    Crab hiddenCrab;

    public TextAsset puzzles;
    public TextAsset levels;

    public InfiniteCrabBrothersPuzzleContainer icbpc;
    public InfiniteCrabBrothersPuzzle currentICBP;
    public InfiniteCrabBrothersLevel currentLevel;
    public CrabResetter buttonPanel;
    public UnityEngine.UI.Text pauseScreenLevel;
    public GameObject pauseUI;

    private bool mGamePaused;
	// Use this for initialization
	void Start()
	{
        AudioSystem.PlayBGM("bgm_icb_circus-sneak-peak");
        mGamePaused = false;

        tumbleTroubleDesign.Load();
		PlayerPrefs.SetInt("CrabbyLevel", GameState.infiniteProg.selectedLevel);
		// PlayerPrefs.SetInt("CrabbyLevel", 12);
		puzzleAt = 0;
		health = 3;
		CrabResources.InitResources();
		icbpc = new InfiniteCrabBrothersPuzzleContainer(puzzles, levels);
		currentLevel = icbpc.levels[PlayerPrefs.GetInt("CrabbyLevel", 80)];
		//Invoke("PlayStartSound", 0.5f);
        //        SelectNextPuzzle();

        SetupUI();

    }

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(gameObject.FindChild("StageIdentifier").FindChild("Text"), "crab_brothers_game_stage");

        MultiLanguageApplicator textProc = new MultiLanguageApplicator(pauseUI);
        textProc.ApplyText("Paused Text", "in_game_menu_title");
        textProc.ApplyText("CurrentStage", "in_game_menu_current_stage");

        textProc.ApplyImage("MainPanel/Pause Panel/Game Title Image"    , "crabby_main_menu_title");
        textProc.ApplyImage("MainPanel/Pause Panel/Button Home"         , "gui_settings_home");
        textProc.ApplyImage("MainPanel/Pause Panel/Button Level Select" , "gui_settings_levelselect");
        textProc.ApplyImage("MainPanel/Pause Panel/Button Resume"       , "gui_settings_resume");
        textProc.ApplyImage("MainPanel/Pause Panel/Button Restart"      , "gui_settings_restart");
    }

    void PlayStartSound()
	{
		AudioSystem.PlaySFX("buttons/JULES_CRAB_INTRO_01");
	}

    float tickTimer;
    // Update is called once per frame
    void Update() {
        if (!mGamePaused) { 
            tickTimer += Time.deltaTime;
            if (tickTimer > 1) { tickTimer = 0; TimeTaken += 1; }
            uiManager.UpdateUI(health, puzzleAt, currentLevel.patternSequence.Count, PlayerPrefs.GetInt("CrabbyLevel", 80));
        }
    }
    public void SelectNextPuzzle()
    {
        if (puzzleAt < currentLevel.patternSequence.Count)
        {
            #region if not at limit, select next puzzle
            for (int i = 0; i < crabGO.Count; i++)
            {
                crabGO[i].gameObject.SetActive(false);
            }

            currentICBP = icbpc.SelectNewPuzzle(PlayerPrefs.GetInt("CrabbyLevel", 80), puzzleAt);
            Debug.Log("Current ICBP AT " + puzzleAt + ": " + currentICBP.toString());
            List<Crab> displayCrabs = (currentICBP.pattern.Count/2 == 4)? crabGO: crabGoSix;
            if (currentICBP.pattern.Count == 6)
            {
                SetupSix();
            }else
            {
                SetupEight();
            }

            #region hiding the crab
            int crabToHide = currentICBP.pattern.Count - 1;
            switch (currentLevel.hiddenCrab)
            {
                case "All":
                    crabToHide = Random.Range(0, currentICBP.pattern.Count - 1);
                    break;
                case "Bottom":
                    crabToHide = Random.Range(currentICBP.pattern.Count / 2, currentICBP.pattern.Count - 1);
                    break;
            }
            Debug.Log("Hiding Crab: " + crabToHide);
            displayCrabs[crabToHide].InitCrab(currentICBP.pattern[crabToHide][0], currentICBP.pattern[crabToHide][1], true);
            hiddenCrab = displayCrabs[crabToHide];
            #endregion

            #region setting up the buttons
            int CrabAnswer = Random.Range(0, 3);
            List<int> foundColors = new List<int>();
            for(int i= 0; i < currentICBP.pattern.Count; i++)
            {
                if (!foundColors.Contains(currentICBP.pattern[i][0]))
                {
                    foundColors.Add(currentICBP.pattern[i][0]);
                }
            }
            if (currentICBP.answers.Count <= 0)
            {

                #region randomAnswers
                for(int i = 0; i < 4; i++)
                {
                    if (CrabAnswer == i)
                    {
                        Debug.Log("ANSWER IS: [" + currentICBP.pattern[crabToHide][0] + " , " + currentICBP.pattern[crabToHide][1] + "]" + " : " + i);
                        buttonCrabs[i].InitCrab(currentICBP.pattern[crabToHide][0], currentICBP.pattern[crabToHide][1], false);

                    }
                    else buttonCrabs[i].InitCrab(foundColors[Random.Range(0, foundColors.Count-1)], Random.Range(1, 9), false);
                    buttonCrabs[i].GetComponent<Animator>().SetInteger("pose", buttonCrabs[i].pose);
                    buttonCrabs[i].GetComponent<Animator>().SetTrigger("reset");
                }
                #endregion
                for(int i = 0; i < crabGO.Count; i++)
                {
                    displayCrabs[i].Reset();
                }

            }
            else
            {
                #region predetermined answers
                for (int i = 0; i < 4; i++)
                {
                    buttonCrabs[i].InitCrab(currentICBP.answers[i][0], currentICBP.answers[i][1], false);
                    buttonCrabs[i].GetComponent<Animator>().SetInteger("pose", buttonCrabs[i].pose);
                    buttonCrabs[i].GetComponent<Animator>().SetTrigger("reset");
                }
                #endregion
                for (int i = 0; i < crabGO.Count; i++)
                {
                    displayCrabs[i].Reset();
                }
            }
            #endregion

            Debug.Log("Puzzle Next: " + puzzleAt);
            puzzleAt += 1;
            #endregion
        }
        else
        {
            WinGame();
        }
    }
    void SetupSix()
    {

        #region setting up the crabs
        for (int j = 0; j < 2; j++)
        {
            Random.InitState((int)Time.timeSinceLevelLoad + Random.Range(0, 123123123) + System.DateTime.Now.Millisecond);
            Random.InitState((int)Time.timeSinceLevelLoad + Random.Range(0, 123123123) + System.DateTime.Now.Millisecond);
            Random.InitState((int)Time.timeSinceLevelLoad + Random.Range(0, 123123123) + System.DateTime.Now.Millisecond);
            int dancePoseToSet = Random.Range(0, 10);
            dancePoseToSet = Random.Range(0, 10);
            dancePoseToSet = Random.Range(0, 7);
            for (int i = 0; i < 3; i++)
            {
                int indexCrab = i + ((j * 3)); // get the index of the current crab
                int indexPattern = i + ((j * 3));
                Debug.Log("Setting Crab at point: " + indexCrab);
                crabGoSix[indexCrab].gameObject.SetActive(true);
                Debug.Log(currentICBP.pattern.Count);
                crabGoSix[indexCrab].GetComponent<Animator>().SetInteger("dancePose", dancePoseToSet);
                Debug.Log("ANSWER IS: [" + currentICBP.pattern[indexPattern][0] + " , " + currentICBP.pattern[indexPattern][1] + "]" + " : " + i);
                crabGoSix[indexCrab].InitCrab(currentICBP.pattern[indexPattern][0], currentICBP.pattern[indexPattern][1], false);
                crabGoSix[indexCrab].numberLabel.text = (++indexCrab).ToString();
            }
            Debug.Log("-----------------------------------");
        }
        PlayStartSound();
        crabGoSix[6].gameObject.SetActive(false);
        crabGoSix[7].gameObject.SetActive(false);
        #endregion
    }
    void SetupEight()
    {

        #region setting up the crabs
        for (int j = 0; j < 2; j++)
        {
            Random.InitState((int)Time.timeSinceLevelLoad + Random.Range(0, 123123123) + System.DateTime.Now.Millisecond);
            Random.InitState((int)Time.timeSinceLevelLoad + Random.Range(0, 123123123) + System.DateTime.Now.Millisecond);
            Random.InitState((int)Time.timeSinceLevelLoad + Random.Range(0, 123123123) + System.DateTime.Now.Millisecond);
            int dancePoseToSet = Random.Range(0, 10);
            dancePoseToSet = Random.Range(0, 10);
            dancePoseToSet = Random.Range(0, 7);
            Debug.Log("Setting Dance: " + dancePoseToSet);
            Debug.Log("-----------------------------------");
            for (int i = 0; i < 4; i++)
            {
                int indexCrab = i + ((j * 4)); // get the index of the current crab
                int indexPattern = i + ((j * 4));
                crabGO[indexCrab].gameObject.SetActive(true);
                Debug.Log("Index at: " + indexPattern);
                Debug.Log(currentICBP.pattern.Count);
                //                    Debug.Log("Pattern Type: " + currentICBP.pattern[indexPattern][0] + "," + currentICBP.pattern[indexPattern][1]-1 + " " + i + "," + j);
                //crabGO[indexCrab].GetComponent<Animator>().SetInteger("dancePose", dancePoseToSet);
                crabGO[indexCrab].InitCrab(currentICBP.pattern[indexPattern][0], currentICBP.pattern[indexPattern][1], false);
                crabGO[indexCrab].numberLabel.text = (++indexCrab).ToString();
            }
            Debug.Log("-----------------------------------");
        }
        PlayStartSound();
        #endregion
    }
    #region UIInteraction
    public void ShowSettings()
    {
        pauseUI.SetActive(true);
        pauseScreenLevel.text = GameState.infiniteProg.selectedLevel + 1 +"";
    }
    public void Restart()
    {
        //MainNavigationController.GoToScene("inifinite_crab_brothers_game");
        //Application.LoadLevel(Application.loadedLevel);
        MainNavigationController.DoAssetBundleLoadLevel(Constants.CRAB_BROS_SCENES, "inifinite_crab_brothers_game");
	}
    public void GoToMenu()
    {
        //MainNavigationController.GoToScene("crab_stage_selected");
        MainNavigationController.DoAssetBundleLoadLevel(Constants.CRAB_BROS_SCENES, "crab_stage_selected");
    }

    public void ExitGame()
    {
        MainNavigationController.GotoMainMenu();
    }

    public bool SelectAnswer(Crab b)
    {
        bool correct = hiddenCrab.Equals(b);
        if (correct)
        {
			if (puzzleAt == currentLevel.patternSequence.Count)
			{
				AudioSystem.PlaySFX("buttons/JULES_CRAB_CORRECT_CLAP_04");
			}
			else
			{
				AudioSystem.PlaySFX("buttons/JULES_CRAB_CORRECT_03");
			}
            answerIndicator.SetTrigger("correct");
            buttonPanel.ResetButtons();
        } else
        {
			AudioSystem.PlaySFX("buttons/JULES_CRAB_WRONG_03");
			answerIndicator.SetTrigger("incorrect");
            Debug.Log("Selected Answer: " + health);
            health -= 1;
            if(health <= 0)
            {
                LoseGame();
            }
        }
        SelectCrabAnswers(correct);
        return correct;
    }
    void SelectCrabAnswers(bool b)
    {
        for (int i = 0; i < crabGO.Count; i++)
        {
            crabGO[i].MakeGuess(b);
        }
    }
    #endregion

    public void PauseGame()
    {
        mGamePaused = true;
    }

    public void ResumeGame()
    {
        mGamePaused = false;
    }


    #region endGameConditions
    public void WinGame()
    {
		var currentLevelId = GameState.infiniteProg.selectedLevel;
		var currentLevelData = GameState.infiniteProg.GetLevel(currentLevelId);
		bool loadBuddy = false;
		int score = currentLevel.patternSequence.Count * 1000;
		int gold = 0, uGem = 0, rGem = 0;

		if (GameState.infiniteProg.GetLevel(currentLevelId).status == InfiniteBrosProgression.ELevelStatus.Available)
		{
			score += tumbleTroubleDesign.levelSettings[currentLevelId].StageClearBonus_Score;
			gold += Random.Range(tumbleTroubleDesign.levelSettings[currentLevelId].FirstClearBonusGold_Min, tumbleTroubleDesign.levelSettings[currentLevelId].FirstClearBonusGold_Max + 1);
			rGem += tumbleTroubleDesign.levelSettings[currentLevelId].FirstClearBonus_Jewel;
			uGem += tumbleTroubleDesign.levelSettings[currentLevelId].FirstClearBonus_Jewel;
            
        }
        if (currentLevelData.status != InfiniteBrosProgression.ELevelStatus.Finished)
        {
            //to send feedback when player first complete this level
            GameState.PostProgress(4, currentLevelId + 1, "Crabby Patty");
        }
        if (GameState.infiniteProg.GetLevel(currentLevelId).status == InfiniteBrosProgression.ELevelStatus.Available && health == 3)
		{
			gold += tumbleTroubleDesign.levelSettings[currentLevelId].ThreeStarReward_Gold;
			rGem += tumbleTroubleDesign.levelSettings[currentLevelId].ThreeStarReward_Jewel;
			uGem += tumbleTroubleDesign.levelSettings[currentLevelId].ThreeStarReward_Jewel;
		}

		gold += Random.Range(tumbleTroubleDesign.levelSettings[currentLevelId].GoldReward_Min, (tumbleTroubleDesign.levelSettings[currentLevelId].GoldReward_Max + 1));
		rGem += Random.Range(tumbleTroubleDesign.levelSettings[currentLevelId].JewelReward_Min, (tumbleTroubleDesign.levelSettings[currentLevelId].JewelReward_Max + 1));
		uGem += Random.Range(tumbleTroubleDesign.levelSettings[currentLevelId].JewelReward_Min, (tumbleTroubleDesign.levelSettings[currentLevelId].JewelReward_Max + 1));

		if ((currentLevelId + 1) % 5 == 0)
		{
			loadBuddy = true;
		}
        if(health == 3)
        {
            score += 500;
        }

		currentLevelData.pointsEarned = Mathf.Max(currentLevelData.pointsEarned, score);
		currentLevelData.starEarned = health;
		currentLevelData.status = InfiniteBrosProgression.ELevelStatus.Finished;
		GameState.infiniteProg.SetLevel(currentLevelId, currentLevelData);

		var nextLevel = GameState.infiniteProg.GetLevel(currentLevelId + 1);
		if (nextLevel.status == InfiniteBrosProgression.ELevelStatus.Locked)
		{
			nextLevel.status = InfiniteBrosProgression.ELevelStatus.Available;
			GameState.infiniteProg.SetLevel(currentLevelId + 1, nextLevel);
		}

		CrabWinScreenUI.Open(true, score, gold, rGem, uGem, health, loadBuddy);
        Debug.Log("Win Game");
    }
    public void LoseGame()
    {
        int score = puzzleAt -1 * 1000;
        CrabWinScreenUI.Open(false, score, 0, 0, 0);
        Debug.Log("Lose Game");
    }
    #endregion
}

#region data structures
public class InfiniteCrabBrothersPuzzleContainer
{
    public List<InfiniteCrabBrothersPuzzle> puzzles;
    public List<InfiniteCrabBrothersLevel> levels;
    public InfiniteCrabBrothersPuzzleContainer(TextAsset puzzlez, TextAsset levelz)
    {
        puzzles = new List<InfiniteCrabBrothersPuzzle>();
        levels = new List<InfiniteCrabBrothersLevel>();
        JSONArray t_s = JSONArray.Parse(levelz.text).AsArray;

        foreach(JSONNode n in t_s)
        {
            InfiniteCrabBrothersLevel icbl = new InfiniteCrabBrothersLevel();
            JSONArray pSequence = n["patternSequence"].AsArray;

            icbl.hiddenCrab = n["hiddenCrab"];
            List<int> patterns = new List<int>();
           
            foreach (JSONNode p in pSequence)
            {
                patterns.Add(p.AsInt);
            }
            icbl.patternSequence = patterns;


            levels.Add(icbl);
        }
        JSONArray p_s = JSONArray.Parse(puzzlez.text).AsArray;
        int i = 0;
        foreach (JSONNode n in p_s)
        {
            InfiniteCrabBrothersPuzzle icbp = new InfiniteCrabBrothersPuzzle();
            JSONArray pSequence = n["pattern"].AsArray;
            icbp.difficulty = n["difficulty"];
            
            Debug.Log("loading pattern [" + i + ": " + n["pattern"].ToString());
            List<List<int>> patterns = new List<List<int>>();
            foreach (JSONNode p in pSequence)
            {
                List<int> innerPattern = new List<int>();
                JSONArray innerArray = p.AsArray;
                foreach (JSONNode ip in innerArray)
                {
                    innerPattern.Add(ip.AsInt);
                }
                patterns.Add(innerPattern);
            }
            
            icbp.pattern = patterns;
            //getting the answers
            JSONArray aSequence = n["solution"].AsArray;
            List<List<int>> answers = new List<List<int>>();
            foreach (JSONNode a in aSequence)
            {
                List<int> innerAnswer = new List<int>();
                JSONArray innerArray = a.AsArray;
                foreach(JSONNode ia in innerArray)
                {
                    innerAnswer.Add(ia.AsInt);
                }
                answers.Add(innerAnswer);
            }
            icbp.answers = answers;
            puzzles.Add(icbp);
            Debug.Log("Translated to : " + icbp.toString());
            i++;

        }
    }
    public InfiniteCrabBrothersPuzzle SelectNewPuzzle(int level,int puzzleAt)
    {

        int index = levels[level].patternSequence[puzzleAt];
        //Index - 1 as index is now start from 1 instead of 0.
        Debug.Log("Loading puzzle at index: " + index);
        return puzzles[index];
    }
}
public class InfiniteCrabBrothersLevel
{
    public string hiddenCrab;
    public List<int> patternSequence;
}
public class InfiniteCrabBrothersPuzzle {
    public string difficulty;
    public List<List<int>> pattern;
    public List<List<int>> answers;
    public string toString()
    {
        string s = "[ ";
        for(int i = 0; i < pattern.Count; i++)
        {
            s += "[" + pattern[i][0] + "," + pattern[i][1] + "]";
                 
        }
        s += "]";
        return s;
    }
}
#endregion
