using AutumnInteractive.SimpleJSON;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UniRx;
using UnityEngine;
using System.Text;
using JULESTech;

public class Player
{
    public string ChildCode;
    public Inventory inventory;

	public string id;
	public string name;
    public string FirstName;
    public string LastName;
	public string username;
	public string email;
	public int coins;
	public PlayerAvatar avatar;
	public Dictionary<int, PlayerAchievement> achievements;

	public string inventoryString
	{
		get
		{
			return mInventoryString;
		}
		set
		{
			if (mInventoryString != value)
				UploadSaveDataPart("player_inventory", value);
			
			mInventoryString = value;
		}
	}
		
	public string furnitureString
	{
		get
		{
			return mFurnitureString;
		}
		set
		{
			if (mFurnitureString != value)
				UploadSaveDataPart("player_furniture_levels", value);
			
			mFurnitureString = value;
		}
	}

	public string pearlyProg
	{
		get
		{
			return mPearlyProg;
		}
		set
		{
			if (mPearlyProg != value)
				UploadSaveDataPart("pearly_progression", value);
			
			mPearlyProg = value;
		}
	}

	public string infiniteProg
	{
		get
		{
			return mInfiniteProg;
		}
		set
		{
			if (mInfiniteProg != value)
				UploadSaveDataPart("crabby_progression", value);
			
			mInfiniteProg = value;
		}
	}

	public string tsumProg
	{
		get
		{
			return mTsumProg;
		}
		set
		{
			if (mTsumProg != value)
				UploadSaveDataPart("tumble_progression", value);
			
			mTsumProg = value;
		}
	}

	public string tangramProg
	{
		get
		{
			return mTangramProg;
		}
		set
		{
			if (mTangramProg != value)
				UploadSaveDataPart("manta_progression", value);

			mTangramProg = value;
		}
	}

	public string takoProg
	{
		get
		{
			return mTakoProg;
		}
		set
		{
			if (mTakoProg != value)
				UploadSaveDataPart("tugotako_progression", value);

			mTakoProg = value;
		}
	}

    public string FvTProg
    {
        get
        {
            return mFvTProg;
        }
        set
        {
            if (mFvTProg != value)
                UploadSaveDataPart("FvT_progression", value);

            mFvTProg = value;
        }
    }
    public string buddyString
	{
		get
		{
			return mBuddyString;
		}
		set
		{
			if (mBuddyString != value)
				UploadSaveDataPart("player_avatar", value);
			
			mBuddyString = value;
		}
	}

	public string iqTestOneResults
	{
		get
		{
			return mIqTestOneResults;
		}
		set
		{
			if (mIqTestOneResults != value)
				UploadSaveDataPart("iq_test_one", value);
			
			mIqTestOneResults = value;
		}
	}

	public string iqTestTwoResults
	{
		get
		{
			return mIqTestTwoResults;
		}
		set
		{
			if (mIqTestTwoResults != value)
				UploadSaveDataPart("iq_test_two", value);
			
			mIqTestTwoResults = value;
		}
	}

	public string iqTestThreeResults
	{
		get
		{
			return mIqTestThreeResults;
		}
		set
		{
			if (mIqTestThreeResults != value)
				UploadSaveDataPart("iq_test_three", value);

			mIqTestThreeResults = value;
		}
	}

	public string worksheetOneResults
	{
		get
		{
			return mWorksheetOneResults;
		}
		set
		{
			if (mWorksheetOneResults != value)
				UploadSaveDataPart("worksheet_one_result", value);

			mWorksheetOneResults = value;
		}
	}

	public string worksheetTwoResults
	{
		get
		{
			return mWorksheetTwoResults;
		}
		set
		{
			if (mWorksheetTwoResults != value)
				UploadSaveDataPart("worksheet_two_result", value);
			
			mWorksheetTwoResults = value;
		}
	}

	public string worksheetThreeResults
	{
		get
		{
			return mWorksheetThreeResults;
		}
		set
		{
			if (mWorksheetThreeResults != value)
				UploadSaveDataPart("worksheet_three_result", value);

			mWorksheetThreeResults = value;
		}
	}

	public string worksheetFourResults
	{
		get
		{
			return mWorksheetFourResults;
		}
		set
		{
			if (mWorksheetFourResults != value)
				UploadSaveDataPart("worksheet_four_result", value);

			mWorksheetFourResults = value;
		}
	}

    private string mInventoryString = "{}";
    private string mFurnitureString = "{}";
	private string mPearlyProg = "{}";
	private string mInfiniteProg = "{}";
	private string mTsumProg = "{}";
	private string mTangramProg = "{}";
	private string mTakoProg = "{}";
    private string mFvTProg = "{}";
    private string mBuddyString = "{}";
	private string mIqTestOneResults = "{}";
	private string mIqTestTwoResults = "{}";
	private string mIqTestThreeResults = "{}";
	private string mWorksheetOneResults = "{}";
	private string mWorksheetTwoResults = "{}";
	private string mWorksheetThreeResults = "{}";
	private string mWorksheetFourResults = "{}";

