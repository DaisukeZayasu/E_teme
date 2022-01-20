using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*
 *  踏切のサイクルスクリプト
 */
 

public class RailroadCrossingCycle : TrafficCycle {

    private float cTime2 = 0f, cTime3 = 0f;
    
    //警報灯の点滅間隔
    public float blinkTime = 0.5f;
    //遮断機の数
    public int barNum = 2;

    //遮断機が下りるまでの時間
    private float crossStartTime = 6f;
    private float crossStartTime2 = 2f;
    //遮断機が下りる速度
    private float rotateSpeed = 0.4f;

    

    //踏切の動作状態
    [System.NonSerialized] public bool rcOn = false;
    //踏切が操作できるかどうか
    [System.NonSerialized] public bool rcEnable = true;

    //踏切警報灯
    private GameObject[] lightR1, lightR2;

    //警報棒
    private GameObject[] railroadBar1, railroadBar2;

    //方向指示器
    private GameObject[] rcArrowBox;
    private GameObject[] rcLeftArrow, rcRightArrow;
    bool rcLeftOn = false, rcRightOn = false;

    //スクリプト
    private RCLightSetting[] rcls1, rcls2, rclsA;

    //動作自動化
    public bool autoOperation = false;

    //列車のセンサー
    private RCTrainSensor[] rcts = new RCTrainSensor[6];


    // Use this for initialization
    void Start() {
        rcOn = false;

        //警報灯の設定
        GameObject[] tempDirection;
        tempDirection = GameObject.FindGameObjectsWithTag("RailroadCrossing1");
        direction1 = getChildObjects(tempDirection);
        tempDirection = GameObject.FindGameObjectsWithTag("RailroadCrossing2");
        direction2 = getChildObjects(tempDirection);
        tempDirection = GameObject.FindGameObjectsWithTag("RCArrow");
        rcArrowBox = getChildObjects(tempDirection);

        //各灯器スクリプトの設定
        rcls1 = new RCLightSetting[direction1.Length];
        for (int i = 0; i < rcls1.Length; i++) {
            rcls1[i] = direction1[i].GetComponent<RCLightSetting>();
        }
        rcls2 = new RCLightSetting[direction2.Length];
        for (int i = 0; i < rcls2.Length; i++) {
            rcls2[i] = direction2[i].GetComponent<RCLightSetting>();
        }
        rclsA= new RCLightSetting[rcArrowBox.Length];
        for (int i = 0; i < rcArrowBox.Length; i++) {
            rclsA[i] = rcArrowBox[i].GetComponent<RCLightSetting>();
        }

        //各灯火の設定
        lightR1 = new GameObject[direction1.Length];
        lightR2 = new GameObject[direction2.Length];
        for (int i = 0; i < lightR1.GetLength(0); i++) {
            if (rcls1[i].lightType == 1) {
                lightR1[i] = direction1[i].transform.Find("LightRed").gameObject;
            } else if (rcls1[i].lightType == 0 || rcls1[i].lightType == 2) {
                lightR1[i] = direction1[i].transform.Find("LightGroupR/PointLightR").gameObject;
            } else if (rcls1[i].lightType == 3) {
                lightR1[i] = direction1[i].transform.Find("LightGroupRLI/PointLightR").gameObject;
            }
        }
        for (int i = 0; i < lightR2.GetLength(0); i++) {
            if (rcls2[i].lightType == 1) {
                lightR2[i] = direction2[i].transform.Find("LightRed").gameObject;
            } else if (rcls2[i].lightType == 0 || rcls2[i].lightType == 2) {
                lightR2[i] = direction2[i].transform.Find("LightGroupR/PointLightR").gameObject;
            } else if (rcls2[i].lightType == 3) {
                lightR2[i] = direction2[i].transform.Find("LightGroupRLI/PointLightR").gameObject;
            }
        }
        //方向指示器
        rcLeftArrow = new GameObject[rcArrowBox.Length];
        rcRightArrow = new GameObject[rcArrowBox.Length];
        for (int i = 0; i < rcArrowBox.GetLength(0); i++) {
            rcLeftArrow[i] = rcArrowBox[i].transform.Find("RCLeftArrow").gameObject;
            rcRightArrow[i] = rcArrowBox[i].transform.Find("RCRightArrow").gameObject;
        }


        //警報棒の設定        
        if (barNum != 2 && barNum != 4) {
            barNum = 0;
        }
        if (barNum > 0) {
            railroadBar1 = new GameObject[2];
            railroadBar1[0] = this.transform.Find("CrossingGate (1)/CrossingBar").gameObject;
            railroadBar1[1] = this.transform.Find("CrossingGate (2)/CrossingBar").gameObject;
        }
        if (barNum == 4) {
            railroadBar2 = new GameObject[2];
            railroadBar2[0] = this.transform.Find("CrossingGate (3)/CrossingBar").gameObject;
            railroadBar2[1] = this.transform.Find("CrossingGate (4)/CrossingBar").gameObject;
        }


        //列車センサーの設定
        if (autoOperation) {
            for(int i=0; i<rcts.Length; i++) {
                rcts[i] = this.transform.Find("TrainSensor (" + (i + 1) + ")").GetComponent<RCTrainSensor>();
            }
        }
    }


