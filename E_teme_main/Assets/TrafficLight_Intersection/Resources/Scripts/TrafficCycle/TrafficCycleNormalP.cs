using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	通常の信号サイクルスクリプト（従道側が歩行者用のみ）
 */

public class TrafficCycleNormalP : TrafficCycle {

	//車灯用の時間
	//方向1青時間、黄時間、全赤時間
	public float greenTime1, yellowTime, allRedTime;

	//歩灯用の時間
	//方向1青時間、方向2青時間、方向1点滅回数、方向2点滅回数
	public float pGreenTime2;
	public int  pBlinkTimes2;

	//開始時のステップ(1 or 2)
	public int startStep = 1;
    //オフセット（時間のずれ）
    public float offset = 0;

    //経過時間ゲージをオンにするかどうか
    //public bool timeGauge = false;

    //音響をオンにするかどうか
    public bool audioOn = false;
    private GameObject[] audioDevice2;
	public AudioClip audioClip2;

    //待ち時間表示器
    private GameObject[] timeDisplay2;


	// Use this for initialization
	void Start () {

		//信号方向の設定
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
		direction1 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("DirectionP2");
		directionP2 = getChildObjects(tempDirection);


		//各灯器スクリプトの設定
		tls1 = new TrafficLightSetting[direction1.Length];
		for (int i = 0; i < tls1.Length; i++) {
			tls1[i] = direction1 [i].GetComponent<TrafficLightSetting> ();
		}
		tlps2 = new TrafficLightPSetting[directionP2.Length];
		for (int i = 0; i < tlps2.Length; i++) {
			tlps2[i] = directionP2 [i].GetComponent<TrafficLightPSetting> ();
		}


		//各灯火の設定
		light1 = new GameObject[direction1.Length, 3];
		if (directionP2.Length != 0) {
			lightP2 = new GameObject[directionP2.Length, 2];
		}

        //車灯（0:青灯火, 1:黄灯火, 2:赤灯火）のセッティング
        for (int i = 0; i < light1.GetLength(0); i++) {
            light1[i, 0] = tls1[i].SetupLightG(direction1[i], 0);
            light1[i, 1] = tls1[i].SetupLightY(direction1[i], 1);
            light1[i, 2] = tls1[i].SetupLightR(direction1[i], 2);
        }

        //歩灯の灯火（0:赤灯火, 1:青灯火）のセッティング
        for (int i = 0; i < lightP2.GetLength(0); i++) {
            lightP2[i, 0] = tlps2[i].SetupLightPR(directionP2[i]);
            lightP2[i, 1] = tlps2[i].SetupLightPG(directionP2[i]);
        }


        //青歩灯の時間に合わせて青車灯の時間を変更
        //float minimumTime1 = 0f, minimumTime2 = 0f;
        if (greenTime1 <= 0) {
			greenTime1 = CONSTS.G_TIME;
		}

		//各点灯時間未設定(0以下を指定)時はデフォルト値に変更
		if (yellowTime <= 0) {
			yellowTime = CONSTS.Y_TIME;
		}
		if (allRedTime <= 0) {
			allRedTime = CONSTS.ALLR_TIME;
		}

		//赤時間(1) ＝ 歩灯青時間(2) 全赤時間 * 2
		redTime1 = pGreenTime2 + CONSTS.BLINK_TIME * 2 * pBlinkTimes2 + allRedTime * 2;
		//赤時間(2) ＝ 青時間(1) + 黄時間 + 全赤時間
		redTime2 = greenTime1 + yellowTime + allRedTime;

		//歩灯赤時間(2)
		if (directionP2.Length != 0) {
			pRedTime2 = redTime2 + allRedTime;
		}

		//1サイクルの時間
		allTime = greenTime1 + yellowTime + redTime1;
		Debug.Log (this.transform.name + " ：" + allTime + "秒");


		//経過時間ゲージ
		restRedTime2 = pRedTime2 - redTime2;


		//音響装置の設定
		if (audioOn) {
			GameObject[] tempAudioDevice;
			tempAudioDevice = GameObject.FindGameObjectsWithTag("AudioDevice2");
			audioDevice2 = getChildObjects(tempAudioDevice);

			for (int j = 0; j < audioDevice2.Length; j++) {
				audioDevice2 [j].GetComponent<AudioSource>().clip = audioClip2;
			}
		}

        //待ち時間表示器
        GameObject[] tempTimeDisplay;
        tempTimeDisplay = GameObject.FindGameObjectsWithTag("TimeDisplay2");
        timeDisplay2 = getChildObjects(tempTimeDisplay);



		//サイクル開始時の状態
		if (startStep == 1) {			//南北方向が青の場合
			cTime = 0;
			cStep1 = 0;
			cStep2 = 2;
		} else if (startStep == 2) {	//東西方向が青の場合
			cTime = redTime2;
			cStep1 = 2;
			cStep2 = 0;
			if (audioOn) {
				for (int j = 0; j < audioDevice2.Length; j++) {
					audioDevice2 [j].GetComponent<AudioSource>().Play();
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
				tls1 [i].SettingLightG (light1 [i, 0], true);
				tls1 [i].SettingLightY (light1 [i, 1], false);
				tls1 [i].SettingLightR (light1 [i, 2], false);
			} else if (cTime < (greenTime1 + yellowTime)) {
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


		//方向2歩灯の信号サイクル

		if (cStep2 == 0) {
			if (cTime >= redTime2 && cTime < redTime2 + pGreenTime2) {
				for (int i = 0; i < lightP2.GetLength (0); i++) {
					//青信号に
					tlps2[i].SettingLightPR (lightP2 [i, 0], false);
					tlps2[i].SettingLightPG (lightP2 [i, 1], true);
				} 
			} else if (cTime >= redTime2 + pGreenTime2){
				cStep2 = 1;		//点滅ステップへ	
				if (audioOn) {
					for (int j = 0; j < audioDevice2.Length; j++) {
						audioDevice2 [j].GetComponent<AudioSource> ().Stop ();
					}
				}
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
				if (audioOn) {
					for (int j = 0; j < audioDevice2.Length; j++) {
						audioDevice2 [j].GetComponent<AudioSource> ().Play ();
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

        //方向2の待ち時間表示器
        if (timeDisplay2.Length != 0) {
            for(int i = 0; i < timeDisplay2.Length; i++) {
                timeDisplay2[i].GetComponent<TimeDisplaySetting>().
                    SettingTimeDisplayR(cTime, allTime - restRedTime2, redTime2, pRedTime2);
                timeDisplay2[i].GetComponent<TimeDisplaySetting>().
                    SettingTimeDisplayG(cTime, redTime2, allTime - restRedTime2, pGreenTime2);
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
