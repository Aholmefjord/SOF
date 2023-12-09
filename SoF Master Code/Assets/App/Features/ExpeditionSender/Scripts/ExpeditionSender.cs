using UnityEngine;
using System.Collections;
using SimpleJSON;
using System;
using UnityEngine.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UniRx;
using System.Security.Cryptography;

namespace FDL.Library.Numeric
{
    public static class RandomNumber
    {
        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

        public static int Between(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            // We are using Math.Max, and substracting 0.00000000001, 
            // to ensure "multiplier" will always be between 0.0 and .99999999999
            // Otherwise, it's possible for it to be "1", which causes problems in our rounding.
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            // We need to add one to the range, to allow for the rounding done with Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }
    }
}
    class ConvertedDate
{
    public DateTime Date { get; set; }
}
public class ExpeditionSender : MonoBehaviour {
    public ResourceController controller;
    string current_time;
    //DateTime beforeTime; // DEPRECATED BY HOWARD!!!!
    public GameObject buddyLocationToMove;
    public GameObject wanderingAvatar;
    public GameObject avatar;
    public Text timeText;
    public Text hoursText;

    MultiLanguageApplicator textProc;

    #region Singleton (w/ Awake(), OnDestroy())
    static ExpeditionSender _instance;
    public static ExpeditionSender Instance {
        get {
            return _instance;
        }
    }
    void Awake()
    {
        if (_instance == null) {
            _instance = this;

        } else {
            Destroy(gameObject);
        }
    }
    private void OnDestroy()
    {
        _instance = null;
    }
    #endregion

    public bool available
	{
		get
		{
			if (GameState.playerBuddy.expeditionCompleteTime > TimeHelper.ServerEpochTime)
				return false;
			
			return true;
		}
	}
	//private bool mAvailable = false;

    public GameObject expeditionButton;
    public GameObject returnButton;
    public GameObject BuddyReturnedPanel;

    private int currentVideoState;
    private bool ExpeditionSent = false; //variable to set the correct uGem after returning
    
    bool isLocal = false;

    private void OnEnable()
    {
        MultiLanguage.getInstance().OnLanguageChanged += LanguageChangedCallback;
    }
    private void OnDisable()
    {
        MultiLanguage.getInstance().OnLanguageChanged -= LanguageChangedCallback;
    }
    void LanguageChangedCallback(string prev, string curr)
    {
        SetupUI();
    }

    void Start ()
    {
        SetupUI();

        try
        {
            if (GameState.playerBuddy != null)
			{
				/* // DEPRECATED BY HOWARD
                if (GameState.playerBuddy.expedition_date.Replace(@"\s+", "") != "")
                {
                    try
                    {
                        JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
                        {
                            //                    DateTimeFormat = "yyyy-MM-ddThh:mm:ssZ";
                            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                            DateTimeZoneHandling = DateTimeZoneHandling.Local
                        };
                        beforeTime = JsonConvert.DeserializeObject<DateTime>(GameState.playerBuddy.expedition_date, microsoftDateFormatSettings);
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log(e);
                    }
                    available = GameState.playerBuddy.hasClaimedExpedition;
                    
                }
                else
                {
                    available = true;
                }
                */
            }
        }
		catch(System.Exception e) 
		{ 
		}

        if (GameState.playerBuddy.time_required == 0) 
			GameState.playerBuddy.time_required = 1;
        
		hoursText.text = "" + GameState.playerBuddy.time_required;
    }
    
