using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using JULESTech;

// TODO: -
// To move the camera around the house
public class HomeCamera : MonoBehaviour
{
    // Singleton Instance
    public static bool playCutscene = false;
	public static HomeCamera Instance { get; private set; }		// Ensure only one of this class, and make it globally accessible
    [Header("CameraStateMachine")]
    public CameraStateMachine stateMachine;
    // Public Variables
    [Header("Objects to Manipulate")]
	public GameObject camera;									// Store the Main Camera from the scene to manipulate
	public GameObject screen;									// Store the Giant Screen from the scene to manipulate
	public GameObject table;									// Store the Table from the scene to manipulate
	public GameObject wall;										// Store the Wall from the scene to manipulate
	public GameObject buildText;								// Store the Build Text from the scene to manipulate
	public GameObject buildPanel;								// Store the Build Panel from the scene to manipulate
	public GameObject buildGrid;                                // Store the Grid from the scene to manipulate
	public GameObject buddy;
    public GameObject buddyName;
    public GameObject attentionText;

	[Header("Slerp Speed")]
	public int moveSpeed;										// Set the move speed of the camera during zooming in and out
	public int turnSpeed;										// Set the turn speed of the camera during rotation

	[Header("Camera Settings")]
	[Tooltip("Value is in float")]
	public float distanceFromStopping;                          // Set when the camera should stop from the target position

    [Header("Table Buttons Container")]
    public GameObject buttonsContainer;                         // Store the Table Buttons

    [Header("Rotation Stuff")]
	public GameObject rotateButtons;							// Store the Rotate Buttons for the furniture's rotation
	public GameObject selected;									// Store the currently selected Furniture

	[Header("Build Variables")]
	public bool building = false;								// Store the checking of whether if we are in Building mode

	// Private Variables
	private GameObject lightBulb;								// Store the GameObject Light Bulb
	private GameObject buildPos;								// Store the Build Position to change to during build mode

	private Vector3 originalPosition;							// Store the camera's original position at the start of the scene
	private Quaternion originalRotation;						// Store the camera's original rotation at the start of the scene
	private Vector3 targetPosition;								// Set the camera's target position to move to
	private Quaternion targetRotation;							// Set the camera's target rotation to rotate to

	private bool moving = false;                                // Store if the camera is currently moving or not
	public bool alreadyHidden { get; set; }
    public bool moveDisabled = false;

    private bool canAccessScreen = true;

	private Button[] buttons;                                   // Store the number of buttons for the furniture

	public NewMapController NMC;

	public GameObject[] interactableButtons;

	public GameObject speechBubble;
	public CanvasGroup videoDoodleCG;

    public BuddyInteraction BIController;

    // Use this for initialization
    void Start()
    {
        originalPosition = camera.transform.position;
        originalRotation = camera.transform.rotation;
        lightBulb = GameObject.Find("Light Bulb");
        buildPos = GameObject.Find("Build Camera Position");

        if (Constants.mainMenuScene.Equals("mapNew"))
        {
            interactableButtons[interactableButtons.Length - 1] = null;
        }

        interactableButtons[interactableButtons.Length - 2] = null;

    }
    public void LateUpdate()
    {
    //    if (playCutscene)
   //     {
    //        playCutscene = false;
     //       transform.root.GetComponent<Animator>().enabled = true;
      //  }
    }
	public void HideUI()
	{
		
		if (alreadyHidden)
		{
			for (int i = 0; i < interactableButtons.Length; i++)
			{
                if (interactableButtons[i] == null)
                    continue;
				interactableButtons[i].SetActive(true);
			}
			alreadyHidden = false;

            interactableButtons[6].SetActive(false);//temp code to disable buddy chat
        }
		else
		{
			for (int i = 0; i < interactableButtons.Length; i++)
            {
                if (interactableButtons[i] == null)
                    continue;
                interactableButtons[i].SetActive(false);
			}
			alreadyHidden = true;
		}
	}

	// Before Start()
	void Awake()
	{
		// For Singleton instance control
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(this);
		}

		buildGrid.SetActive(false);

