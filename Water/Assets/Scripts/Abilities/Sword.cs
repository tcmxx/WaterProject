using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sword : CommonObject {


	public GameObject ColdBuff;
	public  new MeshRenderer renderer;
	public GameObject burningBuffPref;
	public static int SwordDamage = 10;
	public static int SwordFireDamage = 14;

	private bool isAttacking = false;
	public bool IsAttacking{get{ return isAttacking;}}
	private Animator playerAnimator;
	static int attackState = Animator.StringToHash ("Base Layer.AttackSword");
	private List<CommonObject> collidedObjects = new List<CommonObject> ();


	private CharacterInfo.ATTACK_MODE curAttackMode = CharacterInfo.ATTACK_MODE.NORMAL;

	// Use this for initialization
	void Start () {
		playerAnimator = MainPlayer.mainPlayer.anim;
		SwitchMode (CharacterInfo.ATTACK_MODE.NORMAL);

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){

		if (attackMode == CharacterInfo.ATTACK_MODE.NORMAL && interactKey == InteractKey.Attack ) {
			if (direction == InteractDirectionKey.None) {
				if (!isAttacking) {
					Attack ();
				}
			} else if (direction == InteractDirectionKey.Down) {
				//turn of water sword if it is on or turn on water sword otherwise
				if (curAttackMode != CharacterInfo.ATTACK_MODE.WATER) {
					SwitchMode (CharacterInfo.ATTACK_MODE.WATER);
				} else {
					SwitchMode (CharacterInfo.ATTACK_MODE.NORMAL);
				}
				
			} else if (direction == InteractDirectionKey.Up) {
				//turn of fire sword if it is on or turn on fire sword otherwise
				if (curAttackMode != CharacterInfo.ATTACK_MODE.FIRE) {
					SwitchMode (CharacterInfo.ATTACK_MODE.FIRE);
				} else {
					SwitchMode (CharacterInfo.ATTACK_MODE.NORMAL);
				}
			}
			if (playerAnimator.GetCurrentAnimatorStateInfo (0).fullPathHash != attackState && playerAnimator.IsInTransition(0) == false)
			{
				MainPlayer.mainPlayer.characterInfo.energy -= 20;
			}
			return true;
		} 
		return false;
	}


	private void Attack(){

		for(int i = 0; i < collidedObjects.Count; i++) {
			CommonObject obj = collidedObjects [i];
			if (obj == null) {
				collidedObjects.Remove (obj);
			} else {
				AttackObject (obj);
			}
		}

		StartCoroutine (Attacking ());
	}

	IEnumerator Attacking(){





		isAttacking = true;
		playerAnimator.SetFloat ("AttackNumber", (float)Random.Range (0, 3));
		playerAnimator.SetTrigger ("Attack");

		//CharacterInfo.characterInfo.isAnimating = true;
		yield return new WaitForSeconds(0.6f);
		isAttacking = false;
		//CharacterInfo.characterInfo.isAnimating = false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{


		CommonObject obj = col.GetComponent <CommonObject> ();

		if (obj!= null  && col.tag != "Player") {

			//add the object to the list
			if(!collidedObjects.Contains (obj)){
				collidedObjects.Add (obj);
			}

			if (isAttacking) {
				AttackObject (obj);
			}

		}

	}




	void OnTriggerExit2D(Collider2D col){
		CommonObject obj = col.GetComponent <CommonObject> ();

		if (obj != null  && col.tag != "Player") {

			//remove the object in the list
			if (collidedObjects.Contains (obj)) {
				collidedObjects.Remove (obj);
			}
		}
	}


	void SwitchMode(CharacterInfo.ATTACK_MODE attackMode){
		switch (attackMode) {
		case CharacterInfo.ATTACK_MODE.FIRE:
			renderer.material.color = Color.red;
			break;
		case CharacterInfo.ATTACK_MODE.NORMAL:
			renderer.material.color = Color.black;
			break;
		case CharacterInfo.ATTACK_MODE.WATER:
			renderer.material.color = Color.blue;
			break;
		default:
			break;
		}
		curAttackMode = attackMode;
	}


	private void AttackObject(CommonObject obj){
		if (obj == null)
			return;

		Rigidbody2D RB = obj.GetComponent<Rigidbody2D> ();
		if (RB != null && obj.gameObject.transform.position.x < transform.position.x) {
			RB.AddForceAtPosition (new Vector2 (-1, 1) * 100, obj.transform.position);

		} else if (RB != null) {
			RB.AddForceAtPosition (new Vector2 (1, 1) * 100, obj.transform.position);
		}



		switch (curAttackMode) {
		case CharacterInfo.ATTACK_MODE.FIRE:
			obj.TakeDamage (SwordFireDamage);
			GameObject buffBurning = GameObject.Instantiate (burningBuffPref);
			buffBurning.GetComponent <GenericBuff>().AttachToParent (obj);
			break;
		case CharacterInfo.ATTACK_MODE.NORMAL:
			obj.TakeDamage (SwordDamage);
			break;
		case CharacterInfo.ATTACK_MODE.WATER:
			obj.TakeDamage (SwordDamage);
			GameObject buffCold = GameObject.Instantiate (ColdBuff);
			buffCold.GetComponent <GenericBuff>().AttachToParent (obj);
			break;
		default:
			break;
		}
	}
}
