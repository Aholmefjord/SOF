using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameSceneManager {

    #region Content Related Command
    private class ContentGameCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            //Hmmmm, since it's possible we will enter this state while in another state
            //I think we will always clear all states
            GameSys.StateManager.Instance.ClearAllStates();

            //Create game state
            GameSys.GameState gameState = new GameSys.GameState();
            gameState.startLevel = JULESTech.DataStore.Instance.lessonData.GetInt("game_start_level");
            gameState.endLevel = JULESTech.DataStore.Instance.lessonData.GetInt("game_end_level");
            gameState.Name = JULESTech.DataStore.Instance.lessonData.GetString("game_name");

            Debug.Log("game scene name = " + JULESTech.DataStore.Instance.lessonData.GetString("game_name"));

            //Add state
            GameSys.StateManager.Instance.AddFront(gameState);
        }
    }

    private class ContentVideoCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            GameSys.StateManager.Instance.ClearAllStates();

            //Create vidoe state
            GameSys.VideoState videoState = new GameSys.VideoState();

            videoState.Name = JULESTech.DataStore.Instance.lessonData.GetString("video_name");
            string type = JULESTech.DataStore.Instance.lessonData.GetString("video_type");
            string path = JULESTech.DataStore.Instance.lessonData.GetString("video_path");

            if (type.Equals("youtube"))
            {
                videoState.SetYoutubeVideo(path);
            }
            else if (type.Equals("doodle"))
            {
                videoState.SetDoodleVideo(int.Parse(path));
            }
            
            //Add state
            GameSys.StateManager.Instance.AddFront(videoState);
        }
    }

    private class ContentCameraCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            GameSys.StateManager.Instance.ClearAllStates();

            GameSys.CameraState cameraState = new GameSys.CameraState();

            GameSys.StateManager.Instance.AddFront(cameraState);
        }
    }

    private class ContentIQTestCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            GameSys.StateManager.Instance.ClearAllStates();

            GameSys.IQTestState iqTestState = new GameSys.IQTestState();

            iqTestState.TestNumber = JULESTech.DataStore.Instance.lessonData.GetInt("test_number");

            GameSys.StateManager.Instance.AddFront(iqTestState);
        }
    }

    private class StorybookCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            GameSys.StateManager.Instance.ClearAllStates();

            GameSys.ImageDisplayState storyState = new GameSys.ImageDisplayState();

            storyState.ImageType = GameSys.ImageDisplayState.ImageDisplayType.STORYBOOK;

            storyState.UseChapter = JULESTech.DataStore.Instance.lessonData.GetString("storybook_type").Equals("chapter");
            storyState.StartChapter = JULESTech.DataStore.Instance.lessonData.GetInt("storybook_start");
            storyState.EndChapter= JULESTech.DataStore.Instance.lessonData.GetInt("storybook_end");

            Debug.Log("Storybook Use Chapter: " + JULESTech.DataStore.Instance.lessonData.GetString("storybook_type"));
            Debug.Log("Storybook Start Chapter: " + storyState.StartChapter);
            Debug.Log("Storybook End Chapter: " + storyState.EndChapter);

            GameSys.StateManager.Instance.AddFront(storyState);
        }
    }

    private class ImageDisplayCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            GameSys.StateManager.Instance.ClearAllStates();

            GameSys.ImageDisplayState storyState = new GameSys.ImageDisplayState();
            storyState.ImageType = GameSys.ImageDisplayState.ImageDisplayType.IMAGE;

            GameSys.StateManager.Instance.AddFront(storyState);
        }
    }

    private class MusicCommand : JULESTech.ICommand
    {
        public void Execute()
        {
            AudioSystem.StartLoad(JULESTech.DataStore.Instance.lessonData.GetString("music_url"));
        }
    }
    #endregion    

    /* START
     * Content Related Command
     */

    private static GameSceneManager instance = null;

    private JULESTech.CommandMap mContentCommandMap;

    private string mNetworkCommandValue { get; set; }

    private string mCurrentCommand;
    
    private GameSceneManager() { }

    public static GameSceneManager getInstance()
    {
        if (instance == null)
        {
            instance = new GameSceneManager();
            instance.Init();
        }

        return instance;
    }

    private void Init()
    {
        //Add game command to DataStore too
        //bool result = JULESTech.DataStore.Instance.lessonPlanData.LoadTSVFileFromAsset(Resources.Load<TextAsset>(Constants.lessonPlanPath + "game_command.tsv"));

        //TODO Create Command Map Logic
        mContentCommandMap = new JULESTech.CommandMap();

        mCurrentCommand = "";

        //I probabilly shouldn't manually add this
        mContentCommandMap.AddCommand("video", new ContentVideoCommand());
        mContentCommandMap.AddCommand("game", new ContentGameCommand());
        mContentCommandMap.AddCommand("camera", new ContentCameraCommand());
        mContentCommandMap.AddCommand("iqtest", new ContentIQTestCommand());
        mContentCommandMap.AddCommand("storybook", new StorybookCommand());
        mContentCommandMap.AddCommand("image", new ImageDisplayCommand());
        mContentCommandMap.AddCommand("music", new MusicCommand());

        Debug.Log("Successfully loaded GameSceneManager Init");
    }


    public void LoadGameCommand()
    {
        WWWForm loginForm = new WWWForm();
        loginForm.AddField("name", "sof_game_command");

        JULESTech.JulesNet.Instance.SendPOSTRequest("SOFGetContentDataByName.php", loginForm,
         (byte[] _msg) => {
             // Received a response
             StringHelper.DebugPrint(_msg);
             string[] data = Encoding.UTF8.GetString(_msg).Split('\t');

             if (data[0] == "OK")
             {
                 // Success
                 Debug.Log("Game command Success");
                 JSONNode node = JSONNode.Parse(data[1]);

                 string contentData = StringHelper.Base64Decode(node["content_data"]).Trim('\\');
                 JULESTech.DataStore.Instance.gameCommandData.LoadTSVData(contentData);
             }
             else
             {
                 // Failed
                 Debug.LogError("Player game command file failed: " + data[1]);
             }
         },
            (string _error) => {
                // Failed to send
                Debug.LogError("Player game command file failed: " + _error);
            }
        );
    }

    public void TriggerLesson()
    {
        mContentCommandMap.ExecuteCommand(mCurrentCommand);
    }

    public void SetCurrentCommand(string _cmd)
    {
        mCurrentCommand = _cmd;
    }
}