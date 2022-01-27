using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	矢印灯器用スクリプト
 */

public class TrafficLightArrowSetting : MonoBehaviour {

	//矢印灯器用のマテリアル(LED)
	//0矢印点灯, 1:矢印消灯
	public Material[] aLightMaterial = new Material[2];

	//0:電球, 1:LED, 2:LED電球
	public int lightType = 1;

	//電球式変化のスピード
	private int changeSpeed = 10;
    
    [System.NonSerialized] public bool lightOn = false;


    //矢印灯火の切り替え
    //SettingLightA(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
    public void SettingLightA(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType == 0) {	
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_A_INTENSITY / changeSpeed);
				} else {
					pointLight.intensity = CONSTS.MAX_A_INTENSITY;
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_A_INTENSITY / changeSpeed);
				}
			}
		} //LED式
		else if(lightType == 1){
			if (lightStatus) {	//点灯
				lightObject.GetComponent<Renderer> ().material = aLightMaterial [0];
			} else {			//消灯
				lightObject.GetComponent<Renderer> ().material = aLightMaterial [1];
			}
		} //LED電球
		else if (lightType == 2) {
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				pointLight.intensity = CONSTS.MAX_A_INTENSITY;
			} else {			//消灯
				pointLight.intensity = 0;
			}
		}

        if (lightStatus) {
            lightOn = true;
        } else {
            lightOn = false;
        }
    }
}
