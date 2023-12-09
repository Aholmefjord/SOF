using UnityEngine;
using System.Collections;

public class disableLoadingText : MonoBehaviour {
    public GameObject disableText;
    public GameObject other;
	// Use this for initialization
	
	// Update is called once per frame
	void Update () {
        gameObject.SetActive(!other.GetActive());
	}
}
