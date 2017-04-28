using UnityEngine;
using System.Collections;

public class EnemyBase : CommonObject {

	//SF Vars
	[SerializeField]private int Health = 100;

	void Start () {

	}

	void Update () {
		
		
	}


	public virtual void Attack(Vector3 targetPosition, CommonObject target = null){


		if (target != null) {
			bool damage = target.TakeDamage (1,DamageType.Normal, gameObject);
			if (damage) {
				Rigidbody2D bd = target.GetComponent <Rigidbody2D> ();
				if (bd != null) {
					bd.AddForce ((targetPosition - transform.position).normalized * 50, ForceMode2D.Impulse);
				}
			}
		}

		//for debugging
		string name = targetPosition.ToString ();
		if (target != null)
			name = target.name;
		print (this.gameObject.name + " attack " + name);
	}


	public override bool TakeDamage(int damage, DamageType damageType = DamageType.Normal, GameObject damageFromObject = null)
    {
        base.TakeDamage(damage, damageType);
        Health -= damage;
		if (Health <= 0 && gameObject != null)
        {

            Destroy(gameObject);
        }

		return true;
    }
}
