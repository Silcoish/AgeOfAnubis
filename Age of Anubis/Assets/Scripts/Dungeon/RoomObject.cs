/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/

using UnityEngine;
using System.Collections.Generic;

public class RoomObject : MonoBehaviour
{

	public int roomLevel = 1;

	[HideInInspector] public List<Enemy> m_allEnemies;
	public Door m_doorNorth;
	public Door m_doorSouth;
	public Door m_doorEast;
	public Door m_doorWest;

	[HideInInspector] public bool isStartRoom = false;
	[HideInInspector] public int arrayIndex;

	public GameObject m_enemiesParent;

	bool isActiveRoom = false;
	bool cleared = false;

	void Awake()
	{
		m_allEnemies = new List<Enemy>();
		SetupEnemies();
		SetupDoors();
	}

	void Start()
	{
		Transform doorParentTransform = transform.FindChild("Doors");
		int doorChildren = doorParentTransform.childCount;
		for(int i = 0; i < doorChildren; i++)
		{
			Door childDoor = doorParentTransform.GetChild(i).GetComponent<Door>();
			Door.Direction doorDir = childDoor.dir;
			switch(doorDir)
			{
				case Door.Direction.NORTH:
					m_doorNorth = childDoor;
					break;
				case Door.Direction.EAST:
					m_doorEast = childDoor;
					break;
				case Door.Direction.SOUTH:
					m_doorSouth = childDoor;
					break;
				case Door.Direction.WEST:
					m_doorWest = childDoor;
					break;
				default:
					Debug.LogError("Error: Invalid Door Direction on GameObject " + gameObject.name);
					break;
			}
		}

		if(isStartRoom)
		{
			EnteredRoom();
		}
	}

	void Update()
	{
		//DEBUG TO DISABLE
		/*if(Input.GetKeyDown(KeyCode.K) && isActiveRoom)
		{
			for(int i = 0; i < m_allEnemies.Count; i++)
			{
				m_allEnemies[i].GetComponent<Enemy>().m_hitPoints = 0;
			}
		}*/
	}

	public void EnteredRoom()
	{
		isActiveRoom = true;
		

		foreach (var en in m_allEnemies)
		{
			en.gameObject.SetActive(true);
		}

		Camera.main.GetComponent<CameraController>().SetRoom(gameObject);

		//GameManager.inst.PlaceMinimapRoom(arrayIndex, new Color(1, 0, 0, 0.5f), new Color(1, 0.5f, 0.5f, 0.5f));
		//GameManager.inst.CreateVisibleMap(arrayIndex);
		GameManager.inst.currentRoom = arrayIndex;
		GameManager.inst.RefreshMinimap();

		if (m_allEnemies.Count == 0)
			return;

		LockDoors();
	}

	public void LeaveRoom()
	{
		isActiveRoom = false;
		GameManager.inst.ClearedRoom(arrayIndex);
		//GameManager.inst.PlaceMinimapRoom(arrayIndex, new Color(0, 0, 0, 0.5f), new Color(0.5f, 0.5f, 0.5f, 0.5f));
		GameManager.inst.RefreshMinimap();
	}

	void LockDoors()
	{
		print("Lock Doors");
		if(!cleared)
		{
			if (m_doorNorth != null)
				m_doorNorth.Lock();
			if (m_doorSouth != null)
				m_doorSouth.Lock();
			if (m_doorEast != null)
				m_doorEast.Lock();
			if (m_doorWest != null)
				m_doorWest.Lock();

			AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorShut);
			AudioManager.Inst.FadeMusic(AudioManager.Inst.s_fight);
		}
	}

	void UnlockDoors()
	{
		print("Unlock Doors");
		if (m_doorNorth != null)
			m_doorNorth.Unlock();
		if (m_doorSouth != null)
			m_doorSouth.Unlock();
		if (m_doorEast != null)
			m_doorEast.Unlock();
		if (m_doorWest != null)
			m_doorWest.Unlock();

		cleared = true;
        if (LastRunStats.inst != null)
		    LastRunStats.inst.roomsCleared++;

		PlayerInventory.Inst.IncreaseMultiplier();

		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorOpen);
		AudioManager.Inst.FadeMusic(AudioManager.Inst.s_idle);
	}

	public void SetupEnemies()
	{
		Enemy[] tempAllEnemies = gameObject.GetComponentsInChildren<Enemy>();

		foreach (var en in tempAllEnemies)
		{
			m_allEnemies.Add(en);
			en.SetRoom(this);
			en.gameObject.SetActive(false);
		}
	}

	void SetupDoors()
	{
		Door[] tempDoors = gameObject.GetComponentsInChildren<Door>();

		foreach (var d in tempDoors)
		{
			if (d.dir == Door.Direction.NORTH)
				m_doorNorth = d;
			else if (d.dir == Door.Direction.SOUTH)
				m_doorSouth = d;
			else if (d.dir == Door.Direction.EAST)
				m_doorEast = d;
			else if (d.dir == Door.Direction.WEST)
				m_doorWest = d;

			d.parentRoom = gameObject.transform;
		}


	}

	public void EnemyDied(Enemy en)
	{
		m_allEnemies.Remove(en);
		if (m_allEnemies.Count == 0)
			UnlockDoors();
	}

	public void PauseGame()
	{
		for(int i = 0; i < m_allEnemies.Count; i++)
		{
			m_allEnemies[i].PauseGame();
		}
	}

	public void UnpauseGame()
	{
		for (int i = 0; i < m_allEnemies.Count; i++)
		{
			m_allEnemies[i].UnpauseGame();
		}
	}


}