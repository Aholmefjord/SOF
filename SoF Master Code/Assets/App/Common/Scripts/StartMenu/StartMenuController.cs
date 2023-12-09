using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UniRx;
using AutumnInteractive.SimpleJSON;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading;
using System.Net;
using System.Text;
using JULESTech;

public class StartMenuController : MonoBehaviour
{
    public GameObject passwordCinematic;

    public string DEFAULT_PASSWORD;
    public string MASTER_PASSWORD;
    public GameObject PasswordChangePanel;
	public static StartMenuController current;
	public Text versionLabel;
	public GameObject accountMainPanel;
	public GameObject accountPanelsHolder;
	public GameObject loginPanel;
	public GameObject loggedInPanel;
	// Login
	public InputField loginEmailText;
	public InputField loginPasswordText;
	public Button loginBtn;
    public static string textToShow;

	// Sign up Create

    public Button guideBtn;
	public Button startBtn;

	public Button loginTabButton;
	public Sprite disabledTabImage;
	public Sprite enabledTabImage;

    public GameObject loadingText;

	public static PreloadStatus status = PreloadStatus.Init;
	private bool initPlayerData = false;

    public GameObject MediaPanel;

    public GameObject IQPanel;

    private Boolean isDownloadingUpdate;
    private Boolean isDownloadError;
    private Boolean updateDetected;
    private Boolean isServerError;

    public GameObject installButton;

    public GameObject ConnectionErrorPanel;
    public GameObject LoginGO;
    
    public Boolean isFindingJULESBox = false;
	private bool isHashedPassword = false;

    public void EnableMedia()
    {
        MediaPanel.SetActive(true);
    }
    public bool deletePrefs = false;
	public enum PreloadStatus
	{
		Init = 0,
		VersionChecked = 1,
		ConfigChecked = 2,
		PlayerMe = 3,
		PlayerData = 4,
		Ready = 5
	}

	public SOFNetworkStatus netStatus = SOFNetworkStatus.OFFLINE;

	private string register; // store one time unique registration key for the user

	void Start()
	{
        Screen.fullScreen = true;
        CutsceneDialogueController.isPartTwo = false;

        GameState.platformToken = PlayerPrefs.GetString("jules_token", "");	
		SetInit();
        loadLanauge();
        SetupUIText();

        /*
		if (push_notifications._instance != null)
		{
			Destroy(push_notifications._instance.gameObject);
		}
        //*/

		// Starting Point for Network Initialization
		NetworkInit();
    }

	void Awake()
	{
        isServerError = false;
        current = this;
        loadLanauge();
        SetupUIText();
        //guideBtn.gameObject.SetActive(false);
        startBtn.gameObject.SetActive(false);
		loginPanel.SetActive(false);
		accountMainPanel.SetActive(false);

        AudioSystem.PlayBGM("bgm_ms_summer-carousel-loop");
	}
		
	public void NetworkInit()
	{
		// Needs work here...
		if ((netStatus & SOFNetworkStatus.HAS_CONNECTION) != 0)
		{
			// All network setup done
			OnNetworkInitComplete();
			return;
		}
		ConnectToServer(Constants.AppURL);
	}

	public void OnNetworkInitComplete()
	{
		LoginGO.SetActive(true);
		ConnectionErrorPanel.SetActive(false);
		UpdateConnectionErrorMessage("main_menu_check_update_text");

		LoginGO.FindChild("LoadingDots").SetActive(false);
		textToShow = MultiLanguage.getInstance().getString("main_menu_logging_in");
		ShowAccountButtons();
		loginBtn.interactable = true;

		if (PlayerPrefs.HasKey("CachedPlayerUsername") && PlayerPrefs.HasKey("CachedPlayerAccess"))
		{
			loginEmailText.text = PlayerPrefs.GetString("CachedPlayerUsername");
			loginPasswordText.text = PlayerPrefs.GetString("CachedPlayerAccess");
			isHashedPassword = true;
			LoginButtonClick();
		}
		else
		{
			isHashedPassword = false;
		}

        GameSceneManager.getInstance().LoadGameCommand();
    }