    void FixedUpdate() {
        
        if (rcOn) {
            //警報灯
            for (int i = 0; i < lightR1.GetLength(0); i++) {
                if (cTime >= blinkTime && cTime < blinkTime * 2) {
                    //赤点灯
                    rcls1[i].SettingRCLight(lightR1[i], true);
                } else {
                    //赤消灯
                    rcls1[i].SettingRCLight(lightR1[i], false);
                }
            }
            for (int i = 0; i < lightR2.GetLength(0); i++) {
                if (cTime >= blinkTime && cTime < blinkTime * 2) {
                    //赤消灯
                    rcls2[i].SettingRCLight(lightR2[i], false);
                } else {
                    //赤点灯
                    rcls2[i].SettingRCLight(lightR2[i], true);
                }
            }

            //サイクル時間のリセットと更新
            if (cTime >= blinkTime * 2) {
                cTime = 0f;
            } else {
                cTime += Time.deltaTime;
            }
            cTime2 += Time.deltaTime;

            //遮断機が下りる
            if (barNum > 0) {
                if (cTime2 > crossStartTime) {
                    for (int i = 0; i < railroadBar1.Length; i++) {
                        if (railroadBar1[i].transform.localEulerAngles.z > 0 &&
                            railroadBar1[i].transform.localEulerAngles.z <= 91) {
                            railroadBar1[i].transform.Rotate(0, 0, -rotateSpeed);
                        } else {
                            railroadBar1[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                            if (barNum == 2 && !rcEnable) {
                                this.GetComponent<AudioSource>().volume = 0.6f;
                                rcEnable = true;
                            }
                        }
                    }
                }
            }
            //もう一つの遮断機が下りる
            if (barNum == 4) {
                if (railroadBar1[0].transform.localEulerAngles.z == 0) {
                    cTime3 += Time.deltaTime;
                    if (cTime3 >= crossStartTime2) {
                        for (int i = 0; i < railroadBar2.Length; i++) {
                            if (railroadBar2[i].transform.localEulerAngles.z > 0 &&
                                railroadBar2[i].transform.localEulerAngles.z <= 91) {
                                railroadBar2[i].transform.Rotate(0, 0, -rotateSpeed);
                            } else {
                                railroadBar2[i].transform.localRotation = Quaternion.Euler(0, 180, 0);
                                if (!rcEnable) {
                                    this.GetComponent<AudioSource>().volume = 0.6f;
                                    rcEnable = true;
                                }
                            }
                        }
                    }
                }
            }

        } else {
            //警報灯
            for (int i = 0; i < lightR1.GetLength(0); i++) {
                //赤消灯
                rcls1[i].SettingRCLight(lightR1[i], false);        
            }
            for (int i = 0; i < lightR2.GetLength(0); i++) {
                //赤消灯
                rcls2[i].SettingRCLight(lightR2[i], false);
            }

            //遮断機が上がる
            if (barNum > 0) {
                for (int i = 0; i < railroadBar1.Length; i++) {
                    if (railroadBar1[i].transform.localEulerAngles.z < 90) {
                        railroadBar1[i].transform.Rotate(0, 0, rotateSpeed);
                    } else {
                        railroadBar1[i].transform.localRotation = Quaternion.Euler(0, 0, 90f);
                        if (!rcEnable) {
                            rcEnable = true;
                        }
                    }
                }
            }
            if (barNum == 4) {
                for (int i = 0; i < railroadBar2.Length; i++) {
                    if (railroadBar2[i].transform.localEulerAngles.z < 90) {
                        railroadBar2[i].transform.Rotate(0, 0, rotateSpeed);
                    } else {
                        railroadBar2[i].transform.localRotation = Quaternion.Euler(0, 180, 90f);
                    }
                }
            }
        }

        //方向指示器
        for(int i=0; i<rcArrowBox.Length; i++) {
            if (rcLeftOn) {
                rclsA[i].SettingRCLight(rcLeftArrow[i], true);
            } else {
                rclsA[i].SettingRCLight(rcLeftArrow[i], false);
            }
            if (rcRightOn) {
                rclsA[i].SettingRCLight(rcRightArrow[i], true);
            } else {
                rclsA[i].SettingRCLight(rcRightArrow[i], false);
            }
        }

    }


    // Update is called once per frame
    void Update () {
        //自動動作
        if (autoOperation) {

            //遮断機を下ろす
            if (!rcOn) {
                if (rcts[0].sensed || rcts[1].sensed) {                    
                    rcOn = true;
                    cTime = 0f;
                    cTime2 = 0f;
                    cTime3 = 0f;
                    rcEnable = false;
                    this.GetComponent<AudioSource>().Play();
                    this.GetComponent<AudioSource>().volume = 1.0f;
                    rcts[2].sensed = false;
                    rcts[3].sensed = false;
                    rcts[4].sensed = false;
                    rcts[5].sensed = false;
                }
            }

            if (rcOn) {
                //方向指示器
                if (rcts[0].sensed) {
                    rcLeftOn = true;
                }
                if (rcts[1].sensed) {
                    rcRightOn = true;
                }        
                if (rcts[2].sensed) {
                    rcLeftOn = false;
                }
                if (rcts[3].sensed) {
                    rcRightOn = false;
                }

                //踏切動作終了
                if (rcts[4].sensed) {
                    rcts[0].sensed = false;
                    rcts[2].sensed = false;
                    if (!rcts[1].sensed) {
                        rcOn = false;
                        rcEnable = false;
                        this.GetComponent<AudioSource>().Stop();
                        rcts[4].sensed = false;
                    }
                }
                if (rcts[5].sensed) {
                    rcts[1].sensed = false;
                    rcts[3].sensed = false;
                    if (!rcts[0].sensed) {
                        rcOn = false;
                        rcEnable = false;
                        this.GetComponent<AudioSource>().Stop();
                        rcts[5].sensed = false;
                    }
                }
            }


        } //手動動作
        else {
            //踏切の操作
            if (rcEnable) {
                if (!rcOn) {
                    if (Input.GetKey(KeyCode.Alpha1)) { //動作開始
                        rcOn = true;
                        cTime = 0f;
                        cTime2 = 0f;
                        cTime3 = 0f;
                        rcEnable = false;
                        this.GetComponent<AudioSource>().Play();
                        this.GetComponent<AudioSource>().volume = 1.0f;
                    }
                } else {
                    if (Input.GetKey(KeyCode.Alpha0)) { //動作終了
                        rcOn = false;
                        rcEnable = false;
                        this.GetComponent<AudioSource>().Stop();
                    }
                }
            }
            //方向指示器
            if (rcOn) {

                if (Input.GetKey(KeyCode.Alpha2)) {     //左オン
                    rcLeftOn = true;
                }
                if (Input.GetKey(KeyCode.Alpha3)) {     //左オフ
                    rcLeftOn = false;
                }
                if (Input.GetKey(KeyCode.Alpha4)) {     //右オン
                    rcRightOn = true;
                }
                if (Input.GetKey(KeyCode.Alpha5)) {     //右オフ
                    rcRightOn = false;
                }
            } else {
                if (rcLeftOn) {
                    rcLeftOn = false;
                }
                if (rcRightOn) {
                    rcRightOn = false;
                }
            }
        }

	}
}
