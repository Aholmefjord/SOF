using AutumnInteractive.SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Tom: seems to be another level editor that works runtime; not converting to work with assetbundle
/// </summary>
public class AllPuzzleEditScript : MonoBehaviour {

    private JSONArray _allPuzzles; // contains everything else
    private static JSONClass _tangramPieceData;
    public static JSONClass TangramPieceData
    {
        get
        {
            if (_tangramPieceData == null)
            {
                var jsonText = Resources.Load<TextAsset>("json/tangram/pieces").text;
                _tangramPieceData = JSON.Parse<JSONClass>(jsonText);
            }
            return _tangramPieceData;
        }
    }

    public GameObject puzzleNum;
    public GameObject puzzleDifficulty;
    public GameObject puzzleCount;
    public GameObject solutionCount;
    
    private int currentStageNum;

    // Use this for initialization
    void Start()
    {
        _allPuzzles = JSON.Parse<JSONArray>(CachedResources.Load<TextAsset>("json/tangram/puzzles").text);

        currentStageNum = 1;

        HelpMeFind();

        LoadContent();
    }

    void HelpMeFind()
    {
        string result = DetailInformation();
        
        File.WriteAllText(System.Environment.CurrentDirectory + "/Assets/Autumn/Tangram/Resources/json/tangram/" + @"\result.txt", result);

        Debug.Log("Help Me Find Done");
    }

    string ArrangePuzzlesByNumberOfSolutionPiece(bool ignoreSinglePiece = false)
    {
        string result = "";

        //Arrange all puzzles
        ArrayList list = new ArrayList();

        for (int i = 0; i < _allPuzzles.Count; ++i)
        {
            var puzzle = _allPuzzles[i];

            int count = puzzle["inventory"]["pieces"].Count;

            if (ignoreSinglePiece)
            {
                for (int k = 0; k < puzzle["inventory"]["pieces"].Count; k++)
                {
                    var piece = puzzle["inventory"]["pieces"][k];

                    string test = piece["name"];
                    if (test.Equals("s1"))
                        count--;
                }
            }

            list.Add(count);
        }

        //Now I need to sort the list out
        var sortedList = new Dictionary<int, int>();
        
        while (list.Count > 0)
        {
            int num = (int)list[0];

            if (sortedList.ContainsKey(num))
            {
                sortedList[num] = sortedList[num] + 1;
            }
            else
            {
                sortedList.Add(num, 1);
            }

            list.RemoveAt(0);
        }

        //get out the list
        foreach (KeyValuePair<int, int> entry in sortedList)
        {
            result += "Solution piece: ";
            result += entry.Key;
            result += " Count: ";
            result += entry.Value;

            result += System.Environment.NewLine;
        }

        return result;
    }

    string PuzzlesThatHasMoreThanOneSingleDot()
    {
        string result = "";

        //Arrange all puzzles
        var list = new Dictionary<int, int>();

        for (int i = 0; i < _allPuzzles.Count; ++i)
        {
            var puzzle = _allPuzzles[i];

            int count = 0;
            
            for (int k = 0; k < puzzle["inventory"]["pieces"].Count; k++)
            {
                var piece = puzzle["inventory"]["pieces"][k];

                string test = piece["name"];
                if (test.Equals("s1"))
                    count++;
            }

            if (count <= 1)
                continue;

            list.Add(i, count);
        }
        

        //get out the list
        foreach (KeyValuePair<int, int> entry in list)
        {
            result += "Puzzle ID: ";
            result += entry.Key;
            result += "Single Dot Count: ";
            result += entry.Value;

            result += System.Environment.NewLine;
        }

        return result;
    }

    string PuzzlesThatOnlyHasOneSingleDot()
    {
        string result = "";

        //Arrange all puzzles
        var list = new Dictionary<int, int>();

        for (int i = 0; i < _allPuzzles.Count; ++i)
        {
            var puzzle = _allPuzzles[i];

            int count = 0;

            for (int k = 0; k < puzzle["inventory"]["pieces"].Count; k++)
            {
                var piece = puzzle["inventory"]["pieces"][k];

                string test = piece["name"];
                if (test.Equals("s1"))
                    count++;
            }
            
            if(count == 1)
                list.Add(i, count);
        }


        //get out the list
        foreach (KeyValuePair<int, int> entry in list)
        {
            result += "Puzzle ID: ";
            result += entry.Key;

            result += System.Environment.NewLine;
        }

        return result;
    }

    string PuzzlesThatOnlyHasNoSingleDot()
    {
        string result = "Puzzle that has no single dot:";
        result += System.Environment.NewLine;

        //Arrange all puzzles
        var list = new Dictionary<int, int>();

        for (int i = 0; i < _allPuzzles.Count; ++i)
        {
            var puzzle = _allPuzzles[i];

            int count = 0;

            for (int k = 0; k < puzzle["inventory"]["pieces"].Count; k++)
            {
                var piece = puzzle["inventory"]["pieces"][k];

                string test = piece["name"];
                if (test.Equals("s1"))
                    count++;
            }

            if (count == 0)
                list.Add(i, count);
        }


        //get out the list
        foreach (KeyValuePair<int, int> entry in list)
        {
            result += "Puzzle ID: ";
            result += entry.Key;

            result += System.Environment.NewLine;
        }

        return result;
    }

