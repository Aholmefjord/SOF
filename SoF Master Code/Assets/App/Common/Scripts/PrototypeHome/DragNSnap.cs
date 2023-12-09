using UnityEngine;
using System.Collections;

// TODO: Rotation center position
// Used on Spawnable GameObjects so that they can be dragged
public class DragNSnap : MonoBehaviour
{
	// Public Variables
	//public HomeCamera homeCamera;			// Store the Home Camera script
	[Header("Size of Furniture")]
	public int xSize = 1;                   // Set the size of the furniture along the x-axis (To be multiplied by zSize)
	public int zSize = 1;                   // Set the size of the furniture along the z-axis (To be multiplied by xSize)
	[Space(14)]
	public GridSystem grid;                 // Gets the grid
	public bool dragging = false;           // To monitor if current GameObject is getting dragged
	public bool selected = false;			// To monitor if current GameObject is selected

	// Private Variables
	private BoxCollider boxCollider;		// Store the collider to be turned off when dragging
	private Vector3 mouseCell;              // Store the Closest Cell's position
	private Vector3 centerPos;				// Store the center position of the GameObject
	private Vector3[] cellPositions;		// Store all the positions of the cells taken up by the furniture
	private float mouseDistance = 0.55f;    // Set the distance from the mouse to the grid required
	private int cellsTaken;                 // Just size (variable) to the power of 2
	private bool isEven = false;            // Check if the furniture is even
	private MeshRenderer meshRenderer;      // Store this GameObject's Mesh Renderer
	private HomeCamera homeCamera;          // Get the HomeCamera instance
	private int turnSpeed = 5;              // Set the turn speed when rotating
	private int mouseCellIndex;				// Store the mouseCell's index in the array of cells

	// Use this for initialization
	void Start()
	{
		// Store the current GameObject's collider (TODO: Don't just store BoxCollider!)
		boxCollider = GetComponent<BoxCollider>();

		boxCollider.size = new Vector3(xSize, 1, zSize);
	}

