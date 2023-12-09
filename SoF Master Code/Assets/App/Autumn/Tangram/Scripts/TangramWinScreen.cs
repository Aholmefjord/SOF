using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using System.Collections;
using JULESTech;
public class TangramWinScreen : BaseUI {
    public void GoToCinematic()
    {
        CutsceneDialogueController.isPartTwo = true;
        MainNavigationController.GoToCinematic("Cinematics_chpt4");
    }
    public static TangramWinScreen Current { get; private set; }
    private static GameObject thisGO;
	//Hieu let's design things so i don't have to keep on retyping the same variables all over the place
	//(Find in files for this line to see how many times we've retyped this)
	//Also i am typing this on the plane with no mouse :'(
	public static void Open(bool gameWon = true, int TotalScore = 20000, int HighestCombo = 8, int goldEarned = 0, int rGemEarned = 0, int uGemEarned = 0, bool loadBuddy = false) {
        if (Current != null) {
            Current.Close ();
        }
        //thisGO = CachedResources.Spawn("ui/Tangram Results Screen", false);
        CachedResources.Spawn(Constants.MANTA_MATCH_SHARED, "Tangram Results Screen", (loadedAsset) => {
            thisGO = loadedAsset;
            thisGO.transform.SetParent(UIRoot.transform);
            Current = thisGO.GetComponent<TangramWinScreen>();

            Current.GameWon = gameWon;
            Current.TotalScore = TotalScore;
            Current.HighestCombo = HighestCombo;
            Current.Highest = GameState.tangramProg.GetLevel(GameState.tangramProg.selectedLevel).pointsEarned;
            Current.Gold = goldEarned;
            Current.rGem = rGemEarned;
            Current.uGem = uGemEarned;
            Current.loadBuddy = loadBuddy;

            Current.SetupUI();

            GameState.me.inventory.Coins += goldEarned;
            GameState.me.inventory.Jewels += rGemEarned;
            GameState.me.inventory.Steel += uGemEarned;
            GameState.me.Upload();

            if (loadBuddy) {
                ScoreScreenLoadBuddyAvatar.LoadAvatar();
            }
        });
	}

    private bool GameWon = true;
    private int TotalScore = 17500;
    private int HighestCombo = 5000;
    private int Highest = 1500;
    private int Gold, rGem, uGem;
	private bool loadBuddy = false;
	public GameObject avatarParent;
	public GameObject avatarRawImage;
	public Button avatarButton;
	public Image hiFiveBubble;
	public Sprite[] hiFiveSpriteContainer;
	private int hiFiveCounter = 0;

	public static int TwoStarsThreshold, ThreeStarsThreshold;

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

		uiManager["Next Button"].SetActive(false);
		uiManager["Replay Button"].SetActive(false);
		uiManager["Back Button"].SetActive(false);
        
        /*
		uiManager["Back Button"].OnClick (() => {
			GameState.me.Upload();
			Close ();
            if (GameState.tangramProg.selectedLevel == 9 && Constants.uses_cinematics)
            {
                GoToCinematic();
            }
            else 
            {    
            TangramLevelSelectUI.Open ();
            }
        });

        uiManager["Replay Button"].OnClick (() => {
			GameState.me.Upload();
			Close ();
            if (GameState.tangramProg.selectedLevel == 9 && Constants.uses_cinematics)
            {
                GoToCinematic();
            }
            else
            {
                //SceneManager.LoadScene("Tangram");
                MainNavigationController.DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram");
            }
        });

        uiManager["Next Button"].OnClick (() => {
			GameState.me.Upload();
            GameSys.GameState gameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();
			Close ();
           if (GameState.tangramProg.selectedLevel == 9 && Constants.uses_cinematics)
            {
                GoToCinematic();
            }
            if (GameState.tangramProg.selectedLevel >= gameState.endLevel)
            {
                //if at the allowed end level
                //go to main menu
                MainNavigationController.GotoMainMenu();
            }
            else
            {
                GameState.tangramProg.selectedLevel++;
                //SceneManager.LoadScene("Tangram");
                MainNavigationController.DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram");
            }
        });
        //*/
        uiManager["Lose game Text"].SetAlpha (0);