    string DetailInformation()
    {
        string result = "Detail Information of all Puzzles:";
        result += System.Environment.NewLine;
        
        for (int i = 0; i < _allPuzzles.Count; ++i)
        {
            var puzzle = _allPuzzles[i];

            int SolutionCount = puzzle["inventory"]["pieces"].Count;
            int SingleDotCount = 0;
            int Difficulty = puzzle["difficulty"].AsInt;

            for (int k = 0; k < puzzle["inventory"]["pieces"].Count; k++)
            {
                var piece = puzzle["inventory"]["pieces"][k];

                string test = piece["name"];
                if (test.Equals("s1"))
                    SingleDotCount++;
            }

            result += "Puzzle ID: ";
            result += (i + 1).ToString();
            result += " Solution Count: ";
            result += SolutionCount;
            result += " Single Dot Count: ";
            result += SingleDotCount;
            result += " Difficulty: ";
            result += Difficulty;

            result += System.Environment.NewLine;
        }

        Debug.Log("Detail information done");

        return result;
    }

    void ExecuteAction()
    {
        //I want to set all puzzles that has more solution size than puzzle size to difficulty 4
        for (int i = 0; i < _allPuzzles.Count; ++i)
        {
            var puzzle = _allPuzzles[i];

            var blueprint = puzzle["blueprint"]["pieces"].AsArray;
            var inventory = puzzle["inventory"]["pieces"].AsArray;

            int puzzleSize = 0;
            for (int k = 0; k < blueprint.Count; ++k)
            {
                puzzleSize += TangramPieceData[blueprint[k].GetEntry<string>("name")].Count;
            }

            int solutionSize = 0;
            for (int k = 0; k < inventory.Count; ++k)
            {
                solutionSize += TangramPieceData[inventory[k].GetEntry<string>("name")].Count;
            }

            if (solutionSize > puzzleSize)
            {
                puzzle["difficulty"] = "4";
            }
        }

        File.WriteAllText(System.Environment.CurrentDirectory + "/Assets/Autumn/Tangram/Resources/json/tangram/" + @"\puzzles.json", _allPuzzles.ToString());    
    }

    public void Save()
    {
        //Write out the JSON
        var puzzle = _allPuzzles[currentStageNum - 1];
        puzzle["difficulty"] = puzzleDifficulty.GetComponent<InputField>().text;

        File.WriteAllText(System.Environment.CurrentDirectory + "/Assets/Autumn/Tangram/Resources/json/tangram/" + @"\puzzles_test.json", _allPuzzles.ToString());
    }

    public void Search()
    {
        //    Save();

        currentStageNum = int.Parse(puzzleNum.GetComponent<InputField>().text);

        if (currentStageNum >= _allPuzzles.Count)
            currentStageNum = _allPuzzles.Count - 1;

        if (currentStageNum <= 1)
            currentStageNum = 1;

        LoadContent();
    }

    public void Next()
    {
        //    Save();

        currentStageNum++;

        if (currentStageNum > _allPuzzles.Count)
            currentStageNum = _allPuzzles.Count;

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
        var puzzle = _allPuzzles[currentStageNum - 1];

        puzzleNum.GetComponent<InputField>().text = currentStageNum.ToString();
        puzzleDifficulty.GetComponent<InputField>().text = puzzle.GetEntry<string>("difficulty");
        
        var blueprint = puzzle["blueprint"]["pieces"].AsArray;
        var inventory = puzzle["inventory"]["pieces"].AsArray;

        int puzzleSize = 0;
        for (int i = 0; i < blueprint.Count; ++i)
        {
           puzzleSize += TangramPieceData[blueprint[i].GetEntry<string>("name")].Count;
        }

        int solutionSize = 0;
        for (int i = 0; i < inventory.Count; ++i)
        {
            solutionSize += TangramPieceData[inventory[i].GetEntry<string>("name")].Count;
        }

        puzzleCount.GetComponent<Text>().text = puzzleSize.ToString();
        solutionCount.GetComponent<Text>().text = solutionSize.ToString();
    }

    public static void ExportPieceSizeData()
    {
        string exportText = "";
        for (int i = 0; i < TangramPieceData.Count; ++i)
        {
            var obj = _tangramPieceData[i];

            exportText += "Piece: ";
            exportText += _tangramPieceData.GetKey(i);
            exportText += ". Count : ";
            exportText += obj.Count.ToString();
            exportText += System.Environment.NewLine;
        }

        File.WriteAllText(System.Environment.CurrentDirectory + "/Assets/Autumn/Tangram/Resources/json/tangram/" + @"\pieceData.txt", exportText);
    }
}
