using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 電車感知用のスクリプト 
 */

public class StreetCarSensor : MonoBehaviour {

	[System.NonSerialized] public bool sensed;		//電車を感知したかどうか
	[System.NonSerialized] public GameObject streetcar;

	// Use this for initialization
	void Start () {
		sensed = false;
	}

	// Update is called once per frame
	void Update () {
		//電車が非アクティブになったら実行
		if (streetcar != null) {
			if (!streetcar.activeSelf) {
				sensed = false;
				//Debug.Log (" false");
			}
		}
	}

	//衝突判定(電車を感知したら実行)
	void OnTriggerEnter(Collider collisionObject) {
		//if (collisionObject.tag == "Streetcars") {
			streetcar = collisionObject.transform.root.gameObject;
			sensed = true;
			//Debug.Log (collisionObject.tag + " true");
		//}
	}

	//(電車がいなくなったら実行)
	void OnTriggerExit(Collider collisionObject) {
		sensed = false;
		//Debug.Log (" false");
	}

}
