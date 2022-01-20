using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	3方向の信号サイクルスクリプト
 */

public class TrafficCycle3Cycle : TrafficCycle {

    //灯器用スクリプト
    private TrafficLightSetting[] tls3;
    private TrafficLightPSetting[] tlps3;

    //各方向のゲームオブジェクト
    private GameObject[]  direction3, directionP3;
    //灯火のゲームオブジェクト
    private GameObject[,] light3, lightP3;


	//車灯用の時間
	//方向1青時間、方向2青時間、方向3青時間、黄時間、全赤時間
	public float greenTime1, greenTime2, greenTime3, yellowTime, allRedTime;
	//歩灯用の時間
	//方向1青時間、方向2青時間、方向3青時間、方向1点滅回数、方向2点滅回数、方向3点滅回数
	public float pGreenTime1, pGreenTime2, pGreenTime3;
	public int pBlinkTimes1, pBlinkTimes2, pBlinkTimes3;

    private float redTime3 = 0f,  pRedTime3 = 0f;
    private int cStep3 = 2;

	//開始時のステップ(1 or 2 or 3)
	public int startStep = 1;
    //オフセット（時間のずれ）
    public float offset = 0;

    //経過時間ゲージをオンにするかどうか
    //public bool timeGauge = false;

    //経過時間ゲージ関連
    private float restRedTime3;


