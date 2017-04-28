using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BG_WaterfallScript : MonoBehaviour {

    public float timer = 1;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        timer -= 1 * Time.deltaTime;

        if (timer < 0)
        {
            GetComponent<Rigidbody2D>().simulated = false;
        }
	}

    void stopFall()
    {

    }
}
