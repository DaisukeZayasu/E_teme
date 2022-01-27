using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*
 * 右折矢印付き車灯信号サイクルのテスト用
 */

public class TrafficCycleTestArrow : TrafficCycle {

	public float greenTime;
	public float yellowTime, yellowTime2;
	public float redTime;
	public float arrowTime;

    private TrafficLightArrowSetting[, ] tlas1;
    private GameObject[,] lightA1;


	// Use this for initialization
	void Start () {
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
		direction1 = getChildObjects(tempDirection);

		tls1 = new TrafficLightSetting[direction1.Length];
		for (int i = 0; i < tls1.Length; i++) {
			tls1[i] = direction1 [i].GetComponent<TrafficLightSetting> ();
		}
		tlas1 = new TrafficLightArrowSetting[direction1.Length, 1];
		for (int i = 0; i < tlas1.GetLength(0); i++) {
			tlas1[i, 0] = direction1 [i].transform.Find ("TrafficLightArrow_R").GetComponent<TrafficLightArrowSetting> ();
		}

		light1 = new GameObject[direction1.Length, 3];
		lightA1 = new GameObject[direction1.Length, 1];


        //車灯（0:青灯火, 1:黄灯火, 2:赤灯火）のセッティング
        for (int i = 0; i < light1.GetLength(0); i++) {
            light1[i, 0] = tls1[i].SetupLightG(direction1[i], 0);
            light1[i, 1] = tls1[i].SetupLightY(direction1[i], 1);
            light1[i, 2] = tls1[i].SetupLightR(direction1[i], 2);
        }
        //矢印灯(lightA1) 0:右折 1:左折
        for (int i = 0; i < light1.GetLength (0); i++) {
			if (tlas1 [i, 0].lightType == 1) {
				lightA1 [i, 0] = direction1 [i].transform.Find ("TrafficLightArrow_R/TLightArrow_R").gameObject;
			} else if (tlas1 [i, 0].lightType == 0 || tlas1 [i, 0].lightType == 2){
				lightA1 [i, 0] = direction1 [i].transform.Find ("TrafficLightArrow_R/TLightGroupArrow_R/PointLightA").gameObject;
			}

		}


		if (yellowTime <= 0) {
			yellowTime = CONSTS.Y_TIME;
		}
		if (yellowTime2 <= 0) {
			yellowTime2 = CONSTS.Y_TIME;
		}

		allTime = greenTime + yellowTime + arrowTime + yellowTime2 + redTime;
		Debug.Log (this.transform.name + " ：" + allTime + "秒");
	}

	void FixedUpdate(){
		//車灯の信号サイクル
		for (int i = 0; i < light1.GetLength (0); i++) {
			if (cTime < greenTime) {
				//青信号に
				tls1 [i].SettingLightG (light1 [i, 0], true);
				tls1 [i].SettingLightY (light1 [i, 1], false);
				tls1 [i].SettingLightR (light1 [i, 2], false);
				tlas1 [i, 0].SettingLightA (lightA1 [i, 0], false);
			} else if (cTime < (greenTime + yellowTime)) {
				//黄信号
				tls1 [i].SettingLightG (light1 [i, 0], false);
				tls1 [i].SettingLightY (light1 [i, 1], true);
				tls1 [i].SettingLightR (light1 [i, 2], false);
				tlas1 [i, 0].SettingLightA (lightA1 [i, 0], false);
			} else if (cTime < (greenTime + yellowTime + arrowTime)) {
				//赤信号+矢印
				tls1 [i].SettingLightG (light1 [i, 0], false);
				tls1 [i].SettingLightY (light1 [i, 1], false);
				tls1 [i].SettingLightR (light1 [i, 2], true);
				tlas1[i, 0].SettingLightA (lightA1[i, 0], true);
			} else if (cTime < (greenTime + yellowTime + arrowTime + yellowTime2)) {
				//黄信号
				tls1 [i].SettingLightG (light1 [i, 0], false);
				tls1 [i].SettingLightY (light1 [i, 1], true);
				tls1 [i].SettingLightR (light1 [i, 2], false);
				tlas1[i, 0].SettingLightA (lightA1[i, 0], false);
			} else if (cTime < allTime) {
				//赤信号
				tls1 [i].SettingLightG (light1 [i, 0], false);
				tls1 [i].SettingLightY (light1 [i, 1], false);
				tls1 [i].SettingLightR (light1 [i, 2], true);
				tlas1[i, 0].SettingLightA (lightA1[i, 0], false);
			} else {
				//青信号に
				tls1 [i].SettingLightG (light1 [i, 0], true);
				tls1 [i].SettingLightY (light1 [i, 1], false);
				tls1 [i].SettingLightR (light1 [i, 2], false);
				tlas1 [i, 0].SettingLightA (lightA1 [i, 0], false);
			}
		}

		//サイクル時間のリセットと更新
		if (cTime >= allTime) {
			cTime = 0f;
		} else {
			cTime += Time.deltaTime;
		}
	}
}
