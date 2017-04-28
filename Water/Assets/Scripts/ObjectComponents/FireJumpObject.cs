using UnityEngine;
using System.Collections;

public class FireJumpObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void DestroySelf() {

		Destroy (this.gameObject);
	}
}