    public static bool hasseenone = false;
    public static bool hasseenTwo = false;
    public static bool hasseenthree = false;
    public static bool hasseenfour = false;
    public static bool hasseenfive = false;
    public static bool hasseensix = false;
    public static bool hasseenseven = false;

    public static bool hasusedhammer = false;
    public Player()
	{
		achievements = new Dictionary<int, PlayerAchievement>();
        avatar = new PlayerAvatar();
	}
    bool isOffline = true;
	public void init(JSONNode json)
	{
		/* HOWARD DEPRECATED
        Debug.Log(json.ToString() + "  WAS RECEIVED");
        if (json["Id"] != null)
		{
			id = json["Id"];
            //Debug.Log("NEW ID0: " + id);
            id = id.Replace('"', ' ').Trim().Substring(0, 36);
            //Debug.Log("NEW ID: " + id);
		}

		if (json["Name"] != null)
		{
			name = json["Name"];
		}

		if (json["Username"] != null)
		{
			username = json["Username"];
		}
		if (json["Email"] != null)
		{
			email = json["Email"].Value;
		}
        if (json["FirstName"] != null) {
            Debug.Log("BLAHBLAH: " + json["FirstName"].Value);
            FirstName = json["FirstName"].Value;
        }
        if (json["LastName"] != null)
        {
            Debug.Log("BLAHBLAH: " + json["LastName"].Value);
            LastName = json["LastName"].Value;
        }
        if(json["Childcode"] != null)
        {
            Debug.Log("Got Child Code: " + json["Childcode"]);
            ChildCode = json["Childcode"];
        }
        */

		id = DataStore.Instance.cloudData.GetString("account_id");

        CheckIsTeacherAccount();
            
        username = DataStore.Instance.cloudData.GetString("user_name");
		if (json["firstname"] != null) 
		{
			FirstName = json["firstname"].Value;
		}
		if (json["lastname"] != null)
		{
			LastName = json["lastname"].Value;
		}

		ChildCode = "Invalid";
    }

    public void CheckIsTeacherAccount()
    {
        WWWForm loginForm = new WWWForm();
        loginForm.AddField("account_id", id);

        JULESTech.JulesNet.Instance.SendPOSTRequest("SOFGetTeacherInfo.php", loginForm,
         (byte[] _msg) => {
             // Received a response
             StringHelper.DebugPrint(_msg);
             string[] data = Encoding.UTF8.GetString(_msg).Split('\t');

             if (data[0] == "OK")
             {
                 // Success
                 UDPNetwork.getInstance().mGroupName = "teacher";
             }
             else
             {
                 //If doesnt exist, it returns error
                 // Failed
                 UDPNetwork.getInstance().mGroupName = "student";
             }
         },
            (string _error) => {
                // Failed to send
                //Just to be safe, we will accept everything
                UDPNetwork.getInstance().mGroupName = "";
            }
        );
    }

