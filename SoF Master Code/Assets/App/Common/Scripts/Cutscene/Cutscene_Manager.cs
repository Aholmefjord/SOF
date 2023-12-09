using UnityEngine;
using System.Collections;

[System.Serializable]
public class Cutscene_Manager : MonoBehaviour
{
	// Leave as 0 to default to next scene
	[SerializeField]
	public int NextLevel;

	[SerializeField]
	public string NextScene;

	[SerializeField]
	public int GameStageNum;

	[SerializeField]
	public float[] TimeToChange;

	[SerializeField]
	public float TimeToFade;

	[SerializeField]
	public Sprite[] Scenes;

	[SerializeField]
	public UnityEngine.UI.Image CutsceneObject;

	[SerializeField]
	public UnityEngine.UI.Image Fader;

	[HideInInspector]
	public float interval = 0.0f;

	// Public Variables
	[Header("Debugging")]
	public int FrameCount = 0;
	public float TimeElapsed = 0.0f;
	public int NumOfScenes;

	// Private Variables
	private bool fading = false;
	private bool doOnce = false;
	private float[] fadeTime;
	private float[] fadeValue;

	public static GameObject animationScenes;


	// Use this for initialization
	void Start()
	{
		InitSceneAnimation();
		// Remove android's navigation bar
		Screen.fullScreen = true;

		// Set array lengths at Start() or else it causes a NullReferenceException
		fadeTime = new float[Scenes.Length];
		fadeValue = new float[Scenes.Length];

		if (Scenes.Length > 0)
			CutsceneObject.sprite = Scenes[FrameCount];

		if (Fader)
			fading = true;

		// Set the fadeTime as well as the fadeValue for each scene during Start()
		for (int i = 0; i < Scenes.Length; i++)
		{
			fadeTime[i] = TimeToChange[i] - TimeToFade;
			fadeValue[i] = 1.0f / (fadeTime[i]);

			//Debug.Log("Fade Time of Element " + i + ": " + fadeTime[i]);
			//Debug.Log("Fade Value of Element " + i + ": " + fadeValue[i]);
		}

		AudioSystem.stop_bgm();
	}

	// Update is called once per frame
	void Update()
	{
		interval += Time.deltaTime;
		TimeElapsed += Time.deltaTime;

		// For Fading: Only fade when there are scenes left
		if (FrameCount < Scenes.Length)
		{
			Faders();

			// Change scene when the it is time to change
			if (interval > TimeToChange[FrameCount])
			{
				FrameCount++;
				DeactivateCurrentSceneAnimation();
				ActivateCurrentSceneAnimation();
				// Set the sprite to the next scene (As long as FrameCount is smaller than the length of Scenes array)
				if (FrameCount < Scenes.Length)
					CutsceneObject.sprite = Scenes[FrameCount];

				// Reset the timing interval to 0 after setting the sprite
				interval = 0.0f;
			}
		}
		else
		{
			// This should trigger the transition to the next scene
			TransitScene();
		}
	}

	// Move to the next level or scene
	void TransitScene()
	{
		if (!doOnce)
		{
			//print(NextLevel);
			doOnce = true;
			//MainNavigationController.DoLoad(level);

			// if (NextLevel > 0)
			//     MainNavigationController.GoToGame(NextLevel);

			// else
			//     MainNavigationController.DoLoad(NextScene);
			MainNavigationController.DoLoad(NextScene);
		}
	}

	// To create a fading effect between the different scenes
	void Faders()
	{
		// Fading In
		if (fading && interval > (TimeToChange[FrameCount] - (TimeToFade / 2)))
		{
			Color tmp = Fader.color;
			tmp.a += fadeValue[FrameCount] * (Time.deltaTime * 10.0f);

			// Clamp the alpha value
			if (tmp.a > 1.0f)
				tmp.a = 1.0f;

			Fader.color = tmp;
		}

		// Fading Out
		if (fading && interval < (TimeToChange[FrameCount] - TimeToFade))
		{
			Color tmp = Fader.color;
			tmp.a -= fadeValue[FrameCount] * (Time.deltaTime * 10.0f);

			// Clamp the alpha value
			if (tmp.a < 0.0f)
				tmp.a = 0.0f;

			Fader.color = tmp;
		}
	}

	// Self-explanatory
	// For the skip button
	public void SkipCutscene()
	{
		//if (NextLevel > 0)
		//{
		//	MainNavigationController.GoToGame(NextLevel);	
		//}
		//else
		//{
		//	MainNavigationController.DoLoad(NextScene);
		//}
		MainNavigationController.DoLoad(NextScene);
	}

	// Self-explanatory
	// Returns the number of frames currently
	public int CurrentFrame()
	{
		return FrameCount;
	}



	public void InitSceneAnimation()
	{
		animationScenes = GameObject.FindGameObjectWithTag("Scene");
	}

	public void ActivateCurrentSceneAnimation()
	{
		if (FrameCount < animationScenes.transform.childCount)
		{
			for (int i = 0; i < animationScenes.transform.childCount; i++)
			{
				animationScenes.transform.GetChild(FrameCount).gameObject.SetActive(true);
			}
		}
	}
	public void DeactivateCurrentSceneAnimation()
	{
		for (int i = 0; i < animationScenes.transform.childCount; i++)
		{
			animationScenes.transform.GetChild(FrameCount - 1).gameObject.SetActive(false);
		}
	}
}
