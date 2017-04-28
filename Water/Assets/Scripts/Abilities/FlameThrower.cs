using UnityEngine;
using System.Collections;

public class FlameThrower : CommonObject {


	public GameObject burningBuffPref;


	//private ParticleSystem particleSystem;
	// Use this for initialization
	void Start () {
		//particleSystem = GetComponent <ParticleSystem> ();

	}
	
	// Update is called once per frame
	void Update () {
	
	}


	#region OnTrigger Function
	//OnTriggerEnter2D Function
	void OnTriggerEnter2D(Collider2D col)
	{


		if (col.tag != "Player" && !col.isTrigger) {

			CommonObject commonObject = col.gameObject.GetComponent<CommonObject>();
			if(commonObject != null)
			{

				GameObject buff = GameObject.Instantiate (burningBuffPref);
				buff.GetComponent <GenericBuff>().AttachToParent (commonObject);

			}
		}

	}

	//OnTriggerStay2D Function
	void OnTriggerStay2D(Collider2D col)
	{


		if (col.tag != "Player" && !col.isTrigger) {

			CommonObject commonObject = col.gameObject.GetComponent<CommonObject>();
			if(commonObject != null)
			{

				GameObject buff = GameObject.Instantiate (burningBuffPref);
				buff.GetComponent <GenericBuff>().AttachToParent (commonObject);

			}
		}

	}


	#endregion
}
