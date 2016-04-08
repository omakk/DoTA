using UnityEngine;
using System.Collections;

/// <summary>
/// This class represents the model of a Cell.
/// </summary>
public class Cell : MonoBehaviour {

	/// <summary>
	/// Cell contour renderer.
	/// </summary>
	[SerializeField] MeshRenderer contourRenderer = null;
	private int x = 0; //Initialize width
	private int y = 0; //Intiialize height
	/// <summary>
	/// Gets Cell x-value.
	/// </summary>
	/// <value>The x.</value>
	public int X {get{return x;}}
	/// <summary>
	/// Gets Cell y-value.
	/// </summary>
	/// <value>The y.</value>
	public int Y {get{return y;}}

	/// <summary>
	/// Awake this instance and render its contour.
	/// </summary>
	protected virtual void Awake(){
		contourRenderer.enabled = false;
	}

	/// <summary>
	/// Start this instance and do nothing.
	/// </summary>
	protected virtual void Start () {
	
	}
	
	/// <summary>
	/// Update this instance every frame without doing anything.
	/// </summary>
	protected virtual void Update () {
	
	}

	/// <summary>
	/// Enable the Cell's <see cref="contourRenderer"/> when mouse eneter event is raised.
	/// </summary>
	protected virtual void OnMouseEnter()
	{
		contourRenderer.enabled = true;
	}

	/// <summary>
	/// Disenable the Cell's <see cref="contourRenderer"/> when mouse exit event is raised.
	/// </summary>
	protected virtual void OnMouseExit()
	{
		contourRenderer.enabled = false;
	}

	/// <summary>
	/// Do nothing when mouse down event is raised.
	/// </summary>
	protected virtual void OnMouseDown()
	{
		//Purposefully does nothing
	}

	/// <summary>
	/// Sets the position given a width and a height.
	/// </summary>
	/// <param name="w">The width.</param>
	/// <param name="h">The height.</param>
	public void SetPosition(int w, int h)
	{
		x = w;
		y = h;
	}

	/// <summary>
	/// Returns a <see cref="System.String"/> that represents the current <see cref="Cell"/>.
	/// </summary>
	/// <returns>A <see cref="System.String"/> that represents the current <see cref="Cell"/>.</returns>
	public override string ToString ()
	{
		return string.Format ("[Cell: X={0}, Y={1}]", X, Y);
	}
}
