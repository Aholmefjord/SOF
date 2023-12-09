// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkerMenu.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoomController : MonoBehaviour
{
	private Vector2 scrollPos = Vector2.zero;
	
	public bool connectFailed = false;
	
	public static readonly string SceneNameMenu = "mapNew";
	
	public static readonly string SceneNameGame = "mapNewMulti";
	
	private string errorDialog;
	private double timeToClearDialog;
	
	public string ErrorDialog
	{
		get { return this.errorDialog; }
		private set
		{
			this.errorDialog = value;
			if (!string.IsNullOrEmpty(value))
			{
				this.timeToClearDialog = Time.time + 4.0f;
			}
		}
	}
	
	public void Awake()
	{

	}

	public void init(){

		// this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
		PhotonNetwork.automaticallySyncScene = true;

        // the following line checks if this client was just created (and not yet online). if so, we connect
        if (PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
        {
			// Connect to the photon master-server. We use the settings saved in PhotonServerSettings (a .asset file in this project)
			PhotonNetwork.ConnectUsingSettings("0.9");
		}
		
		// generate a name for this player, if none is assigned yet
		if (String.IsNullOrEmpty(PhotonNetwork.playerName))
		{
			try
			{
				PhotonNetwork.playerName = GameState.playerBuddy.name;
			}
			catch {
			}
		}
		//PhotonNetwork.logLevel = NetworkLogLevel.Full;
	}

	

	public void onInviteFriendPress(){
		AudioSystem.PlaySFX("UI/sfx_ui_click");
		DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialgo_box_message_creating_room"), "", DialogueBoxController.Type.NoButtons);
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
			
			if (this.connectFailed)
			{
				Debug.Log("Connection failed. Check setup and use Setup Wizard to fix configuration.");
				Debug.Log(String.Format("Server: {0}", new object[] {PhotonNetwork.ServerAddress}));
				Debug.Log("AppId: " + PhotonNetwork.PhotonServerSettings.AppID.Substring(0, 8) + "****"); // only show/log first 8 characters. never log the full AppId.

			}
			DialogueBoxController.Hide();
			return;
		}

		if (GameState.me != null && GameState.me.username != null) {
			Debug.Log (GameState.me.username);
			PhotonNetwork.playerName = GameState.me.username;
			PhotonNetwork.CreateRoom (GameState.playerBuddy.name + MultiLanguage.getInstance().getString("buddy_home_chat_panel_room"), new RoomOptions () {maxPlayers = 20}, null);
		}

	}

	
	// We have two options here: we either joined(by title, list or random) or created a room.
	public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom");
	}
	
	public void OnPhotonCreateRoomFailed()
	{
		ErrorDialog = "Error: Can't create room (room name maybe already used).";
		Debug.Log("OnPhotonCreateRoomFailed got called. This can happen if the room exists (even if not visible). Try another room name.");
	}
	
	public void OnPhotonJoinRoomFailed(object[] cause)
	{
		ErrorDialog = "Error: Can't join room (full or unknown room name). " + cause[1];
		Debug.Log("OnPhotonJoinRoomFailed got called. This can happen if the room is not existing or full or closed.");
	}
	
	public void OnPhotonRandomJoinFailed()
	{
		ErrorDialog = "Error: Can't join random room (none found).";
		Debug.Log("OnPhotonRandomJoinFailed got called. Happens if no room is available (or all full or invisible or closed). JoinrRandom filter-options can limit available rooms.");
	}
	
	public void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		PhotonNetwork.LoadLevel(SceneNameGame);
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
	public void OnReceivedRoomListUpdate()
	{
		Debug.Log("check room list"+PhotonNetwork.GetRoomList().Length);
		//List here.
	}
}
