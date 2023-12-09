using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JULESTech;
public class BuildFurnitureNode : MonoBehaviour {
    public string category;
    public BuildCamera buildCam;
    public List<GameObject> tierOneFurnitures;
    public List<GameObject> tierTwoFurnitures;
    public List<GameObject> tierThreeFurnitures;
    public List<GameObject> specialTierFurnitures;
    public bool isSmallItem;
    public void SelectNewFurniture(int tier,int level, bool load = false)
    {
        //Debug.Log("Selecting: " + tier + " , " + level);
        for (int i = 0; i < tierOneFurnitures.Count; i++)
        {
            tierOneFurnitures[i].SetActive(false);
        }
        if (category != "misc")
        {
            for (int i = 0; i < tierTwoFurnitures.Count; i++)
            {
                tierTwoFurnitures[i].SetActive(false);
            }
            for (int i = 0; i < tierThreeFurnitures.Count; i++)
            {
                tierThreeFurnitures[i].SetActive(false);
            }
            for (int i = 0; i < specialTierFurnitures.Count; i++)
            {
                specialTierFurnitures[i].SetActive(false);
            }
        }
        //Debug.Log("Tier - " + tier + " Level - " + level);
        if(category != "misc") { 
            switch (tier)
            {
                case 0:

                    tierOneFurnitures[Mathf.Max(0,Mathf.Min(tierOneFurnitures.Count-1, level - 1))].SetActive(true);
                    break;
                case 1:
                    tierTwoFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                    break;
                case 2:
                    tierThreeFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                    break;
                case 3:
                    specialTierFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                    break;
            }
        }else
        {
            tierOneFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, tier))].SetActive(true);
            if (load != true)
            {
                switch (tier)
                {
                    case 0:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed None at " + transform.name);
                        break;
                    case 1:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Toy Train at " + transform.name);
                        break;
                    case 2:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Toy Car at " + transform.name);
                        break;
                    case 3:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Tea Set at " + transform.name);
                        break;
                    case 4:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Doll House at " + transform.name);
                        break;
                    case 5:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Boardgame at " + transform.name);
                        break;
                    case 6:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Teddy Bear at " + transform.name);
                        break;
                    case 7:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Truck at " + transform.name);
                        break;
                    case 8:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Robot at " + transform.name);
                        break;
                    case 9:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Anthurium at " + transform.name);
                        break;
                    case 10:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Aloe at " + transform.name);
                        break;
                    case 11:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Daisy at " + transform.name);
                        break;
                    case 12:
                        AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Bush at " + transform.name);
                        break;

                }
            }
        }

    }
    public void SelectNewFurniture(int tier, bool load = false)
    {
            //Debug.Log(buildCam.rootFurnNode.GetCurrentlySelectedNode());
            int level = GameState.me.inventory.furnitureLevels.getFurnitureLevel(buildCam.rootFurnNode.GetCurrentlySelectedNode(), tier);
            for (int i = 0; i < tierOneFurnitures.Count; i++)
            {
                tierOneFurnitures[i].SetActive(false);
            }
            for (int i = 0; i < tierTwoFurnitures.Count; i++)
            {
                tierTwoFurnitures[i].SetActive(false);
            }
            for (int i = 0; i < tierThreeFurnitures.Count; i++)
            {
                tierThreeFurnitures[i].SetActive(false);
            }
            for (int i = 0; i < specialTierFurnitures.Count; i++)
            {
                specialTierFurnitures[i].SetActive(false);
            }
            if(category != "misc") {
                if (category == "carpet")
                {
                    switch (tier)
                    {
                        case 0:

                            tierOneFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                        if (load != true)
                        {
                            AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Clock Carpet");
                        }
                            break;
                        case 1:
                            tierTwoFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                        if (load != true)
                        {
                            AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Squiggles Carpet");
                        }
                            break;
                        case 2:
                            tierThreeFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                        if (load != true)
                        {
                            AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Carpetception");
                        }
                            break;
                        case 3:
                            specialTierFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                            break;
                    }
                }
                else
                {
                    switch (tier)
                    {
                        case 0:

                            tierOneFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                        if (load != true)
                        {
                            AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Level " + level + " Basic " + category);
                        }
                            break;
                        case 1:
                            tierTwoFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                        if (load != true)
                        {
                            AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Level " + level + " Captain " + category);
                        }
                            break;
                        case 2:
                            tierThreeFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                        if (load != true)
                        {
                            AnalyticsSys.Instance.Report(REPORTING_TYPE.PlaceFurniture, "Placed Level " + level + " Nature " + category);
                        }
                            break;
                        case 3:
                            specialTierFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, level - 1))].SetActive(true);
                            break;
                    }
                }
                //Debug.Log("Selecting new set for:" + transform.name + ". Tier - " + tier + " Level - " + level);
                buildCam.SetFurniture(tier);
            }else{
                tierOneFurnitures[Mathf.Max(0, Mathf.Min(tierOneFurnitures.Count - 1, tier))].SetActive(true);
                   
            }

    }
    public GameObject GetFurniture(int tier, int level)
    {
        try
        {
            Debug.Log(buildCam.rootFurnNode.GetCurrentlySelectedNode());
            if (tier == 0)
            {

            }
            else
            {
                level = GameState.me.inventory.furnitureLevels.getFurnitureLevel(buildCam.rootFurnNode.GetCurrentlySelectedNode(), tier);
            }
            switch (tier)
            {
                case 0:
                    return tierOneFurnitures[level];
                case 1:
                    return tierTwoFurnitures[level];
                case 2:
                    return tierThreeFurnitures[level]; 
                case 3:
                    return specialTierFurnitures[level];
            }
            return null;
        }
        catch (System.Exception e)
        {
            return null;
        }
    }
}
