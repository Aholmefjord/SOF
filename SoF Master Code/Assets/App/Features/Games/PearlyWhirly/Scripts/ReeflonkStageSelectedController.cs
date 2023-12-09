using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UniRx;
using SimpleJSON;

public class ReeflonkStageSelectedController : MonoBehaviour {


    public GameObject[] mapNodes;
    public bool isAR = false;


    Hashtable stages;
    int HighestReeflonkAchievementStorage;


    private int page;
	// Use this for initialization
	void Start () {
        page = 0;

        stages = GameState.configs["Stage"] as Hashtable;
        Debug.Log(stages.Count + " Amount of stages");
        FindHighestCompleted();
        SetMapNode();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    // return to "game0_start_scene"
    public void ReturnButtonClick()
    {
        MainNavigationController.GoToScene("game0_start_scene");
        return;
    }
    // change page and refresh Node
    public void OnChangeStagePage(int p)
    {
        page = p;
        RefreshMapNode();        
    }

    //Set isAR and refresh Node
    public void setAR(bool ar)
    {
        isAR = ar;
        Debug.Log("ARMode "+ar);
       RefreshMapNode();
    }

    // Set boolean AR of all node
    private void RefreshMapNode()
    {
        
        foreach (GameObject mapNode in mapNodes)
        {
            mapNode.GetComponent<MapNodeController>().SetAR(isAR);
        }
    }

    // set map node
    public void SetMapNode()
    {
        int tmpStars = 0;
        Debug.Log("Populating Nodes");
        for (int i = 0; i < 10; ++i)
        {
            int j = 0 + (page * 10) + i + 1;

            mapNodes[i].transform.GetChild(7).GetComponent<Text>().text = "Stage " + j + "";

            if (stages[j] != null && (IsUnlocked(j) || IsStartLevel(j)))
            {
                mapNodes[i].GetComponent<MapNodeController>().stageid = j;
                mapNodes[i].GetComponent<MapNodeController>().stage = stages[j] as Stage;

                // Unlock Node[i]
                LockUnlockNode(mapNodes[i], true);
                //Debug.Log("i : " + i);

                if (GameState.me.achievements.ContainsKey(j))
                {
                    // Depending on stars, choose the right display
                    if (GameState.me.achievements[j].progress >= (stages[j] as Stage).star3ScoreNeeded)
                    {
                        tmpStars = 3;
                    }

                    else if (GameState.me.achievements[j].progress >= (stages[j] as Stage).star2ScoreNeeded)
                    {
                        tmpStars = 2;
                    }

                    else if (GameState.me.achievements[j].progress >= (stages[j] as Stage).star1ScoreNeeded)
                    {
                        tmpStars = 1;
                    }
                    // Set text to "Clear"
                    mapNodes[i].transform.GetChild(5).GetComponent<Text>().text = "Clear!";
                    // Show Stars and pearl base on tmpStars
                    EnableStar(mapNodes[i], tmpStars);
                }
                else
                {
                    tmpStars = 0;
                    //Set text to "New" since there is no score on this node
                    mapNodes[i].transform.GetChild(5).GetComponent<Text>().text = "New!";
                }
                
                //mapNodes[i].GetComponentInChildren<UnityEngine.UI.Image>().sprite = mapNodes[i].GetComponent<MapNodeController>().starSprites[tmpStars];
            }
           
        }
    }

    //to do
    private void ShowScore()
    {
        // to do
        // wait for scoring system
    }

    // reset all node to lock
    private void ResetAllNode()
    {
        for (int i = 0; i < 10; ++i)
        {
            LockUnlockNode(mapNodes[i], false);            
        }
    }

    //Show lock or unlock image of node
    private void LockUnlockNode(GameObject node, bool unlock)
    {
        // unlock node
        if (unlock)
        {
            // Show unlock image such as normal shell, text
            node.GetComponent<Button>().enabled = true;
            node.transform.GetChild(0).gameObject.SetActive(false);
            node.transform.GetChild(1).gameObject.SetActive(true);
            node.transform.GetChild(5).gameObject.SetActive(true);
        }
        /// Reset Node hide star , pearl
        else
        {
            // Show lock image
            node.GetComponent<Button>().enabled = false;
            node.transform.GetChild(0).gameObject.SetActive(true);
            node.transform.GetChild(1).gameObject.SetActive(false);
            node.transform.GetChild(5).gameObject.SetActive(false);

            GameObject PearlsList = node.transform.GetChild(2).gameObject;
            GameObject StarList = node.transform.GetChild(4).gameObject;
            // disable all stars nad pearl
            for (int i = 0; i < 3; i++)
            {
                StarList.transform.GetChild(i).gameObject.SetActive(false);
                PearlsList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        
    }
    /// display Stars and peral
    private void EnableStar(GameObject node,int stars)
    {
        GameObject PearlsList = node.transform.GetChild(2).gameObject;
        GameObject StarList = node.transform.GetChild(4).gameObject;

        PearlsList.transform.GetChild(stars-1).gameObject.SetActive(true);

        for(int i = 0; i < stars; i++)
        {
            StarList.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    // check that id is start level or not
    private bool IsStartLevel(int id)
    {
        if ((id % 1000) == 1)
            return true;
        else
            return false;
    }
    //Check that this id is unlock or not
    private bool IsUnlocked(int id)
    {
        int tmpID = id / 1000;
        if (id - 1 <= HighestReeflonkAchievementStorage)
            return true;
        else
            return false;
    }
    //Find highest complete stage
    public void FindHighestCompleted()
    {
        foreach (KeyValuePair<int, PlayerAchievement> pa in GameState.me.achievements)
        {
            int tmpAchieve = pa.Value.achievementId;

            if (tmpAchieve <= 1000)
            {
                if (tmpAchieve > HighestReeflonkAchievementStorage)
                {
                    // Store the highest for Reefwalk
                    HighestReeflonkAchievementStorage= tmpAchieve;
                }
            }            
        }
    }

    //Page Left click
    public void PageLeftClick()
    {
        if(page>0)
        {
            page--;
            ResetAllNode();
            SetMapNode();
        }
            
        
    }
    //Page Right click
    public void PageRightClick()
    {
        if (page < 5)
        {
            page++;
            ResetAllNode();
            SetMapNode();
        }
            
       
    }

}