    public void SetupUI()
    {
        if(textProc==null)
            textProc = new MultiLanguageApplicator(gameObject);

        textProc.ApplyText("GoExpedition/GoButton/SortText (1)"                     , "buddy_home_expedition_go_button");
        textProc.ApplyText("ExpeditionPanel/PanelTitle"                             , "buddy_home_expedition_title");
        textProc.ApplyText("ExpeditionPanel/SallyButton/SallyButton"                , "buddy_home_expedition_sally");
        textProc.ApplyText("ExpeditionPanel/CoralReefButton/CoralReefButton"        , "buddy_home_expedition_coral");
        textProc.ApplyText("ExpeditionPanel/FindingsPanel/FindingsText"             , "buddy_home_expedition_possible_findings");
        textProc.ApplyText("ExpeditionPanel/MarketSquareButton/Market Square Button", "buddy_home_expedition_market_square");
        textProc.ApplyText("ExpeditionPanel/SunkenFreightButton/SunkenFreightButton", "buddy_home_expedition_sunken_freight");
        //Confirm panel
        textProc.ApplyText("ExpeditionPanel/Confirmation/GoButton/SortText (1)"                 , "buddy_home_expedition_confirm_go_button");
        textProc.ApplyText("ExpeditionPanel/Confirmation/ConfirmationPanel/PanelTitle"          , "buddy_home_expedition_confirm_title");
        textProc.ApplyText("ExpeditionPanel/Confirmation/ConfirmationPanel/TimeDuration/Title"  , "buddy_home_expedition_confirm_time_title");
        textProc.ApplyText("ExpeditionPanel/Confirmation/ConfirmationPanel/TimeDuration/Hrs"    , "buddy_home_expedition_confirm_time_hours");
        textProc.ApplyText("ExpeditionPanel/Confirmation/ConfirmationPanel/FindingsText"        , "buddy_home_expedition_confirm_reward");
        //Return panel
        textProc.ApplyText("ExpeditionPanel/ReturnExpedition/PanelTitle"                , "buddy_home_expedition_return_title");
        textProc.ApplyText("ExpeditionPanel/ReturnExpedition/ReturnButton/SortText (3)" , "buddy_home_expedition_return_returning");
        textProc.ApplyText("ExpeditionPanel/ReturnExpedition/ReturnButton/SortText (4)" , "buddy_home_expedition_return_minutes");
        //Return confirm panel
        textProc.ApplyText("ReturnPanelExpeditionPanel (1)/TitleText"           , "buddy_home_expedition_return_confirmation_text");
        textProc.ApplyText("ReturnPanelExpeditionPanel (1)/YesButton/TitleText" , "buddy_home_expedition_return_confirmation_yes");
        textProc.ApplyText("ReturnPanelExpeditionPanel (1)/NoButton/TitleText"  , "buddy_home_expedition_return_confirmation_no");
        //Returned Panel
        textProc.ApplyText("BuddyReturned/Expedition/TitleText (1)"     , "buddy_home_expedition_returned_title");
        textProc.ApplyText("BuddyReturned/Expedition/TitleText (2)"     , "buddy_home_expedition_returned_title_second_line");
        textProc.ApplyText("BuddyReturned/FindingsPanel/FindingsText"   , "buddy_home_expedition_returned_earned");
        
        //Video Panel
        GameObject videoPanel = gameObject.FindChild("VideoNodes");
        GameObject chapterButtons = videoPanel.FindChild("ChapterButtons");
        GameObject levelText = null;
        MultiLanguage.getInstance().apply(videoPanel.FindChild("Game_Title").FindChild("Text"), "buddy_home_video_title");
        for (int i = 1; i <= 7;  ++i) {
            levelText = chapterButtons.FindChild(i.ToString()).FindChild("Level_Text");
            levelText.GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_video_chapter_" + i);
        }
        //Worksheet Panel
        MultiLanguage.getInstance().apply(gameObject.FindChild("Worksheet").FindChild("Game_Title").FindChild("Text"), "buddy_home_worksheet_title");
        for (int i = 1; i <= 6; ++i) {
            MultiLanguage.getInstance().apply(gameObject.FindChild("Worksheet").FindChild("ChapterButtons").FindChild(i.ToString()).FindChild("1-Prologue").FindChild("Level_Text"), "buddy_home_worksheet_chapter");
            gameObject.FindChild("Worksheet").FindChild("ChapterButtons").FindChild(i.ToString()).FindChild("1-Prologue").FindChild("Level_Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_worksheet_chapter_" + i);
        }
        //Doodle Panel
        MultiLanguage.getInstance().apply(gameObject.FindChild("VideoNodesDoodle").FindChild("Game_Title").FindChild("Text"), "buddy_home_doodle_title");
        for (int i = 1; i <= 23; ++i) {
            MultiLanguage.getInstance().apply(gameObject.FindChild("VideoNodesDoodle").FindChild("Episode_" + i).FindChild("Level_Text"), "buddy_home_doodle_episode");
            gameObject.FindChild("VideoNodesDoodle").FindChild("Episode_" + i).FindChild("Level_Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_doodle_episode") + " " + i;
        }
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("ReturnExpedition").FindChild("ReturnButton").GetComponent<Image>(), "gui_return_button");

        
        if(GameState.playerBuddy.hasClaimedExpedition == true) {
            returnUGem.GetComponent<Image>().sprite = uGemSprites[0];
        }else{
            returnUGem.GetComponent<Image>().sprite = uGemSprites[GameState.playerBuddy.location - 1]; //to set uGem to the correct image
        }
    }

    public void SetVideoState(int _state)
    {
        //0 = Video
        //1 = Worksheet
        //2 = Doodle
        currentVideoState = _state;
    }

