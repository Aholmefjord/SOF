using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement1 : MonoBehaviour {

   
    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.Translate(new Vector3(Time.fixedDeltaTime, 0.51098f * Time.fixedDeltaTime, 0));
    }
}
