using UnityEngine;
using System.Collections;

public class WaterWallScript : CommonObject {

	#region Variables
	//Public Vars

	//Private Vars
	public bool freezed = false;
	public float playerWallDist = 5;
	bool move = true;
	public Vector3 startPos;
	public float height = 2;
	public float upSpeed = 5;
	public float normalTimeAlive = 5;
	public float freezedTimeAlive = 10;
	public float destroyTime = 2;
	public GameObject bottomObj;
	private GameObject bottomObjInst;
	public GameObject topObj;
	private LineRenderer myLine;
	public float yOffset = .7f;
	public Material frozenMat;
	public Material frozenTexture;
	private Material temp1;
	private Material temp2;
	private Material[] mats;
    //SF Vars

    #endregion

    #region Start Function
    //Start Function
    void Start () {

		myLine = GetComponent<LineRenderer> ();

		mats = myLine.materials;

		temp1 = mats [0];
		temp2 = mats [1];

		startPos = new Vector3(transform.position.x, transform.position.y + yOffset, 0.5f);
		transform.position = startPos;

		bottomObjInst = (GameObject)Instantiate (bottomObj, startPos, Quaternion.identity);
		GetComponent<Rigidbody2D> ().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;;
	}
	#endregion

	#region Update Function
	// Update Function
	void Update () {

		myLine.SetPosition (0, bottomObjInst.transform.position); 
		myLine.SetPosition (1, topObj.transform.position);

		if (move) {

			transform.position = Vector2.MoveTowards (this.transform.position, new Vector3 (startPos.x, startPos.y + height, startPos.z), upSpeed * Time.deltaTime);
			WallUpMovement ();

			float dist = Vector2.Distance (this.transform.position, new Vector3 (startPos.x, startPos.y + height, startPos.z));
			if (dist <= .1f) {

				move = false;

				StartCoroutine (Disappear());

			}
		}
	}
	#endregion

	IEnumerator Disappear() {



		yield return new WaitForSeconds (normalTimeAlive);
		if (!freezed) {

			DestroyObj ();
		}

		yield return new WaitForSeconds (freezedTimeAlive);
		DestroyObj ();
	}

	void DestroyObj()  {

		GetComponent<BoxCollider2D> ().enabled = false;
		GetComponent<Rigidbody2D> ().isKinematic = false;

		print ("UNFREEZE");
		mats [0] = temp1;
		mats [1] = temp2;
		myLine.materials = mats;

		Destroy (this.gameObject, destroyTime);
		Destroy (bottomObjInst, destroyTime);
	}

    #region Other Override Methods
	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None)
    {
		if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Special && move == false)
        {
			FreezeWall ();
			freezed = true;
			return true;
        }
		return false;
    }

    public override bool IsHighlightable()
    {
		float dist = Vector2.Distance (this.gameObject.transform.position, MainPlayer.mainPlayer.gameObject.transform.position);

		if (dist <= playerWallDist)
			return true;
		else
			return false;
		//return !freezed;
    }
    #endregion

	void WallUpMovement() {

		//Something here , Good Luck :P

	}

	void FreezeWall() {	

		mats [0] = frozenTexture;
		mats [1] = frozenMat;
		myLine.materials = mats;

		//myLine.materials[1] = frozenMat;
		GetComponent<BoxCollider2D> ().enabled = true;
	}

}
