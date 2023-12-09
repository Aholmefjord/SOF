using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UniRx;
using SimpleJSON;
//using System.
public class CodeBackend : MonoBehaviour {

    // Public Variables
    public NewMapController nmc;
    public List<RewardClass> classes;
    public GameObject errorPopup;
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClaimCode(string code)
    {
        for (int i = 0; i < classes.Count; i++)
        {
            if (code.Equals(classes[i].code))
            {
				Debug.LogError("disabled sfx");
				//AudioSystem.PlaySFX("UI/sfx_ui_click");
//                Success(classes[i]);
                unlock(classes[i].game_id);
                return;
            }
        }
		Debug.LogError("disabled sfx");
		//AudioSystem.PlaySFX("UI/sfx_ui_cancel");
        Failure(code);
    }
    public void unlock(string game_id)
    {
		// HOWARD - Deprecated network code - TO REPLACE
//        WWWForm form = new WWWForm();
//        form.AddField("token", GameState.julesAccessToken);
//        form.AddField("game_id", game_id);
//        form.AddField("user_id", GameState.me.username);
//        form.AddField("setVal", 0);
//        Debug.Log("Unlocking Game: me/user/unlock_game" + game_id + " for " + GameState.me.username);
//        AppServer.CreatePost("me/user/unlock_game" + game_id, form)
//        .Subscribe(
//            x => SuccessGet(x), // onSuccess
//            ex => AppServer.ErrorResponse(ex, "Error Update Password") // onError
//        );
    }

    public void GetIDs()
    {
        nmc.refreshunlocks();
    }

    public void SuccessGet(string x)
    {
        Debug.Log(x);
        gameObject.SetActive(false);
        GetIDs();
    }

    public void Success(RewardClass cls){
        //TODO: add in backend
        cls.claim();
        Debug.Log("Succses: " + cls.code);
    }

    public void Failure(string code)
    {
        //TODO: add in code to show when the code doesn't exist
        errorPopup.SetActive(true);
        gameObject.SetActive(false);
        Debug.Log("Failure: " + code);
    }

    public void Close()
    {
        errorPopup.SetActive(false);
    }
}

