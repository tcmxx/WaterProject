using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterInfo : MonoBehaviour
{



    private GameObject teleporter = null;
	public GameObject telePorter { get { return teleporter; } set { teleporter = value; } }

	[HideInInspector]
	public GameObject waterWave = null;
	[HideInInspector]
	public GameObject shieldBoltObject = null;

    #region Health Related
    public float playerHealth = 3;
    public float healthRegenSpeed = 1f;
    public bool isHealthRegen = true;
	public bool isGodMode = false;
    public float MAX_HEALTH = 3;

    #endregion

    #region movement related




	public bool isPushing = false;
	public bool isOnWaterSpinUp = false;
	public bool isAnimating = false;


    public const float JUMP_SPEED_MIN = 4f;
    public const float JUMP_SPEED_MAX = 8f;
    public const float JUMP_SPEED_RATE = 40f;//change rate if the button is pushed
    public float curJumpSpeed = 0f;

    #endregion

	public float energy;

	public enum ATTACK_MODE
	{
		NORMAL, WATER, FIRE
	};

	public ATTACK_MODE attackMode = ATTACK_MODE.NORMAL;
 








}

