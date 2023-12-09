using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignLevelSelect : MonoBehaviour {
    private GameObject test;
    private GameObject udpTest;
    List<GameObject> horLayout = new List<GameObject>();
	// Use this for initialization
	void Start () {
        test = GameObject.Find("Stage Container");
        udpTest = GameObject.Find("UDP_Network_Singleton");
	}
	
	// Update is called once per frame
	void Update () {
        if (horLayout.Count < 50)
        {
            GameObject copy = Instantiate(test, test.transform.position, test.transform.rotation, test.transform.parent);
            horLayout.Add(copy);
        }
    }
}
