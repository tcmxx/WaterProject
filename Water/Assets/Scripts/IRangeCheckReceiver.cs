using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRangeCheckReceiver {

	void OnRangeEnter (Collider2D col);
	void OnRangeStay (Collider2D col);
	void OnRangeExit (Collider2D col);
}
