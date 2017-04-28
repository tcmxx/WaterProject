using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookColliders : MonoBehaviour {

	public GameObject rightHand;
	public GameObject leftHand;
	public GameObject rightCol;
	public GameObject leftCol;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		rightCol.transform.position = rightHand.transform.position;
		leftCol.transform.position = leftHand.transform.position;
	}
}
