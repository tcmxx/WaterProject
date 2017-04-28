using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Threading;
using System.Collections.Generic;



/// <summary>
/// This script can be attached to any charactor/enemy type object to control its general movement like horizontal movement and wall jump. It uses the circle collide 2D attached to that object to decide the collision with ground and move direciton.
/// A rigid body must be attached to the object for this to work.
/// When this script is attached, the friction will not effect the movenment. It controls the velocity directly.
/// </summary>
public class GeneralMovementControl : MonoBehaviour, IGeneralMovement {



	private Rigidbody2D objectRigidBody2D;
	private CommonObject attachedCommonObject;



	public bool enalbeWallJump = true;


	//this keep a list of all collisions currently with the object for use of the features.
	private Dictionary<GameObject, Collision2D> currentColllisions;



	#region horizontal move and ground related
	/// <summary>
	/// Horizaontal Acceleration on regular ground. The final acceleration used will be this value multiplied by the groundAccelerationFactor.
	/// </summary>
	public float groundAccelerationMove;
	public float groundAccelerationStop;
	/// <summary>
	/// The final acceleration used will be groundAcceleration multiplied by  each entry of the Factor.
	/// </summary>
	public float groundAccelerationFactor{ get{return groundAccelerationFactorField;} set{groundAccelerationFactorField = value;}}
	private float groundAccelerationFactorField = 1f;


	/// <summary>
	///Horizaontal Acceleration on regular air. The final acceleration used will be this value multiplied by the groundAccelerationFactor.
	/// </summary>
	public float airAcceleration;
	/// <summary>
	/// The final acceleration used will be airAcceleration multiplied by airAccelerationFactor.
	/// </summary>
	public float airAccelerationFactor{ get{return airAccelerationFactorField;} set{airAccelerationFactorField = value;}}
	private float airAccelerationFactorField = 1f;


	/// <summary>
	/// The max horizontal speed. If the speed is larger that this, it will decrease to this with the acceleration defined.
	/// </summary>
	public float maxHorizontalSpeed;

	/// <summary>
	/// The final max Horizontal Speed used will be maxHorizontalSpeed multiplied by maxHorizontalSpeedFactor.
	/// </summary>
	public float maxHorizontalSpeedFactor{  get{return maxHorizontalSpeedFactorField;} set{maxHorizontalSpeedFactorField = value;}}
	private float maxHorizontalSpeedFactorField = 1f;

	//when the speed is less than this, go to target speed directly
	public float restHorizontalSpeed;




	public float maxUpSlopeAngle{  get{return maxUpSlopeAngleField;} set{maxUpSlopeAngleField = value; cosSlopeAngle = Mathf.Cos (maxUpSlopeAngleField*Mathf.PI/180f);}}
	/// <summary>
	/// The max up slope the charactor can go to.
	/// </summary>
	[SerializeField]
	private float maxUpSlopeAngleField;
	private float cosSlopeAngle;

	//when the y speed is less than 0 and on ground, the y speed will be added by this to stay on the ground
	private const float downSpeedModifier = -0.1f;


	/// <summary>
	/// Gets a value indicating whether this object is on ground.
	/// </summary>
	/// <value><c>true</c> if is on ground; otherwise, <c>false</c>.</value>
	public bool isOnGround{get{ return isOnGroundField;}}
	[SerializeField]
	private bool isOnGroundField;


	/// <summary>
	/// Gets the ground angle. 0 means flat
	/// </summary>
	/// <value>The ground angle.</value>
	public Vector2 groundDir{get{ return groundDirField;}}
	[SerializeField]
	private Vector2 groundDirField;

	/// <summary>
	/// Called when the player land on the ground
	/// </summary>
	public UnityEvent OnLanded;
	/// <summary>
	/// Called when the player leave the grouhd
	/// </summary>
	public UnityEvent OnLeaveGround;


	/// <summary>
	/// 1 right is pressed, -1 left is pressed, 0 no direction key is pressed
	/// </summary>
	private int moveDirection = 0;

	public GameObject currentGround{get{ return currentGroundField;}}
	private GameObject currentGroundField;
	private Rigidbody2D currentGroundFieldRB;



	private bool movingRightField = true;
	/// <summary>
	/// Which direction the player should face
	/// </summary>
	/// <value><c>true</c> if moving right; otherwise, <c>false</c>.</value>
	public bool movingRight { get { return movingRightField; } set { movingRightField = value; } }

	[SerializeField]
	private bool airMoveEnabled = true;


	private const float Max_Slope_Error = 0.05f;
	#endregion



