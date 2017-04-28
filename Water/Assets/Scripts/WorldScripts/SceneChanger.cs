using UnityEngine;
using System.Collections;

public class SceneChanger : MonoBehaviour {

	public string goTo;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col) {
		if (col.gameObject.name == "MainPlayer") {
			Application.LoadLevel (goTo);
		}
	}
}
