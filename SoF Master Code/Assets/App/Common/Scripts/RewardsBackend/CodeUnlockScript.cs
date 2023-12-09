using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UniRx;
using SimpleJSON;
public class CodeUnlockScript : MonoBehaviour {

    CodeBackend backend;
    public InputField input;

	// Use this for initialization
	void Start () {
        backend = GetComponent<CodeBackend>();
	}

    public void OnClick()
    {
          backend.ClaimCode(input.text);
          //backend.Blah("unlock_game_1");
          //backend.GetIDs();
    }
}
