using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class CampaignLevelController : BaseUI {
    /*
    public GameObject dottrailpattern;
    public GameObject maincointainer;
    public GameObject alllevels;
    public Transform StageContainer;
    public GameObject page;
    public GameObject arrows;
    public Transform pattern1;
    public Transform pattern2;
    public Transform pattern3;
    public Transform pattern4;
    public Transform pattern5;
     float TotalLevel = 0;
    int leveltospawnnode = 0;
    bool hasfinish = false;
    bool spawnfinish = false;
    List<int> listofcolour = new List<int>();
    private int displayLevels = -1;
    private int displayCurrentStars = -1;
    private int currentPage = -1, displayPage = -1;
    private GameSys.GameState _Gamestate;
    string temptext="temp";
    public void SetupUI()
    {
        
        MultiLanguage.getInstance().apply(gameObject.FindChild("ExplorationText"), "campaignlevel_header");
        MultiLanguage.getInstance().apply(gameObject.FindChild("LockedText"), "campaignlevel_locked_text");
        MultiLanguage.getInstance().apply(gameObject.FindChild("NewText"), "campaignlevel_new_text");
        MultiLanguage.getInstance().apply(gameObject.FindChild("CompletedText"), "campaignlevel_completed_text");
  
       

    }
    // Use this for initialization
   protected override void Start () {
        SetupUI();
        for(int i=0;i<5;i++)
        {
            listofcolour.Add(i);
        }
      
        
      
        for (int i = 0; i < dottrailpattern.transform.childCount; i++)
        {
            dottrailpattern.transform.GetChild(i).gameObject.SetActive(false);
        }
        hasfinish = false;
        spawnfinish = false;

        //LoadMapNodes();
        maincointainer.SetActive(false);
      

    }
	
	// Update is called once per frame
	void Update () {
         LoadMapNodes();
      
    }
    public void LoadMapNodes()
    {
        TotalLevel=CampaignMapDetailSingleton.Instance.GetTotalLessonCount();

        if(displayLevels==-1)
        {
            displayLevels = (int)TotalLevel;
        }
       else if (displayLevels!=(int)TotalLevel)
        {
            displayLevels = (int)TotalLevel;
            alllevels.Clear();
            currentPage = 0;
        }

        int totalpage = (int)System.Math.Ceiling(TotalLevel / 10);


        var pagebookmark = new float[totalpage];
        var currentstar = 0;

        page.transform.GetChild(0).gameObject.SetText((currentPage + 1).ToString());
        page.transform.GetChild(2).gameObject.SetText(alllevels.transform.childCount.ToString());
        //arrow child 0 = left 

        arrows.transform.GetChild(0).gameObject.SetActive(currentPage != 0);
        arrows.transform.GetChild(0).gameObject.OnClick(() => { --currentPage; hasfinish = false; });

        arrows.transform.GetChild(1).gameObject.SetActive(currentPage < (alllevels.transform.childCount - 1));
        arrows.transform.GetChild(1).gameObject.OnClick(() => { ++currentPage; hasfinish = false; });
        if (!hasfinish)
        {
            hasfinish = true;


       


            if (spawnfinish == false)
            {
                //totallevel in the term
                for (int i = 0; i < (int)System.Math.Ceiling(TotalLevel / 10); i++)
                {
                
                    //5 map to random 
                   
                    Instantiate(Resources.Load("CampaignUI/pattern" + (listofcolour[i] + 1)), alllevels.transform, false); // base on lesson count >10 1 more
                 
                    

                }

                //adjust pages 

                for (var i = 0; i < TotalLevel; ++i)
                {
                    float temp = i;

                    if (temp != 0)
                    {
                        leveltospawnnode = (int)System.Math.Floor(temp / 10);
                    }
                    else
                    {
                        leveltospawnnode = 0;
                    }

                    Debug.Log(leveltospawnnode + "spawnnode");


                    if (leveltospawnnode != 0)
                    {
                        temp -= leveltospawnnode * 10;

                    }
                    Vector3 tempos = alllevels.transform.GetChild(leveltospawnnode).GetChild((int)temp).position;


                    //get first child(10 node) in all level their i-10 pos 

                    var go = Instantiate(StageContainer, new Vector3(tempos.x, tempos.y, tempos.z), Quaternion.identity, alllevels.transform.GetChild(leveltospawnnode).GetChild((int)temp));

                    //////var currentLevel = GameState.pearlyProg.GetLevel(_Gamestate.startLevel + i - 1);

                    //////  if(currentLevel.status== PearlyProgression.ELevelStatus.Locked )
                    //////  {
                    //////      go.gameObject.FindChild("Locked").SetActive(true);
                    //////      go.gameObject.FindChild("New").SetActive(false);
                    //////      go.gameObject.FindChild("Completed").SetActive(false);

                    //////      // local variable int to use so that the i in all doesnt become last i 
                    //////      int templock = i;
                    //////      //this handle the stage number to find in new // find NEW/LOCKED/Complete per case 
                    //////      go.Find("Locked").gameObject.FindChild("StageNumber").SetText((templock + 1).ToString());
                    //////      go.gameObject.OnClick(() => CampaignMapDetailSingleton.Instance.LaunchLesson((templock + 1)));

                    //////  }
                    //////  else if (currentLevel.status == PearlyProgression.ELevelStatus.Available)
                    //////  {
                    //////      go.gameObject.FindChild("Locked").SetActive(false);
                    //////      go.gameObject.FindChild("New").SetActive(true);
                    //////      go.gameObject.FindChild("Completed").SetActive(false);

                    //////      // local variable int to use so that the i in all doesnt become last i 
                    //////      int tempnew = i;
                    //////      //this handle the stage number to find in new // find NEW/LOCKED/Complete per case 
                    //////      go.Find("New").gameObject.FindChild("StageNumber").SetText((tempnew + 1).ToString());
                    //////      go.gameObject.OnClick(() => CampaignMapDetailSingleton.Instance.LaunchLesson((tempnew + 1)));

                    //////  }
                    //////  else
                    //////  {
                    //////      go.gameObject.FindChild("Locked").SetActive(false);
                    //////      go.gameObject.FindChild("New").SetActive(false);
                    //////      go.gameObject.FindChild("Completed").SetActive(true);

                    //////      // local variable int to use so that the i in all doesnt become last i 
                    //////      int tempclear = i;
                    //////      //this handle the stage number to find in new // find NEW/LOCKED/Complete per case 
                    //////      go.Find("Completed").gameObject.FindChild("StageNumber").SetText((tempclear + 1).ToString());
                    //////      go.gameObject.OnClick(() => CampaignMapDetailSingleton.Instance.LaunchLesson((tempclear + 1)));
                    //////  }
                    go.gameObject.FindChild("Locked").SetActive(false);
                    go.gameObject.FindChild("New").SetActive(false);
                    go.gameObject.FindChild("Completed").SetActive(true);

                    // local variable int to use so that the i in all doesnt become last i 
                    int tempi = i;
                    //this handle the stage number to find in new // find NEW/LOCKED/Complete per case 
                    go.Find("Completed").gameObject.FindChild("StageNumber").SetText((tempi + 1).ToString());
                    //go.gameObject.OnClick(() => CampaignMapDetailSingleton.Instance.LaunchLesson((tempi + 1)));
                    
                    //CampaignMapDetailSingleton.Instance.GetMapIcon(i).name;

                    if (CampaignMapDetailSingleton.Instance.GetMapIcon(i).type == "video")
                    {
                        go.Find("Completed").Find("Box").Find("VideoBox").gameObject.SetActive(true);
                        go.Find("Completed").GetComponent<Button>().targetGraphic = go.Find("Completed").Find("Box").Find("VideoBox").GetComponent<Image>();

                    }
                    else if (CampaignMapDetailSingleton.Instance.GetMapIcon(i).name == "pearly")
                    {
                        go.Find("Completed").Find("Box").Find("PearlBox").gameObject.SetActive(true);
                        go.Find("Completed").GetComponent<Button>().targetGraphic = go.Find("Completed").Find("Box").Find("PearlBox").GetComponent<Image>();
                    }
                    else if (CampaignMapDetailSingleton.Instance.GetMapIcon(i).name == "manta")
                    {
                        go.Find("Completed").Find("Box").Find("MantaBox").gameObject.SetActive(true);
                        go.Find("Completed").GetComponent<Button>().targetGraphic = go.Find("Completed").Find("Box").Find("MantaBox").GetComponent<Image>();
                    }


                    else if (CampaignMapDetailSingleton.Instance.GetMapIcon(i).name == "tumble ")
                    {
                        go.Find("Completed").Find("Box").Find("TumbleBox").gameObject.SetActive(true);
                        go.Find("Completed").GetComponent<Button>().targetGraphic = go.Find("Completed").Find("Box").Find("TumbleBox").GetComponent<Image>();
                    }
                    else if (CampaignMapDetailSingleton.Instance.GetMapIcon(i).name == "crab")
                    {
                        go.Find("Completed").Find("Box").Find("CrabBox").gameObject.SetActive(true);
                        go.Find("Completed").GetComponent<Button>().targetGraphic = go.Find("Completed").Find("Box").Find("CrabBox").GetComponent<Image>();
                    }
                    else if (CampaignMapDetailSingleton.Instance.GetMapIcon(i).name == "chomp")
                    {
                        go.Find("Completed").Find("Box").Find("ChompBox").gameObject.SetActive(true);
                        go.Find("Completed").GetComponent<Button>().targetGraphic = go.Find("Completed").Find("Box").Find("ChompBox").GetComponent<Image>();
                    }
                    else
                    {
                        go.Find("Completed").Find("Box").GetChild(listofcolour[leveltospawnnode]).gameObject.SetActive(true);
                        //box= button
                        go.Find("Completed").GetComponent<Button>().targetGraphic = go.Find("Completed").Find("Box").GetChild(listofcolour[leveltospawnnode]).GetComponent<Image>();
                    }

                  
                  
                        MultiLanguage.getInstance().apply(go.gameObject.FindChild("NameText"), "campaignlevel_nametext");
                    if (CampaignMapDetailSingleton.Instance.GetMapIcon(i).type == "video")
                        go.gameObject.FindChild("NameText").GetComponent<Text>().text = MultiLanguage.getInstance().getString("game_name_" + CampaignMapDetailSingleton.Instance.GetMapIcon(i).type);
                    else
                    go.gameObject.FindChild("NameText").GetComponent<Text>().text = MultiLanguage.getInstance().getString("game_name_" + CampaignMapDetailSingleton.Instance.GetMapIcon(i).name);

                    temptext = MultiLanguage.getInstance().getString("game_name_" + CampaignMapDetailSingleton.Instance.GetMapIcon(i).name);
                    Debug.Log("testtest" + temptext);
                    MultiLanguage.getInstance().apply(go.gameObject.FindChild("LevelText"), "campaignlevel_leveltext");
                    go.gameObject.FindChild("LevelText").GetComponent<Text>().text = CampaignMapDetailSingleton.Instance.GetMapIcon(i).description;

                    spawnfinish = true;
                }
            }
            for (int p = 0; p < alllevels.transform.childCount; p++)
            {
                var tempgo = alllevels.transform.GetChild(p).gameObject;
                
                 pagebookmark[p] = (tempgo.transform.localPosition.x-5);
            }
            if (displayPage == -1)
            {
                displayPage = currentPage = 0;
            }
            else if (displayPage != currentPage)
            {
                Debug.Log("Move");
                displayPage = currentPage;
                alllevels.transform.DOLocalMoveX(-pagebookmark[displayPage], 0.5f).SetUpdate(true);
                
            }
        }
       
    }
    public void GoToChompLevel()
    {
        MainNavigationController.GoToScene("Chomp_Level");
    }
    public void GoToCampaignMode()
    {
        MainNavigationController.GoToScene("CampaignMode");
    }
    public void SelectLevel(int levelID)
    {
        Debug.Log(levelID + "selected lvl");

    }
    public void GoToStartMenu()
    {
        MainNavigationController.GoToScene("start_menu");
    }
    protected override Sequence TweenIn
    {
        get
        {
               return DOTween.Sequence()
                .Append(canvasGroup.DOFade(1f, 1f));
        }
    }
    protected override Sequence TweenOut
    {
        get
        {
            return DOTween.Sequence()
                .Append(canvasGroup.DOFade(0f, 1f))
                .AppendCallback(() => BlankScreenUI.Current.CloseImmediately());
        }
    }
    */
}
