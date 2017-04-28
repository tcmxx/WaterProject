using UnityEngine;
using System.Collections;


public class GenericAbility : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	/// <summary>
	/// Call when holding the attack button
	/// </summary>
	/// <returns></returns>
	public virtual bool Hold(){
		return false;
	}


	public virtual void EnableMode(){
	}
	public virtual void DisableMode(){
	}

	public virtual void GetMainPlayer(MainPlayer mainPlayerSC){
		
	}

	public virtual bool Interact(CharacterInfo.ATTACK_MODE attackMode, CommonObject.InteractKey interactKey, CommonObject.InteractDirectionKey direction = CommonObject.InteractDirectionKey.None){
		return false;
	}
}
