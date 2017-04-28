using UnityEngine;
using System.Collections;

public class jitterSplinePoints : MonoBehaviour
{

    public RageSpline rageSpline;

	void Start (){
	    rageSpline = GetComponent<RageSpline>();
	}
	
	void Update () {
        if (Input.GetKey(KeyCode.J))
            for (int i = 0; i < rageSpline.spline.points.Length-1; i++) {
                float rnd = Random.Range(-0.25f, 0.25f);
                var newPos = rageSpline.GetPosition(i);
                rageSpline.SetPoint(i, new Vector3(newPos.x+rnd, newPos.y+rnd));
            }
        rageSpline.RefreshMesh();

	}
}
