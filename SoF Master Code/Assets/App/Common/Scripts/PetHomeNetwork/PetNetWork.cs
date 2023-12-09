using UnityEngine;
using UnityEngine.SceneManagement;
using ExitGames.Demos.DemoPunVoice;
public class PetNetWork : Photon.MonoBehaviour
{
	public Transform playerPrefab;
	public GameObject ownPlayerAvatar;
    public GameObject audioPrefab;
    public AINode startNode;
    public AINode endNode;
    private static PetNetWork _instance;
    public GameObject goBGM;
	void Awake() {

        if (GameObject.Find("Audio_BGM") != null) {
            goBGM = GameObject.Find("Audio_BGM");
            goBGM.GetComponent<AudioSource>().mute = true;
        }
		_instance = this;
		if (!PhotonNetwork.connected)
		{
			SceneManager.LoadScene("home");
			return;
		}
		// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
		string avatarPath = AvatarLoader.current.GetAvatarBasePath();
		string avatarSkinPath = AvatarLoader.current.GetAvatarSkinPath();
		Debug.Log("Try network create " + avatarPath);

		object[] networkData = new object[2];
		networkData[0] = avatarPath;
		networkData[1] = avatarSkinPath;
        //playerPrefab.gameObject.SetActive(true);
		ownPlayerAvatar = PhotonNetwork.Instantiate(this.playerPrefab.name, HomeNetworkManager.instance.entrancePos.transform.position, Quaternion.identity, 0, networkData) as GameObject;
		Debug.LogError("transform position: " + HomeNetworkManager.instance.entrancePos.transform.position);

        Debug.Log("Passed Object Data: " + ownPlayerAvatar.GetPhotonView().instantiationData);
        ownPlayerAvatar.SetActive(true);
        ownPlayerAvatar.GetComponent<AvatarInstanceController>().loadMaterials = true;
        
        Debug.Log("Spawning");

        //playerPrefab.gameObject.SetActive(false);

        //              ownPlayerAvatar.AddComponent(typeof(AudioSource));
        //        ownPlayerAvatar.GetComponent<PhotonVoiceRecorder>().enabled = true;
        //      mainCamera.GetComponent<NetworkFFFollowCamera> ().m_Target = ownPlayerAvatar.transform;
        //        this.gameObject.AddComponent(typeof(PhotonVoiceNetwork));
    }
    bool triedConnect = false;
    void update()
    {
        //if (!triedConnect)
        //{
      //      triedConnect = true;
//            PhotonVoiceNetwork.Connect();
  //          Debug.Log("TriedConnecting");
    //        ownPlayerAvatar.GetComponent<PhotonVoiceRecorder>().enabled = true;
//        }
    }
    public static PetNetWork instance {
		get {
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<PetNetWork>();
			}
			
			return _instance;
		}
	}

	public void OnMasterClientSwitched(PhotonPlayer player)
	{
		Debug.Log("OnMasterClientSwitched: " + player);
		
		string message;
		InRoomChat chatComponent = GetComponent<InRoomChat>();  // if we find a InRoomChat component, we print out a short message
		
		if (chatComponent != null)
		{
			// to check if this client is the new master...
			if (player.isLocal)
			{
				message = "You are hosting now.";
			}
			else
			{
				message = player.name + " is hosting now.";
			}
			
			
//			chatComponent.AddLine(message); // the Chat method is a RPC. as we don't want to send an RPC and neither create a PhotonMessageInfo, lets call AddLine()
		}
	}
	
	public void OnLeftRoom()
	{
		Debug.Log("OnLeftRoom (local)");
        goBGM.SetActive(true);
		// back to main menu
		SceneManager.LoadScene("mapNew");
	}
	
	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("OnDisconnectedFromPhoton1");
        goBGM.GetComponent<AudioSource>().enabled = false;
        // back to main menu
        SceneManager.LoadScene("mapNew");
	}
	
	public void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("OnPhotonPlayerConnected1: " + player);
	}
	
	public void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		Debug.Log("OnPlayerDisconneced1: " + player);
	}
	
	public void OnFailedToConnectToPhoton()
	{
		Debug.Log("OnFailedToConnectToPhoton");
		
		// back to main menu
		SceneManager.LoadScene("home");
	}
}
