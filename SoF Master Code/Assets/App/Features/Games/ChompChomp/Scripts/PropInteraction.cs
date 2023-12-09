using UnityEngine;
using System.Collections;

public class PropInteraction : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnMouseDown() {
		this.GetComponent<Animator> ().SetTrigger ("onTapProp");
	}
}
