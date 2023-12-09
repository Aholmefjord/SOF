using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using AutumnInteractive.SimpleJSON;
using System.Threading;
using JULESTech;

public class GameState
{
	// the player of this game
    
     
	public static Player me; 
	public static PlayerPortal mePortalMain;

    public static PearlyProgression pearlyProg; // requires JSON
    public static InfiniteBrosProgression infiniteProg; // requires JSON
    public static TsumPlayerProgression tsumProg; // requires JSON load
    public static TangramPlayerProgression tangramProg; // requires JSON load
    public static TakoPlayerProgression takoProg;
    public static FvTProgression FvTProg;
    
    // the player's buddy
    public static Buddy playerBuddy; // requires JSON

    // the version of the binary build told by server
    public static int version;
	public static string deviceId;
	public static string platformToken;
	public static string julesAccessToken;

	// store the versions of the current config data
	public static Dictionary<string, int> versions = new Dictionary<string, int>();

	// a dependency check if all config is loaded
	public static Dictionary<string, bool> configloaded = new Dictionary<string, bool>();

	// store all the config data
	// key is the item type e.g. Weapon, value is the Hashtable of Id and Weapon object
	public static Hashtable configs = new Hashtable();

	// will be set to some session id after login
	//private static Hashtable session = new Hashtable();
	public static string state = "init";
	public static string nextScene;
	public static bool callPreload = false;
	public static double serverTimeStamp;
	public static DateTime serverTime = new DateTime(0);
	public static bool waitingServerResponse = false;
    
    public int game_unlock_1 = 0;
    public int game_unlock_2 = 0;
    public int game_unlock_3 = 0;
    public int game_unlock_4 = 0;
    public int game_unlock_5 = 0;
    public int game_unlock_6 = 0;

    public static void LoadGamesFromPlayerStats()
    {
        StartMenuController.textToShow = MultiLanguage.getInstance().getString("main_menu_loading_player_data");
        StartMenuController.progress = 0;
        StartMenuController.fakeProgressTime = 0.0f;
        //Debug.Log("Starting Threads");
        //start the 5 threads, one for loading each game
        Thread t1 = new Thread(new ThreadStart(LoadGame1)); //new ThreadStart is optional for the same reason 
        t1.Start();
        Thread t2 = new Thread(new ThreadStart(LoadGame2)); //new ThreadStart is optional for the same reason 
        t2.Start();
        Thread t3 = new Thread(new ThreadStart(LoadGame3)); //new ThreadStart is optional for the same reason 
        t3.Start( );
        Thread t4 = new Thread(new ThreadStart(LoadGame4)); //new ThreadStart is optional for the same reason 
        t4.Start();
        Thread t5 = new Thread(new ThreadStart(LoadGame5)); //new ThreadStart is optional for the same reason 
        t5.Start();

    }