    public void GetData(JSONNode _json)
    {
        //JSONNode json = JSONNode.Parse(x);
		inventoryString = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".player_inventory", _json["player_inventory"]);
		furnitureString = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".player_furniture_levels", _json["player_furniture_levels"]);
		buddyString = PlayerPrefs.GetString(GameState.me.id + "_Avatar", _json["player_avatar"]);
		iqTestOneResults = PlayerPrefs.GetString(GameState.me.id + "_IQTestOne", _json["iq_test_one"]);
		iqTestTwoResults = PlayerPrefs.GetString(GameState.me.id + "_IQTestTwo", _json["iq_test_two"]);
		iqTestThreeResults = PlayerPrefs.GetString(GameState.me.id + "_IQTestThree", _json["iq_test_three"]);
		worksheetOneResults = PlayerPrefs.GetString(GameState.me.id + "_WorksheetOne", _json["worksheet_one_result"]);
		worksheetTwoResults = PlayerPrefs.GetString(GameState.me.id + "_WorksheetTwo", _json["worksheet_two_result"]);
		worksheetThreeResults = PlayerPrefs.GetString(GameState.me.id + "_WorksheetThree", _json["worksheet_three_result"]);
		worksheetFourResults = PlayerPrefs.GetString(GameState.me.id + "_WorksheetFour", _json["worksheet_four_result"]);
        GameState.playerBuddy = new Buddy(buddyString);
        GameState.me.inventory = new Inventory(inventoryString, furnitureString);

    }
	public void GetGameSaveData(JSONNode _json)
    {
        try
        {
            //JSONNode json = JSONNode.Parse(x);
            //StartMenuController.textToShow = "Downloading Player Data: ";
            //StartMenuController.progress = 0;
            //StartMenuController.fakeProgressTime = 0.0f;

			tangramProg = _json["manta_progression"];
            //StartMenuController.progress = 20;
			tsumProg = _json["tumble_progression"];
            //StartMenuController.progress = 40;
			infiniteProg = _json["crabby_progression"];
            //StartMenuController.progress = 60;
			pearlyProg = _json["pearly_progression"];
            //StartMenuController.progress = 80;
			//takoProg = _json["tugotako_progression"];
            FvTProg = _json["FvT_progression"];
            //StartMenuController.progress = 100;
            GameState.LoadGamesFromPlayerStats();
        }
        catch (System.Exception e)
        {

        }        
    }
    public void GetGameSaveDataOffline()
    {
        try
        {
            StartMenuController.progress = 0;
            StartMenuController.fakeProgressTime = 0.0f;
            tangramProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tangram_player_progression");
            StartMenuController.progress = 20;
            tsumProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tsum_player_progression");
            StartMenuController.progress = 40;
            infiniteProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".infinite_player_progression");
            StartMenuController.progress = 60;
            pearlyProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".pearly_player_progression");
            StartMenuController.progress = 80;
            //takoProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tako_trouble_progression");
            FvTProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".FvT_player_progression");
            GameState.LoadGamesFromPlayerStats();
            StartMenuController.progress = 100;
        }
        catch (System.Exception e)
        {

        }
    }
    public void FinishDownloading()
    {
        if (isOffline)
        {
            pearlyProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".pearly_player_progression", "{}");
            infiniteProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".infinite_player_progression", "{}");
            tsumProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tsum_player_progression", "{}");
            tangramProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tangram_player_progression", "{}");
            //takoProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tako_trouble_progression");
            FvTProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".FvT_player_progression");
            GameState.LoadGamesFromPlayerStats();
        }
        
    }
    public void Upload()
    {
		StringHelper.DebugPrint("PLAYER SAVE UPLOAD CALLED");
        GameState.SaveGamesToPlayerStats();
        try { 
        if (GameState.playerBuddy != null)
        {
            buddyString = GameState.playerBuddy.SaveAsString();
        }
        else
        {
            buddyString = "{}";
        }
        }catch(System.Exception e)
        {
            Debug.Log(e);
        }
        List<string> data = inventory.SaveAsString();
        inventoryString = data[0];
        furnitureString = data[1];
        //            takoProg = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tako_trouble_progression");
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".pearly_player_progression", pearlyProg);
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".infinite_player_progression", infiniteProg);
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".tsum_player_progression", tsumProg);
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".tangram_player_progression", tangramProg);
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".player_inventory", inventoryString);
        //PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".tako_trouble_progression", takoProg);
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".FvT_player_progression", FvTProg);
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".player_furniture_levels", furnitureString);
        PlayerPrefs.SetString(GameState.me.id + "_Avatar", buddyString);
        PlayerPrefs.SetString(GameState.me.id + "_IQTestOne", iqTestOneResults);
        PlayerPrefs.SetString(GameState.me.id + "_IQTestTwo", iqTestTwoResults);
        PlayerPrefs.SetString(GameState.me.id + "_IQTestThree", iqTestThreeResults);
        PlayerPrefs.SetString(GameState.me.id + "_WorksheetOne", worksheetOneResults);
        PlayerPrefs.SetString(GameState.me.id + "_WorksheetTwo", worksheetTwoResults);
        PlayerPrefs.SetString(GameState.me.id + "_WorksheetThree", worksheetThreeResults);
        PlayerPrefs.SetString(GameState.me.id + "_WorksheetFour", worksheetFourResults);
        PlayerPrefs.SetInt(GameState.me.id + "_SeenOne", hasseenone ? 1 : 0);
        PlayerPrefs.SetInt(GameState.me.id + "_SeenTwo", hasseenTwo? 1: 0);
        PlayerPrefs.SetInt(GameState.me.id + "_SeenThree",hasseenTwo ? 1 : 0);
        PlayerPrefs.SetInt(GameState.me.id + "_SeenFour",hasseenTwo ? 1 : 0);
        PlayerPrefs.SetInt(GameState.me.id + "_SeenFive",hasseenTwo ? 1 : 0);
        PlayerPrefs.SetInt(GameState.me.id + "_SeenSix",hasseenTwo ? 1 : 0);
        PlayerPrefs.SetInt(GameState.me.id + "_SeenSeven", hasseenseven ? 1 : 0);
        PlayerPrefs.SetInt(GameState.me.id + "_Usedhammer", hasusedhammer ? 1 : 0);
       
		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//            
