using UnityEngine;
using System.Collections;
using Water2DTool;



/// <summary>
/// Attachable object is a type of object that the player can attach to. When the player touches the object, the player will stop normal movement and use the interface
/// of the attachable object for special movement, like spiral, rope and others.
/// </summary>
public class AttachableObject : MonoBehaviour {

	public virtual void DoUpdate (MainPlayer playerRef) {
	
	}


	public virtual void OnTriggerEnter2D(Collider2D col)
	{
		MainPlayer player = col.GetComponent <MainPlayer> ();
		if(player != null)
		{
			player.Attach (this);
		}
	}

	public virtual void OnTriggerExit2D(Collider2D col)
	{
		MainPlayer player = col.GetComponent <MainPlayer> ();
		if(player != null )
		{
			player.Detach (this);
		}
	}


	public virtual void Move (float horizontal, MainPlayer playerRef){
	}

	public virtual void Jump (bool start, bool push, MainPlayer playerRef){
	}

	public virtual void Interact (MainPlayer playerRef, 
		CharacterInfo.ATTACK_MODE attackMode, 
		CommonObject.InteractKey interactKey, 
		CommonObject.InteractDirectionKey direction = CommonObject.InteractDirectionKey.None){
	}
	

}
