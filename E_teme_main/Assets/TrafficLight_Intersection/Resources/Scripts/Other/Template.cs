using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	
 *	定数宣言用のスクリプト
 */

public class Template : MonoBehaviour {

}

//定数の名前空間
namespace Consts{
	public static class CONSTS{

		//青灯火のデフォルト点灯時間
		public const float G_TIME = 5f;
		//黄灯火のデフォルト点灯時間
		public const float Y_TIME = 3f;
		//全赤のデフォルト点灯時間
		public const float ALLR_TIME = 3f;
		//歩灯点滅時の点灯時間
		public const float BLINK_TIME = 0.25f;
		//変化の時間(歩灯赤→車灯黄になるまで、右左折車両分離式で歩灯赤→左折矢印までなど)
		public const float CHANGE_TIME = 3f;
		public const float CHANGE_TIME2 = 2f;

		//電球式灯火の強さの最大値
		public const float MAX_INTENSITY = 5f;
		//電球式矢印灯火の強さの最大値
		public const float MAX_A_INTENSITY = 6f;
		//電球式灯火（歩灯）の強さの最大値
		public const float MAX_P_INTENSITY = 8f;

		//通常時の自動車の加速度
		public const float ACCERALATION = 2f;
	}
}
	