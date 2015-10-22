using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager inst;
	//public Dungeon dungeon;
	//public DungeonSets dungeonSets;
    //public Inventory inventory;
	public GameObject player;
	public SaveManager m_saveManager;
    public GameObject coinPrefab;

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
		try
		{
			player = FindObjectOfType<Player>().gameObject;
		}
		catch(Exception e)
		{
			print(e.Message);
		}
	}

}
