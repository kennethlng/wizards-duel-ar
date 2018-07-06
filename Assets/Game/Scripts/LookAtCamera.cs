using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

	public bool faceCamera = true;

	void Update () 
	{
		Vector3 v = Camera.main.transform.position - transform.position;
		v.x = v.z = 0.0f;
		transform.LookAt(Camera.main.transform.position - v);
		
		if (faceCamera)
			transform.Rotate(0,180,0);
	}
}
