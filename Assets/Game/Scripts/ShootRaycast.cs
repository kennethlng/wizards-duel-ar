using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRaycast : MonoBehaviour 
{
	public GameObject abilityParticlesPrefab;
	private ParticleSystem abilityParticles;
	private float abilityRange = 5f;
	public Transform abilitySpawn;

	private WaitForSeconds shotDelay = new WaitForSeconds(0.15f);

	public GameObject hitParticlesPrefab;
	private ParticleSystem hitParticles;

	// Use this for initialization
	public void Setup ()
	{
		abilityParticles = Instantiate(abilityParticlesPrefab, abilitySpawn).GetComponent<ParticleSystem>();
		hitParticles = Instantiate(hitParticlesPrefab).GetComponent<ParticleSystem>();
	}

	public void Shoot ()
	{
		StartCoroutine(DelayedRaycast());
		abilityParticles.Play();
	}

	private IEnumerator DelayedRaycast ()
	{
		yield return shotDelay;

		Vector3 raycastOrigin = abilitySpawn.position;
		Debug.DrawRay(raycastOrigin, abilitySpawn.forward * abilityRange);

		RaycastHit hit; 

		if (Physics.Raycast(raycastOrigin, abilitySpawn.forward, out hit, abilityRange))
		{
			hitParticles.gameObject.transform.position = hit.point;
			hitParticles.Play();

			if (hit.collider.gameObject.tag == "Player")
			{
				//GameManager.Instance().BroadcastHit(GetComponentInParent<PlayerManager>().peerId);
			}
		}
	}
}
