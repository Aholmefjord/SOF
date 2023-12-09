using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelCompleteUI : BaseUI {

    public static LevelCompleteUI Current { get; private set; }
    public static void Open () {
        if (Current != null) {
            Current.Close ();
        }
        //GameObject go = CachedResources.Spawn ("ui/Level Complete Screen", false);
        JULESTech.Resources.AssetBundleManager.LoadAsset(Constants.TUMBLE_TROUBLE_SHARED, "Level Complete Screen", (loadedAsset)=> {
            GameObject go = loadedAsset as GameObject;
            go.transform.SetParent(UIRoot.transform);
            Current = go.GetComponent<LevelCompleteUI>();
        });
    }

    private int page = 1;

    protected override void Start () {
        base.Start ();

        canvasGroup.alpha = 0f;

        uiManager["Back Button"].OnClick (() => {
            Close ();
            DOTween.Sequence ()
                .AppendInterval (1f)
                .AppendCallback (() => LevelSelectUI.Open ())
                .SetUpdate (true);
        });

        string[] critterNames = new string[10] { "Horsie", "Krabby", "Clammy", "Nautilass", "Starry", "Dumbo", "Prawnie", "Shelly", "Puffy", "Hermie" };

        foreach (var critterName in critterNames) {
            var critterData = GameState.tsumProg.GetCritter (critterName);
            var critterGo = uiManager[string.Format ("{0} Container", critterName)];
            var critterUI = new UIManager (critterGo);

            critterUI["Lv Value"].SetText (critterData.level.ToString ());
            critterUI["Point Cap"].SetText (" / " + critterData.maxPoints.ToString ());

            var slider = critterUI["RedBar"].GetComponent<Slider> ();
            slider.maxValue = critterData.maxPoints;
            slider.value = 0;
            slider.DOValue (critterData.points, 2f)
                .SetUpdate (true)
                .OnUpdate (() => {
                    critterUI["Point Value"].SetText (slider.value.ToString ());
                });
        }

        uiManager["Arrow Left"].OnClick (() => {
            page = 1;
            uiManager["Page 1"].transform.DOLocalMove (Vector3.zero, 1f).SetUpdate (true);
            uiManager["Page 2"].transform.DOLocalMove (Vector3.right * Screen.width, 1f).SetUpdate (true);
        });
        uiManager["Arrow Right"].OnClick (() => {
            page = 2;
            uiManager["Page 1"].transform.DOLocalMove (Vector3.left * Screen.width, 1f).SetUpdate (true);
            uiManager["Page 2"].transform.DOLocalMove (Vector3.zero, 1f).SetUpdate (true);
        });

        uiManager["Page 1"].transform.localPosition = Vector3.zero;
        uiManager["Page 2"].transform.localPosition = Vector3.right * Screen.width;
    }

    Vector3 mousedownstart;
    private void Update () {
        uiManager["Arrow Left"].SetActive (page == 2);
        uiManager["Arrow Right"].SetActive (page == 1);
        uiManager["Page"].SetText (string.Format ("Page {0}/2", page));

        if (Input.GetMouseButtonDown(0))
        {
            mousedownstart = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Input.mousePosition.x < mousedownstart.x - 100 && page == 1) uiManager["Arrow Right"].GetComponent<Button>().onClick.Invoke();
            if (Input.mousePosition.x > mousedownstart.x + 100 && page == 2) uiManager["Arrow Left"].GetComponent<Button>().onClick.Invoke();
        }
    }

    protected override Sequence TweenIn {
        get {
            return DOTween.Sequence ()
                .Append (canvasGroup.DOFade (1f, 1f));
        }
    }

    protected override Sequence TweenOut {
        get {
            return DOTween.Sequence ()
                .Append (canvasGroup.DOFade (0f, 1f));
        }
    }
}
