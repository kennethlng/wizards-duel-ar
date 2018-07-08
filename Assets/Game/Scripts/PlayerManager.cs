using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour 
{	
	//[HideInInspector] 
	public bool isUser;
	[HideInInspector] public int peerId;
	[HideInInspector] public string displayName;
	[HideInInspector] public Team team;
	[HideInInspector] public Attack attackComponent;
	[HideInInspector] public CameraTracker cameraTrackerComponent;
	public GameObject widget;
	public GameObject bodyRoot;

	public void Setup ()
	{
		attackComponent = GetComponent<Attack>();
		attackComponent.isUser = isUser;
		attackComponent.team = team;
		attackComponent.Setup();

		cameraTrackerComponent = GetComponent<CameraTracker>();
		cameraTrackerComponent.isUser = isUser;
		cameraTrackerComponent.Setup();

		widget.GetComponentInChildren<Text>().text = displayName;
		widget.GetComponentInChildren<Text>().color = team.color;
	}

	public void DisableAttack ()
	{
		attackComponent.enabled = false;
	}

	public void EnableAttack ()
	{
		attackComponent.enabled = true;
	}

	public void EnterSpectatorMode ()
	{
		gameObject.SetActive(false);
	}
}
