using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour {
    QuickComponentAccess<Button> buttonObj;
    QuickComponentAccess<AnalyticsReportClass> analytic;
    Text textObj;

    public Button ButtonObject {
        get {
            return buttonObj.Value;
        }
    }
    public Text TextObject {
        get {
            return textObj;
        }
    }
    public AnalyticsReportClass AnalyticReportClass {
        get {
            return analytic.Value;
        }
    }

    public void Initialize()
    {
        buttonObj = new QuickComponentAccess<Button>(gameObject);
        analytic = new QuickComponentAccess<AnalyticsReportClass>(gameObject);
        textObj = GetComponentInChildren<Text>();
    }
}
