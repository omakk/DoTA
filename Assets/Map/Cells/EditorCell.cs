using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// This class extends <see cref="Cell"/> and represents the model of a Editor Cell.
/// </summary>
public class EditorCell : Cell
{
	public enum CellType {path, scenery, start, end}
	/// <summary>
	/// Gets or sets the type of Cell.
	/// </summary>
	/// <value>The type.</value>
	public CellType type {get;set;}
	
	[SerializeField] Color startCellColor = Color.blue;
	[SerializeField] Color sceneryCellColor = Color.green;
	[SerializeField] Color pathCellColor = Color.black;

	/// EditorCell mesh renderer.
	private MeshRenderer meshRenderer = null;
	[SerializeField] MeshRenderer number1Renderer = null;
	[SerializeField] MeshRenderer number2Renderer = null;

	/// BoCollider placed on top side of Cell.
	[SerializeField] protected BoxCollider2D upCellFinder = null;

	/// BoxCollider placed on lower side of Cell.
	[SerializeField] protected BoxCollider2D downCellFinder = null;

	/// BoxCollider placed on right side of Cell.
	[SerializeField] protected BoxCollider2D rightCellFinder = null;

	/// BoxCollider placed on left side of Cell.
	[SerializeField] protected BoxCollider2D leftCellFinder = null;

	/// <summary>
	/// List of adjacent EditorCells.
	/// </summary>
	protected List<EditorCell> adjacentCells = null;
	/// <summary>
	/// Gets the number of adjacent cells from the list of <see cref="AdjacentPathCells"/>.
	/// </summary>
	/// <value>The number adjacent cells.</value>
	public int NumberAdjacentCells {get{return AdjecentPathCells.Count;}}
	/// <summary>
	/// Gets the adjecent PathCells from <see cref="adjacentCells"/>.
	/// </summary>
	/// <value>The adjecent path cells.</value>
	public List<Vector2> AdjecentPathCells {get{return adjacentCells.Where(x => x.type == CellType.path || x.type == CellType.start || x.type == CellType.end).Select(x => new Vector2(x.X,x.Y)).ToList();}}

	/// <summary>
	/// Awake this instance, render its contour and color, trigger all its BoxColliders,
	/// and initialize <see cref="adjacentCells"/>.
	/// </summary>
	protected override void Awake ()
	{
		base.Awake ();
		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material.color = sceneryCellColor;
		type = CellType.scenery;
		upCellFinder.isTrigger = true;
		downCellFinder.isTrigger = true;
		rightCellFinder.isTrigger = true;
		leftCellFinder.isTrigger = true;
		adjacentCells = new List<EditorCell>();
	}

	/// <summary>
	/// Raise mouse down event, do nothing if the editor is drawing, and change Cell type from patch to scenery
	/// if cell clicked is path and vice versa.
	/// </summary>
	protected override void OnMouseDown ()
	{
		if(!GameManager.Instance.Editor.Drawing)
			return;
		if(type == CellType.path)
			ChangeType(CellType.scenery);
		else if(type == CellType.scenery)
			ChangeType(CellType.path);
	}

	/// <summary>
	/// Add Cell to <see cref="adjacentCells"/> if two Colliders collide and does nothing if Cell has already been added.
	/// </summary>
	/// <param name="other">Other.</param>
	protected virtual void OnTriggerEnter2D(Collider2D other)
	{
		EditorCell cell = other.GetComponent<EditorCell>();
		if(cell == null)
			return;
		
		if(adjacentCells.Contains(cell))
			return;
		
		adjacentCells.Add(cell);
	}

	/// <summary>
	/// Changes the type of Cell through the enum <see cref="CellType"/>.
	/// </summary>
	/// <param name="newType">New type.</param>
	public void ChangeType(CellType newType)
	{
		switch(newType)
		{
		case CellType.scenery:
			meshRenderer.material.color = sceneryCellColor;
			break;
			
		case CellType.path:
			meshRenderer.material.color = pathCellColor;
			break;
			
		case CellType.start:
			meshRenderer.material.color = startCellColor;
			break;
			
		case CellType.end:
			meshRenderer.material.color = pathCellColor;
			break;
		}
		
		type = newType;
	}

	/// <summary>
	/// Enables the number "1" to render and disenables the number "2".
	/// </summary>
	public void DisplayNumber1()
	{
		number1Renderer.enabled = true;
		number2Renderer.enabled = false;
	}

	/// <summary>
	/// Enables the number "2" to render and disenables the number "1".
	/// </summary>
	public void DisplayNumber2()
	{
		number1Renderer.enabled = false;
		number2Renderer.enabled = true;
	}

	/// <summary>
	/// Disenable both numbers "1"& "2" from rendering.
	/// </summary>
	public void RemoveNumberDisplay()
	{
		number1Renderer.enabled = false;
		number2Renderer.enabled = false;
	}

	/// <summary>
	/// Raises the mouse enter event and calls superclass' <see cref="OnMouseEnter()"/> and calls the <see cref="EditorManager"/>'s
	/// <see cref="CheckPathValidity()"/> method if it is currently drawing.
	/// </summary>
	protected override void OnMouseEnter ()
	{
		if(GameManager.Instance.Editor.Drawing)
		{
			base.OnMouseEnter ();
			GameManager.Instance.Editor.CheckPathValidity();
		}
	}
}