	// Use this for initialization
	void Start () {

		//信号方向の設定
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
		direction1 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("Direction2");
		direction2 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("Direction3");
		direction3 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionP1");
		directionP1 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionP2");
		directionP2 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionP3");
		directionP3 = getChildObjects(tempDirection);


		//各灯器スクリプトの設定
		tls1 = new TrafficLightSetting[direction1.Length];
		tls2 = new TrafficLightSetting[direction2.Length];
		tls3 = new TrafficLightSetting[direction3.Length];
		for (int i = 0; i < tls1.Length; i++) {
			tls1[i] = direction1 [i].GetComponent<TrafficLightSetting> ();
		}
		for (int i = 0; i < tls2.Length; i++) {
			tls2[i] = direction2 [i].GetComponent<TrafficLightSetting> ();
		}
		for (int i = 0; i < tls3.Length; i++) {
			tls3[i] = direction3 [i].GetComponent<TrafficLightSetting> ();
		}
		tlps1 = new TrafficLightPSetting[directionP1.Length];
		tlps2 = new TrafficLightPSetting[directionP2.Length];
		tlps3 = new TrafficLightPSetting[directionP3.Length];
		for (int i = 0; i < tlps1.Length; i++) {
			tlps1[i] = directionP1 [i].GetComponent<TrafficLightPSetting> ();
		}
		for (int i = 0; i < tlps2.Length; i++) {
			tlps2[i] = directionP2 [i].GetComponent<TrafficLightPSetting> ();
		}
		for (int i = 0; i < tlps3.Length; i++) {
			tlps3[i] = directionP3 [i].GetComponent<TrafficLightPSetting> ();
		}


		//各灯火の設定
		light1 = new GameObject[direction1.Length, 3];
		light2 = new GameObject[direction2.Length, 3];
		light3 = new GameObject[direction3.Length, 3];
		if (directionP1.Length != 0) {
			lightP1 = new GameObject[directionP1.Length, 2];
		}
		if (directionP2.Length != 0) {
			lightP2 = new GameObject[directionP2.Length, 2];
		}
		if (directionP3.Length != 0) {
			lightP3 = new GameObject[directionP3.Length, 2];
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
        for (int i = 0; i < light3.GetLength(0); i++) {
            light3[i, 0] = tls3[i].SetupLightG(direction3[i], 0);
            light3[i, 1] = tls3[i].SetupLightY(direction3[i], 1);
            light3[i, 2] = tls3[i].SetupLightR(direction3[i], 2);
        }
        //歩灯の灯火（0:赤灯火, 1:青灯火）のセッティング
        if (directionP1.Length != 0) {
            for (int i = 0; i < lightP1.GetLength(0); i++) {
                lightP1[i, 0] = tlps1[i].SetupLightPR(directionP1[i]);
                lightP1[i, 1] = tlps1[i].SetupLightPG(directionP1[i]);
            }
        }
        if (directionP2.Length != 0) {
            for (int i = 0; i < lightP2.GetLength(0); i++) {
                lightP2[i, 0] = tlps2[i].SetupLightPR(directionP2[i]);
                lightP2[i, 1] = tlps2[i].SetupLightPG(directionP2[i]);
            }
        }
        if (directionP3.Length != 0) {
            for (int i = 0; i < lightP3.GetLength(0); i++) {
                lightP3[i, 0] = tlps3[i].SetupLightPR(directionP3[i]);
                lightP3[i, 1] = tlps3[i].SetupLightPG(directionP3[i]);
            }
        }


		//青歩灯の時間に合わせて青車灯の時間を変更
		float minimumTime1 = 0f, minimumTime2 = 0f, minimumTime3 = 0f;
		if (directionP1.Length != 0) {
			minimumTime1 = pGreenTime1 + CONSTS.BLINK_TIME * 2 * pBlinkTimes1 + CONSTS.CHANGE_TIME;
			if (greenTime1 < minimumTime1) {
				greenTime1 = minimumTime1;
			}
		} else if (greenTime1 <= 0) {
			greenTime1 = CONSTS.G_TIME;
		}
		if (directionP2.Length != 0) {
			minimumTime2 = pGreenTime2 + CONSTS.BLINK_TIME * 2 * pBlinkTimes2 + CONSTS.CHANGE_TIME;
			if (greenTime2 < minimumTime2) {
				greenTime2 = minimumTime2;
			}
		} else if (greenTime2 <= 0) {
			greenTime2 = CONSTS.G_TIME;
		}
		if (directionP3.Length != 0) {
			minimumTime3 = pGreenTime3 + CONSTS.BLINK_TIME * 2 * pBlinkTimes3 + CONSTS.CHANGE_TIME;
			if (greenTime3 < minimumTime3) {
				greenTime3 = minimumTime3;
			}
		} else if (greenTime3 <= 0) {
			greenTime3 = CONSTS.G_TIME;
		}

		//各点灯時間未設定(0以下を指定)時はデフォルト値に変更
		if (yellowTime <= 0) {
			yellowTime = CONSTS.Y_TIME;
		}
		if (allRedTime <= 0) {
			allRedTime = CONSTS.ALLR_TIME;
		}

		//赤時間(1) ＝ 青時間(2) + 青時間(3) + 黄時間 * 2 + 全赤時間 * 3
		redTime1 = greenTime2 + greenTime3 + yellowTime * 2 + allRedTime * 3;
		//赤時間(2) ＝ 青時間(1) + 黄時間 + 全赤時間
		redTime2 = greenTime1 + yellowTime + allRedTime;
		//赤時間(3) ＝ 青時間(1) + 青時間(2) + 黄時間 * 2 + 全赤時間 * 2
		redTime3 = greenTime1 + greenTime2 + yellowTime * 2 + allRedTime * 2;

		//歩灯赤時間(1)
		if (directionP1.Length != 0) {
			pRedTime1 = redTime1 + yellowTime + (greenTime1 - minimumTime1 + CONSTS.CHANGE_TIME);
		}
		//歩灯赤時間(2)
		if (directionP2.Length != 0) {
			pRedTime2 = redTime2 + (greenTime2 - minimumTime2 + CONSTS.CHANGE_TIME) + greenTime3 + yellowTime * 2 + allRedTime * 2;
		}
		//歩灯赤時間(3)
		if (directionP3.Length != 0) {
			pRedTime3 = redTime3 + (greenTime3 - minimumTime3 + CONSTS.CHANGE_TIME) + yellowTime + allRedTime;
		}

		//1サイクルの時間
		allTime = greenTime1 + yellowTime + redTime1;
		Debug.Log (this.transform.name + " ：" + allTime + "秒");


		//経過時間関連
		restRedTime2 = pRedTime2 - redTime2;
		restRedTime3 = pRedTime3 - redTime3;


		//サイクル開始時の状態
		if (startStep == 1) {			//方向1が青の場合
			cTime = 0;
			cStep1 = 0;
			cStep2 = 2;
			cStep3 = 2;
		} else if (startStep == 2) {	//方向2が青の場合
			cTime = redTime2;
			cStep1 = 2;
			cStep2 = 0;
			cStep3 = 2;
		} else if (startStep == 3) {	//方向3が青の場合
			cTime = redTime3;
			cStep1 = 2;
			cStep2 = 2;
			cStep3 = 0;
		}
        //オフセット
        cTime = cTime + offset;
    }


	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
		//方向１車灯の信号サイクル
		for(int i = 0; i < light1.GetLength(0); i++){
			if (cTime < greenTime1) {
				//青信号に
				tls1[i].SettingLightG (light1 [i, 0], true);
				tls1[i].SettingLightY (light1 [i, 1], false);
				tls1[i].SettingLightR (light1 [i, 2], false);
			} else if (cTime < (greenTime1 + yellowTime)) {
				//黄信号に
				tls1[i].SettingLightG (light1 [i, 0], false);
				tls1[i].SettingLightY (light1 [i, 1], true);
				tls1[i].SettingLightR (light1 [i, 2], false);
			} else if (cTime < allTime) {
				//赤信号に
				tls1[i].SettingLightG (light1 [i, 0], false);
				tls1[i].SettingLightY (light1 [i, 1], false);
				tls1[i].SettingLightR (light1 [i, 2], true);
			} else {
				//青信号に
				tls1[i].SettingLightG (light1 [i, 0], true);
				tls1[i].SettingLightY (light1 [i, 1], false);
				tls1[i].SettingLightR (light1 [i, 2], false);
			}
		}

		//方向２車灯の信号サイクル
		for(int i = 0; i < light2.GetLength(0); i++){
			if (cTime < redTime2) {
				//赤信号に
				tls2[i].SettingLightG (light2 [i, 0], false);
				tls2[i].SettingLightY (light2 [i, 1], false);
				tls2[i].SettingLightR (light2 [i, 2], true);
			} else if (cTime < (redTime2 + greenTime2)) {
				//青信号に
				tls2[i].SettingLightG (light2 [i, 0], true);
				tls2[i].SettingLightY (light2 [i, 1], false);
				tls2[i].SettingLightR (light2 [i, 2], false);
			} else if (cTime < (redTime2 + greenTime2 + yellowTime)) {
				//黄信号に
				tls2[i].SettingLightG (light2 [i, 0], false);
				tls2[i].SettingLightY (light2 [i, 1], true);
				tls2[i].SettingLightR (light2 [i, 2], false);
			} else {
				//赤信号に
				tls2[i].SettingLightG (light2 [i, 0], false);
				tls2[i].SettingLightY (light2 [i, 1], false);
				tls2[i].SettingLightR (light2 [i, 2], true);
			}
		}

		//方向３車灯の信号サイクル
		for(int i = 0; i < light3.GetLength(0); i++){
			if (cTime < redTime3) {
				//赤信号に
				tls3[i].SettingLightG (light3 [i, 0], false);
				tls3[i].SettingLightY (light3 [i, 1], false);
				tls3[i].SettingLightR (light3 [i, 2], true);
			} else if (cTime < (redTime3 + greenTime3)) {
				//青信号に
				tls3[i].SettingLightG (light3 [i, 0], true);
				tls3[i].SettingLightY (light3 [i, 1], false);
				tls3[i].SettingLightR (light3 [i, 2], false);
			} else if (cTime < (redTime3 + greenTime3 + yellowTime)) {
				//黄信号に
				tls3[i].SettingLightG (light3 [i, 0], false);
				tls3[i].SettingLightY (light3 [i, 1], true);
				tls3[i].SettingLightR (light3 [i, 2], false);
			} else {
				//赤信号に
				tls3[i].SettingLightG (light3 [i, 0], false);
				tls3[i].SettingLightY (light3 [i, 1], false);
				tls3[i].SettingLightR (light3 [i, 2], true);
			}
		}

		//方向１歩灯の信号サイクル
		if (directionP1.Length != 0) {
			if (cStep1 == 0) {				
				if (cTime < pGreenTime1) {
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

				if (blinkCnt == pBlinkTimes1) {
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
		}

		//方向2歩灯の信号サイクル
		if (directionP2.Length != 0) {
			if (cStep2 == 0) {
				if (cTime >= redTime2 && cTime < redTime2 + pGreenTime2) {
					for (int i = 0; i < lightP2.GetLength (0); i++) {
						//青信号に
						tlps2[i].SettingLightPR (lightP2 [i, 0], false);
						tlps2[i].SettingLightPG (lightP2 [i, 1], true);
					} 
				} else if (cTime >= redTime2 + pGreenTime2){
					cStep2 = 1;		//点滅ステップへ				
				}

			} else if (cStep2 == 1) {	//点滅ステップ

				for (int i = 0; i < lightP2.GetLength (0); i++) {
					if (cPTime >= (CONSTS.BLINK_TIME * blinkCnt * 2) && cPTime < (CONSTS.BLINK_TIME * (blinkCnt*2+1))) {
						//消灯
						tlps2[i].SettingLightPG (lightP2 [i, 1], false);
					} else {
						//青
						tlps2[i].SettingLightPG (lightP2 [i, 1], true);
					}
				}

				//点滅回数の更新
				if(cPTime >= CONSTS.BLINK_TIME * (blinkCnt*2+2)){
					blinkCnt++;
				}
				cPTime += Time.deltaTime;

				if (blinkCnt == pBlinkTimes2) {
					cPTime = 0f;
					blinkCnt = 0;
					cStep2 = 2;
				}

			} else if (cStep2 == 2) {	//赤信号ステップ
				if (cTime > redTime2 + pGreenTime2 || cTime < redTime2) {
					for (int i = 0; i < lightP2.GetLength (0); i++) {
						//赤信号に
						tlps2[i].SettingLightPR (lightP2 [i, 0], true);
						tlps2[i].SettingLightPG (lightP2 [i, 1], false);
					}
				} else {
					cStep2 = 0;
				}
			}
		}

		//方向３歩灯の信号サイクル
		if (directionP3.Length != 0) {
			if (cStep3 == 0) {				
				if (cTime >= redTime3 && cTime < redTime3 + pGreenTime3) {
					for (int i = 0; i < lightP3.GetLength (0); i++) {
						//青信号に
						tlps3[i].SettingLightPR (lightP3 [i, 0], false);
						tlps3[i].SettingLightPG (lightP3 [i, 1], true);
					}
				} else {
					cStep3 = 1;		//点滅ステップへ
				}

			} else if (cStep3 == 1) {	//点滅ステップ
				for (int i = 0; i < lightP3.GetLength (0); i++) {
					if (cPTime >= (CONSTS.BLINK_TIME * blinkCnt * 2) && cPTime < (CONSTS.BLINK_TIME * (blinkCnt*2+1))) {
						//消灯
						tlps3[i].SettingLightPG (lightP3 [i, 1], false);
					} else {
						//青
						tlps3[i].SettingLightPG (lightP3 [i, 1], true);
					}
				}

				//点滅回数の更新
				if(cPTime >= CONSTS.BLINK_TIME * (blinkCnt*2+2)){
					blinkCnt++;
				}
				cPTime += Time.deltaTime;

				if (blinkCnt == pBlinkTimes3) {
					cPTime = 0f;
					blinkCnt = 0;
					cStep3 = 2;
				}

			} else if (cStep3 == 2) {	//赤信号ステップ
				if (cTime > redTime3 + pGreenTime3 || cTime < redTime3) {
					for (int i = 0; i < lightP3.GetLength (0); i++) {
						//赤信号に
						tlps3[i].SettingLightPR (lightP3 [i, 0], true);
						tlps3[i].SettingLightPG (lightP3 [i, 1], false);
					}
				} else {
					cStep3 = 0;
				}
			}
		}

        //方向1の経過時間ゲージ
        if (directionP1.Length != 0) {
            for (int i = 0; i < directionP1.GetLength(0); i++) {
                if (tlps1[i].timeGaugeType != 0) {
                    if (tlps1[i].lightType[0] == 1) {
                        tlps1[i].SettingTimeGaugeR(cTime, allTime - pRedTime1, 0, pRedTime1);
                    }
                    if (tlps1[i].lightType[1] == 1) {
                        tlps1[i].SettingTimeGaugeG(cTime, 0, pGreenTime1);
                    }
                }
            }
        }
        //方向2の経過時間ゲージ
        if (directionP2.Length != 0) {
            for (int i = 0; i < directionP2.GetLength(0); i++) {
                if (tlps2[i].timeGaugeType != 0) {
                    if (tlps2[i].lightType[0] == 1) {
                        tlps2[i].SettingTimeGaugeR(cTime, allTime - restRedTime2, redTime2, pRedTime2);
                    }
                    if (tlps2[i].lightType[1] == 1) {
                        tlps2[i].SettingTimeGaugeG(cTime, redTime2, pGreenTime2);
                    }
                }
            }
        }
        //方向3の経過時間ゲージ
        if (directionP3.Length != 0) {
            for (int i = 0; i < directionP3.GetLength(0); i++) {
                if (tlps3[i].timeGaugeType != 0) {
                    if (tlps3[i].lightType[0] == 1) {
                        tlps3[i].SettingTimeGaugeR(cTime, allTime - restRedTime3, redTime3, pRedTime3);
                    }
                    if (tlps3[i].lightType[1] == 1) {
                        tlps3[i].SettingTimeGaugeG(cTime, redTime3, pGreenTime3);
                    }
                }
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