	public void ConnectToServer(string _appURL)
	{
		JulesNet.Instance.Init(_appURL);
		JulesNet.Instance.SendGETRequest ("SOFGetServerTime.php", 
			(byte[] _msg) => {
				// Received a response
				StringHelper.DebugPrint(_msg);
				string[] data = Encoding.UTF8.GetString (_msg).Split ('\t');
				if (data [0] == "OK") 
				{
					// Success : Store the time difference in the cloud store.
					int serverTime = int.Parse(data[1]);
					int localTime = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
					int timeDiff = serverTime - localTime;
					TimeHelper.SetTimeOffset(timeDiff);

					DataStore.Instance.cloudData.SetInt("ServerTimeDiff", timeDiff);
					netStatus |= SOFNetworkStatus.HAS_CONNECTION;
					//NetworkInit(); 
					CheckForNewVersion();
				} 
				else 
				{
					// Error : Something went wrong
					ConnectionErrorAction ("Invalid Response");
				}
			},
			// Connection Failed
			ConnectionErrorAction);
	}

	public void CheckForNewVersion()
	{
		WWWForm versionForm = new WWWForm();
		versionForm.AddField("version_name", Constants.AppName);
		versionForm.AddField("version_value", Constants.AppVersionValue);
		JulesNet.Instance.SendPOSTRequest("SOFGetVersionData.php", versionForm, 
			(byte[] _msg) => {
				// Received a response
				StringHelper.DebugPrint(_msg);
				string[] data = Encoding.UTF8.GetString (_msg).Split ('\t');
				if (data [0] == "OK") 
				{
					// Proceed with the next phase of logging in
					NetworkInit(); 
				} 
				else if (data[0] == "UPDATE")
				{
					downloadlocation = data[1];

#if UNITY_EDITOR
                    NetworkInit();
#else
                    startUpdate();
#endif
                }
                else 
				{
					// Error : Something went wrong
					ConnectionErrorAction ("Invalid Response");
				}
			},
			// Connection Failed
			ConnectionErrorAction);
	}

	public void ConnectionErrorAction(string _error)
	{
		netStatus = SOFNetworkStatus.OFFLINE;
		LoginGO.SetActive(false);
		UpdateConnectionErrorMessage("main_menu_connection_error_text");
		ConnectionErrorPanel.SetActive(true);
	}

	public void LoginButtonClick()
	{
		PlayerPrefs.DeleteKey("DeviceToken");
		if (loginEmailText.text == "")
		{
			DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_missing_username"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Retry);
			return;
		}
		if (loginPasswordText.text == "")
		{
			DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_missing_password"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Retry);
			return;            
		}

		// Enforce all lower case only
		loginEmailText.text = loginEmailText.text.ToLower();

		loginBtn.interactable = false;
		accountMainPanel.SetActive(false);
		accountPanelsHolder.SetActive(false);
		loadingText.SetActive(true);
		LoginGO.FindChild("LoadingDots").SetActive(true);

		DataStore.Instance.cloudData.SetString("user_name", loginEmailText.text);

		WWWForm loginForm = new WWWForm();
		loginForm.AddField("username", loginEmailText.text);

		if (isHashedPassword)
			loginForm.AddField("password", loginPasswordText.text);
		else
		{
			loginPasswordText.text = loginPasswordText.text.ToLower();
			loginForm.AddField("password", StringHelper.GetMD5Hash(loginPasswordText.text));
		}
		
		JulesNet.Instance.SendPOSTRequest("SOFUsernameAccessLogin.php", loginForm,
			(byte[] _msg) => {
				// Received a response
				StringHelper.DebugPrint(_msg);
				string[] data = Encoding.UTF8.GetString (_msg).Split ('\t');

				if (data[0] == "OK")
				{
					// This is to force the user to change their password
					if (loginPasswordText.text == Constants.DEFAULT_PASSWORD)
					{
						loadingText.SetActive(false);
						passwordCinematic.SetActive(true);
						return;
					}

					PlayerPrefs.SetString("CachedPlayerUsername", loginEmailText.text);
					if (isHashedPassword)
						PlayerPrefs.SetString("CachedPlayerAccess", loginPasswordText.text);
					else
						PlayerPrefs.SetString("CachedPlayerAccess", StringHelper.GetMD5Hash(loginPasswordText.text));
					
					// Success
					AnalyticsSys.Instance.Init("SOFReportAnalytics.php", data[1]);
					GameState.me = new Player();
					DataStore.Instance.cloudData.SetString("account_id", data[1]);
					AnalyticsSys.Instance.Report(REPORTING_TYPE.LoginResult, "Success");
					DownloadUserCloudData();
					//MainNavigationController.GotoMainMenu(); // Testing
				}
				else
				{
					// Failed
					LoginFailed(data[1]);
				}
			},
			(string _error) => {
				// Failed to send
				LoginFailed(_error);
			}
		);
	}

