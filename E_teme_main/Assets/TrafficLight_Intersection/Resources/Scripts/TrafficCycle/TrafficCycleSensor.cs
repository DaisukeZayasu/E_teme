using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Consts;

/*	
 *	車両感応式の信号サイクルスクリプト
 */

public class TrafficCycleSensor : TrafficCycle {

	//感知後のサイクルモード（0：すぐ変化、1：車両用の最低限の青時間を待ってから変化、2：一定の信号サイクルに合わせて変化）
	public int cycleMode = 2;
	//感知してから切り替わる時間、最低限青時間 (モード1の時)
	public float changeTime, minimumGreenTime1;


	//車灯用の時間
	//方向1青時間、方向2青時間、黄時間、全赤時間
	public float greenTime1, greenTime2, yellowTime, allRedTime;
	//歩灯用の時間
	//方向1青時間、方向2青時間、方向1点滅回数、方向2点滅回数
	public float pGreenTime1, pGreenTime2;
	public int pBlinkTimes1, pBlinkTimes2;

    //感知しているかしていないかのステップ
    private int cStep = 0;
    //cTime:黄点滅用カウンタ, cTime2:ボタンを押してからのカウンタ
    //cTime3:最低限の青時間用のボタンを押すまでのカウンタ, cTime4:連動用のサイクル全体のカウンタ
    private float cTime2 = 0f, cTime3 = 0f, cTime4 = 0f;
    private float updateGreenTime1, updatePGreenTime1;  //車灯が実際に変わる時間	
    private float afterSensedTime;						//車を感知してからの時間


	//開始時のステップ(1 or 2)
	public int startStep = 1;


    //感知範囲円柱のオブジェクト
    private GameObject[] carSensorCylinder;
    //感知器ディスプレイのオブジェクト
    private GameObject[] carSensorDisplay;
    //感知器ディスプレイのマテリアル
    private Material[] carSensorDispMat = new Material[2];
    //感知できる状態かどうか
    private bool canSense = true;

    //押しボタンのオブジェクト
    private GameObject[] pushButton;
    //押しボタンディスプレイのマテリアル
    private Material[] buttonDisplay = new Material[2];

    //人型オブジェクト
    private GameObject human;
    private Text canPushText;

	//音響をオンにするかどうか
	public bool audioOn = false;
    private GameObject[] audioDevice1, audioDevice2;
	public AudioClip audioClip1, audioClip2;



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

        //青歩灯の時間に合わせて青車灯の時間を変更
        float minimumTime1 = 0f, minimumTime2 = 0f;
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

		//各点灯時間未設定(0以下を指定)時はデフォルト値に変更

		if (changeTime <= 0) {
			changeTime = CONSTS.G_TIME;
		}
		if(minimumGreenTime1 <= changeTime){
			minimumGreenTime1 = changeTime;
		}
		if (yellowTime <= 0) {
			yellowTime = CONSTS.Y_TIME;
		}
		if (allRedTime <= 0) {
			allRedTime = CONSTS.ALLR_TIME;
		}

		//赤時間(1) ＝ 青時間(2) + 黄時間 + 全赤時間 * 2
		redTime1 = greenTime2 + yellowTime + allRedTime * 2;

		//赤時間(2) ※ボタンを押してからの赤時間
		redTime2 = changeTime + CONSTS.BLINK_TIME * 2 * pBlinkTimes1 + CONSTS.CHANGE_TIME + yellowTime + allRedTime;
		//1サイクルの時間
		afterSensedTime = redTime2 + redTime1 - allRedTime;
		allTime = greenTime1 + yellowTime + redTime1;
		if (cycleMode == 2) {
			Debug.Log (this.transform.name + " ：" + allTime + "秒");
		}


		//初期設定では未感知状態
		updateGreenTime1 = allTime;
		if (directionP1.Length != 0) {
			updatePGreenTime1 = allTime;
		} 
		//赤時間(2) ＝ 青時間(1) + 黄時間 + 全赤時間
		//redTime2 = updateGreenTime1 + yellowTime + allRedTime;


