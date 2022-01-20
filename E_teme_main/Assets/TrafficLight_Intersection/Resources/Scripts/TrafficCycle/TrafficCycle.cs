using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	信号サイクルの継承元スクリプト(信号サイクルのテンプレ)
 */

public class TrafficCycle : MonoBehaviour {

	//灯器用スクリプト
	protected TrafficLightSetting[] tls1, tls2;
	protected TrafficLightPSetting[] tlps1, tlps2;

	//各方向のゲームオブジェクト
	protected GameObject[] direction1, direction2, directionP1, directionP2;
	//灯火のゲームオブジェクト
	protected GameObject[,] light1, light2, lightP1, lightP2;

	//各サイクル時間・カウンタ
	protected float cTime = 0f, cPTime = 0f, allTime = 0f;
	protected float redTime1 = 0f, redTime2 = 0f, pRedTime1 = 0f, pRedTime2 = 0f;
	//歩行者用の点滅カウンタ・ステップ
	protected int blinkCnt = 0, cStep1 = 0, cStep2 = 2;

	//経過時間関係
	//protected GameObject[,] timeGauge1, timeGauge2;
	//protected float oneRedTime1, oneRedTime2, oneGreenTime1, oneGreenTime2;
	protected float restRedTime1, restRedTime2;


	//同じ交差点内のオブジェクトのみを選択する
	protected GameObject[] getChildObjects(GameObject[] tempDirection){
		int childCnt = 0;
		for (int i = 0; i < tempDirection.Length; i++) {
			if(tempDirection[i].transform.root.gameObject.name == this.name){
				childCnt++;
			}
		}
		GameObject[] newDirection = new GameObject[childCnt];
		childCnt = 0;
		for (int i = 0; i < tempDirection.Length; i++) {
			if(tempDirection[i].transform.root.gameObject.name == this.name){
				newDirection [childCnt] = tempDirection [i];
				childCnt++;
			}
		}
		return newDirection;
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
