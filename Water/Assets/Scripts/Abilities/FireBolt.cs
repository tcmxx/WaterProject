using UnityEngine;
using System.Collections;

public class FireBolt : CommonObject {


	public const float ATTACK_SPEED_FIRERBOLT_DEFAULT = 20f;

	private float attackSpeed = ATTACK_SPEED_FIRERBOLT_DEFAULT;
	public const float MAX_LIVE_TIME_NORMAL = 6f;
	public const float MAX_LIVE_TIME_LONG = 10f;
	public int fireBoltDamage = 0;
	public bool isHoldBolt = false;

	public GameObject burningBuffPref;

	private Transform target = null;
	private Vector3 dummyTarget = Vector3.zero;

	private Rigidbody2D rb;

	void Awake(){
		rb = GetComponent <Rigidbody2D> ();
	}


	// Use this for initialization
	void Start () {

		if (isHoldBolt)
		{
			
		} else {
			
			Invoke ("SelfDestroy", MAX_LIVE_TIME_NORMAL);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (target != null ) {

			//transform.position = Vector3.MoveTowards (transform.position, target.position, Time.deltaTime * attackSpeed);

			rb.velocity = (target.position - transform.position).normalized * attackSpeed;

		}  else if (dummyTarget != Vector3.zero) {
			//transform.position = Vector3.MoveTowards (transform.position, dummyTarget, Time.deltaTime * attackSpeed);
			rb.velocity = (dummyTarget - transform.position  ).normalized * attackSpeed;
		}
	}

	#region Attack Function
	//Attack Function
	public void Attack(Transform targetIn, float speed = ATTACK_SPEED_FIRERBOLT_DEFAULT)
	{
		if (target != null  || dummyTarget != Vector3.zero) {
			//if attack is called already. Do not run the codes
			return;
		}

		if (targetIn != null) {
			target = targetIn;

		} else{
			dummyTarget = new Vector3 ((MainPlayer.mainPlayer.generalMovementControl.movingRight ? 99999 : -99999), MainPlayer.mainPlayer.transform.position.y);
		} 



	}

	#endregion

	#region OnTriggerEnter2D Function
	//OnTriggerEnter2D Function
	void OnTriggerEnter2D(Collider2D col)
	{


		if ((target != null || dummyTarget != Vector3.zero)&& col.tag != "Player" && !col.isTrigger) {

			CommonObject commonObject = col.gameObject.GetComponent<CommonObject>();
			if(commonObject != null)
			{
				
				commonObject.TakeDamage (fireBoltDamage,DamageType.Fire,this.gameObject);

				GameObject buff = GameObject.Instantiate (burningBuffPref);
				buff.GetComponent <GenericBuff>().AttachToParent (commonObject);

			}
			Destroy(gameObject);
		}

	}
	#endregion

	#region OnTriggerExit2D Function
	//On TriggerExit2D Function
	void OnTriggerExit2D(Collider2D col)
	{

	}
	#endregion
	#region OnTriggerStay2D Function
	//OnTriggerEnter2D Function
	void OnTriggerStay2D(Collider2D col)
	{



	}
	#endregion




	void SelfDestroy(){
		Destroy (gameObject);
	}
}
