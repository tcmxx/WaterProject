using UnityEngine;
using System.Collections;
using System;
using System.Net.Mail;




[RequireComponent(typeof(WaterAbility))]
[RequireComponent(typeof(NormalAbility))]
[RequireComponent(typeof(FireAbility))]
[RequireComponent(typeof(GeneralMovementControl))]
[RequireComponent(typeof(CharacterInfo))]
public class MainPlayer : CommonObject {

    #region Variables
    
	public int allowedJumpTimes = 0;
	private int maxAllowedJumpTimes = 2;
	private bool canRoll = true;



	private GameObject[] timebar;
	private GameObject[] energybar;



    public Animator anim;

	[HideInInspector]
	public bool doRunAnim = true;


	public GameObject playerModel;
	public GameObject waterSpinUpJump;
	public GameObject fireJumpObject;

	[SerializeField] private float GODMODE_TIME = 1f;
	public GameObject teleporterPref;
    public float ATTACK_RANGE = 10f;

	[HideInInspector]
	public GenericAbility waterAbility;
	[HideInInspector]
	public GenericAbility fireAbility;
	[HideInInspector]
	public GenericAbility normalAbility;

	private GenericAbility curAbility = null;
	public GenericAbility CurAbility { get { return curAbility; } }

	[HideInInspector]
	public GeneralMovementControl generalMovementControl;
	//self reference
    public static MainPlayer mainPlayer;



    //other reference
	[HideInInspector]
    public InteractiveCollider interactiveCollider;
	public ActionObject curActionObject{ get; set;}
	[HideInInspector]
	public AttachableObject attachableObject = null;

	public bool canAttach = true; //Variable to check if the player can attach to the rope

	public const float ROLLING_TIME = 0.3f;

	public CharacterInfo characterInfo;


    #endregion

    #region Callbacks

	void Awake(){
		mainPlayer = this;
		interactiveCollider = GetComponentInChildren<InteractiveCollider>();
		generalMovementControl = GetComponent <GeneralMovementControl> ();
		waterAbility = GetComponent <WaterAbility> ();
		fireAbility = GetComponent <FireAbility> ();
		normalAbility = GetComponent <NormalAbility> ();
		characterInfo = GetComponent <CharacterInfo> ();



		timebar = GameObject.FindGameObjectsWithTag ("timebar");
		for(int i = 0; i < timebar.Length; i++){
			timebar [i].SetActive (false);
		}
		energybar = GameObject.FindGameObjectsWithTag ("energybar");
		for(int i = 0; i < energybar.Length; i++){
			energybar [i].SetActive (true);
		}
	}

    // Use this for initialization
    void Start()
    {
        
		SetMode (CharacterInfo.ATTACK_MODE.NORMAL);

    }

    // Update is called once per frame
    void Update()
    {

		if (attachableObject != null) {
			attachableObject.DoUpdate (this);
		}

        healthUpdate();
    }




    #endregion

    #region Movement methods

    //input argument: start: if true, means the jump button is pushed/releases the first time.
    //                push: if true, it means the button is still pushed, otherwise it means it is not pushed
    public void Jump(bool start, bool push)
    {
		if (characterInfo.isAnimating)
			return;

		//interact the attachable first if there is a attachable attached
		if (attachableObject != null) {
			attachableObject.Jump (start, push,this);
			return;
		}


        //Jump
        if (start && push && characterInfo.curJumpSpeed == 0f)
        {
			if (characterInfo.isOnWaterSpinUp)
			{
				// Something here
			}else if (generalMovementControl.onWall)
			{
				generalMovementControl.WallJump ();
			}
			else if (allowedJumpTimes > 0 || isInWater)
            {
                Rigidbody2D RB = GetComponent<Rigidbody2D>();
				characterInfo.curJumpSpeed = CharacterInfo.JUMP_SPEED_MIN + (RB.velocity.y>0?RB.velocity.y:0);
				RB.velocity = new Vector2(RB.velocity.x, characterInfo.curJumpSpeed);
                
                anim.SetTrigger("Jump");
				anim.SetBool ("isSwimming", false);

				if (isInWater) {
					allowedJumpTimes = 1;
				} else {
					allowedJumpTimes--;
				}
			}
			
			//Special Jump on Water mode , works only if you have 0 jumps remaining and you are above water
			//Max distance of 4
			else if (allowedJumpTimes == 0 && characterInfo.attackMode == CharacterInfo.ATTACK_MODE.WATER){
				
				//Raycast to see if it hits the water
				RaycastHit2D[] hit = Physics2D.RaycastAll(transform.position, Vector2.down, 4);
				
				//Check all objects from the raycast
				for(int i = 0; i < hit.Length; i++){
					
					//If one of those objects have the tag "FluidWater"
					if (hit[i].collider.tag == "FluidWater"){
					
						//Instantiate Animation
						GameObject waterJump = (GameObject) Instantiate (waterSpinUpJump, new Vector2 (this.gameObject.transform.position.x, hit[i].collider.gameObject.transform.position.y), Quaternion.identity);
						
						//Do the jumping
						//This is temporary , will change this to the animation stuff
						SpecialWaterJump ();
					}
				}
			
			//Special Jump on Fire mode , works only if you have 0 jumps remaining and you are in fire mode
			} else if(allowedJumpTimes == 0 && characterInfo.attackMode == CharacterInfo.ATTACK_MODE.FIRE){
			
				//Do Special Jump on Fire mode
				 
				//Instantiate Animation
				GameObject fireJump = (GameObject) Instantiate (fireJumpObject, new Vector2 (this.gameObject.transform.position.x, this.gameObject.transform.position.y -1), Quaternion.identity);
				
				//Do the jumping
				SpecialFireJump ();
			}
        }
        else if(!start && push && characterInfo.curJumpSpeed > 0f && characterInfo.curJumpSpeed < CharacterInfo.JUMP_SPEED_MAX)
        {
            characterInfo.curJumpSpeed = characterInfo.curJumpSpeed + CharacterInfo.JUMP_SPEED_RATE * Time.deltaTime;
            
            Rigidbody2D RB = GetComponent<Rigidbody2D>();
            RB.velocity = new Vector2(RB.velocity.x, characterInfo.curJumpSpeed);
        }
        else
        {
            characterInfo.curJumpSpeed = 0f; 
        }

        

    }

