using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PowGameController : MonoBehaviour
{
    public PowEnemySpawn pes;

    public GameObject dialogPanel;
    public GameObject pausePanel;
    public static int[] fishPlayedAt = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
    public static bool isPaused = false;
    public static bool isFinished = false;
    public static float COOLDOWN_MODIFIER = 1;
    public static float EnemyHealth = 0; //This is in seconds til fully depleted
    public PowEnemyHPUI hpUI;
    public PowEnemyHPUI playerhpUI;
    public PowGameManaUI manaUI;
    public float EnemyHealthStart = 0;
    public static float GlobalScale = 10;
    public static float AttackDistanceScale = 1;
    public static PlayerStats Player;
    Vector3 originalCamPos;
    public TextAsset ta;
    public TextAsset fishesAsset;

    public List<PowLevelSpawns> levelSpawns;
    public List<TextAsset> assets;
    public List<GameObject> objects;

    public List<GameObject> fishButtons;

    public float SallyHealth = 5000;
    int LevelAt = 1;
    public UnityEngine.UI.Text levelText;
    // Use this for initialization
    public PowLevelSpawns pls;
    public List<Sprite> fishImagesToUse;

    public static List<GameObject> enemyFishPool;
    public static List<GameObject> friendlyFishPool;

    private bool isBgmPlaying;
    public TakoStageScroll tss;

    private bool shownDialogueBox;
    private bool readDialogueBox;
    private float delayDialogueBox;
    public bool dialogueSequenceOver;
    private float originalCamSize;
    public static bool restartedlevel = false;

    void Start()
    {

        fishPlayedAt = new int[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        isPaused = false;
        isFinished = false;
        enemyFishPool = new List<GameObject>();
        enemyFishPool.Add(gameObject.FindChild("EnemyLocation"));
        friendlyFishPool = new List<GameObject>();
        friendlyFishPool.Add(gameObject.FindChild("PlayerLocation"));
        if(restartedlevel==true)
        {
            LevelAt = PlayerPrefs.GetInt("TakoLevel", 0);
            LevelAt -= 1;
            restartedlevel = false;
        }
        else
        LevelAt = PlayerPrefs.GetInt("TakoLevel", 0);

        //Start BGM
        AudioSystem.PlayBGM("bgm_tako");
        isBgmPlaying = true;

		SetupUI();        //safe case, just in case we have a bugged out player save file
        if (LevelAt >= 9)
            LevelAt = 9;

        levelText.text = MultiLanguage.getInstance().getString("tako_game_bottom_level") + (LevelAt + 1);
        ta = assets[LevelAt];
        pls = new PowLevelSpawns();
        pls.Load(ta, fishesAsset, fishImagesToUse,LevelAt +1);
        originalCamPos = Camera.main.transform.position;
        #region levelSetup
        Player = new PlayerStats(10, 5000, pls.FishPowerPerSecond, this);
        Player.PlayerMana = 0;
        EnemyHealth = pls.HPKraken;
        EnemyHealthStart = EnemyHealth;
        pes.objectsToSpawn = new List<List<GameObject>>();
        pes.objectSpawnInternal = new List<List<GameObject>>();
        for (int i = 0; i < pls.FishSpawns.Count; i++)
        {
            List<int> characterNumbers = pls.FishSpawns[i];
            List<GameObject> characterLists = new List<GameObject>();
            for (int j = 0; j < characterNumbers.Count; j++)
            {
                characterLists.Add(objects[characterNumbers[j]]);
            }
            pes.objectSpawnInternal.Add(characterLists);
            pes.objectsToSpawn.Add(characterLists);
        }
        pes.objectSpawnRate = pls.SpawnRates;
        pes.objectSpawnRateInternal = new List<List<int>>(pls.SpawnRates);
        Debug.Log("Need to spawn rate: " + pes.objectSpawnRate.Count);
        Debug.Log("Need to spawn rate: " + pes.objectSpawnRateInternal.Count);

        pes.initialSpawnCount = pls.initialSpawnCount;
        pes.packNumber = pls.NumberOfPacks;
        pes.packSpawnTimer = pls.PackTimer;
        tss.DistanceApart = pls.MapLength;
        tss.Init();
        //dialogPanel.GetComponent<TakoPopupBox>().InitPopupBox(pls.boxes);
        for (int i = pls.AvailableFish; i < fishButtons.Count; i++)
        {
            fishButtons[i].SetActive(false);
        }
        try { 
        for(int i = 0; i < GameState.takoProg.lastMinimum; i++)
        {
            fishButtons[i].GetComponent<PowUnitSpawner>().newText.enabled = false;
        }
        GameState.takoProg.lastMinimum = pls.AvailableFish;
        }catch(System.Exception e)
        {

        }
		
        if (pls.AvailableFish <= 4)
        {
            gameObject.FindChild("Page1").FindChild("PageButton").SetActive(false); 
        }
        #endregion

        #region Dialogue Box Sequence
        shownDialogueBox = false;
        readDialogueBox = false;
        delayDialogueBox = 0.0f;
        //if (pls.Level > 6)
        //    dialogueSequenceOver = true;
        //else
            dialogueSequenceOver = false;
        originalCamSize = 20.9f;
        #endregion
    }

    public void SetupUI()
    {
        //Pause Menu
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Game Title Image").GetComponent<Image>(), "tako_main_menu_title");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Buttons Holder Panel").FindChild("Button Resume").GetComponent<Image>(), "gui_settings_resume");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Buttons Holder Panel").FindChild("Button Restart").GetComponent<Image>(), "gui_settings_restart");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Buttons Holder Panel").FindChild("Button Home").GetComponent<Image>(), "gui_settings_home");
        MultiLanguage.getInstance().applyImage(pausePanel.FindChild("Buttons Holder Panel").FindChild("Button Level Select").GetComponent<Image>(), "gui_settings_levelselect");

        MultiLanguage.getInstance().apply(pausePanel.FindChild("Paused Text"), "in_game_menu_title");
        MultiLanguage.getInstance().apply(pausePanel.FindChild("CurrentStage"), "in_game_menu_current_stage");

        pausePanel.FindChild("Current Stage Text").GetComponent<Text>().text = (LevelAt + 1).ToString();

        //bottom UI
        MultiLanguage.getInstance().apply(gameObject.FindChild("LevelIcon").FindChild("Text"), "tako_game_bottom_level");
        MultiLanguage.getInstance().apply(gameObject.FindChild("LevelIcon").FindChild("Text"), "tako_game_bottom_level");

        for (int i = 0; i < fishButtons.Count; i++)
        {
            MultiLanguage.getInstance().apply(fishButtons[i].FindChild("NEWIcon"), "tako_game_bottom_new");
            MultiLanguage.getInstance().apply(fishButtons[i].FindChild("OKIcon"), "tako_game_bottom_ok");
        }

        MultiLanguage.getInstance().apply(gameObject.FindChild("Page1").FindChild("PageButton").FindChild("Text (1)"), "tako_game_bottom_page_1");
        MultiLanguage.getInstance().apply(gameObject.FindChild("Page2").FindChild("PageButton").FindChild("Text (1)"), "tako_game_bottom_page_2");
    }

    float duration = 0.5f;
    float magnitude = 0.2f;
    public IEnumerator Shake()
    {
        float elapsed = 0.0f;
        while (elapsed < duration)
        {

            elapsed += Time.deltaTime;

            float percentComplete = elapsed / duration;
            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            // map value to [-1, 1]
            float x = Random.value * 2.0f - 1.0f;
            float y = Random.value * 2.0f - 1.0f;
            x *= magnitude * damper;
            y *= magnitude * damper;

            Camera.main.transform.position = new Vector3(originalCamPos.x+x, originalCamPos.y + y, originalCamPos.z);

            yield return null;
        }

        Camera.main.transform.position = originalCamPos;
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Logg("Update");
        #region Health and Mana Updates
        hpUI.ratio = EnemyHealth / EnemyHealthStart;
        playerhpUI.ratio = Player.PlayerHealth / 5000f;
        manaUI.ratio = Player.PlayerMana;
        //isPaused = pausePanel.activeSelf || dialogPanel.activeSelf;

        /*
        if (isPaused && isBgmPlaying)
        {
            isBgmPlaying = false;
            //AudioSystem.PauseBGM();
        }

        if (!isPaused && !isBgmPlaying)
        {
            //AudioSystem.ResumeBGM();
            isBgmPlaying = true;
        }
        */

        if (!isPaused && !isFinished)
        {
            Debug.Log("Player Health: " + Player.PlayerHealth + " Enemy Health: " + EnemyHealth);
            Player.Update();
            if (EnemyHealth <= 0)
            {
                isFinished = true;
                WinGame();
            }
            if (Player.PlayerHealth <= 0)
            {
                isFinished = true;
                LoseGame();
            }
        }
        #endregion
        #region Dialogue Box
        if (!dialogueSequenceOver)
        {
            if (!shownDialogueBox)
            {
                // Scrolling camera to enemy base locatoin
                if (tss.positionToAddTo < pls.enemyBasePos_X)
                    tss.positionToAddTo += 1;
                else if (tss.positionToAddTo >= pls.enemyBasePos_X)
                {
                    delayDialogueBox += Time.deltaTime;
                    float camSizeDecrease = delayDialogueBox;

                    // make sure camera size doesn't go lower than 10
                    if (gameObject.GetComponent<Camera>().orthographicSize < 10)
                        gameObject.GetComponent<Camera>().orthographicSize = 10;

                    // decrease camera size to zoom in
                    if (gameObject.GetComponent<Camera>().orthographicSize > 10)
                        gameObject.GetComponent<Camera>().orthographicSize -= delayDialogueBox;
                    else
                        if(delayDialogueBox > 2.0f) shownDialogueBox = true;

                    //Debug.Log("delayDialogueBox: " + delayDialogueBox);
                    //Debug.Log("camSizeDecrease: " + camSizeDecrease + "\ncamSize: " + gameObject.GetComponent<Camera>().orthographicSize);
                }
            }
            else if (shownDialogueBox && !readDialogueBox)
            {
                dialogPanel.GetComponent<TakoPopupBox>().InitPopupBox(pls.boxes);
                readDialogueBox = true;
            }
            else if (readDialogueBox && !dialogPanel.GetActive())
            {
                if(gameObject.GetComponent<Camera>().orthographicSize != originalCamSize)
                gameObject.GetComponent<Camera>().orthographicSize = originalCamSize;

                // Scrolling camera to player base locatoin
                if (tss.positionToAddTo > pls.playerBasePos_X)
                    tss.positionToAddTo -= 1;
                else if (tss.positionToAddTo <= pls.playerBasePos_X)
                    dialogueSequenceOver = true;
            }
        }
        #endregion
    }
    public void CameraShake()
    {
        StartCoroutine("Shake");
    }
    public void WinGame()
    {
        //Hide Game Layer

        AudioSystem.stop_bgm();
        DoUpdateLevel();
    }
    public void LoseGame()
    {
        var currentLevelId = PlayerPrefs.GetInt("TakoLevel");
        var currentLevelData = GameState.takoProg.GetLevel(currentLevelId);
        bool loadBuddy = false;
        int score = 1000;
        int gold = 0; //uGem = 0, rGem = 0;

        if(!currentLevelData.hasPlayedLevel)
        {
            currentLevelData.hasPlayedLevel = true;
            currentLevelData.fishUsedfirstPlay = fishPlayedAt;
            //first time playing game            
        }else
        {

        }
        loadBuddy = true;
        GameState.takoProg.SetLevel(currentLevelId, currentLevelData);
        TakoWinScreen.Open(false, score, gold, 0, 0, 1,true);

        AudioSystem.stop_bgm();
    }

    void DoUpdateLevel()
    {
        var currentLevelId = PlayerPrefs.GetInt("TakoLevel");
        var currentLevelData = GameState.takoProg.GetLevel(currentLevelId);
        bool loadBuddy = false;

        /*
         * 3 Stars: player base health full, 3000 points
         * 2 Stars: player win, 2000 points
         * 1 Star: player lose, 1000 points
         * 
         * */
         
        int score = (Player.PlayerHealth == Player.healthMax) ? 3000 : 2000;
        int gold = 0; //uGem = 0, rGem = 0;

        currentLevelData.pointsEarned = Mathf.Max(currentLevelData.pointsEarned, score);
        currentLevelData.starEarned = (Player.PlayerHealth == Player.healthMax) ? 3 : 2;
        currentLevelData.status = TakoPlayerProgression.ELevelStatus.Finished;
        if (GameState.takoProg.GetLevel(currentLevelId).status == TakoPlayerProgression.ELevelStatus.Available)
        {
            score += tugotakoDesign.levelSettings[currentLevelId].StageClearBonus_Score;
            gold += Random.Range(tugotakoDesign.levelSettings[currentLevelId].FirstClearBonusGold_Min, tugotakoDesign.levelSettings[currentLevelId].FirstClearBonusGold_Max + 1);
        }

        if (GameState.takoProg.GetLevel(currentLevelId).status == TakoPlayerProgression.ELevelStatus.Available)// && health == 3)
        {
            gold += tugotakoDesign.levelSettings[currentLevelId].ThreeStarReward_Gold;
        }

        if (!currentLevelData.hasPlayedLevel)
        {
            currentLevelData.hasPlayedLevel = true;
            currentLevelData.fishUsedfirstPlay = fishPlayedAt;
        }
        else
        {

        }
        if(currentLevelData.starEarned == 3)
        {
            currentLevelData.fishUsed3Stars = fishPlayedAt;
        }
        gold += Random.Range(tugotakoDesign.levelSettings[currentLevelId].GoldReward_Min, (tugotakoDesign.levelSettings[currentLevelId].GoldReward_Max + 1));        
        loadBuddy = true;
        GameState.takoProg.SetLevel(currentLevelId, currentLevelData);
        var nextLevel = GameState.takoProg.GetLevel(currentLevelId + 1);
        if (nextLevel.status == TakoPlayerProgression.ELevelStatus.Locked)
        {
            nextLevel.status = TakoPlayerProgression.ELevelStatus.Available;
            GameState.takoProg.SetLevel(currentLevelId + 1, nextLevel);
        }
        TakoWinScreen.Open(true, score, gold, 0, 0, currentLevelData.starEarned, loadBuddy);
    }


    public void PauseGame()
    {
        pausePanel.SetActive(true);
        isPaused = true;
    }

    public void PauseGameOnly()
    {
        isPaused = true;
    }

    public void ResumeGameOnly()
    {
        isPaused = false;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        isPaused = false;
    }

    public void RestartGame()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene("tako_trouble");
    }

    public void GoToHome()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        MainNavigationController.GotoMainMenu();
    }

    public void GoToLevelSelect()
    {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene("tako_level_select");
    }
}
public class PlayerStats
{
    public float PlayerHealth;
    public float PlayerMana;
    public readonly float manaMax;
    readonly PowGameController pgc;
    public float healthMax;
    int prevMana;
    float regenRate; //in multiples of seconds, if 1 then it regens at 1 mana per second
    public PlayerStats(float ManaMax, float HealthMax, float RegenRate,PowGameController powGC)
    {
        this.manaMax = ManaMax;
        this.healthMax = HealthMax;
        this.PlayerHealth = HealthMax;
        this.regenRate = RegenRate;
        this.pgc = powGC;
        this.prevMana = 0;
    }
    public void Update()
    {
        if(PlayerMana < manaMax && GameObject.Find("TakoGame").GetComponent<PowGameController>().dialogueSequenceOver)
        {
            PlayerMana += Time.deltaTime * regenRate;

            if ((int)System.Math.Floor(PlayerMana) > prevMana)
            {
                prevMana = (int)PlayerMana;
                AudioSystem.PlaySFX("tako/SFX_Tako_GaugePop");
            }
        }
       
    }
    public void AddHealth(float amnt)
    {
        PlayerHealth = Mathf.Min(PlayerHealth+amnt,healthMax);

    }
    public void RemoveHealth(float amnt)
    {
        PlayerHealth -= amnt;
        pgc.CameraShake();
    }
    public bool RemoveMana(float amnt)
    {
        if (PlayerMana - amnt > 0)
        {
            PlayerMana -= amnt;

            prevMana = (int)System.Math.Floor(PlayerMana);
            return true;
        }
        else return false;
    }
}
public class PowLevel
{

}
[System.Serializable()]
public class PowLevelSpawns
{
    public int Level = 1;
    public int initialSpawnCount = 1;
    public int HPKraken = 1;
    public int NumberOfPacks = 1;
    public float FishPowerPerSecond;
    public float PackTimer = 10;
    public List<List<int>> FishSpawns;
    public List<List<int>> SpawnRates;
    public TextAsset assetToLoad;
    public List<TakoPopupImage> boxes;

