using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	private bool can;

	// Use this for initialization
	void Start () 
	{
		can = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp("space") && can)
		{
			GameManager.Instance().FindMatch();
			can = false;
		}

		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) 
		{
			GameManager.Instance().FindMatch();
			can = false;
		}
	}
}
