using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * 自動車の速度表示nGUI用スクリプト
 */

public class CarSpeedText : MonoBehaviour {

	public GameObject car;
    //CarAutonomyMove css;
    private DrivingCar dcs;
    private Text carSpeedText;
    private string gear;

	// Use this for initialization
	void Start () {
		//対象となる車を指定
		if (car == null) {
			car = GameObject.Find ("Car (1)").gameObject;
		}

		//css = car.GetComponent<CarAutonomyMove> ();
		dcs = car.GetComponent<DrivingCar> ();
		carSpeedText = this.GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (dcs.gear == 1) {
			gear = "D";
		} else if(dcs.gear == -1){
			gear = "R";
		}

		//速度表示の更新
		carSpeedText.text = (dcs.speedNow / 1000 * 3600).ToString("f0") + "km / h" + " (" + gear + ")";
	}
}
