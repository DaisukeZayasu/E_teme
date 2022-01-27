using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BendCrossingBar : MonoBehaviour {

    private GameObject[] crossingBar = new GameObject[2];
    private float[] angleZ = new float[2];


	// Use this for initialization
	void Start () {
        crossingBar[0] = this.transform.Find("CrossingBar").gameObject;
        crossingBar[1] = this.transform.Find("CrossingBar/CrossingBar (2)").gameObject;
    }
	
	// Update is called once per frame
	void Update () {
        angleZ[0] = crossingBar[0].transform.localEulerAngles.z;
        angleZ[1] = -angleZ[0];
        crossingBar[1].transform.localRotation = Quaternion.Euler(0, 0, angleZ[1]);
    }
}
