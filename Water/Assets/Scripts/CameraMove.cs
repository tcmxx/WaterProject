using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour {

    public float xDirection;
    public float yDirection;
    public float zDirection;

    public float speed;
	
	// Update is called once per frame
	void LateUpdate () {

        transform.Translate(new Vector3(xDirection * speed * Time.deltaTime, yDirection * speed * Time.deltaTime, zDirection * speed * Time.deltaTime));
		
	}
}
