using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InteractiveCollider : MonoBehaviour
{

    #region Variables
    //Public Static Vars
	private CommonObject curInteractiveObject = null;
	public static List<CommonObject> interactiveOjectsInRange = new List<CommonObject>();

    //Public Vars

    //Private Vars

    //SF Vars

    #endregion

    #region Start Function
    void Start()
    {
        
    }
    #endregion

    #region Update  Function
    void Update()
    {
        getClosestOjectInRange();
    }
    #endregion

    #region getClosestOjectInRange Function
    //Get The Closest object in range
    GameObject getClosestOjectInRange()
    {
		CommonObject tMin = null;
        float minDist = 99999f;
        Vector3 currentPos = transform.position;

        //Foreach enemy , will calculate the distance
		foreach (CommonObject t in interactiveOjectsInRange)
        {
            if (t == null)
            {//if the object is removed by codes, remove it from the list
                interactiveOjectsInRange.Remove(t);
                break;
            }
            float dist = Vector3.Distance(t.transform.position, currentPos);
			if (dist < minDist && t.GetComponent<CommonObject>().IsHighlightable())
            {
                tMin = t;
                minDist = dist;
            }
			t.Highlight (false);
        }
        if (tMin == null)
        {
            curInteractiveObject = null;
			return null;
        }
        else {
			tMin.Highlight (true);
			curInteractiveObject = tMin.GetComponent<CommonObject>();
			return tMin.gameObject;
        }

    }
    #endregion



    /// <summary>
    /// Interact the specified attackMode, interactKey, direction and allObject.
    /// </summary>
    /// <param name="attackMode">Attack mode.</param>
    /// <param name="interactKey">Interact key.</param>
    /// <param name="direction">Direction.</param>
    /// <param name="allObject">If set to <c>true</c> all object in range will be affected by this call. Otherwise only the closest one.</param>
	public bool Interact(CharacterInfo.ATTACK_MODE attackMode, CommonObject.InteractKey interactKey, CommonObject.InteractDirectionKey direction = CommonObject.InteractDirectionKey.None, bool allObject = false)
    {
		UpdateObjects ();
		getClosestOjectInRange ();
		if (curInteractiveObject == null) {
			return false;
		} else if (!allObject) {
			return curInteractiveObject.Interact (attackMode, interactKey, direction);
		} else {
			bool tmpReture = false;
			foreach (CommonObject t in interactiveOjectsInRange)
			{
				if (t.Interact (attackMode, interactKey, direction)) {
					tmpReture = true;
				}
			}
			return tmpReture;
		}
    }

    #region OnTriggerExit2D Function
    //OnTriggerExit2D Function
    void OnTriggerExit2D(Collider2D col)
    {

        // will deactivate the IsNearWater variable
		CommonObject commonObject = col.gameObject.GetComponent <CommonObject> ();
		if(commonObject != null &&
			( ((commonObject.objectProperty & CommonObject.CommonObjectProperty.WaterModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.WATER) ||
				((commonObject.objectProperty & CommonObject.CommonObjectProperty.FireModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.FIRE) ||
				((commonObject.objectProperty & CommonObject.CommonObjectProperty.NormalModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.NORMAL))){

			Debug.Log("Exit interractive object");
			commonObject.Highlight (false);
			interactiveOjectsInRange.Remove(commonObject);
		}
    }
    #endregion


    #region OnTriggerEnter2D Function
    void OnTriggerEnter2D(Collider2D col)
    {
		CommonObject commonObject = col.gameObject.GetComponent <CommonObject> ();
		if(commonObject != null &&
			( ((commonObject.objectProperty & CommonObject.CommonObjectProperty.WaterModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.WATER) ||
				((commonObject.objectProperty & CommonObject.CommonObjectProperty.FireModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.FIRE) ||
				((commonObject.objectProperty & CommonObject.CommonObjectProperty.NormalModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.NORMAL))){

			Debug.Log("Found interractive object");
			interactiveOjectsInRange.Add(commonObject);
    	}
    
	}
	#endregion
 

	#region Update objects
	//update the object list based on attackmode
	public void UpdateObjects(){
		
		Collider2D[] f = Physics2D.OverlapCircleAll (transform.position, GetComponent <CircleCollider2D>().radius);
		foreach(CommonObject commonObj in interactiveOjectsInRange){
			commonObj.Highlight (false);
		}
		interactiveOjectsInRange.Clear ();
		foreach(Collider2D colWithinRange in f){
			CommonObject commonObject = colWithinRange.GetComponent <CommonObject>();
			if(commonObject != null &&
				( ((commonObject.objectProperty & CommonObject.CommonObjectProperty.WaterModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.WATER) ||
					((commonObject.objectProperty & CommonObject.CommonObjectProperty.FireModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.FIRE) ||
					((commonObject.objectProperty & CommonObject.CommonObjectProperty.NormalModeActive) != 0 && MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.NORMAL))){


				interactiveOjectsInRange.Add(commonObject);
			}
		}
	}
	#endregion
}
