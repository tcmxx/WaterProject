using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class BurningBuff : GenericBuff {

	public int damagePerSec = 5;
	public int explodeDamate = 20;
	int curCount = 0;

	void Start() {

	}
	
	// Update is called once per frame
	void Update () {
		DoUpdate ();
	}


	//this will be call in Update function if the timer is still running
	public override void Effect(){
		if (timer > curCount + 1) {
			if (parentCommonObject != null) {
				parentCommonObject.TakeDamage (damagePerSec);
				curCount++;
			}
		}
	}

	//reset the buff. If when trying to attach the buff to the object and the same buff already exist, will call this function automatically
	public override void Reset(){
		base.Reset ();
		curCount = 0;
	}
		
	public override bool IsHighlightable(){
		return true;
	}

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){
		if (attackMode == CharacterInfo.ATTACK_MODE.FIRE && interactKey == InteractKey.Special) {

			Explode ();

			return true;
		}
		return false;
	}


	private void Explode(){
		damagePerSec = 0;
		transform.transform.SetParent (null);
		parentCommonObject.TakeDamage (explodeDamate);
		transform.FindChild ("Explosion").gameObject.SetActive (true);
		transform.FindChild ("Fire_01").gameObject.SetActive (false);
		Invoke ("SelfDestroy",1);
	}

	private void SelfDestroy(){
		Destroy (this.gameObject);
	}

}
