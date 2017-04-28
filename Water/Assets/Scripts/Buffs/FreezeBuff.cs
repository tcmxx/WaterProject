using UnityEngine;
using System.Collections;

public class FreezeBuff : GenericBuff {

	public Color EffectColor;

	private RigidbodyConstraints2D rbConstraint;
	private Color originalColor;

	// Use this for initialization
	void Start () {

	}



	//this will be call in Update function if the timer is still running
	public override void Effect(){

	}


	public override void OnFirstAttach(){
		if (transform.parent) {
			Renderer rend = transform.parent.gameObject.GetComponent<Renderer> ();
			if (rend) {
				originalColor = rend.material.color;
				rend.material.color = EffectColor;
			}

			Rigidbody2D rb = parentCommonObject.GetComponent <Rigidbody2D> ();
			if (rb != null) {
				rbConstraint = rb.constraints;
				rb.constraints = rbConstraint | RigidbodyConstraints2D.FreezePosition;
			}
		}
	}

	public override void OnBuffEnd(){
		if (transform.parent) {
			Renderer rend = transform.parent.gameObject.GetComponent<Renderer> ();
			if (rend) {
				rend.material.color = originalColor;
			}
			Rigidbody2D rb = parentCommonObject.GetComponent <Rigidbody2D> ();
			if (rb != null) {
				rb.constraints = rbConstraint;
			}
		}
	}


	public override bool IsHighlightable(){
		return true;
	}

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){
		if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Special) {


			return true;
		}
		return false;
	}
}
