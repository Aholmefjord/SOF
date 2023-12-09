using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtlePlatformGenerator : MonoBehaviour {

    public GameObject platform;
    public Transform point;

    public float child;
    private float platformwidth;
    private float platformX;

	// Use this for initialization
	void Start () {
        platform = GameObject.FindGameObjectWithTag("GeneratePlatform");
        child = platform.GetComponent<Transform>().transform.GetChild(0).localScale.x;
        platformwidth = platform.GetComponentInChildren<BoxCollider>().size.x;
	}
	
	// Update is called once per frame
	void Update () {
        platform = GameObject.FindGameObjectWithTag("GeneratePlatform");
        
        if (transform.position.x < point.position.x)
        {
            transform.position = new Vector3(transform.position.x + 36.4f, transform.position.y + 18.6f, transform.position.z);
            //transform.eulerAngles = new Vector3(0, 50, 0);
            GameObject newPlatform = (GameObject)Instantiate(platform, transform.position, transform.rotation);
            newPlatform.name = platform.name;
        }
	}
}
