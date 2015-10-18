/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonSets : MonoBehaviour {

	void Awake()
	{
		if (GameManager.inst.dungeonSets == null)
		{
			GameManager.inst.dungeonSets = this;
		}
		else
		{
			Destroy(this);
		}

	}

	[System.Serializable]
	public class DungeonPieces
	{
		[SerializeField] public GameObject player;
		[SerializeField] public GameObject room;
		[SerializeField] public GameObject northDoor;
		[SerializeField] public GameObject eastDoor;
		[SerializeField] public GameObject southDoor;
		[SerializeField] public GameObject westDoor;
		[SerializeField] public List<GameObject> enemies;
		[SerializeField] public List<GameObject> obstacles;
		[SerializeField] public List<GameObject> traps;
	};

	[SerializeField] public List<DungeonPieces> set;
}
*/