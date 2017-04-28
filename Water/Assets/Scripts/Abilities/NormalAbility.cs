using UnityEngine;
using System.Collections;

public class NormalAbility : GenericAbility {

	public GameObject swordModel;
	public GameObject arrowPrefab;
	public GameObject trapPrefab;
	public GameObject hookRopePrefab;
	public GameObject rightHand;
	public Transform hookRopeAnchor;
	public const float ATTACK_MAX_HOLD_TIME = 1f;

	private GameObject sword = null;
	private HookRope hookRope;

	private float holdTime = 0f;
	private Sword swordScript;
	private Arrow arrowScript;

	private MainPlayer player;

	void Awake(){
		player = GetComponent <MainPlayer> ();
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (arrowScript) {
			arrowScript.TrackMouse ();
		}
	}

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, CommonObject.InteractKey interactKey, CommonObject.InteractDirectionKey direction = CommonObject.InteractDirectionKey.None){

		holdTime = 0f;

		//bow
		if (arrowScript != null && arrowScript.Interact (attackMode, interactKey, direction)) {
			arrowScript = null;
			sword.SetActive (true);
			return true;
		}



		if (attackMode == CharacterInfo.ATTACK_MODE.NORMAL && interactKey == CommonObject.InteractKey.Attack && player.characterInfo.energy >= 20f) {
			
			return swordScript.Interact (attackMode, interactKey, direction);

		} else if (attackMode == CharacterInfo.ATTACK_MODE.NORMAL && interactKey == CommonObject.InteractKey.Special) {
			if (direction == CommonObject.InteractDirectionKey.Up){

				//WaterWave ();
			}else if (direction == CommonObject.InteractDirectionKey.Down){

				//CreateShield ();
			}else{

				if (player.characterInfo.energy < 50) { 
				
				} else {
					//PlaceTrap ();
					if (hookRope == null) {
						SpawnHookRope ();
						hookRope.Launch ();
					} else {
						if (!player.attachableObject == hookRope) {
							hookRope.DestroyHookRope ();
							hookRope = null;
						} 

					}
				}
			}
			return true;
		}
		return false;
	}

	public override void EnableMode(){
		sword = (GameObject)Instantiate (swordModel);
		sword.transform.parent = rightHand.transform;
		sword.transform.localPosition = new Vector3(0, 0, 0);
		sword.transform.localRotation = Quaternion.Euler( new Vector3 (273.5f, 134, 70));
		swordScript = sword.GetComponent<Sword> ();
	}
	public override void DisableMode(){
		Destroy (sword);
		sword = null;
		if (arrowScript != null) {
			Destroy (arrowScript.gameObject);
			arrowScript = null;
		}
	}

	public override bool Hold(){
		if (!swordScript.IsAttacking && arrowScript == null) {
			holdTime += Time.deltaTime;
			if (holdTime >= ATTACK_MAX_HOLD_TIME) {
				holdTime = ATTACK_MAX_HOLD_TIME;
				Camera.main.GetComponent <CameraFollowPlayer> ().UseFarView ();
				//creat the arrow
				GameObject arrowObj = (GameObject)Instantiate (arrowPrefab);
				arrowScript = arrowObj.GetComponentInChildren<Arrow>();
				arrowObj.transform.parent = this.gameObject.transform;

				Vector3 initPos = arrowObj.transform.position;
				initPos = -initPos.z * Vector3.back;
				arrowObj.transform.localPosition = initPos;

				sword.SetActive (false);

			}
			return true;
		}
		return false;
	}


	/// <summary>
	/// Places the trap. Not used now
	/// </summary>

	private void PlaceTrap(){
		GameObject h = (GameObject)Instantiate (trapPrefab, player.transform.position, Quaternion.identity);
		//CharacterInfo.characterInfo.trap = h;
		player.characterInfo.energy -= 50;
	}


	private void SpawnHookRope(){
		hookRope = GameObject.Instantiate (hookRopePrefab, hookRopeAnchor.position, Quaternion.identity).GetComponentInChildren <HookRope> ();

		hookRope.Initialize (hookRopeAnchor.localPosition,player);
	}
}
