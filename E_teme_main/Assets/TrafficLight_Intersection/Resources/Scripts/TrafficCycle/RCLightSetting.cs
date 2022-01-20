using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Consts;

public class RCLightSetting : MonoBehaviour {

    //0:点灯, 1:消灯
    public Material[] lightMaterial = new Material[2];

    //光源のタイプ
    //0:電球, 1:LED, 2:LED集約式 or LED電球, 3:矢印
    public int lightType;

    //電球式変化のスピード
    private int changeSpeed = 10;


    //SettingLight(切り替える灯火のオブジェクト, 点灯(true) or 消灯(false))
    public void SettingRCLight(GameObject lightObject, bool lightStatus) {

        GameObject[] rcLights = new GameObject[6];

        //電球式
        if (lightType == 0) {
            Light pointLight;
            pointLight = lightObject.GetComponent<Light>();
            if (lightStatus) {  //点灯
                if (pointLight.intensity < CONSTS.MAX_INTENSITY) {
                    pointLight.intensity += (CONSTS.MAX_INTENSITY / changeSpeed);
                } else {
                    pointLight.intensity = CONSTS.MAX_INTENSITY;
                }
            } else {            //消灯
                if (pointLight.intensity > 0) {
                    pointLight.intensity -= (CONSTS.MAX_INTENSITY / changeSpeed);
                }
            }
        } //LED式
        else if (lightType == 1) {
            if (lightStatus) {  //点灯                
                for (int i = 0; i < 6; i++) {
                    lightObject.transform.Find("RCLED (" + (i + 1) + ")").gameObject.GetComponent<Renderer>().material
                        = lightMaterial[0];
                }
            } else {            //消灯                
                for (int i = 0; i < 6; i++) {
                    lightObject.transform.Find("RCLED (" + (i + 1) + ")").gameObject.GetComponent<Renderer>().material
                        = lightMaterial[1];
                }
            }
        } //LED電球 or LED集約式
        else if (lightType == 2 || lightType == 3) {
            Light pointLight;
            pointLight = lightObject.GetComponent<Light>();
            if (lightStatus) {  //点灯
                pointLight.intensity = CONSTS.MAX_INTENSITY;
            } else {            //消灯
                pointLight.intensity = 0;
            }
        } //矢印
        else if (lightType == 4) {
            if (lightStatus) {  //点灯                
                lightObject.GetComponent<Renderer>().material = lightMaterial[0];
            } else {            //消灯                
                lightObject.GetComponent<Renderer>().material = lightMaterial[1];
            }
        }
    }
}
