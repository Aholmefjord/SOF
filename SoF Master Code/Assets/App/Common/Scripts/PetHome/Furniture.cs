using UnityEngine;
using System.Collections;

public class Furniture {
	public GameObject misPlaceIndi;

	void Start(){
	//	misPlaceIndi = this.gameObject.transform.Find ("MisPlace").gameObject;
		misPlaceIndi.SetActive (false);
	}
	void OnMouseDown() {
	///	if (this.transform.Find ("Mesh").GetComponent<Animator> () != null) {
	//		this.transform.Find ("Mesh").GetComponent<Animator> ().Play ("FurnitureAnimation");
	//	}
		/*
		Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if (Physics.Raycast(raycast, out hit))
		{
			if (hit.collider.gameObject == this.gameObject)
			{
				this.transform.Find ("Mesh").GetComponent<Animator> ().Play ("Take 001");
			}
		}*/
	}
}
