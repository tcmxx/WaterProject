using UnityEngine;
using System.Collections;

public class ActionObject : CommonObject {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public virtual void OnTriggerEnter2D(Collider2D col)
    {
		MainPlayer player = col.GetComponent <MainPlayer> ();
		if(player != null)
		{
			player.curActionObject = this;
		}
    }

	public virtual void OnTriggerExit2D(Collider2D col)
    {
		MainPlayer player = col.GetComponent <MainPlayer> ();
		if(player != null && player.curActionObject == this)
        {
			player.curActionObject = null;
        }
    }




    virtual public void Action()
    {

    }

}
