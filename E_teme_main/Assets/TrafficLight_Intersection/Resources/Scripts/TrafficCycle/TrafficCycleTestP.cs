using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*
 * 歩灯信号サイクルのテスト用
 */

public class TrafficCycleTestP : TrafficCycle {

	public float pGreenTime;
	public int pBlinkTimes;
	public float pRedTime;

    //経過時間ゲージをオンにするかどうか
    //public bool timeGauge = false;

    //待ち時間表示器
    private GameObject[] timeDisplay1;


    // Use this for initialization
    void Start () {
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionP1");
		directionP1 = getChildObjects(tempDirection);

		tlps1 = new TrafficLightPSetting[directionP1.Length];
		for (int i = 0; i < tlps1.Length; i++) {
			tlps1[i] = directionP1 [i].GetComponent<TrafficLightPSetting> ();
		}

		lightP1 = new GameObject[directionP1.Length, 2];

        //歩灯の灯火（0:赤灯火, 1:青灯火）のセッティング
        for (int i = 0; i < lightP1.GetLength(0); i++) {
            lightP1[i, 0] = tlps1[i].SetupLightPR(directionP1[i]);
            lightP1[i, 1] = tlps1[i].SetupLightPG(directionP1[i]);
        }

       
        //cStep1 = 0;
        allTime = pGreenTime + CONSTS.BLINK_TIME * 2 * pBlinkTimes + pRedTime;
		Debug.Log (this.transform.name + " ：" + allTime + "秒");

        //待ち時間表示器
        GameObject[] tempTimeDisplay;
        tempTimeDisplay = GameObject.FindGameObjectsWithTag("TimeDisplay1");
        timeDisplay1 = getChildObjects(tempTimeDisplay);

    }


	// Update is called once per frame
	void Update () {
		
	}


	void FixedUpdate(){
		//方向１歩灯の信号サイクル
		if (cStep1 == 0) {				
			if (cTime < pGreenTime) {
				for (int i = 0; i < lightP1.GetLength (0); i++) {
					//青信号に
					tlps1[i].SettingLightPR (lightP1 [i, 0], false);
					tlps1[i].SettingLightPG (lightP1 [i, 1], true);
				}
			} else {
				cStep1 = 1;		//点滅ステップへ
			}

		} else if (cStep1 == 1) {	//点滅ステップ
			for (int i = 0; i < lightP1.GetLength (0); i++) {
				if (cPTime >= (CONSTS.BLINK_TIME * blinkCnt * 2) && cPTime < (CONSTS.BLINK_TIME * (blinkCnt*2+1))) {
					//消灯
					tlps1[i].SettingLightPG (lightP1 [i, 1], false);
				} else {
					//青
					tlps1[i].SettingLightPG (lightP1 [i, 1], true);
				}
			}

			//点滅回数の更新
			if(cPTime >= CONSTS.BLINK_TIME * (blinkCnt*2+2)){
				blinkCnt++;
			}
			cPTime += Time.deltaTime;

			if (blinkCnt == pBlinkTimes) {
				cPTime = 0f;
				blinkCnt = 0;
				cStep1 = 2;
			}

		} else if (cStep1 == 2) {	//赤信号ステップ
			if (cTime > 1) {
				for (int i = 0; i < lightP1.GetLength (0); i++) {
					//赤信号に
					tlps1[i].SettingLightPR (lightP1 [i, 0], true);
					tlps1[i].SettingLightPG (lightP1 [i, 1], false);
				}
			} else {
				cStep1 = 0;
			}
		}

        //方向1の経過時間ゲージ      
        for (int i = 0; i < directionP1.GetLength(0); i++) {
            if (tlps1[i].timeGaugeType != 0) {
                if (tlps1[i].lightType[0] == 1) {
                    tlps1[i].SettingTimeGaugeR(cTime, allTime - pRedTime, 0, pRedTime);
                    tlps1[i].SettingTimeGaugeR(cTime, allTime - pRedTime, 0, pRedTime);
                }
                if (tlps1[i].lightType[1] == 1) {
                    tlps1[i].SettingTimeGaugeG(cTime, 0, pGreenTime);
                    tlps1[i].SettingTimeGaugeG(cTime, 0, pGreenTime);
                }
            }
        }

        //方向1の待ち時間表示器
        if (timeDisplay1.Length != 0) {
            for (int i = 0; i < timeDisplay1.Length; i++) {
                timeDisplay1[i].GetComponent<TimeDisplaySetting>().
                    SettingTimeDisplayR(cTime, allTime - pRedTime, 0, pRedTime);
                timeDisplay1[i].GetComponent<TimeDisplaySetting>().
                    SettingTimeDisplayG(cTime, 0, allTime - pRedTime, pGreenTime);
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
