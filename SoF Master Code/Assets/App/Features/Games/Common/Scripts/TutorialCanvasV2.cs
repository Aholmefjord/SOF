using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvasV2 : MonoBehaviour
{
    public string GameName = "";
    int mTotalPages = 0;
    int mCurrentPage = 0;

    int num;

    bool mIsSetupDone = false;

    enum Arrow
    {
        LEFT,
        RIGHT
    }


    private void Awake()
    {
        //Setup
        if (mIsSetupDone)
            return;

        mIsSetupDone = true;

        //Hardcode tutorial pages for now

        if (GameName.Equals("chomp"))
        {
            mTotalPages = 4;
        }
        else if (GameName.Equals("crab"))
        {
            mTotalPages = 4;
        }
        else if (GameName.Equals("manta"))
        {
            mTotalPages = 4;
        }
        else if (GameName.Equals("pearly"))
        {
            mTotalPages = 5;
        }
        else if (GameName.Equals("tsum_tsum"))
        {
            mTotalPages = 4;
        }
        else if (GameName.Equals("tako"))
        {
            mTotalPages = 3;
        }

        //MultiLanguage.getInstance().applyImage(gameObject.FindChild("TitleImage").GetComponent<Image>(), GameName + "_tutorial_title");

        if (mTotalPages == 0 || mTotalPages == 1)
        {
            DisableButton(Arrow.LEFT);
            DisableButton(Arrow.RIGHT);

            if (mTotalPages == 0)
                return;
        }

        mCurrentPage = 1;

        DisableButton(Arrow.LEFT);

        LoadImage();
        LoadTutorial();

    }

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadImage()
    {
        MultiLanguage.getInstance().applyImage(gameObject.FindChild("ContentImage").GetComponent<Image>(), GameName + "_tutorial_content_" + mCurrentPage);
    }

    public void NextPage()
    {
        if (mCurrentPage >= mTotalPages)
        {
            return;
        }
        UnloadTutorialSpecialCases();
        mCurrentPage += 1;
        LoadImage();
        LoadTutorial();

        EnableButton(Arrow.LEFT);

        if (mCurrentPage >= mTotalPages)
        {
            mCurrentPage = mTotalPages;
            DisableButton(Arrow.RIGHT);
            return;
        }
    }

    public void PreviosuPage()
    {
        if (mCurrentPage <= 1)
        {
            return;
        }
        UnloadTutorialSpecialCases();
        mCurrentPage -= 1;
        LoadImage();
        LoadTutorial();

        EnableButton(Arrow.RIGHT);

        if (mCurrentPage <= 1)
        {
            mCurrentPage = 1;
            DisableButton(Arrow.LEFT);
            return;
        }
    }

    private void DisableButton(Arrow _arrow)
    {
        gameObject.FindChild(_arrow == Arrow.LEFT ? "LeftButton" : "RightButton").GetComponent<Button>().interactable = false;
    }

    private void EnableButton(Arrow _arrow)
    {
        gameObject.FindChild(_arrow == Arrow.LEFT ? "LeftButton" : "RightButton").GetComponent<Button>().interactable = true;
    }

    private void LoadTitle()
    {
        MultiLanguage.getInstance().apply(gameObject.FindChild("TitleText"), GameName + "_tutorial_title");
    }

    private void LoadDescriptionText()
    {
        MultiLanguage.getInstance().apply(gameObject.FindChild("DescriptionsText"), GameName + "_tutorial_page_"+ mCurrentPage +"_description");
    }

    private void LoadSecondaryDescriptionText()
    {
        MultiLanguage.getInstance().apply(gameObject.FindChild("SecondaryDescriptionsText"), GameName + "_tutorial_page_" + mCurrentPage + "_description_secondary");
    }

    private void LoadCloseButton()
    {
        MultiLanguage.getInstance().apply(gameObject.FindChild("CloseButton").gameObject.FindChild("Text"), "tutorial_close_button");
    }

    private void LoadTutorialSpecialCases()
    {
        if (gameObject.FindChild(GameName + "Specials") == null)
        {
            return;
        }
        if (gameObject.FindChild(GameName + "Specials").gameObject.FindChild(GameName + "_tutorial_page_" + mCurrentPage + "_description_special") == null)
        {
            return;
        }
        gameObject.FindChild(GameName + "Specials").SetActive(true);
        gameObject.FindChild(GameName + "Specials").gameObject.FindChild(GameName+"_tutorial_page_"+mCurrentPage+"_description_special").SetActive(true);

        MultiLanguage.getInstance().apply(gameObject.FindChild(GameName + "Specials").gameObject.FindChild(GameName + "_tutorial_page_" + mCurrentPage + "_description_special").gameObject.FindChild("SpecialDescriptionsText1"),
                                            GameName + "_tutorial_page_" + mCurrentPage + "_description_special_1");
        MultiLanguage.getInstance().apply(gameObject.FindChild(GameName + "Specials").gameObject.FindChild(GameName + "_tutorial_page_" + mCurrentPage + "_description_special").gameObject.FindChild("SpecialDescriptionsText2"),
                                            GameName + "_tutorial_page_" + mCurrentPage + "_description_special_2");
    }

    private void UnloadTutorialSpecialCases()
    {
        if(gameObject.FindChild(GameName + "Specials") == null)
        {
            return;
        }
        if (gameObject.FindChild(GameName + "Specials").gameObject.FindChild(GameName + "_tutorial_page_" + mCurrentPage + "_description_special") == null)
        {
            return;
        }

        gameObject.FindChild(GameName + "Specials").SetActive(false);
        gameObject.FindChild(GameName + "Specials").gameObject.FindChild(GameName + "_tutorial_page_" + mCurrentPage + "_description_special").SetActive(false);

        MultiLanguage.getInstance().apply(gameObject.FindChild(GameName + "Specials").gameObject.FindChild(GameName + "_tutorial_page_" + mCurrentPage + "_description_special").gameObject.FindChild("SpecialDescriptionsText1"),
                                            GameName + "_tutorial_page_" + mCurrentPage + "_description_special_1");
        MultiLanguage.getInstance().apply(gameObject.FindChild(GameName + "Specials").gameObject.FindChild(GameName + "_tutorial_page_" + mCurrentPage + "_description_special").gameObject.FindChild("SpecialDescriptionsText2"),
                                            GameName + "_tutorial_page_" + mCurrentPage + "_description_special_2");
    }

    public virtual void LoadTutorial()
    {
        LoadTitle();
        LoadDescriptionText();
        LoadSecondaryDescriptionText();
        LoadTutorialSpecialCases();
        LoadCloseButton();
    }
}

//public class PeralyTut: TutorialCanvasV2
//{
//    public override void LoadTutorial()
//    {
//        base.LoadTutorial();
//    }
//}