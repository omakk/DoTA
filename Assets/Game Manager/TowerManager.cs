using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// This class controls Tower functionality.
/// </summary>
public class TowerManager : MonoBehaviour
{
	private bool placingTower = false;
	/// <summary>
	/// Gets a value indicating whether this <see cref="TowerManager"/> placing tower.
	/// </summary>
	/// <value><c>true</c> if placing tower; otherwise, <c>false</c>.</value>
	public bool PlacingTower {get{return placingTower;}}
	
	[SerializeField] GameObject towerContainer = null;
	/// <summary>
	/// Gets the tower container.
	/// </summary>
	/// <value>The tower container.</value>
	public GameObject TowerContainer {get{return towerContainer;}}
	
	[SerializeField] GameObject normalTowerPrefab = null;
	/// <summary>
	/// Gets the tower prefab.
	/// </summary>
	/// <value>The tower prefab.</value>
	public GameObject TowerPrefab {get{return normalTowerPrefab;}}
	
	[SerializeField] GameObject splashTowerPrefab = null;
	/// <summary>
	/// Gets the splash tower prefab.
	/// </summary>
	/// <value>The splash tower prefab.</value>
	public GameObject SplashTowerPrefab {get{return splashTowerPrefab;}}

	[SerializeField] GameObject slowTowerPrefab = null;
	/// <summary>
	/// Gets the slow tower prefab.
	/// </summary>
	/// <value>The slow tower prefab.</value>
	public GameObject SlowTowerPrefab {get{return slowTowerPrefab;}}
	
	public enum TowerTypes {normal,splash, slow}
	public TowerTypes selectedType = TowerTypes.normal;

	/// <summary>
	/// Used to check if a tower is selected.
	/// </summary>
	public bool TowerSelected = false;
	/// <summary>
	/// The selected tower object.
	/// </summary>
	public Tower SelectedTower = null;

	/// <summary>
	/// Gets the normal price.
	/// </summary>
	/// <value>The normal price.</value>
	public int NormalPrice {get{return 10;}}
	/// <summary>
	/// Gets the splash price.
	/// </summary>
	/// <value>The splash price.</value>
	public int SplashPrice {get{return 15;}}
	/// <summary>
	/// Gets the slow price.
	/// </summary>
	/// <value>The slow price.</value>
	public int SlowPrice {get{return 12;}}
	/// <summary>
	/// Gets the selected tower price.
	/// </summary>
	/// <value>The selected tower price.</value>
	public int SelectedTowerPrice {get
	{
		int price = 100000;
		if(selectedType == TowerTypes.normal)
			price = NormalPrice;
		if(selectedType == TowerTypes.splash)
			price = SplashPrice;
		if(selectedType == TowerTypes.slow)
			price = SlowPrice;
		return price;
	}} 
	
	[SerializeField] float upgradeModifier = 1.25f;
	
	[SerializeField] GameObject normalTowerButton = null;
	[SerializeField] GameObject splashTowerButton = null;
	[SerializeField] GameObject slowTowerButton = null;
	
	[SerializeField] Text normalPriceTag = null;
	[SerializeField] Text splashPriceTag = null;
	[SerializeField] Text slowPriceTag = null;
	
	[SerializeField] GameObject upgradeTowerWindow = null;
	[SerializeField] GameObject upgradeTowerButton = null;
	[SerializeField] Text sellTowerCost = null;
	[SerializeField] Text upgradeCost = null;
	[SerializeField] Text towerInfo = null;
	
