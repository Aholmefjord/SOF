using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameSys
{
    public class ImageDisplayState : GameSys.IState
    {
        public enum ImageDisplayType
        {
            STORYBOOK,
            IMAGE
        }

        private ImageDisplayType mImageType;
        private List<string> mImageURLList;

        private bool mIsDone = false;
        private bool mIsInit = false;

        private string mName;

        private int mStartChapter;
        private int mEndChapter;
        private bool mUseChapter;
        private ImageDisplayController mParentScript;

        public ImageDisplayType ImageType { get { return mImageType; } set { mImageType = value; } }
        public List<string> ImageURLList { get { return mImageURLList; } set { mImageURLList = value; } }

        public bool IsDone { get { return mIsDone; } set { mIsDone = value; } }
        public bool IsInit { get { return mIsInit; } set { mIsInit = value; } }

        public string Name { get { return mName; } set { mName = value; } }

        public int StartChapter { get { return mStartChapter; } set { mStartChapter = value; } }
        public int EndChapter { get { return mEndChapter; } set { mEndChapter = value; } }
        public bool UseChapter { get { return mUseChapter; } set { mUseChapter = value; } }
        public ImageDisplayController ParentScript { get { return mParentScript; } set { mParentScript = value; } }

        public void OnEnter()
        {
            mName = Constants.ImageDisplayStateName;

            //go to story scene
            //MainNavigationController.GoToScene("InGame Storybook");
            /*
            if (mImageType == ImageDisplayType.STORYBOOK)
                MainNavigationController.DoAssetBundleLoadLevel(Constants.INGAME_STORYBOOK_SCENE, "InGame ImageDisplay");
            else //*/
            if (mImageType == ImageDisplayType.IMAGE)
            {
                mImageURLList = new List<string>();
                for (int i = 1; i <= JULESTech.DataStore.Instance.lessonData.GetInt("image_url_total"); ++i)
                {
                    mImageURLList.Add(JULESTech.DataStore.Instance.lessonData.GetString("image_url_" + i));
                }
            }
            MainNavigationController.DoAssetBundleLoadLevel(Constants.INGAME_STORYBOOK_SCENE, "InGame ImageDisplay");
        }

        public void OnExit()
        {
            //Let the scene to control the flow of exiting
        }

        public void Update(float _dt)
        {

        }

        public void NextPage()
        {
            if (mParentScript != null)
            {
                mParentScript.NextButtonClicked();
            }
        }

        public void PreviousPage()
        {
            if (mParentScript != null)
            {
                mParentScript.PreviousButtonClicked();
            }
        }
    }
}