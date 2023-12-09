using AutumnInteractive.SimpleJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FvTProgression
{

    public int levelCount = 30;
    public int selectedLevel;
    public List<LevelData> levelDataList = new List<LevelData>();



    public void Load()
    {
        var jsonDataText = PlayerPrefs.GetString(GameState.me.id + "." + GameState.me.username + ".FvT_player_progression", "{}");
        var jsonData = JSON.Parse<JSONClass>(jsonDataText);
        Debug.Log(jsonDataText);
        Init(jsonData);
    }
    public void Load(string s)
    {
        var jsonData = JSON.Parse<JSONClass>(s);
        Init(jsonData);
    }
    public string SaveAsString()
    {
        var jsonData = Serialize();
        return jsonData.ToString();
    }
    public void Init(JSONClass jsonData)
    {
        levelCount = jsonData.GetEntry("level_count", 30);

        levelDataList.Clear();
        foreach (JSONClass levelJson in jsonData.GetJson("levels", new JSONArray()))
        {
            levelDataList.Add(new LevelData(levelJson));
        }
    }

    public void Save()
    {
        var jsonData = Serialize();
        PlayerPrefs.SetString(GameState.me.id + "." + GameState.me.username + ".FvT_player_progression", jsonData.ToString());
    }


    public JSONClass Serialize()
    {
        var jsonData = new JSONClass();

        jsonData.Add("level_count", new JSONData(levelCount));

        var levelList = new JSONArray();
        foreach (var level in levelDataList)
        {
            levelList.Add(level.Serialize());
        }
        jsonData.Add("levels", levelList);

        var critterList = new JSONClass();
        jsonData.Add("critters", critterList);

        return jsonData;
    }

    public LevelData GetLevel(int levelId)
    {
        levelId = Mathf.Min(levelId, levelCount - 1);
        while (levelId >= levelDataList.Count)
        {
            levelDataList.Add(new LevelData(levelDataList.Count));
        }
        return levelDataList[levelId];
    }

    public void SetLevel(int levelId, LevelData levelData)
    {
        if (levelId >= levelCount) return;
        while (levelId >= levelDataList.Count)
        {
            levelDataList.Add(new LevelData(levelDataList.Count));
        }
        levelDataList[levelId] = levelData;
    }

    public struct LevelData
    {
        public int levelId;
        public int starEarned;
        public int pointsEarned;
        public ELevelStatus status;

        public LevelData(int levelId)
        {
            this.levelId = levelId;
            starEarned = 0;
            pointsEarned = 0;
            status = levelId == 0 ? ELevelStatus.Available : ELevelStatus.Locked;
        }

        public LevelData(JSONClass jsonData)
        {
            levelId = jsonData.GetEntry("level_id", 0);
            starEarned = jsonData.GetEntry("stars", 0);
            pointsEarned = jsonData.GetEntry("points", 0);
            status = (ELevelStatus)Enum.Parse(typeof(ELevelStatus), jsonData.GetEntry("status", "Locked"));
        }

        public JSONClass Serialize()
        {
            var jsonData = new JSONClass();
            jsonData.Add("level_id", new JSONData(levelId));
            jsonData.Add("stars", new JSONData(starEarned));
            jsonData.Add("points", new JSONData(pointsEarned));
            jsonData.Add("status", status.ToString());
            return jsonData;
        }
    }

    public enum ELevelStatus
    {
        Locked,
        Available,
        Finished
    }

    public void unlock()
    {
        Debug.LogError("UNLOCK");
        for (int i = 0; i < levelCount; i++)
        {
            if (GetLevel(i).status.ToString() == "Locked")
            {
                Debug.LogError("UNLOCK");
                LevelData asd;
                asd.levelId = GetLevel(i).levelId;
                asd.pointsEarned = 0;
                asd.starEarned = 0;
                asd.status = ELevelStatus.Available;
                SetLevel(i, asd);
            }
        }
    }
}
