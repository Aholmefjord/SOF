using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;

public class tumbleCritterRequirement : MonoBehaviour {

	public static List<TumbleTroubleCritter> critterSettings;
	public static void Load()
	{
		//TextAsset t = Resources.Load<TextAsset>("tumbleCritterRequirement");
        JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.TUMBLE_TROUBLE_CONFIG, "tumbleCritterRequirement", (textAsset) => {
            JSONNode arr = JSONNode.Parse<JSONNode>(textAsset.text);
            critterSettings = new List<TumbleTroubleCritter>();
            foreach (JSONNode n in arr["tumbleCritterRequirement"].AsArray) {
                TumbleTroubleCritter mmms = new TumbleTroubleCritter(n);
                critterSettings.Add(mmms);
            }
            Debug.Log("Loaded Manta Level Count: " + critterSettings.Count);
        });
	}
	public static TumbleTroubleCritter getLevelDesign(int at)
	{
		return critterSettings[at];
	}
}

public class TumbleTroubleCritter
{
	public int IconType { get; set; }
	public List<int> levelRequirement { get; set; }


	public TumbleTroubleCritter(JSONNode n)
	{
		IconType = n["iconType"].AsInt;
		levelRequirement = new List<int>();
		foreach (JSONNode node in n["levelRequirement"].AsArray)
		{
			levelRequirement.Add(node.AsInt);
		}
	}
}

