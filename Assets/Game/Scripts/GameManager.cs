using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.RT;

public class GameManager : MonoBehaviour 
{
	// Singleton
	private static GameManager instance; 
	public static GameManager Instance() { return instance;}
	void Awake () { instance = this; }

	public GameObject playerPrefab;
	private List<PlayerManager> playerList = new List<PlayerManager> ();
	public List<PlayerManager> GetPlayerList () { return playerList; }

	public Transform anchorTransform;
	
	public Button spectateButton;

	public TeamRuntimeSet teamOptions;

	[Header("Matchmaking Parameters")]
	public string matchShortCode;
	public string matchGroup;

	void Start ()
	{
		// MatchFoundMessage listener. We are going to reference a method in this class to be called when this listener is called.
		GameSparks.Api.Messages.MatchFoundMessage.Listener += OnMatchFound;

        // MatchUpdatedMessage listener
        GameSparks.Api.Messages.MatchUpdatedMessage.Listener += OnMatchUpdated;
	}

	public void FindMatch() 
	{
		MultiplayerNetworkingManager.Instance().SendMatchmakingRequest(matchShortCode, 0, matchGroup);
	}

	private RTSessionInfo tempRTSessionInfo;
	private void OnMatchFound(GameSparks.Api.Messages.MatchFoundMessage _message) 
	{
        Debug.Log ("GM| Match Found!...");

		// Store the MatchFoundMessage data and have MultiplayerNetworkingManager start a RT session
		// Since MatchFoundMessage only happens once after sending a MatchmakingRequest (to start a new game), a new RT session instance is created each time. 
		tempRTSessionInfo = new RTSessionInfo (_message);
		MultiplayerNetworkingManager.Instance().StartNewRTSession(tempRTSessionInfo);
	}

	// Keeps the SessionInfo playerList updated with the current match participants. This is called each time the match is updated (e.g. a player joins or leaves the match). 
	private void OnMatchUpdated (GameSparks.Api.Messages.MatchUpdatedMessage _message)
    {
		MultiplayerNetworkingManager.Instance().GetSessionInfo().UpdateRTSessionInfo(_message);
    }

	// Use this for intialization. Called by MultiplayerNetworkingManager when the RT session is ready. 
	public void StartGame () 
	{
		// Loop through all the players in the RT session and add the player game objects into the scene.
		for (int i = 0; i < MultiplayerNetworkingManager.Instance().GetSessionInfo().GetPlayerList().Count; i++)
		{
			// Add the player to the game
			AddPlayer(MultiplayerNetworkingManager.Instance().GetSessionInfo().GetPlayerList()[i].peerId);
		}
		
		UIManager.Instance().ResetGameUI();	
	}

	// This method is called by MultiplayerNetworkingManager when an opponent connects to the RT session
	public void OnOpponentConnected(int _peerId)
	{
		Debug.Log("GM| Opponent connected: " + _peerId);

		AddPlayer(_peerId);
	}
	
	// This method is called by MultiplayerNetworkingManager when an opponent disconnects from the RT session
	public void OnOpponentDisconnected(int _peerId)
	{
		Debug.Log("GM| Opponent disconnected: " + _peerId);

		RemovePlayer(_peerId);
	}

	private void AddPlayer (int _peerId)
	{
		GameObject newPlayer = Instantiate(playerPrefab);	
		newPlayer.transform.SetParent(GetParentTransform(_peerId));
		newPlayer.GetComponent<PlayerManager>().peerId = _peerId;
		newPlayer.GetComponent<PlayerManager>().isUser = DetermineIsUser(_peerId);
		newPlayer.GetComponent<PlayerManager>().displayName = GetUsername(_peerId);
		newPlayer.GetComponent<PlayerManager>().team = AssignRandomTeam();
		newPlayer.GetComponent<PlayerManager>().Setup();

		// Add the new player to the player list
		playerList.Add(newPlayer.GetComponent<PlayerManager>());
	}

	private void RemovePlayer (int _peerId)
	{
		// Loop through all the players in the player list
		foreach (PlayerManager player in playerList)
		{
			// Find the player whose peerId is equal to the removed player's peer Id
			if (player.peerId == _peerId)
			{
				// Remove the player from the list of players
				playerList.Remove (player);

				// Destroy the gameObject
				Destroy(player.gameObject);
				break;
			}
		}
	}

	private bool DetermineIsUser (int _peerId)
	{
		// If it's the local user
		if (_peerId == MultiplayerNetworkingManager.Instance().GetRTSession().PeerId)
		{
			return true;
		}
		return false;
	}

