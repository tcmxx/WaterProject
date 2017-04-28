using UnityEngine;
using System.Collections;

public class ColdBuff : GenericBuff
{

	public static float DecelerationRate = 0.5f;	//the actual velocity of object will be this ratio of its original
	public Color EffectColor;
	public GameObject FreezeBuffPref;

	private Vector3 lastPosition;
	private Color originalColor;

	// Use this for initialization
	void Start () {
	
	}
	


	//this will be call in Update function if the timer is still running
	public override void Effect(){
		if (transform.parent) {
			transform.parent.position = (transform.parent.position - lastPosition) * DecelerationRate  + lastPosition;
			lastPosition = transform.parent.position;
		}

	}


	public override void OnFirstAttach(){
		if (transform.parent) {
			Renderer rend = transform.parent.gameObject.GetComponent<Renderer> ();
			if (rend) {
				originalColor = rend.material.color;
				rend.material.color = EffectColor;
			}

			lastPosition = transform.parent.position;
				
		}
	}

	public override void OnBuffEnd(){
		if (transform.parent) {
			Renderer rend = transform.parent.gameObject.GetComponent<Renderer> ();
			if (rend) {
				rend.material.color = originalColor;
			}

		}
	}


	public override bool IsHighlightable(){
		return true;
	}

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){
		if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == InteractKey.Special) {
			OnBuffEnd ();
			GameObject buff = GameObject.Instantiate (FreezeBuffPref);
			buff.GetComponent <GenericBuff>().AttachToParent (parentCommonObject);
			Destroy (this.gameObject);
			return true;
		}
		return false;
	}
}
