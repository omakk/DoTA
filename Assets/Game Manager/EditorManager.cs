using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

/// <summary>
/// This class controls Map Editor functionality.
/// </summary>
public class EditorManager : MonoBehaviour
{
	[SerializeField] Map map = null;
	private bool drawing = true;
	/// <summary>
	/// Gets a value indicating whether this <see cref="EditorManager"/> is drawing.
	/// </summary>
	/// <value><c>true</c> if drawing; otherwise, <c>false</c>.</value>
	public bool Drawing {get{return drawing;}}

	/// <summary>
	/// A potential start cell.
	/// </summary>
	public EditorCell potentialStartCell1 = null;
	/// <summary>
	/// Another potential start cell.
	/// </summary>
	public EditorCell potentialStartCell2 = null;
	private bool choiceMade = false;
	/// <summary>
	/// Gets a value indicating whether this <see cref="EditorManager"/> has made a choice.
	/// </summary>
	/// <value><c>true</c> if choice made; otherwise, <c>false</c>.</value>
	public bool ChoiceMade {get{return choiceMade;}}
	
	[SerializeField] Text drawToolTip = null;
	[SerializeField] GameObject doneDrawingButton = null;
	[SerializeField] Text choiceToolTip = null;
	[SerializeField] Text acknowledgeToolTip = null;
	[SerializeField] GameObject acknowledgeButton = null;
	[SerializeField] GameObject acknowledgeDenyButton = null;
	[SerializeField] GameObject backToDrawButton = null;
	

	/// <summary>
	/// Awake this instance and call <see cref="Setup()"/>.
	/// </summary>
	void Awake ()
	{
		Setup();
	}
	/// <summary>
	/// Setup this instance by enabling<see cref="Drawing"/> and calling <see cref="ShowDrawingUI()"/>.
	/// Also, hides other UI's by calling <see cref="HideChoiceUI()"/> & <see cref="HideAcknowledgeUI()"/>.
	/// </summary>
	public void Setup()
	{
		drawing = true;
		choiceMade = false;
		doneDrawingButton.GetComponent<Button>().interactable = false;
		HideChoiceUI();
		HideAcknowledgeUI();
		ShowDrawingUI();
	}

	/// <summary>
	/// Uupdate this instance and show the apprpriate UI according to GameState.
	/// </summary>
	void Update()
	{
		if(!GameManager.Instance.IsEdit)
		{
			HideChoiceUI();
			HideAcknowledgeUI();
			HideDrawingUI();
		}
		else if(Drawing)
		{
			HideChoiceUI();
			HideAcknowledgeUI();
			ShowDrawingUI();
		}
		else
		{
			HideDrawingUI();
			if(choiceMade)
			{
				HideChoiceUI();
				ShowAcknowledgeUI();
			}
			else
			{
				HideAcknowledgeUI();
				ShowChoiceUI();
			}
		}	
		
	}
	
	#region UI
	/// <summary>
	/// Enables and activates the Tooltips and Buttons specified for drawing.
	/// </summary>
	public void ShowDrawingUI()
	{
		drawToolTip.enabled = true;
		doneDrawingButton.SetActive(true);
	}

	/// <summary>
	/// Disenables and deactivates the Tooltips and Buttons specified for drawing.
	/// </summary>
	public void HideDrawingUI()
	{
		drawToolTip.enabled = false;
		doneDrawingButton.SetActive(false);
	}

	/// <summary>
	/// Enables and activates the Tooltips and Buttons specified for choosing a start Cell.
	/// </summary>
	public void ShowChoiceUI()
	{
		choiceToolTip.enabled = true;
		backToDrawButton.SetActive(true);
	}

	/// <summary>
	/// Disenables and deactivates the Tooltips and Buttons specified for choosing a start Cell.
	/// </summary>
	public void HideChoiceUI()
	{
		choiceToolTip.enabled = false;
		backToDrawButton.SetActive(false);
	}

	/// <summary>
	/// Enables and activates the Tooltips and Buttons specified for acknowledging succesful Map creation.
	/// </summary>
	public void ShowAcknowledgeUI()
	{
		acknowledgeToolTip.enabled = true;
		acknowledgeButton.SetActive(true);
		acknowledgeDenyButton.SetActive(true);
	}

