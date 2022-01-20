using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	一灯点滅式の信号サイクルスクリプト
 */

public class TrafficCycle1Blink : TrafficCycle {

	//灯火のゲームオブジェクト
	private new GameObject[] light1, light2;

	//点滅間隔
	public float blinkTime = 0.5f;


	// Use this for initialization
	void Start () {

		//信号方向の設定
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
		direction1 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("Direction2");
		direction2 = getChildObjects(tempDirection);


		//各灯器スクリプトの設定
		tls1 = new TrafficLightSetting[direction1.Length];
		tls2 = new TrafficLightSetting[direction2.Length];
		for (int i = 0; i < tls1.Length; i++) {
			tls1[i] = direction1 [i].GetComponent<TrafficLightSetting> ();
		}
		for (int i = 0; i < tls2.Length; i++) {
			tls2[i] = direction2 [i].GetComponent<TrafficLightSetting> ();
		}


		//各灯火の設定
		light1 = new GameObject[direction1.Length];
		light2 = new GameObject[direction2.Length];

        //灯火のセッティング
        for (int i = 0; i < light1.GetLength(0); i++) {
            light1[i] = tls1[i].SetupLightY(direction1[i], 1);
        }
        for (int i = 0; i < light2.GetLength(0); i++) {
            light2[i] = tls2[i].SetupLightR(direction2[i], 2);
        }
    }

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
		//方向１の信号サイクル
		for(int i = 0; i < light1.GetLength(0); i++){
			if (cTime >= blinkTime && cTime < blinkTime * 2) {
				//消灯
				tls1[i].SettingLightY (light1 [i], false);
			} else {
				//黄点灯
				tls1[i].SettingLightY (light1 [i], true);
			}
		}
		//方向２の信号サイクル
		for(int i = 0; i < light2.GetLength(0); i++){
			if (cTime >= blinkTime && cTime < blinkTime * 2) {
				//赤点灯
				tls2[i].SettingLightR (light2 [i], true);
			} else {
				//消灯
				tls2[i].SettingLightR (light2 [i], false);
			}
		}
			

		//サイクル時間のリセットと更新
		if (cTime >= blinkTime * 2) {
			cTime = 0f;
		} else {
			cTime += Time.deltaTime;
		}
	}
}
