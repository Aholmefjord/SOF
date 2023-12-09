using AutumnInteractive.SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FvTGame
{
    public class LevelData {

        public struct WaveData
        {
            public List<UnitData> PlayerList;
            public List<UnitData> EnemyList;
        }

        private List<int> mPlayableLanes;
        public List<int> PlayableLanes { get { return mPlayableLanes;  } } 

        private int mCurrentStage;

        private int mCurrentWave;
        public int CurrentWave { get { return mCurrentWave;  } }
        private int mTotalWave;

        private WaveData mCurrentWaveData;
        public WaveData CurrentWaveData { get { return mCurrentWaveData; } }
        
        private JSONArray mLevelDataNode;
        private JSONNode mCurrentStageNode;

        private int mPlayerBaseHealth;
        public int PlayerBaseHealth { get { return mPlayerBaseHealth; } }
        private int mPlayerBaseMaxHealth;
        public int PlayerBaseMaxHealth { get { return mPlayerBaseMaxHealth; } }

        private int mEnemyBaseHealth;
        public int EnemyBaseHealth { get { return mEnemyBaseHealth; } }
        private int mEnemyBaseMaxHealth;
        public int EnemyBaseMaxHealth { get { return mEnemyBaseMaxHealth; } }

        public LevelData(string jsonText)
        {
            mLevelDataNode = JSON.Parse<JSONArray>(jsonText);
        }

        public void LoadStage(int _stage)
        {
            mCurrentStage = _stage;
            
            mCurrentStageNode = mLevelDataNode[_stage - 1];
            mPlayerBaseHealth = mCurrentStageNode["sally"].AsInt;
            mEnemyBaseHealth = mCurrentStageNode["kraken"].AsInt;

            mPlayerBaseMaxHealth = mPlayerBaseHealth;
            mEnemyBaseMaxHealth = mEnemyBaseHealth;

            mPlayableLanes = new List<int>();
            JSONArray lanes = mCurrentStageNode["lane"].AsArray;

            for (int i = 0; i < lanes.Count; ++i)
            {
                mPlayableLanes.Add(lanes[i].AsInt);
            }

            mCurrentWave = 0;
            CalculateWaveTotal();
        }

        public void LoadWave()
        {
            //Load wave content
            mCurrentWaveData = new WaveData();

            JSONNode waveNode = mCurrentStageNode["wave" + mCurrentWave];


            mCurrentWaveData.PlayerList = new List<UnitData>();
            mCurrentWaveData.EnemyList = new List<UnitData>();

            JSONArray playerUnits = waveNode["card"].AsArray;
            JSONArray enemyUnits = waveNode["wave"].AsArray;

            for (int i = 0; i < playerUnits.Count; ++i)
            {
                UnitData data = new UnitData();
                data.LoadDefaultData(GameLogic.GameData.GetString("player_unit_" + playerUnits[i]));
                mCurrentWaveData.PlayerList.Add(data);
            }

            for (int i = 0; i < enemyUnits.Count; ++i)
            {
                int unitID = enemyUnits[i].AsInt;
                if(unitID != 0) {
                    int x = (i % 3) + 4;
                    int y = 3 - (i / 3);

                    UnitData data = new UnitData();
                    data.LoadDefaultData(GameLogic.GameData.GetString("enemy_unit_" + unitID));

                    data.GridLocation = new IntVector2(x, y);
                    mCurrentWaveData.EnemyList.Add(data);
                }
            }
        }

        private void CalculateWaveTotal()
        {
            mTotalWave = 0;

            JSONNode wave;

            do
            {
                mTotalWave++;
                wave = mCurrentStageNode["wave" + mTotalWave];
            }
            while (wave != null);

            mTotalWave--;

        }

        public void AdvanceWave()
        {
            mCurrentWave++;
            LoadWave();
        }

        public bool IsStageCompleted()
        {
            return mCurrentWave >= mTotalWave;
        }

        public bool IsGameCompleted()
        {
            return mPlayerBaseHealth == 0 || mEnemyBaseHealth == 0;
        }

        public void DamagePlayerBase(int _damage)
        {
            mPlayerBaseHealth -= _damage;

            if (mPlayerBaseHealth <= 0)
                mPlayerBaseHealth = 0;
        }

        public void DamageEnemyBase(int _damage)
        {
            mEnemyBaseHealth -= _damage;

            if (mEnemyBaseHealth <= 0)
                mEnemyBaseHealth = 0;
        }
    }
}