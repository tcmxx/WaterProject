using UnityEngine;
using System.Collections;

public class myWaterfall : MonoBehaviour {

	public GameObject waterDrop;
	private float timer ;
	public float timerSet = 10;

	// Use this for initialization
	void Start () {
		Instantiate (waterDrop, transform.position, Quaternion.identity);
		timer = timerSet;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer < 0) {
			//Instantiate (waterDrop, transform.position, Quaternion.identity);
			timer = timerSet;
		} else if (timer > 0) {
			timer -= 1 * Time.deltaTime;
		}
		//print (timer);
		
	}
}
