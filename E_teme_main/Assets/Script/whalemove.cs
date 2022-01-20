using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class whalemove : MonoBehaviour
{
	[SerializeField] int speed;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		//キーボードの上下キーでの操作
		if (Input.GetKey("up"))
		{
			transform.position += transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey("down"))
		{
			transform.position -= transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetKey("right"))
		{
			transform.position += transform.right * speed * Time.deltaTime;
		}
		if (Input.GetKey("left"))
		{
			transform.position -= transform.right * speed * Time.deltaTime;
		}


		//コントローラーでの操作
		if (Input.GetAxis("Vertical") > 0)
		{
			transform.position += transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetAxis("Vertical") < 0)
		{
			transform.position -= transform.forward * speed * Time.deltaTime;
		}
		if (Input.GetAxis("Horizontal") > 0)
		{
			transform.position += transform.right * speed * Time.deltaTime;
		}
		if (Input.GetAxis("Horizontal") < 0)
		{
			transform.position -= transform.right * speed * Time.deltaTime;
		}

	}
}