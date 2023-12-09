using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChatBoxBubble : MonoBehaviour
{
	public Transform target;
	public Transform lookAtTarget;
	float x;
	// Use this for initialization
	void Start ()
	{
        SetupUI();
	}

    private void SetupUI()
    {
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("Button").GetComponent<Image>(), "chomp_title_image");
    }

	// Update is called once per frame
	void Update ()
	{
		if (!GameState.playerBuddy.hasClaimedExpedition)
			gameObject.SetActive(false);
		x = (target.position.x + 5f);
		transform.position = new Vector3(x, transform.position.y, target.position.z);
        /*
		if (PlayerPrefs.GetFloat("BuddyHunger", 0) != 0)
			Debug.Log("HUNGER BAR: " + PlayerPrefs.GetFloat("BuddyHunger", 0));
        //*/
		//gameObject.transform.LookAt(lookAtTarget);
	}

	public void GoToChompChomp()
	{
		//MainNavigationController.GoToScene("game3");
        MainNavigationController.DoAssetBundleLoadLevel(Constants.CHOMPCHOMP_SCENES, "game3");
    }
	public void HideCanvas()
	{
		if (gameObject.GetActive() == true)
			gameObject.SetActive(false);
	}
}
