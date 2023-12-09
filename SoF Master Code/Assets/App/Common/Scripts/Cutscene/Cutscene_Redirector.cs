using UnityEngine;
using System.Collections;

public class Cutscene_Redirector : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Cutscene_Pet_Creation()
    {
        MainNavigationController.DoLoad("Cutscene_2");
    }
}
