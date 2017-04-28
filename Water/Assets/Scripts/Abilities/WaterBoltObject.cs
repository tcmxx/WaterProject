using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using System.Security.Policy;

public class WaterBoltObject : CommonObject
{

    #region Variables
	public GameObject shieldBoltPref;
	public GameObject wavePref;
	public GameObject ColdBuff;
	public GameObject WaterSpashPref;
	public MainPlayer mainPlayerScript;
    //Public Vars
    public float orbitDistance = 1.1f;
    public float orbitDegreesPerSec = 120.0f;
	public const float createShieldTime = 0.2f;
    public Vector3 relativeDistance = Vector3.zero;
    public bool once = true;

	public const float ATTACK_SPEED_WALTERBOLT_DEFAULT = 20f;
	public const float ATTACK_MAX_HOLD_TIME = 1f;
	public const float MAX_LIVE_TIME_NORMAL = 3f;
	public const float MAX_LIVE_TIME_LONG = 10f;
    //States related Vars
    public bool active = false;
    private bool started = false;
	private float holdTime = 0f;
	private bool isFreeze = false;
	private Animator anim;

	//Damage values
	private int waterBoltDamage = 20;
	private int bigWaterBoltDamage = 1000;
	private int waterBoltDamageFreeze = 40;
	private int bigWaterBoltDamageFreeze = 1500;

	//tmp
	private Color originalColor;

	//Water Points and Water Movement Related
	public GameObject waterPoint0;
	public GameObject waterPoint1;
	public GameObject waterPoint2;
	public GameObject waterPoint3;
	public GameObject waterPoint4;
	public GameObject waterPoint5;
	public int waterPointNumber = 0;
	private float bezierTime0 = 0;
	private float bezierTime1 = 0;
	private float bezierTime2 = 0;
	public Transform startPos;
	public GameObject midPoint;
	public GameObject midPointInst;
	public GameObject midPointAtk;
	public Transform lastShieldPos;
	public bool canAttack = false;
	public float frequency;
	public float magnitude;
	private bool once1 = false;
	private bool once2 = false;
	private Vector3 axis;
	private Vector3 pos;
	private bool rightMov = false;
	private Transform startPosAtk;
	private float bezierTime3 = 0;
	public GameObject waterBoltVisual;
	public float firstCurvePointiness;
	public bool doingWaterWave;
	public int waterStep = 0;
	private Vector3 lastWaterWavePos;
	private float bezierTime4 = 0;
	private GameObject closestWater;
	public float getWaterRange = 20;
	public GameObject newWaterWave;
	public GameObject newWaterWaveInst;
    private Rigidbody2D myRB;


    //SF Vars
    [SerializeField]
    private Transform target = null;
	private Vector3 dummyTarget = Vector3.zero;
	private Vector3 shieldPos;
	private bool isCreatingShield = false;
    [SerializeField]
    private float moveSpeed;
	private float attackSpeed = ATTACK_SPEED_WALTERBOLT_DEFAULT;
    #endregion

    #region Start Function
    //Start Function
    void Awake()
    {
		startPos = transform;
		anim = GetComponent <Animator> ();
		Physics2D.IgnoreCollision (MainPlayer.mainPlayer.GetComponent <CircleCollider2D>(), GetComponent <CircleCollider2D>());
		Physics2D.IgnoreCollision (MainPlayer.mainPlayer.GetComponent <PolygonCollider2D>(), GetComponent <CircleCollider2D>());
		originalColor = GetComponentInChildren <SpriteRenderer> ().color;
        myRB = GetComponent<Rigidbody2D>();

		//Find the points
		waterPoint0 = GameObject.Find ("Point0");
		waterPoint1 = GameObject.Find ("Point1");
		waterPoint2 = GameObject.Find ("Point2");
		waterPoint3 = GameObject.Find ("Point3");
		waterPoint4 = GameObject.Find ("Point4");
		waterPoint5 = GameObject.Find ("Point5");

		waterStep = 0;
		doingWaterWave = false;
		frequency = 20.0f;
		magnitude = 0.1f;
		moveSpeed = 2.7f;
		firstCurvePointiness = 1f;
		midPointInst = (GameObject)Instantiate (midPoint, new Vector2( ((transform.position.x + MainPlayer.mainPlayer.transform.position.x) / 2 ), ((transform.position.y + MainPlayer.mainPlayer.transform.position.y) / 2 ) + firstCurvePointiness), Quaternion.identity);
    }
    #endregion

