using UnityEngine;
using System.Collections;

public class Checkpoint : CommonObject {

	void OnTriggerEnter2D(Collider2D col) { 

		if (col.name == "MainPlayer") {

			MainLogic.mainLogic.SaveGameCheckPoint (this.gameObject);
		}
	}
}
