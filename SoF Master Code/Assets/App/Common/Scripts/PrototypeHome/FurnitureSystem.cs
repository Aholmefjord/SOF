using UnityEngine;
using System.Collections;

// TODO: -
// Manage the points by calculating all the Furnitures that are in the home
public class FurnitureSystem : MonoBehaviour {

    // Public Variables
    [HideInInspector]
    public int totalScore;

    // Private Variables
    private int furnitureCount;
    private GameObject[] furnitures;

    // Use this for initialization
    void Start()
    {
	    
	}
	
    // Before Start
    void Awake()
    {
        CheckFurniture();
    }

	// Update is called once per frame
	void Update()
    {
	    
	}

    // Initialise things required for this script
    private void CheckFurniture()
    {
        // Get the number of furniture under the "Furnitures" empty GameObject
        furnitureCount = transform.childCount;

        // Set up the furnitures array to store the furnitures
        furnitures = new GameObject[furnitureCount];

        // Store all the furnitures in the scene into the array
        for (int i = 0; i < furnitureCount; i++)
        {
            furnitures[i] = transform.GetChild(i).gameObject;
            Debug.Log("Furniture Found: " + transform.GetChild(i).gameObject.name);
        }
    }

    // Calculate the total score to return
    private void Calculate()
    {
        
    }
}
