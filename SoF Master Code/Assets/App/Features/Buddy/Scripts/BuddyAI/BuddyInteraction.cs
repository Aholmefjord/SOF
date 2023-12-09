using UnityEngine;
using System.Collections;
using System;

public class BuddyInteraction : MonoBehaviour {

	public GameObject speechBubble;
	bool shouldSFXCountdown;
	bool canPlaySFX = true;
	float sfxTimer;
	public Transform lookAtTarget;
	public string[] randomPlayAnimation;
	public AudioClip[] buddyClickSFX;
	public static bool clearPathway = false;
	public static bool isSpeechBubbleActive = false;
    public CutsceneDialogueController cdc;
    public GameObject blockerObject;

    float cameraOriginalFoV = 45.0f, cameraZoomedFoV = 40.0f;
    bool cameraZooming = false, cameraZoomedIn = false;
    // Use this for initialization
    void Start ()
	{
		try
		{
			if (lookAtTarget == null)
				lookAtTarget = GameObject.FindGameObjectWithTag("MainCamera").transform;
		}
		catch (Exception e)
		{
			lookAtTarget = GameObject.FindGameObjectWithTag("MainCamera").transform;
		}
	}

	void OnDestroy()
	{
		isSpeechBubbleActive = false;
		clearPathway = false;
	}

	// Update is called once per frame
	void Update ()
	{
		isSpeechBubbleActive = speechBubble.GetActive();
		sfxTimerCountDown();

        if (speechBubble.gameObject.GetActive())
        {
            if (!(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView <= cameraZoomedFoV))
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView -= ((cameraOriginalFoV - cameraZoomedFoV) * 0.2f);
            }
            else
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView = cameraZoomedFoV;
            }
            gameObject.transform.LookAt(lookAtTarget);
            WalkingAvatar.canWalk = false;

            transform.GetChild(0).GetComponent<Animator>().SetBool("Run", false);
            if (!WalkingAvatar.animationPlayed)
            {
                int i;
                if (PlayerPrefs.GetFloat("BuddyHunger") > 300) // if buddy hunger is more than 30%
                    i = UnityEngine.Random.Range(0, randomPlayAnimation.Length);

                else
                    i = UnityEngine.Random.Range(randomPlayAnimation.Length - 2, randomPlayAnimation.Length);

                transform.GetChild(0).GetComponent<Animator>().CrossFade(randomPlayAnimation[i], 0.15f);
                clearPathway = true;
                WalkingAvatar.animationPlayed = true;
                WalkingAvatar.runAnimationPlayed = false;
            }
        }
        else
        {
            if (!(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView >= cameraOriginalFoV))
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView += ((cameraOriginalFoV - cameraZoomedFoV) * 0.2f);
            }
            else
            {
                GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView = cameraOriginalFoV;
            }
        }

        if (Input.touchCount > 0)
		{
			if (Input.GetTouch(0).phase == TouchPhase.Began)
			{
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				// TILL HERE IT WORKS 
				if (Physics.Raycast(ray, out hit, Mathf.Infinity))
				{
					// BUT IT DOES NOT PRINT OUT HIT AND MORE 
					Debug.Log("HIT !");
					if (hit.transform == transform)
						Debug.Log("HIT Buddy!");
				}
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			// TILL HERE IT WORKS 
			if (Physics.Raycast(ray, out hit, Mathf.Infinity))
			{

				// BUT IT DOES NOT PRINT OUT HIT AND MORE 

				//Debug.Log("HIT !");
                //Debug.Log("HIT: " + hit.transform.name);
				if (hit.transform == transform)
				{
                    //Debug.Log("Hit this transform");
					if (WalkingAvatar.isClickable || CutsceneDialogueController.isHomeCinematic)
					{
                        CutsceneDialogueController.isHomeCinematic = false;
                        TriggerBuddyHungryAnimation();
					}
				}
			}
		}

	}
    public void TriggerBuddyHungryAnimation()
    {
        if (!blockerObject.activeInHierarchy) {
            if (canPlaySFX && speechBubble.gameObject.GetActive() == false && speechBubble != null)
            {
                canPlaySFX = false;
                SfXManager._instance.PlaySound(buddyClickSFX[UnityEngine.Random.Range(0, buddyClickSFX.Length)]);
                shouldSFXCountdown = true;
            }
            cameraZooming = true;
            speechBubble.gameObject.SetActive(!speechBubble.gameObject.GetActive());
            try
            {
                Debug.Log("TriggeredContinue");
                cdc.TriggerContinue();
            }
            catch (System.Exception e)
            {

            }
        }
    } 
    public void TriggerBuddyStomachGrowl()
    {
        int i = UnityEngine.Random.Range(randomPlayAnimation.Length - 2, randomPlayAnimation.Length);
        transform.GetChild(0).GetComponent<Animator>().CrossFade(randomPlayAnimation[i], 0.15f);
    }
	public void sfxTimerCountDown()
	{
		if (shouldSFXCountdown)
			sfxTimer += Time.deltaTime;
		if (sfxTimer >= 1.5f)
		{
			shouldSFXCountdown = false;
			canPlaySFX = true;
			sfxTimer = 0.0f;
		}
	}
}
