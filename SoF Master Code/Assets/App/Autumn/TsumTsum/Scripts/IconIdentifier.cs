using UnityEngine;
using System.Collections;

public class IconIdentifier : MonoBehaviour {

	public int iconNo;

	void FixedUpdate()
	{
		var currentVelocity = gameObject.GetComponent<Rigidbody2D>().velocity;

		if (currentVelocity.y <= 0f)
			return;

		currentVelocity.y = 0f;

		gameObject.GetComponent<Rigidbody2D>().velocity = currentVelocity;
	}
}
