using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;
using JULESTech.Resources;

public static class PearlyLevelDesignHolder{
    public static List<PearlyLevelDesign> levels;
    public static void LoadDesigns()
    {
        if (levels != null) {
            //either loading or being loaded
            return;
        }
        AssetBundleManager.LoadAssetTextAsset(Constants.PEARLY_CONFIGS, "pearlyScoring", (textAsset) => {
            JSONNode a = JSONNode.Parse(textAsset.text);

            //Debug.Log(a["pearlyScoring"].ToString());
            foreach (JSONNode n in a["pearlyScoring"].AsArray) {
                PearlyLevelDesign pld = new PearlyLevelDesign();
                pld.threeStars = n["threeStars"].AsInt;
                pld.twoStars = n["twoStars"].AsInt;
                pld.orientation = n["orientation"];
                if (levels == null) {
                    levels = new List<PearlyLevelDesign>();
                }
                levels.Add(pld);
            }
        });
    }
}
public class PearlyLevelDesign : MonoBehaviour {
        public int threeStars { get; set; }
        public int twoStars { get; set; }
        public string orientation { get; set; }
}