//        form.AddField("user_id", GameState.me.id);
//        form.AddField("pearly_player_progression", pearlyProg);
//        form.AddField("infinite_player_progression", infiniteProg);
//        form.AddField("tsum_player_progression", tsumProg);
//        form.AddField("tangram_player_progression", tangramProg);
//        form.AddField("tug_o_tako_progression", takoProg);
//        form.AddField("player_inventory", inventoryString);
//        form.AddField("player_furniture_levels", furnitureString);
//        form.AddField("avatar", buddyString);
//        string serial = "";
//        string mac = "";
//        #if UNITY_ANDROID && !UNITY_EDITOR
//            AndroidJavaObject jo = new AndroidJavaObject("android.os.Build");
//            serial = jo.GetStatic<string>("SERIAL");
//            mac = ReturnMacAddress();
//        #else
//            serial = SystemInfo.deviceUniqueIdentifier;
//            mac = GetMacAddress();
//        #endif
//        Debug.Log("Serial: " + serial);
//        Debug.Log("Mac: " + mac);
//        string avatarIcon = (GameState.playerBuddy.avatarIndex+1) + "_" + (GameState.playerBuddy.bodyIndex+1) +".png";
//        
//        form.AddField("avatar_icon", avatarIcon);
//        if (iqTestOneResults == string.Empty) iqTestOneResults = "{null}";
//        if (iqTestTwoResults == string.Empty) iqTestTwoResults = "{null}";
//        if (iqTestThreeResults == string.Empty) iqTestThreeResults = "{null}";
//        form.AddField("iq_one", iqTestOneResults);
//        form.AddField("iq_two", iqTestTwoResults);
//        form.AddField("iq_three", iqTestThreeResults);
//        form.AddField("worksheet_one", worksheetOneResults);
//        form.AddField("worksheet_two", worksheetTwoResults);
//
//        form.AddField("worksheet_three", worksheetThreeResults);
//        form.AddField("worksheet_four", worksheetFourResults);
//        AppServer.CreatePost("me/gamedata/update", form).Subscribe(x => Debug.Log(x), // onSuccess
//            ex => Debug.Log(ex.ToString())
//            );
//
//        WWWForm form2 = new WWWForm();
//        form2.AddField("user_id", GameState.me.id);
//        form2.AddField("avatar_icon", avatarIcon);
//        AppServer.CreatePost("me/avataricon/update", form2).Subscribe(x => Debug.Log(x), ex => Debug.Log(ex.ToString()));
//
//        try { 
//        WWWForm form3 = new WWWForm();
//        form3.AddField("mac_address",mac);
//        form3.AddField("user_id", GameState.me.id);
//        form3.AddField("serial_number",serial);
//        AppServer.CreatePost("me/device/update_info", form3).Subscribe(x => Debug.Log(x), ex => Debug.Log(ex.ToString()));
//        }catch(System.Exception e)
//        {
//            Debug.Log(e);
//        }
    }

	public void UploadSaveDataPart(string _dataType, string _data)
	{
		if (id == null || id == "")
		{
			StringHelper.DebugPrint("Upload Save Data Part : Invalid ID");
			return;
		}

		if (_data == null || _data == "" || _data == "{}")
		{
			StringHelper.DebugPrint("Upload Save Data Part : No Data");
			return;
		}
		
		WWWForm versionForm = new WWWForm();
		versionForm.AddField("account_id", id);
		versionForm.AddField("data_type", _dataType);
		versionForm.AddField("user_data", _data);
		versionForm.AddField("data_hash",  StringHelper.GetMD5Hash(_data));
		JulesNet.Instance.SendPOSTRequest("SOFUploadUserData.php", versionForm, 
			(byte[] _msg) => {
				// Received a response
				StringHelper.DebugPrint(_msg);
				string[] dataTSV = Encoding.UTF8.GetString (_msg).Split ('\t');
				if (dataTSV [0] == "OK") 
				{
					// SUCCESS
					StringHelper.DebugPrint("Updated Save");
				}
				else 
				{
					// FAILED
					StringHelper.DebugPrint("Failed Save");
				}
			},
			// Connection Failed
			null);
	}


    #if UNITY_ANDROID && !UNITY_EDITOR
    AndroidJavaObject mWiFiManager;
    string ReturnMacAddress()
    {
        string macAddr = "";
        if (mWiFiManager == null)
        {
            using (AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            {
                mWiFiManager = activity.Call<AndroidJavaObject>("getSystemService", "wifi");
            }
        }
        macAddr = mWiFiManager.Call<AndroidJavaObject>("getConnectionInfo").Call<string>("getMacAddress");
       return macAddr; 
    }

    #endif
    public string GetMacAddress()
    {
        string macAdress = "";
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        foreach(NetworkInterface adapter in nics)
        {
            PhysicalAddress address = adapter.GetPhysicalAddress();
            if (address.ToString() != "")
            {
                macAdress = address.ToString();
                return macAdress;
            }
        }
        return "{unknown}";
    }
    public void ResetEmpty()
    {
        GameState.SaveGamesToPlayerStats();
        buddyString = GameState.playerBuddy.SaveAsString();
        List<string> data = inventory.SaveAsString();
        inventoryString = data[0];
        furnitureString = data[1];
        if (isOffline)
        {
            Debug.Log("Deleting all, the keys");
			PlayerPrefs.DeleteKey(GameState.me.id + "." + GameState.me.username + ".pearly_player_progression");//, pearlyProg);
			PlayerPrefs.DeleteKey(GameState.me.id + "." + GameState.me.username + ".infinite_player_progression");//, infiniteProg);
			PlayerPrefs.DeleteKey(GameState.me.id + "." + GameState.me.username + ".tsum_player_progression");//, tsumProg);
			PlayerPrefs.DeleteKey(GameState.me.id + "." + GameState.me.username + ".tangram_player_progression");//, tangramProg);
			PlayerPrefs.DeleteKey(GameState.me.id + "." + GameState.me.username + ".player_inventory");//, inventoryString);
			PlayerPrefs.DeleteKey(GameState.me.id + "." + GameState.me.username + ".player_furniture_levels");//, furnitureString);
			PlayerPrefs.DeleteKey(GameState.me.id + "_Avatar");//, buddyString);
            PlayerPrefs.DeleteAll();
            GameState.me = null;
            GameState.playerBuddy = null;
            pearlyProg = "{}";// = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".pearly_player_progression", "{}");
            infiniteProg = "{}";// = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".infinite_player_progression", "{}");
            tsumProg = "{}";// = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tsum_player_progression", "{}");
            tangramProg = "{}";// = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".tangram_player_progression", "{}");
            inventoryString = "{}";// = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".player_inventory", "{}");
            furnitureString = "{}";// = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".player_furniture_levels", "{}");
            buddyString = "{}";// = PlayerPrefs.GetString(GameState.me.id + "_Avatar", "{}");
        }
        else
        {

        }
    }
}

