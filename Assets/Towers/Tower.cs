using UnityEngine;
using System.Collections;

/// <summary>
/// This class represents the model of a Tower.
/// </summary>
public class Tower : MonoBehaviour {
	/// <summary>
	/// Tower's price.
	/// </summary>
	public int price = 10; 
	/// <summary>
	/// Tower's speed.
	/// </summary>
	public float speed = 2f;
	/// <summary>
	/// Damage inflicted by tower.
	/// </summary>
	public float damage = 20f;
	///<summary>
	/// Used to check whether splash damage is applied or not.
	/// </summary>
	public bool isSplashDamage = false;
	///<summary>
	/// Used to modify the splash damage taken by critters within splashDamageRadius.
	/// </summary>
	public float splashDamageModifier = 0.25f;
	///<summary>
	/// Radius of the region where splash damage is applied.
	///</summary>
	public float splashDamageRadius = 0.3f; 
	/// <summary>
	/// Modifies critter speed.
	/// </summary>
	public float slowAmount = 0.25f;
	/// <summary>
	/// Used to check whether slow effect is applied or not.
	/// </summary>
	public bool isSlow = false;
	/// <summary>
	/// Level of tower upgrade. Initially set to 1.
	/// </summary>
	public int upgradeLevel = 1;

	/// <summary>
	/// Tower's bullet.
	/// </summary>
	public Bullet bullet = null;

	/// <summary>
	/// Gets a value indicating whether this instance can upgrade.
	/// </summary>
	/// <value><c>true</c> if this instance can upgrade; otherwise, <c>false</c>.</value>
	public bool CanUpgrade {get{return upgradeLevel < 5 && UpgradeCost <= GameManager.Instance.Player.Cash;}}

	/// <summary>
	/// Gets the upgrade cost. (price/2)
	/// </summary>
	/// <value>The upgrade cost.</value>
	public double UpgradeCost {get{return price/1.5;}}

	/// <summary>
	/// Gets the sell price. (price * 0.75)
	/// </summary>
	/// <value>The sell price.</value>
	public int SellPrice {get{return (int)(price*0.75f);}}

	/// <summary>
	/// Represents the range of the tower.
	/// </summary>
	public SphereCollider range = null;
	/// <summary>
	/// Used to check whether tower can shoot.
	/// </summary>
	bool canShoot = true;
	/// Tower fire rate.
	public float fireRate = 0.5f;
	/// Tower fire timer.
	private float fireTimer = 0f;
	/// The bullet Prefab.
	[SerializeField] GameObject bulletPrefab = null;
	/// The mesh renderer.
	[SerializeField] MeshRenderer meshRenderer = null;
	/// The range mesh renderer.
	[SerializeField] MeshRenderer rangeMeshRenderer = null;
	/// The range transform.
	[SerializeField] Transform rangeTransform = null;

	/// <summary>
	/// Gets or sets the range.
	/// </summary>
	/// <value>The range.</value>
	public float Range {get{return rangeTransform.localScale.x;} set{rangeTransform.localScale = new Vector3(value,value,value);}}

	/// Used to check if this tower is selected.
	private bool isSelected = false;

	/// <summary>
	/// Gets a value indicating whether this <see cref="Tower"/> is selected.
	/// </summary>
	/// <value><c>true</c> if selected; otherwise, <c>false</c>.</value>
	public bool Selected {get{return isSelected;}}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	/// <description>
	/// Defines the position vector of the tower's range
	/// </description>
	void Awake()
	{
		if(range == null)
			range = GetComponent<SphereCollider>();
		
		Vector3 rangePosition = range.transform.position;
		rangePosition.z = 0f;
		range.transform.position = rangePosition;
	}

