using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MainNavigationController : MonoBehaviour
{
	private const string GAMEOBJECT_NAME = "LoadingScreenCanvas3"; //Also resources prefab name
	private static GameObject loadingScreenCanvas;
	private static LoadingScreenInstance loadingScreenInstance;
	private static Stage _stage;
	private static int _currentStageId;
	private static int _currentPuzzleLevel;
	private static string _returnScene; //after going to a game, when they press back or map, to return to which scene?

	public static void SetCurrentStageId (int id)
	{
		_currentStageId = id;
	}

	public static Stage currentStage
	{
		get
		{
			return _stage;
		}
	}

	public static int currentStageId
	{
		get
		{
			return _currentStageId;
		}
	}

	public static int currentPuzzleLevel
	{
		get
		{
			return _currentPuzzleLevel;
		}
	}

	public static string returnScene
	{
		get
		{
			return _returnScene;
		}
	}

	public static void GoToHome()
	{
		DoLoad("mapNew");
	}
    public void GoToLevelClick()
    {
        GoToLevel();

        if (Time.timeScale < 1f)
        {
            Time.timeScale = 1f;
        }
    }

    void GoToLevel()
    {
        MainNavigationController.GoToScene("Tsum Tsum Main Screen");
        MainMenuUI.opentsumui = true;
    }

    public static void GoToAvatarCreation()
	{
        Debug.Log("GoingToPetCreation");
		DoLoad("create_pet");
	}

	public static void GoToMap()
	{
		DoLoad("mapNew");
	}

	public static void GoToStartMenu(bool resetNextScene = true)
	{
		if (resetNextScene)
		{
			GameState.nextScene = null;
		}

        DoLoad("start_menu");
        //DoAssetBundleLoadLevel("start-menu-bundle", "start_menu");
    }
    public static void GoToScene(string name, string returnScene = "")
    {
        Debug.Log("GoToScene: " + name);
        DoLoad(name, returnScene);
    }

    public static void GoToSceneWithItemToLoad(string name, int itemNeededToLoad)
    {
        Debug.Log("GoToScene: " + name);
        DoLoad(name, itemNeededToLoad, returnScene);
    }

    public static void GoToCinematic(string name, string returnScene = "")
    {
        Debug.Log("GoToCinematic: " + name);
        //DoLoad(name, returnScene);
        DoAssetBundleLoadLevel("cinematics-scenes-bundle", name, returnScene);
    }

    public static void GotoMainMenu()
    {
        Debug.Log("MainNavigationController GoToMainMenu");

        //Pop the current front state in state manager
        if(GameSys.StateManager.Instance.NumberOfStates() > 0)
            GameSys.StateManager.Instance.RemoveState(0);

        DoLoad(Constants.mainMenuScene);
      
    }

	public static void GoToFreeze()
	{
		DoLoad("Freeze_scene");
    }

	public static void GoToCrabStage()
	{
		//DoLoad("crab_main_screen");
        DoAssetBundleLoadLevel(Constants.CRAB_BROS_SCENES, "crab_main_screen");
    }

	public static void GoToPearlyStage()
	{
        //DoLoad("pearly_main_screen");
        DoAssetBundleLoadLevel(Constants.PEARLY_SCENES, "pearly_main_screen");
	}

	public static void GoToMMMStage()//Manta Match Mania
	{
		//DoLoad("Tangram Main Menu");
        DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram Main Menu");
    }

    public static void GoToCutScene()
    {
        DoLoad("Cutscene");
    }
    
	public static void GoToTumbleTroubleMainScreen()
    {
        DoAssetBundleLoadLevel(Constants.TUMBLE_TROUBLE_SCENES, "TsumTsum Main Screen");
    }
    public static void GoToTumbleTroubleLevelSelect()
    {
        GoToTumbleTroubleMainScreen();
        MainMenuUI.opentsumui = true;
    }

    public static void GoToGame(int stageId, string returnScene = "")
	{
		_currentStageId = stageId;

		Hashtable stages = GameState.configs["Stage"] as Hashtable;
		if (stages == null) return;
		_stage = stages[stageId] as Stage;
		_currentPuzzleLevel = _stage.puzzleLevel; // Set stage so game knows what to load later
		_returnScene = returnScene;
		DoLoad("game" + _stage.puzzleType + "_start_menu", returnScene);
	}

	public static void setStage(int stageId)
	{
		_currentStageId = stageId;
		Hashtable stages = GameState.configs["Stage"] as Hashtable;
		if (stages == null) return;
		_stage = stages[stageId] as Stage;
		_currentPuzzleLevel = _stage.puzzleLevel;
	}

	public static void GoToGameTest(string gameId, int puzzleLevel = 0, string returnScene = "")
	{
		//This is for testing only
		_currentPuzzleLevel = puzzleLevel; // Set stage so game knows what to load later
		_stage = null;

		_returnScene = returnScene;
		DoLoad("game" + gameId + "_loader", returnScene);
	}

	public static void DoLoad(string scene, string returnScene = "")
	{
        DoLoad(scene, 0, returnScene);
	}

    public static void DoLoad(string scene, int itemToLoad, string returnScene = "")
    {
#if UNITY_EDITOR
        print("Load Scene: " + scene);
#endif
        initLoadingScreen();
        loadingScreenInstance.Init(scene, itemToLoad);
        _returnScene = returnScene;

        if (!loadingScreenInstance)
        {
            Debug.Log("Error with panel instance.");
        }
    }

    public static void DoLoad(string scene, bool game6, string returnScene = "")
    {
#if UNITY_EDITOR

        print("Load Scene: " + scene);
#endif
        initLoadingScreen();
        loadingScreenInstance.Init(scene);
        _returnScene = returnScene;


        if (!loadingScreenInstance)
        {
            Debug.Log("Error with panel instance.");
        }
    }
	public static void GoToNewCutscene()
	{
		DoLoad("Scene1");
	}


    public static void DoAssetBundleLoadLevel(string bundleName, string sceneName, string returnScene = "")
    {
        initLoadingScreen();
        //loadingScreenInstance.Init(sceneName);
        _returnScene = returnScene;

        loadingScreenInstance.StartCoroutine(loadingScreenInstance.LoadFromAssetBundle(bundleName, sceneName));

        if (!loadingScreenInstance) {
            Debug.Log("Error with panel instance.");
        }
    }

    static void initLoadingScreen()
    {
        if (!loadingScreenCanvas) {
            //Create from resources if not already created
            loadingScreenCanvas = Instantiate(Resources.Load("gui/panels/" + GAMEOBJECT_NAME)) as GameObject;
            loadingScreenCanvas.transform.SetParent(null, false);
            loadingScreenCanvas.name = GAMEOBJECT_NAME;
        } else {
            loadingScreenCanvas.SetActive(true);
        }

        loadingScreenInstance = loadingScreenCanvas.GetComponent<LoadingScreenInstance>();
    }

    public static void UpdateLoadingItemProgress(int _count)
    {
        loadingScreenInstance.UpdateLoadingItem(_count);
    }

    public static void LoadItemFailed()
    {
        loadingScreenInstance.DisplayReloadPanel();
    }
}
