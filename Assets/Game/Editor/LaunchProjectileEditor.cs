using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LaunchProjectile))]
public class LaunchProjectileEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        
        LaunchProjectile myScript = (LaunchProjectile)target;
        if(GUILayout.Button("Launch Projectile"))
        {
            myScript.Launch();
        }
	}
}
