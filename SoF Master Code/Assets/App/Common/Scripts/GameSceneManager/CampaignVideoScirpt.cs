using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignVideoScirpt : MonoBehaviour {

    // Use this for initialization
    GameSys.VideoState mState;
    public GameObject titleTextDisplay;

    void Start() {
        //Get the current video state out
        mState = (GameSys.VideoState)GameSys.StateManager.Instance.GetFirstState();
        
        MultiLanguage.getInstance().apply(titleTextDisplay, "campaign_video_title");

        titleTextDisplay.GetComponent<Text>().text = mState.Name;
    }

    // Update is called once per frame
    void Update() {

    }

    public void ExitScene()
    {
        mState.IsDone = true;
        MainNavigationController.GotoMainMenu(); 
    }
}
