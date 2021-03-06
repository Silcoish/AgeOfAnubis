using UnityEngine;
using UnityEditor;
using System.IO;

public class PrefabReset : EditorWindow
{

    [MenuItem("GameObject/PrefabReset_Enemies")]
	static void InitEnemies()
	{
		NestedPrefabs[] allPrefabs = GameObject.FindObjectsOfType<NestedPrefabs>();

		Debug.Log("Prfabs Found: " + allPrefabs.Length);

		foreach (var pf in allPrefabs)
		{
            if (pf.m_enemyType != EnemyType.Door)
			    pf.ResetPrefab();

		}

		GameObject[] allRooms = GameObject.FindGameObjectsWithTag("Room");

		Debug.Log("Rooms Found: " + allRooms.Length);

		foreach (var room in allRooms)
		{
			PrefabUtility.ReplacePrefab(room, PrefabUtility.GetPrefabParent(room), ReplacePrefabOptions.ConnectToPrefab);
		}

		

		Debug.Log("AllPrefabs Reset");
	}

    [MenuItem("GameObject/PrefabReset_Doors")]
    static void InitDoors()
    {
        NestedPrefabs[] allPrefabs = GameObject.FindObjectsOfType<NestedPrefabs>();

        Debug.Log("Prfabs Found: " + allPrefabs.Length);

        foreach (var pf in allPrefabs)
        {
            if (pf.m_enemyType == EnemyType.Door)
                pf.ResetPrefab();

        }

        GameObject[] allRooms = GameObject.FindGameObjectsWithTag("Room");

        Debug.Log("Rooms Found: " + allRooms.Length);

        foreach (var room in allRooms)
        {
            PrefabUtility.ReplacePrefab(room, PrefabUtility.GetPrefabParent(room), ReplacePrefabOptions.ConnectToPrefab);
        }



        Debug.Log("AllPrefabs Reset");
    }


	//void OnGUI()
	//{
	//	if (GUILayout.Button("MyButton"))
	//	{
	//		Debug.Log("MyButton");

	//	}



	//}
	



}