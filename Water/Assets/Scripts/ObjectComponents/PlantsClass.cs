using UnityEngine;
using System.Collections;

public class PlantsClass : CommonObject {

	#region Variables
	[SerializeField]private int waterNumber = 0;
	public GameObject specialResourcePref;


	#endregion

	void Start()
	{

		Physics2D.IgnoreCollision (GetComponent<CircleCollider2D> (), GameObject.Find ("MainPlayer").GetComponent<BoxCollider2D> ());
	}
		
	void OnWateredPlant()
	{

		waterNumber += 1;
		if(waterNumber >= 5)
		{

			DropSpecialResource ();
			waterNumber = 0;
		}
	}

	void DropSpecialResource()
	{

		Instantiate (specialResourcePref, new Vector3(this.transform.position.x, this.transform.position.y, -1f), Quaternion.identity);
	}


    #region Take Damage
	public override bool TakeDamage(int damage, DamageType damageType = DamageType.Normal, GameObject damageFromObject = null)
    {
        OnWateredPlant();
		return true;
    }
    #endregion

}
