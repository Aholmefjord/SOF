using UnityEngine;
using System.Collections;

public class SpawnBuddy : MonoBehaviour {

    [SerializeField]
    GameObject DefaultAvatar;

    [SerializeField]
    GameObject AvatarDefaultPosition;

    [SerializeField]
    float Orientation; 

    GameObject BuddyAvatar;

	// Use this for initialization
	void Start () {
	    // Load the Avatar from the database
        BuddyAvatar = AvatarLoader.current.GetAvatarDivingModeObject();

        // Checks if the Buddy is loaded, if it is not, we will load the placeholder model
        if (!BuddyAvatar)
        {
            print("Failed to load player's avatar");

            if (BuddyAvatar = (GameObject)Instantiate(DefaultAvatar, AvatarDefaultPosition.transform.position + Vector3.up * 0.5f, Quaternion.Euler(0, Orientation, 0)))
                print("Unable to load default model");
        }
    }
}
