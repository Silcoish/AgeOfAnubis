/*
 * using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dungeon : MonoBehaviour{

	public const int GRID_WIDTH  = 10;
	public const int GRID_HEIGHT = 10;


	[System.Serializable]
	public class Room
	{
		DungeonSets.DungeonPieces set;
		int x, y;
		bool isStartRoom = false;
		float xOffset = 19.2f;
		float yOffset = 10.8f;
		public GameObject room;
		public GameObject enemiesParentNode;
		public List<GameObject> doors;
		List<GameObject> enemies;
		List<GameObject> traps;

		public Room(DungeonSets.DungeonPieces sett, bool startRoom)
		{
			set = sett;
			doors = new List<GameObject>();
			enemies = new List<GameObject>();
			traps = new List<GameObject>();
			isStartRoom = startRoom;
		}

		public void SpawnRoom(int xx, int yy)
		{
			x = xx;
			y = yy;
			room = (GameObject)Instantiate(set.room, new Vector2(x * 19.2f, -y * 10.8f), Quaternion.identity);
			room.name = "Room";
			enemiesParentNode = new GameObject();
			enemiesParentNode.transform.parent = room.transform;
			enemiesParentNode.name = "Enemies";
		}

		public void CreateDoor(Door.Direction direction)
		{
			float xxOffset = 0, yyOffset = 0;
			GameObject doorToSpawn = null;
			switch(direction)
			{
				case Door.Direction.NORTH:
					xxOffset = 0;
					yyOffset = 4.09f;
					doorToSpawn = set.northDoor;
					break;
				case Door.Direction.SOUTH:
					xxOffset = 0;
					yyOffset = -4.92f;
					doorToSpawn = set.southDoor;
					break;
				case Door.Direction.EAST:
					xxOffset = 9.15f;
					yyOffset = 0;
					doorToSpawn = set.eastDoor;
					break;
				case Door.Direction.WEST:
					xxOffset = -8.95f;
					yyOffset = 0;
					doorToSpawn = set.westDoor;
					break;
				default:
					Debug.LogError("Creating a door with an invalid direction");
					break;
			}
			GameObject tempDoor = (GameObject)Instantiate(doorToSpawn, new Vector2(x * xOffset + xxOffset, -y * yOffset + yyOffset), doorToSpawn.transform.rotation);
			doors.Add(tempDoor);
			doors[doors.Count - 1].GetComponent<Door>().dir = direction;
		}

		public void SpawnEnemies()
		{
			if (!isStartRoom)
			{
				int amount = Random.Range(1, 7);
				for (int i = 0; i < amount; i++)
				{
					GameObject tempEnemy = (GameObject)Instantiate(set.enemies[Random.Range(0, set.enemies.Count)], new Vector2(x * xOffset, -y * yOffset), Quaternion.identity);
					tempEnemy.transform.parent = enemiesParentNode.transform;
					enemies.Add(tempEnemy);
					enemies[i].GetComponent<NoteSubscribe>().Sub();
				}
			}
			else
			{
				GameManager.inst.player = (GameObject)Instantiate(set.player, room.transform.position, Quaternion.identity);

				//GameManager.inst.player.GetComponent<Player>().SetRoomData(x, y, room);
			}

			enemiesParentNode.SetActive(false);
		}

		public void SpawnTraps()
		{
			if(Random.Range(0, 2) == 0)
			{
				GameObject tempTrap = (GameObject)Instantiate(set.traps[0], new Vector2(x * xOffset, -y * yOffset), Quaternion.identity);
				traps.Add(tempTrap);
			}
		}
	};

	[Range(0, GRID_WIDTH*GRID_HEIGHT)]
	public int roomAmount;
	[SerializeField] List<Room> rooms;
	public int[] grid;

	DungeonGeneration dg;

	public bool shift = false;
	 
	void Start()
	{
		if (GameManager.inst.dungeon == null)
		{
			GameManager.inst.dungeon = this;
		}
		else
		{
			Destroy(this);
		}

		Random.seed = 31231;

		grid = new int[GRID_WIDTH * GRID_HEIGHT];
		ResetRooms();

		CreateRooms();
	}

	void CreateRooms()
	{
		dg = new DungeonGeneration();
		dg.LoadSet(GameManager.inst.dungeonSets.set[0]);

		//PlaceRandomRooms();
		ShiftingGeneration();
	}

	void PlaceRandomRooms()
	{
		rooms.Add(dg.CreateRoom(true));
		grid[GRID_WIDTH * Random.Range(0, GRID_HEIGHT)] = 0;
		for(int i = 1; i < roomAmount; i++)
		{
			rooms.Add(dg.CreateRoom(false));
			int pos = GRID_WIDTH * Random.Range(0, GRID_HEIGHT) + Random.Range(0, GRID_WIDTH - 1);
			print(pos);
			grid[pos] = i;
		}
		SpawnRooms();
	}

	void ShiftingGeneration()
	{
		bool startRoomSet = false;
		for (int i = 0; i < roomAmount; i++)
		{
			if (Random.Range(i, roomAmount) == (roomAmount - 1) && !startRoomSet)
			{
				rooms.Add(dg.CreateRoom(true));
				startRoomSet = true;
			}
			else
				rooms.Add(dg.CreateRoom(false));
			if (Random.Range(0, 2) == 0)
				ShiftRoomsDown();
			else
				ShiftRoomsRight();
			grid[0] = i;
		}
		SpawnRooms();
	}

	void SpawnRooms()
	{
		for(int i = 0; i < grid.Length; i++)
		{
			if(grid[i] != -1)
			{
				rooms[grid[i]].SpawnRoom(i % GRID_WIDTH, (int)Mathf.Floor(i / GRID_HEIGHT));
			}
		}

		SpawnDoors();
	}

	void SpawnDoors()
	{
		for(int i = 0; i < grid.Length; i++)
		{
			if(grid[i] != -1)
			{
				if (grid[i + 1] != -1)
				{
					rooms[grid[i]].CreateDoor(Door.Direction.EAST);//.CreateDoor(rooms[grid[i]], DungeonGeneration.Direction.RIGHT);
					rooms[grid[i + 1]].CreateDoor(Door.Direction.WEST); //dg.CreateDoor(rooms[grid[i + 1]], DungeonGeneration.Direction.LEFT);
				}
				if(grid[i + GRID_WIDTH] != -1)
				{
					rooms[grid[i]].CreateDoor(Door.Direction.SOUTH);//.CreateDoor(rooms[grid[i]], DungeonGeneration.Direction.RIGHT);
					rooms[grid[i + GRID_WIDTH]].CreateDoor(Door.Direction.NORTH); //dg.CreateDoor(rooms[grid[i + 1]], DungeonGeneration.Direction.LEFT);
				}
			}
		}

		SpawnEnemies();
	}

	void SpawnEnemies()
	{
		for (int i = 0; i < grid.Length; i++)
		{
			if (grid[i] != -1)
			{
				rooms[grid[i]].SpawnEnemies();
			}
		}

		SpawnTraps();
	}

	void SpawnTraps()
	{
		for (int i = 0; i < grid.Length; i++)
		{
			if (grid[i] != -1)
			{
				rooms[grid[i]].SpawnTraps();
			}
		}
	}

	void ResetRooms()
	{
		for(int i = 0; i < grid.Length; i++)
		{
			grid[i] = -1;
		}

		rooms.Clear();
	}

	void ShiftRoomsRight()
	{
		
		for(int i = 0; i < GRID_HEIGHT; i++)
		{
			int last = grid[(i + 1) * GRID_WIDTH - 1];
			for (int index = GRID_WIDTH - 2; index >= 0; index--)
				grid[i * GRID_WIDTH + index + 1] = grid[i * GRID_WIDTH + index];
			grid[i * GRID_WIDTH] = last;
		}
	}

	void ShiftRoomsDown()
	{
		int[] last = new int[GRID_WIDTH];
		for (int j = 0; j < GRID_WIDTH; j++)
			last[j] = grid[GRID_WIDTH * GRID_HEIGHT - 1 - (GRID_WIDTH - 1 - j)];
		
		for(int i = GRID_HEIGHT - 2; i >= 0; i--)
		{
			for (int k = 0; k < GRID_WIDTH; k++)
			{
				grid[(i + 1) * GRID_WIDTH + k] = grid[i * GRID_WIDTH + k];
			}
		}

		for(int c = 0; c < GRID_WIDTH; c++)
		{
			grid[c] = last[c];
		}
	}

    /*
	public void SwitchRooms(Door.Direction dir)
	{
		//Check is Quest Complete
		if(GameManager.inst.questManager.currentQuest != null)
		{
			if (GameManager.inst.questManager.currentQuest.CheckProgress())
			{
				GameManager.inst.stats.hpCur = GameManager.inst.stats.hpMax;
				Application.LoadLevel(1);
			}
		}
		
		int tempX, tempY;
		Player tempPlayer = GameManager.inst.player.GetComponent<Player>();
		tempX = tempPlayer.roomData.x;
		tempY = tempPlayer.roomData.y;

		GetRoom(tempX, tempY).enemiesParentNode.SetActive(false);

		switch(dir)
		{
			case Door.Direction.NORTH:
				tempY -= 1;
				tempPlayer.roomData.currentRoom = GetRoom(tempX, tempY).room;
				tempPlayer.roomData.y -= 1;
				break;
			case Door.Direction.EAST:
				tempX += 1;
				tempPlayer.roomData.currentRoom = GetRoom(tempX, tempY).room;
				tempPlayer.roomData.x += 1;
				break;
			case Door.Direction.SOUTH:
				tempY += 1;
				tempPlayer.roomData.currentRoom = GetRoom(tempX, tempY).room;
				tempPlayer.roomData.y += 1;
				break;
			case Door.Direction.WEST:
				tempX -= 1;
				tempPlayer.roomData.currentRoom = GetRoom(tempX, tempY).room;
				tempPlayer.roomData.x -= 1;
				break;
		}

		int intOppDir = (int)dir + 2;
		if(intOppDir >= 4)
			intOppDir -= 4;

		Room tempRoom = rooms[grid[tempY * GRID_WIDTH + tempX]];
		for(int i = 0; i < tempRoom.doors.Count; i++)
		{
			if ((int)tempRoom.doors[i].GetComponent<Door>().dir == intOppDir)
			{
				GameManager.inst.player.transform.position = tempRoom.doors[i].transform.GetChild(0).transform.position;
				tempRoom.enemiesParentNode.SetActive(true);
				

			}
		}

	}
     
    

	Room GetRoom(int x, int y)
	{
		return rooms[grid[y * GRID_WIDTH + x]];
	}

}
*/