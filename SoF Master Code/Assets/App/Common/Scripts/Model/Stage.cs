using SimpleJSON;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Stage
{
	public int id;
	public string name;
	public int order;
	public int puzzleType;
	public int puzzleLevel;
	public int star1ScoreNeeded;
	public int star2ScoreNeeded;
	public int star3ScoreNeeded;
	public int achievementId;
	public int allowedMoves;
	public int[,]piecesParam;
	public int[,]panelParam;
	public string tutorialDialogueID;
	public JSONNode puzzleParams;

	public static bool toTheHouseCleared()
	{
		foreach (KeyValuePair<int, PlayerAchievement> pa in GameState.me.achievements)
		{
			if (pa.Value.achievementId >= 1)
			{
				return true;
			}
		}

		return false;
	}

	public void init(JSONNode json)
	{
		if (json["Id"] != null)
		{
			id = json["Id"].AsInt;
		}

		if (json["AchievementId"] != null)
		{
			achievementId = json["AchievementId"].AsInt;
		}

		if (json["Name"] != null)
		{
			name = json["Name"].Value;
		}

		if (json["Order"] != null)
		{
			order = json["Order"].AsInt;
		}
		if (json["PuzzleType"] != null)
		{
			puzzleType = json["PuzzleType"].AsInt;
		}

		if (json["Level"] != null)
		{
			puzzleLevel = json["Level"].AsInt;
		}

		if (json["Star1"] != null)
		{
			star1ScoreNeeded = json["Star1"].AsInt;
		}

		if (json["Star2"] != null)
		{
			star2ScoreNeeded = json["Star2"].AsInt;
		}

		if (json["Star3"] != null)
		{
			star3ScoreNeeded = json["Star3"].AsInt;
		}

		if (json["AllowedMoves"] != null)
		{
			allowedMoves = json["AllowedMoves"].AsInt;
		}

		if (json["PuzzleParams"] != null)
		{
			puzzleParams = json["PuzzleParams"];
		}
		if (json["TutorialDialogue"] != null)
		{
			tutorialDialogueID = json["TutorialDialogue"];
		}

		if (json["PieceParams"] != null)
		{
			//Debug.Log("json check:"+json["PieceParams"].AsArray[0][0]);
			//piecesParam = json["PieceParams"].AsArray;
			piecesParam = new int [json["PieceParams"].AsArray.Count,json["PieceParams"].AsArray[0].Count];
			for(int x = 0 ; x<json["PieceParams"].AsArray[0].Count; x++){
				for(int y = 0 ; y<json["PieceParams"].AsArray.Count; y++){
					piecesParam[x,y] = json["PieceParams"].AsArray[x][y].AsInt;
				}
			}

		}

		if (json["PanelParams"] != null)
		{
			panelParam = new int [json["PanelParams"].AsArray.Count,json["PanelParams"].AsArray[0].Count];
			for(int x = 0 ; x<json["PanelParams"].AsArray[0].Count; x++){
				for(int y = 0 ; y<json["PanelParams"].AsArray.Count; y++){
					panelParam[x,y] = json["PanelParams"].AsArray[x][y].AsInt;
				}
			}
		}
	}
}