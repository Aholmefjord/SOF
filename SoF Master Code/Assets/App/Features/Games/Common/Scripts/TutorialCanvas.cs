using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour {

    public string GameName = "";
    
    private int mTotalPages = 0;
    private int mCurrentPage = 0;

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

        MultiLanguage.getInstance().applyImage(gameObject.FindChild("TitleImage").GetComponent<Image>(), GameName + "_tutorial_title");

        if (mTotalPages == 0 || mTotalPages == 1)
        {
            DisableButton(Arrow.LEFT);
            DisableButton(Arrow.RIGHT);

            if(mTotalPages == 0)
                return;
        }

        mCurrentPage = 1;

        DisableButton(Arrow.LEFT);

        LoadImage();

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

        mCurrentPage += 1;
        LoadImage();

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

        mCurrentPage -= 1;
        LoadImage();

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
}