	//Function for the Third Fire Jump
	public void SpecialFireJump(){

		Rigidbody2D RB = GetComponent<Rigidbody2D>();
		characterInfo.curJumpSpeed = CharacterInfo.JUMP_SPEED_MIN + (RB.velocity.y>0?RB.velocity.y:0);
		RB.velocity = new Vector2(RB.velocity.x, characterInfo.curJumpSpeed);

		anim.SetTrigger("Jump");

		allowedJumpTimes--;
	}
	
	//Function for the Third Water Jump
	public void SpecialWaterJump(){

		Rigidbody2D RB = GetComponent<Rigidbody2D>();
		characterInfo.curJumpSpeed = CharacterInfo.JUMP_SPEED_MIN + (RB.velocity.y>0?RB.velocity.y:0);
		RB.velocity = new Vector2(RB.velocity.x, characterInfo.curJumpSpeed);

		anim.SetTrigger("Jump");

	}

    public void Move(float horizontal)
	{

		if (characterInfo.isAnimating)
			return;

		//interact the attachable first if there is an attachable attached
		if (attachableObject != null) {
			attachableObject.Move (horizontal,this);
			generalMovementControl.Move (0);
			return;
		}
			




		if (generalMovementControl.isOnGround) {
			//GetComponent<Rigidbody2D> ().velocity = tmp;

			if (doRunAnim) {
				//Walking Animation
				if (Input.GetButton ("Horizontal")) {
					anim.SetBool ("Running", true);
				} else {
					anim.SetBool ("Running", false);
				}
			} else {
				anim.SetBool ("Running", false);
			}
		} else if (generalMovementControl.onWall) {
			if ((horizontal < 0 && generalMovementControl.movingRight) || (horizontal > 0 && !generalMovementControl.movingRight)) {
				LeaveWall ();
			}
		}

        if (horizontal < 0)
        {
			if (generalMovementControl.movingRight == true)
            {
				TurnAround(false);

            }
        }
        else if (horizontal > 0)
        {
			if (generalMovementControl.movingRight == false)
            {
				TurnAround(true);
            }
        }


		int dir;
		if (horizontal > 0) {
			dir = 1;
		} else if (horizontal < 0) {
			dir = -1;
		} else {
			dir = 0;
		}
		generalMovementControl.Move (dir);

    }
    public void TurnAround(bool toRight)
    {
        
		generalMovementControl.movingRight = toRight;
		if (generalMovementControl.movingRight)
        {
			Transform modelTransform = playerModel.transform;
            modelTransform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
			Transform modelTransform = playerModel.transform;
            modelTransform.localRotation = Quaternion.Euler(0, 180, 0);
        }

    }
	
	//Roll function
	public void Roll(bool right){
		if (!characterInfo.isAnimating && anim.GetBool ("isGrounded") && canRoll) {
			Debug.Log ("Rolling towards " + (right ? "Right" : "Left"));
			StartCoroutine (RollCoroutine (right));
			StartCoroutine (EnableRoll());
		}
	}