	/// <summary>
	/// Disenables and deactivates the Tooltips and Buttons specified for acknowledging succesful Map creation.
	/// </summary>
	public void HideAcknowledgeUI()
	{
		acknowledgeToolTip.enabled = false;
		acknowledgeButton.SetActive(false);
		acknowledgeDenyButton.SetActive(false);
	}
	#endregion

	/// <summary>
	/// Processes keyboard inputs where applicable while editing a Map
	/// </summary>
	public void ProcessControls()
	{
		if(Drawing)
		{
			if(Input.GetKeyDown(KeyCode.Return))
				DoneDrawing();
		}
		else //If not drawing then choosing
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				Choose1();
			}
			else if(Input.GetKeyDown(KeyCode.Alpha2))
			{
				Choose2();
			}
			else if(Input.GetKeyDown(KeyCode.Backspace))
			{
				BackToDrawing();
			}
		}
	}

	/// <summary>
	/// Prompts the user to choose the starting cell if the map is valid, and consequently finishes drawing.
	/// </summary>
	public void DoneDrawing()
	{
		if(CheckPathValidity()){
			drawing = false;
			choiceMade = false;
			potentialStartCell1.DisplayNumber1();
			potentialStartCell2.DisplayNumber2();
		}else {
			drawing = true;
			choiceMade = false;
		}
	}

	/// <summary>
	/// Ascribes the cell branded with the number "1" as the the start Cell and the cell beanded with
	/// the number "2" as the end Cell.
	/// </summary>
	public void Choose1()
	{
		potentialStartCell1.ChangeType(EditorCell.CellType.start);
		potentialStartCell2.ChangeType(EditorCell.CellType.end);
		choiceMade = true;
	}

	/// <summary>
	/// Ascribes the cell branded with the number "2" as the the start Cell and the cell beanded with
	/// the number "1" as the end Cell.
	/// </summary>
	public void Choose2()
	{
		potentialStartCell1.ChangeType(EditorCell.CellType.end);
		potentialStartCell2.ChangeType(EditorCell.CellType.start);
		choiceMade = true;
	}

	/// <summary>
	/// Re-enables drawing mode, remove the number displays and changes assignmed start and end Cells
	/// back to path types.
	/// </summary>
	public void BackToDrawing()
	{
		drawing = true;
		choiceMade = false;
		potentialStartCell1.RemoveNumberDisplay();
		potentialStartCell2.RemoveNumberDisplay();
		potentialStartCell1.ChangeType(EditorCell.CellType.path);
		potentialStartCell2.ChangeType(EditorCell.CellType.path);
	}

	/// <summary>
	/// Checks the validity of the path created.
	/// </summary>
	/// <returns><c>true</c>, if path validity was checked, <c>false</c> otherwise.</returns>
	public bool CheckPathValidity()
	{
		int singleNeighbourCount = 0;
		int dualNeighbourCount = 0;
		int pathCellsCount = 0;
		bool success = true;
		
		//Count neighbours for each path cell
		foreach(Cell cell in map.CellGrid)
		{
			EditorCell editorCell = cell as EditorCell;
			if(editorCell == null || editorCell.type != EditorCell.CellType.path)
				continue;
				
			if(editorCell.NumberAdjacentCells == 1)
			{
				singleNeighbourCount++;
				if(singleNeighbourCount == 1)
					potentialStartCell1 = editorCell;
				else if(singleNeighbourCount == 2)
					potentialStartCell2 = editorCell;
			}
			else if(editorCell.NumberAdjacentCells == 2)
				dualNeighbourCount++;
			
			pathCellsCount++;
		}
		
		//check length
		if(pathCellsCount < 2)
			success = false;
			
		if(singleNeighbourCount != 2)
			success = false;
			
		if(singleNeighbourCount + dualNeighbourCount != pathCellsCount)
			success = false;
			
		doneDrawingButton.GetComponent<Button>().interactable = success;
			
		return success;
	}
}

