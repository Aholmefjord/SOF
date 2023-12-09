using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BlankScreenUI : BaseUI {

    public static BlankScreenUI Current { get; private set; }
    public static void Open () {
        if (Current != null) {
            Current.Close ();
        }
        //GameObject go = CachedResources.Spawn ("ui/Blank Screen", false);
        CachedResources.Spawn(Constants.GAMES_SHARED, "Blank Screen", (loadedAsset) => {
            GameObject go = loadedAsset as GameObject;
            go = Instantiate(go);
            go.transform.SetParent(UIRoot.transform);
            Current = go.GetComponent<BlankScreenUI>();
        });
        /*
        JULESTech.Resources.AssetBundleManager.LoadAsset(Constants.GAMES_SHARED, "Blank Screen", (loadedAsset) => {
            GameObject go = loadedAsset as GameObject;
            go = Instantiate(go);
            go.transform.SetParent(UIRoot.transform);
            Current = go.GetComponent<BlankScreenUI>();
        });//*/
    }

	protected override void Start()
	{
		base.Start();
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = true;
	}

    protected override Sequence TweenOut {
        get {
            return DOTween.Sequence ().
                Append (canvasGroup.DOFade (0f, 1f));
        }
    }

}
