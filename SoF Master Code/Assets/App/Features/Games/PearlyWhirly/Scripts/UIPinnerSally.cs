using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIPinnerSally : MonoBehaviour {
    RectTransform trans;
    Vector3 pos;
	// Use this for initialization
	void Start () {
        trans = GetComponent<RectTransform>();
        pos = trans.anchoredPosition3D;
	}
	
	// Update is called once per frame
	void Update () {//Please note this is hardcoded for now as it's faster to apply a hotfix than to find what's causing the problem
             //   trans.anchoredPosition3D = new Vector3(200, 750, 0);
//        trans.anchoredPosition3D = pos;
	}
}
