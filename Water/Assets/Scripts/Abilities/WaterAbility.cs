using UnityEngine;
using System.Collections;
using System.Security.Policy;

public class WaterAbility : GenericAbility {


	public GameObject waterballPref;
	public MainPlayer mainPlayerScript;
	public int ownedWater = 3;
	public float shieldDist = 2;
	public bool canSpawnShield = true;
	public float getWaterRange = 20;
	public const int MAX_OWNED_WATER = 3;
	private WaterBoltObject waterBoltObject = null;
	public WaterBoltObject waterBolt { get { return waterBoltObject; } set { waterBoltObject = value; } }



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject getClosestShield(){

		RaycastHit2D[] hit = Physics2D.CircleCastAll (MainPlayer.mainPlayer.transform.position, getWaterRange, new Vector2(1, 1));
		GameObject closestShield = null;
		float dist = shieldDist;

		for (int i=0; i < hit.Length; i++)
		{
			if (hit[i].transform.tag == "Shield")
			{
				if (hit[i].distance < dist)
				{
					print ("Closest Obj : " + hit[i].transform.tag + " Name : " + hit[i].transform.name);
					closestShield = hit [i].transform.gameObject;
					dist = hit [i].distance;
				}
			}
		}

		return closestShield;
	}

	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, CommonObject.InteractKey interactKey, CommonObject.InteractDirectionKey direction = CommonObject.InteractDirectionKey.None){
		if (waterBoltObject != null && waterBoltObject.Interact (attackMode, interactKey, direction)) {
			return true;

		} else if (getClosestShield () != null/*CharacterInfo.characterInfo.shieldBoltObject != null && Vector2.Distance (mainPlayerScript.gameObject.transform.position, CharacterInfo.characterInfo.shieldBoltObject.transform.position) <= CharacterInfo.characterInfo.shieldBoltObject.GetComponent<ShieldBoltObject> ().shieldBoltsDist*/) {

			//CharacterInfo.characterInfo.shieldBoltObject.GetComponent<ShieldBoltObject> ().Attack ();
			ShieldBoltObject temp = getClosestShield ().GetComponent<ShieldBoltObject> ();
			temp.Attack ();
			temp.shieldBoltsDist = shieldDist;
			print ("Attack Shield was called !"); 


		} else if (MainPlayer.mainPlayer.characterInfo.waterWave != null) {
			
			MainPlayer.mainPlayer.characterInfo.waterWave.GetComponentInChildren<WaterWaveObject> ().Attack (MainPlayer.mainPlayer.GetClosestTarget ());

		} else if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == CommonObject.InteractKey.Attack) {
			//up attack in water mode

			//Circle all cast see if finds water ...
			RaycastHit2D[] hit = Physics2D.CircleCastAll (MainPlayer.mainPlayer.transform.position, getWaterRange, new Vector2(1, 1));
				
			GameObject closestObj = null;
			float closestDist = getWaterRange;

			for(int i=0; i < hit.Length; i++ ) {


				if (hit[i].transform.tag == "FluidWater") {
					
					bool freeze = hit [i].transform.gameObject.GetComponent<FreezableWater> ().isFrozen;

					if (freeze == false) {
						if (hit [i].distance < closestDist) {

							closestObj = hit [i].transform.gameObject;
							closestDist = hit [i].distance;
						}
					}
				}
			}

			if(closestObj != null){

				closestObj.GetComponent<FreezableWater> ().Interact (attackMode, interactKey, direction);
				closestObj.GetComponent<FreezableWater> ().mainPlayerScript = mainPlayerScript;

				return true;

			}else if (ownedWater >= 0 && direction == CommonObject.InteractDirectionKey.None) {
				//if not water pond near, get it from water container
				ownedWater--;
				GameObject h = (GameObject)Instantiate (waterballPref, this.transform.position, Quaternion.identity);
				h.GetComponent <WaterBoltObject> ().mainPlayerScript = mainPlayerScript;
				waterBoltObject = h.GetComponent <WaterBoltObject> ();
				UILogic.uiLogic.SetWaterNum (ownedWater);
				return true;
			} else {
				return MainPlayer.mainPlayer.interactiveCollider.Interact (attackMode, interactKey, direction);
			}

		} else if (attackMode == CharacterInfo.ATTACK_MODE.WATER && interactKey == CommonObject.InteractKey.Special) {
			MainPlayer.mainPlayer.interactiveCollider.Interact (attackMode, interactKey, direction);
		}
	
		return false;
	}
		

	public override void GetMainPlayer (MainPlayer mainPlayerSC)
	{

		mainPlayerScript = mainPlayerSC;
	}
	public override bool Hold(){
		if (waterBoltObject != null) {
			waterBoltObject.IncreasePower (Time.deltaTime);
			return true;
		}
		return false;
	}




}
	
