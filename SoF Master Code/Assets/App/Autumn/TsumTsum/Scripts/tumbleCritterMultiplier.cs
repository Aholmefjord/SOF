using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;

public class tumbleCritterMultiplier : MonoBehaviour {

	public static List<TumbleTroubleCritterMultiplier> critterMultiplier;
	public static void Load()
	{
		//TextAsset t = Resources.Load<TextAsset>("tumbleCritterMultiplier");
        JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.TUMBLE_TROUBLE_CONFIG, "tumbleCritterMultiplier", (textAsset) => {
            JSONNode arr = JSONNode.Parse<JSONNode>(textAsset.text);
            critterMultiplier = new List<TumbleTroubleCritterMultiplier>();
            foreach (JSONNode n in arr["tumbleCritterMultiplier"].AsArray) {
                TumbleTroubleCritterMultiplier mmms = new TumbleTroubleCritterMultiplier(n);
                critterMultiplier.Add(mmms);
            }
        });
	}
	public static TumbleTroubleCritterMultiplier getLevelDesign(int at)
	{
		return critterMultiplier[at];
	}
}

public class TumbleTroubleCritterMultiplier
{
	public int IconType { get; set; }
	public List<float> ScoreMultiplier { get; set; }


	public TumbleTroubleCritterMultiplier(JSONNode n)
	{
		IconType = n["iconType"].AsInt;
		ScoreMultiplier = new List<float>();
		foreach (JSONNode node in n["ScoreMultiplier"].AsArray)
		{
			ScoreMultiplier.Add(node.AsFloat);
		}
	}
}
