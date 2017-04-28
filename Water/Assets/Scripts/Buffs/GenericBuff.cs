using UnityEngine;
using System.Collections;

public class GenericBuff : CommonObject{

	public float maxTime = 5f;

	protected float timer = 0f;
	protected CommonObject parentCommonObject = null;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		DoUpdate ();
	}

	protected void DoUpdate(){
		if (timer >= 0) {
			timer = timer + Time.deltaTime;
		}
		if (timer > maxTime) {
			OnBuffEnd ();
			Destroy (gameObject);
		} 
		Effect ();
	}


	public void AttachToParent(CommonObject parent){
		Transform buff = parent.transform.FindChild (this.gameObject.name);

		if (buff == null) {
			transform.SetParent (parent.transform);
			transform.position = parent.transform.position;
			parentCommonObject = parent;
			OnFirstAttach ();
		} else {
			buff.gameObject.GetComponent <GenericBuff>().Reset ();
			Destroy (gameObject);
		}


	}


	public virtual void OnFirstAttach(){
	}
	public virtual void OnBuffEnd(){
	}
	//this will be call in Update function if the timer is still running
	public virtual void Effect(){

	}

	//reset the buff. If when trying to attach the buff to the object and the same buff already exist, will call this function automatically
	public virtual void Reset(){
		timer = 0f;
	}



}
