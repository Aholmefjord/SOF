using UnityEngine;
using UnityEngine.UI;
using AutumnInteractive.SimpleJSON;
using System.Collections;
using System.Collections.Generic;
public class IQTest : MonoBehaviour {
    public IQTestCinematicEnabler iqce;
    public List<bool> answersMade;
    public List<int> timetakenPerAnswer;

    public List<Button> buttons;
    public Image questionImage;
    public List<char> answers;

    public Text questionText;
    
    public int timePlayed = 0;
    public int indexAt = 0;
    public int correctAnswers = 0;
    public int incorrectAnswers = 0;

    char[] answerLists = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h' };

    public GameObject finishCinematicObject;
    GameSys.IQTestState mState;
    // Use this for initialization
    private void Start()
    {
        //      if(finishCinematicObject != null)
        //        finishCinematicObject.SetActive(false);

        mState = (GameSys.IQTestState)GameSys.StateManager.Instance.GetFirstState();

        
		if (mState == null)
			return;

        if (!mState.Name.Equals("IQ Test State"))
            return;

        Start(mState.TestNumber);

    }

    private string getStringRepresentationOfIndex(int i)
    {
        if (i < 10) return "0" + i;
        else return i.ToString();
    }
    bool testing = false;

	public void Start (int i) {
        gameObject.SetActive(true);
        timePlayed = i;
        indexAt = 0;
        correctAnswers = 0;
        incorrectAnswers = 0;
        answersMade = new List<bool>();
        EnableAnswer(0);
	}
    float CurrTimer = 0;
    int currTime = 0;
	// Update is called once per frame
	void Update () {
        CurrTimer += Time.deltaTime;
        if(CurrTimer > 1f)
        {
            CurrTimer = 0;
            currTime++;
        }
	}
    public void AnswerQuestion(bool correct)
    {
        CurrTimer = 0;
        timetakenPerAnswer.Add(currTime);
        currTime = 0;
        answersMade.Add(correct);
        indexAt++;
        if (correct) correctAnswers += 1;
        else incorrectAnswers += 1;
        if(incorrectAnswers > 2 || indexAt > 29)
        {
            FinishTest();
        }else
        {
            EnableAnswer(indexAt);
        }
    }
    public void FinishTest()
    {
        UploadTest();
        if (iqce != null) iqce.FinishTest();
        if (finishCinematicObject != null)
        {
//            finishCinematicObject.SetActive(true);
            finishCinematicObject.GetComponent<CutsceneDialogueController>().TriggerContinue();
        }
        
        Destroy(this.gameObject);        
    }
    public void UploadTest()
    {
        string s = GetJSONAnswer();
        Debug.Log("Finished IQ Test with results: " + s.ToString());          
        switch (timePlayed)
        {
            case 1:
                Debug.Log("Time 1");
                GameState.me.iqTestOneResults = s;
                break;
            case 2:
                Debug.Log("Time 2");
                GameState.me.iqTestTwoResults = s;
                break;
            case 3:
                Debug.Log("Time 3");
                GameState.me.iqTestThreeResults = s;
                break;
        }
        GameState.me.Upload();
    }
    public void EnableAnswer(int i)
    {
       List<Sprite> images = getSpritesForQuestion(i);
       for(int ii = 0; ii < buttons.Count; ii++)
       {
            buttons[ii].GetComponent<Image>().sprite = images[ii];
            buttons[ii].GetComponent<IQTestButton>().isCorrect = false;
       }
       questionImage.sprite = images[images.Count - 1];
       Resources.UnloadUnusedAssets();
       buttons[getAnswerFromChar(answers[i])].GetComponent<IQTestButton>().isCorrect = true;
       questionText.text = "Question " + (i + 1);
    }
    public string GetJSONAnswer()
    {
        JSONClass n = new JSONClass();
        JSONArray ag = new JSONArray();
        JSONArray ttpa = new JSONArray();
        for(int i = 0; i < answersMade.Count; i++)
        {
            ag.Add(answersMade[i].ToString());
        }
        n.Add("answersgiven", ag);
        for (int i = 0; i < timetakenPerAnswer.Count; i++)
        {
            ttpa.Add(timetakenPerAnswer[i].ToString());
        }
        n.Add("timetakenPerAnswer", ttpa);
        n.Add("correctAnswers",correctAnswers.ToString());
        n.Add("incorrectAnswers",incorrectAnswers.ToString());
        return n.ToProperString();
    }
    public List<Sprite> currentAnswerSetLoaded;
    private List<Sprite> getSpritesForQuestion(int i)
    {
        List<Sprite> questionSet = new List<Sprite>();

        for (int j = 0; j < answerLists.Length; j++)
        {
            Sprite sp = Resources.Load<Sprite>(getStringRepresentationOfIndex(i+1) + "/" + answerLists[j]);
            Debug.Log("Loading: " + getStringRepresentationOfIndex(i+1) + "/" + answerLists[j]);
            questionSet.Add(sp);
        }
        Debug.Log("Loading Final Image: " + getStringRepresentationOfIndex(i+1) + "/" + "Q" + getStringRepresentationOfIndex(i+1) + "b");
        questionSet.Add(Resources.Load<Sprite>(getStringRepresentationOfIndex(i+1) + "/" + "Q" + getStringRepresentationOfIndex(i+1) + "b"));
        currentAnswerSetLoaded = questionSet;
        return questionSet;
    }
    int getAnswerFromChar(char c)
    {
        for(int i = 0; i < answerLists.Length; i++)
        {
            if (answerLists[i] == c) return i;
        }
        return -1;
    }

    public void GoToMainMenu()
    {
        MainNavigationController.GotoMainMenu();
    }
}
