/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
using System.Collections.Generic;
 
public class RoomObject : MonoBehaviour
{
	private List<Enemy> m_allEnemies;
	private Door m_doorNorth;
	private Door m_doorSouth;
	private Door m_doorEast;
	private Door m_doorWest;

	void Awake()
	{
		m_allEnemies = new List<Enemy>();
		SetupEnemies();
		SetupDoors();
	}

	void Start()
	{


	}

	public void EnteredRoom()
	{
		foreach (var en in m_allEnemies)
		{
			en.gameObject.SetActive(true);
		}
		//Camera.main.GetComponent<CameraSystem>().MoveRoom(transform);
	}

	void LockDoors()
	{
		if (m_doorNorth != null)
			m_doorNorth.Lock();
		if (m_doorSouth != null)
			m_doorSouth.Lock();
		if (m_doorEast != null)
			m_doorEast.Lock();
		if (m_doorWest != null)
			m_doorWest.Lock();

		AudioManager.Inst.FadeMusic(AudioManager.Inst.s_fight);
	}

	void UnlockDoors()
	{
		if (m_doorNorth != null)
			m_doorNorth.Unlock();
		if (m_doorSouth != null)
			m_doorSouth.Unlock();
		if (m_doorEast != null)
			m_doorEast.Unlock();
		if (m_doorWest != null)
			m_doorWest.Unlock();

		AudioManager.Inst.FadeMusic(AudioManager.Inst.s_idle);
	}

	public void SetupEnemies()
	{
		Enemy[] tempAllEnemies = gameObject.GetComponentsInChildren<Enemy>();

		foreach (var en in tempAllEnemies)
		{
			m_allEnemies.Add(en);
			en.SetRoom(this);
		}
	}

	void SetupDoors()
	{
		Door[] tempDoors = gameObject.GetComponentsInChildren<Door>();

		foreach (var d in tempDoors)
		{
			if (d.m_dir == Door.Direction.NORTH)
				m_doorNorth = d;
			else if (d.m_dir == Door.Direction.SOUTH)
				m_doorSouth = d;
			else if (d.m_dir == Door.Direction.EAST)
				m_doorEast = d;
			else if (d.m_dir == Door.Direction.WEST)
				m_doorWest = d;

			d.m_parentRoom = gameObject.transform;
		}


	}

	public void EnemyDied(Enemy en)
	{
		m_allEnemies.Remove(en);
		if (m_allEnemies.Count == 0)
			UnlockDoors();
	}


}