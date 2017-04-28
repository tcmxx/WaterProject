using UnityEngine;
using System.Collections;

public class sortParticles : MonoBehaviour {

	public int orderNumber;

	// Use this for initialization
	void Start () {
		GetComponent<ParticleSystem> ().GetComponent<Renderer>().sortingLayerName = "Particles";
		GetComponent<ParticleSystem> ().GetComponent<Renderer> ().sortingOrder = orderNumber;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
