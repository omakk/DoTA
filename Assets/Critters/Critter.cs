using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class represents the model of a Critter.
/// </summary>
public class Critter : MonoBehaviour {

	/// <summary>
	/// Critter's initial health points.
	/// </summary>
	[SerializeField] float initialHealthPoints = 100;
	/// <summary>
	/// Critter's corrected inital health points.
	/// </summary>
	private float correctedInitalHealthPoints = 100;
	/// <summary>
	/// Gets and sets critter's health points.
	/// </summary>
	/// <value>The health points.</value>
	public float HealthPoints {get; private set;}
	/// <summary>
	/// Reward cash for killing this critter.
	/// </summary>
	[SerializeField] int rewardCash = 2;
	/// <summary>
	/// Gets or sets critter's reward cash.
	/// </summary>
	/// <value>The cash.</value>
	public int Cash {get;set;}
	/// <summary>
	/// The index of the target path cell.
	/// </summary>
	int targetPathCellIndex = 0;
	/// <summary>
	/// Critter's close distance.
	/// </summary>
    [SerializeField] float closeDistance = 0.1f;
	/// <summary>
	/// Critter's speed.
	/// </summary>
	[SerializeField] float speed = 0.1f;
	/// <summary>
	/// Critter's initial color.
	/// </summary>
	[SerializeField] Color initialColor = Color.white;
	/// <summary>
	/// Color of critter when dead.
	/// </summary>
	[SerializeField] Color deadColor = Color.red;
	/// <summary>
	/// Critter's mesh renderer.
	/// </summary>
	[SerializeField] MeshRenderer meshRenderer = null;
	/// <summary>
	/// Critter's contour renderer.
	/// </summary>
	[SerializeField] MeshRenderer contourRenderer = null;

	[SerializeField] float slowTime = 4f;
	private float slowTimer = 0f;
	private float slowModifier = 0f;

	/// <summary>
	/// Awake this instance and intializes both the health and reward cash attributes and the mesh renderer.
	/// </summary>
	void Awake(){
		HealthPoints = initialHealthPoints;
		Cash = rewardCash;
		meshRenderer = GetComponent<MeshRenderer>();
	}
	
	/// <summary>
	/// Update this instance once per frame. The target position and current position of the critter will be updated
	/// and be used to maneuvre it towards the next Path Cell by calling <see cref="TargetNextPathCellPosition()"/>. 
	/// 
	/// <see cref="UpdateColor()"/> and <see cref="UpdateSlow()"/> are called straight afterword.
	/// </summary>
	void Update () {
		
		Vector2 targetPosition = new Vector2(TargetPosition.x, TargetPosition.y);
		Vector2 position = new Vector2(transform.position.x, transform.position.y);
		if(Vector2.Distance(targetPosition,position) < closeDistance)
			TargetNextPathCellPosition();
		
		UpdateColor();
		UpdateSlow();
	}

	/// <summary>
	/// Gets the target position depending on the index of the target Path Cell.
	/// </summary>
	/// <returns> The position the critter has to move towards.</returns>
	public Vector3 TargetPosition 
	{
		get
		{
			if(targetPathCellIndex != GameManager.Instance.Map.Path.Count - 1)
				return GameManager.Instance.Map.Path[targetPathCellIndex].transform.position;
			else{
				Vector3 distanceToEndWall = (GameManager.Instance.Map.Path[targetPathCellIndex].transform.position - GameManager.Instance.Map.Path[targetPathCellIndex - 1].transform.position)/2f;
				return GameManager.Instance.Map.Path[targetPathCellIndex].transform.position + distanceToEndWall;
			}
		}
	}

	/// <summary>
	/// Kills the critter and deducts one "life" from the <see cref="Player"/> if the next 
	/// Path Cell targetted from the list of Path Cells is the final one; otherwise, target the next Path Cell 
    /// in the list and record the trajectory towards its position. 
	/// </summary>
	public void TargetNextPathCellPosition()
	{
		if(targetPathCellIndex == GameManager.Instance.Map.Path.Count - 1)
		{
			GameManager.Instance.Player.Leaked();
			KillCritter();
		}
		else
		{
			targetPathCellIndex++;
			Vector3 targetPosition = TargetPosition;
			Vector2 velocity = new Vector2(targetPosition.x - transform.position.x, targetPosition.y - transform.position.y);
			GetComponent<Rigidbody>().velocity = velocity.normalized*(speed - slowModifier*speed);
		}
			
	}

	/// <summary>
	/// Updates the color of the critter depending on its <see cref="HealthPoints"/>.
	/// </summary>
	void UpdateColor()
	{
		meshRenderer.material.color = Color.Lerp(deadColor,initialColor,(float)HealthPoints/correctedInitalHealthPoints);
	}

	/// <summary>
	/// Updates the slow effect on the critter using its <see cref="slowTimer"/> and update its velocity accordingly.
	/// </summary>
	void UpdateSlow()
	{
		if(slowTimer > 0)
		{
			slowTimer -= Time.deltaTime;
			if(slowTimer <= 0)
			{
				slowModifier = 0f;
				Rigidbody rigidbody = GetComponent<Rigidbody>();
				//Return to normal speed
				rigidbody.velocity = rigidbody.velocity.normalized*speed;
			}
		}
	}

	/// <summary>
	/// Applies damage to the critter by deducting a damage value from its <see cref="HealthPoints"/>.
	/// 
	/// The critter is killed if its <see cref="HealthPoints"/> hits or falls below 0.
	/// 
	/// The <see cref="slowMod"/> is applied if that is the case.
	/// </summary>
	/// <param name="damage">Damage.</param>
	/// <param name="slowMod">Slow mod.</param>
	public void ApplyDamage(float damage, float slowMod)
	{
		HealthPoints -= damage;
		if(HealthPoints <= 0)
		{
			GameManager.Instance.Player.Cash += Cash;
			KillCritter();
		}
		else if(slowTimer <= 0 && slowMod > 0f){
			slowTimer = slowTime;
			slowModifier = slowMod;
			Rigidbody rigidbody = GetComponent<Rigidbody>();
			//Give slower speed (ex: if speed = 1 and slowMod = 0.25, speed => 0.75
			rigidbody.velocity = rigidbody.velocity.normalized*(speed - speed*slowModifier);
		}
	}

	/// <summary>
	/// Kills this critter and notifies the <see cref="GameManager"/>.
	/// </summary>
	void KillCritter()
	{
		Destroy(gameObject);
		GameManager.Instance.WaveManager.CritterKilled();
	}

	/// <summary>
	/// Sets the difficulty and updates the health and reward cash attributes of this critter accordingly.
	/// 
	/// <see cref="HealthPoints"/> is increased by (<see cref="diffucltyIncrease"/> * 100)% and
	/// the reward cash, <see cref="Cash"/>, is increased  by itself * <see cref="difficultyIncrease"/> * 2.
	/// </summary>
	/// <param name="difficultyIncrease">Difficulty increase.</param>
	public void SetDifficulty(float difficultyIncrease)
	{
		HealthPoints *= (1f + difficultyIncrease);
		correctedInitalHealthPoints = HealthPoints;
		Cash = Cash + (int)(Cash*2*difficultyIncrease);
	}
}
