using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class IconFade : MonoBehaviour {
    public HomeCamera cam;
    public Button button1;
    public Button button2;
    public Button button3;
    public Button button4;
    public Button button5;

    public GameObject objectTarget;
    public float modifierTime;
    float opacity;

	// Use this for initialization
	void Start () {
	    
	}

    // Update is called once per frame

    void Update () {

        float target = (cam.stateMachine.selectedState.name == "Room") ? 0.5f : 1.0f;
        //Debug.Log("Target Opacity: " + target);
        //        opacity = Mathf.Lerp(opacity, target, Time.deltaTime * modifierTime);
        //        ColorBlock blah = button1.colors; 
        //        blah.normalColor = new Color(1, 1, 1, opacity);
        button1.interactable = cam.stateMachine.selectedState.name == "Room";
        button2.interactable = cam.stateMachine.selectedState.name == "Room";
        button3.interactable = cam.stateMachine.selectedState.name == "Room";
        button4.interactable = cam.stateMachine.selectedState.name == "Room";
        button5.interactable = cam.stateMachine.selectedState.name == "Room";

        //        button1.GetComponent<Image>().CrossFadeAlpha(target,opacity;//= new Color(0,0,0,1);//new Color(255.0f, 255.0f, 255.0f, 255.0f* opacity);
        //       button2.GetComponent<Image>().color;// = new Color(0, 0, 0, 1);//new Color(255.0f, 255.0f, 255.0f, 255.0f * opacity);
        //      button3.GetComponent<Image>().color;// = new Color(0, 0, 0, 1);//new Color(255.0f, 255.0f, 255.0f, 255.0f * opacity);
        //     button4.GetComponent<Image>().color;// = new Color(0, 0, 0, 1);//new Color(255.0f, 255.0f, 255.0f, 255.0f * opacity);
    }
}
