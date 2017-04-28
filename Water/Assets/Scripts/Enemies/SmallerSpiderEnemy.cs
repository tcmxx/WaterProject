using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SmallerSpiderEnemy : MonoBehaviour
{
	public float attackPause = 1;
	public float attackRange = 1;
	private float nextAttack;
	private bool canAttack = false;
	private bool isInRange = false;
	private GameObject player;
	
	void Start()
	{
		
	}
	
	void Update()
	{
		if (isInRange)
		{
			float dist = Vector2.Distance(this.transform.position, player.transform.position);
			if (dist <= attackRange)
			{
				if (nextAttack <= Time.time)
					PlayAnimation();
			} else {
				
				MoveTowardsPlayer();
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D col) {
		
		if (col.gameObject.tag == "Player")
		{
			isInRange = true;
			player = col.gameObject;
		}
    }
	
	void OnTriggerExit2D(Collider2D col) {
		
    }
	
	void MoveTowardsPlayer()
	{
		
		//Will update this soon !!
	}
	
	void PlayAnimation()
	{
		//Something from Animator here
		
		nextAttack = Time.time + attackPause;
	}
}