    public void OpenCurrentStatePanel()
    {
        switch (currentVideoState)
        {
            //0
            case 0:
                gameObject.FindChild("VideoNodes").SetActive(true);
                break;
            //1
            case 1:
                gameObject.FindChild("Worksheet").SetActive(true);
                break;
            //2
            case 2:
                gameObject.FindChild("VideoNodesDoodle").SetActive(true);
                break;
        }
    }

    bool hasChecked = false;
    public bool DEBUG = false;
    public bool wasCancelled;
    public static string GetUniqueID()
    {
        string uniqueID = Guid.NewGuid().ToString();             //random number
        return uniqueID;
    }

    bool sendingOnExpedition = false;

    void sendOnExpedition()
    {
        JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
        {
            //                    DateTimeFormat = "yyyy-MM-ddThh:mm:ssZ";
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Local
        };
		string date = Newtonsoft.Json.JsonConvert.SerializeObject(DateTime.Now.AddHours(GameState.playerBuddy.time_required), microsoftDateFormatSettings);

        hasSentOnExpedition(date);
        ExpeditionSent = true;
    }
    void completeClaim(string ex)
    {
        Debug.Log("Finished claiming expedition reward");

        if (!wasCancelled) {
            GameState.PostActivity("nullvalue", 3, GameState.playerBuddy.time_required, GameState.playerBuddy.location, Guid.NewGuid().ToString(), 0);
        }
    }

	public Text coinEarned;
	public Text jewelEarned;
	public Text uGemEarned;

	public Image uGem;
    public Image returnUGem;
	public Sprite[] uGemSprites;

	public Text potentialCoinEarn;
	public Text potentialJewelEarn;
	public Text potentialUGemEarn;

	public void CalculatePotentialEarning()
	{
		if (GameState.playerBuddy.time_required == 1)
		{	
			potentialCoinEarn.text = ("200");
			potentialJewelEarn.text = ("10 - 15");
			potentialUGemEarn.text = ("10 - 15");
		}
		if (GameState.playerBuddy.time_required == 3)
		{
			potentialCoinEarn.text = ("400");
			potentialJewelEarn.text = ("50 - 60");
			potentialUGemEarn.text = ("50 - 60");
		}
		if (GameState.playerBuddy.time_required == 5)
		{
			potentialCoinEarn.text = ("800");
			potentialJewelEarn.text = ("100 - 150");
			potentialUGemEarn.text = ("100 - 150");
		}
	}

