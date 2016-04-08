using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This class controls wave functionality.
/// </summary>
public class WaveManager : MonoBehaviour {

	[SerializeField] Map map = null;

	[SerializeField] float difficultyIncreasePercentage = 5f;
	/// <summary>
	/// Gets the difficulty increase.
	/// </summary>
	/// <value>The difficulty increase.</value>
	public float DifficultyIncrease{get{return difficultyIncreasePercentage/100f;}}

	/// Level of the wave.
	private int lvl = 0;
	
	private float initialTimeBetweenSpawns = 1f;
	/// <summary>
	/// Gets the time between spawns.
	/// </summary>
	/// <value>The time between spawns.</value>
	public float TimeBetweenSpawns {get{return initialTimeBetweenSpawns - lvl*DifficultyIncrease;}}
	
	[SerializeField] int intitialSpawnAmount = 5;
	[SerializeField] int spawnAmountIncreaseRate = 5;
	/// <summary>
	/// Gets the spawn amount. All levels between every other interval of two consecutive multiples of 5 will get increasingly harder.
	/// </summary>
	/// <value>The spawn amount.</value>
	public int SpawnAmount {get{return intitialSpawnAmount + lvl%spawnAmountIncreaseRate;}}
	
	int spawnedCritters = 0;
	float lastSpawnTime = 0f;
	bool spawningCritters = false;
	int crittersKilled = 0;

	Critter newCritter = null;
	
	bool prompting = true;
	/// <summary>
	/// Gets a value indicating whether this <see cref="WaveManager"/> is prompting.
	/// </summary>
	/// <value><c>true</c> if prompting; otherwise, <c>false</c>.</value>
	public bool Prompting {get{return prompting;}}
	
	[SerializeField] GameObject normalCritterPrefab = null;
	[SerializeField] GameObject fastCritterPrefab = null;
	[SerializeField] GameObject tankCritterPrefab = null;
	[SerializeField] GameObject critterContainer = null;

	/// <summary>
	/// Gets the critter container.
	/// </summary>
	/// <value>The critter container.</value>
	public GameObject CritterContainer {get{return critterContainer;}}
	
	[SerializeField] GameObject nextWaveButton = null;

	/// <summary>
	/// Awake this instance and does nothing.
	/// </summary>
	void Awake(){	
	}

	/// <summary>
	/// Fixed update of this instance and launches wave of critters.
	/// </summary>
	void FixedUpdate(){
		
		if(!spawningCritters)
			return;
		float currentTime = Time.time;
		if(currentTime - lastSpawnTime > TimeBetweenSpawns)
		{
			//get position on outer side of spawner
			Vector3 outerWallDistance = (map.StartCell.transform.position - map.Path[1].transform.position)/2f;
			Vector3 newPosition = map.StartCell.transform.position + outerWallDistance;
			newPosition.z -= map.StartCell.transform.localScale.z;
			GameObject critter = (GameObject)Instantiate(SelectCritterPrefab(),newPosition,Quaternion.Euler(0f,0f,180f));
			critter.transform.SetParent(critterContainer.transform,true);
			newCritter = critter.GetComponent<Critter>();
			newCritter.SetDifficulty(DifficultyIncrease);
			newCritter.TargetNextPathCellPosition();
			spawnedCritters++;
			lastSpawnTime = currentTime;
			
			if(spawnedCritters >= SpawnAmount)
				spawningCritters = false;
		}
	}

	/// <summary>
	/// Setups and intializes this instance.
	/// </summary>
	public void Setup()
	{
		lvl = 0;
		spawnedCritters = 0;
		spawningCritters = false;
		lastSpawnTime = Time.time;
		crittersKilled = SpawnAmount;
		prompting = true;
		nextWaveButton.GetComponent<Button>().interactable = prompting;
	}

	/// <summary>
	/// Processes the controls.
	/// </summary>
	public void ProcessControls()
	{
		// Purposefully empty
	}

	/// <summary>
	/// Selects the critter prefab and determines which type of critter gets spawned depending on level.
	/// </summary>
	/// <returns>The critter prefab.</returns>
	GameObject SelectCritterPrefab()
	{
		GameObject prefab = null;
		if(lvl%3 == 0)
		{
			prefab = fastCritterPrefab;
		}
		else if(lvl%5 == 0)
		{
			prefab = tankCritterPrefab;
		}
		else
		{
			prefab = normalCritterPrefab;
		}
		return prefab;
	}

	/// <summary>
	/// Prepares the next wave.
	/// </summary>
	public void PrepareNextWave()
	{
		//UI Show prompt. prompt user for next level start
		LevelUp();
		prompting = true;
		nextWaveButton.GetComponent<Button>().interactable = prompting;
	}

	/// <summary>
	/// Increases level.
	/// </summary>
	public void LevelUp()
	{
		lvl++;
	}

	/// <summary>
	/// Counts critters killed and calls <see cref="PrepareNetWave()"/> if all critters spawned have been killed.
	/// </summary>
	public void CritterKilled()
	{
		crittersKilled++;
		if(crittersKilled >= SpawnAmount)
			PrepareNextWave();
	}

	/// <summary>
	/// Spawns critters. This method is called when the "Next Wave" button is clicked.
	/// </summary>
	public void SpawnCritters()
	{
		if(spawningCritters == true)
			return;
			
		spawnedCritters = 0;
		crittersKilled = 0;
		spawningCritters = true;
		lastSpawnTime = Time.time;
		prompting = false;
		nextWaveButton.GetComponent<Button>().interactable = prompting;
	}

	/// <summary>
	/// Destroys all critter objects when game is over.
	/// </summary>
	public void GameOver()
	{
		prompting = false;
		spawningCritters = false;
		Critter[] critters = critterContainer.GetComponentsInChildren<Critter>();
		foreach(Critter critter in critters)
			Destroy(critter.gameObject);
	}
	
}
