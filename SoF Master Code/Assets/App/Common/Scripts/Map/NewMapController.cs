using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UniRx;
using SimpleJSON;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class NewMapController : MonoBehaviour
{
   // public GameObject IQTest;
    public Text childCodeDisplayText;
    public GameObject buildCamera;
    public GameObject buildCanvas;
    public GameObject[] buddies;
    public BuildCamera buildC;
    public GameObject SettingCanvas;
    public GameObject ExitCanvas;
    public GameObject SetPasswordCanvas;
    public GameObject LanguageCanvas;
    public GameObject TableCanvas;

    private static NewMapController _instance;

    public bool[] unlockedLevels = new bool[6];

    public string current_level_type;

    [SerializeField]
    GameObject[] MapNodes;

    [SerializeField]
    GameObject TitleCard;

    [SerializeField]
    GameObject GameLogoType;

    [SerializeField]
    GameObject SubMenu;

    [SerializeField]
    GameObject YoutubeMenu;

    /* Reef Walk    - 0
     * Chomp        - 1
     * Plonk        - 2
     * Click        - 3
     * 5th Game     - 4
     * 6th Game     - 5
     * */

    [SerializeField]
    Sprite[] GameLogoSpriteList;

    [SerializeField]
    int NumMapNodes = 10;

    [SerializeField]
    GameObject[] ScrollButton;

    [SerializeField]
    GameObject[] Screenshots;

    [SerializeField]
    GameObject HomeObject;

    [SerializeField]
    GameObject Tower;

    [SerializeField]
    GameObject MovieScreen;

    [SerializeField]
    GameObject AnimScreen;

    Dictionary<string, int> LastLevelIDs;
    Dictionary<string, int> HighestAchievementStorage;

    private int LevelOffset = 0;
    public int CurrentPage = 0;

    bool inFocus = false;

    Hashtable stages;

    const string ReefWalkName = "Reef Walk";
    const string ChompChompName = "Chomp Chomp";
    const string PlonkName = "Plonk!";
    const string ClickName = "Click!";
    const string FojaName = "Whoosh!";

	public Text hungerText;
	public GameObject hungerPanel;
	int hour;
	int min;
	bool isMorning;
	public Text timeText;
	public GameObject buddyGO;
	bool playAnimation = false;

    [Header("SetupUI language change"), SerializeField]
    RoomListReceiver m_ChatPanel;

    public void EnableIQTest(int time)
    {
        GameObject iqGO = GameObject.Instantiate(Resources.Load("ui/IQTest") as GameObject);
        iqGO.SetActive(true);
        iqGO.GetComponent<IQTest>().Start(time);
    }
    public static NewMapController current
    {
        get
        {
            return _instance;
        }
    }
    public void SelectInteractiveWorksheet(int i)
    {
        GameObject b = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("ui/InteractiveWorksheet"));
        b.GetComponent<InteractiveWorksheet>().Init(i);
    }
    void OnDestroy()
	{
        GameState.me.Upload();
		PlayerPrefs.SetString("exitTime", System.DateTime.Now.ToBinary().ToString());
    }

    private void OnEnable()
    {
        MultiLanguage.getInstance().OnLanguageChanged += LanguageChangedCallback;
    }
    private void OnDisable()
    {
        MultiLanguage.getInstance().OnLanguageChanged -= LanguageChangedCallback;
    }
    void LanguageChangedCallback(string prev, string curr)
    {
        SetupUI();
    }

    void Awake()
    {
		sfxSlider.value = AudioSystem.sfx_volume;
		bgmSlider.value = AudioSystem.bgm_volume;
#if UNITY_EDITOR
        /*if (GameState.me == null)
        {
            GameState.nextScene = "map";
            MainNavigationController.GoToStartMenu(false);
            return;
        }*/
#endif          
        _instance = this;        

        GameObject bgm = GameObject.Find("Music");
        if (bgm != null)
        {
            Destroy(bgm);
        }
    }
    [Header("Settings UI")]
	public Slider sfxSlider;
	public Slider bgmSlider;

	public void changeSFX(float value)
	{
		AudioSystem.set_sfx_volume(sfxSlider.value);
	}
	public void changeBGM(float value)
	{
		AudioSystem.set_bgm_volume(bgmSlider.value);
	}

	bool zeroNeeded;
	string GetTime()
	{
		hour = System.DateTime.Now.Hour;
		min = System.DateTime.Now.Minute;
		if (hour > 12)
		{
			hour -= 12;
			isMorning = false;
		}
		else
			isMorning = true;

		if (min < 10)
			zeroNeeded = true;
		else
			zeroNeeded = false;
		
		if (isMorning && zeroNeeded)
			return hour.ToString() + ":0" + min.ToString() + "AM";
		else if( isMorning && !zeroNeeded)
			return hour.ToString() + ":" + min.ToString() + "AM";
		else if (!isMorning && zeroNeeded)
			return hour.ToString() + ":0" + min.ToString() + "PM";
		else
			return hour.ToString() + ":" + min.ToString() + "PM";
	}
    public void GoToKraken()
    {
        if (!Player.hasseenseven && Constants.uses_cinematics)
        {
            Player.hasseenseven = true;
            CutsceneDialogueController.isPartTwo = false;
            MainNavigationController.GoToCinematic("Cinematics_chpt7"); 
        }
        else {

            //Set up game system
            GameSys.GameState gameState = new GameSys.GameState();

            gameState.Name = "fvt";
            gameState.startLevel = 1;//PlayerPrefs.GetInt("TakoLevel")+1;
            gameState.endLevel = 100;
            //gameState.LoadFvtData();
            gameState.loadScene = false;

            GameSys.StateManager.Instance.AddFront(gameState);
            MainNavigationController.DoAssetBundleLoadLevel(Constants.FVT_SCENES, "FvtMainMenu");
        }
    }
	DateTime currentDate;
	DateTime oldDate;
	float hungerLostSinceAway;

    //public GameObject pusher;
    bool isStarted = false;
    // Use this for initialization
    void Start()
    {
        //GameObject b = Instantiate<GameObject>(pusher);
        Debug.LogError("Before Deduction: " + PlayerPrefs.GetFloat("BuddyHunger"));
		currentDate = System.DateTime.Now;
		long temp = Convert.ToInt64(PlayerPrefs.GetString("exitTime"));
		oldDate = DateTime.FromBinary(temp);
		TimeSpan difference = currentDate.Subtract(oldDate);
		hungerLostSinceAway = (float)(difference.TotalSeconds * MatchScoreBar.ScoreReductionMultiplier());
		PlayerPrefs.SetFloat("BuddyHunger", PlayerPrefs.GetFloat("BuddyHunger") - hungerLostSinceAway);
		if (PlayerPrefs.GetFloat("BuddyHunger") < 0)
			PlayerPrefs.SetFloat("BuddyHunger", 0);
		Debug.LogError("Difference: " + difference.TotalSeconds);
        gameObject.GetComponent<FlashScript>().CaptureScreenshot("BuddyHomeSS", 1);

        Debug.LogError("HungerLost: " + hungerLostSinceAway);
        
        Debug.LogError("Deducted buddyhunger: " + PlayerPrefs.GetFloat("BuddyHunger"));
   
        SetupUI();
		// For obtaining ids
		//       WWWForm form = new WWWForm();
		//       form.AddField("user_id", GameState.me.id);
		//       AppServer.CreatePost("me/user/getlevelsunlocked", form)
		//       .Subscribe(
		//           x => SuccessGetIDs(x), // onSuccess
		//           ex => AppServer.ErrorResponse(ex, "Error Update Password") // onError
		//        );

		SubMenu.SetActive(false);
        YoutubeMenu.SetActive(false);

		//if (PlayerPrefs.GetInt("FromChomp") == 1)
		//{
		//	playAnimation = true;
		//	int hunger = (int)(PlayerPrefs.GetFloat("BuddyHunger") / 10);
		//	hungerPanel.SetActive(true);
		//	hungerText.text = hunger.ToString() + "%";
		//	PlayerPrefs.SetFloat("BuddyHunger", (PlayerPrefs.GetFloat("BuddyHunger") + 200));
		//	if (PlayerPrefs.GetFloat("BuddyHunger") > 1000)
		//		PlayerPrefs.SetFloat("BuddyHunger", 1000);
		//	int afterHunger = (int)(PlayerPrefs.GetFloat("BuddyHunger") / 10);
		//	DOTween.Sequence()
		//		.AppendInterval(1f)
		//		.AppendCallback(() =>
		//		{
		//			DOTween.To(() => hunger, x => hunger = x, afterHunger, 1).OnUpdate(() => hungerText.text = hunger.ToString() + "%");
		//		})
		//		.AppendInterval(5f)
		//		.AppendCallback(() =>
		//		{
		//			hungerPanel.SetActive(false);
		//			PlayerPrefs.SetInt("FromChomp", 0);
		//		});
		//}
		if (PlayerPrefs.GetInt("isChildKicked") == 1)
		{
			//PushNotificationPopupDisplay.ShowMessage(PushNotificationPopupDisplay.Type.KickChild);
			PlayerPrefs.SetInt("isChildKicked", 0);
		}
		if (PlayerPrefs.GetInt("isRoomClosed") == 1)
		{
			//PushNotificationPopupDisplay.ShowMessage(PushNotificationPopupDisplay.Type.CloseRoom);
			PlayerPrefs.SetInt("isRoomClosed", 0);
		}

        //Hack for G8
        //interactableButtons[interactableButtons.Length - 1] = null;
    }
    
    void SetupUI()
    {
        MultiLanguage.getInstance().apply(SetPasswordCanvas.FindChild("NewPasswordTitleText"), "map_new_set_password_title");
        MultiLanguage.getInstance().apply(SetPasswordCanvas.FindChild("PasswordPlaceholderText"), "map_new_set_password_text");
        MultiLanguage.getInstance().apply(SetPasswordCanvas.FindChild("PasswordConfirmPlaceholderText"), "map_new_set_password_confirm_text");

        MultiLanguage.getInstance().apply(ExitCanvas.FindChild("GameObject").FindChild("TextPanel").FindChild("Text"), "buddy_home_exit_panel_text");
        
        //Change other script languages too.
        //m_ChatPanel.SetupUI();
        //buildCanvas.GetComponent<BuildUserInterface>().ChangeLanguage();
    }

    public void SwitchLanguage(string language)
    {
        MultiLanguage.getInstance().setLanguage(language);
        SetupUI();
    }

    void Update()
    {
        childCodeDisplayText.text = GameState.me.ChildCode;
		ReduceBuddyHunger();
		//if (playAnimation)
		//{
		//	if (buddyGO.transform.GetChild(0).GetComponent<Animator>() != null)
		//	{
		//		buddyGO.transform.GetChild(0).GetComponent<Animator>().CrossFade("LoveCycle_State", 0.15f);
		//		playAnimation = false;
		//	}
		//}
		timeText.text = GetTime();
		if (!isStarted)
        {
            isStarted = true;
            stages = GameState.configs["Stage"] as Hashtable;
            if (stages != null)
            {
                Debug.Log(stages.Count + " stages total");
                HighestAchievementStorage = new Dictionary<string, int>();
                HighestAchievementStorage.Add(ReefWalkName, 0);
                HighestAchievementStorage.Add(ChompChompName, 0);
                HighestAchievementStorage.Add(PlonkName, 0);
                HighestAchievementStorage.Add(ClickName, 0);
                HighestAchievementStorage.Add(FojaName, 0);

                FindHighestCompleted();

                LastLevelIDs = new Dictionary<string, int>();
                LastLevelIDs.Add(ReefWalkName, 0);
                LastLevelIDs.Add(ChompChompName, 0);
                LastLevelIDs.Add(PlonkName, 0);
                LastLevelIDs.Add(ClickName, 0);
                LastLevelIDs.Add(FojaName, 0);

                FindLastLevelOfEachGame();
            }
        }
    }
    public void refreshunlocks()
    {
		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//        form.AddField("user_id", GameState.me.id);
//        AppServer.CreatePost("me/user/getlevelsunlocked", form)
//        .Subscribe(
//            x => SuccessGetIDs(x), // onSuccess
//            ex => AppServer.ErrorResponse(ex, "Error Update Password") // onError
//        );
    }
    public void SuccessGetIDs(string x)
    {
        JSONClass gameunlocklist = JSONNode.Parse(x) as JSONClass;
        for (int i = 1; i <= 6; i++)
        {
            unlockedLevels[i - 1] = gameunlocklist["unlock_game_" + i].AsBool;
        }
    }
    public void TowerButton()
    {

        if (!inFocus)
        {
            MainNavigationController.SetCurrentStageId(5000);
            MainNavigationController.GoToScene("game6_start_menu");
            //print("Tower");
        }
    }

    public void MovieButton()
    {

        if (!inFocus)
        {
            MovieScreen.SetActive(true);
        }
    }

    public void AnimButton()
    {

        if (!inFocus)
        {
            AnimScreen.SetActive(true);
        }
    }

    public void CameraButton()
    {
        if (!inFocus)
        {
            inFocus = true;
            SubMenu.SetActive(true);
            SetGameCard(ClickName, 3, 3000);
            PopulateMapNodes();
        }
    }

    public void TumbleTroubleButton()
    {
        if (!Player.hasseensix && Constants.uses_cinematics)
        {
            Player.hasseensix = true;
            CutsceneDialogueController.isPartTwo = false;
            MainNavigationController.GoToCinematic("Cinematics_chpt6");
        }
        else
        {

            //Set up game system
            GameSys.GameState gameState = new GameSys.GameState();

            gameState.Name = "tumble";
            gameState.startLevel = 1;
            gameState.endLevel = 100;
            gameState.loadScene = false;

            GameSys.StateManager.Instance.AddFront(gameState);
            MainNavigationController.DoAssetBundleLoadLevel(Constants.TUMBLE_TROUBLE_SCENES, "TsumTsum Main Screen");
        }
    }

    public void InfiniteCrabBrothersButton()
    {

        if (!inFocus)
        {
            /*inFocus = true;
            SubMenu.SetActive(true);
            SetGameCard(ChompChompName, 1, 1000);
            PopulateMapNodes();*/
            Debug.Log(Player.hasseenfive);
           if (!Player.hasseenfive && Constants.uses_cinematics)
            {
                Player.hasseenfive = true;
                CutsceneDialogueController.isPartTwo = false;
                MainNavigationController.GoToCinematic("Cinematics_chpt5");
            }
            else
            {
                //Set up game system
                GameSys.GameState gameState = new GameSys.GameState();

                gameState.Name = "crab";
                gameState.startLevel = 1;
                gameState.endLevel = 100;
                gameState.loadScene = false;

                GameSys.StateManager.Instance.AddFront(gameState);
                MainNavigationController.DoAssetBundleLoadLevel(Constants.CRAB_BROS_SCENES, "crab_main_screen");
                //MainNavigationController.GoToScene("crab_main_screen");
            }
        }
    }

    public void ShopButton()
    {
        if (!inFocus)
        {
            print("Shop");
        }
    }

    public void HomeButton()
    {
        if (!inFocus)
        {
            inFocus = true;
            LoadHomeScene();
        }
    }

    void LoadHomeScene()
    {
        MainNavigationController.GoToHome();
    }

    public void WarehouseButton()
    {
        if (!Player.hasseenfour && Constants.uses_cinematics)
        {
            Player.hasseenfour = true;
            CutsceneDialogueController.isPartTwo = false;
            MainNavigationController.GoToCinematic("Cinematics_chpt4");
        }else {

            //Set up game system
            GameSys.GameState gameState = new GameSys.GameState();

            gameState.Name = "manta";
            gameState.startLevel = 1;
            gameState.endLevel = 100;
            gameState.loadScene = false;

            GameSys.StateManager.Instance.AddFront(gameState);

            MainNavigationController.DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram Main Menu");
            //MainNavigationController.GoToScene("Tangram Main Menu");
        }
        /*
        if (!inFocus)
        {
            inFocus = true;
            SubMenu.SetActive(true);
            SetGameCard("Buzz", 4, 4000);
            PopulateMapNodes();
        }*/
    }

    public void ReefWalkButton()
    {
        if (!inFocus)
        {
            inFocus = true;
            //SubMenu.SetActive(true);
            //SetGameCard(ReefWalkName, 0, 0);
            //PopulateMapNodes();
            if (!Player.hasseenTwo && Constants.uses_cinematics)
            {
                Player.hasseenTwo = true;
                CutsceneDialogueController.isPartTwo = false;
                MainNavigationController.GoToCinematic("Cinematics_chpt2");
            }else {

                //Set up game system
                GameSys.GameState gameState = new GameSys.GameState();

                gameState.Name = "pearly";
                gameState.startLevel = 1;
                gameState.endLevel = 100;
                gameState.loadScene = false;

                GameSys.StateManager.Instance.AddFront(gameState);
                MainNavigationController.DoAssetBundleLoadLevel(Constants.PEARLY_SCENES, "pearly_main_screen");
            }


        }
    }

    public void GarageButton()
    {
        if (!inFocus)
        {
            print("Garage");
        }
    }

    public void PlonkButton()
    {
        //SceneManager.LoadScene("Autumn/TsumTsum/TsumTsum Main Screen");
        MainNavigationController.GoToTumbleTroubleMainScreen();
        /*
        if (!inFocus)
        {
            inFocus = true;
            SubMenu.SetActive(true);
            SetGameCard(PlonkName, 2, 2000);
            PopulateMapNodes();
        }*/
    }

    public void ExitButton()
    {
        inFocus = false;
        SubMenu.SetActive(false);
    }

    private void SetGameCard(string gameType, int spriteNum, int offsetAmt)
    {
        ResetScrollSprites();
        ResetMapSprites();
        current_level_type = gameType;
        TitleCard.GetComponent<UnityEngine.UI.Text>().text = current_level_type;
        GameLogoType.GetComponent<UnityEngine.UI.Image>().sprite = GameLogoSpriteList[spriteNum];
        LevelOffset = offsetAmt;
        SetLevelNodes();
    }

    public void SetLevelNodes()
    {
        // Set the numbering of each map nodes
        for (int i = 0; i < NumMapNodes; ++i)
        {
            int initialOffset = CurrentPage * NumMapNodes;
            MapNodes[i].GetComponentInChildren<UnityEngine.UI.Text>().text = (initialOffset + i + 1).ToString();
        }
    }

    public void PopulateMapNodes()
    {
        int tmpStars = 0;
        Debug.Log("Populating Nodes");
        for (int i = 0; i < NumMapNodes; ++i)
        {
            int j = LevelOffset + (CurrentPage * NumMapNodes) + i + 1;
            if (stages[j] != null && (IsUnlocked(j) || IsStartLevel(j)))
            {
                MapNodes[i].GetComponent<MapNodeController>().stageid = j;
                MapNodes[i].GetComponent<MapNodeController>().stage = stages[j] as Stage;

                LoadScreenshots(i, j);

                //Debug.Log("i : " + i);

                if (GameState.me.achievements.ContainsKey(j))
                {
                    // Depending on stars, choose the right display
                    if (GameState.me.achievements[j].progress >= (stages[j] as Stage).star3ScoreNeeded)
                    {
                        tmpStars = 3;
                    }

                    else if (GameState.me.achievements[j].progress >= (stages[j] as Stage).star2ScoreNeeded)
                    {
                        tmpStars = 2;
                    }

                    else if (GameState.me.achievements[j].progress >= (stages[j] as Stage).star1ScoreNeeded)
                    {
                        tmpStars = 1;
                    }
                }

                else
                {
                    tmpStars = 0;
                }

                MapNodes[i].GetComponentInChildren<UnityEngine.UI.Image>().sprite = MapNodes[i].GetComponent<MapNodeController>().starSprites[tmpStars];
            }
        }
    }

    public void ResetMapSprites()
    {
        for (int i = 0; i < NumMapNodes; ++i)
        {
            MapNodes[i].GetComponentInChildren<MapNodeController>().stageid = 0;
            MapNodes[i].GetComponentInChildren<UnityEngine.UI.Image>().sprite = MapNodes[i].GetComponentInChildren<MapNodeController>().starSprites[4];
        }
    }

    public void ManageButtonScrolls()
    {
        for (int i = 0; i < ScrollButton.Length; ++i)
        {
            if (CurrentPage == i)
                ScrollButton[i].GetComponentInChildren<UnityEngine.UI.Image>().sprite = ScrollButton[i].GetComponentInChildren<MapScrollButton>().ButtonType[1];
            else
                ScrollButton[i].GetComponentInChildren<UnityEngine.UI.Image>().sprite = ScrollButton[i].GetComponentInChildren<MapScrollButton>().ButtonType[0];
        }
    }

    public void ResetScrollSprites()
    {
        CurrentPage = 0;

        for (int i = 0; i < ScrollButton.Length; ++i)
        {
            if (i == 0)
                ScrollButton[i].GetComponentInChildren<UnityEngine.UI.Image>().sprite = ScrollButton[i].GetComponentInChildren<MapScrollButton>().ButtonType[1];
            else
                ScrollButton[i].GetComponentInChildren<UnityEngine.UI.Image>().sprite = ScrollButton[i].GetComponentInChildren<MapScrollButton>().ButtonType[0];
        }
    }

    public void FindHighestCompleted()
    {
        foreach (KeyValuePair<int, PlayerAchievement> pa in GameState.me.achievements)
        {
            int tmpAchieve = pa.Value.achievementId;

            if (tmpAchieve <= 1000)
            {
                if (tmpAchieve > HighestAchievementStorage[ReefWalkName])
                {
                    // Store the highest for Reefwalk
                    HighestAchievementStorage[ReefWalkName] = tmpAchieve;
                }
            }

            else if (tmpAchieve <= 2000)
            {
                if (tmpAchieve > HighestAchievementStorage[ChompChompName])
                {
                    // Store the highest for ChompChomp
                    HighestAchievementStorage[ChompChompName] = tmpAchieve;
                }
            }

            else if (tmpAchieve <= 3000)
            {
                if (tmpAchieve > HighestAchievementStorage[PlonkName])
                {
                    // Store the highest for Plonk
                    HighestAchievementStorage[PlonkName] = tmpAchieve;
                }
            }

            else if (tmpAchieve <= 4000)
            {
                if (tmpAchieve > HighestAchievementStorage[ClickName])
                {
                    // Store the highest for Click
                    HighestAchievementStorage[ClickName] = tmpAchieve;
                }
            }

            else if (tmpAchieve <= 5000)
            {
                if (tmpAchieve > HighestAchievementStorage[FojaName])
                {
                    // Store the highest for Foja
                    HighestAchievementStorage[FojaName] = tmpAchieve;
                }
            }
        }
    }

    private bool IsUnlocked(int id)
    {
        // "Probably switch to switch cases, but I am too damn lazy for this shit." - Poh Peng 2016 || Kelvin 2016
        int tmpID = id / 1000;

        // Reef Walk
        if (tmpID == 0)
        {
            if (id - 1 <= HighestAchievementStorage[ReefWalkName])
                return true;
            else
                return false;
        }

        // Chomp
        if (tmpID == 1)
        {
            if (id - 1 <= HighestAchievementStorage[ChompChompName])
                return true;
            else
                return false;
        }

        // Plonk
        if (tmpID == 2)
        {
            if (id - 1 <= HighestAchievementStorage[PlonkName])
                return true;
            else
                return false;
        }

        // Click
        if (tmpID == 3)
        {
            if (id - 1 <= HighestAchievementStorage[ClickName])
                return true;
            else
                return false;
        }

        // Foja
        if (tmpID == 4)
        {
            if (id - 1 <= HighestAchievementStorage[FojaName])
                return true;
            else
                return false;
        }

        // All else fails, unlock the level, should be relevant to spot the anomaly
        return true;
    }

    private bool IsStartLevel(int id)
    {
        if ((id % 1000) == 1)
            return true;
        else
            return false;
    }

    public void AllNamesAndID()
    {
        foreach (DictionaryEntry i in stages)
        {
            Stage tmp = i.Value as Stage;

            print(tmp.achievementId + " " + tmp.name);
        }
    }

    private void LoadScreenshots(int screenshotElement, int screenshotNum)
    {
        if (screenshotNum > 1000 && screenshotNum < 2000)
            Screenshots[screenshotElement].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("GameSS/" + 1001);

        else
            Screenshots[screenshotElement].GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("GameSS/" + screenshotNum);
    }

    private void FindLastLevelOfEachGame()
    {
        foreach (DictionaryEntry i in stages)
        {
            Stage tmp = i.Value as Stage;

            // Find the highest level of all
            if (tmp.achievementId < 1000 && tmp.achievementId > LastLevelIDs[ReefWalkName])
            {
                LastLevelIDs[ReefWalkName] = tmp.achievementId;
            }

            if (tmp.achievementId > 1000 && tmp.achievementId < 2000 && tmp.achievementId > LastLevelIDs[ChompChompName])
            {
                LastLevelIDs[ChompChompName] = tmp.achievementId;
            }

            if (tmp.achievementId > 2000 && tmp.achievementId < 3000 && tmp.achievementId > LastLevelIDs[PlonkName])
            {
                LastLevelIDs[PlonkName] = tmp.achievementId;
            }

            if (tmp.achievementId > 3000 && tmp.achievementId < 4000 && tmp.achievementId > LastLevelIDs[ClickName])
            {
                LastLevelIDs[ClickName] = tmp.achievementId;
            }

            if (tmp.achievementId > 4000 && tmp.achievementId > LastLevelIDs[FojaName])
            {
                LastLevelIDs[FojaName] = tmp.achievementId;
            }
        }
    }

    private bool CheckTowerActive()
    {
        // Requires at least one of each level for this to work
        if (LastLevelIDs[ReefWalkName] == HighestAchievementStorage[ReefWalkName] &&
            LastLevelIDs[ChompChompName] == HighestAchievementStorage[ChompChompName] &&
            LastLevelIDs[PlonkName] == HighestAchievementStorage[PlonkName] &&
            LastLevelIDs[ClickName] == HighestAchievementStorage[ClickName] &&
            LastLevelIDs[FojaName] == HighestAchievementStorage[FojaName])
            return true;
        return false;
    }

	public WalkingAvatar wa;

    public void InvertBuddyClickable()
    {
		//wa.isClickable = !wa.isClickable;
		WalkingAvatar.isClickable = !WalkingAvatar.isClickable;
    }

    public GameObject[] interactableButtons;
    public void InvertButtonInteractable()
    {

        for (int i = 0; i < interactableButtons.Length; i++)
        {
            //interactableButtons[i].active = !interactableButtons[i].GetActive();
            if (interactableButtons[i] == null)
                continue;

            interactableButtons[i].SetActive(!interactableButtons[i].GetActive());
        }
        interactableButtons[5].SetActive(false); //temp code to disable buddy chat
    }

	public void GoToFreeze()
	{
		MainNavigationController.GoToFreeze();
	}

    public void GoToMainMenu()
    {
        MainNavigationController.GotoMainMenu();
    }

    public void UnlockButtonClick()
	{
		GameState.UnlockAll();
	}

	public void ResetButtonClick()
	{
        GameState.me.ResetEmpty();
		PlayerPrefs.SetInt("NeedsLogout", 1);
		MainNavigationController.GoToStartMenu();
	}

	void ReduceBuddyHunger()
	{
		if(PlayerPrefs.GetFloat("BuddyHunger") > 0)
			PlayerPrefs.SetFloat("BuddyHunger", PlayerPrefs.GetFloat("BuddyHunger") - Time.deltaTime * MatchScoreBar.ScoreReductionMultiplier());
		else
			PlayerPrefs.SetFloat("BuddyHunger", 0);
	}
}