		//感知器の設定
		carSensorDispMat[0] = Resources.Load("Materials/TAccessory/CarSensorDisplayOff") as Material;
		carSensorDispMat[1] = Resources.Load("Materials/TAccessory/CarSensorDisplayOn") as Material;
		GameObject[] tempSensor;
		tempSensor = GameObject.FindGameObjectsWithTag("CarSensorCylinder");
		carSensorCylinder = getChildObjects(tempSensor);
		tempSensor = GameObject.FindGameObjectsWithTag("CarSensorDisplay");
		carSensorDisplay = getChildObjects(tempSensor);
		//carSensorCylinder = GameObject.FindGameObjectsWithTag("CarSensorCylinder");
		//carSensorDisplay = GameObject.FindGameObjectsWithTag("CarSensorDisplay");
		for (int i = 0; i < carSensorDisplay.Length; i++) {
			carSensorDisplay [i].GetComponent<Renderer> ().material = carSensorDispMat [0];
		}

		//押しボタンの設定
		buttonDisplay[0] = Resources.Load("Materials/TAccessory/ButtonDisplayOff") as Material;
		buttonDisplay[1] = Resources.Load("Materials/TAccessory/ButtonDisplayOn") as Material;

		//pushButton = GameObject.FindGameObjectsWithTag("PushButton");
		GameObject[] tempButton;
		tempButton = GameObject.FindGameObjectsWithTag("PushButton");
		pushButton = getChildObjects(tempButton);
		for (int i = 0; i < pushButton.GetLength (0); i++) {
			pushButton[i].transform.Find("ButtonDisplay").GetComponent<Renderer> ().material = buttonDisplay[0];
		}

		//人型オブジェクトの設定
		human = GameObject.FindGameObjectWithTag("Human");
		canPushText = GameObject.Find ("CanPushText").GetComponent<Text> ();


		//音響装置の設定
		if (audioOn) {
			GameObject[] tempAudioDevice;
			tempAudioDevice = GameObject.FindGameObjectsWithTag("AudioDevice1");
			audioDevice1 = getChildObjects(tempAudioDevice);
			tempAudioDevice = GameObject.FindGameObjectsWithTag("AudioDevice2");
			audioDevice2 = getChildObjects(tempAudioDevice);

			for (int j = 0; j < audioDevice1.Length; j++) {
				audioDevice1 [j].GetComponent<AudioSource>().clip = audioClip1;
			}
			for (int j = 0; j < audioDevice2.Length; j++) {
				audioDevice2 [j].GetComponent<AudioSource>().clip = audioClip2;
			}
		}
			
