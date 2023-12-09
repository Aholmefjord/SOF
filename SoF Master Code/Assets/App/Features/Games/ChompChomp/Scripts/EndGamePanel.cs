using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class EndGamePanel : MonoBehaviour {
    public static bool showCutscene = false;
	public GameObject background;
	public Text hungerText;
	public GameObject buddyAvatar;
	bool isPlaying = false;
	public GameObject table;
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!isPlaying)
		{
			if (PlayerPrefs.GetInt("FromChomp") == 1)
			{
				isPlaying = true;
				table.SetActive(false);
				buddyAvatar.transform.GetChild(0).GetComponent<Animator>().Play("LoveCycle_State");
				int hunger = (int)(PlayerPrefs.GetFloat("BuddyHunger") / 10);
				background.SetActive(true);
				hungerText.text = hunger.ToString() + "%";
                if (ChompBar.Camefromchomp == false) //to disable getting hunger percentage when in campaign mode
                {
                    PlayerPrefs.SetFloat("BuddyHunger", (PlayerPrefs.GetFloat("BuddyHunger") + 300));
                    if (PlayerPrefs.GetFloat("BuddyHunger") > 1000)
                        PlayerPrefs.SetFloat("BuddyHunger", 1000);
                    hungerText.enabled = true;
                }
                else
                {
                    hungerText.enabled = false;
                }
                    int afterHunger = (int)(PlayerPrefs.GetFloat("BuddyHunger") / 10);
				DOTween.Sequence()
					.AppendInterval(1f)
					.AppendCallback(() =>
					{
						DOTween.To(() => hunger, x => hunger = x, afterHunger, 1).OnUpdate(() => hungerText.text = hunger.ToString() + "%");
					})
					.AppendInterval(5f)
					.AppendCallback(() =>
                    {
                        background.SetActive(false);
                        PlayerPrefs.SetInt("FromChomp", 0);
                        if (showCutscene && Constants.uses_cinematics)
                        {
                            Debug.Log("Hasn't Seen Three");
                            showCutscene = false;
                            CutsceneDialogueController.isPartTwo = true;
                            MainNavigationController.GoToCinematic("Cinematics_chpt3");
                        }
                        else
                        {
                            //string chompKey = GameSceneManager.getInstance().GetCurrentLessonNumber() + "-" + GameSceneManager.getInstance().GetCurrentLessonPartNumber();
                            string chompKey = "ChompChompLevelKey";
                            PlayerPrefs.SetInt(chompKey, PlayerPrefs.GetInt(chompKey, 0) + 1);
                            Debug.Log(PlayerPrefs.GetInt(chompKey));
                            if (ChompBar.Camefromchomp==true)
                            {
                                //MainNavigationController.DoLoad("Chomp_Level");
                                MainNavigationController.DoAssetBundleLoadLevel(Constants.CHOMPCHOMP_SCENES, "Chomp_Level");
                                ChompBar.Camefromchomp = false;
                            }
                            else
                                MainNavigationController.GotoMainMenu();
                        }
					});
			}
		}
	}
}
