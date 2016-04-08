using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class represents a model of the Map.
/// </summary>
public class Map : MonoBehaviour {

	int xSize = 10;
	int ySize = 10;
	/// Minimum x-value of map
	[SerializeField] int xMinSize = 10;
	/// Maximum x-value of map
	[SerializeField] int xMaxSize = 20;
	/// Minimum y-value of map
	[SerializeField] int yMinSize = 10;
	/// Maximum y-value of map
	[SerializeField] int yMaxSize = 20;
	/// <summary>
	/// Gets or sets the width.
	/// </summary>
	/// <value>The width.</value>
	public int Width {get{return xSize;} set{xSize = Mathf.Clamp(value,xMinSize,xMaxSize);}}
	/// <summary>
	/// Gets or sets the height.
	/// </summary>
	/// <value>The height.</value>
	public int Height {get{return ySize;} set{ySize = Mathf.Clamp(value,yMinSize,yMaxSize);}}
	/// <summary>
	/// The width of the cell.
	/// </summary>
	[SerializeField] float cellWidthSize = 1f;
	/// <summary>
	/// The height of the cell
	/// </summary>
	[SerializeField] float cellHeightSize = 1f;
	[SerializeField] GameObject pathCellPrefab = null;
	[SerializeField] GameObject sceneryCellPrefab = null;
	[SerializeField] GameObject editorCellPrefab = null;
	[SerializeField] GameObject CellContainer = null;

	/// Grid of cells represented as a 2D array.
	private Cell[,] cellGrid = null;
	/// <summary>
	/// Gets the cell grid.
	/// </summary>
	/// <value>The cell grid.</value>
	public Cell[,] CellGrid {get{return cellGrid;}}

	/// The <see cref="PathCell"/> where critters will enter from.
	private PathCell startCell = null;
	/// <summary>
	/// Gets the start cell.
	/// </summary>
	/// <value>The start cell.</value>
	public PathCell StartCell {get{return startCell;}}

	/// The <see cref="PathCell"/> where critters will target.
	private PathCell endCell = null;
	/// <summary>
	/// Gets the end cell.
	/// </summary>
	/// <value>The end cell.</value>
	public PathCell EndCell {get{return endCell;}}

	/// List of PathCells that represents the path
	private List<PathCell> path;
	/// <summary>
	/// Gets the path.
	/// </summary>
	/// <value>The path.</value>
	public List<PathCell> Path {get{return path;}}

	/// <summary>
	/// Awake this instance and initialize <see cref="path"/>.
	/// </summary>
	void Awake(){
		path = new List<PathCell>();
	}

	/// <summary>
	/// Start this instance and does nothing.
	/// </summary>
	// Use this for initialization
	void Start () {
		
	}

	/// <summary>
	/// Update this instance once per frame and does nothing.
	/// </summary>
	void Update () {
		
	}

	/// <summary>
	/// Clears the map.
	/// </summary>
	public void ClearMap()
	{
		if(cellGrid != null)
			for(int w = 0; w < Width; w++)
				for(int h = 0; h < Height; h++)
					Destroy(cellGrid[w,h].gameObject);
		
		cellGrid = null;
	}

	/// <summary>
	/// Initializes the map. Populates the cellGrid array with Cell objects.
	/// Sets x-value of origin at [<see cref="cellWidthSize"/>(1 - <see cref="Width"/>)]/2 &
	/// sets y-value of origin at [<see cref="cellHeightSize"/>(1 -<see cref="Height"/>)]/2
	/// </summary>
	/// <param name="cellPrefab">Cell prefab.</param>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public void InitializeMap(GameObject cellPrefab,int width, int height)
	{
		ClearMap();
		//check input values
		Width = width;
		Height = height;
		
		//Create the cell 2D array
		cellGrid = new Cell[Width,Height];
		
		float xOrigin = -(Width/2)*cellWidthSize + cellWidthSize/2;
		float yOrigin = -(Height/2)*cellHeightSize + cellHeightSize/2;
		
		for(int w = 0; w < Width; w++)
		{
			for(int h = 0; h < Height; h++)
			{
				Vector3 newPosition = new Vector3(xOrigin + w*cellWidthSize, yOrigin + h*cellHeightSize);
				GameObject cell = (GameObject)Instantiate(cellPrefab,newPosition,Quaternion.identity);
				cell.transform.SetParent(CellContainer.transform,true);
				cellGrid[w,h] = cell.GetComponent<Cell>() as Cell;
				cellGrid[w,h].SetPosition(w,h);
			}
		}
	}

	/// <summary>
	/// Creates the editor cell grid by calling <see cref="InitializeMap(GameObject cellPrefab, int width, int height)"/>.
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	public void CreateEditorCellGrid(int width, int height)
	{
		InitializeMap(editorCellPrefab,width,height);
	}

