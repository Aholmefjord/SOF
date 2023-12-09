using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class BuildUserInterface : MonoBehaviour {
    public Text coinText;
    public Text jewelText;
    public Text ceramicText;
    public Text woodText;
    public Text steelText;
    public Text stoneText;
    
    public Button tier1Button;
    public Button tier2Button;
    public Button tier3Button;
    private bool setupUIDone = false;

    private BuildFurnitureNode currentNode;
    private BuildCamera currentCam;
    private MultiLanguageApplicator textApplicator;

    void OnEnable()
    {
        MultiLanguage.getInstance().OnLanguageChanged += LanguageChangeCallback;
    }
    void OnDisable()
    {
        MultiLanguage.getInstance().OnLanguageChanged -= LanguageChangeCallback;
    }
    void LanguageChangeCallback(string prev, string curr) {
        ChangeLanguage();
    }

    // Use this for initialization
    void Start () {
        currentNode = null;
        currentCam = null;
	}

    public void ChangeLanguage()
    {
        //This is because we can only setup UI after RefreshInterface has been called once
        //And we should not override the setupUI before refreshinterface calling it.
        if (setupUIDone)
        {
            SetupUI();
            
            RefreshInterface(currentNode, currentCam);
        }
    }

    private void SetupUI()
    {
        if (textApplicator == null)
            textApplicator = new MultiLanguageApplicator(gameObject);

        textApplicator.ApplyText("BottomUI/UpgradeButton/Text", "buddy_home_build_panel_button_upgrade");
        textApplicator.ApplyText("BottomUI/Style1Button/Text", "buddy_home_build_panel_button_style");
        textApplicator.ApplyText("BottomUI/Style1Button/TextB", "buddy_home_build_panel_button_selected");
        textApplicator.ApplyText("BottomUI/Style2Button/Text", "buddy_home_build_panel_button_style");
        textApplicator.ApplyText("BottomUI/Style2Button/TextB", "buddy_home_build_panel_button_selected");
        textApplicator.ApplyText("BottomUI/Style3Button (1)/Text", "buddy_home_build_panel_button_style");
        textApplicator.ApplyText("BottomUI/Style3Button (1)/TextB", "buddy_home_build_panel_button_selected");
        textApplicator.ApplyText("BottomUI/SpecialStyleButton/Text", "buddy_home_build_panel_button_secret_style");

        textApplicator.ApplyText("BottomUIMisc/Style1Button/Text", "buddy_home_build_panel_button_style");
        textApplicator.ApplyText("BottomUIMisc/Style1Button/TextB", "buddy_home_build_panel_button_selected");
        /*
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUI").FindChild("UpgradeButton").FindChild("Text"), "buddy_home_build_panel_button_upgrade");
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUI").FindChild("Style1Button").FindChild("Text"), "buddy_home_build_panel_button_style");
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUI").FindChild("Style1Button").FindChild("TextB"), "buddy_home_build_panel_button_selected");
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUI").FindChild("Style2Button").FindChild("Text"), "buddy_home_build_panel_button_style");
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUI").FindChild("Style2Button").FindChild("TextB"), "buddy_home_build_panel_button_selected");
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUI").FindChild("Style3Button (1)").FindChild("Text"), "buddy_home_build_panel_button_style");
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUI").FindChild("Style3Button (1)").FindChild("TextB"), "buddy_home_build_panel_button_selected");
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUI").FindChild("SpecialStyleButton").FindChild("Text"), "buddy_home_build_panel_button_secret_style");

        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUIMisc").FindChild("Style1Button").FindChild("Text"), "buddy_home_build_panel_button_style");
        MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUIMisc").FindChild("Style1Button").FindChild("TextB"), "buddy_home_build_panel_button_selected");
        
        //*/
        for (int i=1; i<=12; ++i) {
            textApplicator.ApplyText("BottomUIMisc/Style1Button (" + i + ")/Text", "buddy_home_build_panel_button_style");
            textApplicator.ApplyText("BottomUIMisc/Style1Button (" + i + ")/TextB", "buddy_home_build_panel_button_selected");
            //MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (" + i + ")").FindChild("Text"), "buddy_home_build_panel_button_style");
            //MultiLanguage.getInstance().apply(gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (" + i + ")").FindChild("TextB"), "buddy_home_build_panel_button_selected");
        }

        textApplicator.ApplyText("BottomUIMisc/Style1Button/Text",      "buddy_home_build_panel_button_toytrain");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (1)/Text",   "buddy_home_build_panel_button_toycar");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (2)/Text",   "buddy_home_build_panel_button_teaset");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (3)/Text",   "buddy_home_build_panel_button_dollhouse");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (4)/Text",   "buddy_home_build_panel_button_boardgame");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (5)/Text",   "buddy_home_build_panel_button_teddybear");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (6)/Text",   "buddy_home_build_panel_button_truck");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (7)/Text",   "buddy_home_build_panel_button_robot");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (8)/Text",   "buddy_home_build_panel_button_anthurium");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (9)/Text",   "buddy_home_build_panel_button_aloe");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (10)/Text",  "buddy_home_build_panel_button_daisy");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (11)/Text",  "buddy_home_build_panel_button_bush");
        textApplicator.ApplyText("BottomUIMisc/Style1Button (12)/Text",  "buddy_home_build_panel_button_None");
        /*
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_toytrain");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (1)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_toycar");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (2)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_teaset");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (3)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_dollhouse");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (4)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_boardgame");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (5)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_teddybear");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (6)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_truck");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (7)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_robot");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (8)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_anthurium");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (9)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_aloe");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (10)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_daisy");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (11)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_bush");
        gameObject.FindChild("BottomUIMisc").FindChild("Style1Button (12)").FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_None");
        //*/
        setupUIDone = true;

        if(currentCam != null && currentNode != null)
        {
            RefreshInterface(currentNode, currentCam);
        }
    }

    // Update is called once per frame
    void Update () {
        coinText.text = GameState.me.inventory.Coins.ToString();
        jewelText.text = GameState.me.inventory.Jewels.ToString();
        ceramicText.text = GameState.me.inventory.Ceramic.ToString();
        steelText.text = GameState.me.inventory.Steel.ToString();
        stoneText.text = GameState.me.inventory.StoneTablet.ToString();
		woodText.text = GameState.me.inventory.Wood.ToString();
	}

    public void RefreshInterface(BuildFurnitureNode node,BuildCamera cam)
    {
        if(!setupUIDone)
            SetupUI();

        currentNode = node;
        currentCam = cam;

        if (node.isSmallItem)
        {
            tier1Button.transform.GetChild(0).GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_None");
            tier2Button.transform.GetChild(0).GetComponent<Text>().text = "Globe";
        }
        else
        {
            try
            {
                tier1Button.transform.GetChild(0).GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_" + node.transform.GetChild(0).name);
                tier2Button.transform.GetChild(0).GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_" + node.transform.GetChild(1).name);
                tier3Button.transform.GetChild(0).GetComponent<Text>().text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_" + node.transform.GetChild(2).name);
                tier1Button.GetComponent<BuildButton>().ExchangeSets(node, cam);
                tier2Button.GetComponent<BuildButton>().ExchangeSets(node, cam);
                tier3Button.GetComponent<BuildButton>().ExchangeSets(node, cam);
            }catch(System.Exception e)
            {

            }
        }
    }
}
