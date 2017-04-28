using UnityEngine;
using System.Collections;
using System;

public class CommonObject : MonoBehaviour {

	[EnumFlagAttribute]
	public CommonObjectProperty objectProperty;

	public bool zVariable = false;

    public const float MIN_COL_ERROR = 0.001f;

    public enum DamageType {Normal, Water, Fire, Falling };
	//different command for interaction. more will be added as necessary
	public enum InteractKey { Attack, Action, Special, Other };
	public enum InteractDirectionKey { Up, Down, None };

	[HideInInspector]
	public bool isInWater = false;



	public virtual bool TakeDamage(int damage, DamageType damageType = DamageType.Normal, GameObject damageFromObject = null)
	{
		return false;

	}





	/// <summary>
	/// virtual method of InteractiveObject. When this interactive object is highlighted, and the interact button is pressed, game logic will call this method of that object
	/// </summary>
	/// <param name="command">an arguement pass to this method, which enable it to trigger different events of this interactive object</param>
	/// <returns></returns>
	public virtual bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){
		return false;
	}

	/// <summary>
	/// virtual method of InteractiveObject. If this object can be highlighted, return turn. Otherwise return false
	/// </summary>
	/// <returns></returns>
	public virtual bool IsHighlightable(){
		return false;
	}

	/// <summary>
	/// virtual method of InteractiveObject. If this object can be highlighted, return turn. Otherwise return false
	/// </summary>
	/// <returns></returns>
	public virtual void Highlight(bool highlight){

		Transform tmpGameobject = this.transform.FindChild ("InteractiveObject");
		if (tmpGameobject != null) {
			tmpGameobject.GetComponent<SpriteRenderer>().enabled = highlight;
		}
	}



	[FlagsAttribute]
	public enum CommonObjectProperty{
		Targetable = 1,
		Pushable = 2,
		WaterModeActive = 4,
		FireModeActive = 8,
		NormalModeActive = 16,
		Hookable = 32
	}


}