		//サイクル開始時の状態
		if (cycleMode == 2) {
			if (startStep == 1) {			
				cStep = 0;
				cStep2 = 2;
				if (audioOn) {
					for (int j = 0; j < audioDevice1.Length; j++) {
						audioDevice1 [j].GetComponent<AudioSource>().Play();
					}
				}
			} else if (startStep == 2) {
				SetSensorAndButton (1);
				SetSensorAndButton (0);
				cTime2 = redTime2;
				cTime4 = greenTime1 + yellowTime +  allRedTime;
				cStep = 1;
				cStep1 = 2;
				cStep2 = 0;
				if (audioOn) {
					for (int j = 0; j < audioDevice2.Length; j++) {
						audioDevice2 [j].GetComponent<AudioSource>().Play();
					}
				}
			}
		}
	}


	void FixedUpdate(){
		//Debug.Log (cTime);

		//方向１車灯の信号サイクル
		//感知するまで
		if (cStep == 0) {
			for (int i = 0; i < light1.GetLength (0); i++) {
				//青信号
				tls1[i].SettingLightG (light1 [i, 0], true);
				tls1[i].SettingLightY (light1 [i, 1], false);
				tls1[i].SettingLightR (light1 [i, 2], false);
			}
			cTime3 += Time.deltaTime;

			//感知した後
		} else if(cStep == 1){
			for (int i = 0; i < light1.GetLength (0); i++) {
				if(cTime2 < updateGreenTime1){
					//青信号に
					tls1[i].SettingLightG (light1 [i, 0], true);
					tls1[i].SettingLightY (light1 [i, 1], false);
					tls1[i].SettingLightR (light1 [i, 2], false);
				} else if (cTime2 < (updateGreenTime1 + yellowTime)) {
					//黄信号に
					tls1[i].SettingLightG (light1 [i, 0], false);
					tls1[i].SettingLightY (light1 [i, 1], true);
					tls1[i].SettingLightR (light1 [i, 2], false);
				} else if (cTime2 >= (updateGreenTime1 + yellowTime)) {
					//赤信号に
					tls1[i].SettingLightG (light1 [i, 0], false);
					tls1[i].SettingLightY (light1 [i, 1], false);
					tls1[i].SettingLightR (light1 [i, 2], true);
				}
			}

			//サイクル時間のリセットと更新
			if (cTime2 >= afterSensedTime) {
				cTime2 = 0f;
				cStep = 0;
				if (canSense == false) {
					SetSensorAndButton (-1);
				}
			} else {
				cTime2 += Time.deltaTime;
			}
		}


		//方向２車灯の信号サイクル
		if (cStep == 0) {
			for (int i = 0; i < light2.GetLength (0); i++) {
				tls2 [i].SettingLightG (light2 [i, 0], false);
				tls2 [i].SettingLightY (light2 [i, 1], false);
				tls2 [i].SettingLightR (light2 [i, 2], true);
			}

			//ボタンが押された後
		} else if (cStep == 1) {
			for (int i = 0; i < light2.GetLength (0); i++) {
				if (cTime2 < redTime2) {
					//赤信号に
					tls2 [i].SettingLightG (light2 [i, 0], false);
					tls2 [i].SettingLightY (light2 [i, 1], false);
					tls2 [i].SettingLightR (light2 [i, 2], true);
				} else if (cTime2 < (redTime2 + greenTime2)) {
					//青信号に
					tls2 [i].SettingLightG (light2 [i, 0], true);
					tls2 [i].SettingLightY (light2 [i, 1], false);
					tls2 [i].SettingLightR (light2 [i, 2], false);
					//感知器ディスプレイをオフに
					SetSensorAndButton (0);

				} else if (cTime2 < (redTime2 + greenTime2 + yellowTime)) {
					//黄信号に
					tls2 [i].SettingLightG (light2 [i, 0], false);
					tls2 [i].SettingLightY (light2 [i, 1], true);
					tls2 [i].SettingLightR (light2 [i, 2], false);
				} else {
					//赤信号に
					tls2 [i].SettingLightG (light2 [i, 0], false);
					tls2 [i].SettingLightY (light2 [i, 1], false);
					tls2 [i].SettingLightR (light2 [i, 2], true);
				}
			}
		}


		//方向１歩灯の信号サイクル
		if (directionP1.Length != 0) {
			if (cStep1 == 0) {
				if (cTime2 < updatePGreenTime1) {
					for (int i = 0; i < lightP1.GetLength (0); i++) {
						//青信号に
						tlps1[i].SettingLightPR (lightP1 [i, 0], false);
						tlps1[i].SettingLightPG (lightP1 [i, 1], true);
					}
				} else {
					cStep1 = 1;		//点滅ステップへ
					if (audioOn) {
						for (int j = 0; j < audioDevice1.Length; j++) {
							audioDevice1 [j].GetComponent<AudioSource> ().Stop ();
						}
					}
				}

			} else if (cStep1 == 1) {	//点滅ステップ
				for (int i = 0; i < lightP1.GetLength (0); i++) {
					if (cPTime >= (CONSTS.BLINK_TIME * blinkCnt * 2) && cPTime < (CONSTS.BLINK_TIME * (blinkCnt * 2 + 1))) {
						//消灯
						tlps1[i].SettingLightPG (lightP1 [i, 1], false);
					} else {
						//青
						tlps1[i].SettingLightPG (lightP1 [i, 1], true);
					}
				}

				//点滅回数の更新
				if (cPTime >= CONSTS.BLINK_TIME * (blinkCnt * 2 + 2)) {
					blinkCnt++;
				}
				cPTime += Time.deltaTime;

				if (blinkCnt == pBlinkTimes1) {

					cPTime = 0f;
					blinkCnt = 0;
					cStep1 = 2;
				}

			} else if (cStep1 == 2) {	//赤信号ステップ
				if (cTime2 > 1) {
					for (int i = 0; i < lightP1.GetLength (0); i++) {
						//赤信号に
						tlps1[i].SettingLightPR (lightP1 [i, 0], true);
						tlps1[i].SettingLightPG (lightP1 [i, 1], false);
					}
				} else {
					cStep1 = 0;
					if (audioOn) {
						for (int j = 0; j < audioDevice1.Length; j++) {
							audioDevice1 [j].GetComponent<AudioSource> ().Play ();
						}
					}
				}
			}
		}

		//方向2歩灯の信号サイクル
		if (directionP2.Length != 0) {
			if (cStep2 == 0) {
				if (cTime2 >= redTime2 && cTime2 < redTime2 + pGreenTime2) {
					for (int i = 0; i < lightP2.GetLength (0); i++) {
						//青信号に
						tlps2[i].SettingLightPR (lightP2 [i, 0], false);
						tlps2[i].SettingLightPG (lightP2 [i, 1], true);
					} 
				} else if (cTime2 >= redTime2 + pGreenTime2){
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
				if (cTime2 > redTime2 + pGreenTime2 || cTime2 < redTime2) {
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
		}

		//連動用の時間
		if (cTime4 >= allTime) {
			cTime4 = 0f;
		} else {
			cTime4 += Time.deltaTime;
		}


	}



    //感知器と押ボタンの設定
    private void SetSensorAndButton(int sw){
		//感知器のスクリプト
		CarSensor cses;
		float limitTime;

		//センサーと押ボタンを感知状態に
		if (sw == 1) {
			for (int i = 0; i < carSensorDisplay.Length; i++) {
				carSensorDisplay [i].GetComponent<Renderer> ().material = carSensorDispMat [1];
			}
			for (int i = 0; i < pushButton.GetLength (0); i++) {
				pushButton[i].transform.Find("ButtonDisplay").GetComponent<Renderer> ().material = buttonDisplay[1];
			}
			if (directionP1.Length != 0) {
				limitTime = pGreenTime1;
			} else {
				limitTime = greenTime1;
			}

			cStep = 1;

			//0：すぐに変化
			if(cycleMode == 0){
				updateGreenTime1 = changeTime;
				if (directionP1.Length != 0) {
					updatePGreenTime1 = changeTime;
					updateGreenTime1 = updatePGreenTime1 + CONSTS.BLINK_TIME * 2 * pBlinkTimes1 + CONSTS.CHANGE_TIME;
				} 

			//1：車両用の最低限の青時間を待ってから変化
			} else if (cycleMode == 1) {
				
				if ((cTime3 + changeTime) < minimumGreenTime1) {
					updateGreenTime1 = (minimumGreenTime1 - cTime3);
					if (directionP1.Length != 0) {
						updatePGreenTime1 = (minimumGreenTime1 - cTime3);
						updateGreenTime1 = updatePGreenTime1 + CONSTS.BLINK_TIME * 2 * pBlinkTimes1 + CONSTS.CHANGE_TIME;
					} 
				} else {
					updateGreenTime1 = changeTime;
					if (directionP1.Length != 0) {
						updatePGreenTime1 = changeTime;
						updateGreenTime1 = updatePGreenTime1 + CONSTS.BLINK_TIME * 2 * pBlinkTimes1 + CONSTS.CHANGE_TIME;
					}

				}
				redTime2 = updateGreenTime1 + yellowTime + allRedTime;
				//1サイクルの時間の更新
				afterSensedTime = redTime2 + redTime1 - allRedTime;


			//2：一定の信号サイクルに合わせて変化
			} else if (cycleMode == 2) {
				//感知時の赤になる時間より早ければ、感知状態のサイクルに変更。遅ければ、次回のサイクルで青にする。
				if (cTime4 < limitTime) {
					updateGreenTime1 = (greenTime1 - cTime4);
					if (directionP1.Length != 0) {
						updatePGreenTime1 = (pGreenTime1 - cTime4);
						updateGreenTime1 = updatePGreenTime1 + CONSTS.BLINK_TIME * 2 * pBlinkTimes1 + CONSTS.CHANGE_TIME;
					} 
				} else {
					updateGreenTime1 = (allTime - cTime4) + greenTime1;
					if (directionP1.Length != 0) {
						updatePGreenTime1 = (allTime - cTime4) + pGreenTime1;
						updateGreenTime1 = updatePGreenTime1 + CONSTS.BLINK_TIME * 2 * pBlinkTimes1 + CONSTS.CHANGE_TIME;
					} 
				}
				redTime2 = updateGreenTime1 + yellowTime + allRedTime;
				//1サイクルの時間の更新
				afterSensedTime = redTime2 + redTime1 - allRedTime;

			}
			canSense = false;

			//センサーと押ボタンのディスプレイを消す
		} else if (sw == 0) {
			for (int i = 0; i < carSensorDisplay.Length; i++) {
				carSensorDisplay [i].GetComponent<Renderer> ().material = carSensorDispMat [0];
			}
			for (int i = 0; i < pushButton.GetLength (0); i++) {
				pushButton[i].transform.Find("ButtonDisplay").GetComponent<Renderer> ().material = buttonDisplay[0];
			}

			//センサーを未感知状態に
		} else if (sw == -1) {
			for (int i = 0; i < carSensorCylinder.Length; i++) {
				cses = carSensorCylinder[i].GetComponent<CarSensor> ();
				cses.sensed = false;
			}
			updateGreenTime1 = allTime;
			if (directionP1.Length != 0) {
				updatePGreenTime1 = allTime;
			} 
			redTime2 = updateGreenTime1 + yellowTime + allRedTime;

			canSense = true;
		}

	}


	// Update is called once per frame
	void Update () {
		//車の感知
		CarSensor cses;
		for (int i = 0; i < carSensorCylinder.Length; i++) {
			cses = carSensorCylinder[i].GetComponent<CarSensor> ();
			if (cses.sensed == true && canSense == true) {
				SetSensorAndButton (1);
			}
		}

		//ボタンが押せる位置かどうか
		bool canPush = false;
		for (int i = 0; i < pushButton.GetLength (0); i++) {
			float xLeft = pushButton [i].transform.position.x - 1;
			float xRight = pushButton [i].transform.position.x + 1;
			float zFront = pushButton [i].transform.position.z - 1;
			float zBack = pushButton [i].transform.position.z + 1;

			if (human.transform.position.x >= xLeft && human.transform.position.x <= xRight 
				&& human.transform.position.z >= zFront && human.transform.position.z <= zBack) {
				canPush = true;
			}
		}
		if (canPush) {		//ボタンが押せる位置で
			if (canSense) {	//ボタンが押されていない＆感知されていなければ
				//spaceキーでボタンを押す
				if (Input.GetKey (KeyCode.Space)) {
					for (int i = 0; i < carSensorCylinder.Length; i++) {
						cses = carSensorCylinder[i].GetComponent<CarSensor> ();
						cses.sensed = true;
					}
					SetSensorAndButton (1);
				}
			}
			canPushText.text = "ボタンを押す(スペースキー)";
		} else {
			canPushText.text = "";
		}

	}
}