	public void DownloadUserCloudData()
	{
		// Get the account ID from the data store
		string accountID = DataStore.Instance.cloudData.GetString("account_id");

		// Send the request to download the data
		WWWForm loginForm = new WWWForm();
		loginForm.AddField("account_id", accountID);
		JulesNet.Instance.SendPOSTRequest("SOFGetPlayerSave.php", loginForm,
			(byte[] _msg) => {
				// Received a response
				StringHelper.DebugPrint(_msg);
				string[] data = Encoding.UTF8.GetString (_msg).Split ('\t');

				if (data[0] == "OK")
				{
					// Success, time to setup the game
					if (ProcessGameData(data[1]))
					{
						OnSaveDataProcessed();
					}
					else
						LoginFailed("Data Corrupted");
				}
				else
				{
					// Failed
					LoginFailed(data[1]);
				}
			},
			(string _error) => {
				// Failed to send
				LoginFailed(_error);
			}
		);
			
		/*
		string res = "test"; // This is input from server
		JSONNode jsonPlayerData = JSONNode.Parse(res) as JSONNode;
		JSONNode user = jsonPlayerData["User"] as JSONNode;
		if(user != null)
		{
			GameState.me = new Player();
			GameState.me.init(user);
			Debug.Log("Loaded player buddy information");
		}
		*/
	}

	private void OnSaveDataProcessed()
	{
		loadingText.SetActive(false);
		LoginGO.FindChild("LoadingDots").SetActive(false);

		if (GameState.me.iqTestOneResults == "{}" || GameState.me.iqTestOneResults == String.Empty || GameState.me.iqTestOneResults == "")
		{
			IQPanel.SetActive(true);
		}
		else if (
			GameState.me.avatar == null ||
			GameState.me.avatar.avatar == null ||
			GameState.me.avatar.avatar.prefab == null)
		{
			CutsceneDialogueController.isPartTwo = false;
			CutsceneDialogueController.needsBuddy = false;
			MainNavigationController.GoToScene("create_pet");
			return;
		} else if (!Player.hasseenone && Constants.uses_cinematics) {
			Player.hasseenone = true;

			GameState.me.Upload();
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
			CutsceneDialogueController.isPartTwo = true;
			CutsceneDialogueController.needsBuddy = true;

			MainNavigationController.GoToCinematic("Cinematics_chpt1");
#elif UNITY_IOS
			CutsceneDialogueController.needsBuddy = true;
			MainNavigationController.GoToHome();
#endif
		}
		else
		{
			CutsceneDialogueController.needsBuddy = true;
			MainNavigationController.GotoMainMenu();
		}
	}

	private bool ProcessGameData(string _input)
	{
		AnalyticsSys.Instance.IsDisabled = true;

		string data = StringHelper.Base64Decode(_input);

		JSONNode gameData = JSONNode.Parse(data);
		if (gameData != null)
		{
			GameState.me = new Player();
			GameState.me.init(gameData);
		}
		else
		{
			AnalyticsSys.Instance.IsDisabled = false;
			return false;
		}

		GameState.me.GetData(gameData);
		GameState.me.GetGameSaveData(gameData);

		AnalyticsSys.Instance.IsDisabled = false;
		return true;
	}

    public void CheckForUpdate()
    {
		Debug.Log ("CHECK FOR UPDATE CALLED");
        //Check if we have internet.

		// HOWARD - Deprecated network code - TO REPLACE
//        if (!JulesBox.HAS_FOUND_SERVER && !CheckInternetConnection())
//        {
//            LoginGO.SetActive(false);
//            UpdateConnectionErrorMessage("main_menu_connection_error_text");
//            ConnectionErrorPanel.SetActive(true);
//            return;
//        }
        
        LoginGO.SetActive(true);
        ConnectionErrorPanel.SetActive(false);
        UpdateConnectionErrorMessage("main_menu_check_update_text");
        //TODO version number check

		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//
//        #if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
//        Debug.Log("Version Number: " + Application.version.Replace("sof", ""));
//            form.AddField("versionNumber", Application.version.Replace("sof", ""));
//            
//            AppServer.CreateUpdateServerPost(JulesBox.UpdateApi, form).Subscribe(x => DoUpdate(x), ex => ServerError(ex.Message));
//        #elif UNITY_IOS
//            AccessTokenAutoLogin();
//        #endif


    }

    public void RetryConnection()
    {
		ConnectionErrorPanel.SetActive(false);
		NetworkInit();
    }

