using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	歩灯用スクリプト
 */

public class TrafficLightPSetting : MonoBehaviour {
	
	//歩灯用のマテリアル(LED)
	//0:赤点灯, 1:赤消灯, 2:青点灯, 3:青消灯
	public Material[] pLightMaterial = new Material[4];

    //0:電球, 1:LED, 2:LED電球
    public int[] lightType = new int[2];

    //電球式変化のスピード
    private int changeSpeed = 10;


    //歩灯の灯火のセッティング
    public GameObject SetupLightPR(GameObject direction) {
        GameObject lightObject;
        if (lightType[0] == 1) {
            lightObject = direction.transform.Find("TPLightGroupRL/TPLightRed").gameObject;
        } else if (lightType[0] == 0 || lightType[0] == 2) {
            lightObject = direction.transform.Find("TPLightGroupR/PointLightPR").gameObject;
        } else {
            lightObject = null;
        }
        return lightObject;
    }
    public GameObject SetupLightPG(GameObject direction) {
        GameObject lightObject;
        if (lightType[1] == 1) {
            lightObject = direction.transform.Find("TPLightGroupGL/TPLightGreen").gameObject;
        } else if (lightType[1] == 0 || lightType[1] == 2) {
            lightObject = direction.transform.Find("TPLightGroupG/PointLightPG").gameObject;
        } else {
            lightObject = null;
        }
        return lightObject;
    }

    

