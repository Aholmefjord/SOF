using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UniRx;


//[RequireComponent(typeof (PhotonView))]
public class HomeNetworkManager : Photon.MonoBehaviour
{
//	public NetworkPetHomeSaveScript networkPetHomeSaveScript;
	private bool onBuildMode = false;
	public Text HouseNameText;
	public GameObject speechDialogueUIPanel;
	public GameObject speechDialogue;
	public GameObject emojiDialogue;
	public GameObject playerAvatar;
	public GameObject editableGroup;
	public GameObject inGameGUI;
	public GameObject entrancePos;
	public Dictionary<string, GameObject> allPropsGO; 
	public List<GameObject> allInstantiatedEditables;
	private static HomeNetworkManager _instance;
    private static string _MasterID;
	void Awake() {
		_instance = this;
	}
    public void LateUpdate()
    {
        switch (Application.internetReachability)
        {
            case NetworkReachability.ReachableViaLocalAreaNetwork:
                break;
            case NetworkReachability.ReachableViaCarrierDataNetwork:
                break;
            case NetworkReachability.NotReachable:
                leaveRoom();
                break;
        }
    }
    public static HomeNetworkManager instance {
		get {
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<HomeNetworkManager>();
			}
			
			return _instance;
		}
	}
	
	void Start()
	{
        this.init ();
	}
    string masterID = "";
