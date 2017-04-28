using UnityEngine;
using System.Collections;

public class orbit : MonoBehaviour {

    public GameObject Target;
    private float timer = .5f;
    public float direction = 1;


    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

       

        

        if (timer > 0)
        {
            transform.RotateAround(Target.transform.position, new Vector3(0, direction, 0), 2000 * Time.deltaTime);
            timer -= 1 * Time.deltaTime;
        }

        if(timer <= 0)
        {
      
            //Do nothing
        }
        
	
	}
}
