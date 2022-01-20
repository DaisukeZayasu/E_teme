using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Consts;

/*	
 *	押ボタン式(両方点滅)の信号サイクルスクリプト
 */

public class TrafficCycleButtonBlinking : TrafficCycle {

	//ボタンを押した後のサイクルモード（0：すぐ変化、1：車両用の最低限の青時間を待ってから変化、2：一定の信号サイクルに合わせて変化）
	public int cycleMode = 0;
	//ボタンを押してから切り替わる時間、車両用の最低限青時間 (モード1の時)
	public float changeTime, minimumGreenTime1;
	// (モード2の時)
	public float greenTime1;
	public float greenTime2, yellowTime, allRedTime;
	//歩灯青時間、点滅回数
	public float pGreenTime2;
	public int pBlinkTimes2;

    //ボタンを押しているか押していないかのステップ
    private int cStep = 0;
    //cTime:黄点滅用カウンタ, cTime2:ボタンを押してからのカウンタ
    //cTime3:最低限の青時間用のボタンを押すまでのカウンタ, cTime4:連動用のサイクル全体のカウンタ
    private float cTime2 = 0f, cTime3 = 0f, cTime4 = 0f;
    private float updateGreenTime1;     //車灯が実際に変わる時間	
    private float afterPushTime;		//押ボタンが押された後の時間

	//開始時のステップ(1 or 2)(モード2の時)
	public int startStep = 1;

    //点滅間隔
    private float blinkTime = 0.5f;
    //車灯の状態(true:黄点滅, false:青)
    //bool blinking = true;


    //押しボタンのオブジェクト
    private GameObject[] pushButton;
    //ボタンの状態
    private bool pushed = false;
    //押しボタンディスプレイのマテリアル
    private Material[] buttonDisplay = new Material[2];

    //人型オブジェクト
    private GameObject human;
    private Text canPushText;

	//音響をオンにするかどうか
	public bool audioOn = false;
    private GameObject[] audioDevice2;
	public AudioClip audioClip2;



