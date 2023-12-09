using System;
using UnityEngine;

[Flags]
public enum SOFNetworkStatus
{
	OFFLINE = 0x0,
	HAS_CONNECTION = 0x1,
	IS_JULES_BOX = 0x2
}

public struct IntVector2
{
    public int x;
    public int y;

    public IntVector2(int _x, int _y) { x = _x;  y = _y; }
}

static public class REPORTING_TYPE
{
	public const string LoginResult = "LoginResult";
    public const string ClickButton = "ClickedButton";
    public const string StartGame = "StartedGame";
    public const string PlayVideo = "PlayedVideo";
    public const string ChangedBuddy = "BuddyChangedTo";
    public const string StartedWorksheet = "StartedWorksheet";
    public const string GameResult = "GameResult";
    public const string GameProgress = "GameProgress";
    public const string Exploration = "Exploration";
    public const string BuyFurniture = "FurnitureBought";
    public const string PlaceFurniture = "FurniturePlaced";
    public const string PearlyWhirlyData = "PearlyWhirlyData";
}

static public class Constants
{
	public const int PackageVersion = 37;
    public const bool uses_cinematics = false;
    public const bool uses_tugotako = true;
    public const bool is_sales_build = false;

	public static string DEFAULT_PASSWORD = "iwetmybed";
    
	// HOWARD - TO NOTE!!! Reroute AppURL over to the cloud PHP server address!
#if UNITY_EDITOR
	public static string AppURL = "http://ec2-54-255-246-77.ap-southeast-1.compute.amazonaws.com/";
	public static string IMAGE_URL = "http://api.juleszone.com:80/";
#else
    // public const string AppURL = "http://210.242.193.237:10004/";
    //public static string AppURL = "http://api.juleszone.com:3001/";
	public static string AppURL = "http://ec2-54-255-246-77.ap-southeast-1.compute.amazonaws.com/";
    public static string IMAGE_URL = "http://api.juleszone.com:80/";
    // public const string AppURL = "http://192.168.0.164:3001/";
    // public const string AppURL = "http://localhost:3001/";
    // public const string AppURL = "http://app.jules.sg:3001/";
    // public const string AppURL = "http://210.242.193.237:10004/";
    // public const string AppURL = "http://61.58.43.38:3001/";
    // public const string AppURL = "http://192.168.0.164:3001/";
    // public const string AppURL = "http://localhost:3001/";
#endif 

    public static string UpdateURL = "http://update.juleszone.com/";
#if UNITY_ANDROID
    public const string BundlesPath = "bundles/android/";
#elif UNITY_IOS
	public const string BundlesPath = "bundles/ios/";
#else
	//ERROR
#endif

	public const string AppName = "School of Fish(Oceanize)";
	public const int AppVersionValue = 2;

	public const string StaticServerURL = "http://cdn.juleszone.com/assets/sof/";
	public const string BundlesExt = ".class";
	public const string PLATFORM_JULES = "JULES";
	public const string PLATFORM_DEVICE = "DEV";
	public const string JULES_GAME_ID = "1"; //allocated game id

	public static IFormatProvider CULTURE = new System.Globalization.CultureInfo("fr-FR", true);

	public static string AVATAR_BASE_PATH = "avatar/prefab/";
	public static string AVATAR_SKIN_BASE_PATH = "avatar/skin/";

	public static float HOME_FURNITURE_SCALE = 0.7f;

	public const string defaultDownloadURL = "http://www.jules.sg/app";
	public const string AndroidDownloadURL = "market://details?id=com.juleszone";
	public const string iOSDownloadURL = "https://itunes.apple.com/us/app/school-of-fish/id1091993706?ls=1&mt=8";

    public const string lessonDataPath = "GameSceneManager/lesson_data/";
    public const string lessonPlanPath = "GameSceneManager/lesson_plans/";
    public const string sof_lessonPlanPath = "GameSceneManager/sof_lesson_plans/";
    public const string schoolPath = "GameSceneManager/school_data/";
    public const string mainMenuScene = "mapNew"; //CampaignLevel//CampaignMode 
    public const string defaultLanguage = "English";

