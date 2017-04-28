using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RopeSegments : AttachableObject {

    #region Vars

    public bool isAttachable = false;
    //public float motorSpeed = 10f;
	public bool willDestroy = false;

    public float attachRadius = 0.35f;

	public float timeNotAttachableAfterDetach =  0.5f;

    private bool attached = false;

	/// <summary>
	/// List of players that can not be attached to the rope right now. The float value is the counting down time after which the player will be removed 
	/// from the list
	/// </summary>
	private Dictionary<MainPlayer,float> notAttachablePlayerList;
    #endregion


    void Start () {
		notAttachablePlayerList = new Dictionary<MainPlayer,float> ();
	}

	void Update () {
		List<MainPlayer> keys = new List<MainPlayer> (notAttachablePlayerList.Keys);
		foreach (var p in keys) {
			notAttachablePlayerList[p] -= Time.deltaTime;
			if (notAttachablePlayerList[p] <= 0) {
				notAttachablePlayerList.Remove (p);
			}
		}
    }



    public override void OnTriggerEnter2D(Collider2D col)
    {

		MainPlayer player = col.GetComponent <MainPlayer> ();
		if(player != null && !attached && isAttachable/* && CheckWithinRange(col) */ && !notAttachablePlayerList.ContainsKey(player) && player.canAttach)
		{
			if(player.Attach (this)){
				attachPlayer(col,player);
			}
		}

    }

	public override void OnTriggerExit2D(Collider2D col)
	{
		//do not call the base one
	}

    //Check if the player is at the position where the head of the player is close enough to the rope tip
    bool CheckWithinRange(Collider2D playerCollider)
    {
        Transform playerTransform = playerCollider.transform;
        Vector2 headPosLocal = new Vector2(0f, 0.5f);

        Vector2 headPosGlobal = playerTransform.localToWorldMatrix.MultiplyPoint(headPosLocal);
        Vector2 tipPosGlobal = transform.position;

        float dist = (headPosGlobal - tipPosGlobal).magnitude;

        if(dist < attachRadius)//of the rope tip is within a range from the player head tip
        {
            return true;
        }
        else
        {
            return false;
        }

        
    }


	void attachPlayer(Collider2D col, MainPlayer playerRef)
    {
		

        float rotation = transform.rotation.eulerAngles.z;
		Quaternion playerRot = playerRef.transform.rotation;
        Vector3 playerRotEuler = playerRot.eulerAngles;
        playerRotEuler.z = rotation;
        playerRot.eulerAngles = playerRotEuler;
		playerRef.transform.rotation = playerRot;
		playerRef.allowedJumpTimes = 2;


        HingeJoint2D HJ2D = col.gameObject.AddComponent<HingeJoint2D>();
        HJ2D.connectedBody = this.gameObject.GetComponent<Rigidbody2D>();

        HJ2D.autoConfigureConnectedAnchor = false;
        HJ2D.connectedAnchor = new Vector2(0f, -0.5f);
        HJ2D.anchor = new Vector2(0f, 0.5f);

        JointAngleLimits2D tmp = new JointAngleLimits2D();
        tmp.max = 10;
        tmp.min = -10;
        HJ2D.limits = tmp;


        col.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;

        attached = true;
    }



	public override void Move (float horizontal, MainPlayer playerRef){

		playerRef.GetComponent<Rigidbody2D>().AddForce(Vector2.right* horizontal*5);
		return;
	}

	public override void Jump (bool start, bool push, MainPlayer playerRef){
		if (!(start && push)) {
			return;
		}

		Destroy(playerRef.GetComponent<HingeJoint2D>());
		playerRef.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		playerRef.transform.rotation = Quaternion.identity;

		if (willDestroy)
		{
			Destroy(transform.parent.gameObject);
		}
		attached = false;
		playerRef.canAttach = false;
		StartCoroutine (AttachTimer (playerRef));

		notAttachablePlayerList [playerRef] = timeNotAttachableAfterDetach;

		playerRef.Detach (this);
		playerRef.allowedJumpTimes += 1;
		playerRef.Jump (start, push);
	}

	public override void Interact (MainPlayer playerRef, 
		CharacterInfo.ATTACK_MODE attackMode, 
		CommonObject.InteractKey interactKey, 
		CommonObject.InteractDirectionKey direction = CommonObject.InteractDirectionKey.None){
	}

	IEnumerator AttachTimer(MainPlayer player){

		yield return new WaitForSeconds (1);
		player.canAttach = true;
	}
}
