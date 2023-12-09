using UnityEngine;
using System.Collections;
using AutumnInteractive.SimpleJSON;
using System.Collections.Generic;
public class mantaMatchDesign : MonoBehaviour {
    public static List<MantaMatchManiaSettings> levelSettings;
	public static void Load()
    {
        //TextAsset t = Resources.Load<TextAsset>("matchManiaDesign");
        JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.MANTA_MATCH_CONFIG, "matchManiaDesign", (loadedTextAsset) => {
            JSONNode arr = JSONNode.Parse<JSONNode>(loadedTextAsset.text);
            levelSettings = new List<MantaMatchManiaSettings>();
            foreach (JSONNode n in arr["levelData"].AsArray) {
                MantaMatchManiaSettings mmms = new MantaMatchManiaSettings(n);
                levelSettings.Add(mmms);
            }
        });
    }
    public static MantaMatchManiaSettings getLevelDesign(int at)
    {
        return levelSettings[at];
    }
}
public class MantaMatchManiaSettings
{
    public int FirstClearBonusGold_Max { get; set; }
    public int FirstClearBonusGold_Min { get; set; }
    public int GoldReward_Min { get; set; }
    public int GoldReward_Max { get; set; }
    public int ThreeStarReward_Gold { get; set; }
    public int DiminishedGoldReward_Min { get; set; }
	public int DiminishedGoldReward_Max{ get; set; }
    public int FirstClearBonus_Jewel { get; set; }
    public int JewelReward_Min { get; set; }
    public int ThreeStarReward_Jewel { get; set; }
    public int JewelReward_Max { get; set; }
    public int ScoreThreshold_2Star { get; set; }
    public int ScoreThreshold_3Star { get; set; }
    public int StageClearBonus_Score { get; set; }
    public int PerRemainingSecond_Score { get; set; }
    public int PerPuzzleCleared_Score { get; set; }
    public int PerfectGame_Score { get; set; }
    public int Puzzle_Count { get; set; }
    public int PuzzleTime_Limit { get; set; }
    public MantaMatchManiaSettings(JSONNode n)
    {
        FirstClearBonusGold_Max = n["FirstClearBonusGold_Max"].AsInt;
        FirstClearBonusGold_Min = n["FirstClearBonusGold_Min"].AsInt;
        GoldReward_Min = n["GoldReward_Min"].AsInt;
        GoldReward_Max = n["GoldReward_Max"].AsInt;
        ThreeStarReward_Gold = n["ThreeStarReward_Gold"].AsInt;
        DiminishedGoldReward_Min = n["DiminishedGoldReward_Min"].AsInt;
        FirstClearBonus_Jewel = n["FirstClearBonus_Jewel"].AsInt;
        JewelReward_Min = n["JewelReward_Min"].AsInt;
        ThreeStarReward_Jewel = n["ThreeStarReward_Jewel"].AsInt;
        JewelReward_Max = n["JewelReward_Max"].AsInt;

        ScoreThreshold_2Star = n["ScoreThreshold_2Star"].AsInt;
        ScoreThreshold_3Star = n["ScoreThreshold_3Star"].AsInt;
        StageClearBonus_Score = n["StageClearBonus_Score"].AsInt;
        PerRemainingSecond_Score = n["PerRemainingSecond_Score"].AsInt;
        PerPuzzleCleared_Score = n["PerPuzzleCleared_Score"].AsInt;
        PerfectGame_Score = n["PerfectGame_Score"].AsInt;
        Puzzle_Count = n["Puzzle_Count"].AsInt;
        PuzzleTime_Limit = n["PuzzleTime_Limit"].AsInt;
    }
}