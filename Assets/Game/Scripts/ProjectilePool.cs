using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour 
{
	public GameObject projectilePrefab;
	public int numProjectiles = 10;
	private static Projectile[] projectilePool;										// cached projectile pool we will be getting projectile instances from
	public static Projectile[] GetProjectilePool () { return projectilePool; }		// Getter for the projectile pool 
	
	// Create a singleton so it can be accessed by both clients
	private static ProjectilePool instance;
	public static ProjectilePool Instance() { return instance; }
	void Awake () { instance = this; }

	void Start () 
	{
		InstantiateProjectiles();
	}

	public void InstantiateProjectiles () 
	{
		projectilePool = new Projectile[numProjectiles];

		for (int i = 0; i < numProjectiles; i++ ) 
		{
			GameObject newProjectile = Instantiate(projectilePrefab);		// Instantiate projectile
			newProjectile.transform.SetParent(gameObject.transform);		// Attach it this gameObject to clean up the scene
			newProjectile.gameObject.SetActive(false);						// Deactivate it until it's ready to be used (called in Projectile.Setup())
			projectilePool[i] = newProjectile.GetComponent<Projectile>();	// Add it to the array
		}
	}
}
