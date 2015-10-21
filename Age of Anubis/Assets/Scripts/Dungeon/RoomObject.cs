/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/

using UnityEngine;
using System.Collections.Generic;

public class RoomObject : MonoBehaviour
{
	public List<Enemy> m_allEnemies;
	public Door m_doorNorth;
	public Door m_doorSouth;
	public Door m_doorEast;
	public Door m_doorWest;

	public GameObject m_enemiesParent;

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
	}

	public void EnteredRoom()
	{
		foreach (var en in m_allEnemies)
		{
			en.gameObject.SetActive(true);
		}

		LockDoors();
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

		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_doorShut);
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


}