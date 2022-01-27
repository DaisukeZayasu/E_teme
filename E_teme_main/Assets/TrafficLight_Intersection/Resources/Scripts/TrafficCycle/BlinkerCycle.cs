using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	
 *	ブリンカーライト用のサイクルスクリプト
 *　常時交互黄点滅
 */

public class BlinkerCycle : MonoBehaviour {

	//点滅サイクルの種類（0:交互点滅, 1:両方消灯を挟んだ交互点滅）
	public int cycleMode = 0;
	//黄点滅の間隔（片方の黄点灯時間）
	public float blinkTime = 0.5f;
	//両方消灯の時間
	public float offTime = 0.25f;


	private int blinkCnt = 0;
    private float cTime = 0f;

    private GameObject[] lightBL;
    private TrafficLightSetting tlsBL;

	// Use this for initialization
	void Start () {
		lightBL = new GameObject[2];
		tlsBL = this.GetComponent<TrafficLightSetting> ();

		if (tlsBL.lightType[1] == 1) {
			lightBL [0] = this.transform.Find ("TLightYellow (1)").gameObject;
			lightBL [1] = this.transform.Find ("TLightYellow (2)").gameObject;
		} else if(tlsBL.lightType[1] == 0 || tlsBL.lightType[1] == 2){
			lightBL [0] = this.transform.Find ("TLightGroupY (1)/PointLightY").gameObject;
			lightBL [1] = this.transform.Find ("TLightGroupY (2)/PointLightY").gameObject;
		} else if (tlsBL.lightType[1] == 3) {
            lightBL [0] = this.transform.Find("TLightGroupYLI (1)/PointLightY").gameObject;
            lightBL [1] = this.transform.Find("TLightGroupYLI (2)/PointLightY").gameObject;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate (){
		//交互点滅モード
		if (cycleMode == 0) {
			//黄点滅ステップ 
			if (blinkCnt % 2 == 0) {
				tlsBL.SettingLightY (lightBL [0], true);
				tlsBL.SettingLightY (lightBL [1], false);
			} else {
				tlsBL.SettingLightY (lightBL [0], false);
				tlsBL.SettingLightY (lightBL [1], true);
			}

			//点滅回数の更新
			cTime += Time.deltaTime;
			if (cTime >= blinkTime * (blinkCnt + 1)) {
				blinkCnt++;
			} 
		} 

		//両方消灯を挟んだ交互点滅
		else if (cycleMode == 1) {
			//黄点滅ステップ 
			if (blinkCnt % 4 == 0) {
				tlsBL.SettingLightY (lightBL [0], true);
				tlsBL.SettingLightY (lightBL [1], false);
			} else if (blinkCnt % 4 == 1 || blinkCnt % 4 == 3) {
				tlsBL.SettingLightY (lightBL [0], false);
				tlsBL.SettingLightY (lightBL [1], false);
			} else if (blinkCnt % 4 == 2) {
				tlsBL.SettingLightY (lightBL [0], false);
				tlsBL.SettingLightY (lightBL [1], true);
			}

			//点滅回数の更新
			cTime += Time.deltaTime;
			if (cTime >= blinkTime * (blinkCnt + 1)) {
				blinkCnt++;
			} 
		}

	}

}
