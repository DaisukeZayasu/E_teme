using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	車灯用スクリプト
 */

public class TrafficLightSetting : MonoBehaviour {

	//車灯用のマテリアル(LED)
	//0:青点灯, 1:青消灯, 2:黄点灯, 3:黄消灯, 4:赤点灯, 5:赤消灯
	public Material[] lightMaterial = new Material[6];

	//灯火の状態
	//0:青, 1:黄, 2:赤
	[System.NonSerialized]public int lightOn = 0;

    //光源のタイプ
    //0:電球, 1:LED, 2:LED電球, 3:LED集約式
    public int[] lightType = new int[3];

	//電球式変化のスピード
	private int changeSpeed = 10;


    //車灯の灯火の準備
    public GameObject SetupLightG(GameObject direction, int pos) {
        GameObject lightObject;
        if (lightType[pos] == 1) {
            lightObject = direction.transform.Find("TLightGreen").gameObject;
        } else if (lightType[pos] == 0 || lightType[pos] == 2) {
            lightObject = direction.transform.Find("TLightGroupG/PointLightG").gameObject;
        } else if (lightType[pos] == 3) {
            lightObject = direction.transform.Find("TLightGroupGLI/PointLightG").gameObject;
        } else {
            lightObject = null;
        }
        return lightObject;
    }

    public GameObject SetupLightY(GameObject direction, int pos) {
        GameObject lightObject;
        if (lightType[pos] == 1) {
            lightObject = direction.transform.Find("TLightYellow").gameObject;
        } else if (lightType[pos] == 0 || lightType[pos] == 2) {
            lightObject = direction.transform.Find("TLightGroupY/PointLightY").gameObject;
        } else if (lightType[pos] == 3) {
            lightObject = direction.transform.Find("TLightGroupYLI/PointLightY").gameObject;
        } else {
            lightObject = null;
        }
        return lightObject;
    }

    public GameObject SetupLightR(GameObject direction, int pos) {
        GameObject lightObject;
        if (lightType[pos] == 1) {
            lightObject = direction.transform.Find("TLightRed").gameObject;
        } else if (lightType[pos] == 0 || lightType[pos] == 2) {
            lightObject = direction.transform.Find("TLightGroupR/PointLightR").gameObject;
        } else if (lightType[pos] == 3) {
            lightObject = direction.transform.Find("TLightGroupRLI/PointLightR").gameObject;
        } else {
            lightObject = null;
        }
        return lightObject;
    }


    //車灯の灯火の切り替え（青）
    //SettingLight(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
    public void SettingLightG(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType[0] == 0) {
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_INTENSITY / changeSpeed);
				} else {
					pointLight.intensity = CONSTS.MAX_INTENSITY;
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_INTENSITY / changeSpeed);
				}
			}
		} //LED式
		else if (lightType[0] == 1) {
			if (lightStatus) {	//点灯
				lightObject.GetComponent<Renderer> ().material = lightMaterial [0];
			} else {			//消灯
				lightObject.GetComponent<Renderer> ().material = lightMaterial [1];
			}
		} //LED電球 or LED集約式
		else if (lightType[0] == 2 || lightType[0] == 3) {
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				pointLight.intensity = CONSTS.MAX_INTENSITY;
			} else {			//消灯
				pointLight.intensity = 0;
			}
		}

		if (lightStatus) {
			lightOn = 0;
		}
	}

	//車灯の灯火の切り替え（黄）
	//SettingLight(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
	public void SettingLightY(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType[1] == 0) {
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_INTENSITY / changeSpeed);
				} else {
					pointLight.intensity = CONSTS.MAX_INTENSITY;
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_INTENSITY / changeSpeed);
				}
			}
		} //LED式
		else if (lightType[1] == 1) {
			if (lightStatus) {	//点灯
				lightObject.GetComponent<Renderer> ().material = lightMaterial [2];
			} else {			//消灯
				lightObject.GetComponent<Renderer> ().material = lightMaterial [3];
			}
		} ///LED電球 or LED集約式
		else if (lightType[1] == 2 || lightType[1] == 3) {
            Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				pointLight.intensity = CONSTS.MAX_INTENSITY;
			} else {			//消灯
				pointLight.intensity = 0;
			}
		}

		if (lightStatus) {
			lightOn = 1;
		}
	}

	//車灯の灯火の切り替え（赤）
	//SettingLight(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
	public void SettingLightR(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType[2] == 0) {
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_INTENSITY / changeSpeed);
				} else {
					pointLight.intensity = CONSTS.MAX_INTENSITY;
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_INTENSITY / changeSpeed);
				}
			}
		} //LED式
		else if (lightType[2] == 1) {
			if (lightStatus) {	//点灯
				lightObject.GetComponent<Renderer> ().material = lightMaterial [4];
			} else {			//消灯
				lightObject.GetComponent<Renderer> ().material = lightMaterial [5];
			}
        } //LED電球 or LED集約式
        else if (lightType[2] == 2 || lightType[2] == 3) {
            Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				pointLight.intensity = CONSTS.MAX_INTENSITY;
			} else {			//消灯
				pointLight.intensity = 0;
			}
		}

		if (lightStatus) {
			lightOn = 2;
		}
	}

}