    private bool CheckInternetConnection(string _url)
    {
		Debug.Log ("CHECK INTERNET CONNECTION CALLED");
		// HOWARD - Disabled anything regarding JulesBox for now
        //if (Constants.AppURL == "http://192.168.42.1:8080/") return true;

        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(_url);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
				else
				{
					return false;
				}
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

    public void UpdateConnectionErrorMessage(string message)
    {
        ConnectionErrorPanel.FindChild("Message").GetComponent<Text>().text = MultiLanguage.getInstance().getString(message);
    }

    private void ServerError(string message)
    {
        //Update server error
        //We will let the user to continue play anyway.
        AccessTokenAutoLogin();
    }

    string downloadlocation = "";
    private void DoUpdate(string response)
    {
		Debug.Log ("DO UPDATE CALLED");
        Debug.Log(response);

        JSONNode n = JSONNode.Parse(response);
        if (!n.GetEntry<string>("received_status", "{null}").Equals("{null}") || n.GetEntry<string>("check_status", "").Equals("latest"))
        {
            updateDetected = false;
            isDownloadingUpdate = false;
            hasDownloaded = false;

            updateDetected = false;
        }
        else
        {
            updateDetected = true;
            downloadlocation = n.GetEntry("version_location", "");
        }

        //Procced to login, update button will appear in login panel
        AccessTokenAutoLogin();

    }

    public void startUpdate()
    {
		Debug.Log ("START UPDATE CALLED");
        accountMainPanel.SetActive(false);
        loggedInPanel.SetActive(false);
        loadingText.SetActive(true);

        isDownloadingUpdate = true;
        
        WebClient client = new WebClient();

        client.DownloadProgressChanged += (s, e) =>
        {
            progress = e.ProgressPercentage;
        };

        Debug.Log("Downloading From: " + downloadlocation);
        textToShow = "Downloading new update: ";

        client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadFileCompleted);
        client.DownloadFileAsync(new Uri(downloadlocation), Application.persistentDataPath + "/" + "sof_update.apk");
    }

    bool hasDownloaded = false;
    public void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        installButton.SetActive(true);

