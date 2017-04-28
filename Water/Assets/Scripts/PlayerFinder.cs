using UnityEngine;
using System.Collections;

/*
 *This script is used for player detection by the enemy
 *No part should really be used for editing.
 * 
 */

public class PlayerFinder : MonoBehaviour {

	EnemyVairableSet[] enemyVars;
	EnemyMovementScript[] parent;
	Transform player;
	private RaycastHit2D hit;

	/*static public vars for scripts*/
	public bool playerCol= false;
	public bool los = false;
	/*static public vars for scripts*/

	// Use this for initialization
	void Start () {
		enemyVars = this.transform.GetComponentsInParent<EnemyVairableSet> ();
		parent = this.transform.GetComponentsInParent<EnemyMovementScript> (); // used to get parent vairables from the movement
		player = GameObject.FindGameObjectWithTag ("Player").transform; // used so the enemy is aware of the player at all times
	}
	
	// Update is called once per frame
	void Update () {
		if (playerCol) { // if player was in range of the enemy
			Vector2 ray = new Vector2 (player.position.x, player.position.y); // create a ray / vector of enemy position
			ray.x = ray.x - transform.position.x; // get the distance from enemy
			ray.y = ray.y - transform.position.y; // get the height from enemy
			if (!(enemyVars[0].air)) { // if the player is a ground enemy give him a set vision
				if (ray.y > enemyVars[0].groundHeight) {
					ray.y = enemyVars[0].groundHeight; // if the height is greater than he can see, fix it
				}
			}
			RaycastHit2D hit; // create a local ray cast hit
			int layerMask = (1 << 17) | (1 << 12); // get a layer mask for EnemyRayCast and Floor layers
			hit = Physics2D.Raycast (transform.position, ray, 5, layerMask); // do the raycast
			if (hit.collider != null) { // check if it hit something
				if (hit.collider.name == "EnemyRayCast") { // if hits player he is LOS
					los = true;
				} else {
					los = false;
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player") { // player hits enemy Range
			playerCol = true;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.gameObject.tag == "Player") { // Player leaves enemy Range
			playerCol = false;
			los = false;
		}
	}

}
