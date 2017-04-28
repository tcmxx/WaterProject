using UnityEngine;
using System.Collections;

public class WaterSplash : CommonObject {

	public PhysicsMaterial2D normalMaterial;
	public PhysicsMaterial2D freezedMaterial;
	public float lifeTime = 10f;

	private float existingTime = 0f;
	// Use this for initialization
	void Start () {
		existingTime = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		existingTime += Time.deltaTime;
		if (existingTime > lifeTime) {
			Destroy (this.gameObject);
		}
	}



	public override bool TakeDamage(int damage, DamageType damageType = DamageType.Normal, GameObject damageFromObject = null)
	{
		return false;

	}





	/// <summary>
	/// virtual method of InteractiveObject. When this interactive object is highlighted, and the interact button is pressed, game logic will call this method of that object
	/// </summary>
	/// <param name="command">an arguement pass to this method, which enable it to trigger different events of this interactive object</param>
	/// <returns></returns>
	public override bool Interact(CharacterInfo.ATTACK_MODE attackMode, InteractKey interactKey, InteractDirectionKey direction = InteractDirectionKey.None){
		if (interactKey == InteractKey.Special) {
		}
		return false;
	}

	/// <summary>
	/// virtual method of InteractiveObject. If this object can be highlighted, return turn. Otherwise return false
	/// </summary>
	/// <returns></returns>
	public override bool IsHighlightable(){
		return true;
	}

	/// <summary>
	/// virtual method of InteractiveObject. If this object can be highlighted, return turn. Otherwise return false
	/// </summary>
	/// <returns></returns>
	public override void Highlight(bool highlight){

	}
}
