using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLargeSpider : EnemyBase {


	public GameObject[] spidersToSpawn;

	public float zOffset = 3.0f;

	public float movingTime = 2.0f;
	public float spawningTime = 3.0f;

	public float spawnNum = 5;

	Vector3 spawnPosition;

	float lastSpawnTime = 0;

	enum MovingState{
		Coming,
		Spawning,
		Leaving,
		None,
	}

	MovingState moveState = MovingState.None;
	// Use this for initialization
	void Start () {
		//test
		ShowUp(transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		switch (moveState) {
		case MovingState.Coming:
			
			MoveZ (true);
			break;
		case MovingState.Spawning:
			Spawn ();
			break;
		case MovingState.Leaving:
			MoveZ (false);
			break;
		default:
			break;
		}
	}

	void MoveZ(bool forward){
		Vector3 curPos = transform.position;

		Vector3 move = Vector3.forward * zOffset / movingTime*Time.deltaTime;
		if (forward) {
			move = move * -1;
		}

		curPos += move;

		curPos.z = Mathf.Clamp (curPos.z, spawnPosition.z, spawnPosition.z + zOffset);
		transform.position = curPos;
	}


	void Spawn(){
		if (spidersToSpawn.Length <= 0)
			return;
		
		if (lastSpawnTime <= 0 || Time.time - lastSpawnTime >= spawningTime/spawnNum) {
			if (lastSpawnTime <= 0)
				lastSpawnTime = Time.time;
			else
				lastSpawnTime = lastSpawnTime + spawningTime / spawnNum;
			//current spawn is not rrandom
			GameObject enemy = GameObject.Instantiate (spidersToSpawn[0], transform.position, Quaternion.identity);
			Rigidbody2D body = enemy.GetComponentInChildren <Rigidbody2D>();
			body.velocity = Random.insideUnitCircle*10;
		}
	}


	public void ShowUp(Vector3 position){
		spawnPosition = position;
		StartCoroutine (ComeAndGo());
	}


	IEnumerator ComeAndGo(){
		Vector3 position = spawnPosition + Vector3.forward * zOffset;
		transform.position = position;
		moveState = MovingState.Coming;
		yield return new WaitForSeconds(movingTime);

		moveState = MovingState.Spawning;
		transform.position = spawnPosition;
		yield return new WaitForSeconds(spawningTime);

		moveState = MovingState.Leaving;
		yield return new WaitForSeconds(movingTime);

		GameObject.Destroy (gameObject);
	}
}
