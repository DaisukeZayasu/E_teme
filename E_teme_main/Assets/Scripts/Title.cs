﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown("space")) //スペースキーを押した場合
		{
			SceneManager.LoadScene("SampleScene");//some_senseiシーンをロードする
		}

	}
}