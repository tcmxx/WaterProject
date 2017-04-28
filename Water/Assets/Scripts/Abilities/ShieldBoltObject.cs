using UnityEngine;
using System.Collections;

public class ShieldBoltObject : CommonObject {
	
	bool isCreatingShield = true;
	float bezierTime = 0f;
	Transform lastShieldPos;
	GameObject midPointAtk;
	public GameObject midPoint;
	public GameObject attackObj;
	public Vector3 shieldPos;
	public GameObject shieldPref;
	public float moveSpeed;
	public float upMovementSpeed = 10;
	bool movingToPos = false;
	bool canAtk = false;
	public int atkTimes = 0;
	public float atkMoveSpeed = 5;
	public float shieldBoltsDist = 5;

	public GameObject myTrailObject;
	private TrailRenderer myTrail;

	// Use this for initialization
	void Start () { 

		myTrail = myTrailObject.GetComponent<TrailRenderer> ();
		//myTrail.enabled = false;
	

		lastShieldPos = transform;
		midPointAtk = (GameObject)Instantiate (midPoint, new Vector2 ((lastShieldPos.position.x + shieldPos.x) / 2,(lastShieldPos.position.y + shieldPos.y - 2) / 2 - 1), Quaternion.identity);

	}
	
	// Update is called once per frame
	void Update () {

		if (myTrail.startWidth <= .3f && myTrail.startWidth > 0) {
			myTrail.startWidth -= 0.01f;
		}

		if (isCreatingShield) {
			
			//Calculate the Curve X position
			float curveX1 = (((1 - bezierTime) * (1 - bezierTime)) * lastShieldPos.position.x) + (2 * bezierTime * (1 - bezierTime) * midPointAtk.transform.position.x) + ((bezierTime * bezierTime) * shieldPos.x);

			//Calculate the Curve Y position
			float curveY1 = (((1 - bezierTime) * (1 - bezierTime)) * lastShieldPos.position.y) + (2 * bezierTime * (1 - bezierTime) * midPointAtk.transform.position.y) + ((bezierTime * bezierTime) * (shieldPos.y - 1));

			//Calculate the Curve Y position
			float curveZ1 = (((1 - bezierTime) * (1 - bezierTime)) * lastShieldPos.position.z) + (2 * bezierTime * (1 - bezierTime) * midPointAtk.transform.position.z) + ((bezierTime * bezierTime) * shieldPos.z);

			//Increment the time for the Bezier Curve
			bezierTime = bezierTime + (moveSpeed * Time.deltaTime);
			
			transform.position = new Vector3 (curveX1, curveY1);

			
			if (Vector3.Distance (transform.position, new Vector2(shieldPos.x, shieldPos.y - 1)) < .1f) {
				
				//Instantiate (shieldPref, shieldPos, Quaternion.identity);
				isCreatingShield = false;
				movingToPos = true;
				Destroy (midPointAtk);
				MidPointFunction ();
			}
		}else if (movingToPos){

			transform.position = Vector2.MoveTowards (transform.position, new Vector2(shieldPos.x, shieldPos.y + 1), upMovementSpeed * Time.deltaTime);

			if (Vector3.Distance (transform.position, new Vector2(shieldPos.x, shieldPos.y + 1)) < .01f) {

				movingToPos = false;
				GetComponent<BoxCollider2D> ().enabled = true;
				Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), MainPlayer.mainPlayer.GetComponent<CircleCollider2D> (), true);
				Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), MainPlayer.mainPlayer.GetComponent<PolygonCollider2D> (), true);
				canAtk = true;
				StartCoroutine (ReduceShieldTime ());
			}
		}
	}

	//Reduce the Shield Time 
	IEnumerator ReduceShieldTime() {

		while (atkTimes < 10) {

			yield return new WaitForSeconds (2);
			atkTimes += 1;
			ReduceShieldHeight ();
		}
	}

	//Attack Function
	public void Attack() {

		float dist = Vector2.Distance (MainPlayer.mainPlayer.gameObject.transform.position, this.gameObject.transform.position);
		if (canAtk == false || dist > shieldBoltsDist){ 
			return;
		}

		if ((MainPlayer.mainPlayer.generalMovementControl.movingRight && this.gameObject.transform.position.x > MainPlayer.mainPlayer.transform.position.x) || (!MainPlayer.mainPlayer.generalMovementControl.movingRight && this.gameObject.transform.position.x < MainPlayer.mainPlayer.transform.position.x)) {

			MainPlayer.mainPlayer.anim.SetTrigger ("Punch");

			float offsetY = Random.Range (-0.5f, 0.5f);

			GameObject h = (GameObject)Instantiate (attackObj, new Vector2 (this.transform.position.x, this.transform.position.y - 1 + offsetY), Quaternion.identity);
			h.GetComponent<Rigidbody2D> ().velocity = new Vector2 (1 * atkMoveSpeed * (MainPlayer.mainPlayer.generalMovementControl.movingRight ? 1 : -1), 0);

			Physics2D.IgnoreCollision (h.GetComponent<BoxCollider2D> (), MainPlayer.mainPlayer.gameObject.GetComponent<CircleCollider2D> (), true);
			Physics2D.IgnoreCollision (h.GetComponent<BoxCollider2D> (), MainPlayer.mainPlayer.gameObject.GetComponent<PolygonCollider2D> (), true);

			Destroy (h, 2);

			canAtk = false;
			ReduceShieldHeight ();
			StartCoroutine (AtkTimer ());
		}

	}

	void ReduceShieldHeight() {

		if (atkTimes == 1) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 2.3f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1);


		} else if (atkTimes == 2) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 2f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1.1f);
			myTrail.startWidth = .9f;

		} else if (atkTimes == 3) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 1.9f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1.2f);
			myTrail.startWidth = .8f;

		} else if (atkTimes == 4) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 1.7f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1.3f);
			myTrail.startWidth = .7f;

		} else if (atkTimes == 5) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 1.4f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1.4f);
			myTrail.startWidth = .6f;

		} else if (atkTimes == 6) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 1.2f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1.5f);

		} else if (atkTimes == 7) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 0.9f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1.6f);
			myTrail.startWidth = .5f;

		} else if (atkTimes == 8) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 0.8f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1.7f);
			myTrail.startWidth = .4f;

		} else if (atkTimes == 9) {

			//this.GetComponent<BoxCollider2D> ().size = new Vector2(0.5f, 0.7f);
			//this.GetComponent<BoxCollider2D> ().offset = new Vector2(0, -1.8f);
			myTrail.startWidth = .35f;

		} else if (atkTimes == 10){

			MainPlayer.mainPlayer.characterInfo.shieldBoltObject = null;
			myTrail.startWidth = .3f;
			StartCoroutine (waitForShieldToDisappear ());

		}
		//Something Here , Good Luck :P
	}

	IEnumerator AtkTimer() {
		
		atkTimes += 1;
		ReduceShieldHeight ();
		yield return new WaitForSeconds (.5f);
		canAtk = true;
	}

	IEnumerator waitForShieldToDisappear() {
		yield return new WaitForSeconds (.5f);
		Destroy (this.gameObject);
	}



	void MidPointFunction() {
		//Something Here , Good Luck :P
	
		myTrail.enabled = true;

	}


}
