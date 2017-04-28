using UnityEngine;
using System.Collections;

public class FireAbility : GenericAbility {

	public GameObject fireBoltPref;
	public GameObject flameThrowerPref;
	public GameObject thumbFlamePref;
	public GameObject groundBurstPref;

	private float maxHoldTime;
	public float FireBoltCooldownTime;
	public float GroundBurstCooldownTime;

	private float holdTime = 0f;
	private float cooldownTimer = 0f;
	public float moveSpeed = 5;
	public float particleStartSize = 1f;

	private bool holdAttack = false;
	float holdtime = 0f;
	public float MAXHOLDTIME = 3f; //this is the variable to change for the particle system !!
	GameObject fireBolt = null;
	GameObject point4;
	GameObject point5;
	bool hasFire = false;


	private GameObject flameThrower = null;
	public GameObject thumbFlame = null;
	private bool isHurlingDown = false;

	private GeneralMovementControl moveControl;

	void Awake(){

		moveControl = GetComponent <GeneralMovementControl> ();
		if (moveControl == null) {
			Debug.LogError ("Player needs a GeneralMovementControl script attached to use FireAbility script!");
		}

		point4 = GameObject.Find ("Point4");
		point5 = GameObject.Find ("Point5");
	}
	
	// Update is called once per frame
	void Update () {
		if (flameThrower != null) {
			//update the direction of the flamethrower

			Vector3 tmpDir = (Camera.main.ScreenToWorldPoint (Input.mousePosition) - MainPlayer.mainPlayer.transform.position).normalized;
			tmpDir.z = 0;
			Quaternion tmp = Quaternion.FromToRotation (Vector2.right,tmpDir);
			flameThrower.transform.rotation = tmp;

		}
		
		//Cooldown Timer Reduce Value
		if (cooldownTimer >= 0) {
			cooldownTimer -= Time.deltaTime;
		} else {
			cooldownTimer = 0;
		}
		
		//Hold Attack Stuff
		if (holdAttack) {
			
			//If the player is pressing Attack btn will check if has fire and if so it will do the animation stuff and increase size
			if (Input.GetButton ("Attack")) {

				if (hasFire) {

					if (MainPlayer.mainPlayer.generalMovementControl.movingRight) {

						fireBolt.transform.position = Vector3.Lerp (fireBolt.transform.position, point4.transform.position, moveSpeed * Time.deltaTime);
					} else {

						fireBolt.transform.position = Vector3.Lerp (fireBolt.transform.position, point5.transform.position, moveSpeed * Time.deltaTime);
					}

					//fireBolt.transform.localScale = new Vector2 (1 + holdtime, 1 + holdtime);

					if (holdtime < MAXHOLDTIME) {
						fireBolt.GetComponentInChildren<ParticleSystem> ().startSize = particleStartSize + holdtime;
						holdtime += Time.deltaTime;
						print ("Hold Time = " + holdtime);
					}
				} else {

					fireBolt = (GameObject)Instantiate (fireBoltPref, MainPlayer.mainPlayer.transform.position, Quaternion.identity);
					fireBolt.GetComponent<FireBolt> ().isHoldBolt = true;
					hasFire = true;
				}
			
			//Player stopped clicking Attack btn
			} else {
				
				if (fireBolt != null)
					fireBolt.GetComponent <FireBolt> ().Attack (MainPlayer.mainPlayer.GetClosestTarget ());
				
				//Reset values
				cooldownTimer = FireBoltCooldownTime;
				holdtime = 0f;
				fireBolt = null;
				hasFire = false;
				holdAttack = false;
			}
		}
	}



	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, CommonObject.InteractKey interactKey, CommonObject.InteractDirectionKey direction = CommonObject.InteractDirectionKey.None){

		if (flameThrower != null) {
			Destroy (flameThrower);
			return true;
		}


		if (attackMode == CharacterInfo.ATTACK_MODE.FIRE && interactKey == CommonObject.InteractKey.Attack && cooldownTimer <=0 && !holdAttack) {;
			if (direction == CommonObject.InteractDirectionKey.None && thumbFlame == null) {

				GameObject fireBolt = (GameObject)Instantiate (fireBoltPref, MainPlayer.mainPlayer.transform.position, Quaternion.identity);
				fireBolt.GetComponent <FireBolt> ().Attack (MainPlayer.mainPlayer.GetClosestTarget ());
				cooldownTimer = FireBoltCooldownTime;
				holdTime = 0;
				return true;
			} else if (direction == CommonObject.InteractDirectionKey.Up) {
				ThumbFlameToggle ();
				holdTime = 0;
				return true;
			} else if (direction == CommonObject.InteractDirectionKey.Down && thumbFlame == null) {
				if (MainPlayer.mainPlayer.generalMovementControl.isOnGround)
					GroundFireBurst (MainPlayer.mainPlayer.generalMovementControl.groundDir);
				else if (!MainPlayer.mainPlayer.generalMovementControl.isOnGround && !MainPlayer.mainPlayer.isInWater
					&& !MainPlayer.mainPlayer.generalMovementControl.onWall && !MainPlayer.mainPlayer.characterInfo.isOnWaterSpinUp && flameThrower == null)
					HurlDown ();
				holdTime = 0;
				return true;
			}

		} else if (attackMode == CharacterInfo.ATTACK_MODE.FIRE && interactKey == CommonObject.InteractKey.Special && thumbFlame == null) {
			MainPlayer.mainPlayer.interactiveCollider.Interact (attackMode, interactKey, direction,true);
		}

		return false;
	}



	public override bool Hold(){
		//If there is no flameThrower, increase the holdtime
		if (flameThrower == null)
			holdTime += Time.deltaTime;

		//Choose to do ThrowFlame or HoldAttack
		if (holdTime >= maxHoldTime) {
			holdTime = 0;
			if (thumbFlame != null) {
				ThrowFlame ();
			} else {

				holdAttack = true;
			}
		}

		return true;
	}
		



	void OnCollisionEnter2D(Collision2D col){
		GeneralMovementControl moveControl = GetComponent <GeneralMovementControl> ();
		if (isHurlingDown && moveControl != null) {
			if (!moveControl.isOnGround) {
				Debug.LogError ("The GeneralMovementControl script needs be to before the FireAbility in the inspector!");
			} else if(!MainPlayer.mainPlayer.isInWater) {
				GroundFireBurst (col.transform.localToWorldMatrix.MultiplyVector (Vector3.right).normalized);

			}
		}
		isHurlingDown = false;
	}
		

	private void ThrowFlame(){
		if (thumbFlame != null) {
			flameThrower = (GameObject)Instantiate (flameThrowerPref, MainPlayer.mainPlayer.transform.position, Quaternion.identity);
			flameThrower.transform.SetParent (this.transform);
			Quaternion tmp = flameThrower.transform.localRotation;
			tmp.eulerAngles = new Vector3 (0, MainPlayer.mainPlayer.generalMovementControl.movingRight ? 0 : 180, 0);
			flameThrower.transform.localRotation = tmp;
		}
	}

	private void ThumbFlameToggle(){
		
		if (thumbFlame != null) {
			Destroy (thumbFlame);
		} else {

			thumbFlame = (GameObject)Instantiate (thumbFlamePref, MainPlayer.mainPlayer.transform.position, Quaternion.identity);
			thumbFlame.transform.SetParent(this.transform);
			thumbFlame.transform.localPosition = new Vector2 (0.5f, 0.5f);
		}
		
	}


	private void GroundFireBurst(Vector3 objDir){
		
		GameObject groundBurst = (GameObject)Instantiate (groundBurstPref, MainPlayer.mainPlayer.transform.position + Vector3.down * MainPlayer.mainPlayer.transform.lossyScale.y, 
			Quaternion.FromToRotation (Vector3.right, objDir));
		GetComponent <Rigidbody2D>().velocity = Vector2.zero;
		cooldownTimer = GroundBurstCooldownTime;
	}

	private void HurlDown(){
		isHurlingDown = true;
		GetComponent <Rigidbody2D> ().velocity = Vector2.down * 10;
	}


}
