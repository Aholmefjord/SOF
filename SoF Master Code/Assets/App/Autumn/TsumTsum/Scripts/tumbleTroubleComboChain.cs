using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;

public class tumbleTroubleComboChain : MonoBehaviour {

	public static List<TumbleComboChain> comboChain;
	public static void Load()
	{
		//TextAsset t = Resources.Load<TextAsset>("tumbleTroubleComboChain");
        JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.TUMBLE_TROUBLE_CONFIG, "tumbleTroubleComboChain", (textAsset) => {
            JSONNode arr = JSONNode.Parse<JSONNode>(textAsset.text);
            comboChain = new List<TumbleComboChain>();
            foreach (JSONNode n in arr["comboChain"].AsArray) {
                TumbleComboChain mmms = new TumbleComboChain(n);
                comboChain.Add(mmms);
            }
        });
	}
	public static TumbleComboChain getLevelDesign(int at)
	{
		return comboChain[at];
	}
}


public class TumbleComboChain
{
	public int ComboChainMin{ get; set; }
	public int ComboChainMax { get; set; }

	public TumbleComboChain(JSONNode n)
	{
		ComboChainMin = n["comboChainBonusMin"].AsInt;
		ComboChainMax = n["comboChainBonusMax"].AsInt;
	}
}
