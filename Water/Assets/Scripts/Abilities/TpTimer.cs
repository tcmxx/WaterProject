using UnityEngine;
using System.Collections;

public class TpTimer : MonoBehaviour {

	private bool isEnabled = false;
	public int incValue = 1;
	public static TpTimer tpTimer;
	private Rigidbody2D rb;
	public float timer = 3;

	// Use this for initialization
	void Start () {

		rb = MainPlayer.mainPlayer.GetComponent<Rigidbody2D> ();
		tpTimer = this;
	}
	
	// Update is called once per frame
	void Update () {
	
		//if is enabled
		if (isEnabled){

			//if the player is not moving
			if(rb.velocity != Vector2.zero){

				//Stop the timer
				StopTimer ();
			}

			//Check if timer is at max value or not
			if (timer <= 0) {

				IsMaxValue ();
			} else {
				
				//If user is still pressing the button decrease timer
				if(Input.GetButton ("TpToTeleporter")){
					
					timer -= incValue * Time.deltaTime;
					
				//else stop timer
				}else{
					StopTimer ();
				}
			}
		}
	}

	//Stop timer function
	void StopTimer(){
		timer = 3;
		isEnabled = false;
	}

	//Start timer function
	public void StartTimer(){

		timer = 3;
		isEnabled = true;
	}

	//Max value function
	void IsMaxValue(){

		isEnabled = false;
		MainPlayer.mainPlayer.Teleport ();
	}
}
