using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections;
using JULESTech;
public class WinScreenUI : BaseUI {//istumble

    public static WinScreenUI Current { get; private set; }
    private static GameObject thisGO;
    //Hieu let's design things so i don't have to keep on retyping the same variables all over the place
    //(Find in files for this line to see how many times we've retyped this)
    //Also i am typing this on the plane with no mouse :'(
    public void GoToCutscene()
    {
        CutsceneDialogueController.isPartTwo = true;
        MainNavigationController.GoToCinematic("Cinematics_chpt6");
    }
    public static void Open (bool gameWon, int TotalScore = 20000, int HighestCombo = 8, int goldEarned = 0, int rGemEarned = 0, int uGemEarned= 0, bool loadBuddy = false) {
        if (Current != null) {
            Current.Close ();
        }
        //thisGO = CachedResources.Spawn ("ui/Win Screen", false);
        CachedResources.Spawn(Constants.TUMBLE_TROUBLE_SHARED, "Win Screen", (loadedAsset) => {
            thisGO = loadedAsset as GameObject;
            thisGO.transform.SetParent(UIRoot.transform);
            Current = thisGO.GetComponent<WinScreenUI>();

            Current.gameWon = gameWon;
            Current.TotalScore = TotalScore;
            Current.HighestCombo = HighestCombo;
            Current.Highest = GameState.tsumProg.GetLevel(LevelDesignData.CurrentLevelID).pointsEarned;
            Current.Gold = goldEarned;
            Current.rGem = rGemEarned;
            Current.uGem = uGemEarned;
            Current.loadBuddy = loadBuddy;

            Current.SetupUI();

            GameState.me.inventory.Coins += goldEarned;
            GameState.me.inventory.Jewels += rGemEarned;
            GameState.me.inventory.StoneTablet += uGemEarned;

            GameState.me.Upload();

            if (loadBuddy) {
                ScoreScreenLoadBuddyAvatar.LoadAvatar();
            }
        });
	}


    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(thisGO.FindChild("Total Score Text"), "win_screen_total_score");
        MultiLanguage.getInstance().apply(thisGO.FindChild("Highest Combo Text"), "tsum_win_screen_highest_combo");
        MultiLanguage.getInstance().apply(thisGO.FindChild("Highest Score"), "win_screen_highest_score");
        MultiLanguage.getInstance().apply(thisGO.FindChild("Rewards Text"), "win_screen_rewards");
        MultiLanguage.getInstance().apply(thisGO.FindChild("Lose game Text"), "win_screen_lose_game_text");
    }

    public void HiFiveAnimation()
	{
        if (hiFiveBubble.gameObject.GetActive() == true)
        {
            avatarParent.transform.GetChild(0).GetComponent<Animator>().Play("HighFiveFinish");
            hiFiveBubble.gameObject.SetActive(false);
            AudioSystem.PlaySFX("actions/sfx_actions_voice_yay");
            DOTween.Sequence()
                    .AppendInterval(.95f)
                    .AppendCallback(() =>
                    {
                        AudioSystem.PlaySFX("buttons/JULES_1STAR_02");
                        uiManager["Next Button"].SetActive(true);
                        uiManager["Replay Button"].SetActive(true);
                        uiManager["Back Button"].SetActive(true);
                    //avatarParent.transform.GetChild(0).transform.DOScale(0f, 0.4f);
                    avatarParent.transform.GetChild(0).transform.DOLocalMoveY(-22f, 0.3f);
                    })
                    .AppendInterval(0.1f)
                    .Append(uiManager["Back Button"].GetComponent<Image>().DOFade(1f, 0.15f))
                    .Join(uiManager["Back Button"].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce))

                    .Join(uiManager["Replay Button"].GetComponent<Image>().DOFade(1f, 0.15f))
                    .Join(uiManager["Replay Button"].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce))

                    .Join(uiManager["Next Button"].GetComponent<Image>().DOFade(1f, 0.15f))
                    .Join(uiManager["Next Button"].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce))
                    .AppendInterval(0.4f)
                    .AppendCallback(() =>
                    {
                        avatarButton.gameObject.SetActive(false);
                        avatarRawImage.SetActive(false);
                    });
        }
	}

	private bool gameWon = true;
	private bool loadBuddy = false;
    private int TotalScore = 17500;
    private int HighestCombo = 5000;
    private int Highest = 1500;
    private int Gold, rGem, uGem;
	public GameObject avatarParent;
	public GameObject avatarRawImage;
	public Button avatarButton;
	public Image hiFiveBubble;
	public Sprite[] hiFiveSpriteContainer;
	private int hiFiveCounter = 0;

	protected override void Start () {
			avatarRawImage.SetActive(false);
		base.Start ();
        canvasGroup.alpha = 0f;

        // Turn off all stars
        for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i) {
            var star = uiManager["Stars Container"].transform.GetChild (i).gameObject;
            star.SetActive (false);
        }

        uiManager["Gold Value"].SetText (Gold.ToString ());
        uiManager["Gem Value"].SetText (rGem.ToString ());
		uiManager["uGem Value"].SetText(uGem.ToString());

        uiManager["Highest"].transform.localScale = Vector3.zero;
        uiManager["Highest Value"].SetText (Highest.ToString ("n0"));

        uiManager["Total Score Value"].SetText (string.Empty);
        uiManager["Total Score"].transform.localScale = Vector3.zero;

        uiManager["Highest Combo Value"].SetText (HighestCombo.ToString ("n0"));
        uiManager["Highest Combo"].transform.localScale = Vector3.zero;

        uiManager["Rewards"].GetComponent<CanvasGroup> ().alpha = 0f;
        uiManager["Rewards"].transform.localScale = Vector3.one * 3f;

        uiManager["Back Button"].GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
        uiManager["Back Button"].transform.localScale = Vector3.one * 3f;
        uiManager["Replay Button"].GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
        uiManager["Replay Button"].transform.localScale = Vector3.one * 3f;
        uiManager["Next Button"].GetComponent<Image> ().color = new Color (1f, 1f, 1f, 0f);
        uiManager["Next Button"].transform.localScale = Vector3.one * 3f;

        uiManager["Lose game Text"].SetAlpha(0);
        uiManager["Lose game Text"].transform.localScale = Vector3.one * 3f;

		uiManager["Next Button"].SetActive(false);
		uiManager["Replay Button"].SetActive(false);
		uiManager["Back Button"].SetActive(false);
        
		if (gameWon)
		{
            AnalyticsSys.Instance.Report(REPORTING_TYPE.GameResult, "TumbleTrouble Won Level:" + (GameState.tsumProg.selectedLevel + 1).ToString() + " Stars:" + ConvertScoresToStars(TotalScore)/* + " Score:" + TotalScore.ToString()*/);
            DOTween.Sequence()
				.Append(uiManager["Screen Container"].transform.DOScale(0f, 0.15f).From())       // Scale up the frame
				.AppendInterval(0.15f)
				.AppendCallback(() =>
				{
					var starCount = ConvertScoresToStars(Highest);
					AudioSystem.PlaySFX("buttons/JULES_1STAR_02");
					for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i)
					{
						var star = uiManager["Stars Container"].transform.GetChild(i).gameObject;
						star.SetActive(i < starCount);
						star.transform.DOScale(0f, 0.1f).From().SetEase(Ease.OutBounce);
					}
				})
				.Append(uiManager["Highest"].transform.DOScale(1f, 0.15f))
				.AppendInterval(0.15f)
				.AppendCallback(() =>
				{
					uiManager["Total Score"].transform.localScale = Vector3.one * 3f;
					uiManager["Total Score"].GetComponent<CanvasGroup>().alpha = 0f;
					uiManager["Total Score"].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce);
					uiManager["Total Score"].GetComponent<CanvasGroup>().DOFade(1f, 0.15f);
				})
				.AppendInterval(1.3f)
				.AppendCallback(() =>
				{
					int scoreValue = 0;
					int scoreValue2 = scoreValue;
					DOTween.To(() => scoreValue, x => scoreValue = x, TotalScore, 1.0f).OnUpdate(() =>
					{
						uiManager["Total Score Value"].SetText(scoreValue.ToString("n0"));
						if (scoreValue2 != scoreValue)
						{
							scoreValue2 = scoreValue;
							AudioSystem.PlaySFX("buttons/JULES_SETTINGS_BUTTON_02");
						}
						var starCount = Mathf.Max(ConvertScoresToStars(Highest), ConvertScoresToStars(scoreValue));
						for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i)
						{
							var star = uiManager["Stars Container"].transform.GetChild(i).gameObject;
							var active = i < starCount;
							if (star.activeSelf != active)
							{
								star.SetActive(active);
								star.transform.DOScale(0f, 0.1f).From().SetEase(Ease.OutBounce);
							}
						}
					});
				})
				.AppendInterval(1.05f)
				.AppendCallback(() =>
				{
					uiManager["Highest Combo"].transform.localScale = Vector3.one * 3f;
					uiManager["Highest Combo"].GetComponent<CanvasGroup>().alpha = 0f;
					uiManager["Highest Combo"].transform.DOScale(1f, 0.15f).SetEase(Ease.OutBounce);
					uiManager["Highest Combo"].GetComponent<CanvasGroup>().DOFade(1f, 0.3f);
				})
				.AppendInterval(0.65f)
				.AppendCallback(() =>
				{
					var temp = HighestCombo;
					var temp2 = temp;
					DOTween.To(() => temp, x => temp = x, 0, 0.3f).OnUpdate(() =>
					{
						uiManager["Highest Combo Value"].SetText(temp.ToString("n0"));
						var extraTotalScore = TotalScore + HighestCombo - temp;
						uiManager["Total Score Value"].SetText(extraTotalScore.ToString("n0"));
						if (temp2 != temp)
						{
							temp2 = temp;
							AudioSystem.PlaySFX("buttons/JULES_SETTINGS_BUTTON_02");
						}
						var starCount = Mathf.Max(ConvertScoresToStars(Highest), ConvertScoresToStars(extraTotalScore));
						for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i)
						{
							var star = uiManager["Stars Container"].transform.GetChild(i).gameObject;
							var active = i < starCount;
							if (star.activeSelf != active)
							{
								star.SetActive(active);
								star.transform.DOScale(0f, 0.1f).From().SetEase(Ease.OutBounce);
							}
						}
					});
				})
				.AppendInterval(0.8f)
				.Append(uiManager["Highest Combo"].GetComponent<CanvasGroup>().DOFade(0f, 0.15f))
				.Join(uiManager["Total Score"].transform.DOLocalMoveX(0f, 0.5f))

				.AppendInterval(0.15f)
				.AppendCallback(() =>
				{
					AudioSystem.PlaySFX("buttons/JULES_1STAR_02");
				})
				.Append(uiManager["Rewards"].GetComponent<CanvasGroup>().DOFade(1f, 0.15f))
				.Join(uiManager["Rewards"].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce))

				.AppendInterval(0.65f)
				.AppendCallback(() =>
				{
					if (loadBuddy)
					{
						try
						{
							avatarButton.interactable = true;
							avatarRawImage.SetActive(true);
							avatarParent.transform.GetChild(0).GetComponent<Animator>().Play("HighFive");
							//Debug.LogError("pos: " + avatarParent.transform.GetChild(0).transform.position.y);
							avatarParent.transform.GetChild(0).transform.DOLocalMoveY(-18, 1.0f);
						}
						catch (Exception e)
						{
							Debug.LogError("ERROR");
						}
					}
					else
					{
						AudioSystem.PlaySFX("buttons/JULES_1STAR_02");
						uiManager["Next Button"].SetActive(true);
						uiManager["Replay Button"].SetActive(true);
						uiManager["Back Button"].SetActive(true);
						DOTween.Sequence()
						.Append(uiManager["Back Button"].GetComponent<Image>().DOFade(1f, 0.15f))
						.Join(uiManager["Back Button"].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce))

						.Join(uiManager["Replay Button"].GetComponent<Image>().DOFade(1f, 0.15f))
						.Join(uiManager["Replay Button"].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce))

						.Join(uiManager["Next Button"].GetComponent<Image>().DOFade(1f, 0.15f))
						.Join(uiManager["Next Button"].transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce));
					}
				})
				.AppendInterval(1.5f)
				.AppendCallback(() =>
				{
					if (loadBuddy)
					{
						StartCoroutine(changeSprite());
						hiFiveBubble.gameObject.SetActive(true);
					}
				});





		} else
        {
            AnalyticsSys.Instance.Report(REPORTING_TYPE.GameResult, "TumbleTrouble Lose Level:" + (GameState.tsumProg.selectedLevel + 1).ToString());
            uiManager["Back Button"].SetActive(true);
			uiManager["Replay Button"].SetActive(true);
			uiManager["Next Button"].SetActive(false);
			uiManager["Rewards Container"].SetActive(false);
			DOTween.Sequence()
				.Append(uiManager["Screen Container"].transform.DOScale(0f, 0.3f).From())       // Scale up the frame
				//.Join(uiManager["Next Button"].transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce))
				//.Join(uiManager["Rewards Container"].transform.DOScale(0f, 0.3f).SetEase(Ease.OutBounce))
				.AppendInterval(0.15f)
				.AppendCallback(() =>
				{
					var starCount = ConvertScoresToStars(Highest);
					for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i)
					{
						Debug.LogError("StarC: " + i);
						var star = uiManager["Stars Container"].transform.GetChild(i).gameObject;
						star.SetActive(i < starCount);
						star.transform.DOScale(0f, 0.2f).From().SetEase(Ease.OutBounce);
					}
				})
				.Append(uiManager["Highest"].transform.DOScale(1f, 0.2f))
				.AppendInterval(0.5f)
				.AppendCallback(() =>
				{
					Debug.LogError("PlaySFX");
					AudioSystem.PlaySFX("buttons/JULES_CRAB_WRONG_02");
				})
				.Append(uiManager["Lose game Text"].transform.DOScale(1f, 0.15f).SetEase(Ease.OutBounce))
				.Join(uiManager["Lose game Text"].GetComponent<Graphic>().DOFade(1f, 0.15f))
				.AppendInterval(0.5f)
				.AppendCallback(() =>
				{
					Debug.LogError("PlaySFX");
					AudioSystem.PlaySFX("buttons/JULES_1STAR_02");
				})
				.Append(uiManager["Back Button"].GetComponent<Image>().DOFade(1f, 0.15f))
				.Join(uiManager["Back Button"].transform.DOScale(1f, 0.15f).SetEase(Ease.OutBounce))

				.Join(uiManager["Replay Button"].GetComponent<Image>().DOFade(1f, 0.15f))
				.Join(uiManager["Replay Button"].transform.DOScale(1f, 0.15f).SetEase(Ease.OutBounce));

		}
    }

    private int ConvertScoresToStars (int score) {
		//if (score >= LevelDesignData.GetData ("ScoreThreshold_3Star").AsInt) return 3;
		//if (score >= LevelDesignData.GetData ("ScoreThreshold_2Star").AsInt) return 2;
		//if (score > 0) return 1;
		//return 0;
		if (score >= tumbleTroubleDesign.levelSettings[GameState.tsumProg.selectedLevel].ScoreThreshold_3Star)
			return 3;
		if (score >= tumbleTroubleDesign.levelSettings[GameState.tsumProg.selectedLevel].ScoreThreshold_2Star)
			return 2;
        if (score > 0)
        {
            return 1;
        }
        return 0;
    }

    public void BackButtonPressed()
    {
        GameState.me.Upload();
        Close();
        if (GameState.tsumProg.selectedLevel == 9 && Constants.uses_cinematics) {
            GoToCutscene();
        } else {
            //LevelSelectUI.Open();
            TsumGame.GoToTsumTsumLevelSelect();
        }
    }
    public void ReplayButtonPressed()
    {
        GameState.me.Upload();
        Close();
        if (GameState.tsumProg.selectedLevel == 9 && Constants.uses_cinematics) {
            GoToCutscene();
        } else {
            //UnityEngine.SceneManagement.SceneManager.UnloadScene("Tsum");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Tsum");
            TsumGame.GotoTsumTsumGame();
        }
    }
    public void NextStageButtonPressed()
    {
        GameState.me.Upload();
        GameSys.GameState gameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();
        Close();
        if (GameState.tsumProg.selectedLevel == 9 && Constants.uses_cinematics) {
            GoToCutscene();
        } else if (GameState.tsumProg.selectedLevel >= gameState.endLevel) {
            //Go back to level select
            //MainNavigationController.GoToTumbleTroubleLevelSelect();
            TsumGame.GoToTsumTsumLevelSelect();
        } else {
            GameState.tsumProg.selectedLevel++;
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Tsum");
            TsumGame.GotoTsumTsumGame();
        }
    }

    protected override Sequence TweenIn {
        get {
            return DOTween.Sequence ().
                Append (canvasGroup.DOFade (1f, 1f));
        }
    }

    protected override Sequence TweenOut {
        get {
            return DOTween.Sequence ().
                Append (canvasGroup.DOFade (0f, 1f));
        }
    }
	IEnumerator changeSprite()
	{
		hiFiveCounter++;
		if (hiFiveCounter > hiFiveSpriteContainer.Length - 1)
			hiFiveCounter = 0;
		hiFiveBubble.sprite = hiFiveSpriteContainer[hiFiveCounter];
		yield return new WaitForSeconds(1);
		if (hiFiveBubble.IsActive())
			StartCoroutine(changeSprite());
	}
}
