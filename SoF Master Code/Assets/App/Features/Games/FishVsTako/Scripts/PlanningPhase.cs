using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FvTGame
{
    public delegate Sprite AttackTypeIconGetter(UnitData.UnitAttackType type);
    struct UnitSlot
    {
        public GameObject GO;
        public bool Occupied;
        public string UnitName;
        public UnitData UnitData;
    }

    public class PlanningPhase : MonoBehaviour
    {
        public GameLogic LogicScript;
        public GameObject PlayerUnitsGroup;
        public GameObject EnemeyUnitsGroup;
        public GameObject UICanvas;

        public int PlayerArea = 3;

        public List<GameObject> UnitSlotGO;
        private List<UnitSlot> mUnitSlotContent;

        private enum UnitSelectedSource
        {
            SPAWN_UI,
            GRID
        } 

        private bool mUnitSelected = false;
        private UnitSelectedSource mUnitSelectedSource;

        private int mSelectedSpawnUIUnitSlot;
        private UnitData mSelectedUnitData;

        public GameObject SelectedUnitGO;
        public GameObject SelectedHighlightGO;

        private List<GameObject> mPlacedUnitList;
        private List<UnitData> mPlayerUnitSpawnList;

        private List<GameObject> mEnemyUnitPreviewList;

        #region reference linking through editor
        public GameObject HealthSplitterPrefab;
        public Sprite[] UnitSpriteList;
        private Dictionary<string, Sprite> mUnitSpriteMap = new Dictionary<string, Sprite>();
        // RPS = Rock, Paper, Scissor
        public Sprite[] RPSSpriteList;
        private Dictionary<string, Sprite> mRPSSpriteMap = new Dictionary<string, Sprite>();
        #endregion

        private bool mFirstInit = true;
        private bool mExiting = false;
        private float mExitCounter = 0.0f;

        public void Start()
        {
            if (mFirstInit)
                Init();
        }

        private void Init()
        {
            foreach (var item in UnitSpriteList) {
                mUnitSpriteMap.Add(item.name, item);
            }
            foreach (var item in RPSSpriteList) {
                mRPSSpriteMap.Add(item.name, item);
            }

            mUnitSlotContent = new List<UnitSlot>();
            mPlacedUnitList = new List<GameObject>();
            mEnemyUnitPreviewList = new List<GameObject>();

            SelectedUnitGO.SetActive(false);
            SelectedHighlightGO.SetActive(false);

            //Create Unit slot content according to unit slot GO
            for (int i = 0; i < UnitSlotGO.Count; ++i)
            {
                //Create empty slot
                UnitSlot slot = new UnitSlot();
                slot.GO = UnitSlotGO[i];
                slot.UnitName = "";
                slot.Occupied = false;
                slot.UnitData = new UnitData();
                slot.GO.SetActive(false);
                
                mUnitSlotContent.Add(slot);
                //To reset all slot to default empty
                RemoveUnitSlotContent(i);
            }

            //Avoid init again
            mFirstInit = false;

            //Localization UI
            if(!GameLogic.LocalTest)
                SetupUI();

            GenerateHealthSpliter();
        }

        private void SetupUI()
        {
            for (int i = 0; i < mUnitSlotContent.Count; ++i)
            {
                MultiLanguage.getInstance().apply(mUnitSlotContent[i].GO.FindChild("name plate").FindChild("Text"), "game_fvt_select_text");
            }

            MultiLanguage.getInstance().apply(UICanvas.FindChild("PlayerBaseHealth").FindChild("StageText"), "game_fvt_ui_stage");
            MultiLanguage.getInstance().apply(UICanvas.FindChild("PlayerBaseHealth").FindChild("WaveText"), "game_fvt_ui_wave");
        }

        public void OnEnable()
        {
            if (mFirstInit)
                Init();
            else
            {
                //Show SpawnUnit UI
                foreach (GameObject go in UnitSlotGO)
                {
                    go.SetActive(false);
                }
            }

            //Intro Animation
            UICanvas.FindChild("SpawnUnit").GetComponent<DOTweenAnimation>().DOPlayForward();

            //Show reset button
            UICanvas.FindChild("ResetButton").GetComponent<DOTweenAnimation>().DOPlayForward();

            //Show planning play button
            UICanvas.FindChild("PlanningPlayButton").GetComponent<DOTweenAnimation>().DOPlayForward();

            //Hide Enemy Base Health
            UICanvas.FindChild("EnemyBaseHealth").GetComponent<DOTweenAnimation>().DOPlayBackwards();

            //Hide execution play button
            UICanvas.FindChild("ExecutionPlayButton").GetComponent<DOTweenAnimation>().DOPlayBackwards();
            
            //Clean all list
            mPlacedUnitList.Clear();
            
            mExiting = false;

            //Go to the next wave
            //And load it's information
            LogicScript.GameLevelData.AdvanceWave();

            UpdateStageUI();

            //Copy the unit list that player needs to spawn in this level.
            mPlayerUnitSpawnList = new List<UnitData>(LogicScript.GameLevelData.CurrentWaveData.PlayerList);

            //Spawn enemy unit onto grid
            SpawnEnemyList();

            //Spawn the enemy unit to show on the right side of the screen. As preview
            SpawnEnemyPreviewList();

            //New round started
            GenerateUnitInSlot();
        }

        private void UpdateStageUI()
        {
            UICanvas.FindChild("PlayerBaseHealth").FindChild("StageText").GetComponent<Text>().text = MultiLanguage.getInstance().getString("game_fvt_ui_stage") + (LogicScript.CurrentStage);
            UICanvas.FindChild("PlayerBaseHealth").FindChild("WaveText").GetComponent<Text>().text = MultiLanguage.getInstance().getString("game_fvt_ui_wave") + LogicScript.GameLevelData.CurrentWave;
        }

        private void GenerateHealthSpliter()
        {
            //setup health bar spliter
            if (LogicScript.GameLevelData.PlayerBaseMaxHealth > 1)
            {
                float splitterSize = 500 / LogicScript.GameLevelData.PlayerBaseMaxHealth;

                for (int i = 1; i < LogicScript.GameLevelData.PlayerBaseMaxHealth; ++i)
                {
                    GameObject splitter = (GameObject)Instantiate(HealthSplitterPrefab, UICanvas.FindChild("PlayerBaseHealth").FindChild("SpliterGroup").transform);
                    splitter.transform.localPosition = new Vector3(splitterSize * i, 0, 0);
                }
            }

            if (LogicScript.GameLevelData.EnemyBaseMaxHealth > 1)
            {
                float splitterSize = 500 / LogicScript.GameLevelData.EnemyBaseMaxHealth;

                for (int i = 1; i < LogicScript.GameLevelData.EnemyBaseMaxHealth; ++i)
                {
                    GameObject splitter = (GameObject)Instantiate(HealthSplitterPrefab, UICanvas.FindChild("EnemyBaseHealth").FindChild("SpliterGroup").transform);
                    splitter.transform.localPosition = new Vector3(-(splitterSize * i), 0, 0);
                }
            }
        }

        public void Update()
        {
            if (mExiting)
            {
                mExitCounter -= Time.deltaTime;

                if (mExitCounter <= 0.0f)
                {
                    StartExecutionPhase();
                }

                return;
            }

            if (LogicScript.GamePaused)
                return;
            
            Vector2 touchPosition = Input.touchCount > 0 ? Input.GetTouch(0).position : (Vector2)Input.mousePosition;
            Vector2 worldPos = LogicScript.CameraGO.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, LogicScript.CameraGO.GetComponent<Camera>().farClipPlane));

            //Debug.Log("Touch Position: " + touchPosition + " World Position: " + worldPos);

            //Check if touch is on top of which grid.
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(worldPos.x, worldPos.y), Vector2.zero, 11);

            int gridX = -1;
            int gridY = -1;

            //If grid is being hit
            if (hit.transform != null)
            {
                //Get Grid
                string name = hit.transform.gameObject.name;

                Debug.Log("Collision on grid: " + name);

                //Extract X and Y location of the grid.
                //All grid location information is stored in game object name inside scene.
                name = name.Substring(name.IndexOf("_") + 1);
                gridX = int.Parse(name.Substring(0, name.IndexOf("_")));
                name = name.Substring(name.IndexOf("_") + 1);
                gridY = int.Parse(name);
            }
            
            if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
            {
                //Find collision point for spawn unit UI
                for (int i = 0; i < mUnitSlotContent.Count; ++i)
                {
                    if (mUnitSlotContent[i].GO.GetComponent<BoxCollider2D>().OverlapPoint(touchPosition))
                    {
                        SelectUnitFromSpawnUI(i);
                        break;
                    }
                }

                //check if we clicked on a grid
                if (hit.transform != null)
                {
                    GameGrid gridLocation = LogicScript.GetGrid(gridX, gridY);

                    if (gridLocation.Occupied)
                    {
                        if (gridLocation.UnitGO.tag != "FvTEnemy")
                        {
                            //We just clicked on a unit that is already being placed
                            //Make sure we recorded where the selected unit come from.
                            //later we will use this to determine how to spawn the unit or where to put back the unit
                            mUnitSelectedSource = UnitSelectedSource.GRID;

                            //Extract the unit data, later we will use this to create new unit
                            mSelectedUnitData = gridLocation.UnitGO.GetComponent<Unit>().GetData();

                            //let's just remove the entire item in grid
                            //even if we need to put this back, we will respawn at this location later
                            LogicScript.ResetGrid(gridX, gridY);

                            //reset grid does not destory unit game object
                            Destroy(gridLocation.UnitGO);

                            mUnitSelected = true;

                            //Load selected Unit.
                            ShowAndLoadSelectedUnit(mSelectedUnitData.Name);
                        }
                    }
                }
            }

            if (!mUnitSelected)
                return;

            //Selected Unit will always be following touch position
            ((RectTransform)SelectedUnitGO.transform).position = new Vector3(touchPosition.x, touchPosition.y, 0);
            
            if (Input.GetMouseButton(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved))
            {
                Vector2 position = touchPosition;
                if (hit.transform != null)
                {
                    GameGrid gridLocation = LogicScript.GetGrid(gridX, gridY);

                    if (AllowToPutUnitOnGrid(gridLocation))
                    {
                        //Change position to location on the grid
                        position = gridLocation.GridGO.transform.localPosition;
                        position = LogicScript.CameraGO.GetComponent<Camera>().WorldToScreenPoint(new Vector3(position.x, position.y, LogicScript.CameraGO.GetComponent<Camera>().nearClipPlane));

                        SelectedHighlightGO.SetActive(true);
                        ((RectTransform)SelectedHighlightGO.transform).position = new Vector3(position.x, position.y, 0);
                    }
                    else
                    {
                        SelectedHighlightGO.SetActive(false);
                    }
                }
                else
                {
                    SelectedHighlightGO.SetActive(false);
                }
                
            }
            
            if (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
            {
                SelectedHighlightGO.SetActive(false);

                if (hit.transform == null)
                {
                    if (mUnitSelectedSource == UnitSelectedSource.SPAWN_UI)
                        UnitBackToSpawnUI();
                    else
                    {
                        SelectedUnitGO.SetActive(false);

                        gridX = mSelectedUnitData.GridLocation.x;
                        gridY = mSelectedUnitData.GridLocation.y;

                        //Spawn unit onto grid
                        SpawnSelectedUnitOntoGird(gridX, gridY, SelectedUnitGO.name);
                    }
                    //below 2 lines was missing previously, hence resulting in cloning of unit when reset button is pressed -clinton
                    mUnitSelected = false;
                    SelectedUnitGO.SetActive(false);
                    return;
                }
               
                //Get the grid object from GameLogic Script.
                GameGrid gridLocation = LogicScript.GetGrid(gridX, gridY);

                //If grid is already taken
                //Send unit back to slot
                if (!AllowToPutUnitOnGrid(gridLocation))
                {
                    if (mUnitSelectedSource == UnitSelectedSource.SPAWN_UI)
                        UnitBackToSpawnUI();
                    else
                    {
                        SelectedUnitGO.SetActive(false);

                        gridX = mSelectedUnitData.GridLocation.x;
                        gridY = mSelectedUnitData.GridLocation.y;

                        //Spawn unit onto grid
                        SpawnSelectedUnitOntoGird(gridX, gridY, SelectedUnitGO.name);
                    }
                }
                else
                {
                    SelectedUnitGO.SetActive(false);

                    //Spawn unit onto grid
                    SpawnSelectedUnitOntoGird(gridX, gridY, SelectedUnitGO.name);

                    //Now generate new unit into spawn buttons.
                    GenerateUnitInSlot();
                }

                //No matter if we spawn onto grid or back to spawn slots.
                //Unit is no longer selected
                //and SelectedUnit GameObject is no longer needed.
                mUnitSelected = false;
                SelectedUnitGO.SetActive(false);
            }
        }

        private bool AllowToPutUnitOnGrid(GameGrid _grid)
        {
            if (_grid.Occupied)
                return false;

            if (_grid.LocationInList.x >= PlayerArea)
                return false;

            bool result = false;

            List<int> playableGrid = LogicScript.GameLevelData.PlayableLanes;

            int y = 3 - _grid.LocationInList.y;
            foreach (int lane in playableGrid)
            {
                if (y == (lane - 1))
                    result = true;
            }

            return result;
        }

        public void GenerateUnitInSlot()
        {
            for (int i = 0; i < mUnitSlotContent.Count; ++i)
            {
                UnitSlot slot = mUnitSlotContent[i];
                if (!slot.Occupied)
                {
                    UnitData data = PopPlayerUnit();

                    if (data == null)
                        break;

                    LoadSpawnUIUnitSlotResources(data, i);
                }
            }
        }

        private void LoadSpawnUIUnitSlotResources(UnitData _data, int _slot)
        {
            UnitSlot slot = mUnitSlotContent[_slot];

            slot.GO.SetActive(true);
            slot.UnitName = _data.Name;
            slot.Occupied = true;
            slot.UnitData = _data;

            slot.GO.FindChild("Fish Image").SetActive(true);
            slot.GO.FindChild("Text").SetActive(true);

            //Replace Sprite
            slot.GO.FindChild("Fish Image").GetComponent<Image>().sprite = mUnitSpriteMap[slot.UnitName]; // Resources.Load<Sprite>("Games/FvT/Units/" + slot.UnitName);
            slot.GO.FindChild("Text").GetComponent<Text>().text = MultiLanguage.getInstance().getString("game_fvt_" + slot.UnitName);

            slot.GO.FindChild("Fish Image").GetComponent<Image>().SetNativeSize();

            List<UnitData.UnitAttackType> list = slot.UnitData.UnitAttackTypeList;

            for (int i = 0; i < list.Count; ++i)
            {
                UnitData.UnitAttackType type = list[i];

                //NOTE, there is only 3 game object, because we assume the game balance at most reach 3 unit types.
                slot.GO.FindChild("Unit Type " + (i + 1)).SetActive(true);
                slot.GO.FindChild("Unit Type " + (i + 1)).GetComponent<Image>().sprite = GetAttackTypeIcon(type);
            }

            mUnitSlotContent[_slot] = slot;
        }

        private void RemoveUnitSlotContent(int _slot)
        {
            UnitSlot slot = mUnitSlotContent[_slot];
            
            slot.UnitName = "";
            slot.Occupied = false;

            slot.GO.FindChild("Fish Image").SetActive(false);
            slot.GO.FindChild("Text").SetActive(false);
            
            for (int i = 0; i < 3; ++i)
            {
                //NOTE, there is only 3 game object, because we assume the game balance at most reach 3 unit types.
                slot.GO.FindChild("Unit Type " + (i + 1)).SetActive(false);
            }

            mUnitSlotContent[_slot] = slot;
        }

        public void SelectUnitFromSpawnUI(int _slot)
        {
            if (mExiting)
                return;

            UnitSlot slot = mUnitSlotContent[_slot];

            if (!slot.Occupied)
                return;

            mUnitSelected = true;
            mUnitSelectedSource = UnitSelectedSource.SPAWN_UI;
            mSelectedSpawnUIUnitSlot = _slot;
            mSelectedUnitData = slot.UnitData;


            //Update Selected Unit for dragging
            ShowAndLoadSelectedUnit(slot.UnitName);

            RemoveUnitSlotContent(_slot);
        }

        private void ShowAndLoadSelectedUnit(string _unitName)
        {
            SelectedUnitGO.SetActive(true);
            SelectedUnitGO.name = _unitName;
            SelectedUnitGO.GetComponent<SelectedUnit>().Load(_unitName, mUnitSpriteMap[_unitName], GetAttackTypeIcon);

        }

        private void UnitBackToSpawnUI()
        {
            mUnitSelected = false;
            SelectedUnitGO.SetActive(false);

            LoadSpawnUIUnitSlotResources(mSelectedUnitData, mSelectedSpawnUIUnitSlot);
        }

        private void SpawnSelectedUnitOntoGird(int x, int y, string unitName, System.Action<GameObject> onComplete = null)
        {
            CachedResources.Spawn(Constants.FVT_SHARED, "Unit", (loadedGameObject) => 
            {
                GameGrid gridLocation = LogicScript.GetGrid(x, y);

                GameObject unitGO = loadedGameObject;
                unitGO.name = unitName;
                unitGO.GetComponent<Unit>().Load(unitName, mUnitSpriteMap[unitName], GetAttackTypeIcon);

                bool friendly = unitGO.GetComponent<Unit>().IsFriendly();

                unitGO.transform.parent = friendly ? PlayerUnitsGroup.transform : EnemeyUnitsGroup.transform;

                //Spawn new unit onto the grid
                unitGO.transform.localPosition = gridLocation.GridGO.transform.localPosition;
                unitGO.FindChild("highlight").SetActive(false);

                //Set sorting order according to location on grid.
                switch (y) {
                    case 0:
                        unitGO.GetComponent<Unit>().SetLayerOrder((int)GameLayer.LANE_0);
                        break;
                    case 1:
                        unitGO.GetComponent<Unit>().SetLayerOrder((int)GameLayer.LANE_1);
                        break;
                    case 2:
                        unitGO.GetComponent<Unit>().SetLayerOrder((int)GameLayer.LANE_2);
                        break;
                    case 3:
                        unitGO.GetComponent<Unit>().SetLayerOrder((int)GameLayer.LANE_3);
                        break;
                }

                unitGO.GetComponent<Unit>().SetGridLocation(x, y);

                gridLocation.UnitGO = unitGO;
                gridLocation.Occupied = true;

                LogicScript.UpdateGrid(x, y, gridLocation);

                if (friendly)
                    mPlacedUnitList.Add(unitGO);
                else
                    gridLocation.UnitGO.tag = "FvTEnemy";


            });
        }

        public void ResetPlanning()
        {
            RemoveAllUnitSpawnOnGrid();
            RemoveAllUnitInSpawnSlot();

            //Get the original player list unit out again
            mPlayerUnitSpawnList = new List<UnitData>(LogicScript.GameLevelData.CurrentWaveData.PlayerList);

            //restart the spawn process again
            GenerateUnitInSlot();
        }

        //We want to remove all the unit we placed during this planning phase.
        private void RemoveAllUnitSpawnOnGrid()
        {
            for (int i = 0; i < mPlacedUnitList.Count; ++i)
            {
                //as list slots are not removed when units are moved, this if condition checks if the slot is null - clinton
                if (mPlacedUnitList[i] != null)
                {
                    //Get the unit
                    Unit unit = mPlacedUnitList[i].GetComponent<Unit>();

                    //Reset the grid that the unit is on
                    LogicScript.ResetGrid(unit.GetGridLocation().x, unit.GetGridLocation().y);

                    //Delete the game object.
                    Destroy(mPlacedUnitList[i]);
                }
            }

            //Clear the list
            mPlacedUnitList.Clear();
        }

        private void RemoveAllUnitInSpawnSlot()
        {
            for (int i = 0; i < mUnitSlotContent.Count; ++i)
            {
                RemoveUnitSlotContent(i);
                mUnitSlotContent[i].GO.SetActive(false);
            }
        }

        public void FinshPlanning()
        {
            

            //We need to do clean up before moving to Execution phase.
            mExiting = true;
            mExitCounter = 1.5f;

            //Move camera to center
            DOTween.Sequence()
                .AppendInterval(0.5f)
                .Append(LogicScript.CameraGO.transform.DOMoveX(0, 1.0f));

            //Delete all enemy preview
            foreach (GameObject go in mEnemyUnitPreviewList)
            {
                go.GetComponent<DOTweenAnimation>().DOPlay();
            }
            //clearEnemy();

            //Moveout SpawnUnit UI
            UICanvas.FindChild("SpawnUnit").GetComponent<DOTweenAnimation>().DOPlayBackwards();

            //Hide reset button
            UICanvas.FindChild("ResetButton").GetComponent<DOTweenAnimation>().DOPlayBackwards();

            //Hide planning play button
            UICanvas.FindChild("PlanningPlayButton").GetComponent<DOTweenAnimation>().DOPlayBackwards();

            //Show Enemy Base Health
            UICanvas.FindChild("EnemyBaseHealth").GetComponent<DOTweenAnimation>().DOPlayForward();
            
            //Show execution play button
            UICanvas.FindChild("ExecutionPlayButton").GetComponent<DOTweenAnimation>().DOPlayForward();
        }

        //to remove enemy do tweens when player quit games to prevent index out of range error - clinton
        public void clearEnemy()
        {
            if (mExiting == false)
            {
                foreach (GameObject go in mEnemyUnitPreviewList)
                {
                    go.GetComponent<DOTweenAnimation>().DOKill();
                }
                Debug.Log("Deleting tweens");
            }
            
        }

        private void StartExecutionPhase()
        {
            LogicScript.gameObject.FindChild("ExecutionPhase").SetActive(true);
            this.gameObject.SetActive(false);
        }

        //Get out the first unit inside spawn list
        //and remove it
        private UnitData PopPlayerUnit()
        {
            if (mPlayerUnitSpawnList.Count == 0)
                return null;

            UnitData data = mPlayerUnitSpawnList[0];
            mPlayerUnitSpawnList.RemoveAt(0);

            return data;
        }

        private void SpawnEnemyList()
        {
            //Get current wave enemy needed for spawning
            List<UnitData> enemyList = LogicScript.GameLevelData.CurrentWaveData.EnemyList;

            //Create the unit and place it onto grid
            for (int i = 0; i < enemyList.Count; ++i)
            {
                UnitData unit = enemyList[i];

                int x = unit.GridLocation.x;
                int y = unit.GridLocation.y;

                GameGrid gridLocation = LogicScript.GetGrid(x, y);
                SpawnSelectedUnitOntoGird(x, y, unit.Name, (loadedAsset)=> { loadedAsset.SetActive(false); });
            }
        }

        private void SpawnEnemyPreviewList()
        {
            mEnemyUnitPreviewList.Clear();
            //go through the whole grid system
            //Find out which one is enemy, then we are going to spawn preview onto the screen
            for (int y = 0; y < 4; ++y)
            {

                //Enemy will only exist from grid 4 onwards
                for (int x = 6; x > 3; --x)
                {
                    GameObject parent = UICanvas.FindChild("EnemyPreview").FindChild("Lane " + (y + 1));
                    GameGrid gridLocation = LogicScript.GetGrid(x, y);

                    if (gridLocation.Occupied)
                    {
                        //There is a enemy
                        //spawn preview enemy unit
                        CachedResources.Spawn(Constants.FVT_SHARED, "Enemy Preview Unit", (loadedGameObject) => {
                            UnitData enemyUnitData = gridLocation.UnitGO.GetComponent<Unit>().GetData();
                            loadedGameObject.GetComponent<EnemyPreviewUnit>().Load(enemyUnitData, mUnitSpriteMap[enemyUnitData.Name], GetAttackTypeIcon);
                            loadedGameObject.transform.parent = parent.transform;

                            Vector3 pos = new Vector3(0, 0, 0);
                            pos.x -= (6 - x) * 80;

                            Vector3 size = new Vector3(1, 1, 1);

                            loadedGameObject.transform.localPosition = pos;
                            loadedGameObject.transform.localScale = size;

                            mEnemyUnitPreviewList.Add(loadedGameObject);
                        });
                    }
                }
            }
        }

        Sprite GetAttackTypeIcon(UnitData.UnitAttackType _type)
        {
            string fileName = "";

            switch (_type) {
                case UnitData.UnitAttackType.PAPER:
                    fileName = "paper";
                    break;
                case UnitData.UnitAttackType.SCISSOR:
                    fileName = "scissor";
                    break;
                case UnitData.UnitAttackType.STONE:
                    fileName = "stone";
                    break;
            }

            return mRPSSpriteMap[fileName];
        }

        public void PauseGame()
        {
            //actually dont need to do anything
        }

        public void ResumeGame()
        {
            //actually dont need to do anything
        }
    }
}