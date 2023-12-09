using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignController : MonoBehaviour {

    // Use this for initialization]
    public static CampaignController current;
    public GameObject BackGround;
    public GameObject DefaultImage;
    public GameObject Campaign;
    public GameObject BuddyHome;
    public GameObject Logoutcheck;

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(BackGround.FindChild("HeaderText"), "campaignmode_header");
        MultiLanguage.getInstance().apply(Campaign.FindChild("BottomText"), "campaignmode_campaign_text");
        MultiLanguage.getInstance().apply(BuddyHome.FindChild("BottomText"), "campaignmode_buddyhome_text");
        MultiLanguage.getInstance().apply(BackGround.FindChild("LogoutText"), "map_new_settings_logout_alt_text");
        MultiLanguage.getInstance().apply(Logoutcheck.FindChild("Text"), "buddy_home_exit_panel_text");

        MultiLanguage.getInstance().applyImage(Campaign.FindChild("GreenBoxText").GetComponent<Image>(), "campaign_main_menu_campaign_button");
        MultiLanguage.getInstance().applyImage(BuddyHome.FindChild("BlueBoxText").GetComponent<Image>(), "campaign_main_menu_home_button");
    }
    void Start () {
        PhotoGallery.LoadImageFromDevice("BuddyHomeSS", DefaultImage.GetComponent<RawImage>());
        SetupUI();
    }

    // Update is called once per frame
    void Update () {
		
	}
    public void GoToBuddyHome()
    {
        MainNavigationController.GoToHome();
    }
    public void GoToCampaignLevel()
    {
        MainNavigationController.DoLoad("CampaignLevel");
    }
    public void Logout() 
    {
        PlayerPrefs.SetInt("NeedsLogout", 1);
		PlayerPrefs.DeleteKey("CachedPlayerUsername");
		PlayerPrefs.DeleteKey("CachedPlayerAccess");
        MainNavigationController.GoToStartMenu();
    }
}
