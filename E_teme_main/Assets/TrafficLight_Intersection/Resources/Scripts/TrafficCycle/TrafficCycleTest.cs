using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*
 * 車灯信号サイクルのテスト用
 */

public class TrafficCycleTest : TrafficCycle {

	public float greenTime;
	public float yellowTime;
	public float redTime;
    public float startTime;

	// Use this for initialization
	void Start () {
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
		direction1 = getChildObjects(tempDirection);

		tls1 = new TrafficLightSetting[direction1.Length];
		for (int i = 0; i < tls1.Length; i++) {
			tls1[i] = direction1 [i].GetComponent<TrafficLightSetting> ();
		}

		light1 = new GameObject[direction1.Length, 3];

        //車灯 0:青灯火, 1:黄灯火, 2:赤灯火　のセッティング
        for (int i = 0; i < light1.GetLength(0); i++) {
            light1[i, 0] = tls1[i].SetupLightG(direction1[i], 0);
            light1[i, 1] = tls1[i].SetupLightY(direction1[i], 1);
            light1[i, 2] = tls1[i].SetupLightR(direction1[i], 2);
        }

        if (yellowTime <= 0) {
			yellowTime = CONSTS.Y_TIME;
		}

		allTime = greenTime + yellowTime + redTime;
		Debug.Log (this.transform.name + " ：" + allTime + "秒");


        cTime = startTime;
	}

	void FixedUpdate(){
		//車灯の信号サイクル
		for (int i = 0; i < light1.GetLength (0); i++) {
			if (cTime < greenTime) {
				//青信号に
				tls1 [i].SettingLightG (light1 [i, 0], true);
				tls1 [i].SettingLightY (light1 [i, 1], false);
				tls1 [i].SettingLightR (light1 [i, 2], false);
			} else if (cTime < (greenTime + yellowTime)) {
				//黄信号に
				tls1 [i].SettingLightG (light1 [i, 0], false);
				tls1 [i].SettingLightY (light1 [i, 1], true);
				tls1 [i].SettingLightR (light1 [i, 2], false);
			} else if (cTime < allTime) {
				//赤信号に
				tls1 [i].SettingLightG (light1 [i, 0], false);
				tls1 [i].SettingLightY (light1 [i, 1], false);
				tls1 [i].SettingLightR (light1 [i, 2], true);
			} else {
				//青信号に
				tls1 [i].SettingLightG (light1 [i, 0], true);
				tls1 [i].SettingLightY (light1 [i, 1], false);
				tls1 [i].SettingLightR (light1 [i, 2], false);
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
