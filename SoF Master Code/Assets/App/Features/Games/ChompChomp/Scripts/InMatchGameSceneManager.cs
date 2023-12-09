using UnityEngine;
using System.Collections;

public class InMatchGameSceneManager : MonoBehaviour {

	private static InMatchGameSceneManager _instance;
	void Awake() {
		_instance = this;
	}
	
	public static InMatchGameSceneManager instance {
		get {
			if(_instance == null) {
				_instance = GameObject.FindObjectOfType<InMatchGameSceneManager>();
			}
			
			return _instance;
		}
	}
	void Start(){
		AudioSystem.InstanceCheck();//Creates audio listener if not available
	}
}
