using UnityEngine;
using System.Collections;

public class WaterWaveObject : MonoBehaviour {

	public int waterStep = 0;
	public const float ATTACK_SPEED_WATERWAVE_DEFAULT = 20f;
	public bool doingWaterWave;
	public GameObject closestWater;
	public float moveSpeed;
	public float MAX_LIVE_TIME_NORMAL;
	public GameObject parent;
	public float getWaterWaveAtkDist = 20;
	public GameObject ColdBuff;
	public int waterWaveDamage = 40;
	public bool canAttack = false;
	private bool attack = false;
	private bool once = false;
	public float attackSpeed;
	public Transform target;
	public Vector3 dummyTarget;
	public float frequency;
	public float magnitude;
	//private float bezierTime0 = 0;
	//private float bezierTime1 = 0;
	//private float bezierTime2 = 0;
	private float bezierTime3 = 0;
	private Transform startPos;
	private Vector3 lastPos;
	private bool rightMov;
	private Vector3 axis;
	public float firstCurvePointiness;
	public GameObject midPoint;
	private GameObject midPointInst;
	public GameObject waterWaveVisual;
	public GameObject waterSpashGenerator;

	// Use this for initialization
	void Awake () {
		Physics2D.IgnoreCollision (gameObject.GetComponent <CircleCollider2D>(), MainPlayer.mainPlayer.GetComponent <PolygonCollider2D>(), true);
		Physics2D.IgnoreCollision (gameObject.GetComponent <CircleCollider2D>(), MainPlayer.mainPlayer.GetComponent <CircleCollider2D>(), true);
		Physics2D.IgnoreCollision (parent.GetComponent <CircleCollider2D>(), MainPlayer.mainPlayer.GetComponent <PolygonCollider2D>(), true);
		Physics2D.IgnoreCollision (parent.GetComponent <CircleCollider2D>(), MainPlayer.mainPlayer.GetComponent <CircleCollider2D>(), true);

		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject enemy in enemies) {
			Physics2D.IgnoreCollision (parent.GetComponent <CircleCollider2D> (), enemy.GetComponent <CircleCollider2D> (), true);
		}
	}
	
	// Update is called once per frame
	void Update () {
	


		if (doingWaterWave) {

			if (waterStep == 0) {
				
				parent.GetComponent<Rigidbody2D> ().isKinematic = true;
				//Do something here
				parent.transform.position = Vector3.MoveTowards (parent.transform.position, new Vector2 (closestWater.transform.position.x, closestWater.transform.position.y + 3), moveSpeed * 10 * Time.deltaTime);



				float dist = Vector3.Distance (parent.transform.position, new Vector2 (closestWater.transform.position.x, closestWater.transform.position.y + 3));
				if (dist <= 1) {
					waterStep = 1;

					print ("Water Step :" + waterStep);

				}
			} else if (waterStep == 1) {

				parent.GetComponent<Rigidbody2D> ().isKinematic = false;
				parent.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (1 * (MainPlayer.mainPlayer.generalMovementControl.movingRight ? 1 : -1) * 20, 30), ForceMode2D.Impulse);
				//Destroy (parent, MAX_LIVE_TIME_NORMAL);
				waterStep = 2;
				canAttack = true;
				print ("Water Step :" + waterStep);
				Invoke ("DestroyAir", 1.5f);

			} else if (attack) {
				if (target != null) {

					//Calculate the Curve X position
					float curveX1 = (((1 - bezierTime3) * (1 - bezierTime3)) * parent.transform.position.x) + (2 * bezierTime3 * (1 - bezierTime3) * midPointInst.transform.position.x) + ((bezierTime3 * bezierTime3) * target.transform.position.x);

					//Calculate the Curve Y position
					float curveY1 = (((1 - bezierTime3) * (1 - bezierTime3)) * parent.transform.position.y) + (2 * bezierTime3 * (1 - bezierTime3) * midPointInst.transform.position.y) + ((bezierTime3 * bezierTime3) * target.transform.position.y);

					//Calculate the Curve Y position
					float curveZ1 = (((1 - bezierTime3) * (1 - bezierTime3)) * parent.transform.position.z) + (2 * bezierTime3 * (1 - bezierTime3) * midPointInst.transform.position.z) + ((bezierTime3 * bezierTime3) * target.transform.position.z);

					//Increment the time for the Bezier Curve
					bezierTime3 = bezierTime3 + (5 * Time.deltaTime);

					parent.transform.position = new Vector3 (curveX1, curveY1, curveZ1);

					Destroy (midPointInst, 1);

				} else if (dummyTarget != Vector3.zero) {

					if (once == false) {

						lastPos = parent.transform.position;
						axis = transform.up;
						rightMov = MainPlayer.mainPlayer.generalMovementControl.movingRight;
						once = true;
					}
					if (rightMov) {
						lastPos += transform.right * Time.deltaTime * attackSpeed;
					} else {
						lastPos -= transform.right * Time.deltaTime * attackSpeed;
					}

					parent.transform.position = lastPos + axis * Mathf.Sin (Time.time * frequency) * magnitude;

				}
			}
		}
	}

	void DestroyAir() {

		if (attack){
			return;
		}

		canAttack = false;
		attack = false;
		SelfDestroy();

	}
		
	public void Attack(Transform targetIn, float speed = ATTACK_SPEED_WATERWAVE_DEFAULT)

	{

		//Something
		float dist = Vector3.Distance (MainPlayer.mainPlayer.transform.position, parent.transform.position);
		if (dist <= getWaterWaveAtkDist && canAttack) {

			//midPointInst = (GameObject)Instantiate (midPoint, new Vector2( ((parent.transform.position.x + MainPlayer.mainPlayer.transform.position.x) / 2 ), ((parent.transform.position.y + MainPlayer.mainPlayer.transform.position.y) / 2 ) + firstCurvePointiness), Quaternion.identity);
			doingWaterWave = true;
			//parent.GetComponent<Rigidbody2D> ().isKinematic = true;
			canAttack = false;
			startPos = parent.transform;

			print("Water Wave Attack Function");

			Rigidbody2D rb = parent.GetComponent <Rigidbody2D> ();
			rb.isKinematic = false;

			if (targetIn != null) {
				rb.gravityScale = 0;
				moveSpeed = speed;
				target = targetIn;
				midPointInst = (GameObject)Instantiate (midPoint, new Vector2( ((parent.transform.position.x + targetIn.transform.position.x) / 2 ), ((parent.transform.position.y + targetIn.transform.position.y) / 2 ) + firstCurvePointiness), Quaternion.identity);
				MainPlayer.mainPlayer.characterInfo.waterWave = null;
				Invoke ("SelfDestroy", MAX_LIVE_TIME_NORMAL);
			} else{
				rb.gravityScale = 0;
				dummyTarget = new Vector3 ((MainPlayer.mainPlayer.generalMovementControl.movingRight ? 99999 : -99999), MainPlayer.mainPlayer.transform.position.y);
				moveSpeed = speed;
				MainPlayer.mainPlayer.characterInfo.waterWave = null;
				Invoke ("SelfDestroy", MAX_LIVE_TIME_NORMAL);
			} 

			attack = true;

		}
	}

	void SelfDisappear(){

		//waterWaveVisual.SetActive (false);
		target = null;
		dummyTarget = Vector3.zero;
		//parent.gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezeAll;
		//parent.gameObject.GetComponent<Rigidbody2D> ().isKinematic = true;
		parent.gameObject.GetComponent<CircleCollider2D> ().enabled = false;
	}

	void SelfDestroy(){
		SelfDisappear ();
		Destroy (parent.gameObject, 1);
	}


	#region OnTriggerEnter2D Function
	//OnTriggerEnter2D Function
	void OnTriggerEnter2D(Collider2D col)
	{
			if (col.GetComponentInParent<Rigidbody2D> ()) {
				Rigidbody2D RB = col.GetComponentInParent<Rigidbody2D> ();
			
				if (RB != null && col.gameObject.transform.position.x < transform.position.x) {
					RB.AddForceAtPosition (new Vector2 (-1, 1) * 100, col.transform.position);

				} else if (RB != null) {
					RB.AddForceAtPosition (new Vector2 (1, 1) * 100, col.transform.position);
				}
			} else {
				Rigidbody2D RB = col.GetComponent<Rigidbody2D> ();

				if (RB != null && col.gameObject.transform.position.x < transform.position.x) {
					RB.AddForceAtPosition (new Vector2 (-1, 1) * 100, col.transform.position);

				} else if (RB != null) {
					RB.AddForceAtPosition (new Vector2 (1, 1) * 100, col.transform.position);
				}
		}
			
	}
	#endregion

	public void CollisionEnter2D(Collision2D col)
	{
		if ((target != null || dummyTarget != Vector3.zero) && col.collider.tag != "Player") {

			GenerateWaterSplash (col);
			SelfDisappear ();
			Invoke ("SelfDestroy", MAX_LIVE_TIME_NORMAL);
		}
	}

	private bool GenerateWaterSplash(Collision2D collision){
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


		GameObject obj = (GameObject)GameObject.Instantiate (waterSpashGenerator,pos,Quaternion.identity);
		WaterSplashGenerator gen = obj.GetComponent <WaterSplashGenerator>();
		gen.Generate (-normal);

		return true;
	}

	private void AttachColdBuff(CommonObject obj){
		GameObject buff = GameObject.Instantiate (ColdBuff);
		buff.GetComponent <GenericBuff> ().AttachToParent (obj);
	}
}
