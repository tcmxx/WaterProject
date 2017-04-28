using UnityEngine;
using System.Collections;
using System.CodeDom.Compiler;
using System.Collections.Generic;

public class WaterSplashGenerator : MonoBehaviour {

	public GameObject ballLeft;
	public GameObject ballRight;

	public float spreadingTime = 0.5f;
	public float newPointTimeInterval = 0.1f;
	public float defaultMovingForce = 5f;
	public float defaultDownForce = 10f;

	private LineRenderer lineRenderer;

	private EdgeCollider2D edgeCollider;

	private Rigidbody2D ballLeftRB;
	private Rigidbody2D ballRightRB;

	private Vector2 leftBallForce;
	private Vector2 rightBallForce;

	private List<Vector2> tmpPoints;

	// Use this for initialization
	void Start () {
		ballLeftRB = ballLeft.GetComponent <Rigidbody2D>();
		ballRightRB = ballRight.GetComponent <Rigidbody2D>();
		edgeCollider = GetComponent <EdgeCollider2D> ();
		lineRenderer = GetComponent <LineRenderer> ();

		tmpPoints = new List<Vector2> ();

		Generate (new Vector2(0,-1));
	}

	void FixedUpdate(){
		if (ballLeft != null && ballRight != null) {
			ballLeftRB.AddForce (leftBallForce, ForceMode2D.Force);
			ballRightRB.AddForce (rightBallForce, ForceMode2D.Force);
		}
	}

	// Update is called once per frame
	void Update () {
	}



	public void Generate(Vector2 hitDirection){

		transform.rotation = Quaternion.LookRotation (new Vector3 (0, 0, 1), hitDirection.normalized);


		Vector2 leftForceDir;
		leftForceDir.x = -hitDirection.y;
		leftForceDir.y = hitDirection.x;
		Vector2 rightForceDir;
		rightForceDir.x = hitDirection.y;
		rightForceDir.y = -hitDirection.x;

		leftBallForce = leftForceDir.normalized * defaultMovingForce + hitDirection.normalized  * defaultDownForce;
		rightBallForce = rightForceDir.normalized  * defaultMovingForce + hitDirection.normalized  * defaultDownForce;

		StartCoroutine (RecordBallLocation ());

	}


	IEnumerator RecordBallLocation(){

		float time = 0f;
		List<Vector3> points3D = new List<Vector3> ();
		while (time < spreadingTime) {
			yield return new WaitForSeconds (newPointTimeInterval);
			time += newPointTimeInterval;
			tmpPoints.Insert(0,transform.worldToLocalMatrix.MultiplyPoint3x4(ballLeft.transform.position));
			tmpPoints.Add (transform.worldToLocalMatrix.MultiplyPoint3x4(ballRight.transform.position));

			edgeCollider.points = tmpPoints.ToArray ();

			points3D = new List<Vector3> ();
			foreach (var p in tmpPoints) {
				points3D.Add (p);
			}
			lineRenderer.SetVertexCount (points3D.Count);
			lineRenderer.SetPositions (points3D.ToArray ());
		}

		Destroy (ballLeft);
		Destroy (ballRight);

		edgeCollider.points = tmpPoints.ToArray ();

		points3D = new List<Vector3> ();
		foreach (var p in tmpPoints) {
			points3D.Add (p);
		}
		lineRenderer.SetVertexCount (points3D.Count);
		lineRenderer.SetPositions (points3D.ToArray ());
	}

}
