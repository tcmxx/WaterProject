using UnityEngine;
using System.Collections;

public class CameraFollowPlayer : MonoBehaviour {

    public GameObject target;
	public static CameraFollowPlayer cameraFollowPlayer;
    private float moveSpeed = 3.5f;
	private float scaleSpeed = 1f;
    public float yOffset = 2;
	public float xOffset = 0;

	public float iniZOffset = -13f;
	public float farZOffset = 10f;
	public float desZOffset{get{ return desZOffsetField;}}
	private float desZOffsetField;
	Camera curCamera;

	void Awake(){
		cameraFollowPlayer = this;
	}

	void Start(){
		curCamera = GetComponent <Camera> ();
		desZOffsetField = iniZOffset;
	}

	// Update is called once per frame
	void FixedUpdate () {

		transform.position = Vector3.Lerp(transform.position, new Vector3(target.transform.position.x + xOffset, target.transform.position.y + yOffset, target.transform.position.z + desZOffset), Time.deltaTime * moveSpeed);

	}

	public void UseFarView(){
		ChangeViewSize (farZOffset);
	}

	public void UseInitialView(){
		ChangeViewSize (iniZOffset);
	}

	public void ChangeViewSize(float size){
		desZOffsetField = size;
	}

}