    //歩灯の灯火の切り替え(赤)
    //SettingLightPR(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
    public void SettingLightPR(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType[0] == 0){
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_P_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_P_INTENSITY / changeSpeed);
				} else {
					pointLight.intensity = CONSTS.MAX_P_INTENSITY;
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_P_INTENSITY / changeSpeed);
				}
			}
		} //LED式
		else if(lightType[0] == 1){
			if (lightStatus) {	//点灯
				lightObject.GetComponent<Renderer> ().material = pLightMaterial [0];
			} else {			//消灯
				lightObject.GetComponent<Renderer> ().material = pLightMaterial [1];
			}
		} //LED電球
		else if (lightType[0] == 2) {
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				pointLight.intensity = CONSTS.MAX_P_INTENSITY;
			} else {			//消灯
				pointLight.intensity = 0;
			}
		}
	}

	//歩灯の灯火の切り替え(青)
	//SettingLightPG(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
	public void SettingLightPG(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType[1] == 0){
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_P_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_P_INTENSITY / changeSpeed);
				} else {
					pointLight.intensity = CONSTS.MAX_P_INTENSITY;
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_P_INTENSITY / changeSpeed);
				}
			}
		} //LED式
		else if(lightType[1] == 1){
			if (lightStatus) {	//点灯
				lightObject.GetComponent<Renderer> ().material = pLightMaterial [2];
			} else {			//消灯
				lightObject.GetComponent<Renderer> ().material = pLightMaterial [3];
			}
		} //LED電球
		else if (lightType[1] == 2) {
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				pointLight.intensity = CONSTS.MAX_P_INTENSITY;
			} else {			//消灯
				pointLight.intensity = 0;
			}
		}
	}


	//経過時間ゲージタイプ 0:無し, 1:通常, 2:素子
	public int timeGaugeType = 0;

    //経過時間ゲージオブジェクト
    private GameObject[] timeGaugeR = new GameObject[2];
    private GameObject[] timeGaugeG = new GameObject[2];

    //経過時間ゲージ用マテリアル
    //0:赤、1:青
    private Material[,] timeGaugeMaterial = new Material[2, 9]; 


	void Start(){

        //経過時間ゲージオブジェクト
        if (timeGaugeType != 0) {
            timeGaugeR[0] = this.transform.Find("TPLightGroupGL/TPTimeGaugeRed (1)").gameObject;
            timeGaugeR[1] = this.transform.Find("TPLightGroupGL/TPTimeGaugeRed (2)").gameObject;
            timeGaugeG[0] = this.transform.Find("TPLightGroupRL/TPTimeGaugeGreen (1)").gameObject;
            timeGaugeG[1] = this.transform.Find("TPLightGroupRL/TPTimeGaugeGreen (2)").gameObject;
        }

        //経過時間ゲージマテリアル設定
        for (int j=0; j < timeGaugeMaterial.GetLength (1); j++) {
            if (timeGaugeType == 0) {
                timeGaugeMaterial[0, j] = Resources.Load("Materials/TimeGauge/TPTimeGaugeNone") as Material;
                timeGaugeMaterial[1, j] = Resources.Load("Materials/TimeGauge/TPTimeGaugeNone") as Material;
            } else if (timeGaugeType == 1) {
				timeGaugeMaterial [0, j] = Resources.Load ("Materials/TimeGauge/TPTimeGauge" + j + "Red") as Material;
				timeGaugeMaterial [1, j] = Resources.Load ("Materials/TimeGauge/TPTimeGauge" + j + "Green") as Material;
			} else if (timeGaugeType == 2) {
				timeGaugeMaterial [0, j] = Resources.Load ("Materials/TimeGauge/TPTimeGaugeE" + j + "Red") as Material;
				timeGaugeMaterial [1, j] = Resources.Load ("Materials/TimeGauge/TPTimeGaugeE" + j + "Green") as Material;
			} else if (timeGaugeType == 3) {
                timeGaugeMaterial[0, j] = Resources.Load("Materials/TimeGauge/TPTimeGaugeLC" + j + "Red") as Material;
                timeGaugeMaterial[1, j] = Resources.Load("Materials/TimeGauge/TPTimeGaugeLC" + j + "Green") as Material;
            }
        }
	}


	//経過時間ゲージの設定
	//SettingTimeGauge(切り替えるゲージのオブジェクト, 点灯色(0:red, 1:green), 目盛り)
	public void SettingTimeGauge(GameObject timeGauge, int color, int scale){
		timeGauge.GetComponent<Renderer> ().material = timeGaugeMaterial [color, scale];
	}
    


    //赤ゲージ設定用
    public void SettingTimeGaugeR(float cTime, float redStartTime, float redEndTime, float pRedTime) {

        float oneRedTime = pRedTime / 8f;

        for (int i = 0; i < timeGaugeR.Length; i++) {
            if (cTime >= redStartTime) {
                if (cTime < redStartTime + oneRedTime) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 8];
                } else if (cTime < redStartTime + oneRedTime * 2) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 7];
                } else if (cTime < redStartTime + oneRedTime * 3) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 6];
                } else if (cTime < redStartTime + oneRedTime * 4) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 5];
                } else if (cTime < redStartTime + oneRedTime * 5) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 4];
                } else if (cTime < redStartTime + oneRedTime * 6) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 3];
                } else if (cTime < redStartTime + oneRedTime * 7) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 2];
                } else if (cTime < redStartTime + oneRedTime * 8) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 1];
                } else {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 0];
                }
            } else {
                if (cTime < redEndTime - oneRedTime * 7) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 8];
                } else if (cTime < redEndTime - oneRedTime * 6) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 7];
                } else if (cTime < redEndTime - oneRedTime * 5) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 6];
                } else if (cTime < redEndTime - oneRedTime * 4) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 5];
                } else if (cTime < redEndTime - oneRedTime * 3) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 4];
                } else if (cTime < redEndTime - oneRedTime * 2) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 3];
                } else if (cTime < redEndTime - oneRedTime * 1) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 2];
                } else if (cTime < redEndTime) {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 1];
                } else {
                    timeGaugeR[i].GetComponent<Renderer>().material = timeGaugeMaterial[0, 0];
                }
            }
        }
    }


    //青ゲージ設定用
    public void SettingTimeGaugeG(float cTime, float greenStartTime, float pGreenTime)
    {
        float oneGreenTime = pGreenTime / 8f;

        for (int i = 0; i < timeGaugeG.Length; i++) {
            if (cTime >= greenStartTime) {
                if (cTime < greenStartTime + oneGreenTime) {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 8];
                } else if (cTime < greenStartTime + oneGreenTime * 2) {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 7];
                } else if (cTime < greenStartTime + oneGreenTime * 3) {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 6];
                } else if (cTime < greenStartTime + oneGreenTime * 4) {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 5];
                } else if (cTime < greenStartTime + oneGreenTime * 5) {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 4];
                } else if (cTime < greenStartTime + oneGreenTime * 6) {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 3];
                } else if (cTime < greenStartTime + oneGreenTime * 7) {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 2];
                } else if (cTime < greenStartTime + oneGreenTime * 8) {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 1];
                } else {
                    timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 0];
                }
            } else {
                timeGaugeG[i].GetComponent<Renderer>().material = timeGaugeMaterial[1, 0];
            }
        }
    }

}
