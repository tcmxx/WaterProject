using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class FreezableWater : CommonObject {


    #region Variables
    //Public Vars
    public GameObject waterp;
    public GameObject waterWallp;
	public GameObject waterSpinPrefab;
    public float waterYsize;
	public MainPlayer mainPlayerScript;
	public bool isFrozen = false;
	public float minDistWall = 2;

    //private vars
    private Animator anim;
    private BoxCollider2D iceCollider;
    private GameObject unfreezedWater;
    private GameObject waterWallObject = null;
    #endregion

    // Use this for initialization
    #region Start Callback
    void Start () {
        anim = transform.GetChild(1).GetComponent<Animator>();
        iceCollider = transform.FindChild("freezedWater").GetComponent<BoxCollider2D>();
        unfreezedWater = transform.Find("unfreezedWater").gameObject;
		Physics2D.IgnoreCollision (GetComponent <BoxCollider2D>(), transform.FindChild ("unfreezedWater").GetComponent <BoxCollider2D>());
    }
    #endregion

    // Update is called once per frame
    #region Start Update
    void Update () {
        if (iceCollider.isActiveAndEnabled)
        {
            unfreezedWater.SetActive(false);
        }
        else
        {
            unfreezedWater.SetActive(true);
        }
    }
    #endregion

    #region Other methods

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction)
    {
		if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Special)
        {
            if (anim.GetBool("freezing"))
            {
                anim.SetBool("freezing", false);
				isFrozen = false;
            }
            else
            {
                anim.SetBool("freezing", true);
				isFrozen = true;
            }
			return true;
		}else if(attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Attack && direction == InteractDirectionKey.Up)
        {
			if (isInWater) { WaterSpinUp (); } else { WaterWall(); }
            
			return true;
		}else if(attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Attack  && direction == InteractDirectionKey.None )
        {
            //Instantiate the Water
			if (((WaterAbility)MainPlayer.mainPlayer.waterAbility).waterBolt == null) 
			{
				GameObject h = (GameObject)Instantiate (waterp, this.transform.position, Quaternion.identity);
				h.GetComponent<WaterBoltObject> ().mainPlayerScript = mainPlayerScript;
				((WaterAbility)MainPlayer.mainPlayer.waterAbility).waterBolt = h.GetComponent <WaterBoltObject> ();
			}
			return true;
        }

		return false;
    }


	void WaterSpinUp() {

		Vector2 waterSpinPos = new Vector2 (MainPlayer.mainPlayer.transform.position.x, MainPlayer.mainPlayer.transform.position.y -1.3f);

		GameObject h = (GameObject)Instantiate (waterSpinPrefab, waterSpinPos, Quaternion.identity);
		h.transform.position = new Vector3 (h.transform.position.x, h.transform.position.y, 0.5f);

		MainPlayer.mainPlayer.characterInfo.isOnWaterSpinUp = true;

	}
    //private method. Create a water wall
    private void WaterWall()
    {

		bool isRight = MainPlayer.mainPlayer.generalMovementControl.movingRight;

		float wallColliderHeight = waterWallp.GetComponent <BoxCollider2D>().size.y;
		float waterWallScaleHeight = waterWallp.transform.lossyScale.y;

		Collider2D waterCol = this.GetComponent<BoxCollider2D> ();
		float waterHeight = waterCol.bounds.size.y;
		float waterScaleHeight = this.transform.lossyScale.y;


		float waterPondBondRight = transform.position.x + waterCol.bounds.size.x / 2 +1;
		float waterPondBondLeft = transform.position.x - waterCol.bounds.size.x / 2 -1;


        if (isRight && waterPondBondRight > MainPlayer.mainPlayer.transform.position.x && IsHighlightable())
        {
			//waterWallObject = (GameObject)Instantiate(waterWallp, new Vector2(Mathf.Clamp (MainPlayer.mainPlayer.transform.position.x + 3,waterPondBondLeft, waterPondBondRight), 
			//		waterCol.gameObject.transform.position.y + waterHeight*waterScaleHeight / 2 + wallColliderHeight * waterWallScaleHeight/2 + waterCol.offset.y), Quaternion.identity);
			waterWallObject = (GameObject)Instantiate (waterWallp, new Vector2 (Mathf.Clamp (MainPlayer.mainPlayer.transform.position.x + minDistWall, waterPondBondLeft, waterPondBondRight), waterCol.gameObject.transform.position.y + (waterHeight * waterScaleHeight) / 2 + waterCol.offset.y), Quaternion.identity);

			waterWallObject.transform.position = new Vector3 (waterWallObject.transform.position.x, waterWallObject.transform.position.y, 0.5f);

        }
        else if (!isRight && waterPondBondLeft < MainPlayer.mainPlayer.transform.position.x && IsHighlightable())
        {
			//waterWallObject = (GameObject)Instantiate(waterWallp, new Vector2(Mathf.Clamp (MainPlayer.mainPlayer.transform.position.x - 3,waterPondBondLeft, waterPondBondRight), 
			//		waterCol.gameObject.transform.position.y + waterHeight*waterScaleHeight / 2 + wallColliderHeight * waterWallScaleHeight/2 + waterCol.offset.y), Quaternion.identity);
			waterWallObject = (GameObject)Instantiate (waterWallp, new Vector2 (Mathf.Clamp (MainPlayer.mainPlayer.transform.position.x - minDistWall, waterPondBondLeft, waterPondBondRight), waterCol.gameObject.transform.position.y + (waterHeight * waterScaleHeight) / 2 + waterCol.offset.y), Quaternion.identity);


			waterWallObject.transform.position = new Vector3 (waterWallObject.transform.position.x, waterWallObject.transform.position.y, 0.5f);
        }
        


    }

	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.CompareTag ("Player"))
			MainPlayer.mainPlayer.anim.SetBool ("isSwimming", true);

		CommonObject obj = col.gameObject.GetComponent <CommonObject> ();
		if (obj != null) {
			obj.isInWater = true;
		}
		
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if(col.CompareTag ("Player"))
			MainPlayer.mainPlayer.anim.SetBool ("isSwimming", false);

		CommonObject obj = col.gameObject.GetComponent <CommonObject> ();
		if (obj != null) {
			obj.isInWater = false;
		}
		
	}

	void OnTriggerStay2D(Collider2D col)
	{
		MainPlayer mainplayer = col.GetComponent <MainPlayer> ();

		if (mainplayer != null && MainPlayer.mainPlayer.isInWater) {
			if (mainplayer.GetComponent <Rigidbody2D> ().velocity.y < 0) {
				mainplayer.anim.SetBool ("isSwimming", true);
			}
		}

		if (mainplayer != null && mainplayer.anim.GetBool ("isSwimming"))
		{
            BoxCollider2D waterCollider = unfreezedWater.GetComponent<BoxCollider2D>();
			float playerHeight = 0.5f;
            float volume = (waterCollider.size.y / 2 + waterCollider.offset.y) * unfreezedWater.transform.lossyScale.y + unfreezedWater.transform.position.y -
				(col.transform.position.y -(playerHeight / 2  +playerHeight)* col.transform.lossyScale.y ) -0.8f;

            if (volume < 0)
            {
                volume = 0f;
            }else if(volume > col.transform.lossyScale.y)
            {
                volume = col.transform.lossyScale.y;
            }
			Rigidbody2D RB = col.gameObject.GetComponent<Rigidbody2D> ();
			RB.AddForce (-volume * Physics2D.gravity*35f);
			RB.AddForce (RB.velocity.normalized * RB.velocity.magnitude * RB.velocity.magnitude * -1*10);
		}
	}

    public override bool IsHighlightable()
    {
		return true;

    }
    #endregion
}