        if (GameWon) {
            AnalyticsSys.Instance.Report(REPORTING_TYPE.GameResult, "MantaMatchMania Won Level:" + (GameState.tangramProg.selectedLevel + 1).ToString() + " Stars:" + ConvertScoresToStars(TotalScore).ToString()/* + " Score:" + TotalScore.ToString()*/);
            DOTween.Sequence ()
            .Append (uiManager["Screen Container"].transform.DOScale (0f, 0.15f).From ())       // Scale up the frame
            .AppendInterval (0.15f)
            .AppendCallback (() => {
                var starCount = ConvertScoresToStars (Highest);
				Debug.LogError("PlaySFX");
				AudioSystem.PlaySFX("buttons/JULES_1STAR_02");
				for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i)
				{
                    var star = uiManager["Stars Container"].transform.GetChild (i).gameObject;
                    star.SetActive (i < starCount);
                    star.transform.DOScale (0f, 0.2f).From ().SetEase (Ease.OutBounce);
                }
            })
            .Append (uiManager["Highest"].transform.DOScale (1f, 0.2f))
            .AppendInterval (0.8f)
            .AppendCallback (() => {
                uiManager["Total Score"].transform.localScale = Vector3.one * 3f;
                uiManager["Total Score"].GetComponent<CanvasGroup> ().alpha = 0f;
                uiManager["Total Score"].transform.DOScale (1f, 0.15f).SetEase (Ease.OutBounce);
                uiManager["Total Score"].GetComponent<CanvasGroup> ().DOFade (1f, 0.15f);
            })
            .AppendInterval (0.65f)
            .AppendCallback (() => {
                int scoreValue = 0;
                DOTween.To (() => scoreValue, x => scoreValue = x, TotalScore, 0.5f).OnUpdate (() => {
                    uiManager["Total Score Value"].SetText (scoreValue.ToString ("n0"));
                    var starCount = Mathf.Max (ConvertScoresToStars (Highest), ConvertScoresToStars (scoreValue));
                    for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i) {
                        var star = uiManager["Stars Container"].transform.GetChild (i).gameObject;
                        var active = i < starCount;
                        if (star.activeSelf != active) {
                            star.SetActive (active);
                            star.transform.DOScale (0f, 0.1f).From ().SetEase (Ease.OutBounce);
                        }
                    }
                });
            })
            .AppendInterval (1.05f)
            .AppendCallback (() => {
                uiManager["Highest Combo"].transform.localScale = Vector3.one * 3f;
                uiManager["Highest Combo"].GetComponent<CanvasGroup> ().alpha = 0f;
                uiManager["Highest Combo"].transform.DOScale (1f, 0.15f).SetEase (Ease.OutBounce);
                uiManager["Highest Combo"].GetComponent<CanvasGroup> ().DOFade (1f, 0.15f);
            })
            .AppendInterval (0.65f)
            .AppendCallback (() => {
                var temp = HighestCombo;
                DOTween.To (() => temp, x => temp = x, 0, 0.5f).OnUpdate (() => {
                    uiManager["Highest Combo Value"].SetText (temp.ToString ("n0"));
                    var extraTotalScore = TotalScore + HighestCombo - temp;
                    uiManager["Total Score Value"].SetText (extraTotalScore.ToString ("n0"));
                    var starCount = Mathf.Max (ConvertScoresToStars (Highest), ConvertScoresToStars (extraTotalScore));
                    for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i) {
                        var star = uiManager["Stars Container"].transform.GetChild (i).gameObject;
                        var active = i < starCount;
                        if (star.activeSelf != active) {
                            star.SetActive (active);
                            star.transform.DOScale (0f, 0.1f).From ().SetEase (Ease.OutBounce);
                        }
                    }
                });
            })
            .AppendInterval (0.8f)
            .Append (uiManager["Highest Combo"].GetComponent<CanvasGroup> ().DOFade (0f, 0.15f))
            .Join (uiManager["Total Score"].transform.DOLocalMoveX (0f, 0.5f))

            .AppendInterval (0.15f)
			.AppendCallback(() =>
			{
				Debug.LogError("PlaySFX");
				AudioSystem.PlaySFX("buttons/JULES_1STAR_02");
			})
			.Append (uiManager["Rewards"].GetComponent<CanvasGroup> ().DOFade (1f, 0.15f))
            .Join (uiManager["Rewards"].transform.DOScale (1f, 0.15f).SetEase (Ease.OutBounce))

            .AppendInterval (0.65f)
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
        //game lose animation
		{
            AnalyticsSys.Instance.Report(REPORTING_TYPE.GameResult, "MantaMatchMania Lose Level:" + (GameState.tangramProg.selectedLevel + 1).ToString());
            uiManager["Back Button"].SetActive(true);
			uiManager["Replay Button"].SetActive(true);
			uiManager["Next Button"].SetActive(false);
			uiManager["Rewards Container"].SetActive(false);
			DOTween.Sequence()
				.Append(uiManager["Screen Container"].transform.DOScale(0f, 0.15f).From())       // Scale up the frame
				.AppendInterval(0.15f)
				.AppendCallback(() =>
				{
                    var starCount = ConvertScoresToStars(Highest);
                    for (var i = 0; i < uiManager["Stars Container"].transform.childCount; ++i)
                    {
                        var star = uiManager["Stars Container"].transform.GetChild(i).gameObject;
                        star.SetActive(i < starCount);
                        star.transform.DOScale(0f, 0.2f).From().SetEase(Ease.OutBounce);
                    }
                })
				.Append(uiManager["Highest"].transform.DOScale(1f, 0.2f))
				.AppendInterval(0.3f)
				.AppendCallback(() =>
				{
					Debug.LogError("PlaySFX");
					AudioSystem.PlaySFX("buttons/JULES_CRAB_WRONG_02");
				})
				.Append(uiManager["Lose game Text"].transform.DOScale(1f, 0.15f).SetEase(Ease.OutBounce))
				.Join(uiManager["Lose game Text"].GetComponent<Graphic>().DOFade(1f, 0.15f))
				.AppendInterval(0.15f)
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

    public void SetupUI()
    {
        MultiLanguage.getInstance().apply(thisGO.FindChild("Total Score Text"), "win_screen_total_score");
        MultiLanguage.getInstance().apply(thisGO.FindChild("Highest Combo Text"), "manta_win_screen_highest_combo");
        MultiLanguage.getInstance().apply(thisGO.FindChild("Highest Score"), "win_screen_highest_score");
        MultiLanguage.getInstance().apply(thisGO.FindChild("Rewards Text"), "win_screen_rewards");
        MultiLanguage.getInstance().apply(thisGO.FindChild("Lose game Text"), "win_screen_lose_game_text");
    }

    public static int ConvertScoresToStars (int score) {
		//if (score > ThreeStarsThreshold) return 3;
		//if (score > TwoStarsThreshold) return 2;
		//if (score > 0) return 1;
		//return 0;
		if (score >= mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].ScoreThreshold_3Star)
			return 3;
		if (score >= mantaMatchDesign.levelSettings[GameState.tangramProg.selectedLevel].ScoreThreshold_2Star)
			return 2;
        if (score > 0)
            return 1;
        return 0;
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

    public void BackButtonPressed()
    {
        GameState.me.Upload();
        Close();
        if (GameState.tangramProg.selectedLevel == 9 && Constants.uses_cinematics) {
            GoToCinematic();
        } else {
            //TangramLevelSelectUI.Open();
            MainNavigationController.DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram Level Select");
        }
    
    }
    public void ReplayButtonPressed()
    {
        GameState.me.Upload();
        Close();
        if (GameState.tangramProg.selectedLevel == 9 && Constants.uses_cinematics) {
            GoToCinematic();
        } else {
            //SceneManager.LoadScene("Tangram");
            MainNavigationController.DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram");
        }
    }
    public void NextStageButtonPressed()
    {
        GameState.me.Upload();
        GameSys.GameState gameState = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();
        Close();
        if (GameState.tangramProg.selectedLevel == 9 && Constants.uses_cinematics) {
            GoToCinematic();
        }
        if (GameState.tangramProg.selectedLevel >= gameState.endLevel) {
            //if at the allowed end level
            //go to main menu
            MainNavigationController.GotoMainMenu();
        } else {
            GameState.tangramProg.selectedLevel++;
            MainNavigationController.DoAssetBundleLoadLevel(Constants.MANTA_MATCH_SCENES, "Tangram");
        }
    }
}
