using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

public class Cutscene_Dialogue_Manager : MonoBehaviour {

    [SerializeField]
    Cutscene_Manager CSMGR;

    // String of XML file to be read
    [SerializeField]
    string XMLFileName;

    [SerializeField]
    GameObject DialogueBorder;

    [SerializeField]
    UnityEngine.UI.Text DialogueText;

    private int p_Track_Limit;
    private int p_Track_Counter = 0;
    private int p_Track_Frame;
    private float p_Track_Time_Since_Last_Diag = 0.0f;

    //Data container
    private Cutscene_Serializer Container;

    // Use this for initialization
    void Start()
    {
        Container = Cutscene_Serializer.Load(Path.Combine("data/", XMLFileName));

        p_Track_Limit = Container.TotalSizeDialogue();

        Container.DebugPrintAllDialogue();

        // Capture the initial frame
        p_Track_Frame = CSMGR.CurrentFrame();
    }

    // Update is called once per frame
    void Update () {

        if (!CheckLimit())
        {
            PlayDialogues();
        }

        else
        {
            DialogueBorder.SetActive(false);
        }
	}

    public void PlayDialogues()
    {
        DialogueText.text = Container.DialogueList[p_Track_Counter].DialogueText;

        if(DialogueBorder.activeSelf == false && DialogueText.text.Length > 0)
            DialogueBorder.SetActive(true);
        // Set Dialogue Box off if there is no dialogue
        if (DialogueText.text.Length == 0)
            DialogueBorder.SetActive(false);

        float currentTime = CSMGR.TimeElapsed;

        // Time Based Transition
        if(Container.DialogueList[p_Track_Counter].TimeInSequence > 0.0f &&
           currentTime - p_Track_Time_Since_Last_Diag > Container.DialogueList[p_Track_Counter].TimeInSequence)
        {
            NextDialogue(currentTime);
        }

        // Automated Frame Transition
        else
        {
            if(p_Track_Frame < CSMGR.CurrentFrame())
            {
                NextDialogue(currentTime);
                p_Track_Frame = CSMGR.CurrentFrame();
            }
        }
    }

    private void NextDialogue(float timeElapsed)
    {
        ++p_Track_Counter;
        p_Track_Time_Since_Last_Diag = timeElapsed;
    }

    // Will run the program only if it is not over or at the limit
    private bool CheckLimit()
    {
        if (p_Track_Counter >= p_Track_Limit)
        {
            return true;
        }
        return false;
    }
}