	/// <summary>
	/// Fixed update to fix the time between each bullet fired.
	/// </summary>
	/// <description>
	/// Enables the tower <see cref="range"/> SphereCollider and enables it to shoot
	/// whenever <see cref="fireTimer"/> runs out
	/// </description>
	void FixedUpdate () {
		if(fireTimer > 0)
		{
			fireTimer -= Time.fixedDeltaTime;
			if(fireTimer <= 0)
			{
				range.enabled = true;
				canShoot = true;
			}
		}
	}

	/// <summary>
	/// Shoots the critter.
	/// </summary>
	/// <description>
	/// Creates a bullet instance and sets its relevant attributes to tower attributes
	/// and makes the parameter critter the target of the bullet only if the tower can shoot; otherwise,
	/// does nothing.
	/// </description>
	/// <param name="target">Target.</param>
	public void ShootCritter(Critter target)
	{
		if(!canShoot) return;
		bullet = ((GameObject)Instantiate(bulletPrefab,transform.position,Quaternion.identity)).GetComponent<Bullet>();
		bullet.speed = speed;
		bullet.damage = damage;
		bullet.splashDamageModifier = isSplashDamage?splashDamageModifier:0f;
		bullet.isSplashDamage = isSplashDamage;
		bullet.splashDamageRadius = splashDamageRadius;
		bullet.slowAmount = isSlow?slowAmount:0f;
		bullet.isSlow = isSlow;
		bullet.Target = target;
		range.enabled = false;
		canShoot = false;
		fireTimer = fireRate;
	}

	/// <summary>
	/// Selects the tower.
	/// </summary>
	/// <description>
	/// Selects this tower and renders its range if it's not already selected. 
	/// Unselects all towers and selects this tower if another tower was already selected.
	/// </description>
	public void SelectTower()
	{
		if(isSelected == true) return;
		if(GameManager.Instance.TowerManager.TowerSelected)
			GameManager.Instance.TowerManager.UnSelectAllTowers();
		isSelected = true;
		rangeMeshRenderer.enabled = true;
		GameManager.Instance.TowerManager.SelectedTower = this;
		GameManager.Instance.TowerManager.TowerSelected = true;
	}

	/// <summary>
	/// Unselects the tower if it is selected.
	/// </summary>
	public void UnSelectTower()
	{
		if(isSelected)
		{
			isSelected = false;
			rangeMeshRenderer.enabled = false;
			GameManager.Instance.TowerManager.SelectedTower = null;
			GameManager.Instance.TowerManager.TowerSelected = false;
		}
	}

	/// <summary>
	/// Raises the mouse enter event.
	/// </summary>
	/// <description>
	/// Renders this tower if mouse enter event is raised.
	/// </description>
	void OnMouseEnter()
	{
		meshRenderer.enabled = true;
	}

	/// <summary>
	/// Raises the mouse exit event.
	/// </summary>
	/// <description>
	/// Derenders this tower if mouse exit event is raised.
	/// </description>
	void OnMouseExit()
	{
		meshRenderer.enabled = false;
	}

	/// <summary>
	/// Raises the mouse down event.
	/// </summary>
	/// <description>
	/// Calls <see cref="SelectTower()"/> if mouse down event is raised.
	/// </description>
	void OnMouseDown()
	{
		SelectTower();
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Tower"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Tower"/>.</returns>
	public override string ToString ()
	{
		string splashInfo = isSplashDamage?string.Format("\nSplash Info:\n\t-Splash Radius: {0:0.00}\n\t-Splash %: {1}",splashDamageRadius,(int)(splashDamageModifier*100)):string.Empty;
		string slowInfo = isSlow?string.Format("\nSlow Info:\n\tSlow %:{0}",(int)(slowAmount*100)):string.Empty;
		return string.Format ("Tower Info:\n\t-Level: {6}\n\t-Damage: {0}\n\t-Range: {1:0.00}\n\t-Fire Rate: {2:0.00}\n\t-Bullet Speed: {3:0.00}{4}{5}",damage,Range,fireRate,speed,splashInfo,slowInfo,upgradeLevel);
	}
}
