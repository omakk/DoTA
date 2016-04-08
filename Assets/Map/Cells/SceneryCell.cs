using UnityEngine;
using System.Collections;

/// <summary>
/// This class extends <see cref="Cell"/> and represents the model of a Scenery Cell.
/// </summary>
public class SceneryCell : Cell {

	private Tower tower = null;

	/// <summary>
	/// Calls this superclass' <see cref="OnMouseEnter()"/> if a tower is being placed when a mouse enter event is raised.
	/// </summary>
	protected override void OnMouseEnter ()
	{
		if(tower == null && GameManager.Instance.TowerManager.PlacingTower)
			base.OnMouseEnter ();
	}

	/// <summary>
	/// Calls superclass' <see cref="OnMouseExit()"/> when mouse eit event is raised.
	/// </summary>
	protected override void OnMouseExit ()
	{
		base.OnMouseExit ();
	}

	/// <summary>
	/// Raise mouse down event and calls superclass' <see cref="OnMouseDown()"/> and calls <see cref="TowerManager.UnSelectAllTowers()"/> if the tower is not null;
	/// otherwise, place the tower at the current position if we are placing a tower; otherwise, call <see cref="TowerManager.UnSelectAllTowers()"/>. 
	/// </summary>
	protected override void OnMouseDown ()
	{
		base.OnMouseDown ();
		if(tower != null)
		{
			GameManager.Instance.TowerManager.UnSelectAllTowers();
		}
		else if(GameManager.Instance.TowerManager.PlacingTower)
		{
			Vector3 position = transform.position;
			position.z -= 1.5f;
			tower = GameManager.Instance.TowerManager.PlaceTower(position);
			if(tower != null)
				Debug.Log(string.Format("Placed Tower at {0},{1} cost was {2} player now has {3}",X,Y,tower.price,GameManager.Instance.Player.Cash));
		}
		else
		{
			GameManager.Instance.TowerManager.UnSelectAllTowers();
		}
	}
}