	private Transform GetParentTransform (int _peerId)
	{
		// If it's the local user...
		if (_peerId == MultiplayerNetworkingManager.Instance().GetRTSession().PeerId)
		{
			// ... child the camera avatar to the camera, so the CameraTracker script can share the camera's positiona and rotation relative to the cloud anchor. 
			return Camera.main.transform;
		}

		// Otherwise, child the camera avatar to the cloud anchor so opponents share their localPosition (position relative to the cloud anchor transform)
		return anchorTransform;
	}

	private string GetUsername (int _peerId) 
	{
		// Loop through all the players
		for (int i = 0; i < MultiplayerNetworkingManager.Instance().GetSessionInfo().GetPlayerList().Count; i++)
		{
			// Find the player with this peer Id
			if (_peerId == MultiplayerNetworkingManager.Instance().GetSessionInfo().GetPlayerList()[i].peerId)
			{
				// Return the player's username
				return MultiplayerNetworkingManager.Instance().GetSessionInfo().GetPlayerList()[i].displayName;
			}
		}
		return "";
	}

	private Team AssignRandomTeam ()
	{
		return teamOptions.Items[Random.Range(0, teamOptions.Items.Count)];
	}

	public void SetSpectatorMode ()
	{
		for (int i = 0; i < playerList.Count; i++)
	 	{
			if (playerList[i].isUser)
			{
				using (RTData data = RTData.Get()) {

					// Data doesn't need to contain anything since we are just notifying the other peers that players has attacked
					MultiplayerNetworkingManager.Instance().GetRTSession().SendData(5, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data); // send the data at op-code 3
				
					playerList[i].EnterSpectatorMode();	
					spectateButton.interactable = false;
				}  
				break;
			}
		}
	}

	public void UpdateOpponentCameraMovement (RTPacket _packet) 
	{
		// Loop through all the players
	 	for (int i = 0; i < playerList.Count; i++)
	 	{
			// Find the player whose peerId is equal to the packet sender
			if (playerList[i].peerId == _packet.Sender)
		 	{
				playerList[i].cameraTrackerComponent.goToPos = (Vector3)_packet.Data.GetVector3(1);
				playerList[i].cameraTrackerComponent.goToRot = (Vector3)_packet.Data.GetVector3(2);

				// Break, because we don't need to update the other players
				break;
			}
	 	}
	}

	public void UpdateOpponentAbility (RTPacket _packet)
	{	
		// Loop through all the players
	 	for (int i = 0; i < playerList.Count; i++)
	 	{
			// Find the player whose peerId is equal to the packet sender
			if (playerList[i].peerId == _packet.Sender)
		 	{
				playerList[i].attackComponent.TriggerAbility();

				// Break, because we don't need to update the other players
				break;
			}
	 	}
	}

	public void UpdateOpponentShield (RTPacket _packet)
	{
		// Loop through all the players
	 	for (int i = 0; i < playerList.Count; i++)
	 	{
			// Find the player whose peerId is equal to the packet sender
			if (playerList[i].peerId == _packet.Sender)
		 	{
				switch (_packet.Data.GetInt(1))
				{
					case 1:
						playerList[i].attackComponent.TriggerShield(true);
						break;
					case 2:
						playerList[i].attackComponent.TriggerShield(false);	
						break;
				}

				// Break, because we don't need to update the other players
				break;
			}
	 	}
	}

	public void UpdateOpponentSpectatorMode (RTPacket _packet)
	{
		// Loop through all the players
	 	for (int i = 0; i < playerList.Count; i++)
	 	{
			// Find the player whose peerId is equal to the packet sender
			if (playerList[i].peerId == _packet.Sender)
		 	{
				playerList[i].EnterSpectatorMode();

				break;
			}
	 	}
	}

	public void UpdateOpponentProjectile (RTPacket _packet)
	{
		// Loop through the projectile pool
		for (int i = 0; i < ProjectilePool.GetProjectilePool().Length; i++)
		{
			if (ProjectilePool.GetProjectilePool()[i].ownerPeerId == _packet.Data.GetInt(1))
			{
				ProjectilePool.GetProjectilePool()[i].Collide ();
				break;
			}
		}
	}

	public void EndGame ()
	{
		// End the RT session
		MultiplayerNetworkingManager.Instance().GetRTSession().Disconnect();

		// Remove the players
		foreach (PlayerManager player in playerList)
		{
			// Remove the player from the list of players
			playerList.Remove (player);

			// Destroy the gameObject
			Destroy(player.gameObject);
			break;
		}

		// Enable the matchmaking UI
		UIManager.Instance().ResetMatchmakingUI();
	}

	public void EnableAttack()
	{
		// Loop through all the players
		for (int i = 0; i < playerList.Count; i++)
		{
			playerList[i].EnableAttack();
		}
	}

	public void DisableAttack()
	{
		// Loop through all the players
		for (int i = 0; i < playerList.Count; i++)
		{
			playerList[i].DisableAttack();
		}
	}
}
