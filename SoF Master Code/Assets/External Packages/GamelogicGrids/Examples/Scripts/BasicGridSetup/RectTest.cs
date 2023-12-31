//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using Gamelogic.Grids;
using UnityEngine;

public class RectTest : GLMonoBehaviour
{
	private readonly Vector2 CellDimensions = new Vector2(75, 40); 
	
	public Cell cellPrefab;
	public GameObject root;
	
	public RectGrid<Cell> grid;
	private IMap3D<RectPoint> map;
		
	public void Start()
	{		
		BuildGrid();
        //transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
	}
	
	public void Update()
	{
		if(Input.GetMouseButtonDown(0))
		{
            Vector3 worldPosition = ExampleUtils.ScreenToWorld(root, Input.mousePosition);
            
			RectPoint rectPoint = map[worldPosition];
			
			if(grid.Contains(rectPoint))
			{
				grid[rectPoint].HighlightOn = !grid[rectPoint].HighlightOn;
			}
		}
	}
		
	private void BuildGrid()
	{		
		grid = RectGrid<Cell>
			.BeginShape()
			.Rectangle(5,3)	
			.EndShape();
		
		map = new RectMap(CellDimensions)
			.AnchorCellMiddleCenter()
			.WithWindow(ExampleUtils.ScreenRect)
			.AlignMiddleCenter(grid)
			.To3DXY();
		
		foreach(RectPoint point in grid)
		{
			Cell cell = Instantiate<Cell>(cellPrefab);
			Vector3 worldPoint = map[point];
			
			cell.transform.parent = root.transform;
			cell.transform.localScale = Vector3.one;
			cell.transform.localPosition = worldPoint;
			
		//	cell.SetColor(ExampleUtils.colors[point.GetColor4()]);
			//cell.SetText("(" + point.X + ", " + point.Y + ")");			
			grid[point] = cell;
		}
	}
}