public class Inventory
{
    public int Coins;
    public int Jewels;
    public int Ceramic;		//StoneTablet
    public int Steel;		//Steel Diamond
    public int Wood;		//Wood
    public int StoneTablet;		//StoneTablet Fabric
    public int Bed_Selected;
    public int Shelf_Selected;
    public int Table_Selected;
    public int Carpet_Selected;
    public int Item1_Selected;
    public int Item2_Selected;
    public int Item3_Selected;
    public int Item4_Selected;
    public int Item5_Selected;
    public int Item6_Selected;
    public FurnitureLevels furnitureLevels;
    static bool isTest = false; 
    public Inventory(string inventoryString, string furnitureLevelString)
    {
        Coins = 2500;
        Jewels = 0;
        Ceramic = 0;
        Steel = 0;
        Wood = 0;
        StoneTablet = 0;
        Load(inventoryString);
        furnitureLevels = new FurnitureLevels(furnitureLevelString);
    }
    public int getSelectedTier(int index)
    {
        switch(index)
        {
            case 0: return Bed_Selected;break;
            case 1: return Shelf_Selected; break;
            case 2: return Table_Selected; break;
            case 3: return Carpet_Selected; break;
            case 4: return Item1_Selected; break;
            case 5: return Item2_Selected; break;
            case 6: return Item3_Selected; break;
            case 7: return Item4_Selected; break;
            case 8: return Item5_Selected; break;
            case 9: return Item6_Selected; break;

        }

        return -1;
    }
    public string GetFurnitureString()
    {
        string s = "";
        s += Bed_Selected + "," + Shelf_Selected + "," + Table_Selected + "," + Carpet_Selected + "," + Item1_Selected + "," + Item2_Selected + "," + Item3_Selected + "," + Item4_Selected + "," + Item5_Selected + "," + Item6_Selected;
        return s;
    }

