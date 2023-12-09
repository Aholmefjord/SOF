using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;
public class FurniturePricing {
    public string furniture_category;
    public int furniture_tier;
    public int furniture_level;

    public int costCoin;
    public int costJewel;
    public int costWood;
    public int costDiamond;
    public int costMetal;
    public int costFabric;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public FurniturePricing(JSONNode jsonPlayerData)
    {
        
        furniture_category = jsonPlayerData["furniture_category"];
        furniture_tier = jsonPlayerData["furniture_tier"].AsInt;
        furniture_level = jsonPlayerData["furniture_level"].AsInt;
        costCoin = jsonPlayerData["costCoin"].AsInt;
        costJewel = jsonPlayerData["costJewel"].AsInt;
        costWood = jsonPlayerData["costWood"].AsInt;
        costDiamond = jsonPlayerData["costDiamond"].AsInt;
        costMetal = jsonPlayerData["costMetal"].AsInt;
        costFabric = jsonPlayerData["costFabric"].AsInt;        
    }
    public bool Equals(string category,int tier,int level)
    {
        return (category == this.furniture_category && this.furniture_tier == tier && this.furniture_level == level);
    }
}
public static class FurniturePricings
{
    public static List<FurniturePricing> pricings;

    public static FurniturePricing GetMyPricing(string category, int tier, int level)
    {
        for(int i = 0; i < pricings.Count; i++)
        {
            if(pricings[i].Equals(category,tier,level))
            {
                return pricings[i];
            }
        }
        return null;
    }
    public static void InitializePrices(TextAsset t)
    {
        JSONArray arr = JSONArray.Parse<JSONArray>(t.text);
        pricings = new List<FurniturePricing>();
        foreach(JSONNode n in arr)
        {
            pricings.Add(new FurniturePricing(n));
        }
        //Debug.Log("Loaded " + pricings.Count + " Furniture Pricings");
    }
}