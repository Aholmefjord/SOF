using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapNode : MonoBehaviour {

	public string gameId;
	public int puzzleLevel;
	public GameObject[] stars;
	public string returnScene;
	public Stage stage;
	public GameObject lockImage;
	public Button nodeButton;
	public Text stageText;
	public Animator pulsateAnimator;

	public void ButtonClick()
	{
		Debug.LogError("disabled sfx");
		//AudioSystem.PlaySFX("UI/sfx_ui_click");
		if (stage == null)
		{
			//Test or offline mode
			MainNavigationController.GoToGameTest(gameId, puzzleLevel, returnScene);
		} else {
			MainNavigationController.GoToGame(stage.id, returnScene);
		}
		
	}
}
