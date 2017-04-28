using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MainLogic : MonoBehaviour {
    
    #region Variables
    //mainlogic can be accessed by MainLogic.mainLogic;
    public static MainLogic mainLogic = null; 

    #endregion

    #region Callbacks
    // Use this for initialization
    void Start () {
        //assign this to the public static mainlogic
        mainLogic = this;

    }
	
	// Update is called once per frame
	void Update () {

    }

    #endregion



	#region Save and Load Methods
	public void SaveGameCheckPoint(GameObject checkpoint) { 

		//Vars
		GameObject teleporter = MainPlayer.mainPlayer.characterInfo.telePorter;
		float playerhealth = MainPlayer.mainPlayer.characterInfo.playerHealth;
		bool movingright = MainPlayer.mainPlayer.generalMovementControl.movingRight;

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
		float ckX = checkpoint.transform.position.x;
		float ckY = checkpoint.transform.position.y;
		float ckZ = checkpoint.transform.position.z;

		PlayerPrefs.SetFloat ("CheckpointPosX", ckX);
		PlayerPrefs.SetFloat ("CheckpointPosY", ckY);
		PlayerPrefs.SetFloat ("CheckpointPosz", ckZ);

		//Save the health
		PlayerPrefs.SetFloat ("PlayerHealth", playerhealth);


		
		
		//Save movingRight
		PlayerPrefs.SetInt ("MovingRight", movingright?1:0); //Use PlayerPrefs.GetInt("MovingRight")==1; to load
        

		print ("saved") ;
	}

    IEnumerator OnLevelWasLoaded(int level) {
        

        //wait until objects are started
		while (MainPlayer.mainPlayer == null || MainPlayer.mainPlayer.characterInfo == null)
        {
            yield return new WaitForSeconds(0.1f);
        }
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



        


        print("Loading Complete!!");
    }


	#endregion
}