    public void selectReward(bool wasCancelled)
    {
        Debug.Log("Selecting Reward");
        if (!wasCancelled) { 
            float multiplierCoin = 0;
            float multiplierJewel = 0;
            float multiplierR1 = 0;
            float multiplierR2 = 0;
            float multiplierR3 = 0;
            float multiplierR4 = 0;
            switch (GameState.playerBuddy.location)
            {
                case 1://Around Sally
                    multiplierCoin = 1;
                    multiplierJewel = 1;
                    break;
                case 2://Coral Reef
				    multiplierCoin = 1;
				    multiplierJewel = 1;
				    break;
                case 3://Market Square
				    multiplierCoin = 1;
				    multiplierJewel = 1;
				    break;
                case 4://Sunken Freight
				    multiplierCoin = 1;
				    multiplierJewel = 1;
				    break;  
            }

		    int amountCoinToGive = 0;
		    int amountRegularGemToGive = 0;
		    int amountUniqueGemToGive = 0;

		    if (GameState.playerBuddy.time_required == 1)
		    {
			    amountCoinToGive = 200;
			    amountRegularGemToGive = FDL.Library.Numeric.RandomNumber.Between(10,15);
			    amountUniqueGemToGive = FDL.Library.Numeric.RandomNumber.Between(10,15);
		    }
		    else if (GameState.playerBuddy.time_required == 3)
		    {
			    amountCoinToGive = 400;
			    amountRegularGemToGive = FDL.Library.Numeric.RandomNumber.Between(50, 60);
			    amountUniqueGemToGive = FDL.Library.Numeric.RandomNumber.Between(50, 60);
		    }
		    else
		    {
			    amountCoinToGive = 800;
			    amountRegularGemToGive = FDL.Library.Numeric.RandomNumber.Between(100, 150);
			    amountUniqueGemToGive = FDL.Library.Numeric.RandomNumber.Between(100, 150);
		    }
            
		    int amntCoin = Mathf.Max(amountCoinToGive,0);
            int amntRGem = Mathf.Max(amountRegularGemToGive, 0);
		    int amntUGem = Mathf.Max(amountUniqueGemToGive, 0);
            Inventory inventory = GameState.me.inventory;

            Debug.Log("Before I have: " + inventory.Coins + " coins and " + inventory.Jewels + " jewels");
		    Debug.Log("Giving: " + amntCoin + " coins and " + amntRGem + " jewels");
            if(GameState.playerBuddy.hasBeenAssisted == 1)
            {
                GameState.playerBuddy.hasBeenAssisted = 0;
                amntCoin += (int)((float)amntCoin * 1.2f);
                amntRGem += (int)((float)amntRGem * 1.2f);
                amntUGem += (int)((float)amntUGem * 1.2f);
            }
		    if (GameState.playerBuddy.location == 1)//Around Sally
		    {
			    Debug.LogError("Sally");
			    inventory.Coins += amntCoin;
			    inventory.Jewels += amntRGem;
			    inventory.Wood += amntUGem;
		    }
		    if (GameState.playerBuddy.location == 2)//Coral Reef
		    {
			    Debug.LogError("Coral");
			    inventory.Coins += amntCoin;
			    inventory.Jewels += amntRGem;
			    inventory.Ceramic += amntUGem;
		    }
		    if (GameState.playerBuddy.location == 3)//MarketSquare
		    {
			    Debug.LogError("MarketSquare");
			    inventory.Coins += amntCoin;
			    inventory.Jewels += amntRGem;
			    inventory.Steel += amntUGem;
		    }
		    if (GameState.playerBuddy.location == 4)//Sunken Freight
		    {
			    Debug.LogError("SunkenFreight");
			    inventory.Coins += amntCoin;
			    inventory.Jewels += amntRGem;
			    inventory.StoneTablet += amntUGem;
		    }

		    coinEarned.text = amntCoin.ToString();
		    jewelEarned.text = amntRGem.ToString();
		    uGemEarned.text = amntUGem.ToString();
		    uGem.sprite = uGemSprites[GameState.playerBuddy.location - 1];
        

            if (inventory.Coins < 0) inventory.Coins = 0;
            if (inventory.Jewels < 0) inventory.Jewels = 0;
            GameState.PostActivity("nullvalue", 4, GameState.playerBuddy.time_required, GameState.playerBuddy.location, GameState.playerBuddy.BuddyExpeditionID, 0);
            Debug.Log("Resulting in : " + inventory.Coins + " coins and " + inventory.Jewels + " jewels");
			GameState.playerBuddy.expeditionCompleteTime = 0;
            //GameState.playerBuddy.hasClaimedExpedition = true;
            controller.UpdateCurrencyDisplay();
            GameState.me.Upload();
        }else
        {
            GameState.PostActivity("nullvalue", 5, GameState.playerBuddy.time_required, GameState.playerBuddy.location, GameState.playerBuddy.BuddyExpeditionID, 0);
			GameState.playerBuddy.expeditionCompleteTime = 0;
			//GameState.playerBuddy.hasClaimedExpedition = true;
            controller.UpdateCurrencyDisplay();
            GameState.me.Upload();
            if(ExpeditionSent == false)
            {
                returnUGem.GetComponent<Image>().sprite = uGemSprites[0];
            }
        }
    }
    void hasSentOnExpedition(string date)
    {
        GameState.playerBuddy.BuddyExpeditionID = GetUniqueID();
        GameState.PostActivity("nullvalue", 3, GameState.playerBuddy.time_required, GameState.playerBuddy.location, GameState.playerBuddy.BuddyExpeditionID, 0);

		if(GameState.playerBuddy.time_required == 0)
		{
			GameState.playerBuddy.time_required = 1;
		}
		GameState.playerBuddy.expeditionCompleteTime = TimeHelper.ServerEpochTime + (GameState.playerBuddy.time_required * TimeHelper.HOUR);

		/* // Deprecated by HOWARD
        GameState.playerBuddy.expedition_date = date;
        beforeTime = JsonConvert.DeserializeObject<DateTime>(GameState.playerBuddy.expedition_date,
                    new IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-ddThh:mm:ssZ" });
        beforeTime = DateTime.Now.AddHours(GameState.playerBuddy.time_required);
        */
        //GameState.playerBuddy.hasClaimedExpedition = false;

		sendingOnExpedition = false;
        hasChecked = false;
        isLocal = true;
        GameState.me.Upload();
    }

    public bool ManuallyTriggerExpedition;

