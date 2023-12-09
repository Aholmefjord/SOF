using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarSpawn : MonoBehaviour {

    // Use this for initialization
    public Transform temp;
	void Start () {
        GameObject avatar = AvatarLoader.current.GetAvatarObject();
     
        try
        {
            avatar.transform.position = transform.position;
            avatar.transform.rotation = transform.rotation;
            avatar.transform.localScale = transform.localScale;
        }
        catch (UnityException e)
        {
            Debug.LogError(e);
        }
        avatar.transform.SetParent(temp);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
