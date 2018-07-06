using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;

public class Attack : MonoBehaviour 
{
	// Player Settings
	[HideInInspector] public bool isUser;
	[HideInInspector] public Team team;

	// Attack Settings
	private bool canAttack;
	private float coolDown = 1.0f;
	private float timer;
	private float longPressCutoff = 0.4f;
	private bool isPressed;

	// Ability Settings
	public GameObject abilityRoot;
	private LaunchProjectile projectileLauncher;

	// Shield Settings
	public GameObject shieldRoot;
	private bool shieldActivated;

	public void Setup () 
	{
		// Default values
		canAttack = true;
		shieldActivated = false;

		// Set up projectile launcher
		projectileLauncher = GetComponentInChildren<LaunchProjectile>();
		projectileLauncher.projectileTrailColor = team.color;
		projectileLauncher.projectileOwnerPeerId = GetComponent<PlayerManager>().peerId;

		// Attach a random wand based on the team that this player is in 
		AttachRandomWand();
	}

	public void AttachRandomWand ()
	{
		GameObject wand = Instantiate(team.wands[Random.Range(0, team.wands.Length)], abilityRoot.transform);
		wand.transform.localPosition = new Vector3 (0, -0.85f, -0.8f);
		wand.transform.localEulerAngles = new Vector3 (45, 0, 0);
		wand.transform.localScale = new Vector3 (2, 2, 2);
	}

	void Update () 
	{
		// Only the user can provide input to this Attack script
		if (isUser) 
		{
			if (canAttack) 
			{
				#region Touch Input
				if (Input.touchCount > 0)
				{
					Touch touch = Input.touches[0];

					// Reset the timer once the user touches the screen
					if (touch.phase == TouchPhase.Began)
					{
						timer = 0f;
						isPressed = true;
					}

					if (isPressed)
					{
						timer += Time.deltaTime;

						// If the timer is a long press, then activate the shield
						if (timer > longPressCutoff)
						{
							if (!shieldActivated)
							{
								using (RTData data = RTData.Get())
								{
									data.SetInt(1, 1);
            						MultiplayerNetworkingManager.Instance ().GetRTSession ().SendData (4, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data);

									TriggerShield(true);
								}
								shieldActivated = true;
							}
						}
					}

					// Check the timer amount when the user lifts his finger off the screen
					if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
					{
						if (timer < longPressCutoff)
						{
							// Tap ended
							StartCoroutine(AbilityCooldown());
						}
						else 
						{
							// Long press ended
							if (shieldActivated)
							{
								using (RTData data = RTData.Get())
								{
									data.SetInt(1, 2);
									MultiplayerNetworkingManager.Instance ().GetRTSession ().SendData (4, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data);

									TriggerShield(false);
								}
								shieldActivated = false;
							}
						}

						isPressed = false;
					}

					if (touch.phase == TouchPhase.Moved)
					{
						
					}
				}
				#endregion

				#region Keyboard Input
				// Reset the timer once the user touches the screen
				if (Input.GetKeyDown("space"))
				{
					timer = 0f;
					isPressed = true;
				}

				if (isPressed) 
				{
					timer += Time.deltaTime;

					// If the timer is a long press, then activate the shield
					if (timer > longPressCutoff)
					{
						if (!shieldActivated)
						{
							using (RTData data = RTData.Get())
							{
								data.SetInt(1, 1);
								MultiplayerNetworkingManager.Instance ().GetRTSession ().SendData (4, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data);

								TriggerShield(true);
							}
							shieldActivated = true;
						}
					}
				}

				// Check the timer amount when the user lifts his finger off the screen
				if (Input.GetKeyUp("space"))
				{
					if (timer < longPressCutoff)
					{
						// Tap ended
						StartCoroutine(AbilityCooldown());
					}
					else 
					{
						// Long press ended
						if (shieldActivated)
						{
							using (RTData data = RTData.Get())
							{
								data.SetInt(1, 2);
								MultiplayerNetworkingManager.Instance ().GetRTSession ().SendData (4, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data);

								TriggerShield(false);
							}
							shieldActivated = false;
						}
					}

					isPressed = false;
				}
				#endregion
			}
		}
	}

	// This function ensures that the player can only attack after a certain cooldown
	private IEnumerator AbilityCooldown() 
	{
		canAttack = false;

		// Send a data packet to GameSparks to notify opponent that player has attacked
		using (RTData data = RTData.Get()) {

			// Data doesn't need to contain anything since we are just notifying the other peers that players has attacked
			MultiplayerNetworkingManager.Instance().GetRTSession().SendData(3, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE, data); // send the data at op-code 3
		
			TriggerAbility();
		}  

		// Wait for the cooldown period before letting the player attack again
		yield return new WaitForSeconds (coolDown);	
		canAttack = true;
	}

	public void TriggerAbility() 
	{
		projectileLauncher.Launch();
	}

	public void TriggerShield (bool _isActivated)
	{
		shieldRoot.SetActive(_isActivated);
	}
}
