using UnityEngine;
using System;
using System.Collections.Generic;
using AutumnInteractive.SimpleJSON;

public class TsumPlayerProgression{

    
    public int levelCount = 100;
    public int selectedLevel;
    public List<LevelData> levelDataList = new List<LevelData> ();
    private Dictionary<string, CritterData> critterDataList = new Dictionary<string, CritterData> ();
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

    public void Load () {
        var jsonDataText = PlayerPrefs.GetString (GameState.me.id + "." + GameState.me.username + ".tsum_player_progression", "{}");
        var jsonData = JSON.Parse<JSONClass> (jsonDataText);
        Init (jsonData);
    }

    public void Init (JSONClass jsonData) {
        levelCount = jsonData.GetEntry ("level_count", 100);

        levelDataList.Clear ();
        foreach (JSONClass levelJson in jsonData.GetJson ("levels", new JSONArray ())) {
            levelDataList.Add (new LevelData (levelJson));
        }

        critterDataList.Clear ();
        var critterListJson = jsonData.GetJson ("critters", new JSONClass ());
        foreach (string critterName in critterListJson.GetKeys ()) {
			// Deserialize the critter data
            critterDataList.Add (critterName, new CritterData (critterListJson.GetJson (critterName, new JSONClass ())));
        }
    }
    public void Save () {
        var jsonData = Serialize ();
        PlayerPrefs.SetString (GameState.me.id + "." + GameState.me.username + ".tsum_player_progression", jsonData.ToString ());
    }

    public JSONClass Serialize () {
        var jsonData = new JSONClass ();

        jsonData.Add ("level_count", new JSONData (levelCount));

        var levelList = new JSONArray ();
        foreach (var level in levelDataList) {
            levelList.Add (level.Serialize ());
        }
        jsonData.Add ("levels", levelList);

        var critterList = new JSONClass ();
        foreach (var critterName in critterDataList.Keys) {
            critterList.Add (critterName, critterDataList[critterName].Serialize ());
        }
        jsonData.Add ("critters", critterList);

        return jsonData;
    }

    public LevelData GetLevel (int levelId) {
        levelId = Mathf.Min (levelId, levelCount - 1);
        while (levelId >= levelDataList.Count) {
            levelDataList.Add (new LevelData (levelDataList.Count));
        }
        return levelDataList[levelId];
    }

    public void SetLevel (int levelId, LevelData levelData) {
        if (levelId >= levelCount) return;
        while (levelId >= levelDataList.Count) {
            levelDataList.Add (new LevelData (levelDataList.Count));
        }
        levelDataList[levelId] = levelData;
    }

    public CritterData GetCritter (string name) {
        if (!critterDataList.ContainsKey (name)) {
            critterDataList.Add (name, new CritterData (name));
        }
        return critterDataList[name];
    }

    public void SetCritter (string name, CritterData critterData) {
        if (critterDataList.ContainsKey (name)) {
            critterDataList[name] = critterData;
        } else {
            critterDataList.Add (name, critterData);
        }
    }

    public struct LevelData {
        public int levelId;
        public int starEarned;
        public int pointsEarned;
        public ELevelStatus status;
        public bool needAnimation;

        public LevelData (int levelId) {
            this.levelId = levelId;
            starEarned = 0;
            pointsEarned = 0;
            status = levelId == 0 ? ELevelStatus.Available : ELevelStatus.Locked;
            needAnimation = false;
        }

        public LevelData (JSONClass jsonData) {
            levelId = jsonData.GetEntry ("level_id", 0);
            starEarned = jsonData.GetEntry ("stars", 0);
            pointsEarned = jsonData.GetEntry ("points", 0);
            status = (ELevelStatus) Enum.Parse (typeof (ELevelStatus), jsonData.GetEntry ("status", "Locked"));
            needAnimation = jsonData.GetEntry ("animation", false);
        }

        public JSONClass Serialize () {
            var jsonData = new JSONClass ();
            jsonData.Add ("level_id", new JSONData (levelId));
            jsonData.Add ("stars", new JSONData (starEarned));
            jsonData.Add ("points", new JSONData (pointsEarned));
            jsonData.Add ("status", status.ToString ());
            jsonData.Add ("animation", new JSONData (needAnimation));
            return jsonData;
        }
    }

    public struct CritterData {
        public string name;
        public int level;
        public int points;
        public int maxPoints;

        public CritterData (string name) {
            this.name = name;
            level = 1;
            points = 0;
            maxPoints = 1;
        }

        public CritterData (JSONClass jsonData) {
            name = jsonData.GetEntry ("name", string.Empty);
            level = jsonData.GetEntry ("level", 1);
            points = jsonData.GetEntry ("points", 0);
            maxPoints = jsonData.GetEntry ("max_points", 1);
        }

        public JSONClass Serialize () {
            var jsonData = new JSONClass ();
            jsonData.Add ("name", name);
            jsonData.Add ("level", new JSONData (level));
            jsonData.Add ("points", new JSONData (points));
            jsonData.Add ("max_points", new JSONData (maxPoints));
            return jsonData;
        }
    }

    public enum ELevelStatus {
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
				asd.needAnimation = false;
				asd.status = ELevelStatus.Available;
				SetLevel(i, asd);
			}
		}
	}
}
