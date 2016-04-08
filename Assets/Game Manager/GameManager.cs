using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// This class is the root of where the game mostly starts from.
/// </summary>
public class GameManager : Singleton<GameManager> {
	
	public enum GameState {Menu,Game,Edit};
	private GameState state = GameState.Menu;

	/// <summary>
	/// Gets a value indicating whether this instance is edit GameState.
	/// </summary>
	/// <value><c>true</c> if this instance is edit; otherwise, <c>false</c>.</value>
	public bool IsEdit {get{return state == GameState.Edit;}}
	/// Canvas that gets displayed and activated when application is in the Edit GameState.
	public GameObject EditCanvas = null;

	/// <summary>
	/// Gets a value indicating whether this instance is menu GameState.
	/// </summary>
	/// <value><c>true</c> if this instance is menu; otherwise, <c>false</c>.</value>
	public bool IsMenu {get{return state == GameState.Menu;}}
	/// Canvas that gets displayed and activated when application is in the Main Menu GameState.
	public GameObject MainMenuCanvas = null;

	/// <summary>
	/// Gets a value indicating whether this instance is game GameState.
	/// </summary>
	/// <value><c>true</c> if this instance is game; otherwise, <c>false</c>.</value>
	public bool IsGame {get{return state == GameState.Game;}}
	/// Canvas that gets displayed and activated when application is in Game GameState.
	public GameObject GameCanvas = null;
	
	public float editorWidth = 10f;
	public float editorHeight = 10f;
	
	[SerializeField] Map map = null;
	/// <summary>
	/// Gets the map.
	/// </summary>
	/// <value>The map.</value>
	public Map Map {get{return map;}}

	[SerializeField] WaveManager waveManager = null;
	/// <summary>
	/// Gets the wave manager.
	/// </summary>
	/// <value>The wave manager.</value>
	public WaveManager WaveManager {get{return waveManager;}}
	
	[SerializeField] EditorManager editor = null;
	/// <summary>
	/// Gets the editor.
	/// </summary>
	/// <value>The editor.</value>
	public EditorManager Editor {get{return editor;}}
	
	[SerializeField] Player player = null;
	/// <summary>
	/// Gets the player.
	/// </summary>
	/// <value>The player.</value>
	public Player Player {get{return player;}}
	
	[SerializeField] TowerManager towerManager = null;
	/// <summary>
	/// Gets the tower manager.
	/// </summary>
	/// <value>The tower manager.</value>
	public TowerManager TowerManager {get{return towerManager;}}
	
	private List<Vector2> path1;
	private List<Vector2> path2;
	private List<Vector2> path3;
	
	[SerializeField] float deathTime = 5f;
	private float deathTimer = 0f;
	[SerializeField] GameObject gameOverUI = null;

	/// <summary>
	/// Awake this instance and calls <see cref="InitializePredefinedPaths()"/> and deactivates <see cref="gameOverUI"/>.
	/// </summary>
	void Awake()
	{
		InitializePredefinedPaths();
		gameOverUI.SetActive(false);
	}

	/// <summary>
	/// Start this instance and calls <see cref="StartMenu()"/>.
	/// </summary>
	void Start () {
		StartMenu();
	}

	/// <summary>
	/// Update this instance and processses the correct controls (i.e: <see cref="ProcessEditorControls()"/>, or
	/// <see cref="ProcessGameControls()"/>, or <see cref="ProcessMenuControls()"/>) depending on GameState. Also,
	/// calls <see cref="UpdateDeathTimer()"/>.
	/// </summary>
	void Update () {
		switch(state)
		{
			case GameState.Edit:
				ProcessEditorControls();
				break;
				
			case GameState.Game:
				ProcessGameControls();
				break;
				
			case GameState.Menu:
				ProcessMenuControls();
				break;
		}
			
		UpdateDeathTimer();
	}

	/// <summary>
	/// Starts the editor by activating its canvas and deactivates the other canvases.
	/// </summary>
	public void StartEditor()
	{
		state = GameState.Edit;
		editor.Setup();
		map.CreateEditorCellGrid((int)editorWidth,(int)editorHeight);
		
		GameCanvas.SetActive(false);
		MainMenuCanvas.SetActive(false);
		EditCanvas.SetActive(true);
	}

	/// <summary>
	/// Starts the game by activating its canvas and deactivates the other canvases.
	/// </summary>
	public void StartGame()
	{
		state = GameState.Game;
		Player.Setup();
		WaveManager.Setup();
		
		EditCanvas.SetActive(false);
		MainMenuCanvas.SetActive(false);
		GameCanvas.SetActive(true);
	}

	/// <summary>
	/// Starts the menu by activating its canvas and deactivates the other canvases.
	/// </summary>
	public void StartMenu()
	{
		gameOverUI.SetActive(false);
		state = GameState.Menu;
		map.ClearMap();
		
		EditCanvas.SetActive(false);
		GameCanvas.SetActive(false);
		MainMenuCanvas.SetActive(true);
	}

	// Processes the editor controls by calling the EditorManager's ProcessControls() method.
	private void ProcessEditorControls()
	{
		Editor.ProcessControls();
	}

	//Processes the game controls by calling the WaveManager's & TowerManager's ProcessControls() methods.
	private void ProcessGameControls()
	{
		WaveManager.ProcessControls();
		TowerManager.ProcessControls();
	}

