using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// getting setting file from server
using SimpleJSON;
using System.Text;

namespace FvTGame
{ 
    public enum GameLayer
    {
        LANE_3 = 1,
        LANE_2 = 2,
        LANE_1 = 3,
        LANE_0 = 4,
        SELECTED = 5
    }

    public struct GameGrid
    {
        public Vector2 LocationInScene;
        public IntVector2 LocationInList;
        public bool Occupied;
        public GameObject GridGO;
        public GameObject UnitGO;
    }

    public class GameLogic : MonoBehaviour {
        readonly string GetContentDataByNamePHP = "SOFGetContentDataByName.php";
        public static bool LocalTest = false;

        public GameObject GameBoard;
        public GameObject CameraGO;
        public GameObject PausePanel;
        
        public Vector3 CameraPlanningLocation;

        private int mCurrentStage;
        [HideInInspector]
        public int CurrentStage { get { return mCurrentStage;  } }

        [HideInInspector]
        public bool GameFinish;

        [HideInInspector]
        public bool GamePaused;

        private List<List<GameGrid>> grid2DArray;
        public GameGrid GetGrid(int x, int y) { return grid2DArray[x][y]; }
        public void UpdateGrid(int x, int y, GameGrid _grid) { grid2DArray[x][y] = _grid;  }
        public void ResetGrid(int x, int y) { GameGrid grid = grid2DArray[x][y]; grid.UnitGO = null; grid.Occupied = false; grid2DArray[x][y] = grid; }

        public static JULESTech.DataMap GameData;

        public LevelData GameLevelData;

        // Use this for initialization
        IEnumerator Start()
        {
            GameFinish = false;

            //GameSys.GameState state = (GameSys.GameState)GameSys.StateManager.Instance.GetFirstState();
            //mCurrentStage = state.startLevel;
            
            if(LocalTest)
                mCurrentStage = 1;
            else 
                mCurrentStage = GameState.FvTProg.selectedLevel;

            yield return LoadGameDataCoroutine();
            yield return LoadStageCoroutine(mCurrentStage);

            grid2DArray = new List<List<GameGrid>>();

            GameObject GridGO = GameBoard.FindChild("Grids");

            for (int x = 0; x <= 6; x++)
            {
                List<GameGrid> list = new List<GameGrid>();
                grid2DArray.Add(list);

                for (int y = 0; y <= 3; y++)
                {
                    GameGrid grid = new GameGrid();

                    grid.LocationInList = new IntVector2(x, y);
                    grid.GridGO = GridGO.transform.Find("Grid_" + x + "_" + y).gameObject;
                    grid.UnitGO = null;
                    grid.LocationInScene = GridGO.transform.Find("Grid_" + x + "_" + y).GetComponent<Transform>().localPosition;
                    grid.Occupied = false;

                    list.Add(grid);
                }
            }

            if(!LocalTest)
                SetupUI();

            StartRound();
        }

        private void OnDestroy()
        {
            DG.Tweening.DOTween.Clear();
        }

        private void SetupUI()
        {
            //Pause Menu
            MultiLanguage.getInstance().applyImage(PausePanel.FindChild("Game Title Image").GetComponent<Image>(), "tako_main_menu_title");
            MultiLanguage.getInstance().applyImage(PausePanel.FindChild("Buttons Holder Panel").FindChild("Button Resume").GetComponent<Image>(), "gui_settings_resume");
            MultiLanguage.getInstance().applyImage(PausePanel.FindChild("Buttons Holder Panel").FindChild("Button Restart").GetComponent<Image>(), "gui_settings_restart");
            MultiLanguage.getInstance().applyImage(PausePanel.FindChild("Buttons Holder Panel").FindChild("Button Home").GetComponent<Image>(), "gui_settings_home");
            MultiLanguage.getInstance().applyImage(PausePanel.FindChild("Buttons Holder Panel").FindChild("Button Level Select").GetComponent<Image>(), "gui_settings_levelselect");

            MultiLanguage.getInstance().apply(PausePanel.FindChild("Paused Text"), "in_game_menu_title");
            MultiLanguage.getInstance().apply(PausePanel.FindChild("CurrentStage"), "in_game_menu_current_stage");

            PausePanel.FindChild("Current Stage Text").GetComponent<Text>().text = mCurrentStage.ToString();
        }

        private IEnumerator LoadGameDataCoroutine()
        {
            GameData = new JULESTech.DataMap();

            if (LocalTest) {
                //TextAsset rawData = Resources.Load<TextAsset>("Games/FvT/fvt_game_data.tsv");
                //GameData.LoadTSVData(rawData.text);
                yield return JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset(Constants.FVT_CONFIGS, "fvt_game_data.tsv",
                    (gameData) => {
                        GameData.LoadTSVData(gameData.text);
                    }
                );
            } else {
                //GameData.LoadTSVData(((GameSys.GameState)GameSys.StateManager.Instance.GetFirstState()).GameData);
                WWWForm loginForm = new WWWForm();
                loginForm.AddField("name", "sof_fvt_game_data");

                yield return JULESTech.JulesNet.Instance.SendPOSTRequestCoroutine (GetContentDataByNamePHP, loginForm,
                    (byte[] _msg) => {
                        // Received a response
                        StringHelper.DebugPrint(_msg);
                        string[] data = Encoding.UTF8.GetString(_msg).Split('\t');

                        if (data[0] == "OK") {
                            // Success
                            JSONNode node = JSONNode.Parse(data[1]);
                            GameData.LoadTSVData(StringHelper.Base64Decode(node["content_data"]));
                        } else {
                            //If doesnt exist, it returns error
                            // Failed
                            Debug.LogError("[fvt_game_data.tsv] load failed: data does not exist on server");
                        }
                    },
                    (string _error) => {
                        // Failed to send
                        Debug.LogErrorFormat("[fvt_game_data.tsv] load failed: {0}", _error);
                    }
                );
            }
        }
        
