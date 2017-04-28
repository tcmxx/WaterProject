using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour {

	public Slider energyBarSlider;
	public bool stopEnergyRegen = false;
	public static EnergyBar energyBar;
	// Use this for initialization

	void Awake(){
		energyBar = this;

	}
	void Start () {
	
		//Get the Bar Slider
		energyBarSlider= GameObject.Find ("Energy Bar").GetComponent<Slider> ();
		energyBarSlider.value = 0;
		MainPlayer.mainPlayer.characterInfo.energy = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
		//Get the energy value
		energyBarSlider.value = MainPlayer.mainPlayer.characterInfo.energy;
		
		//Regen the energy IF bar is not at max value or stopEnergyRegen var is false
		if (energyBarSlider.value < 100 && stopEnergyRegen == false) {

			energyBarSlider.value = (energyBarSlider.value + (20 * Time.deltaTime));
		}
		MainPlayer.mainPlayer.characterInfo.energy = energyBarSlider.value;
	}
}