    public void Save()
    {
        var jsonData = Serialize();
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".player_inventory", jsonData.ToString());
        furnitureLevels.Save();
    }
    public List<string> SaveAsString()
    {
        Debug.Log("Saving Inventory as String");
        List<string> str = new List<string>();
        var jsonData = Serialize();

        str.Add(jsonData.ToString());
        str.Add(furnitureLevels.SaveAsString());
        return str;
    }
    public void Load(string s)
    {
        var jsonDataText = s;
        var jsonData = JSON.Parse<JSONClass>(jsonDataText);

        try
        {
            Coins = jsonData.GetEntry("Coins", 0);
            Jewels = jsonData.GetEntry("Jewels", 0);
            Ceramic = jsonData.GetEntry("Metal", 0);
            Steel = jsonData.GetEntry("Tablets", 0);
            Wood = jsonData.GetEntry("Wood", 0);
            StoneTablet = jsonData.GetEntry("Ceramic", 0);
            Bed_Selected = jsonData.GetEntry("Bed_Selected", 0);
            Shelf_Selected = jsonData.GetEntry("Shelf_Selected", 0);
            Table_Selected = jsonData.GetEntry("Table_Selected", 0);
            Carpet_Selected = jsonData.GetEntry("Carpet_Selected", 0);
            Item1_Selected = jsonData.GetEntry("Item1_Selected", 0);
            Item2_Selected = jsonData.GetEntry("Item2_Selected", 0);
            Item3_Selected = jsonData.GetEntry("Item3_Selected", 0);
            Item4_Selected = jsonData.GetEntry("Item4_Selected", 0);
            Item5_Selected = jsonData.GetEntry("Item5_Selected", 0);
            Item6_Selected = jsonData.GetEntry("Item6_Selected", 0);

            Player.hasseenone = PlayerPrefs.GetInt(GameState.me.id + "_SeenOne", jsonData.GetEntry("hasseenone",0)) == 1;
            Player.hasseenTwo = PlayerPrefs.GetInt(GameState.me.id + "_SeenTwo", jsonData.GetEntry("hasseentwo", 0)) == 1;
            Player.hasseenthree = PlayerPrefs.GetInt(GameState.me.id + "_SeenThree", jsonData.GetEntry("hasseenthree", 0)) == 1;
            Player.hasseenfour = PlayerPrefs.GetInt(GameState.me.id + "_SeenFour", jsonData.GetEntry("hasseenfour", 0)) == 1;
            Player.hasseenfive = PlayerPrefs.GetInt(GameState.me.id + "_SeenFive", jsonData.GetEntry("hasseenfive", 0)) == 1;
            Player.hasseensix = PlayerPrefs.GetInt(GameState.me.id + "_SeenSix", jsonData.GetEntry("hasseensix", 0)) == 1;
            Player.hasseenseven = PlayerPrefs.GetInt(GameState.me.id + "_SeenSeven", jsonData.GetEntry("hasseenseven", 0)) == 1;
            Player.hasusedhammer=PlayerPrefs.GetInt(GameState.me.id+ "_Usedhammer", jsonData.GetEntry("hasusedhammer", 0)) == 1;

        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }

    public JSONClass Serialize()
    {
        var jsonData = new JSONClass();
        jsonData.Add("Coins", Coins.ToString());
        jsonData.Add("Jewels", Jewels.ToString());
        jsonData.Add("Metal", Ceramic.ToString());
        jsonData.Add("Tablets", Steel.ToString());
        jsonData.Add("Wood", Wood.ToString());
        jsonData.Add("Ceramic", StoneTablet.ToString());
        jsonData.Add("Bed_Selected", Bed_Selected.ToString());
        jsonData.Add("Shelf_Selected", Shelf_Selected.ToString());
        jsonData.Add("Table_Selected", Table_Selected.ToString());
        jsonData.Add("Carpet_Selected", Carpet_Selected.ToString());
        jsonData.Add("Item1_Selected", Item1_Selected.ToString());
        jsonData.Add("Item2_Selected", Item2_Selected.ToString());
        jsonData.Add("Item3_Selected", Item3_Selected.ToString());
        jsonData.Add("Item4_Selected", Item4_Selected.ToString());
        jsonData.Add("Item5_Selected", Item5_Selected.ToString());
        jsonData.Add("Item6_Selected", Item6_Selected.ToString());
        jsonData.Add("hasseenone", Player.hasseenone.ToString());
        jsonData.Add("hasseentwo", Player.hasseenTwo.ToString());
        jsonData.Add("hasseenthree", Player.hasseenthree.ToString());
        jsonData.Add("hasseenfour", Player.hasseenfour.ToString());
        jsonData.Add("hasseenfive", Player.hasseenfive.ToString());
        jsonData.Add("hasseensix", Player.hasseensix.ToString());
        jsonData.Add("hasseenseven", Player.hasseenseven.ToString());
        jsonData.Add("hasusedhammer", Player.hasusedhammer.ToString());
        return jsonData;
    }

}
public class FurnitureLevels
{
    List<int> bedLevels;
    List<int> shelfLevels;
    List<int> tableLevels;
    List<int> carpetLevels;
    List<int> itemLevels;
    bool[] unlockedItems = new bool[] {true, false, false, false, false, false, false, false, false, false, false, false, false };
    public FurnitureLevels()
    {
        bedLevels = new List<int>(new int[4]);
        bedLevels[0] = 1;
        shelfLevels = new List<int>(new int[4]);
        shelfLevels[0] = 1;
        tableLevels = new List<int>(new int[4]);
        tableLevels[0] = 1;
        carpetLevels = new List<int>(new int[5]);//0 for locked, 1 for unlocked
        carpetLevels[0] = 1;
        itemLevels = new List<int>(new int[20]);//first item is no item (empty) // 0 for locked, 1 for unlocked
        itemLevels[0] = 1;
    }
    public FurnitureLevels(string s)
    {
        bedLevels = new List<int>(new int[4]);
        bedLevels[0] = 1;
        shelfLevels = new List<int>(new int[4]);
        shelfLevels[0] = 1;
        tableLevels = new List<int>(new int[4]);
        tableLevels[0] = 1;
        carpetLevels = new List<int>(new int[5]);//0 for locked, 1 for unlocked
        carpetLevels[0] = 1;
        itemLevels = new List<int>(new int[20]);//first item is no item (empty) // 0 for locked, 1 for unlocked
        itemLevels[0] = 1;
        Load(s);
    }
    public int getFurnitureLevel(int group, int teir)
    {
        int ret = 0;
        switch (group)
        {
            case 0:
                return bedLevels[teir];
            case 1:
                return shelfLevels[teir];
            case 2:
                return tableLevels[teir];
            case 3:
                return carpetLevels[teir];
            case 4:
                return itemLevels[teir];
        }
        return ret;
    }
    public bool getUnlockedMisc(int item)
    {
        return unlockedItems[item];
    }
    public void unlockMisc(int item)
    {
        unlockedItems[item] = true;
    }
    public void setFurnitureLevel(int group, int teir, int val)
    {
        switch (group)
        {
            case 0:
                bedLevels[teir ] = val;
                break;
            case 1:
                shelfLevels[teir] = val;
                break;
            case 2:
                tableLevels[teir] = val;
                break;
            case 3:
                carpetLevels[teir] = val;
                break;
            case 4:
                itemLevels[teir] = val;
                break;
        }
    }
#region Load/Save functions

    public void Load(string s)
    {
        var jsonDataText = s;
        if (jsonDataText.Length > 0)
        {
            try
            {
                var jsonData = JSON.Parse<JSONClass>(jsonDataText);
                Init(jsonData);
            }
            catch (System.Exception e)
            {
                Debug.Log(e);

            }
        }
    }
    public string SaveAsString()
    {
        var jsonData = Serialize();
        string s = jsonData.ToString();
        return s;
    }

