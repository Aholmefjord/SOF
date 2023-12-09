using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SimpleJSON;
using UniRx;

public class Worksheet : MonoBehaviour {
	[SerializeField]
	bool bisIQTest;
	public delegate void dMyDel(int num);
	private delegate void dStartMissionDel();
	private dStartMissionDel dsmdDel;
	public dMyDel dmdDel;
	string sQText;
	int iScore;
	List<Sprite> lSpriteForQuestions = new List<Sprite>();
	List<List<Sprite>> lSpriteForAnswers = new List<List<Sprite>>();
	JSONClass jsJSONfile;
	List<List<int?>> lCorrectAnswer = new List<List<int?>>();
	int?[] iSelectedAnswer;
	int iCurrentQuestion;
	int iMoney;
	[SerializeField]
	int iMoneyIncrease;
	[SerializeField]
	Transform tButtonPanel;
	[SerializeField]
	GameObject goButton;
	[SerializeField]
	Image iImage;
	[SerializeField]
	Text tQuestion;
	[SerializeField]
	Transform tPrevNextPanel;
	[SerializeField]
	TextAsset taTextFile;
	[SerializeField]
	Text tResults;
	[SerializeField]
	Text tGold;
	[SerializeField]
	Text tQuestionNumber;
	[SerializeField]
	GridLayoutGroup glgButtons;
	[SerializeField]
	GameObject goNextQuestionButton;
	[SerializeField]
	Text tGoNextQuestionButtonText;
	[SerializeField]
	GameObject goStartMission;
	// Use this for initialization
	void Start () {
		if (bisIQTest == true)
		{
			dsmdDel = MainNavigationController.GoToCutScene;
			dmdDel = RegisterSelected;
			tPrevNextPanel.gameObject.SetActive (true);
			tGold.transform.parent.gameObject.SetActive (false);
			ActualStart ();
		}
		else
		{
			dsmdDel = Temp;
			dmdDel = toResults;
		}
	}

	public void ActualStart()
	{
		DestroyButtons (1, tButtonPanel);
		//Apparently, unity does not detach a child after it has been destroyed.
		tButtonPanel.DetachChildren ();
		iImage.gameObject.SetActive (true);
		jsJSONfile = JSONNode.Parse(taTextFile.text) as JSONClass;
		allocateValuesInJson();
		CreateButtons ();
		ChangeQuestion (0);
	}

	void allocateValuesInJson()
	{
		sQText = jsJSONfile ["Question"] [0] ["question"];
		tQuestion.text = sQText;
		for (int i = 0; i < jsJSONfile["QuestionImage"].Count; i++)
		{
			List<Sprite> tempList = new List<Sprite> ();
			string path = jsJSONfile ["QuestionImage"] [i] ["fileName"].Value;
			Sprite sp = Resources.Load (path, typeof(Sprite)) as Sprite;
			lSpriteForQuestions.Add (sp);
			List<int?> lCorrectAnswersOneQuestion = new List<int?> ();
			for (int j = 0; j < jsJSONfile["QuestionAnswer"][i].Count; j++)
			{
				path = jsJSONfile["QuestionAnswer"][i][j]["fileName"].Value;
				sp = Resources.Load(path, typeof(Sprite)) as Sprite;
				tempList.Add(sp);
				bool isCorrectAnswer = jsJSONfile ["QuestionAnswer"] [i] [j] ["answerIsCorrect"].AsBool;
				if (isCorrectAnswer == true)
				{
					lCorrectAnswersOneQuestion.Add (j);
				}
			}
			lCorrectAnswer.Add (lCorrectAnswersOneQuestion);
			lSpriteForAnswers.Add(tempList);
			glgButtons.cellSize = lSpriteForAnswers [0] [0].rect.size;
		}
		iSelectedAnswer = new int?[jsJSONfile["QuestionImage"].Count];
	}

	void CreateButtons()
	{
		for (int i = 0; i < lSpriteForAnswers[iCurrentQuestion].Count; i++)
		{
			GameObject go = Instantiate (goButton) as GameObject;
			go.GetComponent<AnswerButtons> ().iPosition = i;
			go.transform.SetParent (tButtonPanel, true);
			go.transform.localScale = Vector3.one;
		}
	}

	void DestroyButtons(int i, Transform t)
	{   
		if (i != 0)
		{
		//	print (i);
			Destroy (t.GetChild (i-1).gameObject);
			DestroyButtons (i - 1, t);
		}
	}

