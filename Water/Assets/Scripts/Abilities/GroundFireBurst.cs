using UnityEngine;
using System.Collections;

public class GroundFireBurst : CommonObject {

	public int Damage;
	public float LiveTime = 2f;
	// Use this for initialization
	void Start () {
		Invoke ("SelfDestroy",LiveTime);
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerStay2D(Collider2D col){
		if ( col.tag != "Player"&&!col.isTrigger) {
			Rigidbody2D RB = col.GetComponent<Rigidbody2D>();
			if (RB != null && col.gameObject.transform.position.x < transform.position.x)
			{
				RB.AddForceAtPosition(new Vector2(-1, 1) * 100, col.transform.position);

			}
			else if (RB != null)
			{
				RB.AddForceAtPosition(new Vector2(1, 1) * 100, col.transform.position);
			}

			CommonObject commonObject = col.gameObject.GetComponent<CommonObject>();
			if(commonObject != null)
			{
				commonObject.TakeDamage(Damage);
			}
		}

		Physics2D.IgnoreCollision (col, GetComponent <Collider2D>());
	}


	void SelfDestroy(){
		Destroy (gameObject);
	}
}
