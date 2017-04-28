using UnityEngine;
using System.Collections;

public class WaterSpinUpObject : CommonObject {

	private bool isFreeze = false;
	bool starting = true;
	public Vector2 firstPos;
	bool move = true;
	public float height = 2;
	public float upSpeed = 2;
	public float offsetY = -0.3f;
    
    public GameObject myTrailObj;
    private TrailRenderer myTrail;

    public GameObject myTrailObj2;
    private TrailRenderer myTrail2;

    private bool eraseTrail = false;


	void Start () {

        myTrail = myTrailObj.GetComponent<TrailRenderer>();
        myTrail2 = myTrailObj2.GetComponent<TrailRenderer>();
		firstPos = new Vector2 (this.transform.position.x, this.transform.position.y + offsetY);
		this.transform.position = firstPos;
	}
	
	// Update is called once per frame
	void Update () {
		if(starting)
		{

			if (move) {
				transform.position = Vector2.MoveTowards (this.transform.position, new Vector2 (this.transform.position.x, firstPos.y + height), upSpeed * Time.deltaTime);
				MainPlayer.mainPlayer.transform.position = new Vector2(transform.position.x, transform.position.y + 1.3f);
			}

			if (Input.GetButtonDown("Jump"))
			{
				DestroyObject ();
				MainPlayer.mainPlayer.Jump(true, true);

			}
		}

        if(eraseTrail)
        {
            eraseMyTrail();
        }
	}


	public void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Player")
		{
			//starting = true;
			MainPlayer.mainPlayer.GetComponent <Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;;

			MainPlayer.mainPlayer.doRunAnim = false;
		}
			
	}
		
	//Destroy the object when the event on the animation is called
	void DestroyObject() 
	{
		leaveWaterSpinUp ();
		Destroy (this.gameObject, 1);
	}

	public override bool IsHighlightable(){

		return !isFreeze;
	}

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){

		if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Special )
		{
			isFreeze = true;
		}
		return false;
	}

	void leaveWaterSpinUp(){
		MainPlayer.mainPlayer.characterInfo.isOnWaterSpinUp = false;
		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), MainPlayer.mainPlayer.GetComponent<CircleCollider2D> (), true);
		Physics2D.IgnoreCollision (GetComponent<BoxCollider2D> (), MainPlayer.mainPlayer.GetComponent<PolygonCollider2D> (), true);
		MainPlayer.mainPlayer.GetComponent <Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		MainPlayer.mainPlayer.doRunAnim = true;

        eraseTrail = true;

       

        starting = false;
	}

    private void eraseMyTrail()
    {

        if (myTrail.startWidth >= 0)
        {
            myTrail.startWidth -= .025f;
        }

        if (myTrail.endWidth >= 0)
        {
            myTrail.endWidth -= .05f;
        }

        if (myTrail2.startWidth >= 0)
        {
            myTrail2.startWidth -= .025f;
        }

        if (myTrail2.endWidth >= 0)
        {
            myTrail2.endWidth -= .025f;
        }


        if(myTrail.startWidth < .0025f)
        {
            myTrail.startWidth = 0;
           
        }
        if (myTrail.endWidth < .0025f)
        {
            myTrail.endWidth = 0;

        }



        if (myTrail2.startWidth < .0025f)
        {
            myTrail2.startWidth = 0;

        }
        if (myTrail2.endWidth < .0025f)
        {
            myTrail2.endWidth = 0;

        }




    }

}
