using UnityEngine;
using System.Collections;

/// <summary>
/// This class handles the logic the moment a critter enters a tower's range 
/// </summary>
public class TowerRangeTracker : MonoBehaviour
{
	[SerializeField] Tower tower = null;

	/// <summary>
	/// Awake this instance and get the <see cref="Tower"/> instance if it exists.
	/// </summary>
	void Awake()
	{
		if(tower == null)
			tower = GetComponentInParent<Tower>();
	}

	/// <summary>
	/// Raises the trigger enter event when a critter GameObject collides with the tower's range SphereCollider.
	/// 
	/// Calls the tower's <see cref="ShootCritter(<typeparamref name ="target"/>)"/> method on that critter.
	/// </summary>
	/// <param name="other">Other.</param>
	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Critter")
			tower.ShootCritter(other.GetComponent<Critter>());
	}
}

