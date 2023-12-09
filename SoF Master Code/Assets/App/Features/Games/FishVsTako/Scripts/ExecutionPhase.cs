using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FvTGame
{
    public class ExecutionPhase : MonoBehaviour
    {
        public GameLogic LogicScript;
        public GameObject UICanvas;

        private bool firstInit = true;

        private List<List<GameObject>> mPlayerUnitList;
        private List<List<GameObject>> mEnemyUnitList;

        private List<ExecutionState> mExecutionStateList;

        private enum StateMachine
        {
            INIT_ANIMATION,
            ACTION_READY,
            ACTION_ANIMATION,
            CLEAN_UP
        }

        private StateMachine currentGameState;
        private float animationCounter = 0.0f;

        public void Start()
        {
            if (firstInit)
                Init();
        }

        private void Init()
        {
            firstInit = false;

            mExecutionStateList = new List<ExecutionState>();
            mPlayerUnitList = new List<List<GameObject>>();
            mEnemyUnitList = new List<List<GameObject>>();
        }

        public void OnEnable()
        {
            if (firstInit)
                Init();

            mExecutionStateList.Clear();
            mPlayerUnitList.Clear();
            mEnemyUnitList.Clear();

            //Get information from the board
            for (int y = 0; y < 4; ++y)
            {
                List<GameObject> playerList = new List<GameObject>();
                mPlayerUnitList.Add(playerList);

                List<GameObject> enemyList = new List<GameObject>();
                mEnemyUnitList.Add(enemyList);

                for (int x = 2; x >= 0; --x)
                {
                    GameGrid grid = LogicScript.GetGrid(x, y);
                    if (grid.Occupied)
                    {
                        playerList.Add(grid.UnitGO);
                    }
                    
                    LogicScript.ResetGrid(x, y);
                }

                for (int x = 4; x < 7; ++x)
                {
                    GameGrid grid = LogicScript.GetGrid(x, y);
                    if (grid.Occupied)
                    {
                        enemyList.Add(grid.UnitGO);
                    }

                    LogicScript.ResetGrid(x, y);
                }
            }
            
            currentGameState = StateMachine.INIT_ANIMATION;
            int counter = 0;
            
            //Start animation
            foreach (List<GameObject> list in mEnemyUnitList)
            { 
                foreach (GameObject enemy in list)
                {
                    enemy.SetActive(true);
                    
                    enemy.transform.localScale = new Vector3(0, 0, 0);

                    float waitTime = counter * 0.5f;
                    
                    DOTween.Sequence()
                        .AppendInterval(waitTime)
                        .Append(enemy.transform.DOScale(1, 0.5f));

                    counter++;

                    animationCounter = waitTime + 0.75f;
                }
            }

            GenerateExecution();
        }

        //Create all possible fighting action needed
        private void GenerateExecution()
        { 
            bool finish = false;

            do
            {
                bool foundAction = false;
                int lane = 3;

                do
                {
                    //get the lane list
                    List<GameObject> playerLaneList = mPlayerUnitList[lane];
                    List<GameObject> enemyLaneList = mEnemyUnitList[lane];

                    //Check if both list is empty
                    if (playerLaneList.Count > 0 || enemyLaneList.Count > 0)
                    {
                        //as long as there is one unit in the lane
                        //means there is action
                        GameObject playerUnit = playerLaneList.Count > 0 ? playerLaneList[0] : null;
                        GameObject enemyUnit = enemyLaneList.Count > 0 ? enemyLaneList[0] : null;

                        //Create execution state and add the objects that are having action
                        ExecutionState state = new ExecutionState();
                        ExecutionState.FightResult fightResult = state.Load(playerUnit, enemyUnit);
                        mExecutionStateList.Add(state);

                        //Simulate fight result
                        if (fightResult == ExecutionState.FightResult.DRAW)
                        {
                            //Remove both from list
                            playerLaneList.RemoveAt(0);
                            enemyLaneList.RemoveAt(0);
                        }
                        else if (fightResult == ExecutionState.FightResult.ENEMY_HIT_BASE || fightResult == ExecutionState.FightResult.PLAYER_WIN)
                        {
                            //Remove enemy from list
                            enemyLaneList.RemoveAt(0);
                        }
                        else if (fightResult == ExecutionState.FightResult.PLAYER_HIT_BASE || fightResult == ExecutionState.FightResult.ENEMY_WIN)
                        {
                            //Remove player from list
                            playerLaneList.RemoveAt(0);
                        }

                        //end the loop
                        //so we start from top and find the next
                        foundAction = true;
                    }
                    else
                    {
                        //the whole lane is empty
                        //try next lane
                        if (lane > 0)
                            lane--;
                        else
                        {
                            //tried all the lane, means we have finished whole execution phase
                            foundAction = true;
                            finish = true;
                        }
                    }

                } while (!foundAction);

            } while (!finish);
        }

        private bool CheckGameEndCondition()
        {
            return LogicScript.GameLevelData.IsGameCompleted();
        }

        public void Update()
        {
            if (LogicScript.GamePaused)
                return;

            if (currentGameState == StateMachine.INIT_ANIMATION)
            {
                animationCounter -= Time.deltaTime;
                if (animationCounter <= 0.0f)
                {
                    animationCounter = 0.0f;
                    currentGameState = StateMachine.ACTION_READY;
                }

                return;
            }
            else if (currentGameState == StateMachine.ACTION_READY)
            {
                //now it's automatically go to action animation
                currentGameState = StateMachine.ACTION_ANIMATION;
                return;
            }
            else if (currentGameState == StateMachine.CLEAN_UP)
            {
                animationCounter -= Time.deltaTime;
                if (animationCounter <= 0.0f)
                {
                    animationCounter = 0.0f;
                    AdvanceRound();
                }

                return;
            }
            
            if (mExecutionStateList.Count == 0)
            {
                CleanUpExecutionPhase();
    
                return;
            }

            ExecutionState currState = mExecutionStateList[0];

            if (!currState.IsInit)
            {
                currState.Parent = this;

                currState.Init();
                currState.IsInit = true;
            }

            currState.Update(Time.deltaTime);

            if (currState.IsDone)
            {
                currState.CleanUp();
                RemoveState(0);

                //check game condition
                if (CheckGameEndCondition())
                {
                    //GAME FINISH
                    DOTween.KillAll();
                    DOTween.Clear();

                    LogicScript.EndGame();
                    
                    this.gameObject.SetActive(false);
                }
                else
                {
                    currentGameState = StateMachine.ACTION_READY;
                }
            }
        }

        private void RemoveState(int _index)
        {
            mExecutionStateList.RemoveAt(_index);
        }

        private void CleanUpExecutionPhase()
        {
            currentGameState = StateMachine.CLEAN_UP;

            //If we finished all the round
            //no need to go back to planning state
            if (LogicScript.GameLevelData.IsStageCompleted())
                return;

            animationCounter = 1.0f;
            
            //Move camera to planning location
            LogicScript.CameraGO.transform.DOMoveX(LogicScript.CameraPlanningLocation.x, 1.0f);
        }

        private void AdvanceRound()
        {
            LogicScript.StartRound();
            this.gameObject.SetActive(false);
        }

        public void DestroyGO(GameObject _obj)
        {
            if (_obj == null)
                return;

            Destroy(_obj);
        }

        public void DamageBase(bool _playerBase, int _damage)
        {
            if (_playerBase)
                LogicScript.GameLevelData.DamagePlayerBase(_damage);
            else
                LogicScript.GameLevelData.DamageEnemyBase(_damage);

            GameObject healthBar = _playerBase ? UICanvas.FindChild("PlayerBaseHealth").FindChild("HealthBar") : UICanvas.FindChild("EnemyBaseHealth").FindChild("HealthBar");
            float maxHealth = _playerBase ? LogicScript.GameLevelData.PlayerBaseMaxHealth : LogicScript.GameLevelData.EnemyBaseMaxHealth;
            float currHealth = _playerBase ? LogicScript.GameLevelData.PlayerBaseHealth : LogicScript.GameLevelData.EnemyBaseHealth;
            float scaleToValue = currHealth / maxHealth;

            healthBar.GetComponent<Image>().DOFillAmount(scaleToValue, 0.5f);
        }

        public void PauseGame()
        {
            DOTween.PauseAll();
        }

        public void ResumeGame()
        {
            DOTween.PlayAll();
        }
    }

    public class ExecutionState
    {
        public enum FightResult
        {
            PLAYER_HIT_BASE,
            ENEMY_HIT_BASE,
            DRAW,
            PLAYER_WIN,
            ENEMY_WIN
        }

        private FightResult mFightResult;
        private GameObject mPlayerUnit;
        private GameObject mEnemyUnit;

        private float animationCounter;

        public ExecutionPhase Parent;
        public bool IsDone;
        public bool IsInit;

        public ExecutionState()
        {
            IsDone = false;
            IsInit = false;
        }

        public FightResult Load(GameObject _playerUnit, GameObject _enemyUnit)
        {
            mPlayerUnit = _playerUnit;
            mEnemyUnit = _enemyUnit;

            //Schedue whole animation 
            mFightResult = UnitFightResult();
            
            return mFightResult;
        }

        public void Init()
        {
            Vector2 collisionLocation = new Vector2();
            if (mFightResult == FightResult.ENEMY_HIT_BASE || mFightResult == FightResult.PLAYER_HIT_BASE)
            {
                GameObject animationGO = mFightResult == FightResult.ENEMY_HIT_BASE ? mEnemyUnit : mPlayerUnit;
                GameObject baseGO = mFightResult == FightResult.ENEMY_HIT_BASE ? GameObject.Find("GameBoard").FindChild("Player Base") : GameObject.Find("GameBoard").FindChild("Enemy Base");
                GameObject collisionGO = mFightResult == FightResult.ENEMY_HIT_BASE ? GameObject.Find("Player Base Collision") : GameObject.Find("Enemy Base Collision");

                collisionLocation.x = collisionGO.transform.position.x;
                
                //Animation sequence
                Sequence animationSequence = DOTween.Sequence();

                //First move the objects to collision point
                animationSequence.Append(animationGO.transform.DOMoveX(collisionLocation.x, 1.0f).SetEase(Ease.OutBack));
                animationSequence.Append(animationGO.transform.DOScale(0.0f, 0.5f));

                //Deal damage
                animationSequence.AppendCallback(DamageBase);
                animationSequence.AppendInterval(0.5f);

                //Base animation
                DOTween.Sequence()
                    .AppendInterval(0.6f)
                    .AppendCallback(() => {
                    
                    baseGO.transform.DOShakePosition(0.3f, new Vector3(1, 0, 0));

                    DOTween.Sequence()
                        .Append(baseGO.GetComponent<SpriteRenderer>().DOColor(Color.red, 0.15f))
                        .Append(baseGO.GetComponent<SpriteRenderer>().DOColor(Color.white, 0.15f));
                });

                //End
                animationSequence.AppendCallback(AnimationFinish);
            }
            else
            {
                //Find out lane
                int lane = mPlayerUnit.GetComponent<Unit>().GetGridLocation().x;

                collisionLocation = Parent.LogicScript.GetGrid(3, lane).LocationInScene;

                //Since enemy is always on the right
                //let's try to calculate the center point of the two
                float differenceX = mEnemyUnit.transform.localPosition.x - mPlayerUnit.transform.localPosition.x;
                //divide it by 2 and add it back to player unit transform location, is where the center is
                collisionLocation.x = mPlayerUnit.transform.localPosition.x + (differenceX / 2.0f);

                //We are assuming there will only be 1 icon for game levels
                //Animation need to rethink and overhaul for icon animations if this logic is no longer true.
                GameObject playerIcon = mPlayerUnit.FindChild("Unit Type 1");
                GameObject enemyIcon = mEnemyUnit.FindChild("Unit Type 1");

                //Animation Sequence
                DOTween.Sequence()
                    .AppendCallback(() =>
                    {
                        mPlayerUnit.transform.DOMoveX(collisionLocation.x - 1, 1.0f).SetEase(Ease.InBack);
                        mEnemyUnit.transform.DOMoveX(collisionLocation.x + 1, 1.0f).SetEase(Ease.InBack);
                    })
                    .AppendInterval(1.0f)
                    .AppendCallback(() =>
                    {
                        //We are assuming there will only be 1 icon for game levels
                        //Animation need to rethink and overhaul for icon animations if this logic is no longer true.
                        
                        //Move icons and let them clash, while enlarging them
                        playerIcon.transform.DOLocalMove(new Vector3(0.2f, 0.0f, 0.0f), 0.5f).SetEase(Ease.OutBounce);
                        playerIcon.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.25f).SetEase(Ease.InSine);

                        enemyIcon.transform.DOLocalMove(new Vector3(-0.2f, 0.0f, 0.0f), 0.5f).SetEase(Ease.OutBounce);
                        enemyIcon.transform.DOScale(new Vector3(1.5f, 1.5f, 1.5f), 0.25f).SetEase(Ease.InSine);
                    })
                    .AppendInterval(0.75f)
                    .AppendCallback(() =>
                    {
                        //We are assuming there will only be 1 icon for game levels
                        //Animation need to rethink and overhaul for icon animations if this logic is no longer true.
                        
                        //Show icon destory each other
                        if (mFightResult == FightResult.DRAW)
                        {
                            playerIcon.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.25f).SetEase(Ease.InBack);
                            enemyIcon.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.25f).SetEase(Ease.InBack);
                        }
                        else if (mFightResult == FightResult.ENEMY_WIN)
                        {
                            DOTween.Sequence()
                            .Append(playerIcon.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.25f).SetEase(Ease.InBack))
                            .AppendInterval(0.25f)
                            .AppendCallback(() =>
                            {
                                enemyIcon.transform.DOLocalMove(new Vector3(0.8f, -0.5f, 0), 0.5f);
                                enemyIcon.transform.DOScale(new Vector3(0.8f, 0.8f, 1), 0.25f).SetEase(Ease.OutSine);
                            });

                            
                        }
                        else if (mFightResult == FightResult.PLAYER_WIN)
                        {
                            DOTween.Sequence()
                            .Append(enemyIcon.transform.DOScale(new Vector3(0.0f, 0.0f, 0.0f), 0.25f).SetEase(Ease.InBack))
                            .AppendInterval(0.25f)
                            .AppendCallback(() =>
                            {
                                playerIcon.transform.DOLocalMove(new Vector3(-0.8f, -0.5f, 0.0f), 0.5f);
                                playerIcon.transform.DOScale(new Vector3(0.8f, 0.8f, 1), 0.25f).SetEase(Ease.OutSine);
                            });
                        }
                    })
                    .AppendInterval(0.75f)
                    .AppendCallback(() =>
                    {
                        //Show one destory another
                        if (mFightResult == FightResult.DRAW)
                        {
                            mPlayerUnit.transform.DOScale(0.0f, 0.5f).SetEase(Ease.InBack);
                            mEnemyUnit.transform.DOScale(0.0f, 0.5f).SetEase(Ease.InBack);
                        }
                        else if (mFightResult == FightResult.ENEMY_WIN)
                        {
                            mPlayerUnit.transform.DOScale(0.0f, 0.5f).SetEase(Ease.InBack);
                        }
                        else if (mFightResult == FightResult.PLAYER_WIN)
                        {
                            mEnemyUnit.transform.DOScale(0.0f, 0.5f).SetEase(Ease.InBack);
                        }
                    })
                    .AppendInterval(0.5f)
                    .AppendCallback(AnimationFinish);
            }
        }

        public Sequence Test()
        {


            return DOTween.Sequence();
        }

        public void Update(float _dt)
        {
        }

        public void CleanUp()
        {
            switch (mFightResult)
            {
                case FightResult.DRAW:
                    Parent.DestroyGO(mPlayerUnit);
                    Parent.DestroyGO(mEnemyUnit);
                    break;
                case FightResult.ENEMY_HIT_BASE:
                case FightResult.PLAYER_WIN:
                    Parent.DestroyGO(mEnemyUnit);
                    break;
                case FightResult.PLAYER_HIT_BASE :
                case FightResult.ENEMY_WIN:
                    Parent.DestroyGO(mPlayerUnit);
                    break;
            }
        }

        private void DamageBase()
        {
            bool playerBase = mFightResult == FightResult.ENEMY_HIT_BASE;

            UnitData data = playerBase ? mEnemyUnit.GetComponent<Unit>().GetData() : mPlayerUnit.GetComponent<Unit>().GetData();

            Parent.DamageBase(playerBase, data.UnitAttackTypeList.Count);
        }

        private void AnimationFinish()
        {
            IsDone = true;
        }

        private FightResult UnitFightResult()
        {
            FightResult result = FightResult.PLAYER_HIT_BASE;

            UnitData playerUnitData = mPlayerUnit == null? null: mPlayerUnit.GetComponent<Unit>().GetData();
            UnitData enemyUnitData= mEnemyUnit == null? null: mEnemyUnit.GetComponent<Unit>().GetData();

            if (playerUnitData != null && enemyUnitData != null)
            {
                int playerPower = playerUnitData.UnitAttackTypeList.Count;
                int enemyPower = enemyUnitData.UnitAttackTypeList.Count;

                //Current logic assumes that each unit only have 1 type of attack type
                UnitData.UnitAttackType playerAttackType = playerUnitData.UnitAttackTypeList[0];
                UnitData.UnitAttackType enemyAttackType = enemyUnitData.UnitAttackTypeList[0];

                if (playerAttackType == enemyAttackType)
                {
                    //draw
                }
                else
                {
                    //SCISSOR = 3, PAPER = 2, STONE = 1
                    if (playerAttackType > enemyAttackType)
                    {
                        if (playerAttackType - enemyAttackType > 1 && enemyAttackType == UnitData.UnitAttackType.STONE)
                            enemyPower++;
                        else
                            playerPower++;
                    }
                    //flip the top logic
                    else
                    {
                        if (enemyAttackType - playerAttackType > 1 && playerAttackType == UnitData.UnitAttackType.STONE)
                            playerPower++;
                        else
                            enemyPower++;
                    }
                }

                if (playerPower == enemyPower)
                {
                    result = FightResult.DRAW;
                    Debug.Log(mPlayerUnit.name + " vs " + mEnemyUnit.name + " : DRAW");
                }
                else if (playerPower > enemyPower)
                {
                    result = FightResult.PLAYER_WIN;
                    Debug.Log(mPlayerUnit.name + " vs " + mEnemyUnit.name + " : WIN");
                }
                else
                {
                    result = FightResult.ENEMY_WIN;
                    Debug.Log(mPlayerUnit.name + " vs " + mEnemyUnit.name + " : LOSE");
                }
            }
            else if (playerUnitData == null)
            {
                result = FightResult.ENEMY_HIT_BASE;
                Debug.Log(mEnemyUnit.name + " HIT BASE");
            }
            else
            {
                result = FightResult.PLAYER_HIT_BASE;
                Debug.Log(mPlayerUnit.name + " HIT BASE");
            }

            return result;
        }

    }
}