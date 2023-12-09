using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using DG.Tweening;

public class StageClearUI : BaseUI {

    private float animationTime;
    private static GameObject thisGO;

    public static StageClearUI Current { get; private set; }
    public static void Open (bool stageWin = true, float animationTime = 3.0f, bool continueStage = false) {
        if (Current != null) {
            Current.Close ();
        }
        //thisGO = CachedResources.Spawn ("ui/Stage Clear UI", false);
        CachedResources.Spawn(Constants.MANTA_MATCH_SHARED, "Stage Clear UI", (loadedAsset) => {
            thisGO = loadedAsset;
            thisGO.transform.SetParent(UIRoot.transform);
            Current = thisGO.GetComponent<StageClearUI>();

            Current.stageWin = stageWin;
            Current.animationTime = animationTime;
            Current.continueStage = continueStage;
        });
    }
    [SerializeField]
    GameObject bigExplosionPrefab;

    private bool stageWin = true;
    private bool continueStage = false;

    protected override void Start () {
        base.Start ();

        SetupUI();

        canvasGroup.alpha = 0f;

        uiManager["Stage Clear"].SetActive (!continueStage && stageWin);
        uiManager["Stage Failed"].SetActive (!continueStage && !stageWin);
        uiManager["Stage Continue"].SetActive(continueStage);
        
        if (continueStage)
        {
            //random continue stage text display.
            int random = Random.Range(1, 6);
            uiManager["Stage Continue"].FindChild("Text").SetText(MultiLanguage.getInstance().getString("manta_game_stage_continue_text_" + random));
            uiManager["Stage Continue"].FindChild("Text").GetComponent<Text>().fontSize = MultiLanguage.getInstance().getFontSize("manta_game_stage_continue_text_" + random);
        }

        //var go = CachedResources.Spawn (stageWin ? "BiggestExplosion" : "BiggestExplosion-Lost");
        if (stageWin) CachedResources.Spawn(bigExplosionPrefab);

        DOTween.Sequence ()
            .AppendInterval (animationTime)
            .AppendCallback (Close);
    }

    public void SetupUI()
    {
        MultiLanguageApplicator textProc = new MultiLanguageApplicator(thisGO);
        textProc.ApplyText("Stage Clear/Stage Clear Text/Stage"     , "manta_game_stage_clear_stage_text");
        textProc.ApplyText("Stage Clear/Stage Clear Text/Clear"     , "manta_game_stage_clear_clear_text");
        textProc.ApplyText("Stage Failed/Stage Clear Text/Stage"    , "manta_game_stage_failed_stage_text");
        textProc.ApplyText("Stage Failed/Stage Clear Text/Failed"     , "manta_game_stage_failed_failed_text");
        textProc.ApplyText("Stage Continue/Stage Continue Text/Text", "manta_game_stage_continue");
    }

    protected override Sequence TweenIn {
        get {
            return DOTween.Sequence ()
                .Append (canvasGroup.DOFade (1f, 0.5f))
                .Join (uiManager["Screen Container"].transform.DOScale (3f, 0.5f).From ().SetEase (Ease.OutBack));
        }
    }

    protected override Sequence TweenOut {
        get {
            return DOTween.Sequence ()
                .Append (canvasGroup.DOFade (0f, 1f))
                .Join (uiManager["Screen Container"].transform.DOScale (3f, 0.5f).SetEase (Ease.InBack));
        }
    }


}