    #region Update Function
    //Update Function
    void Update()
    {
		
		Vector3 playerPos = MainPlayer.mainPlayer.transform.position;
		

		#region Animation stuff
        //Move the water to the playerPos
		if (started == false && (target == null && dummyTarget == Vector3.zero)&& !isCreatingShield)
        {
			//The first movement of the water 
			//The water will go from the instatiate position to the player's waterpoint 0 position
			//It will instantiate the midPoint object so that we can have a midPoint to make the curve
			if (waterPointNumber == 0) {
				
				//Calculate the Curve X position
				float curveX0 = (((1-bezierTime0)*(1-bezierTime0)) * startPos.position.x) + (2 * bezierTime0 * (1 - bezierTime0) * midPointInst.transform.position.x) + ((bezierTime0 * bezierTime0) * waterPoint0.transform.position.x);
				
				//Calculate the Curve Y position
				float curveY0 = (((1-bezierTime0)*(1-bezierTime0)) * startPos.position.y) + (2 * bezierTime0 * (1 - bezierTime0) * midPointInst.transform.position.y) + ((bezierTime0 * bezierTime0) * waterPoint0.transform.position.y);

				//Calculate the Curve Z position
				float curveZ0 = (((1-bezierTime0)*(1-bezierTime0)) * startPos.position.z) + (2 * bezierTime0 * (1 - bezierTime0) * midPointInst.transform.position.z) + ((bezierTime0 * bezierTime0) * waterPoint0.transform.position.z);

				//Increment the time for the Bezier Curve
				bezierTime0 = bezierTime0 + (2 * Time.deltaTime);
				
				//Change the position of the water bolt to the new calculated positions
				transform.position = new Vector3(curveX0, curveY0, curveZ0);
				
				//Calculate the distance to see if its <= 0.1f
				//if its <= 0.1f then go to  the next animation and destroy the midPoint
				float dist0 = Vector3.Distance (transform.position, waterPoint0.transform.position);
				if (dist0 <= 1) {
					waterPointNumber = 1;
					Destroy (midPointInst.gameObject);
				}
				
			//Animation from the waterPoint 0 to the waterPoint 2
			}else if (waterPointNumber == 1) {
				
				//Calculate the Curve X position
				float curveX0 = (((1-bezierTime1)*(1-bezierTime1)) * waterPoint0.transform.position.x) + (2 * bezierTime1 * (1 - bezierTime1) * waterPoint1.transform.position.x) + ((bezierTime1 * bezierTime1) * waterPoint2.transform.position.x);
				
				//Calculate the Curve Y position
				float curveY0 = (((1-bezierTime1)*(1-bezierTime1)) * waterPoint0.transform.position.y) + (2 * bezierTime1 * (1 - bezierTime1) * waterPoint1.transform.position.y) + ((bezierTime1 * bezierTime1) * waterPoint2.transform.position.y);

				//Calculate the Curve Z position
				float curveZ0 = (((1-bezierTime1)*(1-bezierTime1)) * waterPoint0.transform.position.z) + (2 * bezierTime1 * (1 - bezierTime1) * waterPoint1.transform.position.z) + ((bezierTime1 * bezierTime1) * waterPoint2.transform.position.z);

				//Increment the time for the Bezier Curve
				bezierTime1 = bezierTime1 + (10 * Time.deltaTime);
				
				//Change the position of the water bolt to the new calculated position
				transform.position = new Vector3(curveX0, curveY0, curveZ0);

				//Calculate the distance to see if its <= 0.1f
				//if its <= 0.1f then go to  the next animation 
				float dist0 = Vector3.Distance (transform.position, waterPoint2.transform.position);
				if (dist0 <= 1) {
					waterPointNumber = 3;
				}
				
			//Animation form the waterPoint 2 to the waterPoint 4
			}else if (waterPointNumber == 3) {
			
				//Calculate the Curve X position
				float curveX1 = (((1-bezierTime2)*(1-bezierTime2)) * waterPoint2.transform.position.x) + (2 * bezierTime2 * (1 - bezierTime2) * waterPoint3.transform.position.x) + ((bezierTime2 * bezierTime2) * waterPoint4.transform.position.x);
				
				//Calculate the Curve Y position
				float curveY1 = (((1-bezierTime2)*(1-bezierTime2)) * waterPoint2.transform.position.y) + (2 * bezierTime2 * (1 - bezierTime2) * waterPoint3.transform.position.y) + ((bezierTime2 * bezierTime2) * waterPoint4.transform.position.y);

				//Calculate the Curve Y position
				float curveZ1 = (((1-bezierTime2)*(1-bezierTime2)) * waterPoint2.transform.position.z) + (2 * bezierTime2 * (1 - bezierTime2) * waterPoint3.transform.position.z) + ((bezierTime2 * bezierTime2) * waterPoint4.transform.position.z);

				//Increment the time for the Bezier Curve
				bezierTime2 = bezierTime2 + (10 * Time.deltaTime);

				//Change the position of the water bolt to the new calculated position
				transform.position = new Vector3(curveX1, curveY1, curveZ1);

				//Calculate the distance to see if its <= 0.1f
				//if its <= 0.1f then go to  the next animation
				float dist0 = Vector3.Distance (transform.position, waterPoint4.transform.position);
				if (dist0 <= 2) {
					waterPointNumber = 5;
					canAttack = true;
					active = true;
				}
			} else {


				if(!once2){
					//Move the water bolt to the left or the right side of the player
					if (MainPlayer.mainPlayer.generalMovementControl.movingRight){

						transform.position = Vector3.Lerp (transform.position, waterPoint4.transform.position, moveSpeed * Time.deltaTime);
					} else {

						transform.position = Vector3.Lerp (transform.position, waterPoint5.transform.position, moveSpeed * Time.deltaTime);
					}
				}
			}
			
        }
		 
		#endregion
		#region Attack on Update Function
        //Attack the enemy
		if (target != null ) {

			if (once1 == false){

				InstantiateObj ();
				once1 = true;
			}

			//Calculate the Curve X position
			float curveX1 = (((1-bezierTime3)*(1-bezierTime3)) * transform.position.x) + (2 * bezierTime3 * (1 - bezierTime3) * midPointAtk.transform.position.x) + ((bezierTime3 * bezierTime3) * target.transform.position.x);

			//Calculate the Curve Y position
			float curveY1 = (((1-bezierTime3)*(1-bezierTime3)) * transform.position.y) + (2 * bezierTime3 * (1 - bezierTime3) * midPointAtk.transform.position.y) + ((bezierTime3 * bezierTime3) * target.transform.position.y);

			//Calculate the Curve Y position
			float curveZ1 = (((1-bezierTime3)*(1-bezierTime3)) * transform.position.z) + (2 * bezierTime3 * (1 - bezierTime3) * midPointAtk.transform.position.z) + ((bezierTime3 * bezierTime3) * target.transform.position.z);

			//Increment the time for the Bezier Curve
			bezierTime3 = bezierTime3 + (5 * Time.deltaTime);

			transform.position = new Vector3(curveX1, curveY1, curveZ1);
			
		} else if (dummyTarget != Vector3.zero) {

			if (once1 == false){

				pos = transform.position;
				axis = transform.up;
				rightMov = MainPlayer.mainPlayer.generalMovementControl.movingRight;
				once1 = true;
			}
			if (rightMov){
				pos += transform.right * Time.deltaTime * attackSpeed;
			} else {
				pos -= transform.right * Time.deltaTime * attackSpeed;
			}
				
			transform.position = pos + axis * Mathf.Sin (Time.time * frequency) * magnitude;
			//transform.position = Vector3.MoveTowards (transform.position, dummyTarget, Time.deltaTime * attackSpeed);

		//do the water wave
		} else if (doingWaterWave)
		{
			//Water step 0
			if (waterStep == 0) {

				//Calculate the Curve X position
				float curveX1 = (((1-bezierTime4)*(1-bezierTime4)) * lastWaterWavePos.x) + (2 * bezierTime4 * (1 - bezierTime4) * midPointInst.transform.position.x) + ((bezierTime4 * bezierTime4) * closestWater.transform.position.x);

				//Calculate the Curve Y position
				float curveY1 = (((1-bezierTime4)*(1-bezierTime4)) * lastWaterWavePos.y) + (2 * bezierTime4 * (1 - bezierTime4) * midPointInst.transform.position.y) + ((bezierTime4 * bezierTime4) * (closestWater.transform.position.y - 1f));

				//Calculate the Curve Y position
				float curveZ1 = (((1-bezierTime4)*(1-bezierTime4)) * lastWaterWavePos.z) + (2 * bezierTime4 * (1 - bezierTime4) * midPointInst.transform.position.z) + ((bezierTime4 * bezierTime4) * closestWater.transform.position.z);

				//Increment the time for the Bezier Curve
				bezierTime4 = bezierTime4 + (3 * Time.deltaTime);

				transform.position = new Vector3(curveX1, curveY1, curveZ1);

				//See if the dist between the water is <= 1
				float dist = Vector3.Distance (transform.position, closestWater.transform.position);
				if (dist <= 1) {
					waterStep = 1;
					Destroy (midPointInst.gameObject, 1);
					doingWaterWave = true;

					print("Water Step :" + waterStep);

				}
				
			//water step 1
			}else if (waterStep == 1){
				
				//Change all function to the Water Wave Script
				newWaterWaveInst = (GameObject)Instantiate (newWaterWave, transform.position, Quaternion.identity);
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().moveSpeed = moveSpeed;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().doingWaterWave = doingWaterWave;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().closestWater = closestWater;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().MAX_LIVE_TIME_NORMAL = MAX_LIVE_TIME_NORMAL;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().midPoint = midPoint;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().firstCurvePointiness = firstCurvePointiness;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().attackSpeed = attackSpeed;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().target = target;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().dummyTarget = dummyTarget;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().frequency = frequency;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().magnitude = magnitude;
				newWaterWaveInst.GetComponentInChildren<WaterWaveObject> ().ColdBuff = ColdBuff;
				
				//newWaterwaveInst.GetComponentInChildren<WaterWaveObject> ().ChangeValues(moveSpeed, doingWaterWave, closestWater, MAX_LIVE_TIME_NORMAL, midPoint, firstCurvePointiness, attackSpeed, target, dummyTarget, frequency, magnitude, WaterSplashPref, ColdBuff);
				
				MainPlayer.mainPlayer.GetComponent<WaterAbility> ().waterBolt = null;
				MainPlayer.mainPlayer.characterInfo.waterWave = newWaterWaveInst;
				waterStep = 2;
			
				print("Water Step :" + waterStep);
				
				//Destroy the water bolt
				Destroy (gameObject);

			}
				
		}
  	#endregion
    }
    #endregion

