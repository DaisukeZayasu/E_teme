using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

/*	
 *	路面電車灯器用スクリプト
 */

public class TrafficLightStreetcarSetting : MonoBehaviour {

	//路面電車灯器用のマテリアル(LED)
	//0黄矢印点灯, 1:黄矢印消灯, 2:バツ印点灯, 3:バツ印消灯
	public Material[] scLightMaterial = new Material[4];

	//0:電球, 1:LED, 2:LED電球
	public int lightType = 1;

	//電球式変化のスピード
	private int changeSpeed = 10;

	//黄矢印灯火の切り替え
	//SettingLightYA(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
	public void SettingLightYA(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType == 0) {	
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_A_INTENSITY / changeSpeed);
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_A_INTENSITY / changeSpeed);
				}
			}
		} //LED式
		else if(lightType == 1){
			if (lightStatus) {	//点灯
				lightObject.GetComponent<Renderer> ().material = scLightMaterial [0];
			} else {			//消灯
				lightObject.GetComponent<Renderer> ().material = scLightMaterial [1];
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
	}

	//バツ印灯火の切り替え
	//SettingLightX(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
	public void SettingLightX(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType == 0) {	
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_A_INTENSITY / changeSpeed);
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_A_INTENSITY / changeSpeed);
				}
			}
		} //LED式
		else if(lightType == 1){
			if (lightStatus) {	//点灯
				lightObject.GetComponent<Renderer> ().material = scLightMaterial [2];
			} else {			//消灯
				lightObject.GetComponent<Renderer> ().material = scLightMaterial [3];
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
	}

	//白灯の切り替え
	public void SettingLightC(GameObject lightObject, bool lightStatus){
		//電球式
		if (lightType == 0) {	
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				if (pointLight.intensity < CONSTS.MAX_INTENSITY) {
					pointLight.intensity += (CONSTS.MAX_INTENSITY / changeSpeed);
				}
			} else {			//消灯
				if (pointLight.intensity > 0) {
					pointLight.intensity -= (CONSTS.MAX_INTENSITY / changeSpeed);
				}
			}
		} //LED電球
		else if (lightType == 2) {
			Light pointLight;	
			pointLight = lightObject.GetComponent<Light> ();
			if (lightStatus) {	//点灯
				pointLight.intensity = CONSTS.MAX_INTENSITY;
			} else {			//消灯
				pointLight.intensity = 0;
			}
		}
	}
}
