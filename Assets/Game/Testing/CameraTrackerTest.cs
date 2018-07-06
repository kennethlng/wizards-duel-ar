using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrackerTest : MonoBehaviour {

	public bool isUser;
	public Vector3 goToPos, goToRot;
	public Transform anchor;
	public GameObject otherCamera;

	// Use this for initialization
	void Start () {
		if (isUser)
		{
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;
			
			StartCoroutine(SendCameraMovement());
		}
		else
		{
			transform.SetParent(anchor);
			goToRot = this.transform.localEulerAngles;
			goToPos = this.transform.localPosition;
		}
	}
	
	private IEnumerator SendCameraMovement ()
	{
		Vector3 camRelativePosition = anchor.InverseTransformPoint(transform.position);
		Quaternion camRelativeRotation = Quaternion.Inverse(anchor.rotation) *  transform.rotation;

		otherCamera.GetComponent<CameraTrackerTest>().goToPos = camRelativePosition;
		otherCamera.GetComponent<CameraTrackerTest>().goToRot = camRelativeRotation.eulerAngles;

		yield return new WaitForSeconds(0.1f);
		StartCoroutine(SendCameraMovement());
	}

	// Update is called once per frame
	void Update () {
		if (!isUser)
		{
			this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, goToPos, Time.deltaTime / 0.1f);
			this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(goToRot), Time.deltaTime / 0.1f);
		}	
	}
}
