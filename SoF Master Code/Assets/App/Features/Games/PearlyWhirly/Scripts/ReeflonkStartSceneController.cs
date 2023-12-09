using UnityEngine;
using System.Collections;

public class ReeflonkStartSceneController : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		AudioSystem.PlayBGM("Game0_");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void ReturnButtonClick()
    {
        MainNavigationController.GoToMap();
        return;
    }
    public void PlayButtonClick()
    {
        MainNavigationController.GoToScene("game0_stage_selected");
        return;
    }

}
