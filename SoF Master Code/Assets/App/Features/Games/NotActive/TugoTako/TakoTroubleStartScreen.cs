using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Reflection;

public class TakoTroubleStartScreen : MonoBehaviour {

    int lastLevel;
    public Text lastText;
	// Use this for initialization
	void Start () {
        GameSys.GameState gameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();
        lastLevel = gameState.startLevel;
        Debug.Log("gamestate level =  " + gameState.startLevel);
        SetupUI();
	}

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(gameObject.FindChild("LevelSelectButton").FindChild("Text"), "tako_level_select_level_text");
        lastText.text = MultiLanguage.getInstance().getString("tako_level_select_level_text") + (lastLevel);

        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Title").GetComponent<Image>(), "tako_main_menu_title");
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("LevelSelectButton").FindChild("Image").GetComponent<Image>(), "tako_main_menu_start");
    }

    public void ClickToPlay(int level)
    {
        PlayerPrefs.SetInt("TakoLevel", lastLevel - 1);
        MainNavigationController.GoToScene("tako_trouble"); 
    }

    public void ReturnToStartMenu()
    {
        MainNavigationController.GotoMainMenu();
    }
}
