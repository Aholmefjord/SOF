using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class BuildUpgradeUI : MonoBehaviour {
    public int baseCostGold;
    public int baseCostJules;
    public int baseCostMetal;
    public int baseCostFabric;
    public int baseCostDiamond;
    public int baseCostWood;
    public UpgradeVerification uv;
    int costGold;
    int costJules;
    int costMetal;
    int costFabric;
    int costDiamond;
    int costWood;
    public BuildCamera bc;

    public Text textGold;
    public GameObject goGold;
    public Text textJules;
    public GameObject goJules;
    public Text textMetal;
    public GameObject goMetal;
    public Text textFabric;
    public GameObject goFabric;
    public Text titleText;
    public GameObject goDiamond;
    public Text textDiamond;
    public GameObject goWood;
    public Text textWood;
    public Button upgradeButton;

    private Button lastButton;
    private int lastSet;

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(gameObject.FindChild("Image (1)").FindChild("Text"), "buddy_home_upgrade_panel_upgrade_text");
    }

    public void setLastSelectedItemRef(Button btn, int set)
    {
        lastButton = btn;
        lastSet = set;

        SetupUI();
    }

    public void selectLastSet()
    {
        /*
        if (lastButton.CompareTag("MiscItemButton") == false)
            lastButton.SelectSet(lastSet);
        else
            lastButton.SelectMisc(lastSet);
            */
        Debug.LogError("selectLastSet");
        lastButton.onClick.Invoke();
    }

    // Use this for initialization

    // Update is called once per frame
    public bool isUpgrade;
    public bool isMisc;
    int tierSelected;
    FurniturePricing selectedPricing;
    public void UpdateUnlockCost(FurniturePricing tier,bool activate)
    {
        //        tierSelected = tier;

        if (tier == null) return;//CARPET IS KILL THIS

        Debug.Log("SELECTING TO UPGRADE TIER: " + tier.furniture_tier);
        selectedPricing = tier;
        uv.tier = tier.furniture_tier;
        isUpgrade = false;
        uv.isUnlock = true;
        isMisc = tier.furniture_category == "misc";
        Debug.Log("Unlock confirmation for Tier: " + uv.tier);
        costGold = tier.costCoin;
        costJules = tier.costJewel;
        costMetal = tier.costMetal;
        costFabric = tier.costFabric;// tier * baseCostFabric;
        costDiamond = tier.costDiamond;// tier * baseCostDiamond;
        costWood = tier.costWood;
        SetTexts(costGold,costJules, costMetal, costFabric,costDiamond, costWood);
        titleText.text = (isMisc)? MultiLanguage.getInstance().getString("buddy_home_upgrade_panel_unlock_item_text") : MultiLanguage.getInstance().getString("buddy_home_upgrade_panel_unlock_furniture_text");
        greyUpgradeButton();
        gameObject.SetActive(activate);

    }
    int getTier()
    {
        switch (bc.indexAt)
        {
            case 0:
                return GameState.me.inventory.Bed_Selected;
            case 1:
                return GameState.me.inventory.Shelf_Selected;
            case 2:
                return GameState.me.inventory.Table_Selected;
            case 3:
                return GameState.me.inventory.Carpet_Selected;
        }
        return 0;
    }
	int levelPlusOne;
	int tier;


	public void UpdateCosts() {

        isUpgrade = true;
        //int tier = getTier();
        
        uv.tier = selectedPricing.furniture_tier;
        isUpgrade = false;
        uv.isUnlock = true;
        isMisc = selectedPricing.furniture_category == "misc";
        Debug.Log("Unlocking Tier: " + uv.tier);
        costGold = selectedPricing.costCoin;
        costJules = selectedPricing.costJewel;
        costMetal = selectedPricing.costMetal;
        costFabric = selectedPricing.costFabric;// tier * baseCostFabric;
        costDiamond = selectedPricing.costDiamond;// tier * baseCostDiamond;
        costWood = selectedPricing.costWood;
        SetTexts(costGold, costJules, costMetal, costFabric, costDiamond, costWood);
        titleText.text = (isMisc) ? MultiLanguage.getInstance().getString("buddy_home_upgrade_panel_upgrade_item_text") : MultiLanguage.getInstance().getString("buddy_home_upgrade_panel_upgrade_furniture_text");
        gameObject.SetActive(true);
        //     }
    }
    public void UpdateLevelAt()
    {
        isUpgrade = true;
        //int tier = getTier();
        selectedPricing = FurniturePricings.GetMyPricing(selectedPricing.furniture_category, selectedPricing.furniture_tier, selectedPricing.furniture_level + 1);
        if (selectedPricing != null)
        {
            uv.tier = selectedPricing.furniture_tier;
            isUpgrade = false;
            uv.isUnlock = true;
            isMisc = selectedPricing.furniture_category == "misc";
            Debug.Log("Unlocking Tier: " + uv.tier);
            costGold = selectedPricing.costCoin;
            costJules = selectedPricing.costJewel;
            costMetal = selectedPricing.costMetal;
            costFabric = selectedPricing.costFabric;// tier * baseCostFabric;
            costDiamond = selectedPricing.costDiamond;// tier * baseCostDiamond;
            costWood = selectedPricing.costWood;
            SetTexts(costGold, costJules, costMetal, costFabric, costDiamond, costWood);
            titleText.text = (isMisc) ? MultiLanguage.getInstance().getString("buddy_home_upgrade_panel_upgrade_item_text") : MultiLanguage.getInstance().getString("buddy_home_upgrade_panel_upgrade_furniture_text");
            greyUpgradeButton();
        }
    }
	public void greyUpgradeButton()
	{
        //Debug.LogError("greyUpgradeButton: cat: " + bc.selectedNode.category);
		int max = (bc.selectedNode.category == "bedside_table")? 3 : 5;
		tier = getTier();
		levelPlusOne = GameState.me.inventory.furnitureLevels.getFurnitureLevel(bc.indexAt, tier) + 1;
        if (levelPlusOne > max || bc.selectedNode.category == "carpet")
        {
            //Debug.LogError("grey1");
            upgradeButton.interactable = false;
        }else { 
			//Debug.LogError("grey2");
			upgradeButton.interactable = true;
		}

        if (bc.selectedNode.category == "carpet")
        {
            GameObject.Find("UpgradeButton").FindChild("Text").SetText("");
            GameObject.Find("UpgradeButton").SetAlpha(0);
        }
        else if (bc.selectedNode.category != "misc")
        {
            Debug.LogError("selectedNode.category: " + bc.selectedNode.category);
            GameObject.Find("UpgradeButton").FindChild("Text").SetText(MultiLanguage.getInstance().getString("buddy_home_build_panel_button_upgrade"));
            GameObject.Find("UpgradeButton").SetAlpha(1);
        }
	}

    void SetTexts(int gold, int jules, int metal, int fabric, int diamond, int wood)
    {
        
       textGold.text = gold.ToString();
      goGold.SetActive(gold != 0);
        textJules.text = jules.ToString();
      goJules.SetActive(jules != 0);
        textMetal.text = metal.ToString();
        goMetal.SetActive(metal != 0);
      textFabric.text = fabric.ToString();
      goFabric.SetActive(fabric != 0);
        textDiamond.text = diamond.ToString();
        goDiamond.SetActive(diamond != 0);
        textWood.text = wood.ToString();
        goWood.SetActive(wood != 0);

}
    public bool Unlock()
    {
        if (costGold <= GameState.me.inventory.Coins && costJules <= GameState.me.inventory.Jewels && costMetal <= GameState.me.inventory.Ceramic && baseCostFabric <= GameState.me.inventory.StoneTablet && baseCostDiamond <= GameState.me.inventory.Steel)
        {
            GameState.me.inventory.Coins -= costGold;
            GameState.me.inventory.Jewels -= costJules;
            GameState.me.inventory.Ceramic -= costMetal;
            GameState.me.inventory.StoneTablet -= costFabric;
            GameState.me.inventory.Steel -= costDiamond;
            GameState.me.inventory.Wood -= costWood;
            return true;
        }
        else
        {
            return false;
        }

    }
}
