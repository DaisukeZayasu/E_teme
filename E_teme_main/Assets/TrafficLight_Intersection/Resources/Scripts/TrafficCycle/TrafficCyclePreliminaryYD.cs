using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Consts;

/*	
 *	黄点滅・ディスプレイ予告信号のサイクルスクリプト
 */

public class TrafficCyclePreliminaryYD : TrafficCycle {
	//黄点滅の間隔
	public float blinkTime = 0.5f;

    private GameObject[] directionPL;
    private TrafficLightSetting[] tlsPL;
    private GameObject[,] lightPL;

	//予告のベースとなる信号機
	public GameObject baseSignal;
    private TrafficLightSetting tlsBase;

    //予告ディスプレイの設定
    //予告ディスプレイのオブジェクト
    private GameObject[] preliminaryDisplay;
    //予告ディスプレイのマテリアル
    private Material[] preliminaryDispMat = new Material[3];


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

        //車灯（0:青灯火, 1:黄灯火, 2:赤灯火）のセッティング
        for (int i = 0; i < lightPL.GetLength(0); i++) {
            lightPL[i, 0] = tlsPL[i].SetupLightG(directionPL[i], 0);
            lightPL[i, 1] = tlsPL[i].SetupLightY(directionPL[i], 1);
            lightPL[i, 2] = tlsPL[i].SetupLightR(directionPL[i], 2);
        }

        //予告のベースとなる信号機スクリプトの設定
        tlsBase = baseSignal.GetComponent<TrafficLightSetting> ();

		//予告ディスプレイの設定
		preliminaryDispMat[0] = Resources.Load("Materials/TAccessory/PreliminaryDisplayOff") as Material;
		preliminaryDispMat[1] = Resources.Load("Materials/TAccessory/PreliminaryDisplayOnY") as Material;
		preliminaryDispMat[2] = Resources.Load("Materials/TAccessory/PreliminaryDisplayOnR") as Material;


		GameObject[] tempObject;
		tempObject = GameObject.FindGameObjectsWithTag("PreliminaryDisplay");
		preliminaryDisplay = getChildObjects(tempObject);
	}

	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate (){

		//消灯ステップ
		if (tlsBase.lightOn == 0) {
			for (int i = 0; i < lightPL.GetLength (0); i++) {
				tlsPL [i].SettingLightY (lightPL [i, 0], false);
				tlsPL [i].SettingLightG (lightPL [i, 1], false);
				tlsPL [i].SettingLightY (lightPL [i, 2], false);

			}
			for (int i = 0; i < preliminaryDisplay.Length; i++) {
				preliminaryDisplay[i].transform.Find ("PreliminaryDisplayY").
					GetComponent<Renderer> ().material = preliminaryDispMat[0];
				preliminaryDisplay[i].transform.Find ("PreliminaryDisplayR").
					GetComponent<Renderer> ().material = preliminaryDispMat[0];
			}
			blinkCnt = 0;
			cTime = 0;

		} //黄点滅ステップ 
		else if (tlsBase.lightOn != 0) {
			for (int i = 0; i < lightPL.GetLength (0); i++) {
				if (blinkCnt % 2 == 0) {
					tlsPL [i].SettingLightY (lightPL [i, 0], false);
					tlsPL [i].SettingLightG (lightPL [i, 1], true);
					tlsPL [i].SettingLightY (lightPL [i, 2], false);
				} else {
					tlsPL [i].SettingLightY (lightPL [i, 0], false);
					tlsPL [i].SettingLightG (lightPL [i, 1], false);
					tlsPL [i].SettingLightY (lightPL [i, 2], false);
				}
			}
			for (int i = 0; i < preliminaryDisplay.Length; i++) {
				if (blinkCnt % 2 == 0) {
					preliminaryDisplay [i].transform.Find ("PreliminaryDisplayY").
						GetComponent<Renderer> ().material = preliminaryDispMat [0];
					preliminaryDisplay [i].transform.Find ("PreliminaryDisplayR").
						GetComponent<Renderer> ().material = preliminaryDispMat [0];
				} else {
					preliminaryDisplay [i].transform.Find ("PreliminaryDisplayY").
					GetComponent<Renderer> ().material = preliminaryDispMat [1];
					preliminaryDisplay [i].transform.Find ("PreliminaryDisplayR").
					GetComponent<Renderer> ().material = preliminaryDispMat [2];
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
