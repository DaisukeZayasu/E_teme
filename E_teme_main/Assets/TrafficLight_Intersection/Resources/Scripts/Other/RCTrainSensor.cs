using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCTrainSensor : MonoBehaviour {

    [System.NonSerialized] public bool sensed;      //電車を感知したかどうか

    // Use this for initialization
    void Start () {
        sensed = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //衝突判定(電車を感知したら実行)
    void OnTriggerEnter(Collider collisionObject) {
        sensed = true;
    }
}
