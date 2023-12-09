using UnityEngine;
using System.Collections;

public class CharacterSpawner : Photon.MonoBehaviour
{
    private static CharacterSpawner _instance;
    public static CharacterSpawner instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<CharacterSpawner>();
            }

            return _instance;
        }
    }
    void Awake()
    {
        _instance = this;
    }
    // Use this for initialization
    void Start()
    {
        this.init();
    }

    public void init()
    {
//        if (PhotonNetwork.isMasterClient)
//            HouseNameText.text = GameState.playerBuddy.name.ToUpper() + "'S HOME";
  //      else
    //        HouseNameText.text = PhotonNetwork.room.name;
    }
    public void leaveRoom()
    {
        if (PhotonNetwork.isMasterClient)
        {
            foreach (PhotonPlayer pPlayer in PhotonNetwork.playerList)
            {
                PhotonNetwork.CloseConnection(pPlayer);
                PhotonVoiceNetwork.Disconnect();
            }//PhotonVoiceNetwork.Connect();
        }
        else
        {
            PhotonNetwork.LeaveRoom();
            PhotonVoiceNetwork.Disconnect();
        }
        GameObject.Find("Audio_BGM").GetComponent<AudioSource>().mute = false;
    }
    
    // Update is called once per frame
    void Update () {
	
	}
}
