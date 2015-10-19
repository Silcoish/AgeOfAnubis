using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager inst;
	//public Dungeon dungeon;
	//public DungeonSets dungeonSets;
    //public Inventory inventory;
	public GameObject player;
	public SaveManager m_saveManager;

	void Awake()
	{
		if (GameManager.inst == null)
		{
			GameManager.inst = this;
			DontDestroyOnLoad(gameObject);
		}
		else
			Destroy(gameObject);
	}

	void Start()
	{
		player = FindObjectOfType<Player>().gameObject;
	}

}
