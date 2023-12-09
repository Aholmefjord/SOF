using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeHelper
{
	private static int timeOffset = 0;
	private static System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

	public const int MIN = 60;
	public const int HOUR = 3600;
	public const int DAY = 86400;

	public static int ServerEpochTime
	{
		get 
		{
			return (int)(System.DateTime.UtcNow - epochStart).TotalSeconds + timeOffset;
		}
	}

	public static int CurrentEpochTime
	{
		get 
		{
			return (int)(System.DateTime.UtcNow - epochStart).TotalSeconds;
		}
	}

	public static void SetTimeOffset(int _timeOffset)
	{
		timeOffset = _timeOffset;
	}

    //epoch int is also known as unix time
    public static System.DateTime DateTime(int epoch)
    {
        System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return dateTime.AddSeconds(epoch);
    }
}
