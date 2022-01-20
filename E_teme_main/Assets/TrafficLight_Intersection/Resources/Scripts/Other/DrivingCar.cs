using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 車の運転用スクリプト
 */


public class DrivingCar : MonoBehaviour {

	public bool handControl = false;	//運転するかどうか
    public float setSpeed = 0;          //初速度

	[System.NonSerialized] public float speedNow = 0f;	//現在速度 (m/s)
	[System.NonSerialized] public float angleY = 0f;    //車体の方向

    private float acceralation = 0.1f;  //加速度(1フレームあたり)
    private float turning = 0.5f;		//ハンドルを切る度合い

	[System.NonSerialized] public int gear = 1;         //ギア（1:ドライブ, -1:バック）

    private Rigidbody rb;
    private Vector3 speedV;

    //車のライトの設定
    private CarSetting css;
    private GameObject[] frontLight;
    private GameObject[] turnSignal;
    private GameObject[] tailLight;

    //ライトの状態
    private int turnSignalSt = 0;
    private float cTime = 0;
    private float blinkTime = 0.35f;
    private bool frontLightSt = false;


    // Use this for initialization
    void Start () {
		rb = this.GetComponent<Rigidbody> ();	//rigidbodyの取得
		angleY = transform.localEulerAngles.y;	//方向の取得

        //ライトの設定
        css = this.GetComponent<CarSetting>();
        frontLight = new GameObject[2];
        turnSignal = new GameObject[4];
        tailLight = new GameObject[2];
        for (int i = 0; i < frontLight.Length; i++) {
            frontLight[i] = this.transform.Find("FrontLight (" + (i + 1) + ")").gameObject;
        }
        for (int i = 0; i < tailLight.Length; i++) {
            tailLight[i] = this.transform.Find("TailLight (" + (i + 1) + ")").gameObject;
        }
        for (int i = 0; i < turnSignal.Length; i++) {
            turnSignal[i] = this.transform.Find("TurnSignal (" + (i + 1) + ")").gameObject;
        }
        speedNow = setSpeed / 3600 * 1000;
    }

	// Update is called once per frame
	void Update () {
		if (handControl) {

			//ハンドル
			if (Input.GetKey ("right")) {
				this.transform.RotateAround (this.transform.position, this.transform.up, turning);
			} else if (Input.GetKey ("left")) {
				this.transform.RotateAround (this.transform.position, this.transform.up, -turning);
			} else {
				rb.angularVelocity = Vector3.zero;
			}
			
			//ギアの切り替え
			if (speedNow == 0) {
				if (Input.GetKeyDown (KeyCode.Tab)) {
					if (gear == 1) {
						gear = -1;
					} else {
						gear = 1;
					}
				}
			}

            //フロントライト
            if (Input.GetKey(KeyCode.L)) {
                frontLightSt = true;
            }
            if (Input.GetKey(KeyCode.K)) {
                frontLightSt = false;
            }
            SettingFrontLight(frontLightSt);


            //アクセル・ブレーキ
            if (Input.GetKey ("up")) {
				speedNow += gear * acceralation;
			}
			if (Input.GetKey ("down")) {
				if (gear == 1) {
					if (speedNow > 0) {
						speedNow += -1 * gear * acceralation * 1.5f;
					} else {
						speedNow = 0;
					}
				} else {
					if (speedNow < 0) {
						speedNow += -1 * gear * acceralation * 1.5f;
					} else {
						speedNow = 0;
					}
				}
                //ブレーキランプ
                SettingStopLight(true);

            } else {
                //テールランプ
                SettingTailLight(frontLightSt);
            }

            //ウインカー
            if (Input.GetKey(KeyCode.C)) {
                turnSignalSt = 1;   //右折
            }
            if (Input.GetKey(KeyCode.X)) {
                turnSignalSt = 0;
            }
            if (Input.GetKey(KeyCode.Z)) {
                turnSignalSt = -1;  //左折
            }

            SettingTurnSignal(turnSignalSt, cTime);
            if (cTime >= blinkTime * 2) {
                cTime = 0f;
            } else {
                cTime += Time.deltaTime;
            }

        }

        //実際の移動
        if (speedNow == 0) {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        } else {
            //速度ベクトル
            speedV = speedNow * this.transform.forward;
            //位置の更新（移動）
            rb.velocity = new Vector3(speedV.x, 0, speedV.z);
        }
    }

    //フロントライト
    public void SettingFrontLight(bool lightStatus) {
        for (int i = 0; i < frontLight.Length; i++) {
            if (lightStatus) {
                frontLight[i].GetComponent<Renderer>().material = css.frontLightOn;
            } else {
                frontLight[i].GetComponent<Renderer>().material = css.frontLightOff;
            }
        }
    }

    //テールランプ
    public void SettingTailLight(bool lightStatus) {
        for(int i = 0; i < tailLight.Length; i++) {
            if (lightStatus) {
                tailLight[i].GetComponent<Renderer>().material = css.tailLightOn;
            } else {
                tailLight[i].GetComponent<Renderer>().material = css.tailLightOff;
            }
        }
    }

    //ブレーキランプ
    public void SettingStopLight(bool lightStatus) {
        for (int i = 0; i < tailLight.Length; i++) {
            if (lightStatus) {
                tailLight[i].GetComponent<Renderer>().material = css.stopLightOn;
            } else {
                tailLight[i].GetComponent<Renderer>().material = css.tailLightOff;
            }
        }
    }

    //ウインカー
    public void SettingTurnSignal(int lightStatus, float cTime) {
        if (lightStatus == 0) {
            for (int i = 0; i < turnSignal.Length; i++) {
                turnSignal[i].GetComponent<Renderer>().material = css.turnSignalOff;
            }
            /*
            if (turnSignalAS.isPlaying) {
                turnSignalAS.Stop();
            }*/
        } else {
            if (cTime >= blinkTime && cTime < blinkTime * 2) {
                if (lightStatus == 1) {
                    turnSignal[0].GetComponent<Renderer>().material = css.turnSignalOn;
                    turnSignal[1].GetComponent<Renderer>().material = css.turnSignalOff;
                    turnSignal[2].GetComponent<Renderer>().material = css.turnSignalOn;
                    turnSignal[3].GetComponent<Renderer>().material = css.turnSignalOff;
                } else if (lightStatus == -1) {
                    turnSignal[0].GetComponent<Renderer>().material = css.turnSignalOff;
                    turnSignal[1].GetComponent<Renderer>().material = css.turnSignalOn;
                    turnSignal[2].GetComponent<Renderer>().material = css.turnSignalOff;
                    turnSignal[3].GetComponent<Renderer>().material = css.turnSignalOn;
                }
            } else {
                for (int i = 0; i < turnSignal.Length; i++) {
                    turnSignal[i].GetComponent<Renderer>().material = css.turnSignalOff;
                }
            }
            //ウインカーの音
            /*if (!turnSignalAS.isPlaying) {
                turnSignalAS.Play();
            }*/
        }

    }


}
