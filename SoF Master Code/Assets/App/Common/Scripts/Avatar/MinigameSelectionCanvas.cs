using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinigameSelectionCanvas : MonoBehaviour {

    void OnEnable()
    {
        SetupUI();
        MultiLanguage.getInstance().OnLanguageChanged += OnLanguageChangedCallback;
    }
    void OnDisable()
    {
        MultiLanguage.getInstance().OnLanguageChanged -= OnLanguageChangedCallback;
    }
	
    void SetupUI()
    {
        //Change images
        GameObject TableCanvas = gameObject;
        MultiLanguage.getInstance().applyImage(TableCanvas.FindChild("Infinite Crab Brothers").GetComponent<Image>(), "buddy_home_game_table_crab");
        MultiLanguage.getInstance().applyImage(TableCanvas.FindChild("MantaMatchMania").GetComponent<Image>(), "buddy_home_game_table_manta");
        MultiLanguage.getInstance().applyImage(TableCanvas.FindChild("PearlyWhirly").GetComponent<Image>(), "buddy_home_game_table_peraly");
        MultiLanguage.getInstance().applyImage(TableCanvas.FindChild("TumbleTrouble").GetComponent<Image>(), "buddy_home_game_table_tumble");
    }

    void OnLanguageChangedCallback(string previous, string current)
    {
        SetupUI();
    }
}
