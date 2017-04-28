using UnityEngine;
using System.Collections;

public class playerSpawnLocation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameObject.Find ("MainPlayer").transform.position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
