using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;

public class CameraTracker : MonoBehaviour 
{
	[HideInInspector] public bool isUser; 					// Whether or not this gameObject is the User or Opponent
	[HideInInspector] public Vector3 goToPos, goToRot;
	private Transform anchorTransform;
	private Vector3 camRelativePosition;
	private Quaternion camRelativeRotation;
	private float updateRate = 0.1f;

	public void Setup () 
	{
		anchorTransform = GameManager.Instance().anchorTransform;

		if (isUser)
		{
			// At start, make sure the user's cameraAvatar gameObject is childed at the camera's (0,0,0) position and rotation 
			transform.localPosition = Vector3.zero;
			transform.localEulerAngles = Vector3.zero;

			// Send camera's position and rotation
			StartCoroutine(SendCameraMovement());
		}
		else 
		{
			goToPos = this.transform.localPosition;
			goToRot = this.transform.localEulerAngles;
		}
	}

	private IEnumerator SendCameraMovement () 
	{
		// Camera's position relative to the cloud anchor.
		camRelativePosition = anchorTransform.InverseTransformPoint(transform.position);

		// Camera's rotation relative to the cloud anchor.
		camRelativeRotation = Quaternion.Inverse(anchorTransform.rotation) *  transform.rotation;

		// Construct a packet containing the camera's position and rotation relative to the cloud anchor
		using (RTData data = RTData.Get())
		{
			data.SetVector3(1, camRelativePosition);
			data.SetVector3(2, camRelativeRotation.eulerAngles);

			// Send the packet with OpCode 2
			MultiplayerNetworkingManager.Instance().GetRTSession().SendData(2,GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
		}	

		yield return new WaitForSeconds (updateRate);
		StartCoroutine(SendCameraMovement());
	}

	void Update () 
	{
		// Update the opponent's camera avatar with the update data packet containing the opponent's latest position and rotation 
		if (!isUser)
		{
			this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, goToPos, Time.deltaTime / updateRate);
			this.transform.localRotation = Quaternion.Lerp(this.transform.localRotation, Quaternion.Euler(goToRot), Time.deltaTime / updateRate);
		}
	}
}
