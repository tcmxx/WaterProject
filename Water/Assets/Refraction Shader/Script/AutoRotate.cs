using UnityEngine;
using System.Collections;

public class AutoRotate : MonoBehaviour {

	public	float 		speed = 2;
	public	Vector3		direction = Vector3.up;

	void Update () {
		transform.Rotate(direction * speed * Time.deltaTime);
	}
}
