using UnityEngine;
using UniRx;

// HOWARD - OMG TO DO REPLACE EVERYTHING T_T
public class AnalyticsController : MonoBehaviour
{
	static public void Add(int puzzleType, string key, int value)
	{
		Send("add", puzzleType, key, value);
	}

	static public void Replace(int puzzleType, string key, int value)
	{
		Send("replace", puzzleType, key, value);
	}

	static public void Increase(int puzzleType, string key, int value = 1)
	{
		Send("increase", puzzleType, key, value);
	}

	static void Send(string method, int puzzleType, string key, int value)
	{
        StringHelper.DebugPrint("Send");
#if UNITY_EDITOR
        print("SendStat: " + key + ", " + value);
#endif

		if (GameState.julesAccessToken == null || GameState.julesAccessToken == "")
		{
			//Probably testing and hence no login details. Do not attempt to send.
			return;
		}

		// HOWARD - Deprecated network code - TO REPLACE
//		WWWForm form = new WWWForm();
//		//Note Constants.JULES_GAME_ID is derived from jules access token on server side. No need to send.
//		form.AddField("token", GameState.julesAccessToken);
//		form.AddField("sub_game_id", puzzleType.ToString());
//		form.AddField("key", key);
//		form.AddField("val", value.ToString());
//
//		AppServer.CreatePost("me/analytics/" + method, form)
//		.Subscribe(
//			x => ReceiveResult(x), // onSuccess
//			ex => AppServer.ErrorResponse(ex, "Error Get Player Session", false) // onError. No need to show if there is error.
//		);
	}

	static void ReceiveResult(string res)
	{
#if UNITY_EDITOR
		print("Analytics ReceiveResult: " + res);
#endif
	}
}