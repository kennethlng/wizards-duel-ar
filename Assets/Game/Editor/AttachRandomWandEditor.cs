using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Attack))]
public class AttachRandomWandEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        
        Attack myScript = (Attack)target;
        if(GUILayout.Button("Attach Wand"))
        {
            myScript.AttachRandomWand();
        }
	}
}
