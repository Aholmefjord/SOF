using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CheckUnlocked : MonoBehaviour
{
    public bool unlocked = false;
    public NewMapController nmc;
    public int id;
    public Button button;

	// Use this for initialization
	void Start()
	{
		// Need to add logic checking if a module is unlocked.
		//if (unlocked)
		//{
		//button.enabled = unlocked;
		//}
	}

	// Before Start
	void Awake()
	{
		
	}

    void Update()
    {
		unlocked = nmc.unlockedLevels[id];
	}
}
