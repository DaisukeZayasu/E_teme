using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	閃光（黄・赤点滅）方式の信号サイクルスクリプト
 */

public class TrafficCycleBlink : TrafficCycle {

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
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionP1");
		directionP1 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionP2");
		directionP2 = getChildObjects(tempDirection);


		//各灯器スクリプトの設定
		tls1 = new TrafficLightSetting[direction1.Length];
		tls2 = new TrafficLightSetting[direction2.Length];
		for (int i = 0; i < tls1.Length; i++) {
			tls1[i] = direction1 [i].GetComponent<TrafficLightSetting> ();
		}
		for (int i = 0; i < tls2.Length; i++) {
			tls2[i] = direction2 [i].GetComponent<TrafficLightSetting> ();
		}
		tlps1 = new TrafficLightPSetting[directionP1.Length];
		tlps2 = new TrafficLightPSetting[directionP2.Length];
		for (int i = 0; i < tlps1.Length; i++) {
			tlps1[i] = directionP1 [i].GetComponent<TrafficLightPSetting> ();
		}
		for (int i = 0; i < tlps2.Length; i++) {
			tlps2[i] = directionP2 [i].GetComponent<TrafficLightPSetting> ();
		}


		//各灯火の設定
		light1 = new GameObject[direction1.Length, 3];
		light2 = new GameObject[direction2.Length, 3];
		if (directionP1.Length != 0) {
			lightP1 = new GameObject[directionP1.Length, 2];
		}
		if (directionP2.Length != 0) {
			lightP2 = new GameObject[directionP2.Length, 2];
		}

        //車灯（0:青灯火, 1:黄灯火, 2:赤灯火）のセッティング
        for (int i = 0; i < light1.GetLength(0); i++) {
            light1[i, 0] = tls1[i].SetupLightG(direction1[i], 0);
            light1[i, 1] = tls1[i].SetupLightY(direction1[i], 1);
            light1[i, 2] = tls1[i].SetupLightR(direction1[i], 2);
        }
        for (int i = 0; i < light2.GetLength(0); i++) {
            light2[i, 0] = tls2[i].SetupLightG(direction2[i], 0);
            light2[i, 1] = tls2[i].SetupLightY(direction2[i], 1);
            light2[i, 2] = tls2[i].SetupLightR(direction2[i], 2);
        }
        //歩灯(lightP1/2) 0:赤灯火, 1:青灯火
        if (directionP1.Length != 0) {
            for (int i = 0; i < lightP1.GetLength(0); i++) {
                if (tlps1[i].lightType[0] == 1) {
                    lightP1[i, 0] = directionP1[i].transform.Find("TPLightGroupRL/TPLightRed").gameObject;
                } else if (tlps1[i].lightType[0] == 0 || tlps1[i].lightType[0] == 2) {
                    lightP1[i, 0] = directionP1[i].transform.Find("TPLightGroupR/PointLightPR").gameObject;
                }
                if (tlps1[i].lightType[1] == 1) {
                    lightP1[i, 1] = directionP1[i].transform.Find("TPLightGroupGL/TPLightGreen").gameObject;
                } else if (tlps1[i].lightType[1] == 0 || tlps1[i].lightType[1] == 2) {
                    lightP1[i, 1] = directionP1[i].transform.Find("TPLightGroupG/PointLightPG").gameObject;
                }
            }
        }
        if (directionP2.Length != 0) {
            for (int i = 0; i < lightP2.GetLength(0); i++) {
                if (tlps2[i].lightType[0] == 1) {
                    lightP2[i, 0] = directionP2[i].transform.Find("TPLightGroupRL/TPLightRed").gameObject;
                } else if (tlps2[i].lightType[0] == 0 || tlps2[i].lightType[0] == 2) {
                    lightP2[i, 0] = directionP2[i].transform.Find("TPLightGroupR/PointLightPR").gameObject;
                }
                if (tlps2[i].lightType[1] == 1) {
                    lightP2[i, 1] = directionP2[i].transform.Find("TPLightGroupGL/TPLightGreen").gameObject;
                } else if (tlps2[i].lightType[1] == 0 || tlps2[i].lightType[1] == 2) {
                    lightP2[i, 1] = directionP2[i].transform.Find("TPLightGroupG/PointLightPG").gameObject;
                }
            }
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
				tls1[i].SettingLightG (light1 [i, 0], false);
				tls1[i].SettingLightY (light1 [i, 1], false);
				tls1[i].SettingLightR (light1 [i, 2], false);
			} else {
				//黄点灯
				tls1[i].SettingLightG (light1 [i, 0], false);
				tls1[i].SettingLightY (light1 [i, 1], true);
				tls1[i].SettingLightR (light1 [i, 2], false);
			}
		}
		//方向２の信号サイクル
		for(int i = 0; i < light2.GetLength(0); i++){
			if (cTime >= blinkTime && cTime < blinkTime * 2) {
				//赤点灯
				tls2[i].SettingLightG (light2 [i, 0], false);
				tls2[i].SettingLightY (light2 [i, 1], false);
				tls2[i].SettingLightR (light2 [i, 2], true);
			} else {
				//消灯
				tls2[i].SettingLightG (light2 [i, 0], false);
				tls2[i].SettingLightY (light2 [i, 1], false);
				tls2[i].SettingLightR (light2 [i, 2], false);
			}
		}

		//歩行者用は消灯
		if (directionP1.Length != 0) {
			for (int i = 0; i < lightP1.GetLength(0); i++) {
				tlps1[i].SettingLightPR (lightP1 [i, 0], false);
				tlps1[i].SettingLightPG (lightP1 [i, 1], false);
			}
		}
		if (directionP2.Length != 0) {
			for (int i = 0; i < lightP2.GetLength(0); i++) {
				tlps2[i].SettingLightPR (lightP2 [i, 0], false);
				tlps2[i].SettingLightPG (lightP2 [i, 1], false);	
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
