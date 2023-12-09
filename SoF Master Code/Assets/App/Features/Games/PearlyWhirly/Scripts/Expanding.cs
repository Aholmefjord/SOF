using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expanding : MonoBehaviour {
    public float expandsize;
    float tempexpand;
    bool expanding = false;
	// Use this for initialization
	void Start () {
        tempexpand = 6.0f;
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(tempexpand);
        Debug.Log(expanding);
        if (tempexpand < 6.2&&expanding==true)
        {
            expanding = false; 
            tempexpand -= Time.deltaTime * expandsize;
        }
        else if(tempexpand<=6&&expanding==false)
        {
            expanding = true;
            tempexpand += Time.deltaTime * expandsize;
        }
        if(tempexpand>6.2)
        {
            expanding = false;
        }


        transform.localScale = new Vector3(tempexpand, tempexpand, 0);
    }
}
