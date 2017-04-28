using UnityEngine;
using System.Collections;

using UnityEngine.SceneManagement;

public class TestButtonOnclick : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void MyClick()
    {
        // Save game data

        // Close game
        SceneManager.LoadScene("Test3XiaoXiao");
    }
}