	//Function to Instantiate the Mid Point and Destroy after 5 seconds
	private void InstantiateObj() {

		midPointAtk = (GameObject)Instantiate (midPoint, new Vector2 ((transform.position.x + target.transform.position.x) / 2, (transform.position.y + target.transform.position.y) / 2 + 2), Quaternion.identity);
		Destroy (midPointAtk, 5);
	}

    #region Attack Function

	private IEnumerator WaitForAttack(Transform targetIn, float speed){

		while(canAttack == false){

			yield return new WaitForSeconds (.1f);
		}
			
		once2 = false;
		Attack (targetIn, speed);
	}

    //Attack Function
	public void Attack(Transform targetIn, float speed = ATTACK_SPEED_WALTERBOLT_DEFAULT)
    {
		if (target != null || isCreatingShield || dummyTarget != Vector3.zero || once2) {
			//if attack is called already. Do not run the codes
			return;
		}

		if (canAttack == false)
		{
			once2 = true;
			StartCoroutine (WaitForAttack (targetIn, speed));
			return;
		}

		Rigidbody2D rb = GetComponent <Rigidbody2D> ();
		rb.isKinematic = false;

		//Charge Attack
		if(holdTime == ATTACK_MAX_HOLD_TIME) {
			
			Vector3 tmpPos = Input.mousePosition;
			tmpPos.z = -CameraFollowPlayer.cameraFollowPlayer.transform.position.z;
			rb.AddForce ( (Camera.main.ScreenToWorldPoint (tmpPos)- MainPlayer.mainPlayer.transform.position).normalized * 3000);
			active = false;
			started = true;
			MainPlayer.mainPlayer.GetComponent<WaterAbility> ().waterBolt = null;
			Invoke ("SelfDestroy", MAX_LIVE_TIME_LONG);
			Camera.main.GetComponent <CameraFollowPlayer>().UseInitialView ();
		}
		//Target Attack
		else if (targetIn != null) {
			rb.gravityScale = 0;
			moveSpeed = speed;
			active = false;
			target = targetIn;
			MainPlayer.mainPlayer.GetComponent<WaterAbility> ().waterBolt = null;
			Invoke ("SelfDestroy", MAX_LIVE_TIME_NORMAL);
		//Dummy Target Attack
		}else{
			rb.gravityScale = 0;
			dummyTarget = new Vector3 ((MainPlayer.mainPlayer.generalMovementControl.movingRight ? 99999 : -99999), MainPlayer.mainPlayer.transform.position.y);
			active = false;
			moveSpeed = speed;
			MainPlayer.mainPlayer.GetComponent<WaterAbility> ().waterBolt = null;
			Invoke ("SelfDestroy", MAX_LIVE_TIME_NORMAL);
		} 



	}
    #endregion

