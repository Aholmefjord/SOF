using UnityEngine;
using System.Collections;
using JULESTech;
public class UpgradeVerification : MonoBehaviour {
    public bool isUnlock;
    public BuildCamera bc;
    public BuildUpgradeUI buui;
	public GameObject InsufficientPanel;
    public int tier;
	// Use this for initialization
	public void performUpgrade()
    {
		if (buui.Unlock())
		{
            if (buui.isMisc)
            {
                bc.UnlockMisc(tier);
            }else { 
			    if (!isUnlock)
			    {
                    Debug.Log("Not unlocking");
				    bc.UpgradeFurniture(tier);
			    }
			    else
			    {
    				Debug.Log("UNLOCKING TIER: " + tier);
				    bc.UpgradeFurniture(tier);
			    }
            }
            buui.UpdateLevelAt();
            
        }
		else
		{
			InsufficientPanel.SetActive(true);
		}
    }
}
