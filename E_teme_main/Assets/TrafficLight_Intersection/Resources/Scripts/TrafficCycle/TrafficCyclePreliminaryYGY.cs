using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;

/*	
 *	黄青黄予告信号のサイクルスクリプト
 */

public class TrafficCyclePreliminaryYGY : TrafficCycle {
	//黄点滅の間隔
	public float blinkTime = 0.5f;
	//黄点滅を左右どちらから始めるか
	public bool startFromRight = false;

    private GameObject[] directionPL;
    private TrafficLightSetting[] tlsPL;
    private GameObject[,] lightPL;

	//予告のベースとなる信号機
	public GameObject baseSignal;
    private TrafficLightSetting tlsBase;

	//float cTime = 0f;
	//int cStep = 0;
	//int blinkCnt = 0;


	// Use this for initialization
	void Start () {
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionPL");
		directionPL = getChildObjects(tempDirection);

		tlsPL = new TrafficLightSetting[directionPL.Length];
		for (int i = 0; i < tlsPL.Length; i++) {
			tlsPL[i] = directionPL [i].GetComponent<TrafficLightSetting> ();
		}

		lightPL = new GameObject[directionPL.Length, 3];

		//車灯(lightPL/2) 0:青灯火, 1:黄灯火, 2:赤灯火
		for (int i = 0; i < lightPL.GetLength(0); i++) {
			if (tlsPL [i].lightType[0] == 1) {
				lightPL [i, 0] = directionPL [i].transform.Find ("TLightYellow (1)").gameObject;
			} else if(tlsPL [i].lightType[0] == 0 || tlsPL[i].lightType[0] == 2) {
				lightPL [i, 0] = directionPL [i].transform.Find ("TLightGroupY (1)/PointLightY").gameObject;
			} 
            if (tlsPL[i].lightType[1] == 1) {
                lightPL[i, 1] = directionPL[i].transform.Find("TLightGreen").gameObject;
            } else if (tlsPL[i].lightType[1] == 0 || tlsPL[i].lightType[1] == 2) {
                lightPL[i, 1] = directionPL[i].transform.Find("TLightGroupG/PointLightG").gameObject;
            }
            if (tlsPL[i].lightType[2] == 1) {
                lightPL[i, 2] = directionPL[i].transform.Find("TLightYellow (2)").gameObject;
            } else if (tlsPL[i].lightType[2] == 0 || tlsPL[i].lightType[2] == 2) {
                lightPL[i, 2] = directionPL[i].transform.Find("TLightGroupY (2)/PointLightY").gameObject;
            }
        }

		//予告のベースとなる信号機スクリプトの設定
		tlsBase = baseSignal.GetComponent<TrafficLightSetting> ();

	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate (){

		//青点灯ステップ
		if (tlsBase.lightOn == 0) {
			for (int i = 0; i < lightPL.GetLength (0); i++) {
				tlsPL [i].SettingLightY (lightPL [i, 0], false);
				tlsPL [i].SettingLightG (lightPL [i, 1], true);
				tlsPL [i].SettingLightY (lightPL [i, 2], false);
			}
			blinkCnt = 0;
			cTime = 0;

		} //黄点滅ステップ 
		else if (tlsBase.lightOn != 0) {
			for (int i = 0; i < lightPL.GetLength (0); i++) {
				if (blinkCnt % 2 == 0) {
					if (startFromRight) {
						tlsPL [i].SettingLightY (lightPL [i, 0], false);
						tlsPL [i].SettingLightG (lightPL [i, 1], false);
						tlsPL [i].SettingLightY (lightPL [i, 2], true);
					} else {
						tlsPL [i].SettingLightY (lightPL [i, 0], true);
						tlsPL [i].SettingLightG (lightPL [i, 1], false);
						tlsPL [i].SettingLightY (lightPL [i, 2], false);
					}
				} else {
					if (startFromRight) {
						tlsPL [i].SettingLightY (lightPL [i, 0], true);
						tlsPL [i].SettingLightG (lightPL [i, 1], false);
						tlsPL [i].SettingLightY (lightPL [i, 2], false);
					} else {
						tlsPL [i].SettingLightY (lightPL [i, 0], false);
						tlsPL [i].SettingLightG (lightPL [i, 1], false);
						tlsPL [i].SettingLightY (lightPL [i, 2], true);
					}
				}
			}
			//点滅回数の更新
			cTime += Time.deltaTime;
			if (cTime >= blinkTime * (blinkCnt + 1)) {
				blinkCnt++;
			} 
		}

	}
}