    public int AvailableFish = 0;
    public int MapLength = 0;
    public float enemyBasePos_X, playerBasePos_X;

    public void Load(TextAsset textAsset, TextAsset fishesAsset, List<Sprite> spritesToUse, int levelAt)
    {
        Debug.Log("Level At: " + levelAt);
        var jsonData = JSON.Parse<JSONClass>(textAsset.text);
        Level = jsonData.GetEntry<int>("Level", 1);
        initialSpawnCount = jsonData.GetEntry<int>("initialSpawnCount", 1);
        NumberOfPacks = jsonData.GetEntry<int>("NoOfPacks", 0);
        AvailableFish = jsonData.GetEntry<int>("AvailableFish", 0);
        MapLength = jsonData.GetEntry<int>("MapLength", 1)/4;
        PackTimer = jsonData.GetEntry<float>("PackTimer", 10f);

        enemyBasePos_X = jsonData.GetEntry<float>("enemyBasePos_X", 30);
        playerBasePos_X = jsonData.GetEntry<float>("playerBasePos_X", -30);

        #region LevelLists 
        FishSpawns = new List<List<int>>();
        SpawnRates = new List<List<int>>();
        int i = 0;
        while (true)
        {
            try {
                List<int> ints = new List<int>();
                JSONArray levelList1 = jsonData["spawnSet" + i].AsArray;
                foreach (JSONNode j in levelList1)
                {
                    ints.Add(j.AsInt);
                }
                FishSpawns.Add(ints);

                List<int> ints11 = new List<int>();
                JSONArray timerList1 = jsonData["spawnTimer" + i].AsArray;
                foreach (JSONNode j in timerList1)
                {
                    ints11.Add(j.AsInt);
                }
                SpawnRates.Add(ints11);
            } catch (System.Exception e)
            {
                break;
            }
            i++;
        }
        #endregion

        HPKraken = jsonData.GetEntry("HPKraken", 500);
        FishPowerPerSecond = jsonData.GetEntry<float>("FishPowerPerSecond", 1.0f);
        boxes = new List<TakoPopupImage>();
        var jsonFishes = JSON.Parse<JSONClass>(fishesAsset.text);
        foreach (JSONNode j in jsonFishes["DisplayFishes"].AsArray)
        {
            int levelToDisplay = j.GetEntry<int>("AppearIn");
            if (levelToDisplay == levelAt)
            {
                string name = j["FishName"];
                Sprite s = spritesToUse[j["ImageIndex"].AsInt];
                bool isFriendly = j.GetEntry<string>("Alliance", "Enemy").Equals("Friendly");
                string type = j.GetEntry<string>("type", "");
                TakoPopupImage tpi = new TakoPopupImage(s, name, j["ImageIndex"].AsInt, isFriendly, type);
                tpi.isNew = levelToDisplay < levelAt;
                boxes.Add(tpi);
            }
        }
    }


}