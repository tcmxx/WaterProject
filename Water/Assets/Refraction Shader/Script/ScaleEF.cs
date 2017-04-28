using UnityEngine;
using System.Collections;

public class ScaleEF : MonoBehaviour {

	public	float			scale = 3.0f;
	public	float			speed = 0.01f;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.localScale.x < scale){
			transform.localScale += new Vector3(speed,speed,speed);
		}else{
			Destroy(gameObject);
		}
	}
}
