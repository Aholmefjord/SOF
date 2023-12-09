using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;
public class tumbleTroubleDesign : MonoBehaviour {
    public static List<TumbleTroubleSetting> levelSettings;
    public static void Load()
    {
        //TextAsset t = Resources.Load<TextAsset>("tumbleTroubleDesign");
        JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.TUMBLE_TROUBLE_CONFIG, "tumbleTroubleDesign", (textAsset) => {
            JSONNode arr = JSONNode.Parse<JSONNode>(textAsset.text);
            levelSettings = new List<TumbleTroubleSetting>();
            foreach (JSONNode n in arr["levelData"].AsArray) {
                TumbleTroubleSetting mmms = new TumbleTroubleSetting(n);
                levelSettings.Add(mmms);
            }
            //Debug.Log("Loaded Manta Level Count: " + levelSettings.Count);
        });
    }
    public static TumbleTroubleSetting getLevelDesign(int at)
    {
        return levelSettings[at];
    }
}

public class TumbleTroubleSetting{
    public int IconCount { get; set; }
    public int SpecialIconCount { get; set; }
    public List<int> IconTypes { get; set; }
    public int ScoreThreshold_2Star { get; set; }
    public int ScoreThreshold_3Star { get; set; }
    public int StageClearBonus_Score { get; set; }
    public int FirstClearBonusGold_Max { get; set; }
    public int FirstClearBonusGold_Min { get; set; }
    public int GoldReward_Min { get; set; }
    public int GoldReward_Max { get; set; }
    public int ThreeStarReward_Gold { get; set; }
    public int DiminishedGoldReward_Min { get; set; }
    public int FirstClearBonus_Jewel { get; set; }
    public int JewelReward_Min { get; set; }
    public int ThreeStarReward_Jewel { get; set; }
    public int JewelReward_Max { get; set; }
    public TumbleTroubleSetting(JSONNode n)
    {
        IconCount = n["IconCount"].AsInt;
        SpecialIconCount = n["SpecialIconCount"].AsInt;
        ScoreThreshold_2Star = n["ScoreThreshold_2Star"].AsInt;
        ScoreThreshold_3Star = n["ScoreThreshold_3Star"].AsInt;
        StageClearBonus_Score = n["StageClearBonus_Score"].AsInt;
        FirstClearBonusGold_Max = n["FirstClearBonusGold_Max"].AsInt;
        FirstClearBonusGold_Min = n["FirstClearBonusGold_Min"].AsInt;
        GoldReward_Min = n["GoldReward_Min"].AsInt;
        GoldReward_Max = n["GoldReward_Max"].AsInt;
        JewelReward_Max = n["JewelReward_Max"].AsInt;
        ThreeStarReward_Gold = n["ThreeStarReward_Gold"].AsInt;
        DiminishedGoldReward_Min = n["DiminishedGoldReward_Min"].AsInt;
        FirstClearBonus_Jewel = n["FirstClearBonus_Jewel"].AsInt;
        JewelReward_Min = n["JewelReward_Min"].AsInt;
        ThreeStarReward_Jewel = n["ThreeStarReward_Jewel"].AsInt;
        JewelReward_Max = n["JewelReward_Max"].AsInt;
        IconTypes = new List<int>();
        foreach (JSONNode node in n["IconTypes"].AsArray)
        {
            IconTypes.Add(node.AsInt);
        }
    }
}
