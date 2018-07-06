using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public int currentHealth;
	public int maxHealth;

	public void Setup ()
	{
		currentHealth = maxHealth;
	}

	public void TakeDamage (int _amount)
	{
		currentHealth -= _amount;
	}
}
