using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TpVisualBar : MonoBehaviour {

	public bool isEnabled = false;
	public int incValue = 1;
	public static TpVisualBar tpVisualBar;
	public Slider slider;

	// Use this for initialization
	void Start () {

		slider = GetComponent<Slider> ();
		tpVisualBar = this;
		isEnabled = true;
	}

	// Update is called once per frame
	void Update () {
	
		//If is enabled
		if (isEnabled){
			
			//If bar is at max value return
			if(slider.value == slider.maxValue){
				return;
				
			//else increase bar value by incValue per second
			} else {

				slider.value += incValue * Time.deltaTime;
			}
		}
	}
}
