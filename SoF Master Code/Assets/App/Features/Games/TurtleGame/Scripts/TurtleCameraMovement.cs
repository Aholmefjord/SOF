using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleCameraMovement : MonoBehaviour {

    public GameObject player;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 camPos = transform.position;
        camPos.x = player.transform.position.x + 65.0f;
        camPos.y = player.transform.position.y + 25.0f;
        transform.position = Vector3.Lerp(transform.position, camPos, 3 * Time.fixedDeltaTime);
	}
}
