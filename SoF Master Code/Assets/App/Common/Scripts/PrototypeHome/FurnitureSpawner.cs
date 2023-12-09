using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Spawn in furniture when button is pressed
public class FurnitureSpawner : MonoBehaviour
{
	// Public Variables
	public GameObject prefabButton;
	public RectTransform ParentPanel;
	public Transform gridStart;
	public GridSystem grid;
	public GameObject furnitureContainer;
	public GameObject[] spawnables;

	// Private Variables
	private int numOfFurnitures;
	private Vector3 gridCenter;

	// Use this for initialization
	void Start()
	{
		Init();
	}
	
	// Called before Start
	void Awake()
	{
		
	}

	// Update is called once per frame
	void Update ()
	{
		
	}

	// Initialised on Start/Awake
	private void Init()
	{
		// Check the number of furnitures to spawn
		numOfFurnitures = transform.childCount;

		Debug.Log("Grid Center: " + (grid.xSize * (grid.zSize / 2)) + (grid.xSize / 2));

		// Get the center position of the grid
		if (grid == null) return;
		gridCenter = grid.cell[(grid.xSize * (grid.zSize / 2)) + (grid.xSize / 2)].transform.position;

		// Initialise the furnitures GameObject array to store all spawnables
		spawnables = new GameObject[numOfFurnitures];

		// Iterate through all spawnables under the "Spawnables" GameObject
		for (int i = 0; i < transform.childCount; i++)
		{
			Debug.Log("Spawnable Furniture: " + transform.GetChild(i).gameObject.name);
			spawnables[i] = transform.GetChild(i).gameObject;

			GameObject goButton = Instantiate(prefabButton) as GameObject;
			goButton.transform.SetParent(ParentPanel, false);
			goButton.transform.localScale = new Vector3(1, 1, 1);
			goButton.name = prefabButton.name;

			Button tmpButton = goButton.GetComponent<Button>();
			int tempInt = i;
			tmpButton.onClick.AddListener(() => FurnitureButton(tempInt));

			Text tmpText = goButton.transform.GetChild(0).GetComponent<Text>();
			tmpText.text = spawnables[i].name;
		}
	}

	// OnClick function for spawning a cube
	public void FurnitureButton(int i)
	{
		if (spawnables == null) { return; }
		//Debug.Log("Num: " + i);
		GameObject tempGO = Instantiate(spawnables[i]) as GameObject;
		tempGO.transform.position = new Vector3(0, tempGO.transform.position.y, 0);
		tempGO.SetActive(true);
		tempGO.transform.SetParent(furnitureContainer.transform);
		tempGO.name = spawnables[i].name;

		//Debug.Log("Spawning @ X: " + tempGO.transform.position.x + " Y: " + tempGO.transform.position.y + " Z: " + tempGO.transform.position.z);
	}
}
