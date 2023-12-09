using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For use in mapNew, dynamically load the list of languages
/// </summary>
public class LocalizationChoiceInitialization : MonoBehaviour {
    [SerializeField]
    GameObject languageButtonPrefab;
    [SerializeField]
    NewMapController newMapCtrler;
    [SerializeField]
    GameObject parentPanel;
    [SerializeField]
    GameObject languageChoicePanel;
    [SerializeField]
    GameObject buttonContainer;
    
    readonly string analyticStringPreset = "LanguageSet{0}";
    MultiLanguageApplicator textApplicator;

    private void Awake()
    {
        MultiLanguage.getInstance().OnLanguageChanged += LanguageChangedCallback;
    }
    private void OnDestroy()
    {
        MultiLanguage.getInstance().OnLanguageChanged -= LanguageChangedCallback;
    }
    void LanguageChangedCallback(string prev, string curr)
    {
        SetupUI();
    }

    // Use this for initialization
    void Start ()
    {
        SetupUI();

        string[] langlist = MultiLanguage.getInstance().GetAvailableLanguages();
        for(int i = 0; i < langlist.Length; ++i) {
            GameObject tempButtonHolder = null; // this needs to be in forloop body so that the onclick callback will use the
            LanguageButton tempLangBn = null;

            tempButtonHolder = GameObject.Instantiate(languageButtonPrefab, buttonContainer.transform);
            tempButtonHolder.name = langlist[i];

            tempLangBn = tempButtonHolder.GetComponent<LanguageButton>();
            tempLangBn.Initialize();

            tempLangBn.AnalyticReportClass.Details = string.Format(analyticStringPreset, langlist[i]);
            tempLangBn.ButtonObject.onClick.AddListener(() => {
                parentPanel.SetActive(true);
                languageChoicePanel.SetActive(false);
                newMapCtrler.SwitchLanguage(tempButtonHolder.name);
                SetupUI();
            });
            try {
                MultiLanguage.getInstance().ApplyText(langlist[i], "localization_language_name", tempLangBn.TextObject);
            }catch(System.Exception e) {
                Debug.LogError(e.Message+": "+langlist[i]);
            }
        }
	}

    void SetupUI()
    {
        //Setup settings page
        if(textApplicator==null)
            textApplicator = new MultiLanguageApplicator(gameObject);

        textApplicator.ApplyText("SettingsText",            "map_new_settings_text");
        textApplicator.ApplyText("BuddyChangeAltPosText",   "map_new_settings_buddy_change_alt_pos_text");
        textApplicator.ApplyText("BuddyChangeText",         "map_new_settings_buddy_change_text");
        textApplicator.ApplyText("ChangePasswordAltPosText", "map_new_settings_change_password_alt_pos_text");
        textApplicator.ApplyText("LogoutAltPosText",        "map_new_settings_logout_text");
        textApplicator.ApplyText("ChangePasswordText",      "map_new_settings_change_password_text");
        textApplicator.ApplyText("SupportText",             "map_new_settings_support_text");
        textApplicator.ApplyText("AddChildText",            "map_new_settings_add_child_text");
        textApplicator.ApplyText("LogoutText",              "map_new_settings_logout_alt_text");
        textApplicator.ApplyText("ResetText",               "map_new_settings_reset_text");
        textApplicator.ApplyText("UnlockText",              "map_new_settings_unlock_text");
        textApplicator.ApplyText("Language Button/Text",    "map_new_language_title");

        MultiLanguage.getInstance().apply(languageChoicePanel.FindChild("Title"), "map_new_language_title");
    }
}
