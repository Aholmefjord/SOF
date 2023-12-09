using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchScore : MonoBehaviour {

	[HideInInspector]
	public MatchGameManager gm
	{
		get { return JMFUtils.gm; }
	}

	private float initialScore; 
	// Use this for initialization
	void Start () {
		initialScore = PlayerPrefs.GetFloat("BuddyHunger",0);
		
		this.GetComponent<Text> ().text = gm.score.ToString();
	}
	
	// Update is called once per frame
	void Update () {

        //Score should never drop because of time.
        //gm.score -= Time.deltaTime * 100f;

        Debug.Log(gm.score);

        if (initialScore != gm.score) {
			this.GetComponent<Text> ().text = gm.score.ToString();
			initialScore = gm.score;
            PlayerPrefs.SetFloat("BuddyHunger", initialScore);
        }

	}
}