    // Update is called once per frame
    public void CycleLeft()
    {
        switch (GameState.playerBuddy.time_required)
        {
			case 1:
				GameState.playerBuddy.time_required = 5;
				break;
			case 3:
				GameState.playerBuddy.time_required = 1;
				break;
			case 5:
				GameState.playerBuddy.time_required = 3;
				break;
		}
    }
    public void CycleRight()
    {
        switch (GameState.playerBuddy.time_required)
        {
			case 1:
				GameState.playerBuddy.time_required = 3;
				break;
			case 3:
				GameState.playerBuddy.time_required = 5;
				break;
			case 5:
				GameState.playerBuddy.time_required = 1;
				break;
		}
    }

	public Button[] LocationButtons;

	void DisableLocationButton()
	{
		for (int i = 0; i < LocationButtons.Length; i++)
		{
			LocationButtons[i].gameObject.SetActive(false);
		}
	}

	void EnableLocationButton()
	{
		for (int i = 0; i < LocationButtons.Length; i++)
		{
			LocationButtons[i].gameObject.SetActive(true);
		}
	}


	public void SelectedExploreMapTime(int timeHours)
	{
		//GameState.playerBuddy.time_required = timeHours;
	}

	public void SelectedExploreLocation(int location)
	{
		GameState.playerBuddy.location = location;
	}

	public GameObject tableTop;
    void Update()
    {   
		if (!GameState.playerBuddy.hasClaimedExpedition)
		{
			tableTop.GetComponent<Button>().interactable = false;
			DisableLocationButton();
            
        }
		else
		{
			EnableLocationButton();
			tableTop.GetComponent<Button>().interactable = true;
		}
        hoursText.text = "" + GameState.playerBuddy.time_required;
        if (ManuallyTriggerExpedition)
        {
            ManuallyTriggerExpedition = false;
            BuddyReturnedPanel.SetActive(true);
            selectReward(false);
        }
        if (GameState.playerBuddy != null)
        {
            //available = GameState.playerBuddy.hasClaimedExpedition;
            wanderingAvatar.SetActive(available);
            
			//TimeSpan left = beforeTime - DateTime.Now;
			TimeSpan left = TimeSpan.FromSeconds(GameState.playerBuddy.expeditionCompleteTime - TimeHelper.ServerEpochTime);
			if (!GameState.playerBuddy.hasClaimedExpedition && (left.Hours + left.Minutes + left.Seconds >= 0))
            {
                timeText.text = left.Hours + ":" + left.Minutes + ":" + left.Seconds;
                expeditionButton.gameObject.SetActive(false);
                returnButton.gameObject.SetActive(true);
            }
            else if (!GameState.playerBuddy.hasClaimedExpedition)
            {
                timeText.text = MultiLanguage.getInstance().getString("buddy_home_expedition_claiming_reward");
                if (!hasChecked)
                {
                    hasChecked = true;
                    BuddyReturnedPanel.SetActive(true);
                    selectReward(false);
                }
                expeditionButton.gameObject.SetActive(false);
                returnButton.gameObject.SetActive(true);
            }
            else
            {
                timeText.text = MultiLanguage.getInstance().getString("buddy_home_expedition_ready_to_send_out");
                expeditionButton.gameObject.SetActive(true);
                returnButton.gameObject.SetActive(false);
            }
        }
    }

        TimeSpan timespanToRoundedString(TimeSpan t1,int precision)
    {
        const int TIMESPAN_SIZE = 7; // it always has seven digits
                                     // convert the digitsToShow into a rounding/truncating mask
        int factor = (int)Math.Pow(10, (TIMESPAN_SIZE - precision));

    //    Console.WriteLine("Input: " + t1);
  //      TimeSpan truncatedTimeSpan = new TimeSpan(t1.Ticks - (t1.Ticks % factor));
//        Console.WriteLine("Truncated: " + truncatedTimeSpan);
        TimeSpan roundedTimeSpan =
            new TimeSpan(((long)Math.Round((1.0 * t1.Ticks / factor)) * factor));

        return roundedTimeSpan;
       // Console.WriteLine("Rounded: " + roundedTimeSpan);
    }

    public void SendOnExhibition()
    {
        sendOnExpedition();
    }

	public void moreJewelsPlease() // temporary function to add resources
	{
		GameState.me.inventory.Jewels += 5000;
		GameState.me.inventory.Ceramic += 5000;
		GameState.me.inventory.Steel += 5000;
		GameState.me.inventory.StoneTablet += 5000;
		GameState.me.inventory.Coins += 5000;
        GameState.me.Upload();
	}

	public void unlockAll()
	{
		GameState.pearlyProg.unlock();
		GameState.tangramProg.unlock();
		GameState.tsumProg.unlock();
		GameState.infiniteProg.unlock();
	}

}
