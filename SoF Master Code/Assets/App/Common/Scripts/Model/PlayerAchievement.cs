using SimpleJSON;
using UnityEngine;
using System;

public class PlayerAchievement
{
	public int id;
	public long userId;
	public int achievementId;
	public bool rewardClaimed;
	public int progress;
	public DateTime completedTime;

	public void init(JSONNode json)
	{
		if (json["Id"] != null)
		{
			id = json["Id"].AsInt;
		}

		if (json["UserId"] != null)
		{
			userId = json["UserId"].AsLong;
		}

		if (json["AchievementId"] != null)
		{
			achievementId = json["AchievementId"].AsInt;
		}

		if (json["RewardClaimed"] != null)
		{
			rewardClaimed = json["RewardClaimed"].AsBool;
		}

		if (json["Progress"] != null)
		{
			progress = json["Progress"].AsInt;
		}
	}
}