	void assignImagesToButton()
	{
		for (int i = 0; i < tButtonPanel.childCount; i++)
		{
			tButtonPanel.GetChild (i).GetComponent<Image>().sprite = lSpriteForAnswers[iCurrentQuestion][i];
		}
	}

	void RegisterSelected(int i)
	{
		DeselectButton (iSelectedAnswer[iCurrentQuestion].HasValue, iSelectedAnswer[iCurrentQuestion]);
		iSelectedAnswer [iCurrentQuestion] = i;
		SelectButton (iSelectedAnswer [iCurrentQuestion].HasValue, iSelectedAnswer [iCurrentQuestion]);
	}

	void DeselectButton(bool b, int? value)
	{
		if (b)
		{
			tButtonPanel.GetChild (value.Value).SendMessage ("ChangeisSelectedTo", false);
		}
	}

	void SelectButton(bool b, int? value)
	{
		if (b)
		{
			tButtonPanel.GetChild (value.Value).SendMessage ("ChangeisSelectedTo", true);
		}
	}

	public void ChangeQuestion(int i)
	{
        StringHelper.DebugPrint("ChangeQuestion");
        DeselectButton (iSelectedAnswer[iCurrentQuestion].HasValue, iSelectedAnswer[iCurrentQuestion]);
		iCurrentQuestion = Mathf.Max (iCurrentQuestion + i, 0);
		if (iCurrentQuestion >= lSpriteForAnswers.Count)
		{
			DestroyButtons (lSpriteForAnswers[iCurrentQuestion-1].Count, tButtonPanel);
			tButtonPanel.DetachChildren ();
			DestroyButtons (2, tPrevNextPanel);
			tPrevNextPanel.DetachChildren ();
			for (int j = 0; j < lSpriteForQuestions.Count; j++)
			{
				List<int?> l = lCorrectAnswer [j];
				if (l.Contains(iSelectedAnswer[j]))
				{
					iScore += (int)Mathf.Pow(2.0f,(float) j);
				}
			}
			tGold.text = "Gold Obtained:\n" + iMoney.ToString ();
			//tQuestion.text = iScore.ToString ();
			tQuestionNumber.text = "All Questions Cleared";
			tResults.text = iScore.ToString ();
			tResults.gameObject.SetActive (true);
			iImage.gameObject.SetActive (false);
			goStartMission.SetActive (true);

			// HOWARD - Deprecated network code - TO REPLACE
//            WWWForm form = new WWWForm();
//            form.AddField("user_id", GameState.me.username);
//            form.AddField("result", iScore);
//            AppServer.CreatePost("me/user/updateiq2", form)
//            .Subscribe(
//                x => print(x), // onSuccess
//                ex => AppServer.ErrorResponse(ex, "Error Submitting analytics") // onError
//            );
        }
		else
		{
			tQuestionNumber.text = "Question " + (iCurrentQuestion+1).ToString ();
			SelectButton (iSelectedAnswer [iCurrentQuestion].HasValue, iSelectedAnswer [iCurrentQuestion]);
			iImage.sprite = lSpriteForQuestions [iCurrentQuestion];
			assignImagesToButton ();
		}
	}

	void toResults(int i)
	{
		iSelectedAnswer [iCurrentQuestion] = i;
		tResults.gameObject.SetActive (true);
		goNextQuestionButton.SetActive (true);
		tButtonPanel.gameObject.SetActive (false);
		iImage.gameObject.SetActive (false);
		GivePoints (iCurrentQuestion);
		if (iCurrentQuestion+1 >= lSpriteForAnswers.Count)
		{
			tGoNextQuestionButtonText.text = "Results Screen";
		}
	}

	void GivePoints(int j)
	{
		List<int?> l = lCorrectAnswer [j];
		if (l.Contains(iSelectedAnswer[j]))
		{
			iMoney += iMoneyIncrease;
			tResults.text = "Correct";
		}
		else
		{
			tResults.text = "Wrong";
		}
		tGold.text = "Gold Obtained:" + iMoney.ToString ();
		//tQuestion.text = sQText + iMoney.ToString ();
	}

	public void goNextQuestion()
	{
		tResults.gameObject.SetActive (false);
		goNextQuestionButton.SetActive (false);
		tButtonPanel.gameObject.SetActive (true);
		iImage.gameObject.SetActive (true);
		ChangeQuestion (1);
	}

	public void StartMission()
	{
		dsmdDel ();
	}

	void Temp()
	{
		print ("TempFunction until I get a clearer picture");
	}

	public void StartMissionButtonClick()
	{
		MainNavigationController.GoToMap();
	}
}