        //buttons = buttonsContainer.GetComponentsInChildren<Button>();
		SetTableInteractability(false);
	}

	// To be called if destroyed when scene is changed (Which is hopefully never when the scene is running)
	void OnDestroy()
	{
		if (Instance == this)
		{
			Instance = null;
		}
	}
    public void SelectNewState(string s)
    {
        if (s == "Table" && !GameState.playerBuddy.hasClaimedExpedition) return;
		if (moveDisabled)
			return;
        if (s == "Table")
        {
            AudioSystem.PlaySFX("buttons/JULES_WHOOSH_IN_03");
            AnalyticsSys.Instance.Report(REPORTING_TYPE.ClickButton, "GameTable");
        }
        moving = true;
		//Debug.LogError("ok");
        
        stateMachine.SelectNewState(s);
    }

    public void enableScreenAccess()
    {
        canAccessScreen = true;
    }

    public void disableScreenAccess()
    {
        canAccessScreen = false;
    }

    public Button backButton;

	public void invertMoveDisabled()
	{
		moveDisabled = !moveDisabled;
	}
    
    public void SelectScreen()
    {
        if (!canAccessScreen)
        {
            Debug.Log("HomeCamera.canAccessScreen = false");
            return;
        }

        if (stateMachine.selectedState.name != "Screen")
        {
            //Debug.LogError("SCREEN SFX");
            AudioSystem.PlaySFX("buttons/JULES_ZOOMIN_WHOOSH_02");
            if (buddy.GetActive())
                buddy.SetActive(false);
        }
        moving = true;
        stateMachine.SelectNewState("Screen");
        AnalyticsSys.Instance.Report(REPORTING_TYPE.ClickButton, "SmartScreen");
    }
    public void GoToKraken()
    {
        MainNavigationController.GoToScene("");
    }
	// Update is called once per frame 
	void Update()
	{
        if (buddyName.GetComponent<Text>().text != GameState.playerBuddy.name + MultiLanguage.getInstance().getString("buddy_home_room"))
        {
            MultiLanguage.getInstance().apply(buddyName, "buddy_home_room");
            buddyName.SetText(GameState.playerBuddy.name + MultiLanguage.getInstance().getString("buddy_home_room"));

            if (attentionText != null)
            {
                MultiLanguage.getInstance().apply(attentionText, "buddy_home_freeze_message");
            }
        }

        if (stateMachine.selectedState.name == "Screen")
			videoDoodleCG.blocksRaycasts = true;
		else
			videoDoodleCG.blocksRaycasts = false;

		try { 
		if (speechBubble.GetActive() && stateMachine.selectedState.name != "Room")
			speechBubble.SetActive(false);
        }catch(System.Exception e) { }

		BuddyClickableIfNotClickable();
		DisableTableClick();

		if (stateMachine.selectedState.name == "Room" && backButton.IsActive())
			backButton.gameObject.SetActive(false);

		else if (stateMachine.selectedState.name != "Room" && !backButton.IsActive())
		{
			backButton.gameObject.SetActive(true);
			HideUI();
		}
		// For handling the movement to the Table and the Giant Screen
		if (Input.GetMouseButtonDown(0))
		{
			if (!moveDisabled)
			{
				if (!building)
				{
					if (!moving)
					{
						if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
						{

							RaycastHit hit;
							Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
							if (Physics.Raycast(ray, out hit))
							{
								if (hit.collider != null)
								{
                                    if(hit.collider.gameObject.tag == "Avatar")
                                    {
                                        // TOM: not sure who wrote this, but the implementation was not committed; came from carpe diem branch
                                        //BIController.BuddyOnClick();
                                    }
									// If player clicked on the Giant Screen
									else if (hit.collider.gameObject.name == "Giant Screen")
									{

                                        if (stateMachine.selectedState.name != "Screen")
										{
											Debug.LogError("SCREEN SFX");
											AudioSystem.PlaySFX("buttons/JULES_ZOOMIN_WHOOSH_02");
											if (buddy.GetActive())
												buddy.SetActive(false);
										}
										moving = true;
										stateMachine.SelectNewState("Screen");
									}

									// If player clicked on the Table
									else if (hit.collider.gameObject.name == "Table")
									{
                                        if (stateMachine.selectedState.name != "Table")
										{
											Debug.Log("TABLE SFX");
											AudioSystem.PlaySFX("buttons/JULES_WHOOSH_IN_03");
										}
										if (GameState.playerBuddy.hasClaimedExpedition) // Make table unclickable when buddy on expedition
										{
											//if (GameState.playerBuddy.hasClaimedExpedition)
											//	HideUI();
											moving = true;
											//SetTableInteractability(true);
											stateMachine.SelectNewState("Table");
                                            
                                        }
									}
									else if (hit.collider.gameObject.name == "Room")
									{
										Debug.Log("Room? CLICKED");
										moving = true;
										//SetTableInteractability(true);
										stateMachine.SelectNewState("Room");
									}
									else if (hit.collider.gameObject.name == "Sally")
									{
										Debug.Log("Sally Click");
										moving = true;
										stateMachine.SelectNewState("Sally");
									}
								}
							}
						}
					}
				}
			}
		}

		// Slerp the Camera towards the Target
		if (moving)
		{
            targetPosition = stateMachine.selectedState.targetTransform.position;
            targetRotation = stateMachine.selectedState.targetTransform.rotation;
			//Quaternion.LookRotation(-(camera.transform.position - stateMachine.selectedState.targetTransform.position));
			camera.transform.position = Vector3.Slerp(camera.transform.position, targetPosition, Time.deltaTime * moveSpeed);
			camera.transform.rotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, Time.deltaTime * turnSpeed);

			if (Vector3.Distance(camera.transform.position, targetPosition) < distanceFromStopping)
			{
				moving = false;
			}
		}

		// Move the Rotate Buttons to be beside the currently selected Furniture
		if (selected != null)
		{
			if (rotateButtons.activeSelf)
			{
				rotateButtons.transform.position = camera.GetComponent<Camera>().WorldToScreenPoint(selected.transform.position) + new Vector3(50f, 0f, 0f);
			}
		}
	}

	void ShowIfNotShown()
	{
		for (int i = 0; i < interactableButtons.Length; i++)
		{
            if (interactableButtons[i] == null)
                continue;

			if (interactableButtons[i].GetActive() == false)
			{
				interactableButtons[i].SetActive(true);
			}
		}
        interactableButtons[6].SetActive(false);//temp code to disable buddy chat
    }


	WalkingAvatar wa;
	void BuddyClickableIfNotClickable()
	{
		if (wa == null)
			wa = buddy.GetComponent<WalkingAvatar>();
		if (stateMachine.selectedState.name == "Table")
		{
			if (WalkingAvatar.isClickable == true)
				WalkingAvatar.isClickable = false;
		}
		if (stateMachine.selectedState.name == "Screen")
		{
			if (WalkingAvatar.isClickable == true)
				WalkingAvatar.isClickable = false;
		}
	}

	// OnClick Function for the "Back" Button
	public void BackToOriginalTransform()
	{
		ShowIfNotShown();
		if (!buddy.GetActive())
			buddy.SetActive(true);
		//if (!moving)
	    //{
			//Debug.Log("Returning");
			moving = true;
			//targetPosition = originalPosition;
		    //targetRotation = originalRotation;

			building = false;

        //lightBulb.transform.GetChild(0).gameObject.SetActive(true);
        //lightBulb.transform.GetChild(1).gameObject.SetActive(true);

            enableScreenAccess();

            switch (stateMachine.selectedState.name)
            {
                case "Table":
                    stateMachine.SelectNewState("Room");
                    break;
                case "Build":
                    screen.SetActive(true);
                    wall.SetActive(true);
                    buildPanel.SetActive(false);
                    buildGrid.SetActive(false);
					//camera.GetComponent<Camera>().orthographic = false;
                    //camera.GetComponent<Camera>().fieldOfView = 45;
                    stateMachine.SelectNewState("Room");
                    break;
                case "Screen":
                    stateMachine.SelectNewState("Room");
					Debug.LogError("OFF THE SCREEN");
                    break;
            }

			//camera.GetComponent<Camera>().orthographic = false;
			//camera.GetComponent<Camera>().fieldOfView = 60;

			//screen.SetActive(true);
			//wall.SetActive(true);
			//buildPanel.SetActive(false);
			//buildGrid.SetActive(false);

			//SetTableInteractability(false);
		//}
	}

	public Button tableButton;

	public void DisableTableClick()
	{
		if (stateMachine.selectedState.name == "Room")
		{
			tableButton.interactable = true;
		}
		else
			tableButton.interactable = false; 

	}

	// OnClick Function for the "Build" Button
	public void Build()
	{
		if (!moving)
		{
            //Debug.Log("Building");
            moving = true;
            stateMachine.SelectNewState("Build");
			//camera.GetComponent<Camera>().orthographic = true;
			//camera.GetComponent<Camera>().orthographicSize = 8.47f;

            building = true;

			//lightBulb.transform.GetChild(0).gameObject.SetActive(false);
			//lightBulb.transform.GetChild(1).gameObject.SetActive(false);

            screen.SetActive(false);
            wall.SetActive(false);
  //          buildPanel.SetActive(true);
//            buildGrid.SetActive(true);
        }
	}

	// Function to set the interactable value of the buttons on the table
	private void SetTableInteractability(bool interact)
	{
		if (buttons != null)
		{
			for (int i = 0; i < buttons.Length - 1; i++)
			{
				if (interact == true)
				{
					// Make it interactable only if user already unlocked the game
					buttons[i].interactable = buttons[i].gameObject.GetComponent<CheckUnlocked>().unlocked;
				}
				else
				{
					// Make all buttons un-interactable
					buttons[i].interactable = false;
				}
			}

			// Setting the Unlock button's interactability
			buttons[buttons.Length - 1].interactable = interact;
		}
	}

	// OnClick function for rotating this GameObject clockwise
	public void RotateClockwise()
	{
		selected.GetComponent<DragNSnap>().Rotate(true);
	}

	// OnClick function for rotating this GameObject counters-clockwise
	public void RotateCounterClockwise()
	{
		selected.GetComponent<DragNSnap>().Rotate(false);
	}
}

[System.Serializable]
public class CameraStateMachine
{
    public List<CameraState> cameraStates;
    public CameraState selectedState;

    public void SelectNewState(string name)
    {
        for(int i = 0; i < cameraStates.Count; i++)
        {
			if (cameraStates[i].name == name)
			{
				selectedState = cameraStates[i];
			}
        }
    }
}

[System.Serializable]
public class CameraState
{
    public string name;
    public Transform targetTransform;
}
