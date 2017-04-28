using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHook : MonoBehaviour {

	public float impulseForceUp = 100;
	public float impulseForceSide = 100;
	public float impulseForceSideUp = 10;
	private bool onWallHook = false;
	private GameObject player;
	private bool canHook = true;
	private bool canJump = true;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame

	void Update () {
		if (onWallHook){

			if (Input.GetAxis("Vertical") > 0 && Input.GetButtonDown ("Jump") && canJump){

				player.gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeRotation;
				player.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(0,impulseForceUp), ForceMode2D.Impulse);
				ExitHook ();
			}

			if (Input.GetAxis ("Horizontal") > 0 && gameObject.transform.position.x < MainPlayer.mainPlayer.transform.position.x && Input.GetButtonDown("Jump") && canJump){

				player.gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeRotation;
				player.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(impulseForceSide,impulseForceSideUp), ForceMode2D.Impulse);
				ExitHook ();
			}

			if (Input.GetAxis ("Horizontal") < 0 && gameObject.transform.position.x > MainPlayer.mainPlayer.transform.position.x && Input.GetButtonDown("Jump") && canJump){

				player.gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeRotation;
				player.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(-impulseForceSide,impulseForceSideUp), ForceMode2D.Impulse);
				ExitHook ();
			}

			if (Input.GetAxis("Vertical") < 0 && Input.GetButtonDown("Jump") && canJump){

				player.gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeRotation;
				if (gameObject.transform.position.x > MainPlayer.mainPlayer.transform.position.x ){

					player.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(-2,0), ForceMode2D.Impulse);
				} else {

					player.gameObject.GetComponent<Rigidbody2D> ().AddForce (new Vector2(2,0), ForceMode2D.Impulse);
				}
				ExitHook ();
			}
		}
		}


	#region Collision Check
	void OnTriggerEnter2D(Collider2D col){

		//If collider is not player, return
		if (col.gameObject.tag != "wallhook" && canHook){
			return;
		}

		MainPlayer.mainPlayer.gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeAll;
		MainPlayer.mainPlayer.gameObject.GetComponent<GeneralMovementControl> ().enalbeWallJump = false;
		player = MainPlayer.mainPlayer.gameObject;
		MainPlayer.mainPlayer.allowedJumpTimes = 0;
		onWallHook = true;
		canJump = false;
		StartCoroutine (TimeJump ());
	}/*
	void OnTriggerExit2D(Collider2D col){

		//If collider is not player, return
		if (col.gameObject.tag != "wallhook"){
			return;
		}
		player = null;
		onWallHook = false;
		MainPlayer.mainPlayer.gameObject.GetComponent<GeneralMovementControl> ().enalbeWallJump = true;
		canHook = false;
		StartCoroutine (TimeHook ());
	}*/
	void ExitHook(){

		player = null;
		onWallHook = false;
		MainPlayer.mainPlayer.gameObject.GetComponent<GeneralMovementControl> ().enalbeWallJump = true;
		canHook = false;
		StartCoroutine (TimeHook ());
}

	IEnumerator TimeJump(){

		yield return new WaitForSeconds (.3f);
		canJump = true;
	}

	IEnumerator TimeHook(){

		yield return new WaitForSeconds (1);
		canHook = true;
	}
	#endregion
}