	/// <summary>
	/// Start and intialize this instance.
	/// </summary>
	void Start ()
	{
		normalPriceTag.text = NormalPrice.ToString();
		splashPriceTag.text = SplashPrice.ToString();
		slowPriceTag.text = SlowPrice.ToString();
	}
	/// <summary>
	/// Update this instance and update all prices and and upgrade info.
	/// </summary>
	void Update ()
	{
		//when shift is released, toggle the placing tower tool
		if(placingTower)
			if(Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
				placingTower = false;
				
		CheckTowerPrices();
		CheckUpgradeInfo();
	}

	/// <summary>
	/// Handles different cases of select keyboard inputs and conditions related to towers
	/// </summary>
	public void ProcessControls()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1) && GameManager.Instance.Player.Cash >= NormalPrice)
			ToggleNormalTowerTool();
		else if(Input.GetKeyDown(KeyCode.Alpha2) && GameManager.Instance.Player.Cash >= SplashPrice)
			ToggleSplashTowerTool();
		else if(Input.GetKeyDown(KeyCode.Alpha3) && GameManager.Instance.Player.Cash >= SlowPrice)
			ToggleSlowTowerTool();
		else if(Input.GetKeyDown(KeyCode.S) && SelectedTower != null && TowerSelected)
			SellTower();
		else if(Input.GetKeyDown(KeyCode.Backspace))
			UnSelectAllTowers();
		else if(Input.GetKeyDown(KeyCode.U) && SelectedTower != null)
			UpgradeTower();
	}

	/// <summary>
	/// Checks the tower prices and determine whether towers can be purchased or not.
	/// </summary>
	void CheckTowerPrices()
	{
		int cash = GameManager.Instance.Player.Cash;
		
		if(cash < NormalPrice)
			normalTowerButton.GetComponent<Button>().interactable = false;
		else
			normalTowerButton.GetComponent<Button>().interactable = true;
			
		if(cash < SplashPrice)
			splashTowerButton.GetComponent<Button>().interactable = false;
		else
			splashTowerButton.GetComponent<Button>().interactable = true;
			
		if(cash < SlowPrice)
			slowTowerButton.GetComponent<Button>().interactable = false;
		else
			slowTowerButton.GetComponent<Button>().interactable = true;
	}

	/// <summary>
	/// Checks the upgrade info and displays it. Also, will either enable or disenable the upgrade button if
	/// the tower can legally upgrade.
	/// </summary>
	void CheckUpgradeInfo()
	{
		if(SelectedTower == null)
			upgradeTowerWindow.SetActive(false);
		else
		{
			upgradeTowerWindow.SetActive(true);
			towerInfo.text = SelectedTower.ToString();
			upgradeCost.text = SelectedTower.UpgradeCost.ToString();
			sellTowerCost.text = SelectedTower.SellPrice.ToString();
			if(SelectedTower.CanUpgrade)
				upgradeTowerButton.GetComponent<Button>().interactable = true;
			else
				upgradeTowerButton.GetComponent<Button>().interactable = false;
		}
	}

	/// <summary>
	/// Unselects all towers.
	/// </summary>
	public void UnSelectAllTowers()
	{
		if(TowerSelected == false) return;
		Tower[] towers = towerContainer.GetComponentsInChildren<Tower>();
		foreach(Tower tower in towers)
			tower.UnSelectTower();
		SelectedTower = null;
	}

	/// <summary>
	/// Confirms placing normal tower.
	/// </summary>
	public void ToggleNormalTowerTool()
	{
		if(selectedType == TowerTypes.normal)
			placingTower = !placingTower;
		else{
			selectedType = TowerTypes.normal;
			placingTower = true;
		}
		GameManager.Instance.TowerManager.UnSelectAllTowers();
	}

	/// <summary>
	/// Confirms plaing splash tower.
	/// </summary>
	public void ToggleSplashTowerTool()
	{
		if(selectedType == TowerTypes.splash)
			placingTower = !placingTower;
		else{
			selectedType = TowerTypes.splash;
			placingTower = true;
		}
		GameManager.Instance.TowerManager.UnSelectAllTowers();
	}

	/// <summary>
	/// Confirms plaing slow Tower.
	/// </summary>
	public void ToggleSlowTowerTool()
	{
		if(selectedType == TowerTypes.slow)
			placingTower = !placingTower;
		else{
			selectedType = TowerTypes.slow;
			placingTower = true;
		}
		GameManager.Instance.TowerManager.UnSelectAllTowers();
	}

	/// <summary>
	/// Places a tower and validates its placement.
	/// </summary>
	/// <returns>The tower.</returns>
	/// <param name="position">Position.</param>
	public Tower PlaceTower(Vector3 position)
	{
		if(placingTower == false ||  GameManager.Instance.Player.Cash < SelectedTowerPrice) return null;
		Tower newTower = ((GameObject) Instantiate(SelectTowerPrefab(), position, Quaternion.identity)).GetComponent<Tower>();
		newTower.transform.SetParent(towerContainer.transform,true);
		
		if(!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
			placingTower = false;
		
		GameManager.Instance.Player.Cash -= SelectedTowerPrice;
		return newTower;
	}

	/// <summary>
	/// Confirms tower type and then selects the tower Prefab.
	/// </summary>
	/// <returns>The tower type prefab.</returns>
	GameObject SelectTowerPrefab()
	{
		GameObject tower = null;
		switch(selectedType)
		{
			case TowerTypes.normal:
				tower = normalTowerPrefab;
				break;
			case TowerTypes.splash:
				tower = SplashTowerPrefab;
				break;
			case TowerTypes.slow:
				tower = slowTowerPrefab;
				break;
		}
		return tower;
	}

	/// <summary>
	/// Upgrades a tower by applying <see cref="updateModifier"/> on its attributes .
	/// </summary>
	public void UpgradeTower()
	{
		Tower tower = SelectedTower;
		if(!tower.CanUpgrade) return;
		int price = tower.price;
		int upgradeCost = (int)tower.UpgradeCost;
		
		if(GameManager.Instance.Player.Cash < upgradeCost) return;
		
		tower.damage = tower.damage*upgradeModifier;
		tower.Range = tower.Range*upgradeModifier;
		tower.fireRate = tower.fireRate*(2f - upgradeModifier);
		tower.splashDamageModifier = tower.splashDamageModifier*upgradeModifier;
		tower.splashDamageRadius = tower.splashDamageRadius*upgradeModifier;
		tower.slowAmount = tower.slowAmount*upgradeModifier;
		tower.upgradeLevel++;
		tower.price = price + upgradeCost;
		
		GameManager.Instance.Player.Cash -= upgradeCost;
		
		Debug.Log(string.Format("Upgrade tower: upgradeLevel:{0}->{1} Cost:{2} Player now has: {3}",tower.upgradeLevel -1, tower.upgradeLevel, upgradeCost, GameManager.Instance.Player.Cash));
	}

	/// <summary>
	/// Sells and destroys a tower and increases Player's cash by adding the tower's <see cref="SellPrice"/>.
	/// </summary>
	public void SellTower()
	{
		Tower tower = SelectedTower;
		if(tower ==  null) return;

		GameManager.Instance.Player.Cash += tower.SellPrice;
		Debug.Log(string.Format("Sold tower for {0} player now has {1}",tower.SellPrice,GameManager.Instance.Player.Cash));
		Destroy(tower.gameObject);
		
	}

	/// <summary>
	/// Deletes all towers.
	/// </summary>
	public void DeleteAllTowers()
	{
		Tower[] towers = towerContainer.GetComponentsInChildren<Tower>();
		foreach(Tower tower in towers)
			Destroy(tower.gameObject);
	}
}

