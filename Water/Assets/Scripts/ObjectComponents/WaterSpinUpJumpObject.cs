using UnityEngine;
using System.Collections;

public class WaterSpinUpJumpObject : CommonObject {


	public float offsetY = .3f;
	public Vector2 startPos;
	public Vector2 targetPos;
	public bool move = false;
	public float height = 3;
	public float moveSpeed = 15;
	public float playerHeight = 2;

	// Use this for initialization
	void Start () {

		startPos = new Vector2(this.transform.position.x, this.transform.position.y + offsetY);
		targetPos = new Vector2(startPos.x, startPos.y + height);
		transform.position = startPos;
		move = true;
	}
	
	// Update is called once per frame
	void Update () {
		targetPos = new Vector2(this.transform.position.x, MainPlayer.mainPlayer.gameObject.transform.position.y - playerHeight/2);
		
		//Move Up and hit Player
		if (move) {
		
			transform.position = Vector2.MoveTowards(this.transform.position, targetPos, moveSpeed * Time.deltaTime);
			
			//If hits the player then move down
			float dist = Vector2.Distance(this.transform.position, targetPos);
			if (dist <= .1f) {
			
				move = false;
				MoveDown();
				
			}
		}
	}

	void MoveDown() 
	{
		this.GetComponent<Rigidbody2D> ().isKinematic = false;
		GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation ;;
		Destroy(this.gameObject, 1);
	
	}
}
