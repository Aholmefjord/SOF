using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class MatchGameController : MonoBehaviour {
	public static MatchGameController current;
	public GameObject ownPlayerGO;
	public Animator ownPlayerAnimator;
	public GameObject friendPlayerGO;
	public Animator friendAnimator;
    public GameObject settingUI;
	public GameObject avatarParent;
	public Text hungerText;
	public GameObject hungerPanel;

	void Awake () {
		current = this;
	}
    public void SetupUI()
    {
        MultiLanguage.getInstance().applyImage(settingUI.FindChild("Button Exit").GetComponent<Image>(), "gui_settings_home");
        MultiLanguage.getInstance().applyImage(settingUI.FindChild("Button Resume").GetComponent<Image>(), "gui_settings_resume");
        MultiLanguage.getInstance().applyImage(settingUI.FindChild("Button Replay").GetComponent<Image>(), "gui_settings_restart");
    }
    void Start()
	{
		LoadAvatar();
        SetupUI();
		//ScoreScreenLoadBuddyAvatar.LoadAvatar();
	}
	
	void LoadAvatar()
	{
		// Destroy old pet objects if any
		foreach (Transform g in ownPlayerGO.GetComponentsInChildren<Transform>())
		{
			if (g.gameObject != ownPlayerGO)
			{
				Destroy(g.gameObject);
			}
		}

		GameObject avatar = AvatarLoader.current.GetAvatarObject();
		avatar.transform.SetParent(ownPlayerGO.transform, false);
		ownPlayerAnimator = avatar.GetComponent<Animator>();
	}

	public void onMapButtonPress(){
		MainNavigationController.DoLoad ("map");
	}

	void Update()
	{ //Debug.LogError("HUNGER: " + PlayerPrefs.GetFloat("BuddyHunger"));
		//if (PlayerPrefs.GetInt("FromChomp") == 1)
		//{
		//	PlayerPrefs.SetInt("FromChomp", 0);
		//	int hunger = (int)(PlayerPrefs.GetFloat("BuddyHunger") / 10);
		//	hungerPanel.SetActive(true);
		//	hungerText.text = hunger.ToString() + "%";
		//	PlayerPrefs.SetFloat("BuddyHunger", (PlayerPrefs.GetFloat("BuddyHunger") + 200));
		//	if (PlayerPrefs.GetFloat("BuddyHunger") > 1000)
		//		PlayerPrefs.SetFloat("BuddyHunger", 1000);
		//	int afterHunger = (int)(PlayerPrefs.GetFloat("BuddyHunger") / 10);

		//	if (avatarParent.transform.GetChild(0).GetComponent<Animator>() != null)
		//	{
		//		avatarParent.transform.GetChild(0).GetComponent<Animator>().CrossFade("LoveCycle_State", 0.15f);
		//	}

		//	DOTween.Sequence()
		//		.AppendInterval(1f)
		//		.AppendCallback(() =>
		//		{
		//			DOTween.To(() => hunger, x => hunger = x, afterHunger, 1).OnUpdate(() => hungerText.text = hunger.ToString() + "%");
		//		})
		//		.AppendInterval(5f)
		//		.AppendCallback(() =>
		//		{
		//			hungerPanel.SetActive(false);
		//			MainNavigationController.GoToMap();
		//		});
		//}
	}
}
