using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This class represents the model of a Player.
/// </summary>
public class Player : MonoBehaviour {

	/// <summary>
	/// Player's starting cash.
	/// </summary>
	[SerializeField] int initialCash = 50;
	private int cash = 0;
	/// <summary>
	/// Gets or sets player's cash.
	/// </summary>
	/// <value>The cash.</value>
	public int Cash {get {return cash;} set {cash = Mathf.Clamp(value,0,9999999);}}

	/// <summary>
	/// Player's initial health points.
	/// </summary>
	[SerializeField] int initialHealthPoints = 20;
	/// <summary>
	/// Gets or sets player's health points.
	/// </summary>
	/// <value>The health points.</value>
	public int HealthPoints {get;set;}

	/// <summary>
	/// Text underneath player's health icon.
	/// </summary>
	[SerializeField] Text playerHealthUI = null;
	/// <summary>
	/// Tet underneath player's cash icon.
	/// </summary>
	[SerializeField] Text playerCashUI = null;

	/// <summary>
	/// Awake this instance. Calls <see cref="Setup"/>.
	/// </summary>
	void Awake(){
		Setup();
	}
	
	/// <summary>
	/// Start this instance. Does nothing.
	/// </summary>
	void Start () {
		
	}
	
	/// <summary>
	/// Update this instance of Player's health and cash. 
	/// </summary>
	void Update () {
		playerHealthUI.text = HealthPoints.ToString();
		playerCashUI.text = cash.ToString();
	}

	/// <summary>
	/// Setup this instance and initializes its health and cash attributes.
	/// </summary>
	public void Setup()
	{
		HealthPoints = initialHealthPoints;
		Cash = initialCash;
	}

	/// <summary>
	/// This method is called every time a critter reaches its target position.
	/// 
	/// This instance's health is deducted by 1 for every critter that reaches its target position.
	/// 
	/// <see cref="GameManager.Instance.GameLost()"/> is called if health falls to 0.
	/// </summary>
	public void Leaked()
	{
		HealthPoints--;
		if(HealthPoints <= 0)
			GameManager.Instance.GameLost();
	}
}
