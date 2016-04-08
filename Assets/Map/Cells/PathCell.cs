using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class extends <see cref="Cell"/> and represents the model of a Path Cell.
/// </summary>
public class PathCell : Cell {

	/// <summary>
	/// Gets or sets the adjacent path cells.
	/// </summary>
	/// <value>The adjacent path cells.</value>
	public List<Vector2> AdjacentPathCells {get;set;}

	/// <summary>
	/// Awake this instance by invoking superclass' <see cref="Awake()"/> and initializing <see cref="AdjacentPathCells"/>.
	/// </summary>
	protected override void Awake ()
	{
		base.Awake ();
		AdjacentPathCells = new List<Vector2>();
	}

	/// <summary>
	/// Do nothing when mouse enter event is raised.
	/// </summary>
	protected override void OnMouseEnter ()
	{
		//Do nothing purposely
	}

	/// <summary>
	/// Do nothing when mouse exit event is raised.
	/// </summary>
	protected override void OnMouseExit ()
	{
		//Do nothing purposely
	}

	/// <summary>
	/// Calls <see cref="TowerManager.UnSelectAllTowers()"/> when mouse down event is raised.
	/// </summary>
	protected override void OnMouseDown ()
	{
		GameManager.Instance.TowerManager.UnSelectAllTowers();
	}
}