	#region Ability Related Stuff
	public void CreateShield(){

		float dis = MainPlayer.mainPlayer.generalMovementControl.movingRight?2:-2;

		shieldPos = new Vector3 (MainPlayer.mainPlayer.transform.position.x + dis, MainPlayer.mainPlayer.transform.position.y, MainPlayer.mainPlayer.transform.position.z);
		//isCreatingShield = true;
		//active = false;
		//attackSpeed = Vector3.Distance (shieldPos,transform.position) / createShieldTime;
		GameObject h = (GameObject)Instantiate (shieldBoltPref, transform.position, Quaternion.identity);
		h.transform.position = new Vector3 (h.transform.position.x, h.transform.position.y, 0.5f);
		h.GetComponent<ShieldBoltObject> ().midPoint = midPoint;
		h.GetComponent<ShieldBoltObject> ().shieldPos = shieldPos;
		MainPlayer.mainPlayer.characterInfo.shieldBoltObject = h;
		Destroy (this.gameObject);
	}


	#endregion



	#region OnCollisionEnter2D Function
	//OnCollisionEnter2D Function
	public void OnCollisionEnter2D(Collision2D col)
	{
		if ((target != null || dummyTarget != Vector3.zero) && col.collider.tag != "Player") {
			Rigidbody2D RB = col.gameObject.GetComponent<Rigidbody2D> ();
			if (RB != null && col.gameObject.transform.position.x < transform.position.x) {
				RB.AddForceAtPosition (new Vector3 (-1, 1) * 100, col.transform.position);

			} else if (RB != null) {
				RB.AddForceAtPosition (new Vector3 (1, 1) * 100, col.transform.position);
			}

			CommonObject commonObject = col.gameObject.GetComponent<CommonObject> ();
			if (commonObject != null) {
				if (isFreeze) {

					Collider2D[] f = Physics2D.OverlapCircleAll (transform.position, 1f);

					foreach (Collider2D colWithinRange in f) {
						CommonObject commonObject1 = colWithinRange.GetComponent <CommonObject> ();
						if (commonObject1 != null) {
							if (commonObject1.TakeDamage (waterBoltDamageFreeze, DamageType.Water, gameObject)) {
								AttachColdBuff (commonObject);
							}
						}
					}

				} else {
					if (commonObject.TakeDamage (waterBoltDamage)) {
						AttachColdBuff (commonObject);
					}
				}
			}


			GenerateWaterSplash (col);
			SelfDisappear ();
			Invoke ("SelfDestroy", MAX_LIVE_TIME_NORMAL);
		} else if (holdTime == ATTACK_MAX_HOLD_TIME) {
			Collider2D[] f = Physics2D.OverlapCircleAll (transform.position, 1f);

			foreach (Collider2D colWithinRange in f) {
				CommonObject commonObject = colWithinRange.GetComponent <CommonObject> ();
				if (commonObject != null) {
					if (commonObject.TakeDamage (isFreeze ? bigWaterBoltDamageFreeze : bigWaterBoltDamage, DamageType.Water, gameObject)) {
						AttachColdBuff (commonObject);
					}
				}
			}

			GenerateWaterSplash (col);
			SelfDisappear ();
			Invoke ("SelfDestroy", MAX_LIVE_TIME_NORMAL);
		}
	}
	#endregion


