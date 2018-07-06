using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ProjectileSettings : ScriptableObject 
{
	public int damage;
	public float lifetime;
	public GameObject collidedEffectsPrefab;
}
