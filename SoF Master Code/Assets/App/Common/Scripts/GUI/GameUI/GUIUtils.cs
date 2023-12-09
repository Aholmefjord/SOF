using UnityEngine;
using System.Collections;

public class GUIUtils : MonoBehaviour {
	public static void HideCanvasGroup (CanvasGroup c)
	{
		c.alpha = 0;
		c.interactable = false;
		c.blocksRaycasts = false;
	}

	public static void ShowCanvasGroup(CanvasGroup c)
	{
		c.alpha = 1;
		c.interactable = true;
		c.blocksRaycasts = true;
	}

}
