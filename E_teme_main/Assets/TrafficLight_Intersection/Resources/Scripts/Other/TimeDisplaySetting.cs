using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 *  待ち時間表示器設定用スクリプト
 */


public class TimeDisplaySetting : MonoBehaviour {

    //待ち時間表示器のタイプ
    //0:小糸ゲージ, 1:数字表示
    public int displayType;

    //青の経過時間も表示するかどうか
    public bool greenOn = true;

    //待ち時間表示オブジェクト
    private GameObject[] timeDisplay = new GameObject[2];

    //待ち時間表示マテリアル
    private Material[,] timeDisplayMaterial = new Material[2, 11];


    // Use this for initialization
    void Start() {

        //小糸ゲージ
        if (displayType == 0) {
            timeDisplay[0] = this.transform.Find("TimeDisplay").gameObject;
            for (int j = 0; j < timeDisplayMaterial.GetLength(1); j++) {
                timeDisplayMaterial[0, j] = Resources.Load("Materials/TimeGauge/TimeDisplay_Koito" + j + "_Red") as Material;
                timeDisplayMaterial[1, j] = Resources.Load("Materials/TimeGauge/TimeDisplay_Koito" + j + "_Green") as Material;
            }
        //数字表示
        } else if (displayType == 1) {
            timeDisplay[0] = this.transform.Find("TimeDisplay (1)").gameObject;     //十の位
            timeDisplay[1] = this.transform.Find("TimeDisplay (2)").gameObject;     //一の位
            for (int j = 0; j < timeDisplayMaterial.GetLength(1)-1; j++) {
                timeDisplayMaterial[0, j] = Resources.Load("Materials/TimeGauge/TimeDisplay_Number" + j + "_Red") as Material;
                timeDisplayMaterial[0, 10] = Resources.Load("Materials/TimeGauge/TimeDisplay_NumberOff") as Material;
                timeDisplayMaterial[1, j] = Resources.Load("Materials/TimeGauge/TimeDisplay_Number" + j + "_Green") as Material;
                timeDisplayMaterial[1, 10] = Resources.Load("Materials/TimeGauge/TimeDisplay_NumberOff") as Material;
            }
        }
    }

