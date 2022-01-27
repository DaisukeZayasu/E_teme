using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*
 * 踏切と連動した信号サイクル
 */

public class TrafficCycleRC2 : TrafficCycle {

    //連動のベースとなる踏切
    public GameObject railroadCrossing;
    private RailroadCrossingCycle rcc;

    //踏切が開いてから青になるまでの時間
    public float changeTime = 4f;

    private int cStep = 0, cStep_2 = 0;
    private float cTime2;

    //灯器用スクリプト
    private TrafficLightSetting[] tls1E;

    //各方向のゲームオブジェクト
    private GameObject[] direction1E;
    //灯火のゲームオブジェクト
    private GameObject[,] light1E;

    //連動のベースとなる前方信号機
    public GameObject baseSignal;
    private TrafficLightSetting tlsBase;


    // Use this for initialization
    void Start() {
        //ベースとなる踏切
        rcc = railroadCrossing.GetComponent<RailroadCrossingCycle>();

        GameObject[] tempDirection;
        tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
        direction1 = getChildObjects(tempDirection);
        tempDirection = GameObject.FindGameObjectsWithTag("Direction1E");
        direction1E = getChildObjects(tempDirection);

        tls1 = new TrafficLightSetting[direction1.Length];
        tls1E = new TrafficLightSetting[direction1E.Length];
        for (int i = 0; i < tls1.Length; i++) {
            tls1[i] = direction1[i].GetComponent<TrafficLightSetting>();
        }
        for (int i = 0; i < tls1E.Length; i++) {
            tls1E[i] = direction1E[i].GetComponent<TrafficLightSetting>();
        }

        light1 = new GameObject[direction1.Length, 3];
        light1E = new GameObject[direction1E.Length, 3];

        //車灯（0:青灯火, 1:黄灯火, 2:赤灯火）のセッティング
        for (int i = 0; i < light1.GetLength(0); i++) {
            light1[i, 0] = tls1[i].SetupLightG(direction1[i], 0);
            light1[i, 1] = tls1[i].SetupLightY(direction1[i], 1);
            light1[i, 2] = tls1[i].SetupLightR(direction1[i], 2);
        }
        for (int i = 0; i < light1E.GetLength(0); i++) {
            light1E[i, 0] = tls1E[i].SetupLightG(direction1E[i], 0);
            light1E[i, 1] = tls1E[i].SetupLightY(direction1E[i], 1);
            light1E[i, 2] = tls1E[i].SetupLightR(direction1E[i], 2);
        }
        
        //ベースとなる前方信号機スクリプトの設定
        tlsBase = baseSignal.GetComponent<TrafficLightSetting>();
    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {

        //踏切との連動
        if (cStep == 0) {           //青
            if (rcc.rcOn) {
                cTime = 0f;
                cStep = 1;
            }
        } else if (cStep == 1) {    //黄
            if (cTime < CONSTS.Y_TIME) {
                cTime += Time.deltaTime;
            } else {
                cStep = 2;
                cTime = 0f;
            }

        } else if (cStep == 2) {    //赤
            if (!rcc.rcOn) {
                if (cTime < changeTime) {
                    cTime += Time.deltaTime;
                } else {
                    cStep = 0;
                    cTime = 0f;
                }
            }
        }

        //前方信号との連動
        if (tlsBase.lightOn == 0) {         //青
            cStep_2 = 0;
        } else if (tlsBase.lightOn == 1) {  //黄
            cStep_2 = 1;
        } else if (tlsBase.lightOn == 2) {  //赤
            cStep_2 = 2;
        }

            //方向１車灯の信号サイクル
        for (int i = 0; i < light1.GetLength(0); i++) {
            if (cStep == 0) {
                //青信号に
                tls1[i].SettingLightG(light1[i, 0], true);
                tls1[i].SettingLightY(light1[i, 1], false);
                tls1[i].SettingLightR(light1[i, 2], false);
            } else if (cStep == 1) {
                //黄信号に
                tls1[i].SettingLightG(light1[i, 0], false);
                tls1[i].SettingLightY(light1[i, 1], true);
                tls1[i].SettingLightR(light1[i, 2], false);

            } else if (cStep == 2) {
                //赤信号に
                tls1[i].SettingLightG(light1[i, 0], false);
                tls1[i].SettingLightY(light1[i, 1], false);
                tls1[i].SettingLightR(light1[i, 2], true);

            }
        }
        //方向１-2車灯の信号サイクル
        for (int i = 0; i < light1E.GetLength(0); i++) {
            if (cStep == 0 && cStep_2 == 0) {
                //青信号に
                tls1E[i].SettingLightG(light1E[i, 0], true);
                tls1E[i].SettingLightY(light1E[i, 1], false);
                tls1E[i].SettingLightR(light1E[i, 2], false);
            } else if ((cStep == 1 && cStep_2 == 0)||(cStep == 0 && cStep_2 == 1)||(cStep == 1 && cStep_2 == 1)) {
                //黄信号に
                tls1E[i].SettingLightG(light1E[i, 0], false);
                tls1E[i].SettingLightY(light1E[i, 1], true);
                tls1E[i].SettingLightR(light1E[i, 2], false);
            } else  {
                //赤信号に
                tls1E[i].SettingLightG(light1E[i, 0], false);
                tls1E[i].SettingLightY(light1E[i, 1], false);
                tls1E[i].SettingLightR(light1E[i, 2], true);
            }
        }

    }
}
