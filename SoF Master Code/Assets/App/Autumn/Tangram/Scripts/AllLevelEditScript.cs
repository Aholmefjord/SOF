using AutumnInteractive.SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tom: seems to be another level editor that works runtime; not converting to work with assetbundle
/// </summary>
public class AllLevelEditScript : MonoBehaviour {

    private JSONArray _allLevels; // contains everything else
    private JSONNode _maniaDesign; //contains puzzle count


    public GameObject stageNum;

    public GameObject puzzleNum;
    public GameObject puzzleTime;
    public GameObject puzzleList;


    private int currentStageNum;

    // Use this for initialization
    void Start () {
        TextAsset t = Resources.Load<TextAsset>("matchManiaDesign");

        _maniaDesign = JSONNode.Parse<JSONNode>(t.text);

        _allLevels = JSON.Parse<JSONArray>(CachedResources.Load<TextAsset>("json/tangram/all_levels").text);

        currentStageNum = 1;

        LoadContent();
    }
	
    public void Save()
    {
        //Write out the JSON
        var stage = _allLevels[currentStageNum - 1];
        stage["puzzle_time"] = puzzleTime.GetComponent<InputField>().text;
        stage["puzzles"] = puzzleList.GetComponent<InputField>().text.Replace(@"\", @"").Replace(@"\""", @"");
        
        //write out
        File.WriteAllText(Environment.CurrentDirectory + "/Assets/Autumn/Tangram/Resources/json/tangram/" + @"\all_levels.json", _allLevels.ToString());

        var puzzle = _maniaDesign["levelData"][currentStageNum - 1];
        puzzle["Puzzle_Count"] = puzzleNum.GetComponent<InputField>().text;

        File.WriteAllText(Environment.CurrentDirectory + "/Assets/LevelDesigns/Resources/" + @"\matchManiaDesign.json", _maniaDesign.ToString());
    }

    public void Search()
    {
    //    Save();

        currentStageNum = int.Parse(stageNum.GetComponent<InputField>().text);

        if (currentStageNum >= _allLevels.Count)
            currentStageNum = _allLevels.Count - 1;

        if (currentStageNum <= 1)
            currentStageNum = 1;

        LoadContent();
    }

    public void Next()
    {
    //    Save();

        currentStageNum++;

        if (currentStageNum > _allLevels.Count)
            currentStageNum = _allLevels.Count;

        LoadContent();
    }

    public void Previous()
    {
    //    Save();

        currentStageNum--;
        
        if (currentStageNum <= 1)
            currentStageNum = 1;

        LoadContent();
    }

    void LoadContent()
    {
        //Puzzle
        var stage = _allLevels[currentStageNum - 1];
        var puzzle = _maniaDesign["levelData"][currentStageNum - 1];

        stageNum.GetComponent<InputField>().text = currentStageNum.ToString();
        puzzleNum.GetComponent<InputField>().text = puzzle.GetEntry<string>("Puzzle_Count");
        puzzleTime.GetComponent<InputField>().text = stage.GetEntry<string>("puzzle_time");
        puzzleList.GetComponent<InputField>().text = stage.GetEntry<string>("puzzles");
    }
}
