using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpiralObject : AttachableObject 
{
	#region Variables
	private float orbitDegreesPerSec = 360.0f;

	private float thisHeight = 0f;


    //dynamics when leaving the spiral object
    private float initVelX = 15f;
    private float initVelY = 15f;
	private float initVelYBottom = 4f;
    private float rotationAngleError = 20f;

	Dictionary<MainPlayer, PlayerDataSet> attachedPlayers = new Dictionary<MainPlayer, PlayerDataSet>();


    #endregion

    #region Start Method
    void Start () {
	
		thisHeight = GetComponent<BoxCollider2D> ().bounds.size.y;

	}
	#endregion

	#region Update Method
	public override void DoUpdate(MainPlayer player)
	{
        
		Transform model = player.transform.FindChild ("Model");
		PlayerDataSet dataset = attachedPlayers [player];

		if (dataset.isActive)
		{
			Orbit(player);
        }
		float curModelAngle = model.rotation.eulerAngles.y;
		if (dataset.isReadyToGo)
        {
            //cross the 180 or 360 degree. Should release the player
            if (Mathf.Abs(curModelAngle - 0) < rotationAngleError || Mathf.Abs(curModelAngle - 360) < rotationAngleError) {
				release(false, player);
            }
            else if(Mathf.Abs(curModelAngle - 180) < rotationAngleError)
            {
				release(true, player);
            }
        }

        

		dataset.prevModelAngle = model.rotation.eulerAngles.y;
		dataset.prevPlayerPositoin = player.transform.position;

		attachedPlayers [player] = dataset;
    }
	#endregion



	#region Orbit Method
	void Orbit(MainPlayer player)
	{
		Transform model = player.transform.FindChild ("Model");
		PlayerDataSet dataset = attachedPlayers [player];

		if (dataset.isOrbiting)
        {
            //rotates
			model.RotateAround(new Vector2(transform.position.x, player.transform.position.y), Vector2.up, orbitDegreesPerSec * Time.deltaTime*(dataset.isReadyToGo?2:1));
        }

		if (player.transform.position.y <= dataset.dLimit || dataset.isAtBottom)
        {
			
			dataset.isAtBottom = true;
			Rigidbody2D RB = player.gameObject.GetComponent<Rigidbody2D>();
            RB.velocity = new Vector2(0f, 0f);
			player.transform.position = dataset.prevPlayerPositoin;

            //stop rotatingwhen the player reach right or left side of the object
			float curModelAngle =model.rotation.eulerAngles.y;

			if (dataset.isAtBottom && ((curModelAngle < 90 && dataset.prevModelAngle > 270) || (curModelAngle > 180 && dataset.prevModelAngle < 180)))
            {
				dataset.isOrbiting = false;
            }
        }
        else
        {
			dataset.isAtBottom = false;
			Rigidbody2D RB = player.gameObject.GetComponent<Rigidbody2D>();
            RB.velocity = new Vector2(0f, -1f);

        }

		attachedPlayers [player] = dataset;
        
    }
	#endregion

	#region Other Methods
    //the player leaves the spiral object
	void release(bool toRight, MainPlayer playerRef)
	{
		
		playerRef.TurnAround(toRight);

		if (attachedPlayers[playerRef].isAtBottom) {
			Rigidbody2D RB = playerRef.gameObject.GetComponent<Rigidbody2D> ();
            RB.velocity = Vector2.up * initVelY + Vector2.right * (toRight ? 1 : -1) * initVelX;
        } else {

			Rigidbody2D RB = playerRef.gameObject.GetComponent<Rigidbody2D> ();
            RB.velocity = Vector2.up * initVelYBottom + Vector2.right * (toRight ? 1 : -1) * initVelX;
        }

        //reset the position of the 3d model and game object
		Vector3 tmpPosition = playerRef.transform.FindChild("Model").position;
		playerRef.transform.position = tmpPosition;
		playerRef.transform.FindChild("Model").position = tmpPosition;

		//remove the player from the list
		attachedPlayers.Remove (playerRef);
    }
	#endregion



	public override void OnTriggerEnter2D(Collider2D col)
	{
		MainPlayer player = col.GetComponent <MainPlayer> ();
		if(player != null)
		{
			player.Attach (this);

			PlayerDataSet dataSet = new PlayerDataSet();

			dataSet.playerHeight = player.GetComponent<CapsuleCollider2D> ().bounds.size.y;
			dataSet.dLimit = ( ( transform.position.y - ( thisHeight / 2 ) ) + ( dataSet.playerHeight / 2 ) ) ;

			if (player.transform.position.y >= dataSet.dLimit) 
			{
				Vector3 tmpPosition = player.transform.position;
				player.transform.position = tmpPosition;
				dataSet.isOrbiting = true;
				dataSet.isReadyToGo = false;
				dataSet.isActive = true;
			}
			attachedPlayers [player] = dataSet;

		}
	}

	public override void OnTriggerExit2D(Collider2D col)
	{
		MainPlayer player = col.GetComponent <MainPlayer> ();
		if(player != null )
		{
			player.Detach (this);
		}
	}



	public override void Jump (bool start, bool push, MainPlayer playerRef){
		if (!(start && push)) {
			return;
		}
		playerRef.anim.SetTrigger("Jump");
		playerRef.allowedJumpTimes = 1;
		PlayerDataSet dataset = attachedPlayers [playerRef];
		dataset.isReadyToGo = true;
		attachedPlayers [playerRef] = dataset;
	}


	struct PlayerDataSet{
		public float prevModelAngle;
		public Vector3 prevPlayerPositoin;
		public bool isOrbiting;
		public bool isActive; //if the player is in this spiral object, this will be true;
		public bool isReadyToGo;
		public float playerHeight;
		public bool isAtBottom;
		public float dLimit;
	}



}