    //赤ゲージ設定用
    public void SettingTimeDisplayR(float cTime, float redStartTime, float redEndTime, float pRedTime) {

        float oneRedTime = pRedTime / 10f;
        int restRedTime;
        int tensPlace, onesPlace;
        int fiveTime;

        //小糸ゲージ
        if (displayType == 0) {
            if (cTime >= redStartTime) {
                if (cTime < redStartTime + oneRedTime) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 10];
                } else if (cTime < redStartTime + oneRedTime * 2) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 9];
                } else if (cTime < redStartTime + oneRedTime * 3) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 8];
                } else if (cTime < redStartTime + oneRedTime * 4) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 7];
                } else if (cTime < redStartTime + oneRedTime * 5) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 6];
                } else if (cTime < redStartTime + oneRedTime * 6) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 5];
                } else if (cTime < redStartTime + oneRedTime * 7) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 4];
                } else if (cTime < redStartTime + oneRedTime * 8) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 3];
                } else if (cTime < redStartTime + oneRedTime * 9) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 2];
                } else if (cTime < redStartTime + oneRedTime * 10) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 1];
                } else {
                    if (!greenOn) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 0];
                    }
                }
            } else {
                if (cTime < redEndTime - oneRedTime * 9) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 10];
                } else if (cTime < redEndTime - oneRedTime * 8) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 9];
                } else if (cTime < redEndTime - oneRedTime * 7) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 8];
                } else if (cTime < redEndTime - oneRedTime * 6) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 7];
                } else if (cTime < redEndTime - oneRedTime * 5) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 6];
                } else if (cTime < redEndTime - oneRedTime * 4) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 5];
                } else if (cTime < redEndTime - oneRedTime * 3) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 4];
                } else if (cTime < redEndTime - oneRedTime * 2) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 3];
                } else if (cTime < redEndTime - oneRedTime * 1) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 2];
                } else if (cTime < redEndTime) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 1];
                } else {
                    if (!greenOn) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, 0];
                    }
                }
            }

        //数字表示
        } else if(displayType == 1) {
            //残り赤時間の計算
            if(cTime >= redStartTime) {
                restRedTime = Mathf.CeilToInt(pRedTime - (cTime - redStartTime));             
            } else if (cTime < redEndTime) {
                restRedTime = Mathf.CeilToInt(redEndTime - cTime);
            } else {
                restRedTime = 0;
            }
            //5秒単位で計算
            fiveTime = (int)(pRedTime / 5) * 5;
            if (pRedTime >= 99 && restRedTime > 95) {
                tensPlace = 9;
                onesPlace = 9;
            } else if (restRedTime > fiveTime) {
                tensPlace = (int)pRedTime / 10;
                onesPlace = (int)pRedTime % 10;
            } else {
                fiveTime = Mathf.CeilToInt(restRedTime / 5.0f) * 5;
                tensPlace = fiveTime / 10;
                onesPlace = fiveTime % 10;
            }

            if (tensPlace == 0) {
                if (onesPlace == 0) {
                    onesPlace = 10;
                }
                tensPlace = 10;
            }
            //表示切替
            timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[0, tensPlace];   //十の位
            timeDisplay[1].GetComponent<Renderer>().material = timeDisplayMaterial[0, onesPlace];   //一の位
            
        }

    }

    //青ゲージ設定用
    public void SettingTimeDisplayG(float cTime, float greenStartTime, float redStartTime ,float pGreenTime) {

        float oneGreenTime = pGreenTime / 10f;
        int restGreenTime;
        int tensPlace, onesPlace;
        int fiveTime;

        if (displayType == 0) {
            if (cTime >= greenStartTime) {
                if (greenOn) {
                    if (cTime < greenStartTime + oneGreenTime) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 10];
                    } else if (cTime < greenStartTime + oneGreenTime * 2) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 9];
                    } else if (cTime < greenStartTime + oneGreenTime * 3) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 8];
                    } else if (cTime < greenStartTime + oneGreenTime * 4) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 7];
                    } else if (cTime < greenStartTime + oneGreenTime * 5) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 6];
                    } else if (cTime < greenStartTime + oneGreenTime * 6) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 5];
                    } else if (cTime < greenStartTime + oneGreenTime * 7) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 4];
                    } else if (cTime < greenStartTime + oneGreenTime * 8) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 3];
                    } else if (cTime < greenStartTime + oneGreenTime * 9) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 2];
                    } else if (cTime < greenStartTime + oneGreenTime * 10) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 1];
                    } else if (cTime < redStartTime) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 0];
                    }
                } else {
                    if (cTime < redStartTime) {
                        timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 0];
                    }
                }
            }

        //数字表示
        } else if (displayType == 1) {
            if (greenOn) {
                if (cTime > greenStartTime && cTime < redStartTime) {
                    //残り赤時間の計算
                    restGreenTime = Mathf.CeilToInt(pGreenTime - (cTime - greenStartTime));
                    if (restGreenTime < 0) {
                        restGreenTime = 0;
                    }
                    //5秒単位で計算
                    fiveTime = (int)(pGreenTime / 5) * 5;
                    if (pGreenTime >= 99 && restGreenTime > 95) {
                        tensPlace = 9;
                        onesPlace = 9;
                    } else if (restGreenTime > fiveTime) {
                        tensPlace = (int)pGreenTime / 10;
                        onesPlace = (int)pGreenTime % 10;
                    } else {
                        fiveTime = Mathf.CeilToInt(restGreenTime / 5.0f) * 5;
                        tensPlace = fiveTime / 10;
                        onesPlace = fiveTime % 10;
                    }

                    if (tensPlace == 0) {
                        if (onesPlace == 0) {
                            onesPlace = 10;
                        }
                        tensPlace = 10;
                    }

                    //表示切替
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, tensPlace];   //十の位
                    timeDisplay[1].GetComponent<Renderer>().material = timeDisplayMaterial[1, onesPlace];   //一の位
                }
            } else {
                if (cTime > greenStartTime && cTime < redStartTime) {
                    timeDisplay[0].GetComponent<Renderer>().material = timeDisplayMaterial[1, 10];   //十の位
                    timeDisplay[1].GetComponent<Renderer>().material = timeDisplayMaterial[1, 10];   //一の位
                }
            }
        }

    }
}
