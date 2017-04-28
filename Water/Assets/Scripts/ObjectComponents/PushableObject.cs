using UnityEngine;
using System.Collections;

public class PushableObject : CommonObject {

	public void OnCollisionEnter2D(Collision2D col) {

		if (col.gameObject.tag == "Player") 
		{
			if((objectProperty & CommonObjectProperty.Pushable)!= 0)
			{

				MainPlayer.mainPlayer.characterInfo.isPushing = true;
				MainPlayer.mainPlayer.anim.SetBool ("isPushing", true);
			}
		}
	}

	public void OnCollisionExit2D(Collision2D col) {

		if (col.gameObject.tag == "Player") 
		{
			if((objectProperty & CommonObjectProperty.Pushable)!= 0)
			{

				MainPlayer.mainPlayer.characterInfo.isPushing = false;
				MainPlayer.mainPlayer.anim.SetBool ("isPushing", false);
			}
		}
	}
}
