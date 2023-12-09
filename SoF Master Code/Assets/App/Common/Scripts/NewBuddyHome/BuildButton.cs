using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
public class BuildButton : MonoBehaviour
{
    public bool Unlocked;
    public int StageAT;
    GameObject selectedObject;
    BuildFurnitureNode selectedNode;
    public BuildCamera bc;
    public BuildUpgradeUI bui;
    public UpgradeVerification uv;// Use this for initialization

    Button thisButton;

    void Start()
    {
        thisButton = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ExchangeSets(BuildFurnitureNode newNode, BuildCamera bc)
    {
        this.bc = bc;
        selectedNode = newNode;
    }

    public void ClearNonMiscSelectedIndicator()
    {
        var otherButton1 = GameObject.Find("Style1Button");
        var txtCompo = otherButton1.FindChild("TextB");
        txtCompo.SetText("");

        var otherButton2 = GameObject.Find("Style2Button");
        txtCompo = otherButton2.FindChild("TextB");
        txtCompo.SetText("");

        var otherButton3 = GameObject.Find("Style3Button (1)");
        txtCompo = otherButton3.FindChild("TextB");
        txtCompo.SetText("");
    }

    public void ClearMiscSelectedIndicator()
    {
        GameObject[] goArr = GameObject.FindGameObjectsWithTag("MiscItemButton");
        for (int i = 0; i < 13; i++)
        {
            var txtCompo = goArr[i].GetComponentsInChildren<Text>();
            txtCompo[1].text = "";
        }
    }

    public void SelectSet(int SetToSelect)
    {
        //    Debug.Log("Selected Set " + SetToSelect);
        //     if(SetToSelect == 0)
        //      {
        //           selectedNode.SelectNewFurniture(SetToSelect);
        //           return;
        //        }


        ClearNonMiscSelectedIndicator();

        var arr4 = thisButton.GetComponentsInChildren<Text>();
        arr4[1].text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_selected");


        Debug.Log(GameState.me.inventory.furnitureLevels.getFurnitureLevel(bc.indexAt, SetToSelect) + " IS BED LEVEL");
        Debug.Log("Selecting Index: " + bc.indexAt + ", Set:" + SetToSelect);
        int levelAt = GameState.me.inventory.furnitureLevels.getFurnitureLevel(bc.indexAt, SetToSelect);
        if (levelAt > 0)
        {
            selectedNode.SelectNewFurniture(SetToSelect);
            bui.UpdateUnlockCost(FurniturePricings.GetMyPricing(bc.selectedNode.category, SetToSelect, levelAt + 1), false);

            bui.setLastSelectedItemRef(thisButton, SetToSelect);
        }
        else
        {
            Debug.Log("NewFurniture being selected to unlock: " + SetToSelect);
            Debug.Log("UnlockCategory: " + bc.selectedNode.category);
            bui.UpdateUnlockCost(FurniturePricings.GetMyPricing(bc.selectedNode.category, SetToSelect, levelAt + 1), true);
        }
    }
    public void SelectMisc(int MiscToSelect)
    {
        GameObject[] goArr = GameObject.FindGameObjectsWithTag("MiscItemButton");
        GameObject thisButtonGO = goArr[MiscToSelect];

        ClearMiscSelectedIndicator();

        var txtCompo = thisButtonGO.GetComponentsInChildren<Text>();
        txtCompo[1].text = MultiLanguage.getInstance().getString("buddy_home_build_panel_button_selected");

        Debug.Log("Selecting Index: " + bc.indexAt + ", Set:" + MiscToSelect);
        bool unlocked = GameState.me.inventory.furnitureLevels.getUnlockedMisc(MiscToSelect);
        if (unlocked || MiscToSelect == 0)
        {
            bc.SetMisc(MiscToSelect);
            bui.setLastSelectedItemRef(thisButtonGO.GetComponent<Button>(), MiscToSelect);
        }
        else
        {
            Debug.Log("NewFurniture being selected to unlock: " + MiscToSelect);
            Debug.Log("UnlockCategory: " + bc.selectedNode.category);
            bui.UpdateUnlockCost(FurniturePricings.GetMyPricing(bc.selectedNode.category, MiscToSelect, 1), true);
        }
    }
}
