using UnityEngine;
using System.Collections;

public class waterWallRiseScript : MonoBehaviour {

	public float thrust;
	public Rigidbody2D rb;
	private float timer ;
	public float timerSet;


	void Start() {
		timer = timerSet;
		rb = GetComponent<Rigidbody2D>();
		rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
	}

	void Update() {
		if (timer < 0) {
			//rb.AddForce(transform.up * thrust, ForceMode2D.Impulse);
			//timer = timerSet;
			rb.isKinematic = true;
		} else if (timer > 0) {
			timer -= 1 * Time.deltaTime;
		}
	}
}
