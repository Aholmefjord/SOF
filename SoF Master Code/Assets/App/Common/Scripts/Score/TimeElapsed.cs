using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeElapsed : MonoBehaviour
{
	public float timeElapsed = 0.0f;
	[SerializeField]
	Text timeText;
	[SerializeField]
	Color col;

	// Use this for initialization
	void Start ()
	{
		if (timeText)
			timeText.color = col;
    }

	public void startTimer()
	{
		//StartCoroutine("UpdateTime");
	}

	public void stopTimer()
	{
		//StopCoroutine("UpdateTime");
	}
	
	// Update is called once per frame
	IEnumerator UpdateTime () {
		while (true)
		{
			timeElapsed += Time.deltaTime;

			if (timeText)
				timeText.text = ((int)timeElapsed).ToString("D3");

			yield return null;
		}
	}
}