	// Use this for initialization
	void Start () {

		//信号方向の設定
		GameObject[] tempDirection;
		tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
		direction1 = getChildObjects(tempDirection);
		tempDirection = GameObject.FindGameObjectsWithTag("Direction2");
		direction2 = getChildObjects(tempDirection);
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
		tlps2 = new TrafficLightPSetting[directionP2.Length];
		for (int i = 0; i < tlps2.Length; i++) {
			tlps2[i] = directionP2 [i].GetComponent<TrafficLightPSetting> ();
		}

		//各灯火の設定
		light1 = new GameObject[direction1.Length, 3];
		light2 = new GameObject[direction2.Length, 3];
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
        if (directionP2.Length != 0) {
            for (int i = 0; i < lightP2.GetLength(0); i++) {
                lightP2[i, 0] = tlps2[i].SetupLightPR(directionP2[i]);
                lightP2[i, 1] = tlps2[i].SetupLightPG(directionP2[i]);
            }
        }


        //サイクルモードが不正値の場合1に
        if (cycleMode < 0 || cycleMode > 2) {
			cycleMode = 1;
		}

		//各点灯時間未設定(0以下を指定)時はデフォルト値に変更
		if (changeTime <= 0) {
			changeTime = CONSTS.G_TIME;
		}
		if(minimumGreenTime1 <= changeTime){
			minimumGreenTime1 = changeTime;
		}



		//青歩灯の時間に合わせて青車灯の時間を変更
		float minimumTime2 = 0f;
		if (directionP2.Length != 0) {
			minimumTime2 = pGreenTime2 + CONSTS.BLINK_TIME * 2 * pBlinkTimes2 + CONSTS.CHANGE_TIME;
			if (greenTime2 < minimumTime2) {
				greenTime2 = minimumTime2;
			}
		} else if (greenTime2 <= 0) {
			greenTime2 = CONSTS.G_TIME;
		}

		if (yellowTime <= 0) {
			yellowTime = CONSTS.Y_TIME;
		}
		if (allRedTime <= 0) {
			allRedTime = CONSTS.ALLR_TIME;
		}

		//赤時間(1)
		redTime1 = greenTime2 + yellowTime + allRedTime * 2;
		//赤時間(2) ※ボタンを押してからの赤時間
		redTime2 = changeTime + yellowTime + allRedTime;
		//1サイクルの時間
		afterPushTime = redTime2 + redTime1 - allRedTime;
		allTime = greenTime1 + yellowTime + redTime1;
		if (cycleMode == 2) {
			Debug.Log (this.transform.name + " ：" + allTime + "秒");
		}


		//押しボタンディスプレイのマテリアルの設定
		buttonDisplay[0] = Resources.Load("Materials/TAccessory/ButtonDisplayOff") as Material;
		buttonDisplay[1] = Resources.Load("Materials/TAccessory/ButtonDisplayOn") as Material;

		//押しボタン箱・ディスプレイの設定
		GameObject[] tempButton;
		tempButton = GameObject.FindGameObjectsWithTag("PushButton");
		pushButton = getChildObjects(tempButton);
		//pushButton = GameObject.FindGameObjectsWithTag("PushButton");
		for (int i = 0; i < pushButton.GetLength (0); i++) {
			pushButton[i].transform.Find("ButtonDisplay").GetComponent<Renderer> ().material = buttonDisplay[0];
		}

		//人型オブジェクトの設定
		human = GameObject.FindGameObjectWithTag("Human");
		canPushText = GameObject.Find ("CanPushText").GetComponent<Text> ();


		//音響装置の設定
		if (audioOn) {
			GameObject[] tempAudioDevice;
			tempAudioDevice = GameObject.FindGameObjectsWithTag("AudioDevice2");
			audioDevice2 = getChildObjects(tempAudioDevice);

			for (int j = 0; j < audioDevice2.Length; j++) {
				audioDevice2 [j].GetComponent<AudioSource>().clip = audioClip2;
			}
		}


		//サイクル開始時の状態
		if (cycleMode == 2) {
			if (startStep == 1) {			
				cStep = 0;
				cStep2 = 2;
			} else if (startStep == 2) {
				pushed = true;	
				cTime2 = redTime2;
				cTime4 = greenTime1 + yellowTime +  allRedTime;
				cStep = 1;
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

		//方向１車灯の信号サイクル
		//ボタンが押されるまで
		if (cStep == 0) {
			for (int i = 0; i < light1.GetLength (0); i++) {
			
				if (cTime >= blinkTime && cTime < blinkTime * 2) {
					//黄点灯
					tls1[i].SettingLightG (light1 [i, 0], false);
					tls1[i].SettingLightY (light1 [i, 1], true);
					tls1[i].SettingLightR (light1 [i, 2], false);
				} else {
					//消灯
					tls1[i].SettingLightG (light1 [i, 0], false);
					tls1[i].SettingLightY (light1 [i, 1], false);
					tls1[i].SettingLightR (light1 [i, 2], false);
				} 
			}
			//サイクル時間のリセットと更新
			if (cTime >= blinkTime * 2) {
				cTime = 0f;
			} else {
				cTime += Time.deltaTime;
			}
			cTime3 += Time.deltaTime;

			//ボタンが押された後
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
			if (cTime2 >= afterPushTime) {
				cTime2 = 0f;
				cTime = 0f;
				cStep = 0;
			} else {
				cTime2 += Time.deltaTime;
			}
		}

		//方向２車灯の信号サイクル
		if (cStep == 0) {
			for (int i = 0; i < light2.GetLength (0); i++) {

				if (cTime >= blinkTime && cTime < blinkTime * 2) {
					//消灯
					tls2 [i].SettingLightG (light2 [i, 0], false);
					tls2 [i].SettingLightY (light2 [i, 1], false);
					tls2 [i].SettingLightR (light2 [i, 2], false);
				} else {
					//赤点灯
					tls2 [i].SettingLightG (light2 [i, 0], false);
					tls2 [i].SettingLightY (light2 [i, 1], false);
					tls2 [i].SettingLightR (light2 [i, 2], true);
				} 
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


		//方向2歩灯の信号サイクル
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
			//「おまちください」を消す
			for (int i = 0; i < pushButton.GetLength (0); i++) {
				//pushButton [i].transform.Find ("ButtonText").GetComponent<TextMesh> ().text = "";
				pushButton[i].transform.Find("ButtonDisplay").GetComponent<Renderer> ().material = buttonDisplay[0];
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
				pushed = false;
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

		//連動用の時間
		if (cTime4 >= allTime) {
			cTime4 = 0f;
		} else {
			cTime4 += Time.deltaTime;
		}

	}



	// Update is called once per frame
	void Update () {
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
			if (!pushed) {	//ボタンが押されていなければ
				//spaceキーでボタンを押す
				if (Input.GetKey (KeyCode.Space)) {
					//「おまちください」を表示
					for (int i = 0; i < pushButton.GetLength (0); i++) {
						pushButton[i].transform.Find("ButtonDisplay").GetComponent<Renderer> ().material = buttonDisplay[1];
					}
					cStep = 1;
					pushed = true;

					//0：すぐ変化
					if (cycleMode == 0) {
							updateGreenTime1 = changeTime;
					}
					//1：車両用の最低限の青時間を待ってから変化
					else if (cycleMode == 1) {
						//最低限の青時間を確保
						if ((cTime3 + changeTime) < minimumGreenTime1) {
							updateGreenTime1 = (minimumGreenTime1 - cTime3);
						} else {
							updateGreenTime1 = changeTime;
						}
						cTime3 = 0f;
						//歩灯赤時間(2) ※ボタンを押してからの赤時間　の更新
						redTime2 = updateGreenTime1 + yellowTime + allRedTime;
						//1サイクルの時間の更新
						afterPushTime = redTime2 + redTime1 - allRedTime;
					} 
					//2：一定の信号サイクルに合わせて変化
					else if (cycleMode == 2){	
						//押下時の赤になる時間より早ければ、押下時サイクルに変更。遅ければ、次回のサイクルで青にする。
						if (cTime4 < greenTime1) {
							updateGreenTime1 = (greenTime1 - cTime4);
						} else {
							updateGreenTime1 = (allTime - cTime4) + greenTime1;
						}

						//歩灯赤時間(2) ※ボタンを押してからの赤時間　の更新
						redTime2 = updateGreenTime1 + yellowTime + allRedTime;
						//1サイクルの時間の更新
						afterPushTime = redTime2 + redTime1 - allRedTime;

					}
				}
			}
			canPushText.text = "ボタンを押す(スペースキー)";
		} else {
			canPushText.text = "";
		}

	}
}
