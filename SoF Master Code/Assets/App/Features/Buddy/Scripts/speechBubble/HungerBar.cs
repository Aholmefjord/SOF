using UnityEngine;
using System.Collections;

public class HungerBar : MonoBehaviour {

	public GameObject existingBar;

	public Transform startPoint;
	public Transform endPoint;

	private float existingBarInitialXLength;
	private float score;

	// Use this for initialization
	void Start()
	{
		existingBarInitialXLength = existingBar.transform.localScale.x;
		existingBar.transform.localScale = new Vector3(0.0f, existingBar.transform.localScale.y, existingBar.transform.localScale.z);
	}
	bool hasLoadedMap = false;
	// Update is called once per frame
	void Update()
	{
		if (PlayerPrefs.GetFloat("BuddyHunger") > 1000)
			score = 1000;
		else
			score = PlayerPrefs.GetFloat("BuddyHunger");
		updateScoreBar();
	}
	void updateScoreBar()
	{
		existingBar.transform.localScale = new Vector3((score/1000) * existingBarInitialXLength,
											existingBar.transform.localScale.y,
											existingBar.transform.localScale.z);
	}
}
