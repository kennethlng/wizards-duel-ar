using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StayOnGround : MonoBehaviour {

	Vector3 newPos;
	Vector3 newRot;

	void Update ()
	{
		// Remove rotations on the X and Z axis
		newRot = transform.eulerAngles;
		newRot.x = 0;
		newRot.z = 0;
		transform.eulerAngles = newRot;

		/*		
		newPos = Camera.main.transform.position - Camera.main.transform.forward;
		newPos.y = GameManager.Instance().anchorTransform.position.y;

		transform.position = newPos;
		*/

		
		// Keep the body collider level with the cloud anchor's Y position
		newPos = gameObject.transform.parent.position - transform.forward;
		//newPos.y = GameManager.Instance().anchorTransform.position.y;
		transform.position = newPos;
	}
}