	#region wall jump related
	/// <summary>
	/// Only wall surface angle is between those values, the wall can have wall jump
	/// </summary>
	public float minWallJumpAgle = 80f;
	public float maxWallJumpAgle = 100f;
	public float wallJumpPower = 5f;
	public float normalJumpPower = 10f;

	public bool onWall = false;	//pausing on wall or sliding on wall
	public bool wallAtRight;//direction of the wall
	public float slidingWallSpeed = 0.5f;
	private GameObject currentWall = null;//current wall that is attached to 

	#endregion


	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="GeneralMovementControl"/> is active.
	/// When it is not active, it allows other controls to control the movement of the object
	/// </summary>
	/// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
	public bool active{ get { return activeField;} set{ activeField = value;}}
	private bool activeField = true;

	private Vector2 previousPosition;




	void Awake(){
		objectRigidBody2D = GetComponent <Rigidbody2D> ();
		attachedCommonObject = GetComponent <CommonObject>();
		currentColllisions = new Dictionary<GameObject, Collision2D> ();
	}


	// Use this for initialization
	void Start () {

		cosSlopeAngle = Mathf.Cos (maxUpSlopeAngleField*Mathf.PI/180f);
	}






	// Update is called once per frame
	void FixedUpdate () {
	

		if (active) {



			HorizontalMove ();
			OnWallMove ();

		}

		previousPosition = transform.position;

	}






	#region Collision Callbacks

	public void OnCollisionEnter2D(Collision2D col)
	{
		airMoveEnabled = true;

		UpdateCollisionList (col,false);

		CheckWallJumpObject (col);
	}
	public void OnCollisionStay2D(Collision2D col)
	{
		UpdateCollisionList (col,false);
		CheckWallJumpObject (col);

	}



	public void OnCollisionExit2D(Collision2D col)
	{
		UpdateCollisionList (col,true);
		if (col.gameObject == currentWall) {
			DetachWall ();
		}

		//Debug.Log ("Exit collision");
	}

	#endregion




	/// <summary>
	/// Move the specified horizontal.
	/// </summary>
	/// <param name="horizontal">Horizontal. -1 means left, 0 means stop, 1 means right</param>
	public void Move(int horizontal){
		moveDirection = horizontal;

	}

	/// <summary>
	/// Very basic jump
	/// </summary>
	public void Jump(){
		if (isOnGround || attachedCommonObject.isInWater) {
			Rigidbody2D RB = GetComponent<Rigidbody2D> ();
			RB.velocity = new Vector2 (RB.velocity.x, normalJumpPower);
		}
	}


	/// <summary>
	/// Check if this contact can be consider as a ground
	/// </summary>
	/// <returns><c>true</c>, if it is ground  <c>false</c> otherwise.</returns>
	/// <param name="contact">Contact.</param>
	private bool CheckIfGround(ContactPoint2D contact){
		
		Vector2 normal = contact.normal;
		Vector2 contactPoint =contact.point;

		//angle between up and the normal. value is between 0 to 180
		float angle = Mathf.Abs(Vector2.Angle (normal, Vector2.up));

		if (angle < maxUpSlopeAngle) {


			return true;

		} else {
			return false;
		}

	}





	private void HorizontalMove(){
		Vector2 vel = objectRigidBody2D.velocity;
		Vector2 groundVel = Vector2.zero;
		if (currentGroundFieldRB != null) {
			groundVel = currentGroundFieldRB.velocity;
		}

		float targetSpeed = moveDirection * maxHorizontalSpeedFactor * maxHorizontalSpeed + groundVel.x;
		if (!isOnGround && moveDirection == 0) {
			targetSpeed = vel.x;
		}



		float targetDir = targetSpeed - vel.x;

		if (Mathf.Abs (targetDir) < restHorizontalSpeed) {
			targetDir = 0;
		}

		float acc = 0f;

		//whether to use air acc, stop acc or move acc
		if (isOnGround) {
			if ((groundVel.x > vel.x && targetSpeed > vel.x) || (groundVel.x < vel.x && targetSpeed < vel.x)) {
				acc = groundAccelerationStop * groundAccelerationFactorField;
			} else {
				acc = groundAccelerationMove * groundAccelerationFactorField;
			}
		} else if (airMoveEnabled) {
			acc = airAcceleration * airAccelerationFactorField;
		} else {
			return;
		}


		if (targetDir > 0) {
			vel.x += acc * Time.deltaTime;
			if (isOnGround) {
				vel.y += acc * Time.deltaTime * groundDir.y;
				if(vel.y < 0){
					vel.y += downSpeedModifier;
				}

			}
			if (vel.x > targetSpeed) {
				vel.x = targetSpeed;
			}
		} else if (targetDir < 0) {
			vel.x -= acc * Time.deltaTime;
			if (isOnGround) {
				vel.y -= acc * Time.deltaTime * groundDir.y;
				if(vel.y < 0){
					vel.y += downSpeedModifier;
				}

			}
			if (vel.x < targetSpeed) {
				vel.x = targetSpeed;
			}
		} else {
			vel.x = targetSpeed;
		}

		objectRigidBody2D.velocity = vel;

	}




