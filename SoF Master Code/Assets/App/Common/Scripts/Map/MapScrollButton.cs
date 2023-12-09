using UnityEngine;
using System.Collections;

public class MapScrollButton : MonoBehaviour {

    [SerializeField]
    NewMapController Controller;

    [SerializeField]
    public int PageID = 0;

    // 0 - Off, 1 - On
    [SerializeField]
    public Sprite[] ButtonType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClickScrollButton()
    {
		Debug.LogError("disabled sfx");
		//AudioSystem.PlaySFX("UI/sfx_ui_click");
        Controller.CurrentPage = PageID;
        Controller.ManageButtonScrolls();
        Controller.ResetMapSprites();
        Controller.PopulateMapNodes();
        Controller.SetLevelNodes();

		//Debug.Log("PageID: " + PageID);
    }
}
