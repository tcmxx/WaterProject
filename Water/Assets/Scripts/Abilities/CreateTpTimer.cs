using UnityEngine;
using System.Collections;

public class CreateTpTimer : MonoBehaviour {

	private bool isEnabled = false;
	public int incValue = 1;
	public static CreateTpTimer createTpTimer;
	private Rigidbody2D rb;
	public float timer = 3;

	// Use this for initialization
	void Start () {
		
		//Get RB from the player
		rb = MainPlayer.mainPlayer.GetComponent<Rigidbody2D> ();
		createTpTimer = this;
	}

	// Update is called once per frame
	void Update () {
	
		//if is enabled
		if (isEnabled){
			
			//if player is moving
			if(rb.velocity != Vector2.zero){
				
				//stop the timer
				StopTimer ();
			}

			//Check if the timer value is max or not
			if (timer <= 0) {

				IsMaxValue ();
			} else {
				
				//if the user is still pressing the key , continue the timer
				if(Input.GetButton ("CreateTeleporter")){

					timer -= incValue * Time.deltaTime;
					
				//else it will stop the timer
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
		MainPlayer.mainPlayer.CreateTeleport ();
	}
}
