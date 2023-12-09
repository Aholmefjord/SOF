using UnityEngine;
using System.Collections;

public class PearlBehaviour : MonoBehaviour {

	Vector3 newPosition;
	float speed = 7.5f;

	// Use this for initialization
	void Start ()
	{
		newPosition = transform.position;
		Destroy(gameObject, 0.5f);
	}
	
	// Update is called once per frame
	void Update ()
	{
		newPosition.y += Time.deltaTime * speed;
		transform.position = newPosition;
	}

	void OnDestroy()
	{
		GameObject go = (GameObject)Instantiate(Resources.Load("BiggerExplosion"), new Vector3(transform.position.x, transform.position.y, transform.position.z - 5), transform.rotation);
		go.transform.localScale *= 1.5f;
		AudioSystem.PlaySFX("buttons/JULES_CRAB_CORRECT_02");
	}
}
