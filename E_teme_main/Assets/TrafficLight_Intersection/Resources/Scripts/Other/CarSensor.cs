using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	
 *	車両感知器用のスクリプト
 */

public class CarSensor : MonoBehaviour {
	[System.NonSerialized] public bool sensed;		//車を感知したかどうか

	// Use this for initialization
	void Start () {
		sensed = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//衝突判定(車を感知したら実行)
	void OnTriggerEnter(Collider collisionObject) {
		//Debug.Log (collisionObject.name);
		sensed = true;
	}

}
