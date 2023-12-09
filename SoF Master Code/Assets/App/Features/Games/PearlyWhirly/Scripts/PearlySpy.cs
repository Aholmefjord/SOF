using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JULESTech;

public class PearlySpy {

    private static PearlySpy instance = null;

    public static PearlySpy Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PearlySpy();
                instance.Init();
            }

            return instance;
        }
    }

    public float TotalTime;
    public int TotalAttempts;
    public float AttemptTime;
    public List<string> ButtonSequence = new List<string>();
    public List<string> AttemptSequence = new List<string>();
    bool PlanningMode;

    public int ActionsToComplete = 0;
    int TotalClearUsed = 0;
    int TotalBackUsed = 0;

	// Use this for initialization
	public void Init() {
        TotalAttempts = 1;
        TotalTime = 0;
        AttemptTime = 0;
        PlanningMode = true;
        ActionsToComplete = 0;
        TotalClearUsed = 0;
        TotalBackUsed = 0;
	}
	
	// Update is called once per frame
	public void Update (float _dt) {
		if(PlanningMode == true)
        {
            AttemptTime += _dt;
        }
	}

    public void LeftPressed()
    {
        ButtonSequence.Add("Left");
        AttemptSequence.Add("Left");
    }
    public void RightPressed()
    {
        ButtonSequence.Add("Right");
        AttemptSequence.Add("Right");
    }
    public void BackPressed()
    {
        ButtonSequence.Add("Back");
        if (AttemptSequence.Count > 0)
        {
            AttemptSequence.RemoveAt(AttemptSequence.Count - 1);
        }

        TotalBackUsed++;
    }
    public void ClearPressed()
    {
        ButtonSequence.Add("Clear");
        AttemptSequence.Clear();
        TotalClearUsed++;
    }
    public void SendResults(bool lose, bool quitGame = false)
    {   
        /*
        string buttonSeqStr = "<";
        for(int i = 0; i < ButtonSequence.Count; ++i)
        {
            buttonSeqStr += " " + ButtonSequence[i] + " ";
        }
        buttonSeqStr += ">";
        ButtonSequence.Clear();

        string attemptSeqStr = "<";
        for (int i = 0; i < AttemptSequence.Count; ++i)
        {
            attemptSeqStr += " " + AttemptSequence[i] + " ";
        }
        attemptSeqStr += ">";

        if (lose == true)
        {
            AnalyticsSys.Instance.Report(REPORTING_TYPE.PearlyWhirlyData, "Level " + (GameState.pearlyProg.selectedLevel + 1).ToString() + ", Lose, Attempt: " + TotalAttempts + ", AttemptSequence: " + attemptSeqStr + ", ButtonSequence: " + buttonSeqStr + ", AttemptTime: " + AttemptTime + ", OverallTime: " + TotalTime);
            PlanningMode = true;
            AttemptTime = 0;
            ++TotalAttempts;
        }
        else
        {
            AnalyticsSys.Instance.Report(REPORTING_TYPE.PearlyWhirlyData, "Level " + (GameState.pearlyProg.selectedLevel + 1).ToString() + ", Win, Attempt: " + TotalAttempts + ", AttemptSequence: " + attemptSeqStr + ", ButtonSequence: " + buttonSeqStr + ", AttemptTime: " + AttemptTime + ", OverallTime: " + TotalTime);
        }
        */

        string reportString = "";
        //<Level>
        reportString += (GameState.pearlyProg.selectedLevel + 1).ToString();
        //reportString += ">";

        //<ActionsToComplete>
        reportString += ",";
        reportString += ActionsToComplete.ToString();
        //reportString += ">";

        //<Status>
        reportString += ",";
        reportString += lose ? (quitGame ? "2":"0"):"1";
        //reportString += ">";

        //<Attemp>
        reportString += ",";
        reportString += TotalAttempts.ToString();
        //reportString += ">";

        //<SequenceCount>
        reportString += ",";
        reportString += AttemptSequence.Count.ToString();
        //reportString += ">";

        //<ClearedSequenceCount>
        reportString += ",";
        reportString += lose ? "0":AttemptSequence.Count.ToString();
        //reportString += ">";

        //<Use Of Clear Functions>
        reportString += ",";
        reportString += TotalClearUsed.ToString();
        //reportString += ">";

        //<Use Of Back Functions>
        reportString += ",";
        reportString += TotalBackUsed.ToString();
        //reportString += ">";

        //<ActionCount>
        reportString += ",";
        reportString += ButtonSequence.Count.ToString();
        //reportString += ">";

        //<AttempTime>
        reportString += ",";
        reportString += AttemptTime.ToString();
        //reportString += ">";

        //<OverallTime>
        reportString += ",";
        reportString += TotalTime.ToString();
        //reportString += ">";

        //<Decisions per second>
        reportString += ",";
        reportString += ((float)(ButtonSequence.Count) / AttemptTime).ToString();
        //reportString += ">";

        Debug.Log(reportString);
        if (lose)
        {
            PlanningMode = true;
            AttemptTime = 0;
            ++TotalAttempts;
        }
        else
        {
            TotalAttempts = 0;
        }

        ButtonSequence.Clear();
        AttemptSequence.Clear();
        TotalClearUsed = 0;
        TotalBackUsed = 0;

        AnalyticsSys.Instance.Report(REPORTING_TYPE.PearlyWhirlyData, reportString);

    }
    public void StartPressed()
    {
        PlanningMode = false;
        TotalTime += AttemptTime;
    }

    public void QuitGame()
    {
        SendResults(true, true);
        
        /*
        string buttonSeqStr = "<";
        for (int i = 0; i < ButtonSequence.Count; ++i)
        {
            buttonSeqStr += " " + ButtonSequence[i] + " ";
        }
        buttonSeqStr += ">";
        ButtonSequence.Clear();

        string attemptSeqStr = "<";
        for (int i = 0; i < AttemptSequence.Count; ++i)
        {
            attemptSeqStr += " " + AttemptSequence[i] + " ";
        }
        attemptSeqStr += ">";
        AttemptSequence.Clear();

        AnalyticsSys.Instance.Report(REPORTING_TYPE.PearlyWhirlyData, "Level " + (GameState.pearlyProg.selectedLevel + 1).ToString() + ", Quit, Attempt: " + TotalAttempts + ", AttemptSequence: " + attemptSeqStr + ", ButtonSequence: " + buttonSeqStr + ", AttemptTime: " + AttemptTime + ", OverallTime: " + TotalTime);
        */
    }
}