    public const string ImageDisplayStateName = "Image Display State";

    #region AssetBundleLocations
    public const string AssetBundleServerAddress = "https://s3-ap-southeast-1.amazonaws.com/jules-sof-assetbundles";
    /// final bundle location should be AndroidAssetBundleServer/BuildBranch/GetPlatform()/
    //public const string BuildBranch = "CarpeDiem"; // "G8", "AnotherCompanyName"

    public const string LOCALIZATION_BUNDLE_TEMPLATE = "localization/{0}";
    public const string LOCALIZATION_BUNDLE_CONFIG = "localization/localization-config-bundle";
    public const string FONTS_SHARED_BUNDLE = "shared/shared-fonts-bundle";
    public const string SFX_SHARED_BUNDLE = "shared/shared-audio-sfx-bundle";
    public const string BGM_SHARED_BUNDLE = "shared/shared-audio-bgm-bundle";
    public const string GAMES_SHARED = "games/games-shared-bundle";
    //public const string CHOMPCHOMP_SHARED = "games/chompchomp-bundle";
    public const string CHOMPCHOMP_SCENES = "games/chompchomp-bundle";
    public const string CRAB_BROS_SHARED = "games/infinitecrabbros-bundle";
    public const string CRAB_BROS_SCENES = "games/infinitecrabbros-scenes-bundle";
    public const string CRAB_BROS_CONFIG = "games/infinitecrabbros-configs-bundle";
    public const string MANTA_MATCH_SHARED = "games/mantamatchmania-bundle";
    public const string MANTA_MATCH_CONFIG = "games/mantamatchmania-configs-bundle";
    public const string MANTA_MATCH_SCENES = "games/mantamatchmania-scenes-bundle";
    public const string TUMBLE_TROUBLE_SHARED = "games/tumbletrouble-bundle";
    public const string TUMBLE_TROUBLE_CONFIG = "games/tumbletrouble-configs-bundle";
    public const string TUMBLE_TROUBLE_SCENES = "games/tumbletrouble-scenes-bundle";
    public const string PEARLY_SHARED = "games/pearly-bundle";
    public const string PEARLY_CONFIGS = "games/pearly-configs-bundle";
    public const string PEARLY_SCENES = "games/pearly-scenes-bundle";
    public const string FVT_CONFIGS = "games/fishvtako-configs-bundle";
    public const string FVT_SCENES = "games/fishvtako-scenes-bundle";
    public const string FVT_SHARED = "games/fishvtako-bundle";
    public const string INGAME_STORYBOOK_SCENE = "storybook/storybook-scene-bundle";
    public const string INGAME_STORYBOOK_CONFIG = "storybook/chapter1";
    public const string INGAME_STORYBOOK_TEMPLATE = "storybook/chapter{0}";
    #endregion

    public enum ItemType
	{
		General,
		Clothing,
		Furniture
	}

	public enum ItemSubType 
	{
		General,
		LivingRoom,
		Kitchen,
		BedRoom,
		Toilet
	}
}

