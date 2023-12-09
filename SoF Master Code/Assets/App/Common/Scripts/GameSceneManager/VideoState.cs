using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameSys
{
    public class VideoState : GameSys.IState
    {
        private bool mIsDone = false;
        private bool mIsInit = false;
        private string mName = "";

        public bool IsDone { get { return mIsDone; } set { mIsDone = value;  } }

        public bool IsInit { get { return mIsInit; } set { mIsInit = value; } }

        public string Name { get { return mName; } set { mName = value; } }

        public void OnEnter()
        {
            //go to game scene
            MainNavigationController.GoToScene("CampaignVideoScene");
        }

        public void OnExit()
        {
            //Let the scene to control the flow of exiting
        }

        public void Update(float _dt)
        {

        }
        
        public void SetYoutubeVideo(string episode)
        {
            PlayerPrefs.SetString("EpisodeToPlay", episode);
            //our youtube video is currently on AWS
            PlayerPrefs.SetInt("IsYoutube", 0);
        }

        public void SetDoodleVideo(int number)
        {
			// HOWARD - Deprecated network code - TO REPLACE
//            Debug.Log("Has found server: " + (JulesBox.HAS_FOUND_SERVER ? 0 : 1));
//            Debug.Log("Finding episode: " + VideoURL.GetDoodleEpisodeLink(number - 1));

            PlayerPrefs.SetString("EpisodeToPlay", VideoURL.GetDoodleEpisodeLink(number - 1));
            
			//All video on AWS
            PlayerPrefs.SetInt("IsYoutube", 0);// JulesBox.HAS_FOUND_SERVER ? 0 : 1);
        }
    }
}
