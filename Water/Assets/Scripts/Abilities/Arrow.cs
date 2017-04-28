using UnityEngine;
using System.Collections;

public class Arrow : CommonObject {



	public const float MAX_LIVE_TIME_LONG = 100f;
	public const float MAX_LIVE_TIME_SHORT = 3f;
	public const int DAMAGE = 20;
	public const float SHOOT_FORCE = 200f;

	public bool isAttacking;



	// Use this for initialization
	void Start () {
		Physics2D.IgnoreCollision (MainPlayer.mainPlayer.GetComponent <BoxCollider2D>(), GetComponent <BoxCollider2D>());
		//Vector2 tmp = GetComponent <Rigidbody2D> ().centerOfMass;
		//GetComponent <Rigidbody2D> ().centerOfMass = Vector2.up * 0.5f;
		//Physics2D.IgnoreCollision (MainPlayer.mainPlayer.GetComponent <BoxCollider2D>(), HeadRigidBody.gameObject.GetComponent <BoxCollider2D>());
	}

	// Update is called once per frame
	void Update () {
		Vector2 vDir = GetComponent <Rigidbody2D> ().velocity.normalized;

		if (isAttacking && vDir.magnitude >0) {
			
			float angle = Vector2.Angle (Vector2.up, -vDir);

			if (vDir.x < 0) {
				angle += 180;
			} else {
				angle = -angle + 180;
			}

			transform.eulerAngles = Vector3.back * (angle);

		}
	}


	public void TrackMouse(){
		Vector3 tmpPos = Input.mousePosition;
		tmpPos.z = -CameraFollowPlayer.cameraFollowPlayer.transform.position.z;
		Vector2 mousePos = Camera.main.ScreenToWorldPoint (tmpPos);

		float angle = Vector2.Angle (Vector2.up, mousePos - (Vector2)transform.position);
		if (mousePos.x > transform.position.x) {
			angle = -angle;
		}
		transform.eulerAngles = -Vector3.back * angle;
	}

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){
		if (attackMode == CharacterInfo.ATTACK_MODE.NORMAL && interactKey == InteractKey.Attack && direction == InteractDirectionKey.None) {
			transform.parent = null;
			isAttacking = true;

			Rigidbody2D rb = GetComponent <Rigidbody2D> ();
			rb.isKinematic = false;
			GetComponent <Collider2D>().isTrigger = false;
			Vector3 tmpPos = Input.mousePosition;
			tmpPos.z = -CameraFollowPlayer.cameraFollowPlayer.transform.position.z;
			rb.AddForce ( (Camera.main.ScreenToWorldPoint (tmpPos)- MainPlayer.mainPlayer.transform.position).normalized * SHOOT_FORCE);

			Invoke ("SelfDestroy", MAX_LIVE_TIME_LONG);
			Camera.main.GetComponent <CameraFollowPlayer>().UseInitialView ();

			return true;
		}

		return false;
	}



	public void OnCollisionEnter2D(Collision2D col)
	{

		Collider2D[] f = Physics2D.OverlapCircleAll (transform.position, 1f);

		if(isAttacking){
			CommonObject commonObject = col.gameObject.GetComponent <CommonObject>();
			if (commonObject != null) {
				commonObject.TakeDamage (DAMAGE , DamageType.Normal, gameObject);
			}

			isAttacking = false;
		}

		Invoke ("SelfDestroy", MAX_LIVE_TIME_SHORT);
	}


	void SelfDestroy(){
		Destroy (gameObject);
	}
}
