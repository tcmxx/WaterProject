using UnityEngine;
using System.Collections;

public class EnemyVairableSet : MonoBehaviour {
	/* reference in EnemyMovementScript.cs */
	public float leftbound=10.0f,rightbound=10.0f,upperbound=5.0f,lowerbound=2.0f;
	public bool pylons = true;
	public bool air = false;
	public bool charge = true;
	public float speed = 10.0f;
	/* reference in EnemyMovementScript.cs */

	public float giveUpDis =3; // Reference in ObjectFinder .cs

	public float groundHeight =2.0f; // Reference in PlayerFinder.cs

}
