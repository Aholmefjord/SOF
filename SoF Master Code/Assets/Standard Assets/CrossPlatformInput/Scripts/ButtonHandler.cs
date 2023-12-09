using System;
using UnityEngine;

namespace UnityStandardAssets.CrossPlatformInput
{
    public class ButtonHandler : MonoBehaviour
    {
		public delegate void ButtonDownAction();
		public event ButtonDownAction OnButtonDown;
        public string Name;

		//public bool enabledButton = true;

        void OnEnable()
        {

        }

        public void SetDownState()
        {
			if (enabled)
			{
				CrossPlatformInputManager.SetButtonDown(Name);
				if (OnButtonDown != null)
					OnButtonDown();
			}
        }


        public void SetUpState()
        {
			if(enabled)
				CrossPlatformInputManager.SetButtonUp(Name);
        }


        public void SetAxisPositiveState()
        {
			if (enabled)
				CrossPlatformInputManager.SetAxisPositive(Name);
        }


        public void SetAxisNeutralState()
        {
			if (enabled)
				CrossPlatformInputManager.SetAxisZero(Name);
        }


        public void SetAxisNegativeState()
        {
			if (enabled)
				CrossPlatformInputManager.SetAxisNegative(Name);
        }
    }
}
