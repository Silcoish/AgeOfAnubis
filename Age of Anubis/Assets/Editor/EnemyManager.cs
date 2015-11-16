using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class EnemyManager : MonoBehaviour 
{
	public static EnemyManager Inst;
	public GameObject[] allScarabs = new GameObject[5];
	public GameObject[] allSpiders = new GameObject[5];
	public GameObject[] allSnakes = new GameObject[5];
	public GameObject[] allEyes = new GameObject[5];

    public GameObject m_doors;

#if UNITY_EDITOR


	// Update is called once per frame
	void Update () 
	{
		if (Inst == null)
			Inst = this;
	}

	public GameObject GetPrefab(EnemyType en, int level)
	{
        if (en != EnemyType.Door && level == 0)
            return null;
		switch (en)
		{
			case EnemyType.Scarab:
				if (allScarabs[level - 1] == null)
					Debug.LogError("Prefab Missing from EnmeyManager", allScarabs[level - 1]);
				return allScarabs[level - 1];
			case EnemyType.Spider:
				if (allSpiders[level - 1] == null)
					Debug.LogError("Prefab Missing from EnmeyManager", allSpiders[level - 1]);
				return allSpiders[level - 1];
			case EnemyType.Snake:
				if (allSnakes[level - 1] == null)
					Debug.LogError("Prefab Missing from EnmeyManager", allSnakes[level - 1]);
				return allSnakes[level - 1];
			case EnemyType.Eye:
				if (allEyes[level - 1] == null)
					Debug.LogError("Prefab Missing from EnmeyManager", allEyes[level - 1]);
				return allEyes[level - 1];
            case EnemyType.Door:
                return m_doors;
			default:
				Debug.LogError("Prefab Unknown to EnemyManager");
				return null;

		}



	}

#endif
}