	 /// <summary>
	 /// Generates the water splash at the point if the collider has a WaterSplashable attached and it is possible to generate a splash
	 /// </summary>
	 /// <returns><c>true</c>, if water splash was generated, <c>false</c> otherwise.</returns>
	public bool GenerateWaterSplash(Collision2D collision){
		GeneralPlatform splashable = collision.gameObject.GetComponent <GeneralPlatform> ();
		if (splashable == null || !splashable.enableWaterSplash)
			return false;


		Vector2 point = collision.contacts [0].point;
		Vector2 normal = collision.contacts [0].normal;
		Quaternion rot = Quaternion.FromToRotation (Vector2.up, normal);


		Vector2 pos;

		if (collision.collider is BoxCollider2D) {
			BoxCollider2D f = collision.collider as BoxCollider2D;
			pos = WaterProjectUtils.GetClosestPointOnBound2D (f, point);
		} else if (collision.collider is PolygonCollider2D) {
			PolygonCollider2D f = collision.collider as PolygonCollider2D;
			pos = WaterProjectUtils.GetClosestPointOnBound2D (f, point);
		} else {
			pos = point;
		}

		Vector2 tmp = WaterProjectUtils.ClosestPointOnLine2D (Vector2.up, Vector2.down, Vector2.right);
		Vector3 pos3d = pos;
		pos3d.z = collision.collider.transform.position.z - 0.01f;
		GameObject.Instantiate (WaterSpashPref,pos3d,rot);

		return true;
	}







