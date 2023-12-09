using UnityEngine;
using System.Collections;


/// <summary> ##################################
/// 
/// NOTICE :
/// This script is the win text + animation
/// 
/// DO NOT TOUCH UNLESS REQUIRED
/// 
/// </summary> ##################################


public class WinTextPopUp : MonoBehaviour {


    private Vector3 oriScale, newSize;
    public GameObject wintext;
    public GameObject background;

    private bool finished;

    public bool isFinished()
    {
        return finished;
    }

    // Use this for initialization
    void Start()
    {
        finished = false;

        MultiLanguage.getInstance().apply(wintext, "chomp_chomp_win_text");

        oriScale = wintext.transform.localScale;
        wintext.transform.localScale = Vector3.zero; // initialy not shown
        newSize = Vector3.Scale(oriScale, new Vector3(1.5f, 1.5f, 1.5f));

        background.SetActive(false);
    }

    // called by GameManager script
    public IEnumerator startAnimation()
    {
        if (!finished)
        {
            AudioSystem.stop_bgm();
            //SFX length 1.4
            AudioSystem.PlaySFX("buttons/JULES_MANTA_COINS_03");
            //we are done here
            finished = true;
            
            background.SetActive(true);

            wintext.transform.localScale = Vector3.zero; // start from nothing
            
            // animate it (makes it pop-out big)
            LeanTween.scale(wintext, newSize, 0.3f).setEase(LeanTweenType.easeOutElastic);
            yield return new WaitForSeconds(0.3f);
            LeanTween.scale(wintext, oriScale, 0.15f).setEase(LeanTweenType.easeInElastic);

            yield return new WaitForSeconds(1.3f);

            //move out
            LeanTween.scale(wintext, Vector3.zero, 0.1f).setEase(LeanTweenType.easeOutElastic);

            //exit
            PlayerPrefs.SetInt("FromChomp", 1);
            //MainNavigationController.GoToMap();
        }
    }
}