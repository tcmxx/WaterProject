using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimerBar : MonoBehaviour {

	public Slider timerBarSlider;
	public static TimerBar timerBar;

	void Awake(){
		timerBar = this;

		timerBarSlider= GameObject.Find ("Time Bar").GetComponent<Slider> ();
		timerBarSlider.value = 10;

	}
	
	// Update is called once per frame
	void Update () {
	
		if (MainPlayer.mainPlayer.characterInfo.attackMode == CharacterInfo.ATTACK_MODE.FIRE) {

			TimeDown ();
		} else {

			TimerRegen ();
		}
	}

	void TimeDown() {

		if (timerBarSlider.value != 0) {
			timerBarSlider.value -= 1 * Time.deltaTime;
		} else {
			MainPlayer.mainPlayer.SwitchMode ();
		}
	}

	void TimerRegen() {
		if (timerBarSlider.value < 10) {
			timerBarSlider.value += 1 * Time.deltaTime;
		}
	}
}
