using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MatchMovesLeft : MonoBehaviour {
	[HideInInspector] public MatchGameManager gm {get{return JMFUtils.gm;}}
	private int initialMoves; 
	// Use this for initialization
	void Start () {
		initialMoves = gm.moves;
		this.GetComponent<Text> ().text = (gm.gameObject.GetComponent<WinningConditions>().allowedMoves - gm.moves).ToString();
	}
	
	// Update is called once per frame
	void Update () {
		if (initialMoves != gm.moves) {
			this.GetComponent<Text> ().text = (gm.gameObject.GetComponent<WinningConditions>().allowedMoves - gm.moves).ToString();
			initialMoves = gm.moves;
		}
	}
}
