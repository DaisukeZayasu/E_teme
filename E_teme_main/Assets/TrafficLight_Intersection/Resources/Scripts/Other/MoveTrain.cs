using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrain : MonoBehaviour {

    private bool moveOn = false;
    public float speedKH = 60;
    private float speedNow;

    private Vector3 speedV;
    private Rigidbody rb;


	// Use this for initialization
	void Start () {
        speedNow = speedKH / 3600 * 1000;
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {

        if (Input.GetKey(KeyCode.Return)) {
            moveOn = true;
        }

        if (moveOn) {
            //速度ベクトル
            speedV = speedNow * this.transform.forward;
            //位置の更新（移動）
            rb.velocity = new Vector3(speedV.x, 0, speedV.z);

        }
    }
}