	// Processes the menu controls based on keyboard inputs.
	private void ProcessMenuControls()
	{
		if(Input.GetKeyDown(KeyCode.Alpha1))
			LaunchGamePath1();
		if(Input.GetKeyDown(KeyCode.Alpha2))
			LaunchGamePath2();
		if(Input.GetKeyDown(KeyCode.Alpha3))
			LaunchGamePath3();
		else if(Input.GetKeyDown(KeyCode.E))
			StartEditor();
		else if(Input.GetKeyDown(KeyCode.Escape))
			CloseApplication();
	}

	/// <summary>
	/// Initializes the predefined paths.
	/// </summary>
	#region predefmaps
	void InitializePredefinedPaths()
	{
		path1 = new List<Vector2>
		{
			new Vector2(0,5),
			new Vector2(1,5),
			new Vector2(2,5),
			new Vector2(3,5),
			new Vector2(4,5),
			new Vector2(5,5),
			new Vector2(6,5),
			new Vector2(7,5),
			new Vector2(8,5),
			new Vector2(9,5)
		};
		
		path2 = new List<Vector2>
		{
			new Vector2(0,2),
			new Vector2(1,2),
			new Vector2(2,2),
			new Vector2(3,2),
			new Vector2(4,2),
			new Vector2(4,3),
			new Vector2(4,4),
			new Vector2(4,5),
			new Vector2(4,6),
			new Vector2(5,6),
			new Vector2(6,6),
			new Vector2(7,6),
			new Vector2(8,6),
			new Vector2(8,5),
			new Vector2(8,4),
			new Vector2(7,4),
			new Vector2(6,4),
			new Vector2(6,3),
			new Vector2(6,2),
			new Vector2(7,2),
			new Vector2(8,2),
			new Vector2(9,2),
			new Vector2(10,2),
			new Vector2(11,2),
			new Vector2(12,2),
			new Vector2(12,3),
			new Vector2(12,4),
			new Vector2(12,5),
			new Vector2(12,6),
			new Vector2(12,7),
			new Vector2(12,8),
			new Vector2(12,9)
		};
		
		path3 = new List<Vector2>();
		for(int i = 19; i >= 0; i--)
			path3.Add(new Vector2(0,i));
		for(int i = 1; i < 20; i++)
			path3.Add(new Vector2(i,0));
		for(int i = 1; i < 20; i++)
			path3.Add(new Vector2(19,i));
	}
	#endregion

	/// <summary>
	/// Launches <see cref="path1"/> and calls <see cref="StartGame()"/>.
	/// </summary>
	public void LaunchGamePath1()
	{
		map.LoadMap(10,10,path1);
		StartGame();
	}

	/// <summary>
	/// Launches <see cref="path2"/> and calls <see cref="StartGame()"/>.
	/// </summary>
	public void LaunchGamePath2()
	{
		map.LoadMap(20,10,path2);
		StartGame();
	}

	/// <summary>
	/// Launches <see cref="path3"/> and calls <see cref="StartGame()"/>.
	/// </summary>
	public void LaunchGamePath3()
	{
		map.LoadMap(20,20,path3);
		StartGame();
	}

	/// <summary>
	/// Launches the map created from the editor and calls <see cref="StartGame()"/>.
	/// </summary>
	public void LaunchEditorGame()
	{
		map.CreateMapFromEditorMap();
		StartGame();
	}

	/// <summary>
	/// Activates the <see cref="gameOverUI"/> for a certain amount of time pertaining to the <see cref="deathTime"/> 
	/// and deletes all towers and critters off the map.
	/// </summary>
	public void GameLost()
	{
		//Display Death for a while
		deathTimer = deathTime;
		gameOverUI.SetActive(true);
		waveManager.GameOver();
		towerManager.DeleteAllTowers();
	}

	/// <summary>
	/// Deletes all towers and critters off the map and calls <see cref="StartMenu()"/>.
	/// This method is called whenever the "Main Menu" button is clicked.
	/// </summary>
	public void InstantGameLost()
	{
		waveManager.GameOver();
		towerManager.DeleteAllTowers();
		StartMenu();
	}

	/// <summary>
	/// Updates the death timer, calls <see cref="StartMenu()"/>, and deactivates the <see cref="gameOverUI"/>.
	/// </summary>
	private void UpdateDeathTimer()
	{
		if(deathTimer > 0f)
		{
			deathTimer -= Time.deltaTime;
			if(deathTimer <= 0)
			{
				gameOverUI.SetActive(false);
				StartMenu();
			}
		}
	}


	/// <summary>
	/// Closes the application. This method is called when the "Eit to Desktop" button is clicked.
	/// </summary>
	public void CloseApplication()
	{
		Application.Quit();
	}

	/// <summary>
	/// Changes the width of the editor map whenever the value on the slider is changed
	/// </summary>
	/// <param name="width">Width.</param>
	public void WidthValueChanged(float width)
	{
		editorWidth = width;
	}

	/// <summary>
	/// Changes the height og the editor map whenever the value on the slider is changed.
	/// </summary>
	/// <param name="height">Height.</param>
	public void HeightValueChanged(float height)
	{
		editorHeight = height;
	}
	
}
