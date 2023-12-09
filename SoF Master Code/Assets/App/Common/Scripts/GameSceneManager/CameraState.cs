using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameSys
{
    public class CameraState : GameSys.IState
    {
        private bool mIsDone = false;
        private bool mIsInit = false;
        private string mName = "";

        public bool IsDone { get { return mIsDone; } set { mIsDone = value; } }

        public bool IsInit { get { return mIsInit; } set { mIsInit = value; } }

        public string Name { get { return mName; } set { mName = value; } }

        public void OnEnter()
        {
            mName = "Camera State";

            //go to game scene
            MainNavigationController.GoToScene("buddySelfie");
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