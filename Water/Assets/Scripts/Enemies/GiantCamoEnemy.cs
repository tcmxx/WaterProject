using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GiantCamoEnemy : MonoBehaviour
{
	public float attackPause = 1;
	private float nextAttack;
	private bool isAttacking = false;
	
	void Start()
	{
		
	}
	
	void Update()
	{
		if (isAttacking)
		{
			if (Time.time >= nextAttack)
			{
				PlayAnimation();
			}
		}
	}
	
	void OnTriggerEnter2D(Collider2D col) {
		
		if (col.gameObject.tag == "Player")
		{
			isAttacking = true;
			nextAttack = Time.time + attackPause;
		}
    }
	
	void OnTriggerExit2D(Collider2D col) {
		
        if (col.gameObject.tag == "Player")
		{
			isAttacking = false;
		}
    }
	
	void PlayAnimation()
	{
		//Something from Animator here
		
		nextAttack = Time.time + attackPause;
	}
}