	private IEnumerator RollCoroutine(bool right){
		characterInfo.isAnimating = true;
		generalMovementControl.active = false;
		GetComponent <Rigidbody2D> ().AddForce (generalMovementControl.groundDir * (right ? 1 : -1) * 5000);
		yield return new WaitForSeconds(ROLLING_TIME);
		characterInfo.isAnimating = false;
		generalMovementControl.active = true;
	}

	private IEnumerator EnableRoll(){
		canRoll = false;
		yield return new WaitForSeconds(1);
		canRoll = true;
	}


	public void OnGround()
    {

        anim.SetBool("isGrounded", true);
		allowedJumpTimes = maxAllowedJumpTimes;
    }
    public void LeaveGround()
    {
        anim.SetBool("isGrounded", false);
    }


    public void OnWall()
    {
		characterInfo.curJumpSpeed = 0;
		TurnAround (generalMovementControl.movingRight);
        anim.SetBool("isOnWall", true);
    }
    public void LeaveWall()
    {
        anim.SetBool("isOnWall", false);
		TurnAround (generalMovementControl.movingRight);
		allowedJumpTimes = 0;
    }
    #endregion

    



	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){

		//interact the attachable first if there is a attachable attached
		if (attachableObject != null) {
			attachableObject.Interact (this,attackMode,interactKey,direction);
			return true;
		}


		if (interactKey == InteractKey.Attack) {
			
			curAbility.GetMainPlayer (this);
			return curAbility.Interact (attackMode, interactKey, direction);
		} else if (interactKey == InteractKey.Special) {
			curAbility.GetMainPlayer (this);
			return curAbility.Interact (attackMode, interactKey, direction);
		} else if (interactKey == InteractKey.Action) {
			if (curActionObject != null) {
				curActionObject.Action ();
				return true;
			} else {
				return false;
			}
		}
		return false;
	}
		
	/// <summary>
	/// Attach the specified attachable to the player. Return whether it is sucessfully attached
	/// </summary>
	/// <param name="attachable">Attachable.</param>
	public bool Attach(AttachableObject attachable){
		if (attachableObject == null) {
			attachableObject = attachable;
			return true;
		}
		return false;
	}
	/// <summary>
	/// Detach the specified attachable.Return whether it is sucessfully detached
	/// </summary>
	/// <param name="attachable">Attachable.</param>
	public bool Detach(AttachableObject attachable){
		if (attachableObject == attachable) {
			attachableObject = null;
			return true;
		}
		return false;
	}




	#region Health Medhods
	
	//Take Damage Function
	public override bool TakeDamage(int damage, DamageType damageType = DamageType.Normal, GameObject damageFromObject = null) {
		
		if (characterInfo.isGodMode == false) 
		{
			//take damage
			if (damageType == DamageType.Falling) {
				anim.SetTrigger("Roll");
			}
			characterInfo.isHealthRegen = false;
			characterInfo.playerHealth -= damage;
			if (characterInfo.playerHealth <= 0) {
				characterInfo.isHealthRegen = false;
				die ();
			} else {
				characterInfo.isGodMode = true;
				//activate god mode
				StartCoroutine (IsGodMode ());
			}
			return true;
		}



		return false;
	}

    IEnumerator IsGodMode()
    {


        float tempTime = 0f;
        while(tempTime <= GODMODE_TIME){
            this.GetComponentInChildren<SkinnedMeshRenderer>().enabled = !this.GetComponentInChildren<SkinnedMeshRenderer>().enabled;
            yield return new WaitForSeconds(0.1f);
            tempTime += 0.1f;
        }
        this.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
		//deactivate god mode
		characterInfo.isGodMode = false;
		characterInfo.isHealthRegen = true;
    }

    public void die()
    {
        gameObject.SetActive(false);
		Invoke ("revive", 5f);
    }

    public void revive()
    {
		print ("Called Revive"); 
        gameObject.SetActive(true);
		characterInfo.isHealthRegen = true;
		characterInfo.playerHealth = 2.9f;
    }

	//Regen HP
    private void healthUpdate()
    {
		if (characterInfo.playerHealth < characterInfo.MAX_HEALTH)
        {



			if (characterInfo.isHealthRegen)
            {
				
				//Regen healthRegenSpeed per second 
				characterInfo.playerHealth = characterInfo.playerHealth + (characterInfo.healthRegenSpeed * Time.deltaTime);
            }

        }
    }
	#endregion

	#region Teleporter Methods
	
	//Create Teleporter function, the input key will lead to here 
	public void CreateTeleporter() {
		
		//If the player is on ground and the teleport energy bar is full
		if (generalMovementControl.isOnGround && TpVisualBar.tpVisualBar.slider.value == TpVisualBar.tpVisualBar.slider.maxValue) {
			
			//Start timer on the Create TP Timer
			CreateTpTimer.createTpTimer.StartTimer ();
		}
	} 

	//CreateTpTimer will lead to here
	public void CreateTeleport(){
		
		//if the teleporter is not null, destroy teleporter and instantiate a new one
		//else instantiate the teleporter
		if (characterInfo.telePorter != null) {

			Destroy (characterInfo.telePorter);
			GameObject h = (GameObject)Instantiate (teleporterPref, this.transform.position, Quaternion.identity);
			characterInfo.telePorter = h;
		} else{

			GameObject h = (GameObject)Instantiate (teleporterPref, this.transform.position, Quaternion.identity);
			characterInfo.telePorter = h;
		}
		
		//after instantiating the teleporter 
		//Teleporter energy bar will be empty
		TpVisualBar.tpVisualBar.slider.value = 0;
	}

	//Teleport to Teleporter Function
	//TpToTeleporter Key will lead to this function
	public void TpToTeleporter() {
		
		//Check if teleporter is null, if it is null then return
		if(characterInfo.telePorter == null){ return;}
		
		//If attachable Object is different than null return
		if (attachableObject != null) {return;}
		
		//calculate distance between player and teleporter
		float distance = Vector2.Distance (this.gameObject.transform.position, characterInfo.telePorter.transform.position);
		
		//If distance between player and teleporter is more than 100, dont teleport
		if (distance <= 100) {
			
			//Start TpTimer Function
			TpTimer.tpTimer.StartTimer ();
		} else {

			print ("Teleporter out of range");
		}
	}

	//TpTimer Function will lead to here (call this function)
	public void Teleport() {
		
		//Teleport the player to the teleporter position
		transform.position = new Vector2 (characterInfo.telePorter.transform.position.x, characterInfo.telePorter.transform.position.y + 1);
	}
	#endregion

	#region Get closest target

	//Get The Closest Target in range
	public Transform GetClosestTarget()
	{

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll((Vector2.up * transform.position.y + Vector2.right * transform.position.x), ATTACK_RANGE);

        Transform tMin = null;
		float minDist = 999;
		Vector3 currentPos = transform.position;

        //Foreach target,check the position and if it is targetable
        foreach (Collider2D t in hitColliders)
        {
            CommonObject tmpObject = t.GetComponent<CommonObject>();
            //if the target is targetable, calculate the distance and find the closest one
			if (tmpObject != null && (tmpObject.objectProperty & CommonObjectProperty.Targetable) != 0)
            {
                float dist = Vector3.Distance(t.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t.transform;
                    minDist = dist;
                }
            }
		}
		return tMin;
	}
	#endregion

	#region Switch Mode
	public void SwitchMode(){
		if (characterInfo.attackMode == CharacterInfo.ATTACK_MODE.NORMAL) {
			for(int i = 0; i < energybar.Length; i++){
				energybar [i].SetActive (false);
			}
			SetMode (CharacterInfo.ATTACK_MODE.WATER);

		} else if (characterInfo.attackMode == CharacterInfo.ATTACK_MODE.WATER) {
			for(int i = 0; i < timebar.Length; i++){
				timebar [i].SetActive (true);
			}
			for(int i = 0; i < energybar.Length; i++){
				energybar [i].SetActive (true);
			}
			SetMode (CharacterInfo.ATTACK_MODE.FIRE);

		} else if (characterInfo.attackMode == CharacterInfo.ATTACK_MODE.FIRE) {
			for(int i = 0; i < timebar.Length; i++){
				timebar [i].SetActive (false);
			}
			FireAbility fireAbility = GetComponent<FireAbility> ();
			Destroy (fireAbility.thumbFlame);
			SetMode (CharacterInfo.ATTACK_MODE.NORMAL);

		}

		interactiveCollider.UpdateObjects ();
	}

	public void SetMode(CharacterInfo.ATTACK_MODE attackMode){
		if (curAbility != null) {
			curAbility.DisableMode ();
		}
		characterInfo.attackMode = attackMode;
		UILogic.uiLogic.SwitchMode (attackMode);
		interactiveCollider.UpdateObjects ();

		switch (attackMode) {
		case CharacterInfo.ATTACK_MODE.NORMAL:
			curAbility = normalAbility;
			break;
		case CharacterInfo.ATTACK_MODE.WATER:
			curAbility = waterAbility;
			break;
		case CharacterInfo.ATTACK_MODE.FIRE:
			curAbility = fireAbility;
			break;
		default:
			curAbility = normalAbility;
			break;
		}
		curAbility.EnableMode ();
	}
	#endregion
}
