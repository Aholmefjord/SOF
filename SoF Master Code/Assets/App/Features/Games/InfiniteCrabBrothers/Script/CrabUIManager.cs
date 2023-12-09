using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class CrabUIManager : MonoBehaviour {
    public Image[] hearts;
    public Text upperFraction;
    public Text lowerFraction;
    public Text StageDisplay;
	// Use this for initialization
	
	// Update is called once per frame
	public void UpdateUI(int heartsLeft,int puzzlesAt, int maxPuzzles,int stage)
    {
        for(int i = heartsLeft; i < hearts.Length; i++)
        {
            hearts[i].enabled = false;
        }
        upperFraction.text = puzzlesAt + "";
        lowerFraction.text = maxPuzzles + "";
        StageDisplay.text = MultiLanguage.getInstance().getString("crab_brothers_game_stage") + (stage+1);
    }
}