    public void Save()
    {
        var jsonData = Serialize();
        string s = jsonData.ToString();
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".player_furniture_levels", s);
    }

    public void Init(JSONClass jsonData)
    {
        for (int i = 0; i < jsonData.GetEntry<int>("bedCount"); i++)//each(int level in jsonData.GetJson("bedLevels",new JSONArray()))
        {
            bedLevels[i] = jsonData.GetEntry<int>("b" + i,(i==0)?1:0);
        }
        for (int i = 0; i < jsonData.GetEntry<int>("shelfCount"); i++)//each(int level in jsonData.GetJson("bedLevels",new JSONArray()))
        {
            shelfLevels[i] = jsonData.GetEntry<int>("s" + i, (i == 0) ? 1 : 0);
        }
        for (int i = 0; i < jsonData.GetEntry<int>("tableCount"); i++)//each(int level in jsonData.GetJson("bedLevels",new JSONArray()))
        {
            tableLevels[i] = jsonData.GetEntry<int>("t" + i, (i == 0) ? 1 : 0);
        }
        for (int i = 0; i < jsonData.GetEntry<int>("carpetCount"); i++)//each(int level in jsonData.GetJson("bedLevels",new JSONArray()))
        {
            carpetLevels[i] = jsonData.GetEntry<int>("c" + i, (i == 0) ? 1 : 0);
        }
        for (int i = 0; i < jsonData.GetEntry<int>("itemCount"); i++)//each(int level in jsonData.GetJson("bedLevels",new JSONArray()))
        {
            itemLevels[i] = jsonData.GetEntry<int>("i" + i, (i == 0) ? 1 : 0);
        }
        for (int i = 0; i < unlockedItems.Length; i++)
        {
            unlockedItems[i] = jsonData.GetEntry<bool>("itemUnlocked-" + i, false);
        }
    }

    public JSONClass Serialize()
    {
        var jsonData = new JSONClass();
        //beds
        jsonData.Add("bedCount", bedLevels.Count.ToString());
        for (int i = 0; i < bedLevels.Count; i++)
        {
            jsonData.Add("b" + i, bedLevels[i].ToString());
        }
        //shelves
        jsonData.Add("shelfCount", shelfLevels.Count.ToString());
        for (int i = 0; i < shelfLevels.Count; i++)
        {
            jsonData.Add("s" + i, shelfLevels[i].ToString());
        }
        //tables
        jsonData.Add("tableCount", tableLevels.Count.ToString());
        for (int i = 0; i < tableLevels.Count; i++)
        {
            jsonData.Add("t" + i, tableLevels[i].ToString());
        }
        //carpet
        jsonData.Add("carpetCount", carpetLevels.Count.ToString());
        for (int i = 0; i < carpetLevels.Count; i++)
        {
            jsonData.Add("c" + i, carpetLevels[i].ToString());
        }
        //item_levels
        jsonData.Add("itemCount", itemLevels.Count.ToString());
        for (int i = 0; i < itemLevels.Count; i++)
        {
            jsonData.Add("i" + i, itemLevels[i].ToString());
        }
        for (int i = 0; i < unlockedItems.Length; i++)
        {
            jsonData.Add("itemUnlocked-" + i,unlockedItems[i].ToString());
        }
        return jsonData;
    }
#endregion
}
public class Buddy
{
    public PlayerAvatar p;
    public string name;
    //public string expedition_date; // DEPRECATED BY HOWARD
	public int expeditionCompleteTime = 0;
    public int time_required = 0;
    public int location = -1;
    
	//public bool hasClaimedExpedition; // DEPRECATED BY HOWARD
	public bool hasClaimedExpedition
	{
		get
		{
			// Zero for expedition complete time denotes no expedition active
			if (expeditionCompleteTime == 0)
				return true;

			return false;
		}
	}

