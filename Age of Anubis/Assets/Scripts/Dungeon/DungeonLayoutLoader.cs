using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class DungeonLayoutLoader : MonoBehaviour
{
	
	[Tooltip("If you set this value, it will use the specified layout instead of randomly choosing one")]
	public string fileName = "";
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
					//GameObject tempDoor1 = (GameObject)Instantiate(doorEast, (Vector2)rooms[i].gameObject.transform.position + new Vector2(doorOffset[1], 0f), doorEast.transform.rotation);
					//GameObject tempDoor2 = (GameObject)Instantiate(doorWest, (Vector2)rooms[i + 1].gameObject.transform.position - new Vector2(doorOffset[3], 0f), doorWest.transform.rotation);
					//tempDoor1.transform.parent = rooms[i].gameObject.transform;
					//tempDoor2.transform.parent = rooms[i + 1].gameObject.transform;

					Door tempDoor1Door = rooms[i].m_doorWest;//tempDoor1.GetComponent<Door>();
					Door tempDoor2Door = rooms[i].m_doorEast;//tempDoor2.GetComponent<Door>();
					tempDoor1Door.partnerDoor = tempDoor2Door.gameObject.transform;
                    tempDoor2Door.partnerDoor = tempDoor1Door.gameObject.transform;
					tempDoor1Door.parentRoom = rooms[i].gameObject.transform;
					tempDoor2Door.parentRoom = rooms[i + 1].gameObject.transform;

					//rooms[i].doorEast = tempDoor1Door;
					//rooms[i + 1].doorWest = tempDoor2Door;
				}
			}

			//check for room on the bottom
			if(i < (SIZE * SIZE) - SIZE)
			{
				if(rooms[i] != null && rooms[i + SIZE] != null)
				{
					GameObject tempDoor1 = (GameObject)Instantiate(doorSouth, (Vector2)rooms[i].gameObject.transform.position - new Vector2(0f, doorOffset[2]), doorSouth.transform.rotation);
					GameObject tempDoor2 = (GameObject)Instantiate(doorNorth, (Vector2)rooms[i + SIZE].gameObject.transform.position + new Vector2(0f, doorOffset[0]), doorNorth.transform.rotation);
					tempDoor1.transform.parent = rooms[i].gameObject.transform;
					tempDoor2.transform.parent = rooms[i + SIZE].gameObject.transform;

                    Door tempDoor1Door = tempDoor1.GetComponent<Door>();
                    Door tempDoor2Door = tempDoor2.GetComponent<Door>();
                    tempDoor1Door.partnerDoor = tempDoor2.transform;
                    tempDoor2Door.partnerDoor = tempDoor1.transform;
					tempDoor1Door.parentRoom = rooms[i].gameObject.transform;
					tempDoor2Door.parentRoom = rooms[i + SIZE].gameObject.transform;

					rooms[i].m_doorSouth = tempDoor1Door;
					rooms[i + SIZE].m_doorNorth = tempDoor2Door;
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
							if(entries[i] != "0" && entries[i] != " 0")
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
								GameObject tempPlayer = (GameObject)Instantiate(player, rooms[lineNum * SIZE + i].gameObject.transform.position, Quaternion.identity);
                                //GameManager.inst.player = tempPlayer;
								//Camera.main.GetComponent<CameraSystem>().MoveRoom(rooms[lineNum * SIZE + i].transform);
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
