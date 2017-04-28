using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnemyMovementScript : MonoBehaviour {
	/*Game objects to get*/
	GameObject[] floor;
	GameObject Platform;
	GameObject specific;
	Transform player;
	EnemyVairableSet[] enemyVars;
	ObjectFinder[] obj;
	PlayerFinder[] objPlayer;

	Vector2 airBounds;
	Vector3 local;

	private Rigidbody2D rb;
	Transform trans;
	private bool look = false;
	bool left = false;
	bool up = false;
	private bool usePylons = true;


	/*static public vars for scripts*/
	public bool los = false;
	public bool overideLeft = false;
	public bool overide = false;
	/*static public vars for scripts*/

	Vector3 move = new Vector3(0.0f,0.0f,0.0f);

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody2D> ();
		trans = GetComponent<Transform> ();
		Platform = GameObject.Find ("Platform");
		obj = this.transform.GetComponentsInChildren<ObjectFinder> ();
		objPlayer = this.transform.GetComponentsInChildren<PlayerFinder> ();
		enemyVars = this.transform.GetComponentsInParent<EnemyVairableSet> ();
		player =  GameObject.FindGameObjectWithTag ("Player").transform;
		airBounds = new Vector2 (0.0f, 0.0f);
		if (enemyVars[0].air) {
			airBounds = new Vector2 (enemyVars[0].lowerbound, enemyVars[0].upperbound);
			rb.gravityScale = 0;
		}

	}
	// Update is called once per frame
	void Update () {
		usePylons = false;
		los = objPlayer [0].los;
		if (enemyVars[0].charge) {
			if (obj [0].inbounds) {
				if (objPlayer [0].los && !obj [0].giveup) {
					overide = true;
					MovementFollow ();
				} else {
					overide = false;
					usePylons = true;
				}
			} else {
				overide = false;
				usePylons = true;
			}
		} else {
			usePylons = true;
		}
		if (usePylons) {
			MovementPylon ();
		}
	}

	void MovementPylon(){
		local.Set (trans.position.x, trans.position.y, trans.position.z);
		left = obj[0].left;
		if (left) {
			move = new Vector3 (-1.0f, move.y, move.z);
		} else {
			move = new Vector3 (1.0f, move.y, move.z);
		}
		if (enemyVars[0].air) {
			if (local.y >= airBounds.y) {
				up = false;
			} else if (local.y <= airBounds.x) {
				up = true;
			}
			if (up) {
				move = new Vector3 (move.x, 1.0f, move.z);
			} else {
				move = new Vector3 (move.x, -1.0f, move.z);
			}
		}
		rb.AddForce (move * enemyVars[0].speed);
	}
	void MovementFollow(){
		if (transform.position.x < player.transform.position.x) {
			overideLeft = false;
			move = new Vector3 (1.0f, 0.0f, 0.0f);
		}
		if (transform.position.x > player.transform.position.x) {
			overideLeft = true;
			move = new Vector3 (-1.0f, 0.0f, 0.0f);
		}
		if (enemyVars[0].air) {
			if (transform.position.y < player.transform.position.y) {
				move = new Vector3 (move.x, 1.0f, move.z);
			}
			if (transform.position.y > player.transform.position.y) {
				move = new Vector3 (move.x, -1.0f, move.z);
			}
		}
		rb.AddForce (move * enemyVars[0].speed);
	}
		
}
	