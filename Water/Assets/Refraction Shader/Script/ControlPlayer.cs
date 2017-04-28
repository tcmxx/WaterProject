using UnityEngine;
using System.Collections;

public class ControlPlayer : MonoBehaviour {

	public	Transform	ef;
	public	Transform	exp;
	public	LineRenderer	lineRender;
	public	Transform	weaponPoint;

	public	float		physicRange = 20;
	public	float 		physicForce	= 100;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0)){
			MouseLeft();
		}
		if(Input.GetMouseButton(1)){
			MouseRight();
		}else{
			lineRender.gameObject.SetActive(false);
		}
	}
	private void MouseLeft(){
		RaycastHit hit;
		if(Physics.Raycast( Camera.main.transform.position,Camera.main.transform.forward,out hit)){
			//Effect
			Transform gEf = Instantiate(ef, hit.point, Quaternion.identity) as Transform;
			gEf.parent = transform;
			Destroy(gEf.gameObject,1.0f);
			//Effect
			Transform	gExp = Instantiate(exp, hit.point,Quaternion.identity) as Transform;
			gExp.parent = transform;
			gExp.rotation = Quaternion.FromToRotation(gExp.forward,hit.normal);
			Destroy(gExp.gameObject, 1.0f);

			//physic weapon
			Vector3 dir = Vector3.zero;
			Collider[] cols = Physics.OverlapSphere( hit.point, physicRange);
			foreach(Collider col in cols){
				Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
				if(rb){
					dir = hit.point - rb.transform.position;
					rb.AddForce( -dir * (physicForce / 2));
				}
			}
		}
	}
	private void MouseRight(){
		RaycastHit hit;
		if(Physics.Raycast( Camera.main.transform.position,Camera.main.transform.forward,out hit)){
			//Effect
			Transform gEf = Instantiate(ef, hit.point, Quaternion.identity) as Transform;
			gEf.parent = transform;
			Destroy(gEf.gameObject,1.0f);
			//Effect
			Transform	gExp = Instantiate(exp, hit.point,Quaternion.identity) as Transform;
			gExp.parent = transform;
			gExp.rotation = Quaternion.FromToRotation(gExp.forward,hit.normal);
			Destroy(gExp.gameObject, 1.0f);

			//Turn on line
			lineRender.gameObject.SetActive(true);
			lineRender.SetPosition(0,hit.point);
			lineRender.SetPosition(1,weaponPoint.position);

			//physic weapon
			Vector3 dir = Vector3.zero;
			Collider[] cols = Physics.OverlapSphere( hit.point, physicRange);
			foreach(Collider col in cols){
				Rigidbody rb = col.gameObject.GetComponent<Rigidbody>();
				if(rb){
					dir = hit.point - rb.transform.position;
					rb.AddForce( -dir * physicForce );
				}
			}

		}
	}
	void OnGUI(){
		GUI.Label(new Rect(10,10,300,40),"You click on left mouse to one shooting");
		GUI.Label(new Rect(10,50,300,40),"You click on right mouse to long shooting");
	}
}
