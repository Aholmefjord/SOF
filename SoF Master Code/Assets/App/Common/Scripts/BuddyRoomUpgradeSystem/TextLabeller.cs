using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextLabeller : MonoBehaviour {
    Text textToEdit;
    public int val;
	// Use this for initialization
	void Start () {
        textToEdit =    GetComponent<Text>();
	}   
	
	// Update is called once per frame
	void Update () {
        switch (val)
        {
            case 0:
                textToEdit.text = GameState.me.inventory.Coins.ToString();
                break;
            case 1:
                textToEdit.text = GameState.me.inventory.Jewels.ToString();
                break;
            case 2:
                textToEdit.text = GameState.me.inventory.Ceramic.ToString();
                break;
            case 3:
                textToEdit.text = GameState.me.inventory.Steel.ToString();
                break;
            case 4:
                textToEdit.text = GameState.me.inventory.StoneTablet.ToString();
                break;
			case 5:
				textToEdit.text = GameState.me.inventory.Wood.ToString();
				break;
        }

	}
}
