using UnityEngine;
using System.Collections;

public class CameraZoomScript : MonoBehaviour {

	private CircleCollider2D playerCollider;
	private Camera myCamera1;
    private Camera myCamera2;
  



    public float cameraRange = 80;
	public float cameraZoomSpeed = 1;

	public float cameraYOffset = 2;
	public float cameraXOffset = 0;

	private float initialValue;
	private float newFoV = 0;

	// Use this for initialization
	void Start () {
		playerCollider = GameObject.Find ("MainPlayer").GetComponent<CircleCollider2D> ();
		myCamera1 = GameObject.Find ("Camera_main").GetComponent<Camera>();
        myCamera2 = GameObject.Find("Camera_blur1").GetComponent<Camera>();


        newFoV = myCamera1.fieldOfView;
		initialValue = myCamera1.fieldOfView;
	}
	
	// Update is called once per frame
	void Update () {
		myCamera1.fieldOfView = Mathf.Lerp (myCamera1.fieldOfView, newFoV, cameraZoomSpeed * Time.deltaTime);
        myCamera2.fieldOfView = Mathf.Lerp(myCamera2.fieldOfView, newFoV, cameraZoomSpeed * Time.deltaTime);



    }

	void OnTriggerEnter2D(Collider2D col) {
		
		if (col.gameObject.name == "MainPlayer") {			
			newFoV = cameraRange;
			myCamera1.GetComponent<CameraFollowPlayer> ().yOffset = cameraYOffset;
			myCamera1.GetComponent<CameraFollowPlayer> ().xOffset = cameraXOffset;

            myCamera2.GetComponent<CameraFollowPlayer>().yOffset = cameraYOffset;
            myCamera2.GetComponent<CameraFollowPlayer>().xOffset = cameraXOffset;

        }
	}

    void OnTriggerStay2D(Collider2D col)
    {

        if (col.gameObject.name == "MainPlayer")
        {
            newFoV = cameraRange;
            myCamera1.GetComponent<CameraFollowPlayer>().yOffset = cameraYOffset;
            myCamera1.GetComponent<CameraFollowPlayer>().xOffset = cameraXOffset;

            myCamera2.GetComponent<CameraFollowPlayer>().yOffset = cameraYOffset;
            myCamera2.GetComponent<CameraFollowPlayer>().xOffset = cameraXOffset;

  
        }
    }

    void OnTriggerExit2D(Collider2D col) {

		if (col.gameObject.name == "MainPlayer") {
			newFoV = initialValue;
			myCamera1.GetComponent<CameraFollowPlayer> ().yOffset = 2;
			myCamera1.GetComponent<CameraFollowPlayer> ().xOffset = 0;

            myCamera2.GetComponent<CameraFollowPlayer>().yOffset = 2;
            myCamera2.GetComponent<CameraFollowPlayer>().xOffset = 0;
            
        }
	}
}