//need to link houseName
	public void init()
    {
        HouseNameText.text = PlayerPrefs.GetString("MulitpayerRoomName");

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            if (PhotonNetwork.playerList[i].isMasterClient)
            {
                masterID = PhotonNetwork.playerList[i].name;
            }
        }
        Debug.Log("Master ID: " + masterID);
        if (PhotonNetwork.isMasterClient)
        {
            masterID = PhotonNetwork.player.name;
            GameState.PostActivity("nullvalue", 0, 0, 0,PhotonNetwork.room.name, 0);
        }else
        {
            GameState.PostActivity(masterID, 2, 0, 0, PhotonNetwork.room.name, 0);
        }
	}
	
    public void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("Player Connected");
        if (PhotonNetwork.isMasterClient) { 
            GameState.PostActivity(other.name, 1, 0, 0,PhotonNetwork.room.name, 0);
        }
    }	
    public void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log("Player Disconnected");
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("Doing internal");
                bool containedKicked = false;
                try {
                    containedKicked  = other.customProperties.ContainsKey("wasKicked");
                }
                catch(System.Exception e)
                {
                Debug.Log(e);   
                }
                bool containedclosed = false;
                try
                {
                    containedclosed = other.customProperties.ContainsKey("wasClosed");    
                }
                catch(System.Exception e)
                {

                    Debug.Log(e);
                }
                //only use this when leaving under normal conditions
                if (!containedclosed && !containedKicked){
                    Debug.Log("Posting when normal leave");

                }
        }
    }
	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			MainNavigationController.GoToStartMenu();
		}
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log(GameObject.Find("HomeManager").name);
            Debug.Log(GameObject.Find("HomeManager").GetComponent<HomeNetworkManager>().name);
        }
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

	public void popDialogue(string newLine, string senderName){
		//GameObject senderChar = GameObject.Find (senderName + " Character").gameObject;
        
		GameObject tempSpeech = Instantiate (speechDialogue) as GameObject;
		tempSpeech.transform.Find ("Text").GetComponent<Text> ().text = newLine;
		tempSpeech.transform.SetParent (speechDialogueUIPanel.transform);
		tempSpeech.GetComponent<NetworkDialogue>().followRecordedPos(senderName);
	}

	public void popEmoji(Texture emojiTexture, string senderName){
		GameObject tempSpeech = Instantiate (emojiDialogue) as GameObject;
		tempSpeech.transform.SetParent (speechDialogueUIPanel.transform);
		tempSpeech.transform.Find ("EmojiImg").GetComponent<RawImage> ().texture = emojiTexture;
		tempSpeech.GetComponent<NetworkDialogue>().followRecordedPos(senderName);
	}
    public void OnMasterClientSwitched()
    {
        Debug.Log("Master Client Switched");
		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//        form.AddField("user_id", masterID);
//        form.AddField("room_name", "closed");
//        form.AddField("display_name", "closed");
//        form.AddField("active", 0);
//        Debug.Log("Update the room in the database");
//        AppServer.CreatePost("rooms/update", form)
//        .Subscribe(
//            x => Debug.Log(x), // onSuccess
//            ex => Debug.Log(ex) // onError
//        );
        GameState.PostActivityOther(masterID, " ", 8, 0, 0, PhotonNetwork.room.name, 0);
        if (PhotonNetwork.isMasterClient)
        {
            for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (PhotonNetwork.playerList[i].isMasterClient)
                {
                    ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                    h["notDroppedOut"] = true;
                    PhotonNetwork.playerList[i].SetCustomProperties(h);
                }
            }
            foreach (PhotonPlayer pPlayer in PhotonNetwork.playerList)
            {
                PhotonNetwork.CloseConnection(pPlayer);
                PhotonVoiceNetwork.Disconnect();
            }
            wasForced = false;
        }
        

    }
    public bool wasForced = false;
	public void leaveRoom()
    {
		if (PhotonNetwork.isMasterClient) {
			// HOWARD - Deprecated network code - TO REPLACE
//            WWWForm form = new WWWForm();
//            form.AddField("user_id", GameState.me.id);
//            form.AddField("room_name", "closed");
//            form.AddField("display_name", GameState.playerBuddy.name.ToUpper());
//            form.AddField("active", 0);
//            Debug.Log("Update the room in the database");
//            AppServer.CreatePost("rooms/update", form)
//            .Subscribe(
//                x => Debug.Log(x), // onSuccess
//                ex => Debug.Log(ex) // onError
//            );
            if (!wasForced) { 
                GameState.PostActivity(" ", 8, 0, 0,PhotonNetwork.room.name, 0);
            }
            else
            {
                GameState.PostActivity(" ", 8, 0, 0, PhotonNetwork.room.name, 1);
            }
            for(int i = 0; i < PhotonNetwork.playerList.Length; i++)
            {
                if (PhotonNetwork.playerList[i].isMasterClient)
                {
                    ExitGames.Client.Photon.Hashtable h = new ExitGames.Client.Photon.Hashtable();
                    h["notDroppedOut"] = true;
                    PhotonNetwork.playerList[i].SetCustomProperties(h);
                }
            }
            foreach (PhotonPlayer pPlayer in PhotonNetwork.playerList) {
                ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
                table["wasKicked"] = 1;
                pPlayer.SetCustomProperties(table);
				PhotonNetwork.CloseConnection (pPlayer);
                PhotonVoiceNetwork.Disconnect();
            }//PhotonVoiceNetwork.Connect();
            wasForced = false;
        } else {
            if (PhotonNetwork.player.customProperties.ContainsKey("wasKicked"))
            {
                GameState.PostActivityOther(masterID, GameState.me.id, 6, 0, 0, PhotonNetwork.room.name, 1);
                GameState.PostActivity(masterID, 7, 0, 0, PhotonNetwork.room.name,1);
            }else
            {
                if (!wasForced) {
                    GameState.PostActivityOther(masterID, GameState.me.id, 6, 0, 0, PhotonNetwork.room.name, 0);
                    GameState.PostActivity(masterID, 7, 0, 0, PhotonNetwork.room.name, 0);
                }
            }
            PhotonNetwork.LeaveRoom ();
            PhotonVoiceNetwork.Disconnect();
            wasForced = false;
        }
        GameObject.Find("Audio_BGM").GetComponent<AudioSource>().mute = false;
    }

    public void resetAvatarPosition()
    {
		//Vector3 tempPos = new Vector3(entrancePos.transform.position.x,0.0f,entrancePos.transform.position.z);

		//PetNetWork.instance.ownPlayerAvatar.transform.position = entrancePos.transform.position;
	}
    private void OnApplicationFocus(bool focus)
    {
        /*
#if !UNITY_EDITOR
        if (focus == false)
        {
                wasForced = true;
                leaveRoom();
                
                if (PhotonNetwork.isMasterClient)
                {
                        WWWForm form = new WWWForm();
                        form.AddField("user_id", masterID);
                        form.AddField("room_name", PhotonNetwork.room.name);
                        form.AddField("display_name", GameState.playerBuddy.name.ToUpper());
                        form.AddField("active", 0);
                        Debug.Log("Update the room in the database");
                        AppServer.CreatePost("rooms/update", form)
                        .Subscribe(
                            x => Debug.Log(x), // onSuccess
                            ex => Debug.Log(ex) // onError
                        );
                        GameState.PostActivity(" ", 8, 0, 0, PhotonNetwork.room.name, 0);
                }
                else
                {
                    GameState.PostActivityOther(masterID, GameState.me.id, 6, 0, 0, PhotonNetwork.room.name, 0);
                    GameState.PostActivity(masterID, 7, 0, 0, PhotonNetwork.room.name, 0);
                }

        }
#endif
*/
    }
    private void OnApplicationQuit()
    {
        wasForced = true;
        leaveRoom();

        if (PhotonNetwork.isMasterClient)
        {
			// HOWARD - Deprecated network code - TO REPLACE
//            WWWForm form = new WWWForm();
//            form.AddField("user_id", masterID);
//            form.AddField("room_name", PhotonNetwork.room.name);
//            form.AddField("display_name", GameState.playerBuddy.name.ToUpper());
//            form.AddField("active", 0);
//            Debug.Log("Update the room in the database");
//            AppServer.CreatePost("rooms/update", form)
//            .Subscribe(
//                x => Debug.Log(x), // onSuccess
//                ex => Debug.Log(ex) // onError
//            );
            GameState.PostActivity(" ", 8, 0, 0, PhotonNetwork.room.name, 0);
        }
        else
        {
            GameState.PostActivity(masterID, 7, 0, 0, PhotonNetwork.room.name, 0);
        }
    }
}