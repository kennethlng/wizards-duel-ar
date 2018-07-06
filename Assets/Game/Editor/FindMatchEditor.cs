using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GameManager))]
public class FindMatchEditor : Editor 
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
        
        GameManager myScript = (GameManager)target;
        if(GUILayout.Button("Find Match"))
        {
            myScript.FindMatch();
        }
	}
}
