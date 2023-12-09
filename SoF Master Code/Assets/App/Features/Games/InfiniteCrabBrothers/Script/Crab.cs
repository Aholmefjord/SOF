using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using JULESTech.Resources;

public class Crab : MonoBehaviour {
    public const int BODYPARTS = 3;//5;

    public Image body;
    public Image leftClaw;
    public Image rightClaw;
    public Image leftLeg;
    public Image rightLeg;
    public Image bucket;
    public Image Shadow;
    public Text questionMark1, questionMark2;
    public Text numberLabel;
    public GameObject dustCloud;
    public int color;
    public int pose;
    public bool isHidden;
    Animator animator;
    public bool TriggerPose;
    public bool isReady;
    // Use this for initialization
    private bool CrabReset;// used for reset later on
    
    public void InitCrab(int color, int pose,bool isHidden)
    {
        Debug.Log("CRAB AT " + gameObject.name + " SET IS: [" + color + " , " + pose + "]" + " : ");

        this.color = color;
        this.pose = pose;
        this.isHidden = isHidden;
        CrabResources.UpdateCrabSprites(this, isHidden);
        animator = GetComponent<Animator>();
        animator.SetInteger("pose",pose);
        CrabReset = true;
    }
    public void DisableCrab()
    {
        this.gameObject.SetActive(false);
    }
	void Start () {

        animator = GetComponent<Animator>();
 //       InitCrab(color, pose, isHidden);
	}
    public void AmReady()
    {
        isReady = true;
    }
    public void StartPose()
    {
        isReady = false;
        try
        {
            animator.SetTrigger("BeginPose");
        }catch(System.Exception e)
        {

        }
        
    }
    // Update is called once per frame
    private void Update()
    {
        if (TriggerPose)
        {
            TriggerPose = false;

        }
        if (leftClaw.transform.localPosition.z != 0)
            leftClaw.transform.localPosition = new Vector3(leftClaw.transform.localPosition.x, leftClaw.transform.localPosition.y, 0);
        if(rightClaw.transform.localPosition.z != 0)
            rightClaw.transform.localPosition = new Vector3(rightClaw.transform.localPosition.x, rightClaw.transform.localPosition.y, 0);

        //Temp fix for disappearing crab - Clinton
        if (CrabReset == true)
        {
            Reset();
            Debug.Log("resetting");
            CrabReset = false;
        }
    }
    private void IncorrectGuess()
    {
        Guess(false);
    }
    private void CorrectGuess()
    {
        Guess(true);
    }
    private void Guess(bool guessed)
    {
        if (!guessed)
        {
            animator.SetTrigger("BeginPose");
        }
        else
        {

        }
    }
    public void MakeGuess(bool make_a_guess)
    {
        if (make_a_guess)
        {
            animator.SetTrigger("Correct");
        }else
        {
            animator.SetTrigger("InCorrect");
        }
    }

    public void ClearReset()
    {
        animator.ResetTrigger("Restart");
    }
    public void SelectAnswer()
    {
        if (!GameObject.FindGameObjectWithTag("CrabGameController").GetComponent<InfiniteCrabBrothersController>().SelectAnswer(this))
        {
            transform.parent.parent.parent.GetComponent<Animator>().SetTrigger("PressedDown");
        }

        transform.parent.parent.parent.GetComponent<Button>().interactable = false;
    }
    public void Reset()
    {
        animator.SetTrigger("Restart");
    }

    public bool Equals(Crab b)
    {
        return (b.color == this.color && b.pose == this.pose);
    }

    public void RepositionQuestionMark()
    {
        try
        {
            questionMark1.gameObject.transform.localPosition = new Vector3(Random.Range(bucket.transform.position.x - 15, bucket.transform.position.x + 60), Random.Range(bucket.transform.position.y - 45, bucket.transform.position.y + 15), 0);
            questionMark2.gameObject.transform.localPosition = new Vector3(Random.Range(bucket.transform.position.x - 15, bucket.transform.position.x + 60), Random.Range(bucket.transform.position.y - 45, bucket.transform.position.y + 15), 0);
        }catch(System.Exception e)
        {

        }
    }

    public void ToggleRigidBody(int awake)
    {
        if (awake == 0)
            gameObject.GetComponentInChildren<Rigidbody2D>().WakeUp();
        else if (awake == 1)
            gameObject.GetComponentInChildren<Rigidbody2D>().Sleep();
    }
}
public static class CrabResources
{
    // the lists should be as follows,
    /*
     * 
     *  0 - body 
     *  1 - claw 
     *  2 - leg
     */
    public static List<Sprite> oneSprites;
    public static List<Sprite> twoSprites;
    public static List<Sprite> threeSprites;
    public static List<Sprite> fourSprites;
    public static Sprite bucketSprite;
    //static string onePre = "Crab/crab01_";
    //static string twoPre = "Crab/crab02_";
    //static string threePre = "Crab/crab03_";
    //static string fourPre = "Crab/crab04_";
    private static int initialized = 0; // 0==not init, 1==init-ing, 2==done

