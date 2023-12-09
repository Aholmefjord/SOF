using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UniRx;
using SimpleJSON;

public class DiaryController : MonoBehaviour {
	public CanvasGroup canvasGroup;
	public Text nameText;
	public Text emailText;
	public Text referralCodeText;

	public GameObject achievementHolder;
	public CanvasGroup confirmEmailCanvasGroup;
	public CanvasGroup confirmPasswordCanvasGroup;

	public InputField setEmailText;
	public InputField setPasswordText;
	public InputField setPasswordConfirmText;
    public GameObject passwordChangeField;
	void Awake()
	{
		Hide();
	}

	void Hide()
	{
        try
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 0;
        }catch(System.Exception e)
        {

        }
        
	}

	void HideSetEmailPanel()
	{
		confirmEmailCanvasGroup.interactable = false;
		confirmEmailCanvasGroup.blocksRaycasts = false;
		confirmEmailCanvasGroup.alpha = 0;
	}

	void HideSetPasswordPanel()
	{
        if (passwordChangeField != null)
        {
            passwordChangeField.SetActive(false);
        }
        else
        {
            confirmPasswordCanvasGroup.interactable = false;
            confirmPasswordCanvasGroup.blocksRaycasts = false;
            confirmPasswordCanvasGroup.alpha = 0;

        }
	}

	void ShowSetEmailPanel()
	{
		confirmEmailCanvasGroup.interactable = true;
		confirmEmailCanvasGroup.blocksRaycasts = true;
		confirmEmailCanvasGroup.alpha = 1;
	}

	void ShowSetPasswordPanel()
	{
		confirmPasswordCanvasGroup.interactable = true;
		confirmPasswordCanvasGroup.blocksRaycasts = true;
		confirmPasswordCanvasGroup.alpha = 1;
	}

	void Show()
	{
		canvasGroup.interactable = true;
		canvasGroup.blocksRaycasts = true;
		canvasGroup.alpha = 1;
		HideSetEmailPanel();
		HideSetPasswordPanel();

		nameText.text = "Name: " + GameState.me.name;
		bool isValidEmail = Regex.IsMatch(GameState.me.email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
		if (isValidEmail)
		{
			emailText.text = "Email: " + GameState.me.email;
		} else {
			emailText.text = "Email: Not set";
		}

		// Destroy old pet objects if any
		foreach (Transform g in achievementHolder.GetComponentsInChildren<Transform>())
		{
			if (g.gameObject != achievementHolder)
			{
				Destroy(g.gameObject);
			}
		}

		foreach (KeyValuePair<int, PlayerAchievement> pa in GameState.me.achievements)
		{
			GameObject panel = Instantiate(Resources.Load("gui/panels/AchievementPanel")) as GameObject;
			AchievementPanel ap = panel.GetComponent<AchievementPanel>();
			//Temp
			int stage = pa.Value.achievementId;
			if (stage > 1) stage -= 2;

			ap.titleText.text = "Stage " + stage + " Progress: ";
			ap.valueText.text = pa.Value.progress.ToString();
			panel.transform.SetParent(achievementHolder.transform,false);
		}
	}

	public void HideButtonClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");
		Hide();
	}

	public void HideSetEmailButtonClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");
		HideSetEmailPanel();
	}

	public void HideSetPasswordButtonClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");
		HideSetPasswordPanel();
	}

	public void ShowButtonClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");
		Show();
	}

	public void SetEmailButtonClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");
		ShowSetEmailPanel();
	}

	public void SetPasswordButtonClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");
		ShowSetPasswordPanel();
	}

	public void SubmitSetEmailButtonClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");

		// HOWARD - Deprecated network code - TO REPLACE
//		WWWForm form = new WWWForm();
//		form.AddField("token", GameState.julesAccessToken);
//		form.AddField("email", setEmailText.text);
//
//		DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_processing"), "", DialogueBoxController.Type.NoButtons);
//
//		// observe the http request
//		AppServer.CreatePost("me/email/update", form)
//		.Subscribe(
//			x => SetEmailResult(x), // onSuccess
//			ex => AppServer.ErrorResponse(ex, "Error Update Email") // onError
//		);
	}

	void SetEmailResult(string res)
	{
#if UNITY_EDITOR
		print("SetEmailResult: " + res);
#endif
		DialogueBoxController.Hide();
		JSONNode t = JSONNode.Parse(res);
		if (t == null || t["Status"] == null || t["Status"] == "0") // Null = has error.
		{
			if (t["msg"] != null)
			{
				DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_error_updating_email") + t["msg"].Value, MultiLanguage.getInstance().getString("dialog_box_error_title"));
				return;
			}
			DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_error_updating_email_please_try_again"), MultiLanguage.getInstance().getString("dialog_box_error_title"));
			return;
		}
		HideSetEmailPanel();
		emailText.text = "Email: " + setEmailText.text;
	}

	public void SubmitSetPasswordButtonClick()
	{
		AudioSystem.PlaySFX("ui/sfx_ui_click");

		// HOWARD - Deprecated network code - TO REPLACE
//		WWWForm form = new WWWForm();
//		form.AddField("token", GameState.julesAccessToken);
//		form.AddField("new_password", setPasswordText.text);		
//		DialogueBoxController.ShowMessage(MultiLanguage.getInstance().getString("dialog_box_processing"), "", DialogueBoxController.Type.NoButtons);
//        Debug.Log("Creating Password Set");
//		// observe the http request
//		AppServer.CreatePost("me/password/update", form)
//		.Subscribe(
//			x => SetPasswordResult(x), // onSuccess
//			ex => AppServer.ErrorResponse(ex, "Error Update Password") // onError
//		);
	}

	void SetPasswordResult(string res)
	{
#if UNITY_EDITOR
		print("SetPasswordResult: " + res);
#endif
		DialogueBoxController.Hide();
		HideSetPasswordPanel();
	}
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
