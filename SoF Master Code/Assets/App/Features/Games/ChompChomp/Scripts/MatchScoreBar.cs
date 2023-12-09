using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MatchScoreBar : MonoBehaviour {
	public GameObject existingBar ;
    public bool isGame6 = false;
	[HideInInspector] public MatchGameManager gm {get{return JMFUtils.gm;}}
	private float initialScore; 
	public Transform startPoint;
	public Transform endPoint;
	public GameObject thirdStar;
	public GameObject secStar;
	public GameObject firstStar;
	public Sprite fullStarSprite;
	private float maxLength;
	private float existingBarInitialXLength;
    public GameObject inGameMenu;
	static float scoreReductionMultiplier = 0.011574f;
    public int winningScore = 0;
    public WinTextPopUp wintextpopup;
    //static float scoreReductionMultiplier = 5;

       //to check if the game is supposed to be ended.
    private bool isGameEnded = false;
    //we want to have a delay before the game is officially ended.
    //because the transition of winning is too suddent
    public float winningCountdown = 3.0f;
    private float currentWinningCountdown = 0.0f;
    
	public static float ScoreReductionMultiplier()
	{
		return scoreReductionMultiplier;
	}

    public bool getIsGameEnded()
    {
        return isGameEnded;
    }

	void OnDestroy()
	{
		PlayerPrefs.SetString("exitTime", System.DateTime.Now.ToBinary().ToString());
		//float buddyHunger = PlayerPrefs.GetFloat("BuddyHunger", 0);
		//PlayerPrefs.SetFloat("BuddyHunger", buddyHunger += 200); // increase buddy hunger by 20%

		//if (PlayerPrefs.GetFloat("BuddyHunger") > 1000)
			//PlayerPrefs.SetFloat("BuddyHunger", 1000);
	}

	// Use this for initialization
	void Start ()
	{
		maxLength = endPoint.position.y - startPoint.position.y;
		Debug.Log ("match3 initializing winning");
		initialScore = 0;
		existingBarInitialXLength = existingBar.transform.localScale.x;
		existingBar.transform.localScale = new Vector3 (0.0f, existingBar.transform.localScale.y, existingBar.transform.localScale.z);
		Vector3 tempStarPos = thirdStar.transform.position;
		tempStarPos.y = startPoint.position.y+maxLength;
		thirdStar.transform.position = tempStarPos;
		tempStarPos.y = startPoint.position.y+(maxLength*((float)gm.GetComponent<WinningConditions>().scoreMilestone2/(float)gm.GetComponent<WinningConditions>().scoreMilestone3));
		secStar.transform.position = tempStarPos;
		tempStarPos.y = startPoint.position.y+(maxLength*((float)gm.GetComponent<WinningConditions>().scoreToReach/(float)gm.GetComponent<WinningConditions>().scoreMilestone3));
		firstStar.transform.position = tempStarPos;
        //initialScore = PlayerPrefs.GetFloat("BuddyHunger", 0);
        gm.score = initialScore;

        wintextpopup = GameObject.Find("WinPanel").GetComponent<WinTextPopUp>();
    }

    bool hasLoadedMap = false;
    // Update is called once per frame
    void Update()
	{
	//	if (gm.score > 0)
		//{
		//	gm.score -= Time.deltaTime * 2;
		//}
		//else
        if( gm.score <=0)
			gm.score = 0;

		if (initialScore != gm.score) {
            initialScore = gm.score;
            //PlayerPrefs.SetFloat("BuddyHunger", initialScore);
            updateScoreBar();
            updateStars();
            if(gm.score >= winningScore && !isGameEnded)
            {
                isGameEnded = true;
                //just in case.
                currentWinningCountdown = 0.0f;	
            }
        }

        if (isGameEnded && currentWinningCountdown >= winningCountdown)
        {
            /*
            if (!hasLoadedMap)
            {
                //winning animation is needed
                hasLoadedMap = true;
                PlayerPrefs.SetInt("FromChomp", 1);
                MainNavigationController.GoToMap();
            }
            */

            gm.comboScript.resetComboAnim();
            wintextpopup.StartCoroutine("startAnimation");
        }
        else if(isGameEnded)
        {
            currentWinningCountdown += Time.deltaTime;
        }

        if (!inGameMenu.GetActive())
        {
            inGameMenu.SetActive(true);
        }

	}
	void updateScoreBar(){
        
		if ((float)initialScore / (float)gm.GetComponent<WinningConditions>().scoreMilestone3 >= 1) {
			existingBar.transform.DOScale(new Vector3 (existingBarInitialXLength, 
			                                               existingBar.transform.localScale.y, 
			                                               existingBar.transform.localScale.z), 0.3f).SetEase(Ease.OutSine);
		} else {
			existingBar.transform.DOScale(new Vector3 (((float)initialScore / (float)gm.GetComponent<WinningConditions>().scoreMilestone3) * existingBarInitialXLength, 
											existingBar.transform.localScale.y, 
											existingBar.transform.localScale.z), 0.3f).SetEase(Ease.OutSine);
        }
	}
	void updateStars(){
		if ((float)initialScore >= (float)gm.GetComponent<WinningConditions> ().scoreToReach) {
            //Debug.Log("Got First Star");
			firstStar.GetComponent<SpriteRenderer>().sprite = fullStarSprite;
		}
		if ((float)initialScore >= (float)gm.GetComponent<WinningConditions> ().scoreMilestone2) {
            //Debug.Log("Got Second Star");
			secStar.GetComponent<SpriteRenderer>().sprite = fullStarSprite;
		}
		if ((float)initialScore >= (float)gm.GetComponent<WinningConditions> ().scoreMilestone3) {
            //Debug.Log("Got Third Star");
			thirdStar.GetComponent<SpriteRenderer>().sprite = fullStarSprite;
      //      if (isGame6)
    //        {
  //              tde.animate = true;
//            }
		}
	}
}
