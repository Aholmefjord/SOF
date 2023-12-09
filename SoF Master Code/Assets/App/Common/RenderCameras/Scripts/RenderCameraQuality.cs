using UnityEngine;
using System.Collections;

public class RenderCameraQuality : MonoBehaviour {
    public Camera mainCam;
	// Use this for initialization
	void Start () {
        mainCam = Camera.main;
        transform.position = mainCam.transform.position;
        transform.rotation = mainCam.transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