	/// <summary>
	/// check if the collided object allows wall jump. If yes, enable the wall jump of attached character.
	/// </summary>
	private void CheckWallJumpObject(Collision2D col){
		//wall jump is not enabled for this character or the character is currently on ground.
		if (enalbeWallJump == false || isOnGround || attachedCommonObject.isInWater) {
			if (currentWall != null) {
				DetachWall ();
			}
			return;
		}

		GeneralPlatform platform = col.collider.GetComponent <GeneralPlatform> ();
		if (platform == null || !platform.enableWallJump) {
			return;
		}

		Vector2 normal = col.contacts [0].normal;

		if (Mathf.Cos (maxWallJumpAgle*Mathf.PI/180) < normal.y && Mathf.Cos (minWallJumpAgle*Mathf.PI/180) > normal.y) {
			//the angle is within the wall jump range(might want to move the Cos to initialization to improve efficiency)
			onWall = true;
			if (currentWall == col.collider.gameObject && wallAtRight == col.contacts [0].point.x > transform.position.x) {
				//still stay in current wall and the save direction
				//do not do anything
			} else{
				wallAtRight = col.contacts [0].point.x > transform.position.x;
				currentWall = col.collider.gameObject;
			}



		} else {
		}


	}

	public void WallJump(){
		airMoveEnabled = false;
		//curWall = null;
		DetachWall();
		Rigidbody2D RB = GetComponent<Rigidbody2D>();
		RB.velocity = new Vector2((wallAtRight?-1:1)*wallJumpPower, 10);

		movingRight = !wallAtRight;

		//OnWallJump.Invoke ();
		//print ("Jump Off wall");

	}



	private void StartWallSliding(){
	}

	private void DetachWall (){
		onWall = false;
	}
	private void OnWallMove(){
		if (!onWall)
			return;

		if (objectRigidBody2D.velocity.y < 0 && onWall) {
			//objectRigidBody2D.gravityScale = 0;
			Vector3 vel = objectRigidBody2D.velocity;
			vel.y = -slidingWallSpeed;
			objectRigidBody2D.velocity = vel;
			return;
		} else {
		}

	}








	/// <summary>
	/// Checks the whehther to apply falling damage and apply it if necessary. Currently hardcoded.
	/// </summary>
	private void CheckFallingDamage(){
		Vector2 curVel = objectRigidBody2D.velocity;


		if (attachedCommonObject == null) {
			return;
		}
			
		if (curVel.y <= -20) //We should create a const var for this value
		{
			//kill the player
			attachedCommonObject.TakeDamage (4, CommonObject.DamageType.Falling);
		}
		else if (curVel.y <= -15) //We should create a const var for this value
		{
			attachedCommonObject.TakeDamage(2, CommonObject.DamageType.Falling);
		}
		else if (curVel.y <= -10) //We should create a const var for this value
		{
			attachedCommonObject.TakeDamage(1, CommonObject.DamageType.Falling);
		}
	}





	/// <summary>
	/// Updates the collision list for detect whether the player is on ground right now
	/// </summary>
	/// <param name="collision">Collision.</param>
	/// <param name="colExit">If set to <c>true</c> col exit.</param>
	private void UpdateCollisionList(Collision2D collision, bool colExit){

		//update the list
		if (!colExit) {
			currentColllisions [collision.gameObject] = collision;
		} else {
			currentColllisions.Remove (collision.gameObject);
		}

		//Update the ground states
		bool foundGround = false;
		foreach (var v in currentColllisions.Values) {
			foreach (var contact in v.contacts) {
				if (CheckIfGround (contact)) {
					foundGround = true;

					if (isOnGroundField == false) {
						//land on the ground. Call the event
						OnLanded.Invoke ();
					}
					isOnGroundField = true;

					groundDirField.x = contact.normal.y;
					groundDirField.y = -contact.normal.x;
					groundDirField.Normalize ();
					currentGroundField = v.collider.gameObject;
					currentGroundFieldRB = v.collider.GetComponent<Rigidbody2D>();
					break;
				}
			}
			if (foundGround)
				break;
		}

		if (!foundGround) {
			if (isOnGroundField == true) {
				OnLeaveGround.Invoke ();
			}
			isOnGroundField = false;
		}

	}

}
