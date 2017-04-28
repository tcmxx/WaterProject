using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UILogic : MonoBehaviour {

    public static UILogic uiLogic;

	public GameObject waterContainer;
	public Animator attackModeAnimator;
	public GameObject escMenu;
    private Image redBorder;
	private bool isMenu = false;


	void Awake(){
		uiLogic = this;
	}
    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

		if(Input.GetButtonDown ("Menu")) {

			if (isMenu) {

				Time.timeScale = 1;
				escMenu.SetActive (false);
				isMenu = false;
			} else {

				Time.timeScale = 0;
				escMenu.SetActive (true);
				isMenu = true;
			}
		}

        updateHealth();
    }

	public void OnBackMainMenuButtonClick() 
	{

		SceneManager.LoadScene ("TestIntroScene");
	}

	public void OnSaveButtonClick()
	{ 
			//Vars
		GameObject teleporter = MainPlayer.mainPlayer.characterInfo.telePorter;
		float playerhealth = MainPlayer.mainPlayer.characterInfo.playerHealth;

			//Save the teleporter
		if (teleporter != null) {
			float tpX = teleporter.transform.position.x;
			float tpY = teleporter.transform.position.y;
			float tpZ = teleporter.transform.position.z;

			PlayerPrefs.SetFloat ("TeleportPosX", tpX);
			PlayerPrefs.SetFloat ("TeleportPosY", tpY);
			PlayerPrefs.SetFloat ("TeleportPosz", tpZ);
		}

			//Save the checkpoint
		float ckX = MainPlayer.mainPlayer.gameObject.transform.position.x;
		float ckY = MainPlayer.mainPlayer.gameObject.transform.position.y;
		float ckZ = MainPlayer.mainPlayer.gameObject.transform.position.z;

		PlayerPrefs.SetFloat ("CheckpointPosX", ckX);
		PlayerPrefs.SetFloat ("CheckpointPosY", ckY);
		PlayerPrefs.SetFloat ("CheckpointPosz", ckZ);

			//Save the health
		PlayerPrefs.SetFloat ("PlayerHealth", playerhealth);





		print ("saved On UI") ;
	}

	public void OnLoadButtonClick()
	{
		//Load the teleporter
		if (PlayerPrefs.GetFloat("TeleportPosX", 9999999f) != 9999999f)
		{

			float tpX = PlayerPrefs.GetFloat("TeleportPosX");
			float tpY = PlayerPrefs.GetFloat("TeleportPosY");
			float tpZ = PlayerPrefs.GetFloat("TeleportPosz");


			GameObject h = (GameObject)Instantiate(MainPlayer.mainPlayer.teleporterPref, new Vector3(tpX, tpY, tpZ), Quaternion.identity);

			MainPlayer.mainPlayer.characterInfo.telePorter = h;
		}

		//Load at the checkpoint
		if (PlayerPrefs.GetFloat("CheckpointPosX", 9999999f) != 9999999f)
		{

			float ckX = PlayerPrefs.GetFloat("CheckpointPosX");
			float ckY = PlayerPrefs.GetFloat("CheckpointPosY");
			float ckZ = PlayerPrefs.GetFloat("CheckpointPosz");

			MainPlayer.mainPlayer.transform.position = new Vector3(ckX, ckY, ckZ);
		}

		//Load the health
		if (PlayerPrefs.GetFloat("PlayerHealth", 9999999f) != 9999999f)
		{

			MainPlayer.mainPlayer.characterInfo.playerHealth = PlayerPrefs.GetFloat("PlayerHealth");
		}
		else {

			MainPlayer.mainPlayer.characterInfo.playerHealth = 3;
		}



		print ("Loading Complete on UI"); 
	}



    #region Update health ui
    private void updateHealth()
    {
        redBorder = transform.FindChild("FullScreenCanvas").FindChild("RedBorder").GetComponentInChildren<Image>();

		if (MainPlayer.mainPlayer.characterInfo.playerHealth < MainPlayer.mainPlayer.characterInfo.MAX_HEALTH && MainPlayer.mainPlayer.characterInfo.playerHealth > 0) {
			redBorder.color = new Color(redBorder.color.r, redBorder.color.g, redBorder.color.b, (float)(-0.333333333333333 * MainPlayer.mainPlayer.characterInfo.playerHealth + 1));
        }
    }

    #endregion


	#region Switch Mode
	public void SwitchMode(CharacterInfo.ATTACK_MODE attackMode){
		if (attackMode == CharacterInfo.ATTACK_MODE.NORMAL) {
			waterContainer.SetActive (false);
			attackModeAnimator.SetFloat ("Mode", 0f);
		} else if (attackMode == CharacterInfo.ATTACK_MODE.WATER) {
			waterContainer.SetActive (true);
			SetWaterNum (((WaterAbility)MainPlayer.mainPlayer.waterAbility).ownedWater);
			attackModeAnimator.SetFloat ("Mode", 1f);
		} else if (attackMode == CharacterInfo.ATTACK_MODE.FIRE) {
			waterContainer.SetActive (false);
			attackModeAnimator.SetFloat ("Mode", 2f);

		}
		
	}

	#endregion

	public void SetWaterNum(int num){
		waterContainer.GetComponent<Animator> ().SetFloat ("NumberOfWater", num);
	}


}
