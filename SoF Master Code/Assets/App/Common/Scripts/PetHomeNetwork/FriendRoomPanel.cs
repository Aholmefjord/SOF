using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FriendRoomPanel : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onThisPanelPress(){
		
		DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialgo_box_message_joining_room"), "", DialogueBoxController.Type.NoButtons);
		PhotonNetwork.JoinRoom(transform.Find("Name").GetComponent<Text>().text);
	}
}
