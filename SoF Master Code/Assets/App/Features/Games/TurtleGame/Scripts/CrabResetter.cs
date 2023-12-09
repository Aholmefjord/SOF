using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class CrabResetter : MonoBehaviour {
    public bool button1;
    public bool button2;
    public bool button3;
    public bool button4;
	// Use this for initialization
	void Start () {
	
	}
    public ButtonAnimatorController[] buttonControllers;
    // Update is called once per frame
    void Update () {
	    if(button1 && button2 && button3 && button4)
        {
            button1 = button2 = button3 = button4 = false;
            Debug.Log("Resetting Game");
            GameObject.FindGameObjectWithTag("CrabGameController").GetComponent<InfiniteCrabBrothersController>().SelectNextPuzzle();
        }
	}
    public void ResetButtons() {
        for (int i = 0; i < buttonControllers.Length; i++)
        {
            buttonControllers[i].ResetAnimator();
        }
    }
}