        if (e.Error == null)
        {
            textToShow = MultiLanguage.getInstance().getString("main_menu_download_complete");
            hasDownloaded = true;

            openDowloadedAPK();
        }
        else
        {
            Debug.LogError(e.Error);
            //textToShow = "Download Error. \nPlease close the app and retry.";

            UpdateConnectionErrorMessage("main_menu_connection_error_text");
            LoginGO.SetActive(false);
            ConnectionErrorPanel.SetActive(true);
        }
    }

    public void SelectTab(string tab)
	{
		accountPanelsHolder.SetActive(true);
        if (tab == "login")
		{
            if (PlayerPrefs.GetInt("NeedsLogout") == 1)
            {
                Debug.Log("Logging out From Tab");
                PlayerPrefs.SetInt("NeedsLogout", 0);
                loadingText.SetActive(false);
                this.LogoutButtonClick();
            }
            else
            {
                loginPanel.SetActive(true);
            }

            accountMainPanel.SetActive(false);
        }
	}

	public void StartGameButtonClick()
	{
		NetworkInit();
    }
	private void SetInit()
	{
		enabled = true;
		status = PreloadStatus.Init;
        versionLabel.text = "v " + Application.version;
	}

    private void loadLanauge()
    {
        //Calling the multilanguage instance will call it's init function too.
        MultiLanguage.getInstance();
    }

    private void SetupUIText()
    {
        MultiLanguage.getInstance().apply(loginPanel.FindChild("TitleText"), "main_menu_login_panel_title");
        MultiLanguage.getInstance().apply(loginPanel.FindChild("InstructionsText"), "main_menu_login_panel_login_to_continue");
        MultiLanguage.getInstance().apply(loginPanel.FindChild("InputEmailFieldPlaceholder"), "main_menu_login_panel_username_placeholder");
        MultiLanguage.getInstance().apply(loginPanel.FindChild("InputPasswordFieldPlaceholder"), "main_menu_login_panel_password_placeholder");

        MultiLanguage.getInstance().apply(loadingText, "main_menu_loading");

        MultiLanguage.getInstance().apply(accountMainPanel.FindChild("GuideText"), "main_menu_guide");
        MultiLanguage.getInstance().apply(loggedInPanel.FindChild("UpdateText"), "main_menu_update");
        MultiLanguage.getInstance().apply(loggedInPanel.FindChild("DownloadText"), "main_menu_download");
        
        MultiLanguage.getInstance().apply(ConnectionErrorPanel.FindChild("Title"), "main_menu_connection_error_title");
        MultiLanguage.getInstance().apply(ConnectionErrorPanel.FindChild("Message"), "main_menu_connection_error_text");
        MultiLanguage.getInstance().apply(ConnectionErrorPanel.FindChild("RetryText"), "main_menu_connection_error_retry");

        //Go button and Login button
        MultiLanguage.getInstance().applyImage(loginPanel.FindChild("Login Button").GetComponent<Image>(), "gui_login_button");
        MultiLanguage.getInstance().applyImage(accountMainPanel.FindChild("Login Tab Button").GetComponent<Image>(), "gui_login_button");
        MultiLanguage.getInstance().applyImage(accountPanelsHolder.FindChild("Login Button").GetComponent<Image>(), "gui_login_button");
        MultiLanguage.getInstance().applyImage(loggedInPanel.FindChild("Start Button").FindChild("Image").GetComponent<Image>(), "gui_go_button");

        textToShow = MultiLanguage.getInstance().getString("main_menu_logging_in");
    }


	public void AccessTokenAutoLogin()
	{
		Debug.Log ("ACCESS TOKEN AUTO LOGIN CALLED");
        Debug.Log("Now accessing token login");
        this.isFindingJULESBox = false;
        LoginGO.FindChild("LoadingDots").SetActive(false);

        textToShow = MultiLanguage.getInstance().getString("main_menu_logging_in");
        GameState.julesAccessToken = PlayerPrefs.GetString("jules_access_token", null);
        if (GameState.julesAccessToken == null || GameState.julesAccessToken == "")
		{
			ShowAccountButtons();
			return;
		}

		status = PreloadStatus.PlayerMe;
		GetUser();
	}

	void GetUser()
	{
		
//		WWWForm form = new WWWForm();
//		form.AddField("token", GameState.julesAccessToken);
//		AppServer.CreatePost("me", form, null, true)
//		.Subscribe(
//			x => ReceiveUser(x), // onSuccess
//			ex => AppServer.ErrorResponse(ex, "Error Get Player Session") // onError
//		);
	}

	void ReceiveUser(string res)
	{
        if (status != PreloadStatus.PlayerMe)
		{
			return; // not correct sequence - might be reloaded
		}

		// HOWARD - Deprecated network code - TO CHECK
		//JSONNode t = AppServer.HasError(res);
		JSONNode t = JSONNode.Parse(res);
		if (t == null)
		{
			return;
		}
		status = PreloadStatus.PlayerMe;
		initPlayerData = true;
		GetPlayerData();
	}

	void GetPlayerData()
	{
		if (initPlayerData != true) return;

		// HOWARD - Deprecated network code - TO REPLACE
//		WWWForm form = new WWWForm();
//		form.AddField("token", GameState.julesAccessToken);
//        accountPanelsHolder.SetActive(false);
//        loadingText.SetActive(true);
//		AppServer.CreatePost("me/data", form, null, true)
//		.Subscribe(
//			xs => ReceivePlayerData(xs),
//			ex => AppServer.ErrorResponse(ex, "Error Get Player Data")
//		);


    }

	void ReceivePlayerData(string res)
	{
		if (status != PreloadStatus.PlayerMe)
		{
#if UNITY_EDITOR
#endif
			return; // not correct sequence - might be reloaded
		}
		status = PreloadStatus.PlayerData;
		UpdateUserLastLogin();
		if (SetupPlayerData(res) == true)
		{
            Debug.Log("Login Successful");
			// HOWARD - Deprecated network code - TO REPLACE
//            JulesBox.Authenticate();
//            WWWForm form1 = new WWWForm();  
//            form1.AddField("user_id", GameState.me.id);
//            AppServer.CreatePost("me/gamedata/get", form1, null, true)
//            .Subscribe(
//                x => ReceivePlayerGameData(x), // onSuccess
//                ex => ReceivePlayerGameData(ex.ToString()) // onError
//            );
        }
        
	}
    void ReceivePlayerGameData(string res)
    {
        if (PlayerPrefs.GetInt("NeedsLogout") == 1)
        {
            Debug.Log("Logging out from receiving receive player data");
            PlayerPrefs.SetInt("NeedsLogout", 0);
            this.LogoutButtonClick();
            loadingText.SetActive(false);
            loginPanel.SetActive(true);
            loginBtn.gameObject.SetActive(true);
            return;
        }
        Debug.Log("Receiving Player Game Data");
        if (AcquirePlayerData(res) == true)
        {
           
        }

    }
	public static bool SetupPlayerData(string res)
	{
		JSONNode jsonPlayerData = JSONNode.Parse(res) as JSONNode;
		Hashtable configAvatar = GameState.configs["Avatar"] as Hashtable;
		JSONArray jsonAvatar = jsonPlayerData["Avatars"] as JSONArray;
        JSONNode user = jsonPlayerData["User"] as JSONNode;
        if(user != null)
        {
            GameState.me = new Player();
            GameState.me.init(user);
            Debug.Log("Loaded player buddy information");
        }        
        jsonPlayerData = null;
		res = null;
        return true;
	}

    public static bool AcquirePlayerData(string res)
    {
        GameState.me.GetData(res);
		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form1 = new WWWForm();
//       form1.AddField("user_id", GameState.me.id);
//       AppServer.CreatePost("me/savedata", form1)
//       .Subscribe(
//           x => unzipString(x), // onSuccess
//           ex => Debug.Log(ex.ToString()) // onError
//       );f
        return true;
    }
    static string output = "";
    public static float fakeProgressTime = 0.0f;
    public static int progress = 0;
    public static bool finished = false;
    public static void unzipString(string cs)
    {
        try
        {
            finished = false;
            progress = 0;
            JSONClass c = JSONClass.Parse<JSONClass>(cs);
            string s = c["data"];
            byte[] arr = stringtobytearray(s).ToArray();
            output = UnZip(arr);
            if (PlayerPrefs.HasKey(GameState.me.id + "." + GameState.me.username + ".tangram_player_progression"))
            {
                AcquireOfflinePlayerSaveData();
            }
            else
            {
                Thread t = new Thread(new ThreadStart(SetupPlayerSaveData)); //new ThreadStart is optional for the same reason 
                t.Start();
            }
        }
        catch (System.Exception e)
        {
            AcquireOfflinePlayerSaveData();
        }
    }
    private static void SetupPlayerSaveData()
    {
        AcquirePlayerSaveData(output);
    }

    public static List<byte> stringtobytearray(string s)
    {

        List<byte> res = new List<byte>();
        string sparse = s.TrimStart('[');
        sparse = sparse.TrimEnd(']');
        string[] ss = sparse.Split(',');
        for(int i = 0; i < ss.Length; i++)
        {
            byte b = (byte)int.Parse(ss[i]);
            res.Add(b);
        }
        return res;
    }
    public static string UnZip(byte[] byteArray)
    {

        //Prepare for decompress
        using (var stream = new System.IO.MemoryStream(byteArray))
        using (var gzip = new Ionic.Zlib.GZipStream(stream, Ionic.Zlib.CompressionMode.Decompress))
        using (var sr = new System.IO.StreamReader(gzip, System.Text.Encoding.UTF8))
        {
            return sr.ReadToEnd();
        }
    }
    public static bool AcquirePlayerSaveData(string res)
    {

        textToShow = MultiLanguage.getInstance().getString("main_menu_downloading_game_data");
        GameState.me.GetGameSaveData(res);
        return true;
    }
    public static bool AcquireOfflinePlayerSaveData()
    {
        textToShow = MultiLanguage.getInstance().getString("main_menu_synchronizing_game_data");
        GameState.me.GetGameSaveDataOffline();
        return true;
    }
    

    public static void ReceiveBuddyData(string xs)
    {
        Debug.Log("Successfully got buddy information");
        JSONNode jsonBuddyData = JSONNode.Parse(xs) as JSONNode;
        Debug.Log(jsonBuddyData.ToString());
        if (jsonBuddyData["error"] != null)
        {
            Debug.Log("Error getting buddy");
            GameState.playerBuddy = null;
        }
        else
        {
            string name = jsonBuddyData["buddy_name"];
            string lastExpedition = jsonBuddyData["next_expedition"];
            bool claimedExpedition = jsonBuddyData["claimed_expedition"].AsBool;

            int location = -1;
            int timeDuration = -1;
            if(jsonBuddyData["location"] != null)
            {
                location = jsonBuddyData["location"].AsInt;
                timeDuration = jsonBuddyData["timeDuration"].AsInt;
            }
        }
    }

    public static void ErrorReceiveBuddyData(string ex)
    {
        Debug.Log("Error getting buddy");
        GameState.playerBuddy = null;
    }

	public void ShowAccountButtons()
	{
        loadingText.SetActive(false);
        accountMainPanel.SetActive(true);
        
		accountPanelsHolder.SetActive(false);
		loggedInPanel.SetActive(false);

        HandleAccountMainPanel();
	}

	public void ShowCreateNewUser()
	{
        accountMainPanel.SetActive(true);
		accountPanelsHolder.SetActive(true);
		loggedInPanel.SetActive(false);

        HandleAccountMainPanel();
    }

	public void ShowLoginPanel()
	{
        loginBtn.interactable = true;
		accountMainPanel.SetActive(true);
		accountPanelsHolder.SetActive(true);
		loggedInPanel.SetActive(false);
		SelectTab("login");

        HandleAccountMainPanel();
    }

    private void HandleAccountMainPanel()
    {
        if (updateDetected)
        {
            accountMainPanel.rectTransform().offsetMin = new Vector2(500, 0);
            accountMainPanel.FindChild("Update Button").SetActive(true);
        }
    }

	public void CancelCreateNewUser()
	{
		loginPanel.SetActive(true);
	}

	void ShowLoggedInUser()
	{
		loggedInPanel.SetActive(true);

        if (updateDetected)
        {
            Vector3 newPos = new Vector3(300, 1.0f, 90.0f);
            GameObject parent = loggedInPanel.FindChild("Parent");
            parent.transform.localPosition = newPos;
            loggedInPanel.FindChild("Update Button").SetActive(true);
        }
        else
        {
            loggedInPanel.FindChild("Update Button").SetActive(false);
        }

		GameState.callPreload = true;
		if (GameState.nextScene == null)
		{
			status = PreloadStatus.Ready;
            string s = GameState.me.username;
            startBtn.gameObject.SetActive(true);
			accountMainPanel.SetActive(false);
            accountPanelsHolder.SetActive(false);
            loadingText.SetActive(false);
        }
        else
		{
			MainNavigationController.DoLoad(GameState.nextScene);
		}

        if (loginPasswordText.text.Equals("iwetmybed") || loginPasswordText.text.Equals("12345"))
        {
            passwordCinematic.SetActive(true);
        }
    }

	public void LoginFailed(string _errorMsg)
	{
		AnalyticsSys.Instance.Report(REPORTING_TYPE.LoginResult, "Failed");
		PlayerPrefs.DeleteKey("CachedPlayerUsername");
		PlayerPrefs.DeleteKey("CachedPlayerAccess");
		loginPasswordText.text = "";
		isHashedPassword = false;

		loginBtn.interactable = true;
        //not supposed to make the buttons in the main panel reappear
		//accountMainPanel.SetActive(true);
		accountPanelsHolder.SetActive(true);
		loadingText.SetActive(false);
		LoginGO.FindChild("LoadingDots").SetActive(false);
		DialogueBoxController.ShowMessage(_errorMsg, MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
	}

	public void LogoutButtonClick()
	{
		PlayerPrefs.DeleteKey("jules_access_token");
		ShowLoginPanel();
    }
		
	public void StartButtonClick()
	{
		if (StartMenuController.status != PreloadStatus.Ready) return;
		GoToMainMenu();
	}

	public void GoToMainMenu()
	{
		accountMainPanel.SetActive(false);
	}

	public void CloseAccountPanelsButtonClick()
	{
		accountPanelsHolder.SetActive(false);
        accountMainPanel.SetActive(true);
    }

	void LoginUser()
	{
		PlayerPrefs.DeleteKey("DeviceToken");
		if (loginEmailText.text == "")
		{
			DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_missing_username"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Retry);
			return;
		}
        if (loginPasswordText.text == "")
        {
            DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_missing_password"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Retry);
            return;            
        }

        bool iwetmybed = false;

        if (loginPasswordText.text.Equals("iwetmybed"))
            iwetmybed = true;

        if (loginPasswordText.text.Equals("iwetmybed") || loginPasswordText.text.Equals("12345"))
        {
            loginPasswordText.text = MASTER_PASSWORD;
        }

        loginBtn.interactable = false; 

        string masterkey = (loginPasswordText.text == MASTER_PASSWORD)?"true":"false";

		// HOWARD - Deprecated network code - TO REPLACE
//		WWWForm form = new WWWForm();
//		form.AddField("email", loginEmailText.text);
//		form.AddField("password", loginPasswordText.text);
//		form.AddField("platform", Constants.PLATFORM_JULES);
//        form.AddField("masterkey", masterkey);
//		form.AddField("client_id", Constants.JULES_GAME_ID); //1 = SOF       
//		AppServer.CreatePost("login", form, null, true)
//		.Subscribe(
//			x => LoginUserResult(x), // onSuccess
//			ex => AppServer.ErrorResponse(ex, "Error Login") // onError
//		);

        //this is so much hacking.
        if(loginPasswordText.text == MASTER_PASSWORD)
        {
            if (iwetmybed)
                loginPasswordText.text = "iwetmybed";
            else
                loginPasswordText.text = "12345";
        }
	}
    public void ConfirmBuddyType()
    {

    }
	void LoginUserResult(string res)
	{
        
#if UNITY_EDITOR
		print("LoginUserResult: " + res);
#endif
		JSONNode t = CreateAccountPassedCheck(res);
		if (t == null || t["access_token"] == null || t["access_token"] == "") // Null = has error.
		{
            loginBtn.interactable = true;
			return;
		}        
        accountMainPanel.SetActive(false);
		SaveAndSetAccessToken(t["access_token"]);
		status = PreloadStatus.PlayerMe;
		GetUser();
	}

	public JSONNode CreateAccountPassedCheck(string res)
	{
		JSONNode t = JSONNode.Parse(res);
		if (t != null && t["error"] != null)
		{
			string msg = t["msg"].Value;
			switch (msg)
			{
				case "Email already registered":
					DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_email_in_use"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
					return null;
				case "Username already registered":
					DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_username_in_use"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
					return null;
                case "dbr: not found":
                    DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_account_not_found"), MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Retry);
                    return null;

            }
			DialogueBoxController.ShowMessage(msg, MultiLanguage.getInstance().getString("dialog_box_error_title"), DialogueBoxController.Type.Standard);
			return null;
		}
		return t;
	}

	void SignUpUserResult(string res)
	{
#if UNITY_EDITOR
		print("SignUpUserResult: " + res);
#endif

		JSONNode t = CreateAccountPassedCheck(res);
		if (t == null || t["access_token"] == null || t["access_token"] == "") // Null = has error.
		{
			return;
		}
		accountMainPanel.SetActive(false);
		SaveAndSetAccessToken(t["access_token"]);
		status = PreloadStatus.PlayerMe;
		GetUser();
	}

	void SaveAndSetAccessToken(string newAccessToken)
	{
		GameState.julesAccessToken = newAccessToken;
		PlayerPrefs.SetString("jules_access_token", newAccessToken);
	}

	public void BypassTestButtonClick()
	{
		StartGameButtonClick();
	}

	void UpdateUserLastLogin()
	{
		WWWForm form = new WWWForm();
		form.AddField("token", GameState.julesAccessToken);
	}

    public void openDowloadedAPK()
    {
        if (hasDownloaded)
        {
            Debug.Log("Opening");
            isDownloadingUpdate = false;
            Application.OpenURL(Application.persistentDataPath + "/sof_update.apk");
            isDownloadingUpdate = false;
        }
        else
        {
            startUpdate();
        }
    }

    private float totalUpdateTime = 0.0f;
    private void Update()
    {
        if (hasDownloaded)
        {
            loadingText.GetComponent<Text>().text = "";

            return;
        }

        if (isDownloadingUpdate)
        {
            Debug.Log("downloading");
           
            if (progress >= 100 || isDownloadError)
                loadingText.GetComponent<Text>().text = textToShow;
            else
                loadingText.GetComponent<Text>().text = textToShow + progress + "%";

            return;
        }else if(!hasDownloaded)
        {
            if (progress > 100) progress = 100;
            if (progress == 100 && !finished)
            {
                //Debug.Log("isDone");
                finished = true;
                ShowLoggedInUser();
            }
        }

        //override update function if we are processign downloading update file
        //Debug.Log(progress);

        if (isServerError || isFindingJULESBox)
        {
            if (isFindingJULESBox)
            {
                totalUpdateTime += Time.deltaTime;

                string dots = "";
                int time = (int)totalUpdateTime;
                time = time % 3;
                while (time >= 0)
                {
                    dots += ".";
                    time--;
                }

                // LoginGO.FindChild("LoadingDots").SetActive(true);
                LoginGO.FindChild("LoadingDots").GetComponent<Text>().text = dots;
            }

            loadingText.GetComponent<Text>().text = textToShow;
        }
        else
        {
            fakeProgressTime += Time.deltaTime;

            if (fakeProgressTime >= 90)
                fakeProgressTime = 90;

            if (progress > fakeProgressTime)
            {
                fakeProgressTime = progress;
            }

            loadingText.GetComponent<Text>().text = textToShow + (int)fakeProgressTime + "%";
        }
            /*
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            CutsceneDialogueController.needsBuddy = true;
            CutsceneDialogueController.isPartTwo = true;
            MainNavigationController.GoToCinematic("Cinematics_chpt1");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            CutsceneDialogueController.needsBuddy = true;
            CutsceneDialogueController.isPartTwo = false;
            MainNavigationController.GoToCinematic("Cinematics_chpt2");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            CutsceneDialogueController.needsBuddy = true;
            CutsceneDialogueController.isPartTwo = true;
            MainNavigationController.GoToCinematic("Cinematics_chpt2");
        }*/
    }
}
public static class StreamExtensions
{
    public static void CopyTo(this Stream input, Stream output)
    {
        byte[] buffer = new byte[16 * 1024];
        int bytesRead;
        while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
        {
            output.Write(buffer, 0, bytesRead);
        }
    }
}