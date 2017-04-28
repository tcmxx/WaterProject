using UnityEngine;
using System.Collections;

public class Hook : ActionObject {

    public GameObject ropePrefab;
	public GameObject floor;
	private GameObject ropeObj;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void Action()
    {
        base.Action();

		if (ropeObj == null) {
			
			ropeObj = (GameObject)Instantiate (ropePrefab, this.transform.position, Quaternion.identity);
			ropeObj.transform.FindChild ("Tip").GetComponent<RopeSegments> ().willDestroy = true;

		} else {
			StartCoroutine (Fade ());
		}
    }

	IEnumerator Fade() {

		MainPlayer.mainPlayer.gameObject.GetComponent<Rigidbody2D> ().gravityScale = 0;
		MainPlayer.mainPlayer.gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
		floor.GetComponent<Collider2D> ().enabled = false;

        MainPlayer.mainPlayer.transform.position = new Vector2(transform.position.x, MainPlayer.mainPlayer.transform.position.y);


        for (int i = 1; i <= 5; i++) {

			MainPlayer.mainPlayer.GetComponent<Rigidbody2D> ().velocity = Vector3.down * 4;
			if (MainPlayer.mainPlayer.attachableObject == ropeObj) break;
			yield return new WaitForSeconds (.1f);
		}
		MainPlayer.mainPlayer.GetComponent<Rigidbody2D> ().velocity = Vector3.zero;
		MainPlayer.mainPlayer.gameObject.GetComponent<Rigidbody2D> ().gravityScale = 1;
		MainPlayer.mainPlayer.gameObject.GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.None;
		floor.GetComponent<Collider2D> ().enabled = true;

	}
}