    public static void InitResources() {
        if (initialized > 0) {
            return;
        }

        oneSprites = new List<Sprite>();
        twoSprites = new List<Sprite>();
        threeSprites = new List<Sprite>();
        fourSprites = new List<Sprite>();
        
        List<List<Sprite>> containers = new List<List<Sprite>>();
        containers.Add(oneSprites);
        containers.Add(twoSprites);
        containers.Add(threeSprites);
        containers.Add(fourSprites);
        
        for (int j = 0; j < containers.Count; ++j) {
            for (int i = 0; i < Crab.BODYPARTS; i++) {
                AssetBundleManager.LoadAsset(Constants.CRAB_BROS_SHARED, string.Format("crab0{0}_{1}", j+1, i), (loadedAsset) => {
                    Sprite temp = AssetBundleManager.GetSprite(loadedAsset);
                    containers[j].Add(temp);
                });
            }        
        }
        AssetBundleManager.LoadAsset(Constants.CRAB_BROS_SHARED, "bucket", (loadedAsset) => {
            bucketSprite = loadedAsset as Sprite;
        });
        initialized = 2;
    }
    public static IEnumerator InitResourcesAsync()
    {
        initialized++;
        oneSprites = new List<Sprite>();
        twoSprites = new List<Sprite>();
        threeSprites = new List<Sprite>();
        fourSprites = new List<Sprite>();

        List<List<Sprite>> containers = new List<List<Sprite>>();
        containers.Add(oneSprites);
        containers.Add(twoSprites);
        containers.Add(threeSprites);
        containers.Add(fourSprites);

        for (int j = 0; j < containers.Count; ++j) {
            for (int i = 0; i < Crab.BODYPARTS; i++) { 
                yield return AssetBundleManager.LoadAsset(Constants.CRAB_BROS_SHARED, string.Format("crab0{0}_{1}", j + 1, i), (loadedAsset) => {
                    //Debug.Log("crab piece loaded: "+loadedAsset);
                    Sprite temp = AssetBundleManager.GetSprite(loadedAsset);
                    containers[j].Add(temp);
                    //Debug.LogFormat("crab piece loaded: {0} - {1}",loadedAsset, temp);
                });
            }
        }
        yield return AssetBundleManager.LoadAsset(Constants.CRAB_BROS_SHARED, "bucket", (loadedAsset) => {
            bucketSprite = loadedAsset as Sprite;
        });
        Debug.Log("crabresources all loaded");
        initialized++;
    }

    public static void UpdateCrabSprites(Crab b, bool isHidden)
    {
        List<Sprite> spritesToSet = getCrabSprites(b.color);
        try { 
            b.body.enabled = !isHidden;
            b.leftClaw.enabled = !isHidden;
            b.leftLeg.enabled = !isHidden;
            b.rightClaw.enabled = !isHidden;
            b.rightLeg.enabled = !isHidden;
            if(b.Shadow != null)
                b.Shadow.enabled = !isHidden;
            if(b.bucket != null)
                b.bucket.enabled = isHidden;
            if(b.questionMark1 != null)
                b.questionMark1.enabled = isHidden;
            if(b.questionMark2 != null)
                b.questionMark2.enabled = isHidden;
            if (b.dustCloud != null)
                b.dustCloud.SetActive(!isHidden);

            if (!isHidden)
            {
                b.body.sprite = spritesToSet[0];
                b.leftClaw.sprite = spritesToSet[1];
                b.leftLeg.sprite = spritesToSet[2];
                b.rightClaw.sprite = spritesToSet[1];
                b.rightLeg.sprite = spritesToSet[2];
            }
        }catch(System.Exception e) {
            Debug.LogError(e);
            Debug.LogError(b.color);
            Debug.LogError(spritesToSet.Count);
            Debug.LogError(spritesToSet[0]);
            Debug.LogError(spritesToSet[1]);
            Debug.LogError(spritesToSet[2]);
        }
    }
    public static List<Sprite> getCrabSprites(int color)
    {
        switch (color-1)
        {
            case 0:
                return oneSprites;
            case 1:
                return twoSprites;
            case 2:
                return threeSprites;
            case 3:
                return fourSprites;
        }
        return null;
    }
}