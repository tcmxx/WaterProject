using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemyAI : MonoBehaviour, IRangeCheckReceiver {

	//To use this script make sure you have a trigger collider to detect the player!

	bool canAttack = true;
	public float attackDist = 1;
	public LayerMask mask;
	GameObject player;

	IGeneralMovement generalMovementControl;
	EnemyBase		enemyBase;

	protected  virtual void Awake(){
		generalMovementControl = GetComponent <IGeneralMovement> ();
		enemyBase = GetComponent <EnemyBase> ();
	}

	protected virtual void Update () {

		if (player == null)
			return;

		if (GetDistFromTarget (player) <= attackDist && canAttack) 
		{
			Attack ();
		} else 
		{
			MoveToTarget (player);
		}
	}

	float GetDistFromTarget(GameObject target) 
	{
		return (float)Vector2.Distance (this.gameObject.transform.position, target.gameObject.transform.position);
	}



	public virtual void OnRangeEnter (Collider2D col){
		if (col.gameObject.tag == "Player") {
			player = col.gameObject;
		}
	}
	public virtual void OnRangeStay (Collider2D col){
	}
	public virtual void OnRangeExit (Collider2D col){
		if (col.gameObject.tag == "Player") {
			player = null;
		}
	}
		
	void Attack()
	{
		//Attack Animation
		//Player Takes Damage
		if(enemyBase != null)
			enemyBase.Attack (player.transform.position,player.GetComponent <CommonObject>());
		canAttack = false;
		StartCoroutine (AttackCooldown ());
	}

	IEnumerator AttackCooldown()
	{
		
		yield return new WaitForSeconds (1);
		canAttack = true;
	}

	void MoveToTarget(GameObject target) 
	{
		//Something here

		if (target.transform.position.x > this.transform.position.x)
			MoveRight ();
		else
			MoveLeft ();
			

	}

	void MoveLeft()
	{
		generalMovementControl.Move (-1);
		RaycastHit2D hit = Physics2D.Raycast (this.transform.position, Vector2.left, 1, mask);
		if (hit.collider != null)
			generalMovementControl.Jump ();
	}

	void MoveRight()
	{
		generalMovementControl.Move (1);
		RaycastHit2D hit = Physics2D.Raycast (this.transform.position, Vector2.right, 1, mask);
		if (hit.collider != null) 
			generalMovementControl.Jump ();
	}

}
