using UnityEngine;
using System.Collections;

public class HomeController : MonoBehaviour {
	private static HomeController _instance;

	public static HomeController current {
		get
		{
			return _instance;
		}
	}

	void Awake() {
		_instance = this;
	}

	public void MapButtonClick()
	{
		LoadMapScene();
    }

	public void ExitButtonClick()
	{
		//DialogueBoxController.ShowMessage("Exit to start?", "", DialogueBoxController.Type.Confirm, gotoStartMenu);
	}

	public void gotoStartMenu()
	{
		MainNavigationController.GoToStartMenu();
	}

	void LoadMapScene()
	{
		Debug.LogError("SFX disabled");
		//AudioSystem.PlaySFX("UI/sfx_ui_click");
		MainNavigationController.GoToMap();
    }

	void Update()
	{

	}

}