	/// <summary>
	/// This method uses the information gathered after editing your Map, stores it and then calls
	/// <see cref="GeneratePathFromStart()"/>
	/// </summary>
	public void CreateMapFromEditorMap()
	{
		//replace all editor cells with scenery cells and path cells
		for(int w = 0; w < Width; w++)
		{
			for(int h = 0; h < Height; h++)
			{
				EditorCell editorCell = cellGrid[w,h] as EditorCell;
				switch(editorCell.type)
				{
					case EditorCell.CellType.scenery:
						cellGrid[w,h] = ((GameObject)Instantiate(sceneryCellPrefab,editorCell.transform.position,Quaternion.identity)).GetComponent<SceneryCell>();
						break;
					case EditorCell.CellType.path:
						cellGrid[w,h] = ((GameObject)Instantiate(pathCellPrefab,editorCell.transform.position,Quaternion.identity)).GetComponent<PathCell>();
						(cellGrid[w,h] as PathCell).AdjacentPathCells = editorCell.AdjecentPathCells;
						break;
					case EditorCell.CellType.start:
						cellGrid[w,h] = ((GameObject)Instantiate(pathCellPrefab,editorCell.transform.position,Quaternion.identity)).GetComponent<PathCell>();
						startCell = cellGrid[w,h] as PathCell;
						startCell.AdjacentPathCells = editorCell.AdjecentPathCells;
						break;
					case EditorCell.CellType.end:
						cellGrid[w,h] = ((GameObject)Instantiate(pathCellPrefab,editorCell.transform.position,Quaternion.identity)).GetComponent<PathCell>();
						endCell = cellGrid[w,h] as PathCell;
						endCell.AdjacentPathCells = editorCell.AdjecentPathCells;
						break;
				}
				cellGrid[w,h].SetPosition(w,h);
				cellGrid[w,h].transform.SetParent(CellContainer.transform,true);
				Destroy(editorCell.gameObject);
			}
		}
		Debug.Log(string.Format("Map generated from editor\nStart Cell at : {0},{1}\nEnd Cell at : {2},{3}",startCell.X,startCell.Y,endCell.X,endCell.Y));
		GeneratePathFromStart();
	}

	/// <summary>
	/// Loads a map given a <see cref="Width"/>, a <see cref="Height"/>, and a list of vectors representing the <see cref="path"/>.
	/// <see cref="InitializeMap(GameObject cellPrefab, int width, int height)"/> is first called followed by laying out the positions
	/// of each PathCell in the list of PathCells.
	/// </summary>
	/// <param name="width">Width.</param>
	/// <param name="height">Height.</param>
	/// <param name="pathPositions">Path positions.</param>
	public void LoadMap (int width, int height, List<Vector2> pathPositions)
	{
		InitializeMap(sceneryCellPrefab,width,height);
		path = new List<PathCell>();
		//generate Path
		foreach(Vector2 position in pathPositions)
		{
			int w = (int)position.x;
			int h = (int)position.y;
			Cell currentCell = cellGrid[w,h];
			cellGrid[w,h] = ((GameObject)Instantiate(pathCellPrefab,currentCell.transform.position,Quaternion.identity)).GetComponent<PathCell>();
			cellGrid[w,h].SetPosition(w,h);
			Destroy(currentCell.gameObject);
			path.Add(cellGrid[w,h] as PathCell);
			cellGrid[w,h].transform.SetParent(CellContainer.transform,true);
		}
		
		//set start and end
		startCell = cellGrid[(int)pathPositions[0].x,(int)pathPositions[0].y] as PathCell;
		endCell = cellGrid[(int)pathPositions[pathPositions.Count - 1].x,(int)pathPositions[pathPositions.Count - 1].y] as PathCell;
		
		string cellInfo = string.Empty;
		foreach(PathCell cell in path)
			cellInfo += cell + "\n";
		Debug.Log(string.Format("Path generated: \n{0}",cellInfo));
	}

	/// <summary>
	/// Generates the entire path by saving each PathCell with its position on the map into a list, <see cref="path"/>,
	/// with the start Cell at index 0 and the end Cell at the final index.
	/// </summary>
	private void GeneratePathFromStart()
	{
		PathCell lastPathCell = startCell;
		path = new List<PathCell>();
		path.Add(lastPathCell);
		Vector2 currentPathCellPosition = startCell.AdjacentPathCells[0];
		PathCell currentPathCell = cellGrid[(int)currentPathCellPosition.x,(int)currentPathCellPosition.y] as PathCell;

		// construct the path
		// start and end Cell should have one adjacent path cell while others will have two.
		while(currentPathCell.AdjacentPathCells.Count == 2)
		{
			Vector2 nextPathCellPosition = currentPathCell.AdjacentPathCells[0];
			if((int)nextPathCellPosition.x == lastPathCell.X && (int)nextPathCellPosition.y == lastPathCell.Y)
				nextPathCellPosition = currentPathCell.AdjacentPathCells[1];
			PathCell nextPathCell = cellGrid[(int)nextPathCellPosition.x,(int)nextPathCellPosition.y] as PathCell;
			lastPathCell = currentPathCell;
			path.Add(lastPathCell);
			currentPathCell = nextPathCell;
		}
		
		path.Add (endCell);
		
		string cellInfo = string.Empty;
		foreach(PathCell cell in path)
			cellInfo += cell + "\n";
		Debug.Log(string.Format("Path generated: \n{0}",cellInfo));
	}
}
