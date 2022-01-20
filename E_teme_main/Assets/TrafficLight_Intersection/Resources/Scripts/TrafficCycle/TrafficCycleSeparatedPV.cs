using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	歩車分離式（スクランブル式）の信号サイクルスクリプト
 */

public class TrafficCycleSeparatedPV : TrafficCycle {

    //灯器用スクリプト
    private TrafficLightPSetting[] tlps3;

    //各方向のゲームオブジェクト
    private GameObject[] directionP3;
    //灯火のゲームオブジェクト
    private GameObject[,] lightP3;


	//車灯用の時間
	//方向1青時間、方向2青時間、黄時間、全赤時間
	public float greenTime1, greenTime2, yellowTime, allRedTime;
	//歩灯用の時間
	//青時間、点滅回数
	public float pGreenTime3;
	public int pBlinkTimes3;
    //歩灯青&点滅時間
    private float pGreenBlinkTime3;


    private float pRedTime3 = 0f;
    private int cStep3 = 2;

	//開始時のステップ(1~3)
	public int startStep = 1;
    //オフセット（時間のずれ）
    public float offset = 0;

    //経過時間ゲージをオンにするかどうか
    //public bool timeGauge = false;

    //経過時間ゲージ関係
    private float restRedTime3;

	//音響をオンにするかどうか
	public bool audioOn = false;
    private GameObject[] audioDevice1;
	public AudioClip audioClip1;

    //待ち時間表示器
    private GameObject[] timeDisplay3;


