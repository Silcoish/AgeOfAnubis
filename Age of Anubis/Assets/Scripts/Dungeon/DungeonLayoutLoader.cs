using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DungeonLayoutLoader : MonoBehaviour
{
	
	[Tooltip("If you set this value, it will use the specified layout instead of randomly choosing one")]
	public string fileName = "";
	public GameObject startRoom;
	public GameObject bossRoom;
	public List<GameObject> templateRooms;
	[SerializeField] RoomObject[] rooms;
	public GameObject player;
	public GameObject doorNorth;
	public GameObject doorSouth;
	public GameObject doorEast;
	public GameObject doorWest;
	public Vector2 roomOffset = new Vector2(19.2f, 10.8f);
	[Tooltip("North Door, East Door, South Door, West Door")]
	public Vector4 doorOffset = new Vector4(4.08f, 9.16f, 4.95f, 8.95f);

	private int SIZE = 15;

	void Start()
	{
		rooms = new RoomObject[SIZE * SIZE];

		if(fileName == "")
			ChooseLayout();
		SetupLayout();
		PlaceDoors();
	}

	void PlaceDoors()
	{
		for(int i = 0; i < SIZE * SIZE; i++)
		{
			//check for room on the left
			if((i + 1) % SIZE != 0)
			{
				if (rooms[i] != null && rooms[i + 1] != null)
				{
					Door tempEastDoor = rooms[i].m_doorEast;
					Door tempWestDoor = rooms[i + 1].m_doorWest;
					tempEastDoor.partnerDoor = tempWestDoor;
                    tempWestDoor.partnerDoor = tempEastDoor;
					tempEastDoor.parentRoom = rooms[i].gameObject.transform.FindChild("Doors");
					tempWestDoor.parentRoom = rooms[i + 1].gameObject.transform.FindChild("Doors");

				}
			}

			//check for room on the bottom
			if(i < (SIZE * SIZE) - SIZE)
			{
				if(rooms[i] != null && rooms[i + SIZE] != null)
				{
					Door tempSouthDoor = rooms[i].m_doorSouth;
                    Door tempNorthDoor = rooms[i + SIZE].m_doorNorth;
					tempSouthDoor.partnerDoor = tempNorthDoor;
					tempNorthDoor.partnerDoor = tempSouthDoor;
					tempSouthDoor.parentRoom = rooms[i].gameObject.transform.FindChild("Doors");
					tempNorthDoor.parentRoom = rooms[i + SIZE].gameObject.transform.FindChild("Doors");
                }
			}
		}
	}

	void ChooseLayout()
	{
		DirectoryInfo info = new DirectoryInfo(Application.dataPath + "/Layouts/");
		FileInfo[] fileInfo = info.GetFiles();
		List<string> fileNames = new List<string>();

		for (int i = 0; i < fileInfo.Length; i++)
		{
			if (!fileInfo[i].Name.Contains("meta"))
			{
				fileNames.Add(fileInfo[i].Name);
			}
		}

		fileName = fileNames[Random.Range(0, fileNames.Count)];
	}

	void SetupLayout()
	{
		string filePath = Application.dataPath + "/Layouts/" + fileName;

		string line;
		int lineNum = 0;
		StreamReader reader = new StreamReader(filePath);
		using (reader)
		{
			do
			{
				line = reader.ReadLine();

				if (line != null)
				{
					string[] entries = line.Split(',');
					if (entries.Length > 0)
					{
						for (int i = 0; i < entries.Length-1; i++)
						{
							if(entries[i] == "1" || entries[i] == " 1")
							{
								GameObject tempRoom = (GameObject)Instantiate(templateRooms[0], new Vector2(i * roomOffset.x, -lineNum * roomOffset.y), Quaternion.identity);
								rooms[lineNum * SIZE + i] = tempRoom.GetComponent<RoomObject>();
								rooms[lineNum * SIZE + i].m_enemiesParent = tempRoom.transform.FindChild("Enemies").gameObject;

								//rooms[lineNum * SIZE + i].m_enemiesCount = rooms[lineNum * SIZE + i].m_enemiesParent.transform.childCount;
								rooms[lineNum * SIZE + i].SetupEnemies();
								rooms[lineNum * SIZE + i].m_enemiesParent.SetActive(false);
							}

							if(entries[i] == "2" || entries[i] == " 2")
							{
								GameObject tempRoom = (GameObject)Instantiate(startRoom, new Vector2(i * roomOffset.x, -lineNum * roomOffset.y), Quaternion.identity);
								rooms[lineNum * SIZE + i] = tempRoom.GetComponent<RoomObject>();

								GameObject tempPlayer = (GameObject)Instantiate(player, rooms[lineNum * SIZE + i].gameObject.transform.position, Quaternion.identity);
							}
						}
					}
				}

				lineNum++;
			}
			while (line != null);

			reader.Close();
		}
	}
}