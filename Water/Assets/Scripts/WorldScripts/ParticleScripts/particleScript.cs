using UnityEngine;
using System.Collections;

public class particleScript : MonoBehaviour {

	public float pullRadius = 2;
	public float pullForce = 1000;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void FixedUpdate() {
		foreach (Collider collider in Physics.OverlapSphere(transform.position, pullRadius))
			{
			// calculate direction from target to me
			Vector3 forceDirection = transform.position - collider.transform.position;

			// apply force on target towards me
			collider.GetComponent<Rigidbody>().AddForce(forceDirection.normalized * pullForce * Time.fixedDeltaTime);
			}
		}



}
