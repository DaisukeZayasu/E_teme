using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	
 *	歩行者目線カメラ用のスクリプト
 */

public class HumanCamera : MonoBehaviour {

    private Transform cameraPos;
    //Vector3 cameraForward;
    private Vector3 moveV;

	public float walkSpeed = 0.05f;		//移動速度
	public float viewSpeed = 1.0f;		//見渡す速度
	//AudioSource walkSound;


	// Use this for initialization
	void Start () {
		//カーソル非表示
		//Cursor.visible = false;
		//足音の設定
		//walkSound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
		//カメラの取得
		cameraPos = transform.Find("HumanCamera");

		float horiMove;
		float vertMove;
		float horiRotate = 0f;
		float vertRotate = 0f;

		//カーソル位置の取得
		horiRotate = Input.GetAxis ("Mouse X") * viewSpeed;
		vertRotate = Input.GetAxis ("Mouse Y") * viewSpeed;
	
		//見ている方向の更新（カメラのみ角度変更）
		cameraPos.RotateAround(cameraPos.position, Vector3.up, horiRotate);			//水平方向
		cameraPos.RotateAround(cameraPos.position, cameraPos.right, -vertRotate);	//上下方向


		//キーボードによる移動
		if (Input.GetKey (KeyCode.W)) {			//前進
			vertMove = walkSpeed;
		} else if (Input.GetKey (KeyCode.S)) {	//後退
			vertMove = -walkSpeed;
		} else {
			vertMove = 0f;
		}
		if (Input.GetKey (KeyCode.D)) {			//右移動
			horiMove = walkSpeed;
		} else if (Input.GetKey (KeyCode.A)) {	//左移動
			horiMove = -walkSpeed;
		} else {
			horiMove = 0f;
		}

		//足音の設定
		/*if (Input.GetKeyDown (KeyCode.W) || Input.GetKeyDown (KeyCode.S) 
			|| Input.GetKeyDown (KeyCode.D) || Input.GetKeyDown (KeyCode.A)) {
			walkSound.Play ();
		}
		if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.S)
		   || Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.A)) {	
		} else {
			walkSound.Stop ();
		}*/
		
		//カメラの方向
		//cameraForward = Vector3.Scale(cameraPos.forward, new Vector3 (1, 0, 1)).normalized;

		//移動ベクトル
		//moveV = vertMove * cameraForward + horiMove * cameraPos.right;
		moveV = vertMove * cameraPos.forward + horiMove * cameraPos.right;
		//位置の更新（グループごと移動）
		transform.position = new Vector3 (
			transform.position.x + moveV.x, transform.position.y, transform.position.z + moveV.z);

	}
}
