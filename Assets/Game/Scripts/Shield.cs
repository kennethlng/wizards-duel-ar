using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour 
{
	/*
	public int currentHealth;
	private int maxHealth = 50;

	void Start () 
	{
		currentHealth = maxHealth;	
	}
	
	void Update () 
	{
		
	}

	private void TakeDamage (int _amount)
	{
		currentHealth -= _amount;
	}

	void OnCollisionEnter (Collision _col)
	{
		if (_col.gameObject.tag == "Projectile")
		{
			// Deactivate the projectile
			_col.gameObject.SetActive(false);

			// If the projectile is not my own
			if (_col.gameObject.GetComponent<Projectile>().ownerPeerId != GetComponentInParent<PlayerManager>().peerId)
			{
				print("taking damage");
				TakeDamage(_col.gameObject.GetComponent<Projectile>().damage);
			}
		}
	}

	void OnDisable ()
	{

	}

	void OnEnable ()
	{
		//currentHealth = maxHealth;	
	}
	 */
}