    public static void LoadGame1()
    {
        pearlyProg = new PearlyProgression(); pearlyProg.Load(GameState.me.pearlyProg);
        StartMenuController.progress = StartMenuController.progress + 20;
        //Debug.Log("Done 1");
    }
    public static void LoadGame2()
    {
        infiniteProg = new InfiniteBrosProgression(); infiniteProg.Load(GameState.me.infiniteProg);
        StartMenuController.progress = StartMenuController.progress + 20;
        //Debug.Log("Done 2");
    }
    public static void LoadGame3() {
        tsumProg = new TsumPlayerProgression(); tsumProg.Load(GameState.me.tsumProg);
        StartMenuController.progress = StartMenuController.progress + 20;
        //Debug.Log("Done 3");
    }
    public static void LoadGame4()
    {
        tangramProg = new TangramPlayerProgression(); tangramProg.Load(GameState.me.tangramProg);
        StartMenuController.progress = StartMenuController.progress + 20;
        //Debug.Log("Done 4");
    }
    //public static void LoadGame5()
    //{
    //    StartMenuController.progress = StartMenuController.progress + 20;
    //    takoProg = new TakoPlayerProgression(); FvTProg.Load(GameState.me.takoProg);
    //    Debug.Log("Done 5");
    //}
    public static void LoadGame5()
    {
        StartMenuController.progress = StartMenuController.progress + 20;
        FvTProg = new FvTProgression(); FvTProg.Load(GameState.me.FvTProg);
        //Debug.Log("Done 5");
    }
    public static void SaveGamesToPlayerStats()
    {
        GameState.me.pearlyProg = pearlyProg.SaveAsString();
        GameState.me.infiniteProg = infiniteProg.SaveAsString();
        GameState.me.tsumProg = tsumProg.SaveAsString();
        GameState.me.tangramProg = tangramProg.SaveAsString();
        //GameState.me.takoProg = takoProg.SaveAsString();
        GameState.me.FvTProg = FvTProg.SaveAsString();


    }
    public static void UnlockAll()
    {
		for (int i = 0; i < 100; i++)
		{
			var tumbleLevelUnlock = GameState.tsumProg.GetLevel(i);
			var pearlyLevelUnlock = GameState.pearlyProg.GetLevel(i);
			var mantaLevelUnlock = GameState.tangramProg.GetLevel(i);
            var infiniteLevelUnlock = GameState.infiniteProg.GetLevel(i);
			tumbleLevelUnlock.status = TsumPlayerProgression.ELevelStatus.Available;
			pearlyLevelUnlock.status = PearlyProgression.ELevelStatus.Available;
			mantaLevelUnlock.status = TangramPlayerProgression.ELevelStatus.Available;
            infiniteLevelUnlock.status = InfiniteBrosProgression.ELevelStatus.Available;
            tumbleLevelUnlock.starEarned = 3;
            pearlyLevelUnlock.starEarned = 3;
            mantaLevelUnlock.starEarned = 3;
            infiniteLevelUnlock.starEarned = 3;

            GameState.infiniteProg.SetLevel(i, infiniteLevelUnlock);
            GameState.tsumProg.SetLevel(i, tumbleLevelUnlock);
			GameState.pearlyProg.SetLevel(i, pearlyLevelUnlock);
			GameState.tangramProg.SetLevel(i, mantaLevelUnlock);
		}
        GameState.me.inventory.Coins += 190847521;
        GameState.me.inventory.Jewels += 190847521;
        GameState.me.inventory.Ceramic += 190847521;
        GameState.me.inventory.StoneTablet += 190847521;
        GameState.me.inventory.Steel += 190847521;
        GameState.me.inventory.Wood += 190847521;
        GameState.me.Upload();
    }
    public static void ResetAll()
    {
		GameState.me.ResetEmpty();
    }
    public static int pearlyIndex;
    public static int infiniteIndex;
    public static int tsumIndex;
    public static int tangramIndex;
    public static void UpdateIndex()
    {
        StringHelper.DebugPrint("UpdateIndex");
        CTIndexScorer.Initialize();
        pearlyIndex = CTIndexScorer.GetCTScoreForGame(pearlyProg);
        infiniteIndex = CTIndexScorer.GetCTScoreForGame(infiniteProg);
        tsumIndex = CTIndexScorer.GetCTScoreForGame(tsumProg);
        tangramIndex = CTIndexScorer.GetCTScoreForGame(tangramProg);
        //Debug.Log("Indices: " + pearlyIndex + "," + infiniteIndex + ",." + tsumIndex + "," + tangramIndex);

		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//        form.AddField("user_id", GameState.me.id);
//        form.AddField("pearly_prog", pearlyIndex.ToString());
//        form.AddField("infinite_prog", infiniteIndex.ToString());
//        form.AddField("tsum_prog", tsumIndex.ToString());
//        form.AddField("tangram_prog", tangramIndex.ToString());
//        AppServer.CreatePost("me/heatmap/update", form)
//        .Subscribe(
//            x => Debug.Log(x), // onSuccess
//            ex => Debug.Log(ex) // onError
//        );
    }
    public static void SaveGames()
    {
        pearlyProg.Save();
        infiniteProg.Save();
        tsumProg.Save();
        tangramProg.Save();
    }

	public static Dictionary<string, int> FurnitureCostTable = new Dictionary<string, int>();

	public enum State
	{
		Creation = 0,
		Room = 1,
		Explore = 2
	}

	public enum GameQuality
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public static GameQuality quality = GameQuality.Low;

	public static void SetQuality (GameQuality q)
	{
		#if UNITY_WEBPLAYER || UNITY_WEBGL
		switch (GameState.quality) {
		case GameState.GameQuality.Low:
			QualitySettings.pixelLightCount = 1;
			QualitySettings.vSyncCount = 0;
			QualitySettings.antiAliasing = 0;
			break;
		case GameState.GameQuality.Medium:
			QualitySettings.pixelLightCount = 2;
			QualitySettings.vSyncCount = 1;
			QualitySettings.antiAliasing = 4;
			break;
		case GameState.GameQuality.High:
			QualitySettings.pixelLightCount = 2;
			QualitySettings.vSyncCount = 1;
			QualitySettings.antiAliasing = 8;
			break;
		}
		#else
		//IMPORTANT: In 4.6 setting vSync causes blackscreen on some android devices (xiaomi note tested)
		switch (GameState.quality) {
		case GameState.GameQuality.Low:
			QualitySettings.pixelLightCount = 1;
			QualitySettings.vSyncCount = 0;
			QualitySettings.antiAliasing = 0;
			break;
		case GameState.GameQuality.Medium:
			QualitySettings.pixelLightCount = 1;
			QualitySettings.vSyncCount = 0;
			QualitySettings.antiAliasing = 1;
			break;
		case GameState.GameQuality.High: // same as medium for mobile due giving black screen for certain higher settings
			QualitySettings.pixelLightCount = 1;
			QualitySettings.vSyncCount = 0;
			QualitySettings.antiAliasing = 2;
			break;
		}
		#endif

		quality = q;
	}

