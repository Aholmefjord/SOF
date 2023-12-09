using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using SimpleJSON;
using System;
using System.Collections.Generic;

public class RoomListReceiver : MonoBehaviour
{
    public List<NetworkRoom> rooms;
    public List<GameObject> Buttons;
    public GameObject parentButtonObject;
    public GameObject roomButtonObject;
    public static string FurnitureString;
    bool connectFailed = false;
    public string ID;
    // Use this for initialization
    public void init()
    {
		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//        form.AddField("user_id", GameState.me.id);
//        ID = GetUniqueID();
//        Debug.Log("Unique ID for chat: " + ID);
//        form.AddField("room_name", ID);
//        form.AddField("display_name", GameState.playerBuddy.name.ToUpper() + MultiLanguage.getInstance().getString("buddy_home_chat_panel_room"));
//        form.AddField("active", 0);
//        Debug.Log("Update the room in the database");
//        AppServer.CreatePost("rooms/check", form)
//        .Subscribe(
//            x => CheckRoom(x), // onSuccess
//            ex => Debug.Log(ex) // onError
//        );

        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)//PeerState.PeerCreated)
        {
            // Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
            PhotonNetwork.ConnectUsingSettings("0.9");
        }

        // generate a name for this player, if none is assigned yet
        if (String.IsNullOrEmpty(PhotonNetwork.playerName))
        {
            try
            {
                PhotonNetwork.playerName = GameState.me.id;
            }
            catch
            {
            }
        }
        PhotonNetwork.JoinLobby();

