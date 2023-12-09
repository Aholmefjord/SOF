using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChompBar : MonoBehaviour
{

    public GameObject FollowProgress;
    public GameObject Barsplitter;
    public GameObject shortLine;
    public GameObject indicator;
    public GameObject BarFiller;
    private UIManager uiManager;
    public GameObject buddyanim;
    public GameObject BarPopup;
    public static bool Camefromchomp;
    int buddypos = 0;
    float maxvalue = -0.55f;
    float minvalue = 0.55f;
    bool updateposition = false;
    string completed = "";
    //the buddy flip 180
    int Amounttoclear = 0;
    bool trigger = false;
    GameSys.GameState mState;
    public void SetupUI()
    {
        // header text 
        // kai shi text  
        // 1/5 completed- completed 
        

        MultiLanguage.getInstance().applyImage(gameObject.FindChild("GameTitle").GetComponent<Image>(), "chomp_title_image");
        MultiLanguage.getInstance().apply(gameObject.FindChild("CompletedText"), "chomp_completed_text");
        MultiLanguage.getInstance().apply(gameObject.FindChild("TitleText"), "chomp_title_text");
        MultiLanguage.getInstance().apply(gameObject.FindChild("StartText"), "chomp_start_text");
        
    }

    // Use this for initialization
    void Start()
    {
        mState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();
        Amounttoclear = (mState.endLevel);

        //5 = from textfile
        uiManager = new UIManager(gameObject);
        //string chompKey = GameSceneManager.getInstance().GetCurrentLessonNumber() + "-" + GameSceneManager.getInstance().GetCurrentLessonPartNumber();
        string chompKey = "ChompChompLevelKey";
        //lesson variable 

        uiManager["BarFiller"].GetComponent<Slider>().value = PlayerPrefs.GetInt(chompKey, 0) + 1;    //Start from progression, +1 is to temp fix the progression bug
       
        Debug.Log("+valueofproggres "+PlayerPrefs.GetInt(chompKey, 0));
        Debug.Log("slidervalue " + uiManager["BarFiller"].GetComponent<Slider>().value);

        uiManager["BarFiller"].GetComponent<Slider>().maxValue = Amounttoclear;
        SetupUI();
        string tempstring = gameObject.FindChild("CompletedText").GetComponent<Text>().text;
        uiManager["BarSplitter"].Clear();

        //boxes + activate active 

         
        
        for (var i = 0; i < (Amounttoclear - 1); ++i)
        { 
            GameObject barLine = Instantiate(shortLine ); // base on lesson count >10 1 more
            barLine.transform.SetParent(Barsplitter.transform,false);
            barLine.transform.localScale = new Vector3(2, 2, 2);

            if(BarFiller.GetComponent<Slider>().value>(i+1))
            {
                GameObject newPopup = Instantiate(indicator);
                newPopup.SetText(((i + 1) + "/" + Amounttoclear + "   " + tempstring).ToString());
                newPopup.transform.SetParent(barLine.transform, false);
                newPopup.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            }

        }
        Barsplitter.GetComponent<GridLayoutGroup>().spacing = new Vector2(((gameObject.FindChild("BarBg").GetComponent<RectTransform>().rect.width) / (Amounttoclear)), 0);
      
        FollowProgress.transform.localPosition = new Vector3(BarFiller.GetComponent<Slider>().value * Barsplitter.GetComponent<GridLayoutGroup>().spacing.x, 0, 0);


         
        WalkingAvatar.canWalk = false;
        //float temp = (BarFiller.GetComponent<Slider>().value) * (Barsplitter.GetComponent<GridLayoutGroup>().spacing.x);
        //for (int i=0;i< BarFiller.GetComponent<Slider>().value;i++)
        //{
        //    Barsplitter.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
        //}
    
   
    }
 

    // Update is called once per frame
    void Update()
    {


        WalkingAvatar.canWalk = false;
        if(updateposition==false)
        {
            float temppos = 0;
            temppos = maxvalue - minvalue;
            temppos /= Amounttoclear;
            temppos *= BarFiller.GetComponent<Slider>().value;
            temppos -= maxvalue;

            buddyanim.transform.GetChild(0).localPosition = new Vector3(temppos, -0.2f, 0);

            if (BarFiller.GetComponent<Slider>().value == 0)
                buddyanim.transform.GetChild(0).localPosition = new Vector3(minvalue, -0.2f, 0);
            if (BarFiller.GetComponent<Slider>().value == Amounttoclear)
                buddyanim.transform.GetChild(0).localPosition = new Vector3(maxvalue, -0.2f, 0);

          //  updateposition = true;
        }




        if (!buddyanim.transform.GetChild(0).GetComponent<Animator>().GetBool("Receiving") && trigger == false)
        {
            trigger = true;
            buddyanim.transform.GetChild(0).GetComponent<Animator>().SetBool("Receiving", true);
        }

        if (buddyanim.transform.GetChild(0).GetComponent<Animator>().GetBool("Receiving"))
        {
            buddyanim.transform.GetChild(0).GetComponent<Animator>().SetBool("Wave", true);
            buddyanim.transform.GetChild(0).GetComponent<Animator>().SetBool("Receiving", false);
        }

        else if (buddyanim.transform.GetChild(0).GetComponent<Animator>().GetBool("Wave"))
        {
            buddyanim.transform.GetChild(0).GetComponent<Animator>().SetBool("Receiving", true);
            buddyanim.transform.GetChild(0).GetComponent<Animator>().SetBool("Wave", false);
        }

        // buddypos.transform.GetChild(0).localPosition =new Vector3((BarFiller.GetComponent<Slider>().value * ), 0, 0);
        FollowProgress.transform.localPosition = new Vector3((BarFiller.GetComponent<Slider>().value * Barsplitter.GetComponent<GridLayoutGroup>().spacing.x - 752), 0, 0);

    }
    public void GoToCampaignLevel()
    {
        //Quit state
        mState.IsDone = true;

        MainNavigationController.GotoMainMenu();
    }

    public void GoToChompChomp()
    {
        if (Application.loadedLevelName == "Chomp_Level")
            Camefromchomp = true;
        //MainNavigationController.GoToScene("game3");
        MainNavigationController.DoAssetBundleLoadLevel(Constants.CHOMPCHOMP_SCENES, "game3");
    }

    public void Addvalue()
    {
        BarFiller.GetComponent<Slider>().value += 1;
    }
}
