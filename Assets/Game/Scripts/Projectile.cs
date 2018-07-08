using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.RT;

public class Projectile : MonoBehaviour 
{
	[HideInInspector] public int ownerPeerId;
	[HideInInspector] public Transform spawnPos;		// Where the projectile is spawned from. 
	[HideInInspector] public Color trailColor;
	public ProjectileSettings projectileSettings;
	public ParticleSystem trailParticles;				// Reference to the projectile's particle trail. Needed to change the color when a player fires the projectile
	private ParticleSystem collidedEffect;				// Particle effects to play when colliding with anything
	private float countdownTimer;						// Timer to count each projectile's lifetime from the moment it was shot
	private Rigidbody rigidbodyComponent;				// Reference to the rigidbody component

	void Start ()
	{
		collidedEffect = Instantiate(projectileSettings.collidedEffectsPrefab, this.gameObject.transform).GetComponentInChildren<ParticleSystem>();
		rigidbodyComponent = this.gameObject.GetComponent<Rigidbody>();
	}

	// Called each time the project is launched
	public void Reset ()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;				// Get rid of the velocity from previous shot
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;		// Get rid of the angular velocity from previous shot	
		GetComponent<Rigidbody>().isKinematic = false;
		transform.position = spawnPos.position;							// Set the spawn position
		transform.eulerAngles = spawnPos.eulerAngles;
		countdownTimer = 0;												// Reset the countdown timer for its inactivate itself

		// Set the color of the trail particles
		var trailParticleSystemRef = trailParticles.main;
		trailParticleSystemRef.startColor = trailColor;

		gameObject.SetActive(true);

		trailParticles.Play();
	}

	void Update ()
	{
		// Automatically destroy self after certain lifetime
		if (gameObject.activeSelf) 
		{
			countdownTimer += Time.deltaTime;

			if (countdownTimer >= projectileSettings.lifetime) 
			{
				this.gameObject.SetActive(false);
			}
		}
	}

	void OnCollisionEnter (Collision _col)
	{
		Debug.Log(_col.gameObject.name);

		using (RTData data = RTData.Get ()) 
		{
			data.SetInt(1, ownerPeerId);
			MultiplayerNetworkingManager.Instance().GetRTSession().SendData(6, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);

			Collide ();
		}  
	}

	public void Collide ()
	{
		GetComponent<Rigidbody>().velocity = Vector3.zero;				// Get rid of the velocity from previous shot
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;		// Get rid of the angular velocity from previous shot	
		GetComponent<Rigidbody>().isKinematic = true;
		collidedEffect.Play();
	}
}