        //PhotonNetwork.logLevel = NetworkLogLevel.Full;
    }
    public bool IsInNetwork = true;
    public void Start()
    {

        //        FurnitureString = 
        if (IsInNetwork)
        {
			// HOWARD - Deprecated network code - TO REPLACE
//            WWWForm form = new WWWForm();
//            form.AddField("user_id", 0);
//            Debug.Log("START TO GET THE ROOMS");
//            AppServer.CreatePost("rooms", form)
//            .Subscribe(
//                x => GetRooms(x), // onSuccess
//                ex => Debug.Log(ex) // onError
//            );
        }

        SetupUI();
    }

    public void SetupUI()
    {
        //Chat Room
        MultiLanguage.getInstance().apply(gameObject.FindChild("TitleText"), "buddy_home_chat_panel_title");
        MultiLanguage.getInstance().apply(gameObject.FindChild("ExplainText"), "buddy_home_chat_panel_explain");
        MultiLanguage.getInstance().apply(gameObject.FindChild("RefreshButton").FindChild("SortText (1)"), "buddy_home_chat_panel_refresh");
        MultiLanguage.getInstance().apply(gameObject.FindChild("CreateButton").FindChild("SortText (1)"), "buddy_home_chat_panel_ecreate_new");
        MultiLanguage.getInstance().apply(gameObject.FindChild("ChatRoom").FindChild("CreateButton").FindChild("SortText (2)"), "buddy_home_chat_panel_ecreate_new_second_line");

        //MultiLanguage.getInstance().apply(gameObject.FindChild("RoomCreate").FindChild("Text"), "buddy_home_chat_panel_room");
    }


    public void CreateRoom()
    {
        Debug.Log(GameState.me.id);
        if (IsInNetwork)
        {
			// HOWARD - Deprecated network code - TO REPLACE
//            WWWForm form = new WWWForm();
//            form.AddField("user_id", GameState.me.id);
//            ID = GetUniqueID();
//            Debug.Log("Unique ID for chat: " + ID);
//            form.AddField("room_name", ID);
//            form.AddField("display_name", GameState.playerBuddy.name.ToUpper() + MultiLanguage.getInstance().getString("buddy_home_chat_panel_room"));
//            form.AddField("active", 1);
//            Debug.Log("Update the room in the database");
//            AppServer.CreatePost("rooms/check", form)
//            .Subscribe(
//                x => CheckRoom(x), // onSuccess
//                ex => Debug.Log(ex) // onError
//            );
        }
    }
    public void CheckRoom(string s)
    {
        if (IsInNetwork)
        {
            JSONNode jsonPlayerData = JSONNode.Parse(s) as JSONNode;
            Debug.Log(s);
            if (jsonPlayerData["msg"] != null)
            {
                if (((string)jsonPlayerData["msg"]).CompareTo("dbr: not found") == 0)
                {
                    Debug.Log("Wasn't in database, creating now");
					// HOWARD - Deprecated network code - TO REPLACE
//                    WWWForm form = new WWWForm();
//                    Debug.Log(GameState.me.id);
//                    form.AddField("user_id", GameState.me.id);
//                    ID = GetUniqueID();
//                    Debug.Log("Unique ID for chat: " + ID);
//                    form.AddField("room_name", ID);
//                    form.AddField("display_name", GameState.playerBuddy.name.ToUpper());
//                    Debug.Log(GameState.me.id);
//                    Debug.Log(GameState.playerBuddy.name.ToUpper() + MultiLanguage.getInstance().getString("buddy_home_chat_panel_room"));
//                    AppServer.CreatePost("rooms/create", form)
//                    .Subscribe(
//                        x => onInviteFriendPress(x), // onSuccess
//                        ex => Debug.Log(ex) // onError
//                    );
                }
            }
            else
            {
				// HOWARD - Deprecated network code - TO REPLACE
//                WWWForm form = new WWWForm();
//                form.AddField("user_id", GameState.me.id);
//                ID = GetUniqueID();
//                Debug.Log("Unique ID for chat: " + ID);
//                form.AddField("room_name", ID);
//                form.AddField("display_name", GameState.playerBuddy.name.ToUpper());
//                form.AddField("active", 1);
//                Debug.Log("Update the room in the database");
//                AppServer.CreatePost("rooms/update", form)
//                .Subscribe(
//                    x => CreateRoomInDB(x), // onSuccess
//                    ex => Debug.Log(ex) // onError
//                );
            }
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    void CreateRoomInDB(string er)
    {
        JSONNode jsonPlayerData = JSONNode.Parse(er) as JSONNode;
        Debug.Log(er);
        if (jsonPlayerData["msg"] != null)
        {
            if (((string)jsonPlayerData["msg"]).CompareTo("dbr: not found") == 0)
            {
                Debug.Log("Wasn't in database, creating now");

				// HOWARD - Deprecated network code - TO REPLACE
//                WWWForm form = new WWWForm();
//                Debug.Log(GameState.me.id);
//                form.AddField("user_id", GameState.me.id);
//                ID = GetUniqueID();
//                Debug.Log("Unique ID for chat: " + ID);
//                form.AddField("room_name", ID);
//                form.AddField("display_name", GameState.playerBuddy.name.ToUpper());                
//                Debug.Log("Create the room in the database");
//                AppServer.CreatePost("rooms/create", form)
//                .Subscribe(
//                    x => onInviteFriendPress(x), // onSuccess
//                    ex => Debug.Log(ex) // onError
//                );

            }
        }
        else
        {
            onInviteFriendPress(er);
        }
    }
    void GetRooms(string x)
    {
        JSONNode jsonPlayerData = JSONNode.Parse(x) as JSONNode;
        JSONArray arr = jsonPlayerData.AsArray;
        Debug.Log("Got Rooms of count " + arr.Count);
        rooms = new List<NetworkRoom>();
        for (int i = 0; i < Buttons.Count; i++)
        {
            Destroy(Buttons[i]);
        }
        Buttons = new List<GameObject>();
        for (int i = 0; i < arr.Count; i++)
        {
            NetworkRoom r = new NetworkRoom();
            r.user_id = arr[i]["user_id"].AsInt;
            r.room_name = arr[i]["room_name"];
            Debug.Log("ROOM NAME: " + r.room_name);
            r.display_name = arr[i]["display_name"] + MultiLanguage.getInstance().getString("buddy_home_chat_panel_room");
            r.is_active = arr[i]["active"].AsInt;
            rooms.Add(r);
        }
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].is_active == 1)
            {
                try
                {
                    GameObject g = Instantiate<GameObject>(roomButtonObject);
                    g.transform.parent = parentButtonObject.transform;
                    g.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    g.transform.localScale = new Vector3(1, 1, 1);
                    g.transform.localPosition = new Vector3(g.transform.localPosition.x, g.transform.localPosition.y, 0);
                    g.transform.GetChild(0).GetComponent<Text>().text = rooms[i].display_name;
                    g.AddComponent<NetworkRoomButton>();
                    g.GetComponent<NetworkRoomButton>().user_id = i;
                    g.GetComponent<NetworkRoomButton>().room_name = rooms[i].room_name;
                    g.GetComponent<NetworkRoomButton>().display_name = rooms[i].display_name;
                    g.GetComponent<NetworkRoomButton>().furniture_string = rooms[i].furnitureString;
                    g.GetComponent<Button>().onClick.AddListener(delegate { OnConnect(g.GetComponent<Button>()); });
                    g.SetActive(true);
                    Buttons.Add(g);
                }
                catch (Exception e) { }
            }
        }
    }

    public void OnConnect(Button b)
    {

        Debug.Log("CLICKED: " + b.GetComponent<NetworkRoomButton>().room_name);
        PlayerPrefs.SetString("MulitpayerRoomName", b.GetComponent<NetworkRoomButton>().display_name);
        //        PhotonNetwork.JoinRandomRoom();
        PhotonNetwork.player.name = GameState.me.id;
        PhotonNetwork.JoinRoom(b.GetComponent<NetworkRoomButton>().room_name);
    }
    public string FurnitureLevels()
    {
        string s = "";
        s += GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, GameState.me.inventory.Bed_Selected);
        s += ",";
        s += GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, 1);
        s += ",";
        s += GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, 1);
        s += ",";
        s += GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, 1);
        return s;
    }
    public void onInviteFriendPress(string x)
    {
        Debug.Log("RECEIVED: " + x);
        JSONNode jsonPlayerData = JSONNode.Parse(x) as JSONNode;
        Debug.Log(x);
        if (jsonPlayerData["Status"] == null)
        {
            return;
        }
        DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialgo_box_message_creating_room"), "", DialogueBoxController.Type.NoButtons);
        /*   if (!PhotonNetwork.connected)
           {
               if (PhotonNetwork.connecting)
               {
                   Debug.Log("Connecting to: " + PhotonNetwork.ServerAddress);
               }
               else
               {
                   Debug.Log("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
               }

               if (this.connectFailed)
               {
                   Debug.Log("Connection failed. Check setup and use Setup Wizard to fix configuration.");
                   Debug.Log(String.Format("Server: {0}", new object[] { PhotonNetwork.ServerAddress }));
                   Debug.Log("AppId: " + PhotonNetwork.PhotonServerSettings.AppID.Substring(0, 8) + "****"); // only show/log first 8 characters. never log the full AppId.
               }
               DialogueBoxController.Hide();
               return;
           }*/

        if (GameState.me != null && GameState.me.username != null)
        {
            Debug.Log(GameState.me.username);
            PhotonNetwork.playerName = GameState.me.id;
            PhotonNetwork.CreateRoom(ID, new RoomOptions() { maxPlayers = 20, publishUserId = true }, null);
        }

    }

    public void JoinRoom(string x)
    {
        Debug.Log("Received ID: " + x);
        PhotonNetwork.playerName = GameState.me.id;
        PhotonNetwork.JoinRoom(x);
    }
    // We have two options here: we either joined(by title, list or random) or created a room.
    //s    public void OnJoinedRoom()
    //   {
    //        Debug.Log("OnJoinedRoom");

    //   }

    public void OnPhotonCreateRoomFailed()
    {
        //    ErrorDialog = "Error: Can't create room (room name maybe already used).";
        Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
        DialogueBoxController.Hide();
        DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialgo_box_message_unable_to_create_room"), "", DialogueBoxController.Type.Confirm, null);
    }

    public void OnPhotonJoinRoomFailed(object[] cause)
    {
        //        ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
        Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
        DialogueBoxController.Hide();
        DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialgo_box_message_unable_to_join_room"),"", DialogueBoxController.Type.Confirm,null);
    }

    public void OnPhotonRandomJoinFailed()
    {
        //      ErrorDialog = "Error: Can't join random room (none found).";
        Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        Debug.Log(PhotonNetwork.room.name);
        PlayerPrefs.SetString("MulitpayerRoomName", GameState.playerBuddy.name.ToUpper() + MultiLanguage.getInstance().getString("buddy_home_chat_panel_room"));
        PlayerPrefs.SetString("FurnitureString", GameState.me.inventory.GetFurnitureString());
        PlayerPrefs.SetString("FurnitureLevels", FurnitureLevels());
        PlayerPrefs.SetInt("Item1", GameState.me.inventory.Item1_Selected);
        PlayerPrefs.SetInt("Item2", GameState.me.inventory.Item2_Selected);
        PlayerPrefs.SetInt("Item3", GameState.me.inventory.Item3_Selected);
        PlayerPrefs.SetInt("Item4", GameState.me.inventory.Item4_Selected);
        PlayerPrefs.SetInt("Item5", GameState.me.inventory.Item5_Selected);
        PlayerPrefs.SetInt("Item6", GameState.me.inventory.Item6_Selected);
        PhotonNetwork.LoadLevel("mapNewMulti");
    }

    public void OnDisconnectedFromPhoton()
    {
        Debug.Log("Disconnected from Photon.");
    }

    public void OnFailedToConnectToPhoton(object parameters)
    {
        this.connectFailed = true;
        Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.ServerAddress);
    }

    public static string GetUniqueID()
    {
        string uniqueID = Guid.NewGuid().ToString();             //random number
        return uniqueID;
    }

}
[Serializable]
public class NetworkRoom
{
    public int user_id;
    public string room_name;
    public string display_name;
    public int is_active;
    public string furnitureString;
    public string furnitureLevels;

}
