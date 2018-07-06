using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchProjectile : MonoBehaviour 
{
	private float launchForce = 1000f;
	[HideInInspector] public Color projectileTrailColor;
	[HideInInspector] public int projectileOwnerPeerId;

	public void Launch ()
	{
		// Loop through all the projectiles in the projectile pool
		for (int i = 0; i < ProjectilePool.GetProjectilePool().Length; i++)
		{
			// Find one that is currently inactive
			if (!ProjectilePool.GetProjectilePool()[i].gameObject.activeSelf) 
			{
				// Assign values to the projectile
				ProjectilePool.GetProjectilePool()[i].spawnPos = gameObject.transform;
				ProjectilePool.GetProjectilePool()[i].trailColor = projectileTrailColor;
				ProjectilePool.GetProjectilePool()[i].ownerPeerId = projectileOwnerPeerId;

				// Initialize the projectile
				ProjectilePool.GetProjectilePool()[i].Reset();

				// Launch the projectile
				ProjectilePool.GetProjectilePool()[i].gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * launchForce);
				
				break;
			}
		}
	}
}
