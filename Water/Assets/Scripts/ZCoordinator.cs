using UnityEngine;
using System.Collections;


/// <summary>
/// This script will change the Z coordinate of objects that collided with it based on position
/// </summary>
public class ZCoordinator : MonoBehaviour {


	private float YRotation;

	// Use this for initialization
	void Start () {
		YRotation = transform.rotation.eulerAngles.y;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



	void OnTriggerStay2D(Collider2D col)
	{
		CommonObject obj = col.gameObject.GetComponent <CommonObject> ();
		if (obj != null && obj.zVariable)
		{
			float z = (transform.position.x - col.transform.position.x) * Mathf.Tan (YRotation/180f*Mathf.PI) + transform.position.z;
			Vector3 tempPos = col.transform.position;
			tempPos.z = z;
			col.transform.position = tempPos;
		}
	}
}
