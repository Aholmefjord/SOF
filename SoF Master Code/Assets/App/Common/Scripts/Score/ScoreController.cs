using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour {

	public int				pPlayerScore = 0;
	public float			pTimeTaken = 0.0f;

	public GameObject[]		StarArray;
	public GameObject		ScoreText;
	public GameObject		TimeText;
	public GameObject		ResultPanel;

	// Use this for initialization
	void Start () {
		ResultPanel = GameObject.Find("Result Panel");
		ResultPanel.SetActive(false);

		// Set all star to inactive
		for(int i = 0; i < 3; ++i)
			StarArray[i].SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
		// Instead of pausing the game, disable controls of the UI when stage finishes
		// Checks finish when results page is active
	}

	// Call this in the game script to set final score upon finishing level
	void SetScore(int score)
	{
		Text scoreString = ScoreText.GetComponent<Text>();
		scoreString.text = "Score: " + score.ToString();
	}

	// Call this in the game script to set final score upon finishing level
	void SetTimeTaken(float timeTaken)
	{
		Text timeString = TimeText.GetComponent<Text>();
		timeString.text = "Time: " + timeTaken.ToString();
	}
}
