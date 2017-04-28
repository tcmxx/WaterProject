using UnityEngine;
using System.Collections;

public class crumble : MonoBehaviour {

	private GameObject player;
	private CircleCollider2D playerCol;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("MainPlayer");
		playerCol = player.GetComponent<CircleCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.name == "MainPlayer") {
			GetComponent<Rigidbody2D> ().isKinematic = false;
		}
	}
}