        private IEnumerator LoadStageCoroutine(int _stage)
        {
            string jsonText = "";

            if (LocalTest) {
                //jsonText = Resources.Load<TextAsset>("Games/FvT/fvt_level").text;
                yield return JULESTech.Resources.AssetBundleManager.LoadAssetTextAsset 
                    (Constants.FVT_CONFIGS, "fvt_level", (loadedTextAsset) => jsonText = loadedTextAsset.text);
            } else {
                //jsonText = ((GameSys.GameState)GameSys.StateManager.Instance.GetFirstState()).GameLevelData;
                yield return GameLevelDataFromServer((loadedJsonData) => jsonText = loadedJsonData);
            }

            GameLevelData = new LevelData(jsonText);
            GameLevelData.LoadStage(_stage);

            //Change lane allowed
            List<int> lanes = GameLevelData.PlayableLanes;

            for (int i = 1; i <= 4; ++i) {
                bool exist = false;
                foreach (int lane in lanes) {
                    if (lane == i)
                        exist = true;
                }

                //Show grid overlay
                GameBoard.FindChild("Grids").FindChild("Lane " + i + " Overlay").SetActive(!exist);
            }
        }
        private IEnumerator GameLevelDataFromServer(System.Action<string> onComplete)
        {
            WWWForm loginForm2 = new WWWForm();
            loginForm2.AddField("name", "sof_fvt_game_level_data");

            yield return JULESTech.JulesNet.Instance.SendPOSTRequestCoroutine (GetContentDataByNamePHP, loginForm2,
                (byte[] _msg) => {
                    // Received a response
                    StringHelper.DebugPrint(_msg);
                    string[] data = Encoding.UTF8.GetString(_msg).Split('\t');

                    if (data[0] == "OK") {
                        // Success
                        JSONNode node = JSONNode.Parse(data[1]);

                        if (onComplete != null) {
                            onComplete(StringHelper.Base64Decode(node["content_data"]));
                        }
                    } else {
                        //If doesnt exist, it returns error
                        // Failed
                        Debug.LogErrorFormat("[fvt_level] load failed: data does not exist on server");
                    }
                },
                (string _error) => {
                    // Failed to send
                    //Just to be safe, we will accept everything
                    Debug.LogErrorFormat("[fvt_level] load failed: {0}", _error);
                }
            );
        }

        public void EndGame()
        {
            if (GameFinish)
                return;

            GameFinish = true;

            //Game End
            //check the game status
            bool playerWin = true;

            //this means the player did not defeat the enemy base within the stage
            if (!GameLevelData.IsGameCompleted())
            {
                //Player lose
                playerWin = false;
            }
            else
            {
                playerWin = GameLevelData.PlayerBaseHealth > 0 ? true : false;
            }
            if (playerWin == true)
            {
                var currentLevelId = GameState.FvTProg.selectedLevel - 1;
                var currentLevelData = GameState.FvTProg.GetLevel(currentLevelId);

                currentLevelData.pointsEarned = Mathf.Max(currentLevelData.pointsEarned, 20000);
                currentLevelData.starEarned = 3;

                currentLevelData.status = FvTProgression.ELevelStatus.Finished;
                GameState.FvTProg.SetLevel(currentLevelId, currentLevelData);

                var nextLevel = GameState.FvTProg.GetLevel(currentLevelId + 1);
                if (nextLevel.status == FvTProgression.ELevelStatus.Locked)
                {
                    nextLevel.status = FvTProgression.ELevelStatus.Available;
                    GameState.FvTProg.SetLevel(currentLevelId + 1, nextLevel);
                }
            }

            FvTWinScreen.Open(playerWin);

        }

        public void StartRound()
        {
            //check if we have finished the round
            if (GameLevelData.IsStageCompleted())
            { 
                //game ended
                EndGame();

                return;
            }
            
            gameObject.FindChild("PlanningPhase").SetActive(true);
        }

        public void PauseGame()
        {
            GamePaused = true;
            PausePanel.SetActive(true);

            if (gameObject.FindChild("PlanningPhase").activeSelf)
                gameObject.FindChild("PlanningPhase").GetComponent<PlanningPhase>().PauseGame();

            if (gameObject.FindChild("ExecutionPhase").activeSelf)
                gameObject.FindChild("ExecutionPhase").GetComponent<ExecutionPhase>().PauseGame();
        }

        public void ResumeGame()
        {
            GamePaused = false;
            PausePanel.SetActive(false);

            if (gameObject.FindChild("PlanningPhase").activeSelf)
                gameObject.FindChild("PlanningPhase").GetComponent<PlanningPhase>().ResumeGame();

            if (gameObject.FindChild("ExecutionPhase").activeSelf)
                gameObject.FindChild("ExecutionPhase").GetComponent<ExecutionPhase>().ResumeGame();
        }

        public void GotToHome()
        {
            MainNavigationController.GotoMainMenu();
        }

        public void GoToLevelSelect()
        {
            //SceneManager.LoadScene("FvTLevelSelect");

            MainNavigationController.DoAssetBundleLoadLevel(Constants.FVT_SCENES, "FvTLevelSelect");
        }

        public void RestartGame()
        {
            //SceneManager.LoadScene("FvTGameScene");
            MainNavigationController.DoAssetBundleLoadLevel(Constants.FVT_SCENES, "FvTGameScene");
        }
    }
}