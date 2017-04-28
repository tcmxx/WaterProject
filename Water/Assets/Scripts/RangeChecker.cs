using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeChecker : MonoBehaviour {

	public GameObject[] receiversToInvoke;

	List<IRangeCheckReceiver> receivers;

	void Awake(){
		receivers = new List<IRangeCheckReceiver> ();
		foreach (var obj in receiversToInvoke) {
			receivers.AddRange (obj.GetComponents <IRangeCheckReceiver>());
		}
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		foreach (var obj in receivers) {
			obj.OnRangeEnter (col);
		}
	}
	void OnTriggerStay2D(Collider2D col)
	{
		foreach (var obj in receivers) {
			obj.OnRangeStay (col);
		}
	}
	void OnTriggerExit2D(Collider2D col)
	{
		foreach (var obj in receivers) {
			obj.OnRangeExit (col);
		}
	}


}