    public int avatarIndex;
    public int bodyIndex;
    public int eyeIndex;
    public int hasBeenAssisted = 0;
    public string BuddyExpeditionID;
    public Buddy(string n, string e, bool claimed)
    {
        name = n;
        //expedition_date = e;
        //hasClaimedExpedition = claimed;
    }
    public Buddy(string n, int e, bool claimed, int time_required, int location, int avatarIndex, int bodyIndex, int eyeIndex, PlayerAvatar playerAvatar)
    {
        name = n;
        this.expeditionCompleteTime = e;
        //this.hasClaimedExpedition = claimed;
        this.time_required = time_required;
        this.location = location;
        this.avatarIndex = avatarIndex;
        this.bodyIndex = bodyIndex;
        this.eyeIndex = eyeIndex;
        this.p = playerAvatar;
    }
    public Buddy() // default Constructor calls load
    {
        try
        {
            var jsonDataText = PlayerPrefs.GetString(GameState.me.id + "_Avatar", "{}");
            var jsonData = JSON.Parse<JSONClass>(jsonDataText);
            name = jsonData["Name"];

			//expedition_date = jsonData["expedition_date"]; // DEPRECATED BY HOWARD
			jsonData.Remove("expedition_date"); // Forcefully remove entry for all incoming data
			expeditionCompleteTime = jsonData["expeditionCompleteTime"].AsInt;

			time_required = jsonData["time_required_expedition"].AsInt;
            location = jsonData["location_expedition"].AsInt;
            
			//hasClaimedExpedition = jsonData["hasClaimedExpedition"].AsBool;
			jsonData.Remove("hasClaimedExpedition"); // Forcefully remove entry for all incoming data

            avatarIndex = jsonData["avatarIndex"].AsInt;
            bodyIndex = jsonData["bodyIndex"].AsInt;
            eyeIndex = jsonData["eyeIndex"].AsInt;
            BuddyExpeditionID = jsonData.GetEntry<string>("ExpeditionID", " ");
            p = new PlayerAvatar();
            p.avatar = new Avatar();
            p.avatarSkin.id = jsonData["playerAvatarIndex"].AsInt;
            p.avatarSkin.prefab = jsonData["playerAvatarPrefab"];
            p.avatar.id = jsonData["currentAvatarIndex"].AsInt;
            p.avatar.prefab = jsonData["currentAvatarPrefab"];

            UpdateAvatar();
        }
        catch (System.Exception e)
        {
            Debug.Log(e);
        }
    }
    public Buddy(string s) // default Constructor calls load
    {
        //Debug.Log(s);
        try 
		{ 
            var jsonDataText = s;
            var jsonData = JSON.Parse<JSONClass>(jsonDataText);
            name = jsonData["Name"];
            
			//expedition_date = jsonData.GetEntry<string>("expedition_date",""); // DEPRECATED BY HOWARD
			jsonData.Remove("expedition_date"); // Forcefully remove entry for all incoming data
			expeditionCompleteTime = jsonData.GetEntry<int>("expeditionCompleteTime", 0);

            time_required = jsonData.GetEntry<int>("time_required_expedition",0);
            location = jsonData.GetEntry<int>("location_expedition",0);

            //hasClaimedExpedition = jsonData.GetEntry<bool>("hasClaimedExpedition", false);
			jsonData.Remove("hasClaimedExpedition"); // Forcefully remove entry for all incoming data

            avatarIndex = jsonData["avatarIndex"].AsInt;
            bodyIndex = jsonData["bodyIndex"].AsInt;
            eyeIndex = jsonData["eyeIndex"].AsInt;
            p = new PlayerAvatar();
            p.avatar = new Avatar();
            p.avatarSkin.id = jsonData["playerAvatarIndex"].AsInt;
            p.avatarSkin.prefab = jsonData["playerAvatarPrefab"];
            p.avatar.id = jsonData["currentAvatarIndex"].AsInt;
            p.avatar.prefab = jsonData["currentAvatarPrefab"];
            BuddyExpeditionID = jsonData.GetEntry<string>("ExpeditionID", " ");
            hasBeenAssisted = jsonData.GetEntry<int>("hasBeenAssisted", 0);
            UpdateAvatar();
        }
		catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
    public string SaveAsString()
    {
        var jsonData = new JSONClass();
        jsonData.Add("Name", name);
        //jsonData.ToString();
        //jsonData.Add("expedition_date", expedition_date); // DEPRECATED BY HOWARD
		jsonData.Add("expeditionCompleteTime", expeditionCompleteTime.ToString());
        //jsonData.ToString();
        jsonData.Add("time_required_expedition", time_required.ToString());
        //jsonData.ToString();
        jsonData.Add("location_expedition", location.ToString());
        //jsonData.ToString();
		//jsonData.Add("hasClaimedExpedition", hasClaimedExpedition.ToString()); // DEPRECATED BY HOWARD
        //jsonData.ToString();
        jsonData.Add("avatarIndex", avatarIndex.ToString());
        //jsonData.ToString();
        jsonData.Add("bodyIndex", bodyIndex.ToString());
        //jsonData.ToString();
        jsonData.Add("eyeIndex", eyeIndex.ToString());
        //jsonData.ToString();
        
		if (BuddyExpeditionID == null) 
			BuddyExpeditionID = "null";
        
		jsonData.Add("ExpeditionID", BuddyExpeditionID);
        //jsonData.ToString();
        try
        {
            jsonData.Add("playerAvatarIndex", p.avatarSkin.id.ToString());
            //jsonData.ToString();
            jsonData.Add("playerAvatarPrefab", p.avatarSkin.prefab.ToString());
            //jsonData.ToString();
            jsonData.Add("currentAvatarIndex", p.avatar.id.ToString());
            //jsonData.ToString();
            jsonData.Add("currentAvatarPrefab", p.avatar.prefab.ToString());
            //jsonData.ToString();
        }

        catch (System.Exception e)
        {
        }
        jsonData.Add("hasBeenAssisted", hasBeenAssisted.ToString());
        //jsonData.ToString();
        UpdateAvatar();
        return jsonData.ToString();
    }
    void UpdateAvatar()
    {
        GameState.me.avatar = p;
        GameState.me.avatar.avatarId = avatarIndex;
        GameState.me.avatar.skinId = bodyIndex;
        GameState.me.avatar.eyeId = eyeIndex;
    }
}