    public static void PostActivity(string interact_id, int activityType, int duration, int location,string room_id,int was_kicked)
    {
        StringHelper.DebugPrint("PostActivity");
        Debug.Log("Posting activity with Interact ID: " + interact_id + " activity type: " + activityType + " unique id: " + room_id);
        //analytic code here - Clinton
        string LocationName = null;
        switch (location)
        {
            case 1:
                LocationName = "Around Sally";
                break;
            case 2:
                LocationName = "Coral Reef";
                break;
            case 3:
                LocationName = "Market Square";
                break;
            case 4:
                LocationName = "Sunken Freight";
                break;

        }
        switch (activityType)
        {
            case 3:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.Exploration, "Went Exploring at " + LocationName + " for " + duration.ToString() + " hours");
                break;
            case 4:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.Exploration, "Exploration Completed at " + LocationName + " for " + duration.ToString() + " hours");
                break;
            case 5:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.Exploration, "Returned from Exploration at " + LocationName + " for " + duration.ToString() + " hours");
                break;
        }

        //if(location == 1)
        //{
        //    LocationName = "Sally";
        //}
        //else if (location == 2)
        //{
        //    LocationName = "Coral";
        //}
        //else if (location == 3)
        //{
        //    LocationName = "Market";
        //}
        //else if (location == 4)
        //{
        //    LocationName = "Freight";
        //}
        //else
        //{
        //    LocationName = "nullspace lel";
        //}
        //if (activityType == 3)
        //{
        //    AnalyticsSys.Instance.Report(REPORTING_TYPE.Exploration, "Went Exploring at "+ LocationName + " for "  + duration.ToString() + " hours" );
        //}
        //else if (activityType == 4)
        //{
        //    AnalyticsSys.Instance.Report(REPORTING_TYPE.Exploration, "Exploration Completed at " + LocationName + " for " + duration.ToString() + " hours");
        //}
        //else if (activityType == 5)
        //{
        //    AnalyticsSys.Instance.Report(REPORTING_TYPE.Exploration, "Returned from Exploration at " + LocationName + " for " + duration.ToString() + " hours");
        //}
        // HOWARD - Deprecated network code - TO REPLACE
        //        WWWForm form = new WWWForm();
        //        form.AddField("child_id", GameState.me.id);
        //        form.AddField("interact_id", interact_id);
        //        form.AddField("activity_type", activityType);
        //        form.AddField("duration", duration.ToString());
        //        form.AddField("location", location.ToString());
        //        form.AddField("room_id", room_id);
        //        string blah = (was_kicked == 0) ? "FALSE" : "TRUE";
        //        Debug.Log("Sending: " + blah);
        //        form.AddField("was_kicked", blah);
        //        AppServer.CreatePost("activity/post", form)
        //        .Subscribe(
        //            x => Debug.Log(x), // onSuccess//1479366761349616900
        //            ex => Debug.Log(ex) // onError
        //        );
    }
    public static void PostActivity2(string interact_id, int activityType, int duration, int location, string room_id, int was_kicked)
    {
        StringHelper.DebugPrint("PostActivity2");
        Debug.Log("Posting activity with Interact ID: " + interact_id + " activity type: " + activityType + " unique id: " + room_id);
		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//        form.AddField("child_id", interact_id);
//        form.AddField("interact_id", GameState.me.id);
//        form.AddField("activity_type", activityType);
//        form.AddField("duration", duration.ToString());
//        form.AddField("location", location.ToString());
//        form.AddField("room_id", room_id);
//        string blah = (was_kicked == 0) ? "FALSE" : "TRUE";
//        form.AddField("was_kicked", blah);
//        Debug.Log("Sending: " + blah);
//        AppServer.CreatePost("activity/post", form)
//        .Subscribe(
//            x => Debug.Log(x), // onSuccess//1479366761349616900
//            ex => Debug.Log(ex) // onError
//        );
    }
    public static void PostActivityOther(string child_id, string interact_id, int activityType, int duration, int location, string room_id, int was_kicked)
    {
        StringHelper.DebugPrint("PostActivityOther");
        Debug.Log("Posting activity with Interact ID: " + interact_id + " activity type: " + activityType + " unique id: " + room_id);
		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//        form.AddField("child_id", child_id);
//        form.AddField("interact_id", interact_id);
//        form.AddField("activity_type", activityType);
//        form.AddField("duration", duration.ToString());
//        form.AddField("location", location.ToString());
//        form.AddField("room_id", room_id);
//        string blah = (was_kicked == 0) ? "FALSE" : "TRUE";
//        form.AddField("was_kicked", blah);
//        Debug.Log("Sending: " + blah);
//        AppServer.CreatePost("activity/post", form)
//        .Subscribe(
//            x => Debug.Log(x), // onSuccess//1479366761349616900
//            ex => Debug.Log(ex) // onError
//        );
    }
    public static void PostSelfie(int feed_type, string description)
    {
        StringHelper.DebugPrint("PostSelfie");
        // HOWARD - Deprecated network code - TO REPLACE
        //        WWWForm form = new WWWForm();
        //        form.AddField("child_id", GameState.me.id);
        //        form.AddField("feed_type", feed_type.ToString());
        //        form.AddField("first_name", GameState.me.FirstName);
        //        form.AddField("display_photo", description);
        //        string avatarIcon = (GameState.playerBuddy.avatarIndex + 1) + "_" + (GameState.playerBuddy.bodyIndex + 1) + ".png";
        //        form.AddField("avatar_icon", avatarIcon);
        //        AppServer.CreatePost("feed/post/selfie", form)
        //        .Subscribe(
        //            x => Debug.Log(x), // onSuccess
        //            ex => Debug.Log(ex) // onError
        //        );
    }
    public static void PostProgress(int feed_type, int level, string gametitle)
    {
        StringHelper.DebugPrint("PostProgress");
        StringHelper.DebugPrint(level.ToString());
        StringHelper.DebugPrint(gametitle);

        AnalyticsSys.Instance.Report(REPORTING_TYPE.GameProgress, "Completed Level " + level + " in " + gametitle);

		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//        form.AddField("child_id", GameState.me.id);
//        form.AddField("feed_type", feed_type.ToString());
//        form.AddField("first_name", GameState.me.FirstName);
//        form.AddField("level", level.ToString());
//        string avatarIcon = (GameState.playerBuddy.avatarIndex + 1) + "_" + (GameState.playerBuddy.bodyIndex + 1) + ".png";
//        form.AddField("avatar_icon", avatarIcon);
//        form.AddField("game_title", gametitle);
//        AppServer.CreatePost("feed/post/progress", form)
//        .Subscribe(
//            x => Debug.Log(x), // onSuccess
//            ex => Debug.Log(ex) // onError
//        );
    }
    public static void PostDuration(int feed_type, int duration, string gametitle)
    {
        StringHelper.DebugPrint("PostDuration");
        // HOWARD - Deprecated network code - TO REPLACE
        //        WWWForm form = new WWWForm();
        //        form.AddField("child_id", GameState.me.id);
        //        form.AddField("feed_type", feed_type.ToString());
        //        form.AddField("first_name", GameState.me.FirstName);
        //        form.AddField("duration", duration.ToString());
        //        string avatarIcon = (GameState.playerBuddy.avatarIndex + 1) + "_" + (GameState.playerBuddy.bodyIndex + 1) + ".png";
        //        form.AddField("avatar_icon", avatarIcon);
        //        form.AddField("game_title", gametitle);
        //        AppServer.CreatePost("feed/post/duration", form)
        //        .Subscribe(
        //            x => Debug.Log(x), // onSuccess
        //            ex => Debug.Log(ex) // onError
        //        );
    }
}
public class ActivityFeedIds
{
	public static readonly int FEED_DURATION_EVENT = 1;
	public static readonly int FEED_GAME_PROGRESS = 4;
	public static readonly int FEED_SELFIE = 3;
	public static readonly int ACTIVITY_BUDDY_EXPEDITION_SEND = 3;
	public static readonly int ACTIVITY_BUDDY_EXPEDITION_RETURN = 4;
	public static readonly int ACTIVITY_BUDDY_EXPEDITION_CANCEL = 5;
}
public class GameNamesFeed
{
	public static readonly string GAME_NAME_INFINITE = "Infinite Crab Brothers";
	public static readonly string GAME_NAME_PEARLY = "Pearly Whirly";
	public static readonly string GAME_NAME_TUMBLE = "Tumble Trouble";
	public static readonly string GAME_NAME_CHOMP = "Chomp Chomp";
	public static readonly string GAME_NAME_MANTA = "Manta Match Mania";
	public static readonly string GAME_NAME_TUGOTAKO = "Tug O Tako";
}