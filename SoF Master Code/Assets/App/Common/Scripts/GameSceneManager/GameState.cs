using AutumnInteractive.SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GameSys
{
    public class GameState : GameSys.IState
    {
        private bool mIsDone;
        private bool mIsInit;
        private string mName;

        private int mStartLevel;
        private int mEndLevel;
        private int mLevelCount;

        private string mGameData;
        private string mGameLevelData;
        
        private int mLoadingRequiredCount;

        public bool IsDone { get { return mIsDone;  } set { mIsDone = value; } }

        public bool IsInit { get { return mIsInit; } set { mIsInit = value; } }

        public string Name { get { return mName; } set { mName = value; } }

        public int startLevel { get { return mStartLevel; } set { mStartLevel = value; } }
        public int endLevel { get { return mEndLevel; } set { mEndLevel = value; } }
        public int levelCount { get { return mLevelCount; } set { mLevelCount = value; } }

        public string GameData { get { return mGameData;  } }
        public string GameLevelData { get { return mGameLevelData; } }

        public bool loadScene = true;
        public void OnEnter()
        {
            //Calculate ourself the level counts.
            mLevelCount = mEndLevel - mStartLevel + 1;
            
            LoadScene();
        }

        private void LoadScene()
        {
            if (loadScene == false)
                return;
            //go to game scene
            MainNavigationController.GoToSceneWithItemToLoad(JULESTech.DataStore.Instance.gameCommandData.GetString(Name), mLoadingRequiredCount);
        }

        public void OnExit()
        {
            //Let the scene to control the flow of exiting
            int i = 0;
            i++;

        }

        public void Update(float _dt)
        {
        }
    }
}
