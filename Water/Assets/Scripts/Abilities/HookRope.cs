using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.AccessControl;
using System.Reflection;

public class HookRope : AttachableObject {
	


	public float launchVel;
	public float hookMaxRange;
	public float maxPullingForce;
	public float desiredPullingSpeed;
	public Transform ropeHead;
	public Transform ropeEnd;
	public SliderJoint2D sliderJoint;
	public LineRenderer lineRenderer;
	private FixedJoint2D fixedJoint;

	private Rigidbody2D hookBd;

	private bool launched;
	private bool hooked = false;
	private bool hookedHasRigidBody;

	private MainPlayer attachedPlayer;

	void Awake(){
		fixedJoint = GetComponent <FixedJoint2D> ();
		hookBd = GetComponent <Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		lineRenderer.SetPosition (0,ropeHead.position);
		lineRenderer.SetPosition (1,ropeEnd.position);

		if (hooked == false && (ropeEnd.position - ropeHead.position).magnitude >= hookMaxRange) {
			DestroyHookRope ();
		}
		if (hooked == true) {
			UpdateHookForce ();
		}
	}


	public void Initialize(Vector3 anchorPoint, MainPlayer player){
		attachedPlayer = player;
		FixedJoint2D joint = ropeEnd.GetComponent <FixedJoint2D> ();
		joint.connectedBody = player.GetComponent <Rigidbody2D>();
		joint.connectedAnchor = anchorPoint;
		//sliderJoint.connectedBody = player.GetComponent <Rigidbody2D>();
		//sliderJoint.connectedAnchor = anchorPoint;

		JointTranslationLimits2D limit = new JointTranslationLimits2D ();
		limit.max = hookMaxRange;
		limit.min = 0;
		sliderJoint.limits = limit;
	}



	public void Launch(){

		CommonObject commonObj;
		Vector3 closestPoint;
		FindClosestHookableInRange (out closestPoint, out commonObj);

		if (commonObj != null) {
			hookBd.gravityScale = 0;
			hookBd.velocity = (closestPoint - transform.position).normalized * launchVel;
			sliderJoint.enabled = false;
			launched = true;
			transform.rotation = Quaternion.FromToRotation (Vector3.up, (closestPoint - transform.position).normalized);
		} else {
			DestroyHookRope ();
		}

	}

	public void DestroyHookRope(){
		Destroy (transform.parent.gameObject);
	}




	public override void OnTriggerEnter2D(Collider2D col){

		GameObject obj;
		if (col.attachedRigidbody != null) {
			obj = col.attachedRigidbody.gameObject;
		} else {
			obj = col.gameObject;
		}

		CommonObject common = obj.GetComponent <CommonObject> ();

		if (launched && common != null && (common.objectProperty & CommonObject.CommonObjectProperty.Hookable) != 0) {
			//launcher.PullBack ();
			launched = false;

			Rigidbody2D bd = obj.GetComponent <Rigidbody2D> ();
			JointTranslationLimits2D lim = sliderJoint.limits;
			if (bd != null) {
				//attached to dynaic rigidbody
				fixedJoint.connectedBody = bd;
				fixedJoint.connectedAnchor = obj.transform.InverseTransformPoint (transform.position);
				hookedHasRigidBody = true;
				lim.min = 0;
			} else {
				//attached to static object
				fixedJoint.connectedAnchor = transform.position;
				hookedHasRigidBody = false;
				attachedPlayer.Attach (this);
				attachedPlayer.GetComponent <Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
				lim.min = -sliderJoint.jointTranslation;
			}
			fixedJoint.enabled = true;
			sliderJoint.enabled = true;
			//reset the limit
			lim.max = sliderJoint.jointTranslation;
			sliderJoint.limits = lim;

			hooked = true;
		}
	}
	public override void OnTriggerExit2D(Collider2D col)
	{
		//do not call the base one
	}

	private void UpdateHookForce(){
		if (hookedHasRigidBody) {
			JointMotor2D motor = sliderJoint.motor;
			motor.maxMotorTorque = maxPullingForce;
			motor.motorSpeed = -desiredPullingSpeed;
			sliderJoint.motor = motor;
			sliderJoint.useMotor = true;
		} 
	}


	/// <summary>
	/// Finds the closest hookable in range.
	/// </summary>
	/// <param name="foundPoint">Found closest point</param>
	/// <param name="resultObj">Result object with the closest point</param>
	private void FindClosestHookableInRange(out Vector3 foundPoint, out CommonObject resultObj){
		Collider2D[] allColliders = Physics2D.OverlapCircleAll (sliderJoint.connectedBody.transform.position, hookMaxRange);

		resultObj = null;
		foundPoint = Vector3.zero;

		float minDist = float.MaxValue;
		foreach (var col in allColliders) {

			GameObject obj;
			if (col.attachedRigidbody != null) {
				obj = col.attachedRigidbody.gameObject;
			} else {
				obj = col.gameObject;
			}

			CommonObject commonObj = obj.GetComponent <CommonObject> ();

			//only update the result if the object is hookable and closer than the previous one
			if (commonObj != null &&
				(commonObj.objectProperty & CommonObject.CommonObjectProperty.Hookable) != 0)  {
				Vector3 closestPoint = col.bounds.ClosestPoint (sliderJoint.connectedBody.transform.position);
				float dist = Vector3.Distance (closestPoint, sliderJoint.connectedBody.transform.position);

				if (dist < minDist) {
					resultObj = commonObj;
					minDist = dist;
					foundPoint = closestPoint;
				}
			} 

		}

	}



	public override void Move (float horizontal, MainPlayer playerRef){

		playerRef.GetComponent<Rigidbody2D>().AddForce(Vector2.right* horizontal*1);
		return;
	}

	public override void Jump (bool start, bool push, MainPlayer playerRef){
		if (!(start && push)) {
			return;
		}
		playerRef.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
		playerRef.transform.rotation = Quaternion.identity;
		playerRef.Detach (this);
		playerRef.Jump (start, push);
		DestroyHookRope ();
	}

	public override void Interact (MainPlayer playerRef, 
		CharacterInfo.ATTACK_MODE attackMode, 
		CommonObject.InteractKey interactKey, 
		CommonObject.InteractDirectionKey direction = CommonObject.InteractDirectionKey.None){
	}

}
