using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerManager))]
public class SetupPlayerEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        
        PlayerManager myScript = (PlayerManager)target;
        if(GUILayout.Button("Setup"))
        {
            myScript.Setup();
        }
	}
}