	// Called before Start (Apparently)
	void Awake()
	{
		// Get the current HomeCamera instance
		HomeCamera homeCamera = GetComponent<HomeCamera>();

		// Get the current GameObject's renderer material
		meshRenderer = gameObject.GetComponent<MeshRenderer>();

		// Set the current GameObject's cells taken
		cellsTaken = xSize * zSize;

		// Initialise the cellPositions array with the number of cells it is gonna occupy
		cellPositions = new Vector3[cellsTaken];

		// Determine if this GameObject's size is even
		if (cellsTaken % 2 == 0)
		{
			isEven = true;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		// Update it's position to the pointer if it is getting dragged
		if (dragging)
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (hit.collider.tag == "Ground")
				{
					Vector3 cursorPoint = new Vector3(hit.point.x, hit.transform.position.y, hit.point.z);

					FindClosestCell(cursorPoint);
				}
			}
		}
	}

	// Function to check what's the closest cell from the current GameObject
	void FindClosestCell(Vector3 cursorPosition)
	{
		for (int i = 0; i < grid.numOfCells; i++)
		{
			if (Vector3.Distance(cursorPosition, grid.cell[i].transform.position) < mouseDistance)
			{
				mouseCell = grid.centerPos[i];
				mouseCellIndex = i;

				// Check if the GameObject's top left, top right and bottom right corners exists
				if (grid.cell[i + (xSize - 1)].GetComponent<BoxCollider>().enabled != false &&
					grid.cell[i + ((zSize - 1) * grid.xSize)].GetComponent<BoxCollider>().enabled != false &&
					grid.cell[i + (grid.xSize * (zSize - 1)) + (xSize - 1)].GetComponent<BoxCollider>().enabled != false)
				{
					transform.position = FindCenter(i, xSize, zSize);
				}
			}
		}
	}

	// Function to get the positions of all the taken cells
	private Vector3 FindCenter(int i, int sizeOfX, int sizeOfZ)
	{
		// Check along the x-axis
		for (int x = 0; x < sizeOfX; x++)
		{
			cellPositions[x] = grid.centerPos[i + x];

			// Check along the z-axis
			for (int z = 1; z < sizeOfZ; z++)
			{
				cellPositions[(sizeOfX * z) + x] = grid.centerPos[i + x + (grid.xSize * z)];
			}
		}

		// Variables to store the minimum and maximum of the X and Z
		float xMin, xMax, zMin, zMax;

		// Find the minimum and maximum x-axis values
		xMin = cellPositions[0].x;
		xMax = cellPositions[sizeOfX - 1].x;
		zMin = cellPositions[0].z;
		zMax = cellPositions[cellPositions.Length - 1].z;

		// Set the transform to the center of all the cells
		centerPos = new Vector3((xMin + xMax) / 2, transform.position.y, (zMin + zMax) / 2);

		// Set current GameObject to red if the cell is already taken
		CheckCells();

		return centerPos;
	}

	// Function to check if this GameObject is in a cell already occupied
	// If it is, set this one as the occupant
	private void CheckCells()
	{
		bool timeToBreak = false;

		// Run through all the cells' position that the object is taking up
		for (int j = 0; j < cellPositions.Length; j++)
		{
			for (int i = 0; i < grid.numOfCells; i++)
			{
				if (cellPositions[j] == grid.cell[i].transform.position)
				{
					if (grid.cell[i].GetComponent<CellSystem>().isOccupied && grid.cell[i].GetComponent<CellSystem>().occupant != gameObject)
					{
						meshRenderer.material.color = Color.red;
						timeToBreak = true;
					}
					else
					{
						meshRenderer.material.color = Color.white;
					}
				}
			}

			if (timeToBreak)
				break;
		}
	}

	// To be called when rotate button is pressed
	public void Rotate(bool clockwise)
	{
		// If rotate clockwise, increase current Y rotation by 90
		// Else, decrease current Y rotation by 90
		if (clockwise)
		{
			transform.Rotate(Vector3.up, 90f);
		}
		else
		{
			transform.Rotate(Vector3.up, -90f);
		}

		// Temporary integers to store the current xSize and zSize
		int xTemp, zTemp;

		// Store the xSize and zSize into the temporary integers
		xTemp = xSize;
		zTemp = zSize;

		// Swap xSize and zSize
		xSize = zTemp;
		zSize = xTemp;

		// Recalculate it's center position after rotation
		FindCenter(mouseCellIndex, xSize, zSize);
	}

	// When LMB is clicked on this
	void OnMouseDown()
	{
		// Drag mode enabled, collider disabled to not block raycast (Only if we are in building mode)
		if (HomeCamera.Instance.building)
		{
			dragging = true;
			boxCollider.enabled = false;

			// Set itself to be selected
			selected = !selected;

			if (selected)
			{
				// If there is no selected furniture, set this as the selected furniture
				// Else if there is already a selected furniture, remove the previous selection and set this as selected furniture instead
				if (HomeCamera.Instance.selected == null)
				{
					HomeCamera.Instance.selected = gameObject;
				}
				else
				{
					HomeCamera.Instance.selected = null;
					HomeCamera.Instance.selected = gameObject;
				}

				// Turn on the Rotate Buttons
				HomeCamera.Instance.rotateButtons.SetActive(true);
			}
			else
			{
				// Turn off the Rotate Buttons
				HomeCamera.Instance.rotateButtons.SetActive(false);

				// If this object is de-selected, and the current selected object stored in HomeCamera is this, set the current selection to be null
				if (HomeCamera.Instance.selected == gameObject)
				{
					HomeCamera.Instance.selected = null;
				}
			}
		}
	}

	// When LMB clicked on dragged around
	void OnMouseDrag()
	{
		
	}

	// When LMB is released
	void OnMouseUp()
	{
		// Drag mode disabled, collider enabled as we are not raycasting anymore (Only if we are in building mode)
		if (HomeCamera.Instance.building)
		{
			dragging = false;
			boxCollider.enabled = true;
		}
	}
}
