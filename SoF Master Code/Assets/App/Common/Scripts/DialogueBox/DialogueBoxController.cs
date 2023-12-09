using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DialogueBoxController : MonoBehaviour
{
	private const string GAMEOBJECT_NAME = "DialogueBoxCanvas"; //Also resources prefab name
	private static GameObject thisCanvas;
	private static DialogueBoxInstance thisInstance;

	public enum Type
	{
		Standard = 1,
		NoButtons = 2,
		Confirm = 3,
		AutoClose = 4,
		Error = 5,
		Download = 6,
		Retry = 7,
        StandardWithCallBack = 8
	}

	static void Init()
	{
		//Ensure event system is present
		EventSystem[] eventSystem = FindObjectsOfType<EventSystem>();
		if (eventSystem.Length <= 0)
		{
			GameObject newEventSystem = new GameObject();
			newEventSystem.AddComponent<EventSystem>();
			newEventSystem.AddComponent<StandaloneInputModule>();
			newEventSystem.AddComponent<TouchInputModule>();
			newEventSystem.name = "EventSystem";
		}

		if (!thisCanvas)
		{
			//Create from resources if not already created
			thisCanvas = Instantiate(Resources.Load("gui/panels/" + GAMEOBJECT_NAME)) as GameObject;
			thisCanvas.transform.SetParent(null, false);
			thisCanvas.name = GAMEOBJECT_NAME;

            //Setup UI text objects with multilanguage
            MultiLanguage.getInstance().apply(thisCanvas.FindChild("TitleText"), "dialog_box_title_text");
            MultiLanguage.getInstance().apply(thisCanvas.FindChild("MessageText"), "dialog_box_message_text");
            MultiLanguage.getInstance().apply(thisCanvas.FindChild("ButtonYesText"), "dialog_box_button_yes");
            MultiLanguage.getInstance().apply(thisCanvas.FindChild("ButtonNoText"), "dialog_box_button_no");
            MultiLanguage.getInstance().apply(thisCanvas.FindChild("ButtonRetryText"), "dialog_box_button_retry");
        }
		else
		{
			thisCanvas.SetActive(true);
		}

		thisInstance = thisCanvas.GetComponent<DialogueBoxInstance>();
        if (!thisInstance)
		{
			Debug.Log("Error with panel instance.");
		}
	}

	public static void ShowMessage(string msg, string title = "", Type type = Type.Standard,
									DialogueBoxInstance.CallBack confirmCallBackFunction = null,
									DialogueBoxInstance.CallBack cancelCallBackFunction = null)
	{
		Init();
		thisInstance.ShowMessage(msg, title, type, confirmCallBackFunction, cancelCallBackFunction);
	}

	public static void Hide()
	{
		if (thisCanvas == null) return;
		thisCanvas.SetActive(false);
	}
}
