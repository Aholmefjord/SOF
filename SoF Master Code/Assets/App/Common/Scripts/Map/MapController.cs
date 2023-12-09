using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapController : MonoBehaviour {
	private static MapController _instance;
	public GameObject ForeGroundPropsHolder;
	public GameObject StageNodeHolder;
	public ScrollRect MapScrollRect;

	public GameObject DomeButtonLockImage;
	public Button DomeButton;

	public static MapController current {
		get
		{
			return _instance;
		}
	}

	void Awake() {
#if UNITY_EDITOR
		if (GameState.me == null)
		{
			GameState.nextScene = "map";
			Application.LoadLevel("start_menu");
		}
#endif

		_instance = this;
		Animator[] propsAnim = ForeGroundPropsHolder.GetComponentsInChildren<Animator>();
		foreach (Animator a in propsAnim)
		{
			a.Play(0, -1, Random.value);
		}

		AudioSystem.PlayBGM("bgm_map");

		//Temp destroy TtR bgm
		GameObject bgm = GameObject.Find("Music");
		if (bgm != null)
		{
			Destroy(bgm);
		}
	}

	void Start()
	{
		if (MapScrollRect != null)
		{
			MapScrollRect.horizontalNormalizedPosition = PlayerPrefs.GetFloat("map_posit", 0);
		}

		MapNode[] nodes = StageNodeHolder.GetComponentsInChildren<MapNode>();
		Hashtable stages = GameState.configs["Stage"] as Hashtable;
		if (stages == null) return;

		bool totheHouseCleared = Stage.toTheHouseCleared();
		if (!totheHouseCleared)
		{
			MainNavigationController.GoToGame(1);
			return;
		}
		//Avatar not created
		if (GameState.me.avatar == null || GameState.me.avatar.id == 0)
		{
			MainNavigationController.GoToAvatarCreation();
			return;
		}

		int stageId = 1;
		int unlockedProgressOrder = stageId;

		foreach (MapNode n in nodes)
		{
			if (stages[stageId] == null)
			{
				Debug.LogWarning("Stage data not found for index: " + (stageId) );
				continue;
			}

			n.stage = stages[stageId] as Stage; //Due buddy creation takes the first 3 stages.
			int stars = 0;

			foreach (KeyValuePair<int, PlayerAchievement> pa in GameState.me.achievements)
			{
				if (pa.Value.achievementId == n.stage.achievementId)
				{
					if (pa.Value.progress >= n.stage.star3ScoreNeeded)
					{
						stars = 3;
					}
					else if (pa.Value.progress >= n.stage.star2ScoreNeeded)
					{
						stars = 2;
					}
					else if (pa.Value.progress >= n.stage.star1ScoreNeeded)
					{
						stars = 1;
					}

					if (stars >= 1)
					{
						//Has unlocked only if got at least 1 star
						unlockedProgressOrder = n.stage.order + 1;
					}
				}
			}

			//Check if house to unlock
			if (unlockedProgressOrder > 1)
			{
				DomeButton.interactable = true;
				DomeButtonLockImage.SetActive(false);
			} else
			{
				DomeButton.interactable = false;
				DomeButtonLockImage.SetActive(true);
			}

			int count = 0;
			foreach (GameObject o in n.stars)
			{
				count++;
				if (stars >= count)
				{
					o.SetActive(true);
				} else
				{
					o.SetActive(false);
				}
			}

			n.stageText.text = n.stage.id.ToString();

			if (n.stage.order == unlockedProgressOrder)
			{
				//n.pulsateAnimator.Play();
			} else
			{
				n.pulsateAnimator.enabled = false;
			}

			if (n.stage.order <= unlockedProgressOrder)
			{
				n.lockImage.SetActive(false);
				n.nodeButton.interactable = true;
			}
			else
			{
				n.pulsateAnimator.enabled = false;
				n.lockImage.SetActive(true);
				n.nodeButton.interactable = false;
			}

			stageId++;
		}
	}

	public void HomeButtonClick()
	{
		Debug.LogError("disabled sfx");
		//AudioSystem.PlaySFX("UI/sfx_ui_click");
		LoadHomeScene();
    }

	public void DialogueTestButtonClick()
	{
		Debug.LogError("disabled sfx");
		//AudioSystem.PlaySFX("UI/sfx_ui_click");
		MainNavigationController.DoLoad("dialogue_demo");
	}

	void LoadHomeScene()
	{
		MainNavigationController.GoToHome();
    }

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			bool totheHouseCleared = Stage.toTheHouseCleared();
			if (totheHouseCleared)
			{
				MainNavigationController.GoToHome();
			} else
			{
				MainNavigationController.GoToStartMenu();
			}
		}
	}

	public void ToMainMapButtonClick()
	{
		Debug.LogError("disabled sfx");
		//AudioSystem.PlaySFX("UI/sfx_ui_click");
		MainNavigationController.GoToMap();
	}

	public void OnMapScrolled()
	{
		PlayerPrefs.SetFloat("map_posit", MapScrollRect.horizontalNormalizedPosition);
	}


}
