using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class TestImageReplace : MonoBehaviour {
    public Sprite s;
	// Use this for initialization
	void Start () {
	    foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>())
        {
            sr.sprite = s;
            sr.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
            sr.gameObject.GetComponent<BoxCollider>().size = new Vector3(0.8f, 0.8f, 0.01f);
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