	/// <summary>
	/// Attachs the cold buff.
	/// </summary>
	/// <param name="obj">Object.</param>
	private void AttachColdBuff(CommonObject obj){
		GameObject buff = GameObject.Instantiate (ColdBuff);
		buff.GetComponent <GenericBuff> ().AttachToParent (obj);
	}


	public void IncreasePower(float deltaTime){
		if (active) {
			holdTime += deltaTime;
			if (holdTime >= ATTACK_MAX_HOLD_TIME) {
				holdTime = ATTACK_MAX_HOLD_TIME;
				transform.localScale = Vector3.one * (1 + ATTACK_MAX_HOLD_TIME);
				Camera.main.GetComponent <CameraFollowPlayer> ().UseFarView ();
			} else {
				anim.SetFloat ("Blend", holdTime);
			}
		}

	}

	#region Override funtions

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){

		if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Special && !isFreeze) {
			Freeze ();
			return true;
		} else if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Attack) {
			if (direction == InteractDirectionKey.Up){

				//Play Animation Water Rope Up
				//Sends comnand to MainPLayer Script to do Raycast
				//RaycastAll to see if it hits anything
				//MainPlayer Scripts Sends a return value to this script (true or false)
				//If it doesnt hit anything, play Animation Water Rope Destroy to destroy Water Rope
				//If it does, instantiate the rope





				RaycastHit2D[] hit = Physics2D.CircleCastAll (MainPlayer.mainPlayer.transform.position, getWaterRange, new Vector2(1, 1));

				GameObject closestObj = null;
				float closestDist = getWaterRange;

				for(int i=0; i < hit.Length; i++ ) {


					if (hit[i].transform.tag == "FluidWater") {

						bool freeze = hit [i].transform.gameObject.GetComponent<FreezableWater> ().isFrozen;

						if (freeze == false) {
							if (hit [i].distance < closestDist) {

								closestObj = hit [i].transform.gameObject;
								closestDist = hit [i].distance;
							}
						}
					}
				}

				if (closestObj != null){

					doingWaterWave = true;
					lastWaterWavePos = transform.position;
					target = null;
					closestWater = closestObj;
					dummyTarget = Vector3.zero;
					started = true;
					once2 = true;

					midPointInst = (GameObject)Instantiate (midPoint, new Vector2 ((transform.position.x + closestWater.transform.position.x) / 2, (transform.position.y + closestWater.transform.position.y) / 2 + firstCurvePointiness), Quaternion.identity);


					//WaterWave ();
				}
			}
			else if (direction == InteractDirectionKey.Down && MainPlayer.mainPlayer.generalMovementControl.isOnGround)
				
				CreateShield ();
			else
				Attack (MainPlayer.mainPlayer.GetClosestTarget ());
			return true;
		}
		return false;
	}

	#endregion

	public void Freeze(){

		isFreeze = true;
		GetComponentInChildren <SpriteRenderer> ().color = Color.blue;
		originalColor  = Color.blue;
	}

	void SelfDisappear(){

		waterBoltVisual.SetActive (false);
		started = true;
		target = null;
		dummyTarget = Vector3.zero;
		gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeAll;
		gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;
		gameObject.GetComponent<CircleCollider2D> ().enabled = false;
	}

	void SelfDestroy(){
		SelfDisappear ();
		Destroy (gameObject, 1);
	}


	public override bool IsHighlightable()
	{
		return !isFreeze;
	}


	public override void Highlight(bool highlight){

		if (highlight) {
			GetComponentInChildren <SpriteRenderer> ().color = Color.cyan;
		} else {
			GetComponentInChildren <SpriteRenderer> ().color = originalColor;
		}
	}
}
