using UnityEditor;
using UnityEngine;
using System.Collections;


[CustomEditor(typeof(AudioManager))]
public class Editor_AudioManager : Editor 
{

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		AudioManager myScript = (AudioManager)target;
		if (GUILayout.Button("Previw Sounds"))
		{
			myScript.Start();

			myScript.PreviewSounds();
		}
		if (GUILayout.Button("Clear Sources"))
		{
			myScript.ClearSources();
		}
	}
}