    // Use this for initialization
    void Start () {

		//信号方向の設定
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
		direction1 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("Direction2");
		direction2 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionP3");
		directionP3 = getChildObjects(tempDirection);


		//各灯器スクリプトの設定
		tls1 = new TrafficLightSetting[direction1.Length];
		tls2 = new TrafficLightSetting[direction2.Length];
		for (int i = 0; i < tls1.Length; i++) {
			tls1[i] = direction1 [i].GetComponent<TrafficLightSetting> ();
		}
		for (int i = 0; i < tls2.Length; i++) {
			tls2[i] = direction2 [i].GetComponent<TrafficLightSetting> ();
		}
		tlps3 = new TrafficLightPSetting[directionP3.Length];
		for (int i = 0; i < tlps3.Length; i++) {
			tlps3[i] = directionP3 [i].GetComponent<TrafficLightPSetting> ();
		}


		//各灯火の設定
		light1 = new GameObject[direction1.Length, 3];
		light2 = new GameObject[direction2.Length, 3];

		lightP3 = new GameObject[directionP3.Length, 2];

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
        //歩灯の灯火（0:赤灯火, 1:青灯火）のセッティング
        if (directionP3.Length != 0) {
            for (int i = 0; i < lightP3.GetLength(0); i++) {
                lightP3[i, 0] = tlps3[i].SetupLightPR(directionP3[i]);
                lightP3[i, 1] = tlps3[i].SetupLightPG(directionP3[i]);
            }
        }
        

        //青歩灯の時間に合わせて青車灯の時間を変更
        //float minimumTime1 = 0f, minimumTime2 = 0f;
        //各点灯時間未設定(0以下を指定)時はデフォルト値に変更
        if (greenTime1 <= 0) {
			greenTime1 = CONSTS.G_TIME;
		}
		if (greenTime2 <= 0) {
			greenTime2 = CONSTS.G_TIME;
		}
		if (pGreenTime3 <= 0) {
			pGreenTime3 = CONSTS.G_TIME;
		}
		if (yellowTime <= 0) {
			yellowTime = CONSTS.Y_TIME;
		}
		if (allRedTime <= 0) {
			allRedTime = CONSTS.ALLR_TIME;
		}

		//歩灯青&点滅時間
		pGreenBlinkTime3 = pGreenTime3 + CONSTS.BLINK_TIME * 2 * pBlinkTimes3;
		//赤時間(1) ＝ 青時間(2) + 黄時間 + 歩灯青&点滅時間 + 全赤時間 * 3
		redTime1 = greenTime2 + yellowTime + pGreenBlinkTime3 + allRedTime * 3;
		//赤時間(2) ＝ 青時間(1) + 黄時間 + 全赤時間
		redTime2 = greenTime1 + yellowTime + allRedTime;
		//歩灯赤時間(3) ＝ 赤時間(2) + 青時間(2) + 黄時間 + 全赤時間
		pRedTime3 = redTime2 + greenTime2 + yellowTime + allRedTime;

		//1サイクルの時間
		allTime = greenTime1 + yellowTime + redTime1;
		Debug.Log (this.transform.name + " ：" + allTime + "秒");


		//経過時間ゲージ関連
		restRedTime3 = allRedTime;


		//音響装置の設定
		if (audioOn) {
			GameObject[] tempAudioDevice;
			tempAudioDevice = GameObject.FindGameObjectsWithTag("AudioDevice1");
			audioDevice1 = getChildObjects(tempAudioDevice);

			for (int j = 0; j < audioDevice1.Length; j++) {
				audioDevice1 [j].GetComponent<AudioSource>().clip = audioClip1;
			}
		}


        //待ち時間表示器
        GameObject[] tempTimeDisplay;
        tempTimeDisplay = GameObject.FindGameObjectsWithTag("TimeDisplay3");
        timeDisplay3 = getChildObjects(tempTimeDisplay);


        //サイクル開始時の状態
        if (startStep == 1) {			//南北方向が青の場合
			cTime = 0;
			cStep3 = 2;
		} else if (startStep == 2) {	//東西方向が青の場合
			cTime = redTime2;
			cStep3 = 2;
		} else if (startStep == 3) {	//歩行者用が青の場合
			cTime = pRedTime3;
			cStep3 = 0;
			if (audioOn) {
				for (int j = 0; j < audioDevice1.Length; j++) {
					audioDevice1 [j].GetComponent<AudioSource>().Play();
				}
			}
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
			} else if (cTime < (greenTime1 + yellowTime)){
				//黄信号に
				tls1 [i].SettingLightG (light1 [i, 0], false);
				tls1 [i].SettingLightY (light1 [i, 1], true);
				tls1 [i].SettingLightR (light1 [i, 2], false);
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
			} else if(cTime < (redTime2 + greenTime2)){
				//青信号に
				tls2 [i].SettingLightG (light2 [i, 0], true);
				tls2 [i].SettingLightY (light2 [i, 1], false);
				tls2 [i].SettingLightR (light2 [i, 2], false);
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

		//方向３歩灯の信号サイクル
		if (directionP3.Length != 0) {
			if (cStep3 == 0) {				
				if (cTime < pRedTime3 + pGreenTime3) {
					for (int i = 0; i < lightP3.GetLength (0); i++) {
						//青信号に
						tlps3[i].SettingLightPR (lightP3 [i, 0], false);
						tlps3[i].SettingLightPG (lightP3 [i, 1], true);
					}
				} else {
					cStep3 = 1;		//点滅ステップへ
					if (audioOn) {
						for (int j = 0; j < audioDevice1.Length; j++) {
							audioDevice1 [j].GetComponent<AudioSource> ().Stop ();
						}
					}
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
				if (cTime > pRedTime3 + pGreenBlinkTime3 || cTime < pRedTime3) {
					for (int i = 0; i < lightP3.GetLength (0); i++) {
						//赤信号に
						tlps3[i].SettingLightPR (lightP3 [i, 0], true);
						tlps3[i].SettingLightPG (lightP3 [i, 1], false);
					}
				} else {
					cStep3 = 0;
					if (audioOn) {
						for (int j = 0; j < audioDevice1.Length; j++) {
							audioDevice1 [j].GetComponent<AudioSource> ().Play ();
						}
					}
				}
			}
		}

        //方向3の経過時間ゲージ
        if (directionP3.Length != 0) {
            for (int i = 0; i < directionP3.GetLength(0); i++) {
                if (tlps3[i].timeGaugeType != 0) {
                    if (tlps3[i].lightType[0] == 1) {
                        tlps3[i].SettingTimeGaugeR(cTime, allTime - restRedTime3, pRedTime3, pRedTime3 + allRedTime);
                    }
                    if (tlps3[i].lightType[1] == 1) {
                        tlps3[i].SettingTimeGaugeG(cTime, pRedTime3, pGreenTime3);
                    }
                }
            }
        }


        //方向3の待ち時間表示器
        if (timeDisplay3.Length != 0) {
            for (int i = 0; i < timeDisplay3.Length; i++) {
                timeDisplay3[i].GetComponent<TimeDisplaySetting>().
                    SettingTimeDisplayR(cTime, allTime - restRedTime3, pRedTime3, pRedTime3 + allRedTime);
                timeDisplay3[i].GetComponent<TimeDisplaySetting>().
                    SettingTimeDisplayG(cTime, pRedTime3, allTime - restRedTime3, pGreenTime3);
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
