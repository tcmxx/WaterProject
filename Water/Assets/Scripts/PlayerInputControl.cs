using UnityEngine;
using System.Collections;

public class PlayerInputControl : MonoBehaviour {

	public const float DoubleClickInterval = 0.3f ;
	private bool doubleClickedFirstRight = false;
	private bool doubleClickedFirstLeft = false;
	private float lastClickTime = 0;

	private MainPlayer mainPlayer;
	private GeneralMovementControl generalMovementControl;


	#region Callbacks


	void Awake(){
		mainPlayer = GetComponent <MainPlayer> ();
		generalMovementControl = GetComponent <GeneralMovementControl> ();
	}
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		inputDetect();

	}

	#endregion

	#region Input Control functions


	/// <summary>
	/// This method will be called in every loop in Update()
	/// </summary>
	/// <returns></returns>
	private void inputDetect()
	{
		handleHorizontalInput(Input.GetAxis("Horizontal"));

		CommonObject.InteractDirectionKey dir = CommonObject.InteractDirectionKey.None;
		if (Input.GetAxis ("Vertical") > 0) {
			dir = CommonObject.InteractDirectionKey.Up;
		} else if (Input.GetAxis ("Vertical") < 0) {
			dir = CommonObject.InteractDirectionKey.Down;
		}


		if (Input.GetButtonDown ("Horizontal") && Input.GetAxis ("Horizontal") > 0) {
			if (doubleClickedFirstRight && (Time.time - lastClickTime) < DoubleClickInterval) {
				mainPlayer.Roll (true);
				doubleClickedFirstRight = false;
			} else {
				doubleClickedFirstRight = true;
			}
			doubleClickedFirstLeft = false;
			lastClickTime = Time.time;
		}
		else if(Input.GetButtonDown ("Horizontal") && Input.GetAxis ("Horizontal") < 0){
			if (doubleClickedFirstLeft && (Time.time - lastClickTime) < DoubleClickInterval) {
				mainPlayer.Roll (false);
				doubleClickedFirstLeft = false;
			} else {
				doubleClickedFirstLeft = true;
			}
			doubleClickedFirstRight = false;
			lastClickTime = Time.time;
		}


		if (Input.GetButtonDown("Action"))
		{
			mainPlayer.Interact (mainPlayer.characterInfo.attackMode, CommonObject.InteractKey.Action, dir);
		}

		if (Input.GetButton ("Attack")) {
			mainPlayer.CurAbility.Hold ();
		}
		if (Input.GetButtonUp("Attack"))
		{
			mainPlayer.Interact (mainPlayer.characterInfo.attackMode, CommonObject.InteractKey.Attack, dir);
		}

		if (Input.GetButtonDown("Special"))
		{
			mainPlayer.Interact (mainPlayer.characterInfo.attackMode, CommonObject.InteractKey.Special, dir);
		}

		if (Input.GetButtonDown("Jump"))
		{
			mainPlayer.Jump(true, true);
		}
		mainPlayer.Jump(false, Input.GetButton("Jump"));
		if (Input.GetButtonUp("Jump"))
		{
			mainPlayer.Jump(true, false);
		}

		if (Input.GetButtonDown("CreateTeleporter"))
		{
			mainPlayer.CreateTeleporter();
		}
		if (Input.GetButtonDown("TpToTeleporter"))
		{
			mainPlayer.TpToTeleporter();
		}
		if (Input.GetButtonDown("Test"))
		{
			//used for testing
			mainPlayer.TakeDamage(1);
		}
		if (Input.GetButtonDown("SwitchMode")) {
			mainPlayer.SwitchMode ();
		}

	}


	/// <summary>
	/// Method to handle the horizontal input such as left and right. 
	/// </summary>
	/// <param name="horizontalInput">the inputDetect() will pass the axis value from GetAxis. It can be from -1 to 1 for keyboard and joystick</param>
	/// <returns></returns>
	private void handleHorizontalInput(float horizontalInput)
	{
		//MainPlayer.mainPlayer.move(horizontalInput);

		mainPlayer.Move (horizontalInput);
	}


	#endregion
}
