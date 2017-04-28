using UnityEngine;
using System.Collections;

public class ObjectFinder : MonoBehaviour {
	GameObject leftPylon, rightPylon;
	GameObject Platform;
	EnemyMovementScript[] parent;
	EnemyVairableSet[] enemyVars;
	Vector2 floorBounds;
	/*static public vars for scripts*/
	public bool left= false;
	public bool giveup = false;
	public bool inbounds = false;
	/*static public vars for scripts*/
	void Start(){
		Platform = GameObject.Find ("Platform");
		parent = this.transform.GetComponentsInParent<EnemyMovementScript> ();
		enemyVars = this.transform.GetComponentsInParent<EnemyVairableSet> ();
		floorBounds = new Vector2 (transform.position.x - enemyVars [0].leftbound, transform.position.x + enemyVars [0].rightbound);
	}

	void Update(){
		if ((parent [0].overide)) {
			if (parent [0].los) {
				left = parent [0].overideLeft;
			}
			if (leftPylon != null) {
				if ((transform.position.x - enemyVars[0].giveUpDis) <= leftPylon.transform.position.x) {
					left = false;
					giveup = true;
				}
			}
			if (rightPylon != null){
				if (rightPylon.transform.position.x <= (transform.position.x + enemyVars[0].giveUpDis)) {
					left = true;
					giveup = true;
				}
			}
		}
		if (leftPylon != null) {
			if (transform.position.x > (leftPylon.transform.position.x) ){
				inbounds = true;
			} else {
				inbounds = false;
			}
		}
		if (rightPylon != null) {
			if (transform.position.x < rightPylon.transform.position.x) {
				inbounds = true;
			} else {
				inbounds = false;
			}
		}
		if (rightPylon != null && leftPylon != null) {
			if ((transform.position.x > leftPylon.transform.position.x) && (transform.position.x < rightPylon.transform.position.x)) {
				inbounds = true;
			} else {
				inbounds = false;
				giveup = true;
			}
		}
		if (enemyVars [0].pylons) {
			if (!inbounds) {
				if (transform.position.x < leftPylon.transform.position.x) {
					left = false;
					giveup = true;
				}
				if (rightPylon.transform.position.x < transform.position.x) {
					left = true;
					giveup = true;
				}
			}
		} else {
			if (transform.position.x >= floorBounds.y - 2.0f) {
				left = true;
			} else if (transform.position.x <= floorBounds.x + 2.0f) {
				left = false;
			}
		}

	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (enemyVars[0].pylons) {
			if (col.gameObject.name.ToString () == "LeftPylon") {
				leftPylon = col.gameObject;
				left = false;
				giveup = true;
			}
			if (col.gameObject.name.ToString () == "RightPylon") {
				rightPylon = col.gameObject;
				left = true;
				giveup = true;
			}
		}

	}

	void OnTriggerExit2D(Collider2D col)
	{

		if (col.gameObject.name.ToString () == "LeftPylon") {
			giveup = false;
		}
		if (col.gameObject.name.ToString () == "RightPylon") {
			giveup = false;
		}
	}
}