public class VideoURL
{
	#region EpisodeLinks
	public static string[] episodesOnlineEnglish = new string[]
	{
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep01.mp4",//"https://youtu.be/qddGp-tieQk", //1
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep02.mp4",//"https://youtu.be/rJWO9diiX3k",//2
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep03.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep04.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep05.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep06.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep07.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep08.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep09.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep10.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep11.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep12.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep13.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep14.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep15.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep16.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep17.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep18.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep19.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep20.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep21.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep22.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/en/DL_Ep23.mp4"
		/*
        "https://youtu.be/8nnSBogmzJY",//3
        "https://youtu.be/Huozydk0GIE",//4
        "https://youtu.be/4s9ghxLd_EI",//5
        "https://youtu.be/5fNTZ-YHV_g",//6
        "https://youtu.be/hILVfeLLqDg",//7
        "https://youtu.be/Iw096DWp0aI",//8
        "https://youtu.be/6Ul7Vfe6HkI",//9
        "https://youtu.be/ACCdlgx2J60",//10
        "https://youtu.be/spJeclGTrU8",//11
        "https://youtu.be/qPRyH-_WnxY",//12
        "https://youtu.be/nOcwn1vtn04",//13
        "https://youtu.be/V4bHWJQ0EWc",//14
        "https://youtu.be/sZYYy7VEVZ8",//15
        "https://youtu.be/x-b9OxdfLiQ",//16
        "https://youtu.be/AA4RQMmm9TE",//17
        "https://youtu.be/vpMMXijKZY4",//18
        "https://youtu.be/zdGe_PWzc7o",//19
        "https://youtu.be/yGy53KTY8Kw",//20
        "https://youtu.be/YoauPdqoDBo",//21
        "https://youtu.be/rVlXGDXwKhY",//22
        "https://youtu.be/_PBoUlqqCW0"//23
        */
	};
	public static string[] episodesPiboxEnglish = new string[]
	{
		"DL_Ep01.mp4", //1
		"DL_Ep02.mp4",//2
		"DL_Ep03.mp4",//3
		"DL_Ep04.mp4",//4
		"DL_Ep05.mp4",//5
		"DL_Ep06.mp4",//6
		"DL_Ep07.mp4",//7
		"DL_Ep08.mp4",//8
		"DL_Ep09.mp4",//9
		"DL_Ep10.mp4",//10
		"DL_Ep11.mp4",//11
		"DL_Ep12.mp4",//12
		"DL_Ep13.mp4",//13
		"DL_Ep14.mp4",//14
		"DL_Ep15.mp4",//15
		"DL_Ep16.mp4",//16
		"DL_Ep17.mp4",//17
		"DL_Ep18.mp4",//18
		"DL_Ep19.mp4",//19
		"DL_Ep20.mp4",//20
		"DL_Ep21.mp4",//21
		"DL_Ep22.mp4",//22
		"DL_Ep23.mp4"//23
	};
	public static string[] episodesOnlineChinese = new string[]
	{
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep01CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep02CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep03CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep04CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep05CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep06CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep07CN.mp4", //this actually doesn't exist
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep08CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep09CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep10CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep11CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep12CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep13CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep14CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep15CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep16CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep17CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep18CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep19CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep20CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep21CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep22CN.mp4",
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/cn/DL_Ep23CN.mp4", //doesnt exist
		/*
        "https://youtu.be/qddGp-tieQk", //1
        "https://youtu.be/rJWO9diiX3k",//2
        "https://youtu.be/8nnSBogmzJY",//3
        "https://youtu.be/Huozydk0GIE",//4
        "https://youtu.be/4s9ghxLd_EI",//5
        "https://youtu.be/5fNTZ-YHV_g",//6
        "https://youtu.be/hILVfeLLqDg",//7
        "https://youtu.be/Iw096DWp0aI",//8
        "https://youtu.be/6Ul7Vfe6HkI",//9
        "https://youtu.be/ACCdlgx2J60",//10
        "https://youtu.be/spJeclGTrU8",//11
        "https://youtu.be/qPRyH-_WnxY",//12
        "https://youtu.be/nOcwn1vtn04",//13
        "https://youtu.be/V4bHWJQ0EWc",//14
        "https://youtu.be/sZYYy7VEVZ8",//15
        "https://youtu.be/x-b9OxdfLiQ",//16
        "https://youtu.be/AA4RQMmm9TE",//17
        "https://youtu.be/vpMMXijKZY4",//18
        "https://youtu.be/zdGe_PWzc7o",//19
        "https://youtu.be/yGy53KTY8Kw",//20
        "https://youtu.be/YoauPdqoDBo",//21
        "https://youtu.be/rVlXGDXwKhY",//22
        "https://youtu.be/_PBoUlqqCW0"//23
        */
	}; public static string[] episodesOnlineTraditionalChinese = new string[]
     {
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep01TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep02TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep03TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep04TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep05TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep06TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep07TW.mp4", //this actually doesn't exist
		"https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep08TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep09TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep10TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep11TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep12TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep13TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep14TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep15TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep16TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep17TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep18TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep19TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep20TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep21TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep22TW.mp4",
        "https://s3-ap-southeast-1.amazonaws.com/jules-sof-media/Doodle/zh-TW/DL_Ep23TW.mp4", //doesnt exist
        /*
        "https://youtu.be/qddGp-tieQk", //1
        "https://youtu.be/rJWO9diiX3k",//2
        "https://youtu.be/8nnSBogmzJY",//3
        "https://youtu.be/Huozydk0GIE",//4
        "https://youtu.be/4s9ghxLd_EI",//5
        "https://youtu.be/5fNTZ-YHV_g",//6
        "https://youtu.be/hILVfeLLqDg",//7
        "https://youtu.be/Iw096DWp0aI",//8
        "https://youtu.be/6Ul7Vfe6HkI",//9
        "https://youtu.be/ACCdlgx2J60",//10
        "https://youtu.be/spJeclGTrU8",//11
        "https://youtu.be/qPRyH-_WnxY",//12
        "https://youtu.be/nOcwn1vtn04",//13
        "https://youtu.be/V4bHWJQ0EWc",//14
        "https://youtu.be/sZYYy7VEVZ8",//15
        "https://youtu.be/x-b9OxdfLiQ",//16
        "https://youtu.be/AA4RQMmm9TE",//17
        "https://youtu.be/vpMMXijKZY4",//18
        "https://youtu.be/zdGe_PWzc7o",//19
        "https://youtu.be/yGy53KTY8Kw",//20
        "https://youtu.be/YoauPdqoDBo",//21
        "https://youtu.be/rVlXGDXwKhY",//22
        "https://youtu.be/_PBoUlqqCW0"//23
        */
     };
    public static string[] episodesPiboxChinese = new string[]
	{
		"DL_Ep01CN.mp4", //1
		"DL_Ep02CN.mp4",//2
		"DL_Ep03CN.mp4",//3
		"DL_Ep04CN.mp4",//4
		"DL_Ep05CN.mp4",//5
		"DL_Ep06CN.mp4",//6
		"DL_Ep07CN.mp4",//7 doesnt exist
		"DL_Ep08CN.mp4",//8
		"DL_Ep09CN.mp4",//9
		"DL_Ep10CN.mp4",//10
		"DL_Ep11CN.mp4",//11
		"DL_Ep12CN.mp4",//12
		"DL_Ep13CN.mp4",//13
		"DL_Ep14CN.mp4",//14
		"DL_Ep15CN.mp4",//15
		"DL_Ep16CN.mp4",//16
		"DL_Ep17CN.mp4",//17
		"DL_Ep18CN.mp4",//18
		"DL_Ep19CN.mp4",//19
		"DL_Ep20CN.mp4",//20
		"DL_Ep21CN.mp4",//21
		"DL_Ep22CN.mp4",//22
		"DL_Ep23CN.mp4"//23 doesnt exist
	};
	#endregion
	public static string GetDoodleEpisodeLink(int i)
	{
        //Debug.Log("HAS FOUND SERVER: " + JulesBox.HAS_FOUND_SERVER);
        //string prefix = JulesBox.HAS_FOUND_SERVER ? JulesBox.JulesBoxUrl + "media/" : "";
        //if (JulesBox.HAS_FOUND_SERVER)
        //{
        //	return prefix + (PlayerPrefs.GetString("language") == "English" ? episodesPiboxEnglish[i] : episodesPiboxChinese[i]);
        //}
        //else {
        if (PlayerPrefs.GetString("language") == "Chinese")
            return episodesOnlineChinese[i];
        else if (PlayerPrefs.GetString("language") == "TraditionalChinese")
            return episodesOnlineTraditionalChinese[i];

		return episodesOnlineEnglish[i];
		//}
	}
}