using UnityEngine;
using System.Collections;

/// <summary>
/// This script will have some properties for flatform. Including  effect the horizontal movement of the object on it if there is a GeneralMOvementControl script attached to that object,
/// walljump enable, water splash enable and so on.
/// </summary>
public class GeneralPlatform : MonoBehaviour {
	

	public bool enableWallJump = false;
	public bool enableWaterSplash = false;


	public float groundAccelerationFactor = 1;
	public float airAccelerationFactor = 1;
	public float maxHorizontalSpeedFactor = 1;

	private ArrayList currentEffectedObjects;

	// Use this for initialization
	void Start () {
		currentEffectedObjects = new ArrayList ();
	}
	


	#region Collision Callbacks

	public void OnCollisionEnter2D(Collision2D col)
	{
		GeneralMovementControl moveControl = col.gameObject.GetComponent <GeneralMovementControl> ();
		if (moveControl != null) {
			currentEffectedObjects.Add (moveControl);
			ChangeMovementProperty (moveControl);
		}
	}


	public void OnCollisionExit2D(Collision2D col)
	{GeneralMovementControl moveControl = col.gameObject.GetComponent <GeneralMovementControl> ();
		if (moveControl != null) {
			currentEffectedObjects.Remove (moveControl);
			UnchangeMovementProperty (moveControl);
		}
	}

	public void OnTriggerEnter2D(Collider2D col)
	{
		GeneralMovementControl moveControl = col.gameObject.GetComponent <GeneralMovementControl> ();
		if (moveControl != null) {
			currentEffectedObjects.Add (moveControl);
			ChangeMovementProperty (moveControl);
		}
	}


	public void OnTriggerExit2D(Collider2D col)
	{GeneralMovementControl moveControl = col.gameObject.GetComponent <GeneralMovementControl> ();
		if (moveControl != null) {
			currentEffectedObjects.Remove (moveControl);
			UnchangeMovementProperty (moveControl);
		}
	}

	#endregion


	private void ChangeMovementProperty(GeneralMovementControl movementControl){
		movementControl.groundAccelerationFactor = movementControl.groundAccelerationFactor * groundAccelerationFactor;
		movementControl.airAccelerationFactor = movementControl.airAccelerationFactor * airAccelerationFactor;
		movementControl.maxHorizontalSpeedFactor = movementControl.maxHorizontalSpeedFactor * maxHorizontalSpeedFactor;
	}

	private void UnchangeMovementProperty(GeneralMovementControl movementControl){
		movementControl.groundAccelerationFactor = movementControl.groundAccelerationFactor / groundAccelerationFactor;
		movementControl.airAccelerationFactor = movementControl.airAccelerationFactor / airAccelerationFactor;
		movementControl.maxHorizontalSpeedFactor = movementControl.maxHorizontalSpeedFactor / maxHorizontalSpeedFactor;
	}


	//check the list for null object and remove them from the list
	private void CheckListForNull(){
		for (int i = 0; i < currentEffectedObjects.Count; i++) {
			if (currentEffectedObjects [i] == null) {
				currentEffectedObjects.RemoveAt (i);
				i--;
				Debug.Log ("Remove object in Object list of " + gameObject.name);
			}
		}
	}


}
