using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using JULESTech;

public class BuildCamera : MonoBehaviour
{
    public List<GameObject> objects;
    public List<string> names;
    public Text titleName;
    public int indexAt = 0;
    public BuildFurnitureRootNode rootFurnNode;
    public BuildUserInterface buildUI;
    public GameObject BuildUI;
    public GameObject MiscUI;
    public BuildFurnitureNode selectedNode;
    public GameObject upgradeMenu;
    public GameObject canvasGO;
    public TextAsset furniturePricings;


    // Use this for initialization
    public void Start()
    {
        selectedNode = rootFurnNode.selectNewNode(1);
        Init();
        canvasGO.gameObject.SetActive(false);
        transform.parent.gameObject.SetActive(false);
    }
    public void Init()
    {
        //   Debug.Log("BED FURNITURE IS: " + GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, GameState.me.inventory.Bed_Selected));
        FurniturePricings.InitializePrices(furniturePricings);
        try { rootFurnNode.nodeList[0].SelectNewFurniture(GameState.me.inventory.Bed_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, GameState.me.inventory.Bed_Selected), true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[1].SelectNewFurniture(GameState.me.inventory.Shelf_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(1, GameState.me.inventory.Shelf_Selected), true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[2].SelectNewFurniture(GameState.me.inventory.Table_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(2, GameState.me.inventory.Table_Selected), true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[3].SelectNewFurniture(GameState.me.inventory.Carpet_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(3, GameState.me.inventory.Carpet_Selected), true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[4].SelectNewFurniture(GameState.me.inventory.Item1_Selected, 0, true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[5].SelectNewFurniture(GameState.me.inventory.Item2_Selected, 0, true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[6].SelectNewFurniture(GameState.me.inventory.Item3_Selected, 0, true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[7].SelectNewFurniture(GameState.me.inventory.Item4_Selected, 0, true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[8].SelectNewFurniture(GameState.me.inventory.Item5_Selected, 0, true); } catch (System.Exception e) { }
        try { rootFurnNode.nodeList[9].SelectNewFurniture(GameState.me.inventory.Item6_Selected, 0, true); } catch (System.Exception e) { }
        CycleLeft();
        CycleRight();
    }
    public void Finished()
    {
        GameState.me.Upload();
    }


    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, objects[indexAt].transform.position, Time.deltaTime * 10);
        transform.rotation = Quaternion.Lerp(transform.rotation, objects[indexAt].transform.rotation, Time.deltaTime * 20);
        titleName.text = names[indexAt];
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameState.me.inventory.Coins += 10000;
            GameState.me.inventory.StoneTablet += 10000;
            GameState.me.inventory.Jewels += 10000;
            GameState.me.inventory.Ceramic += 10000;
            GameState.me.inventory.Steel += 10000;
            GameState.me.inventory.Wood += 10000;
        }
    }

    public void updateBuildBottomUI(int indexAt)
    {
        if (selectedNode.category != "misc")
        {
            int tier = GameState.me.inventory.getSelectedTier(indexAt);
            
            int levelAt = GameState.me.inventory.furnitureLevels.getFurnitureLevel(indexAt, tier);

            selectedNode.SelectNewFurniture(tier, true);
            upgradeMenu.GetComponent<BuildUpgradeUI>().UpdateUnlockCost(FurniturePricings.GetMyPricing(selectedNode.category, tier, levelAt + 1), false);

            GameObject selectedButton = null;

            switch (tier)
            {
                case 0: selectedButton = GameObject.Find("Style1Button"); ; break;
                case 1: selectedButton = GameObject.Find("Style2Button"); ; break;
                case 2: selectedButton = GameObject.Find("Style3Button (1)"); ; break;
            }

            //Debug.LogError("Non-Misc Item: tier: " + tier + "  index: " + indexAt + "  level: " + levelAt);

            selectedButton.GetComponent<BuildButton>().ClearNonMiscSelectedIndicator();
            selectedButton.GetComponentsInChildren<Text>()[1].text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_selected");
            upgradeMenu.GetComponent<BuildUpgradeUI>().setLastSelectedItemRef(selectedButton.GetComponent<Button>(), tier);
        }
        else
        {
            
            int tier = GameState.me.inventory.getSelectedTier(indexAt);

            //Debug.LogError("MiscItem: " + indexAt + " tier: " + tier);

            GameObject[] goArr = GameObject.FindGameObjectsWithTag("MiscItemButton");
            GameObject selectedButton = goArr[tier];

            for (int i = 0; i < 13; i++)
            {
                var otherButton1 = goArr[i];
                otherButton1.GetComponentsInChildren<Text>()[1].text = "";
                //Debug.LogError(i + " Button name : " + otherButton1.name);
            }
            
            selectedButton.GetComponentsInChildren<Text>()[1].text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_selected");
            
            bool unlocked = GameState.me.inventory.furnitureLevels.getUnlockedMisc(tier);
            if (unlocked || tier == 0)
            {
                //SetMisc(tier);
            }
            else
            {
                upgradeMenu.GetComponent<BuildUpgradeUI>().UpdateUnlockCost(FurniturePricings.GetMyPricing(selectedNode.category, tier, 1), true);
            }
       
            upgradeMenu.GetComponent<BuildUpgradeUI>().setLastSelectedItemRef(selectedButton.GetComponent<Button>(), tier);
        }
    }

    public void CycleLeft()
    {
        indexAt--;
        if (indexAt < 0) indexAt = objects.Count - 1;
        selectedNode = rootFurnNode.selectNewNode(indexAt);
        buildUI.RefreshInterface(selectedNode, this);

        BuildUI.SetActive(selectedNode.category != "misc");
        MiscUI.SetActive(selectedNode.category == "misc");

        updateBuildBottomUI(indexAt);

    }
    public void CycleRight()
    {
        indexAt++;
        if (indexAt > objects.Count - 1) indexAt = 0;
        selectedNode = rootFurnNode.selectNewNode(indexAt);
        buildUI.RefreshInterface(selectedNode, this);
        BuildUI.SetActive(selectedNode.category != "misc");
        MiscUI.SetActive(selectedNode.category == "misc");

        updateBuildBottomUI(indexAt);

    }

    public void SetButtons()
    {

    }
    public void SetFurniture(int tier)
    {
        int index = rootFurnNode.GetCurrentlySelectedNode();
        //  Debug.Log("Setting selected as Tier: " + (tier));
        switch (index)
        {
            case 0:
                GameState.me.inventory.Bed_Selected = tier;
                break;
            case 1:
                GameState.me.inventory.Shelf_Selected = tier;
                break;
            case 2:
                GameState.me.inventory.Table_Selected = tier;
                break;
            case 3:
                GameState.me.inventory.Carpet_Selected = tier;
                break;
            case 4:
                GameState.me.inventory.Item1_Selected = tier;
                break;
            case 5:
                GameState.me.inventory.Item2_Selected = tier;
                break;
            case 6:
                GameState.me.inventory.Item3_Selected = tier;
                break;
            case 7:
                GameState.me.inventory.Item4_Selected = tier;
                break;
            case 8:
                GameState.me.inventory.Item5_Selected = tier;
                break;
            case 9:
                GameState.me.inventory.Item6_Selected = tier;
                break;
        }
        // GameState.me.inventory.Save();
    }
    public void UpgradeFurniture()
    {
        int index = indexAt;
        int tier = 0;

        switch (index)
        {
            case 0:
                tier = GameState.me.inventory.Bed_Selected;
                break;
            case 1:
                tier = GameState.me.inventory.Shelf_Selected;
                break;
            case 2:
                tier = GameState.me.inventory.Table_Selected;
                break;
            case 3:
                tier = GameState.me.inventory.Carpet_Selected;
                break;
        }
        int currentLevel = GameState.me.inventory.furnitureLevels.getFurnitureLevel(index, tier);

        if (currentLevel < 5)
        {
            currentLevel += 1;
        }
        Debug.Log("Index: " + indexAt + " Tier: " + tier + " is being upgraded to level " + currentLevel);
        GameState.me.inventory.furnitureLevels.setFurnitureLevel(index, tier, currentLevel);
        switch (index)
        {
            case 0:
                rootFurnNode.nodeList[0].SelectNewFurniture(GameState.me.inventory.Bed_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, GameState.me.inventory.Bed_Selected));
                break;
            case 1:
                rootFurnNode.nodeList[1].SelectNewFurniture(GameState.me.inventory.Shelf_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(1, GameState.me.inventory.Shelf_Selected));
                break;
            case 2:
                rootFurnNode.nodeList[2].SelectNewFurniture(GameState.me.inventory.Table_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(2, GameState.me.inventory.Table_Selected));
                break;
            case 3:
                rootFurnNode.nodeList[3].SelectNewFurniture(GameState.me.inventory.Carpet_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(3, GameState.me.inventory.Carpet_Selected));
                break;
        }
        upgradeMenu.SetActive(false);
    }
    public void UpgradeFurniture(int tier)
    {
        int index = indexAt;
        int currentLevel = GameState.me.inventory.furnitureLevels.getFurnitureLevel(index, tier);

        if (currentLevel < 5)
        {
            currentLevel += 1;
        }
        Debug.Log("Index: " + indexAt + " Tier: " + tier + " is being upgraded to level " + currentLevel);
        if (currentLevel > 5) currentLevel = 5;
        GameState.me.inventory.furnitureLevels.setFurnitureLevel(index, tier, currentLevel);
        switch (index)
        {
            case 0:
                GameState.me.inventory.Bed_Selected = tier;
                rootFurnNode.nodeList[0].SelectNewFurniture(GameState.me.inventory.Bed_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(0, GameState.me.inventory.Bed_Selected));
                switch (tier)
                {
                    case 0:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Basic Bed");
                        break;
                    case 1:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Captain Bed");
                        break;
                    case 2:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Nature Bed");
                        break;
                }
                break;
            case 1:
                GameState.me.inventory.Shelf_Selected = tier;
                rootFurnNode.nodeList[1].SelectNewFurniture(GameState.me.inventory.Shelf_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(1, GameState.me.inventory.Shelf_Selected));
                switch (tier)
                {
                    case 0:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Basic BedsideTable");
                        break;
                    case 1:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Captain BedsideTable");
                        break;
                    case 2:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Nature BedsideTable");
                        break;
                }
                break;
            case 2:
                GameState.me.inventory.Table_Selected = tier;
                rootFurnNode.nodeList[2].SelectNewFurniture(GameState.me.inventory.Table_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(2, GameState.me.inventory.Table_Selected));
                switch (tier)
                {
                    case 0:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Basic Shelf");
                        break;
                    case 1:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Captain Shelf");
                        break;
                    case 2:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Level " + currentLevel + " Nature Shelf");
                        break;
                }
                break;
            case 3:
                GameState.me.inventory.Carpet_Selected = tier;
                rootFurnNode.nodeList[3].SelectNewFurniture(GameState.me.inventory.Carpet_Selected, GameState.me.inventory.furnitureLevels.getFurnitureLevel(3, GameState.me.inventory.Carpet_Selected));
                switch (tier)
                {
                    case 0:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Clock Carpet");
                        break;
                    case 1:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Squiggles Carpet");
                        break;
                    case 2:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Carpetception");
                        break;
                }
                break;
        }
        //switch (tier)
        //{
        //    case 0:
        //        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Buying Level " + currentLevel + " Basic " + transform.name);
        //        break;
        //    case 1:
        //        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Buying Level " + currentLevel + " Captain " + transform.name);
        //        break;
        //    case 2:
        //        AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Buying Level " + currentLevel + " Nature " + transform.name);
        //        break;
        //}
        upgradeMenu.SetActive(false);
    }

    public void UnlockMisc(int tier)
    {
        //        Debug.Log("Index: " + indexAt + " Tier: " + tier + " is being upgraded to level " + currentLevel);
        GameState.me.inventory.furnitureLevels.unlockMisc(tier);
        switch (tier)
        {
            case 0:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought None");
                break;
            case 1:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Toy Train");
                break;
            case 2:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Toy Car at");
                break;
            case 3:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Tea Set");
                break;
            case 4:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Doll House");
                break;
            case 5:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Boardgame");
                break;
            case 6:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Teddy Bear");
                break;
            case 7:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Truck");
                break;
            case 8:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Robot");
                break;
            case 9:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Anthurium");
                break;
            case 10:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Aloe");
                break;
            case 11:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Daisy");
                break;
            case 12:
                AnalyticsSys.Instance.Report(REPORTING_TYPE.BuyFurniture, "Bought Bush");
                break;

        }
        SetMisc(tier);
        upgradeMenu.SetActive(false);
    }
    public void SetMisc(int tier)
    {
        int index = indexAt;
        switch (index)
        {
            case 4:
                GameState.me.inventory.Item1_Selected = tier;
                break;
            case 5:
                GameState.me.inventory.Item2_Selected = tier;
                break;
            case 6:
                GameState.me.inventory.Item3_Selected = tier;
                break;
            case 7:
                GameState.me.inventory.Item4_Selected = tier;
                break;
            case 8:
                GameState.me.inventory.Item5_Selected = tier;
                break;
            case 9:
                GameState.me.inventory.Item6_Selected = tier;
                break;
        }
        rootFurnNode.nodeList[index].SelectNewFurniture(tier, 0);
      }
}
