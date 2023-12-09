using UnityEngine;
using System.Collections;

public class MapNodeController : MonoBehaviour {

    public int gameId;
    public int stageid;
    public int puzzleLevel;
    public string maptype;
    public Stage stage;
    public int level;
    public int stars;
    public string returnScene;
    public bool isAR = true;


    [SerializeField]
    public Sprite[] starSprites;

    // This will be the class to go to the specific level
    public void SetAR(bool AR)
    {
        isAR = AR;
    }
    public void GoToLevel()
    {
		Debug.LogError("disabled sfx");
		//AudioSystem.PlaySFX("UI/sfx_ui_click");

        if (stage == null)
        {
            //Test or offline mode
            // MainNavigationController.GoToGame(stageid, returnScene);
            print("ERROR! NO SUCH LEVEL!");
        }
        else
        {
			//1337 HACK
			if (stageid > 5000 && stageid < 6000)
			{
				Debug.Log("Setting ID : " + stageid);
				MainNavigationController.setStage(stageid);
				MainNavigationController.GoToScene("game6_start_menu", "mapNew");
			}
            if(stageid > 4000 && stageid < 5000)
            {
                MainNavigationController.setStage(stageid);
                MainNavigationController.GoToScene("game5_start_menu", "mapNew");
            }
            else if (stageid <1000)
            {
                // if selected stage is reeflonk AR 1-15
                if(stageid >=1 && stageid<=30)
                {
                    if (isAR)
                    {
                        PlayerPrefs.SetInt("PEARLYWHIRLY", stageid);
                        MainNavigationController.setStage(stageid);
                        MainNavigationController.DoLoad("game0_level0_AR", "game0_stage_selected");
                    }
                    else
                    {
                        Debug.Log("Stage Selected: " + stageid);
                        PlayerPrefs.SetInt("PEARLYWHIRLY", stageid);
                        MainNavigationController.setStage(stageid);
                        MainNavigationController.DoLoad("game0_level0", "game0_stage_selected");
                        //MainNavigationController.GoToGame(stageid, "mapNew");
                    }
                    
                }
                else
                {
                    Debug.Log("No more map");
                    MainNavigationController.GoToScene("game0_start_scene");
                    
                    //MainNavigationController.GoToGame(stageid, "mapNew");
                }
            }
            else if (stageid > 1000 && stageid < 2000)
            {
                PlayerPrefs.SetInt("CRAB", stageid);
                MainNavigationController.setStage(stageid);
                MainNavigationController.DoLoad("InfiniteBrothers", "crab_stage_selected");
            }else if(stageid > 5000 && stageid < 6000)
            {
                PlayerPrefs.SetInt("CRAB", stageid-5000);
                MainNavigationController.setStage(stageid);
                MainNavigationController.DoLoad("<SCENE NAME>", "game5_stage_selected");
            }
			else
			{
				switch (stageid)
				{
					case 1:
						MainNavigationController.GoToScene("Cutscene");
						break;
					case 2001:
						MainNavigationController.GoToScene("Cutscene_3");
						break;
					default:
						MainNavigationController.GoToGame(stageid, "mapNew");
						break;
				}
			}
            //MainNavigationController.GoToGame(stageid, returnScene);
            //MainNavigationController.GoToGame(stage.id, returnScene);
        }
    }
}
