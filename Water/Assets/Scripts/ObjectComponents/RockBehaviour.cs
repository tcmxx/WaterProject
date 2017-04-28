using UnityEngine;
using System.Collections;

public class RockBehaviour : PushableObject {

	#region Variables

	public static RockBehaviour rockBehaviour;
	private Rigidbody2D rBody;


	#endregion

	void Start()
	{
		rockBehaviour = this;
		rBody = this.gameObject.GetComponent<Rigidbody2D> ();
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		if (rBody.velocity.y < -10)
		{

			Explode ();
		}
	}

	void Explode()
	{
		Destroy(this.gameObject.GetComponent<Rigidbody2D> ());
		Destroy(this.gameObject.GetComponent<PolygonCollider2D> ());
		foreach (Transform child in transform)
		{

			Rigidbody2D RB2D = child.gameObject.AddComponent <Rigidbody2D>() as Rigidbody2D;
			RB2D.mass = 5;
			RB2D.drag = 1;
			child.gameObject.AddComponent <PolygonCollider2D>();
		}
	}
}
