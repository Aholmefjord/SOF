using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FriendRoomList : MonoBehaviour {
	public GameObject roomController;
	public GameObject friendRoomPanelContainer;
	public GameObject friendRoomPanelPrefab;
	private static FriendRoomList _instance;
	void Awake() {
		if(_instance == null) {
			//If I am the first instance, make me the Singleton
			_instance = this;
			DontDestroyOnLoad(this);
		}
	}
	
	public static FriendRoomList instance {
		get {
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<FriendRoomList>();
				
				//Tell unity not to destroy this object when loading a new scene!
				if(_instance != null) {
					DontDestroyOnLoad(_instance.gameObject);
				}
			}
			
			return _instance;
		}
	}
	public void refreshRoomList()
	{
		//check the number of prefabs in resource folder -> PetHome -> Inventory Assets
		//create number of panels according to the the number of prefabs found
		//change text of panel to the name of prefabs in the resource folder
		//allRawProps = (GameObject[])Resources.LoadAll("PetHome/InventoryAssets");
		//Debug.Log (inventoryPanelPrefab);

		Debug.Log ("initializing friend room list");
//        PhotonNetwork.Disconnect();
  //      PhotonNetwork.ConnectToRegion(CloudRegionCode.eu, "1.0");
//        PhotonNetwork.ConnectTo
		if (!PhotonNetwork.connected)
		{
			if (PhotonNetwork.connecting)
			{
				Debug.Log("Connecting to: " + PhotonNetwork.ServerAddress);
			}
			else
			{
				Debug.Log("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
			}
			
			if (roomController.GetComponent<RoomController>().connectFailed)
			{
				Debug.Log("Failed to connect");
			}
			
			return;
		}

        Debug.Log("check lobby "+PhotonNetwork.insideLobby);

		//Debug.Log(PhotonNetwork.GetRoomList().Length);

		foreach (Transform child in friendRoomPanelContainer.transform) {
			GameObject.Destroy(child.gameObject);
		}
		foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
		{
			//GUILayout.BeginHorizontal();
			//GUILayout.Label(roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
			//if (GUILayout.Button("Join", GUILayout.Width(150)))
			//{
			//	PhotonNetwork.JoinRoom(roomInfo.name);
			//}
			
			Debug.Log(roomInfo.name);
			GameObject insFriendPanel = Instantiate(friendRoomPanelPrefab) as GameObject ;
			insFriendPanel.transform.Find("Name").GetComponent<Text>().text = roomInfo.name;
			insFriendPanel.transform.SetParent(friendRoomPanelContainer.transform,false);
			
			//Debug.Log(rawProp.name);
			
			
			//GUILayout.EndHorizontal();
		}



	}
}
