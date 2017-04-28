using UnityEngine;
using System.Collections;

public class WaterWaveObject2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		GetComponentInChildren<WaterWaveObject> ().CollisionEnter2D (col);
	}
}
