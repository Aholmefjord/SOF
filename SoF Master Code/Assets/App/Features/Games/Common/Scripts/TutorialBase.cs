using UnityEngine;

public class TutorialBase: MonoBehaviour
{
	public delegate void EndTutorialAction();
	public static event EndTutorialAction OnTutorialEnd;

	// References to directional buttons
	public GameObject UpBtn;
	public GameObject DownBtn;
	public GameObject LeftBtn;
	public GameObject RightBtn;
	public GameObject JuncBtn;
	public GameObject playBtn;
	public GameObject resetBtn;
	public GameObject Queue;
	public GameObject Overlay;

    public GameObject MenuHide;
    public GameObject MenuExpand;
    public GameObject Build;
    public GameObject Pet;
    public GameObject Video;
    public GameObject Invite;
    public GameObject Join;
    public GameObject Diary;
    public GameObject MapButton;
	public GameObject DoodleButton;

    public void Init()
	{
		// Find the directional Buttons
		UpBtn = GameObject.Find("UpCommandButton");
		DownBtn = GameObject.Find("DownCommandButton");
		LeftBtn = GameObject.Find("LeftCommandButton");
		RightBtn = GameObject.Find("RightCommandButton");
		JuncBtn = GameObject.Find("JunctionCommandButton");
		playBtn = GameObject.Find("PlayButton");
		resetBtn = GameObject.Find("ResetButton");
		Queue = GameObject.Find("QueuePanel");
		Overlay = GameObject.Find("Overlay");

        // House tutorial 
        MenuHide = GameObject.Find("Menu Hide Button");
        MenuExpand = GameObject.Find("Menu Expand Button");
        Build = GameObject.Find("BuildButton");
        Pet = GameObject.Find("Pet Button");
        Video = GameObject.Find("Video Button");
        Invite = GameObject.Find("Invite Button");
        Join = GameObject.Find("Join Button");
        Diary = GameObject.Find("Diary Button");
        MapButton = GameObject.Find("Map Button");
		DoodleButton = GameObject.Find ("DoodlePanel");
    }

	protected void callEnd()
	{
		if (OnTutorialEnd != null)
			OnTutorialEnd();
	}
}
