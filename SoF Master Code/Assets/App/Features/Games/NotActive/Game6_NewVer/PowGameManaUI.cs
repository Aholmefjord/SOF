using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowGameManaUI : MonoBehaviour {
    public List<GameObject> manaObjects;

    public float ratio;
    int GameObjectsToShow;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
       GameObjectsToShow = (int)Mathf.Floor(ratio);
        for(int i = 0; i < GameObjectsToShow; i++)
        {
            manaObjects[i].SetActive(true);
        }
        for (int i = GameObjectsToShow; i < manaObjects.Count; i++) {
            manaObjects[i].SetActive(false);
        }
    }
}
