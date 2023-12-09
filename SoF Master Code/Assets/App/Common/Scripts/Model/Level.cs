using SimpleJSON;
using UnityEngine;

public class Level {
	public static int MAX_PLAYER_LEVEL = 50; 

	public static int GetPlayerLevelByXp(int xp) {
		float xpf = (float) xp + 500f; //Need to cast to float to prevent rounding before results
		float lvl = Mathf.Sqrt(xpf * 0.002f);
		if (lvl < 1) lvl = 1;
		return Mathf.FloorToInt(lvl);
	}

	public static int GetPlayerXpByLevel(int lvl) {
		return (lvl * lvl - 1) * 500;
	}

	public static int GetLevelByXp(int xp) {
		float xpf = (float) xp + 100f; //Need to cast to float to prevent rounding before results
		float lvl = Mathf.Sqrt(xpf) * 0.1f;
		if (lvl < 1) lvl = 1;
		return Mathf.FloorToInt(lvl);
	}

	public static int GetXpByLevel(int lvl) {
		return (lvl * lvl - 1) * 100;
	}


}