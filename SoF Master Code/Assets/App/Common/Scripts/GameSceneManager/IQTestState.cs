using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSys
{
    public class IQTestState : GameSys.IState
    {
        private bool mIsDone = false;
        private bool mIsInit = false;
        private string mName = "";
        private int mTestNumber = 0;

        public bool IsDone { get { return mIsDone; } set { mIsDone = value; } }

        public bool IsInit { get { return mIsInit; } set { mIsInit = value; } }

        public string Name { get { return mName; } set { mName = value; } }

        public int TestNumber{ get { return mTestNumber; } set { mTestNumber = value; } }


        public void OnEnter()
        {
            mName = "IQ Test State";

            //go to game scene
            MainNavigationController.GoToScene("iqTest");
        }

        public void OnExit()
        {
            //Let the scene to control the flow of exiting
        }

        public void Update(float _dt)
        {

        }
    }
}