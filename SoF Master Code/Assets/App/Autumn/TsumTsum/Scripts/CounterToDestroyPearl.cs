using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CounterToDestroyPearl : MonoBehaviour
{
	public int counterToDestroy;
	public bool shouldCounterincrease;
	public int counter;
	public bool shouldHighlight;
	public Sprite[] pearlImage;

	public GameObject pearl;
	// Use this for initialization
	void Start() {
	}

	// Update is called once per frame
	void Update() {
		if (shouldHighlight)
			transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
		else
			transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;

		if (counter < 3)
			gameObject.GetComponent<SpriteRenderer>().sprite = pearlImage[counter];
	}

	public void SpawnOnDestroy()
	{
		//not using OnDestroy() cause it would get called when levels is being replayed
		Instantiate(pearl, transform.position, transform.rotation);
	}
}
