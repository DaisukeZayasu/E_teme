using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*
 * 踏切信号のサイクル
 */

public class TrafficCycleRC1 : TrafficCycle {

    //連動のベースとなる踏切
    public GameObject railroadCrossing;
    private RailroadCrossingCycle rcc;

    //踏切が開いてから青になるまでの時間
    public float changeTime = 4f;

    private int cStep = 0;
    private float cTime2;


	// Use this for initialization
	void Start () {
        //ベースとなる踏切
        rcc = railroadCrossing.GetComponent<RailroadCrossingCycle>();

        GameObject[] tempDirection;
        tempDirection = GameObject.FindGameObjectsWithTag("Direction1");
        direction1 = getChildObjects(tempDirection);

        tls1 = new TrafficLightSetting[direction1.Length];
        for (int i = 0; i < tls1.Length; i++) {
            tls1[i] = direction1[i].GetComponent<TrafficLightSetting>();
        }

        light1 = new GameObject[direction1.Length, 3];
        //車灯（0:青灯火, 1:黄灯火, 2:赤灯火）のセッティング
        for (int i = 0; i < light1.GetLength(0); i++) {
            light1[i, 0] = tls1[i].SetupLightG(direction1[i], 0);
            light1[i, 1] = tls1[i].SetupLightY(direction1[i], 1);
            light1[i, 2] = tls1[i].SetupLightR(direction1[i], 2);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate() {

        //踏切との連動
        if(cStep == 0) {        //青
            if (rcc.rcOn) {
                cTime = 0f;
                cStep = 1;
            }
        } else if(cStep == 1) { //黄
            if (cTime < CONSTS.Y_TIME) {
                cTime += Time.deltaTime;
            } else {
                cStep = 2;
                cTime = 0f;
            }

        } else if(cStep == 2) { //赤
            if (!rcc.rcOn) {
                if (cTime < changeTime) {
                    cTime += Time.deltaTime;
                } else {
                    cStep = 0;
                    cTime = 0f;
                }
            }
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
         
            } else if(cStep == 2) {
                //赤信号に
                tls1[i].SettingLightG(light1[i, 0], false);
                tls1[i].SettingLightY(light1[i, 1], false);
                tls1[i].SettingLightR(light1[i, 2], true);
                
            }
        }
   
    }
}
