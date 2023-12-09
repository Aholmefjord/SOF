using UnityEngine;
using System.Collections;

// Create the grid in the PrototypeHome Scene
public class GridSystem : MonoBehaviour
{
	// Public Variables	
	[Header("Grid Properties")]
	public int xSize;
	public int zSize;
	public float spacing;

	[HideInInspector]
	public Vector3[] centerPos;

	[HideInInspector]
	public GameObject[] cell;

	[HideInInspector]
	public int numOfCells;

	// Private Variables
	private Vector3[] vertices;	
	private Mesh mesh;
	private Material mat;

	private Vector3[] centerPosition;
	private GameObject[] cellGrid;
    public bool GenerateNow;

	// Use this for initialization
	void Start()
	{
		
	}

	// Called before Start
	void Awake()
	{
		//Generate();

		Init();
	}

	// Update is called once per frame
	void Update()
	{

	}

	// Generate the Grid
	private void Generate()
	{
		// Get the Color of the material of the Mesh Renderer
		mat = GetComponent<MeshRenderer>().material;
		
		// Initialise the centerPos Vector3, and the cell GameObject arrays
		centerPosition = new Vector3[xSize * zSize];
		cellGrid = new GameObject[centerPosition.Length];

		for (int i = 0, z = 0; z < zSize; z++)
		{
			for (int x = 0; x < xSize; x++, i++)
			{
				// Set the center of the cube's position
				centerPosition[i] = new Vector3(transform.position.x + (x * spacing) + (spacing / 2), transform.position.y, transform.position.z + (z * spacing) + (spacing / 2));

				// Create the cube and set it's position, material and parent GameObject
				GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
				MeshRenderer cubeRenderer = cube.GetComponent<MeshRenderer>();
				cube.transform.position = centerPosition[i];
				cube.transform.SetParent(transform);
				cube.transform.localScale = new Vector3(spacing, 0.1f, spacing);
				cubeRenderer.material = mat;
				cubeRenderer.receiveShadows = false;
				cubeRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
				cubeRenderer.useLightProbes = false;
				cubeRenderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
				cube.GetComponent<BoxCollider>().isTrigger = true;
				cube.AddComponent<CellSystem>();
				cube.name = "Cube" + (z + 1) + "_" + (x + 1);
				cube.tag = "Ground";

				// Store it into the cell array
				cellGrid[i] = cube;
			}
		}
	}

	// Store the cells' position
	private void Init()
	{
		// Check for the number of children under the Grid
		numOfCells = transform.childCount;

		// Initialise the private arrays
		centerPos = new Vector3[numOfCells];
		cell = new GameObject[numOfCells];

		// Store all the children's position & GameObject
		for (int i = 0; i < numOfCells; i++)
		{
			centerPos[i] = transform.GetChild(i).position;
			cell[i] = transform.GetChild(i).gameObject;
		}

		Debug.Log("Array Size: " + numOfCells);
	}

	// To draw the in Editor
	private void OnDrawGizmos()
	{
		//Gizmos.color = Color.black;

		//if (vertices == null) { return; }
		//for (int i = 0; i < vertices.Length; i++)
		//{
		//	Gizmos.DrawSphere(new Vector3(transform.position.x + vertices[i].x, vertices[i].y, transform.position.z + vertices[i].z), 0.1f);
		//}

		//if (centerPos == null) { return; }
		//for (int i = 0; i < centerPos.Length; i++)
		//{
		//	Gizmos.DrawSphere(new Vector3(centerPos[i].x, centerPos[i].y, centerPos[i].z), 0.1f);
		//}
	}
}
