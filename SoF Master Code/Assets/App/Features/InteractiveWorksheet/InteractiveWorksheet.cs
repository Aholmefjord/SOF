using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AutumnInteractive.SimpleJSON;
using System.Collections.Generic;
public class InteractiveWorksheet : MonoBehaviour {
    public List<bool> answersMade;
    public List<int> timetakenPerAnswer;
    public List<Button> buttons;
    public Text QuestionText;
    public Text NameText;
    public Text FunFactText;
    public Image HabitatImage;
    public Text CorrectIncorrect;

    public GameObject factObject;

    public List<TextAsset> assets;


    public List<Question> questions;
    public List<Animal> animalList;


    public TextAsset animals;


    int questionIndex = 0;
    int correctAnswer = 0;

    public List<Sprite> animalImages;
    public List<Sprite> habitatImages;

    // Use this for initialization
    int WorkSheetTaken = 0;

    public void Init(int i)
    {
        WorkSheetTaken = i;
        TextAsset t = assets[i];

        questions = new List<Question>();
        JSONNode c = JSONNode.Parse<JSONNode>(t.text);
        foreach (JSONNode node in c["question"].AsArray)
        {
            Question q = new Question();
            q.CorrectAnimalIndex = node["CorrectAnimalIndex"].AsInt;
            q.HabitatIndex = node["HabitatIndex"].AsInt;
            q.name = node["name"];
            q.possibleAnswerIndex = new List<int>();
            foreach(JSONNode nodeInner in node["possibleAnswerIndex"].AsArray)
            {
                q.possibleAnswerIndex.Add(nodeInner.AsInt);
            }
            questions.Add(q);
        }

        animalList = new List<Animal>();
        JSONNode m = JSONNode.Parse<JSONNode>(animals.text);   
        foreach (JSONNode node in m["animals"].AsArray)
        {
            Animal a = new Animal();
            a.imageIndex = node["imageIndex"].AsInt;
            a.funFact = node["funFact"];
            animalList.Add(a);
        }
        SelectNewQuestion();
    }


    void SelectNewQuestion()
    {
        correctAnswer = Random.Range(0, 3);
        string setText = "";
        for(int i = 0; i < buttons.Count; i++)
        {
            Image ii = buttons[i].GetComponent<Image>();
            ii.sprite = (i == correctAnswer) ?animalImages[animalList[questions[questionIndex].CorrectAnimalIndex].imageIndex] : animalImages[questions[questionIndex].possibleAnswerIndex[i]];
            if(i == correctAnswer)
            {
                setText = animalList[questions[questionIndex].CorrectAnimalIndex].funFact;
            }
        }
        HabitatImage.sprite = habitatImages[questions[questionIndex].HabitatIndex];
        FunFactText.text = setText;
        NameText.text = questions[questionIndex].name;
        QuestionText.text = "Question " + (questionIndex + 1);
    }
    // Update is called once per frame
    public void SelectAnswer(int answerIndex)
    {
        bool answerChosen = chooseAnswer(answerIndex);
        answersMade.Add(answerChosen);
        timetakenPerAnswer.Add(currTime);
        currTime = 0;
        CurrTimer = 0;
        SelectNewAnswer();
        CorrectIncorrect.text = (answerChosen) ? "Correct" : "Incorrect";
    }
    float CurrTimer = 0;
    int currTime = 0;
    // Update is called once per frame
    void Update()
    {
        CurrTimer += Time.deltaTime;
        if (CurrTimer > 1f)
        {
            CurrTimer = 0;
            currTime++;
        }
    }
    bool chooseAnswer(int i)
    {
        return i == correctAnswer;
    }
    void SelectNewAnswer()
    {
       factObject.SetActive(true);
    }
    public void CheckToSeeDisabled()
    {
        questionIndex++;
        if (questionIndex < questions.Count)
        {
            factObject.SetActive(true);
            SelectNewQuestion();
        }
        else
        {
            Upload();
            gameObject.SetActive(false);
        }
    }
    public void Upload()
    {
        switch (WorkSheetTaken)
        {
            case 0:
                GameState.me.worksheetOneResults = GetJSONAnswer();
                break;
            case 1:
                GameState.me.worksheetTwoResults = GetJSONAnswer();
                break;
            case 2:
                GameState.me.worksheetThreeResults = GetJSONAnswer();
                break;
            case 3:
                GameState.me.worksheetFourResults = GetJSONAnswer();
                break;
        }
    }
    public string GetJSONAnswer()
    {
        JSONClass n = new JSONClass();
        JSONArray ag = new JSONArray();
        JSONArray ttpa = new JSONArray();
        for (int i = 0; i < answersMade.Count; i++)
        {
            ag.Add(answersMade[i].ToString());
        }
        n.Add("answersgiven", ag);
        for (int i = 0; i < timetakenPerAnswer.Count; i++)
        {
            ttpa.Add(timetakenPerAnswer[i].ToString());
        }
        n.Add("timetakenPerAnswer", ttpa);
        return n.ToProperString();
    }
}

public class Question
{
    public int HabitatIndex;
    public int CorrectAnimalIndex;
    public string name;
    public List<int> possibleAnswerIndex;    
}

public class Animal
{
    public int imageIndex;
    public string funFact;
}
