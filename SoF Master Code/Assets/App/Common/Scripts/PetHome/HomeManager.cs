using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class HomeManager : MonoBehaviour
{
    public GameObject videoPlayer;
//	public PetHomeSaveScript petHomeSaveScript;
	private bool onBuildMode = false;
    public bool DraggingObject = false;
	private bool onFriendRoomListMode = false;
	public Text HouseNameText;
	public GameObject playerAvatar;
	public GameObject editableGroup;
	public GameObject inGameGUI;
	public GameObject editableControlButtons;
	public GameObject entrancePos;
    public GameObject doodleVideos;
	public GameObject exitButton;
    public GameObject interactionButton;

	public CanvasGroup sideButtonsCanvasGroup;
	public Button expandMenuButton;
	public Button hideMenuButton;

	public RoomController roomController;
	public Dictionary<string, GameObject> allPropsGO; 
	public List<GameObject> allInstantiatedEditables;
	public List<Furniture> beforeEditFurnitures;
    public HomeCamera homeCameraController;
	private static HomeManager _instance;
    public bool loadNewHome;
	void Awake() {
        mantaMatchDesign.Load();
        tumbleTroubleDesign.Load();
        PearlyLevelDesignHolder.LoadDesigns();
        //tugotakoDesign.Load();
    #if UNITY_EDITOR
        PlayerPrefs.SetInt("TakoLevel", 0);//Use this to debug Tako (It resets progression)
    #endif
        _instance = this;
        PlayerPrefs.SetInt("FromStart", 0);
        PlayerPrefs.SetInt("PlayedVideo", 0);
        interactionButton.SetActive(true);
    #if UNITY_EDITOR

        if (GameState.me == null)
        {
            if (loadNewHome) {
               
            GameState.nextScene = "mapNew";
            MainNavigationController.GoToStartMenu(false);
            return;
            } else
            {
            GameState.nextScene = "home";
            MainNavigationController.GoToStartMenu(false);
                return;
            }

        }else
        {
            GameState.UpdateIndex();
        }
#endif
    }
	public void EnableDoodle()
    {
		AudioSystem.PlaySFX("UI/sfx_ui_click");
        doodleVideos.SetActive(true);
        return;
    }
	public static HomeManager instance {
		get {
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<HomeManager>();
			}
			
			return _instance;
		}
	}

    public void EnableIQTest(int i)
    {
        GameObject b = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("ui/IQTest"));
        b.GetComponent<IQTest>().Start(i);
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

    void Start()
	{
		Screen.fullScreen = true;

		// HOWARD - Deprecated network code - TO REPLACE
//        JulesBox.Authenticate();

	    LoadAvatar();
		this.init ();
		AudioSystem.PlayBGM("bgm_bh_hardly-working");

		//HideMenuButtons();
	}
    public GameObject tutorialBox;
	public IEnumerator WaitFrame ()
	{
		yield return 0;
        tutorialBox.SetActive(true);
        HideMenuButtons();
	}

	void HideMenuButtons()
	{
		GUIUtils.HideCanvasGroup(sideButtonsCanvasGroup);
		expandMenuButton.gameObject.SetActive(true);
		hideMenuButton.gameObject.SetActive(false);
	}

	void ShowMenuButtons()
	{
		GUIUtils.ShowCanvasGroup(sideButtonsCanvasGroup);
		expandMenuButton.gameObject.SetActive(false);
		hideMenuButton.gameObject.SetActive(true);
	}

	public void HideMenuButtonsClick()
	{
		AudioSystem.PlaySFX("UI/sfx_ui_click");
		HideMenuButtons();
	}

	public void ShowMenuButtonsClick()
	{
		AudioSystem.PlaySFX("UI/sfx_ui_click");
		ShowMenuButtons();
	}

	void LoadAvatar ()
	{
		if (GameState.me != null && GameState.me.avatar == null)
		{
			MainNavigationController.GoToAvatarCreation();
		}

		GameObject avatar = AvatarLoader.current.GetAvatarObject();
		if (avatar == null)
		{
			return;
		}

		// Destroy old pet objects if any
		foreach (Transform g in playerAvatar.GetComponentsInChildren<Transform>())
		{
			if (g.gameObject != playerAvatar)
			{
				Destroy(g.gameObject);
			}
		}

		avatar.transform.localPosition = new Vector2(0, -0.339f);
		avatar.transform.SetParent(playerAvatar.transform, false);
		Animator avatarAnimator = avatar.GetComponent<Animator>();
        try
        {
            AnimationControllerScript.current.SetAnimator(avatarAnimator);
        }catch(System.Exception e)
        {
            Debug.Log(e);
        }
	}

	public void init(){

        SetupUI();

		roomController.init();
//		PropsInventory.instance.init ();
		inGameGUI.SetActive (true);
		editableControlButtons.SetActive (false);
		setBuildMode (false);
		allInstantiatedEditables = new List<GameObject> ();
		beforeEditFurnitures = new List<Furniture> ();
		resetAvatarPosition ();
	}

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(GameObject.Find("Home Title Panel").FindChild("Title Text"), "buddy_home_room");

        MultiLanguage.getInstance().applyImage(GameObject.Find("buddy_room").FindChild("VideoIcon").GetComponent<Image>(), "buddy_home_video_panel_video_icon");
        MultiLanguage.getInstance().applyImage(GameObject.Find("buddy_room").FindChild("Worksheet").GetComponent<Image>(), "buddy_home_video_panel_worksheet_icon");
        MultiLanguage.getInstance().applyImage(GameObject.Find("buddy_room").FindChild("VideoIcon (1)").GetComponent<Image>(), "buddy_home_video_panel_doodle_icon");

        if (GameState.playerBuddy != null && GameState.playerBuddy.name != null)
        {
            HouseNameText.text = GameState.playerBuddy.name.ToUpper() + MultiLanguage.getInstance().getString("buddy_home_room");
        }
    }


	void Update()
	{
        if (Input.GetKeyDown(KeyCode.O))
        {
            GameState.UpdateIndex();
        }
	}

	public void ExitButtonClick()
	{
		DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_exit_to_start"), "", DialogueBoxController.Type.Confirm, gotoStartMenu);
	}

	public void gotoStartMenu()
	{
        PlayerPrefs.SetInt("NeedsLogout", 1);
		PlayerPrefs.DeleteKey("CachedPlayerUsername");
		PlayerPrefs.DeleteKey("CachedPlayerAccess");
		MainNavigationController.GoToStartMenu();
	}
    public void gotoInteraction()
    {
        AudioSystem.PlaySFX("UI/sfx_ui_click");
        MainNavigationController.GoToScene("interaction");
    }

	public void MapButtonClick()
	{

		LoadMapScene();
	}
	
	void LoadMapScene()
	{
		AudioSystem.PlaySFX("UI/sfx_ui_click");
		MainNavigationController.GoToMap();
	}

	public void UpdateAvatarButtonClick()
	{
		AudioSystem.PlaySFX("UI/sfx_ui_click");
		MainNavigationController.GoToAvatarCreation();
    }

	public void buildButtonOnPress(){
		AudioSystem.PlaySFX("UI/sfx_ui_click");
        if (homeCameraController != null)
            homeCameraController.stateMachine.SelectNewState("Build");
        //        homeCameraController

        playerAvatar.SetActive (false);
		inGameGUI.SetActive (false);
		exitButton.SetActive(false);
		setBuildMode (true);
		editableControlButtons.SetActive (true);
		setOutOfCameraViewEditableControlButtons ();
		
		//save prev format before any editing goes
		beforeEditFurnitures.Clear ();
		long iter = 0;
		foreach(GameObject furnitureGO in allInstantiatedEditables){
			beforeEditFurnitures.Add(furnitureGO.GetComponent<Furniture>());
			iter++;
			//Debug.Log(iter);
		}

	}

	public void friendRoomListButtonOnPress(){
        if (!PhotonNetwork.insideLobby)
        {
            PhotonNetwork.JoinLobby();
            PhotonNetwork.JoinLobby();
            PhotonNetwork.JoinLobby();
        }
        //FriendRoomList.instance.refreshRoomList ();
		playerAvatar.SetActive (false);
		inGameGUI.SetActive (false);
		//friendRoomList.SetActive (true);
		setBuildMode (false);
		editableControlButtons.SetActive (false);
		setFriendRoomMode (true);
		AudioSystem.PlaySFX("UI/sfx_ui_click");
		setOutOfCameraViewEditableControlButtons ();


        //PhotonNetwork.JoinRoom("Tianyang Test's Room");
    }

	public void friendRoomListCancelButtonOnPress(){
		AudioSystem.PlaySFX("UI/sfx_ui_click");
		//friendRoomList.SetActive (false);
		playerAvatar.SetActive (true);
		inGameGUI.SetActive (true);
		setBuildMode (false);
		editableControlButtons.SetActive (false);
		setFriendRoomMode (false);
		//Camera.main.GetComponent<FFFollowObject> ().resetPosition ();
		
	}
	public void buildCancelButtonOnPress(){
		
		playerAvatar.SetActive (true);
		inGameGUI.SetActive (true);
		exitButton.SetActive(true);
		setBuildMode (false);
		editableControlButtons.SetActive (false);
		//reset to prev format
		allInstantiatedEditables.Clear ();
		foreach (Transform child in editableGroup.transform) {
			GameObject.Destroy(child.gameObject);
		}
		/*foreach (Furniture furnitureItem in beforeEditFurnitures) {
			GameObject justInstantiatedGO = Instantiate(Resources.Load("PetHome/InventoryAssets/"+furnitureItem.getInitialName())) as GameObject;
			justInstantiatedGO.name = furnitureItem.getInitialName();
			justInstantiatedGO.GetComponent<Collider>().enabled=true;
			justInstantiatedGO.AddComponent<DragObject>();
			justInstantiatedGO.AddComponent<Furniture>();
			Destroy(justInstantiatedGO.GetComponent<PhotonView>());
			Destroy(justInstantiatedGO.GetComponent<FurnitureNetwork>());
			justInstantiatedGO.transform.localPosition = new Vector3(furnitureItem.getInitialPositionX(),furnitureItem.getInitialPositionY(),furnitureItem.getInitialPositionZ());
			justInstantiatedGO.transform.localScale = new Vector3(Constants.HOME_FURNITURE_SCALE, Constants.HOME_FURNITURE_SCALE, Constants.HOME_FURNITURE_SCALE);
			justInstantiatedGO.transform.Rotate(0f,furnitureItem.getInitialRotationValue(),0f);
			justInstantiatedGO.transform.SetParent(editableGroup.transform,true);
			justInstantiatedGO.GetComponent<Furniture>().saveValues();
			allInstantiatedEditables.Add(justInstantiatedGO);
		} 
		/// 
		foreach (GameObject furnitureGO in allInstantiatedEditables) {
			furnitureGO.GetComponent<Furniture>().saveValues();
			Debug.Log (furnitureGO.GetComponent<Furniture>().getInitialName());
			Debug.Log (furnitureGO.GetComponent<Furniture>().getInitialPositionX());
			Debug.Log (furnitureGO.GetComponent<Furniture>().getInitialPositionY());
			Debug.Log (furnitureGO.GetComponent<Furniture>().getInitialPositionZ());
			Debug.Log (furnitureGO.GetComponent<Furniture>().getInitialRotationValue());
		}
		petHomeSaveScript.consolidateDataIntoFurnitureItems (allInstantiatedEditables);*/
		
		resetAvatarPosition ();
//		Camera.main.GetComponent<FFFollowObject> ().resetPosition ();

	}

	public void buildDoneButtonOnPress(){
		AudioSystem.PlaySFX("UI/sfx_ui_click");

		playerAvatar.SetActive (true);
		inGameGUI.SetActive (true);
		exitButton.SetActive(true);
		setBuildMode (false);
		editableControlButtons.SetActive (false);

		resetAvatarPosition ();
	}


	public void buildCreateObject(string nameOfProp){
		Ray ray;
		RaycastHit hit;
		ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
		if (Physics.Raycast(ray, out hit, 100.0f,9)){
			GameObject tempPropObject ;
			if(allPropsGO.TryGetValue(nameOfProp,out tempPropObject)){
				GameObject justInstantiatedGO = Instantiate(tempPropObject, hit.point, Quaternion.identity) as GameObject;
				justInstantiatedGO.name = tempPropObject.name;
				justInstantiatedGO.GetComponent<Collider>().enabled=true;
				justInstantiatedGO.transform.localScale = new Vector3(Constants.HOME_FURNITURE_SCALE, Constants.HOME_FURNITURE_SCALE, Constants.HOME_FURNITURE_SCALE);
				justInstantiatedGO.transform.SetParent(editableGroup.transform);
				allInstantiatedEditables.Add(justInstantiatedGO);

			}
		}
	}

	public void resetAvatarPosition(){
		//playerAvatar.transform.position = entrancePos.transform.position;

	}
	public bool getFriendRoomMode(){
		return onFriendRoomListMode;
	}
	public void setFriendRoomMode(bool setMode){
		onFriendRoomListMode = setMode;
	}

	public bool getBuildMode(){
		return onBuildMode;
	}
	public void setBuildMode(bool setMode){
		onBuildMode = setMode;
	}
	public void disableEditableControlButtons(){
        Debug.Log("Disabling Buttons");
		editableControlButtons.SetActive (false);
	}
	public void enableEditableControlButtons(){
        Debug.Log("Enabling Buttons");
        editableControlButtons.SetActive (true);
	}
	public void setOutOfCameraViewEditableControlButtons(){
        Debug.Log("SetOutOfCameraView");
        editableControlButtons.transform.position = new Vector3 (-999f, -999f, 0);
	}
    public void playYoutubeVideo(string episode)
    {
        PlayerPrefs.SetString("EpisodeToPlay", episode);
        PlayerPrefs.SetInt("IsYoutube", 1);
        //        Debug.Log(count);
        videoPlayer.SetActive(true);
    }

    public void playAWSVideo(string episode)
    {
        PlayerPrefs.SetString("EpisodeToPlay", episode);
        PlayerPrefs.SetInt("IsYoutube", 0);
        //        Debug.Log(count);
        videoPlayer.SetActive(true);
    }

    public void playDoodleEpisode(string episode)
    {
        PlayerPrefs.SetString("EpisodeToPlay", episode);
        PlayerPrefs.SetInt("IsYoutube", 1);
        //        Debug.Log(count);
        videoPlayer.SetActive(true);

    }
    public void playDoodleChineseEpisode(int number) // now does both automagically
    {
		// HOWARD - Deprecated network code - TO REPLACE
//        Debug.Log("Has found server: " + (JulesBox.HAS_FOUND_SERVER ? 0 : 1)); 
//        Debug.Log("Finding episode: " + VideoURL.GetDoodleEpisodeLink(number-1));
        PlayerPrefs.SetString("EpisodeToPlay", VideoURL.GetDoodleEpisodeLink(number-1));

        PlayerPrefs.SetInt("IsYoutube", 0);//use AWS and not youtube // JulesBox.HAS_FOUND_SERVER? 0 : 1);
        //        Debug.Log(count);
        videoPlayer.SetActive(true);

    }
    public void PlayInteractiveCutscene()
    {
        MainNavigationController.GoToScene("test_cutscene");
    }
	
}