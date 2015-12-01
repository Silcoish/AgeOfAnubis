using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public enum RoomType
	{
		CURRENT,
		BOSS,
		CLEARED,
		START,
		SEEN,
		EMPTY
	}
	public static GameManager inst;
	public GameObject player;
	public SaveManager m_saveManager;
	public GameObject m_lastRunStats;
    public GameObject coinPrefab;
    public GameObject healthPotionPrefab;
	private float hpStartChance;
	public float hpDropChance = 0.2F;
	public int maxEnemiesBeforeHPDrop = 10;
	public GameObject minimap;
	public Texture2D minimapTex;
	Vector2 minimapStartPos;
	public GameObject visibleMap;
	public Texture2D visibleMapTex;

	public bool destroyNextScene = false;

	public Texture2D room_complete;
	public Texture2D room_current;
	public Texture2D room_boss;
	public Texture2D room_starting;
	public Texture2D room_notComplete;
	public Texture2D room_empty;

	[HideInInspector] public DungeonLayoutLoader dungeonLayout;
	[HideInInspector] public int currentRoom;
	[HideInInspector] public List<int> seenRooms;
	[HideInInspector] public List<int> clearedRooms;
	[HideInInspector] public List<int> shrineLocation;
	[HideInInspector] public int startLocation;
	[HideInInspector] public int endLocation;

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
        catch (Exception e)
        {
            print(e.Message);
        }

		hpStartChance = hpDropChance;
		seenRooms = new List<int>();
		clearedRooms = new List<int>();
		shrineLocation = new List<int>();
		CheckForObjects();
	}

	void OnLevelWasLoaded(int level){
		CheckForObjects();
	}

	void CheckForObjects()
	{
		if (GameObject.FindGameObjectWithTag("Minimap") != null)
		{
			visibleMap = GameObject.FindGameObjectWithTag("Minimap");
		}

		if (visibleMap != null)
			minimapStartPos = visibleMap.GetComponent<RectTransform>().localPosition;

		if (GameObject.FindGameObjectWithTag("Save") != null)
		{
			m_saveManager = GameObject.FindGameObjectWithTag("Save").GetComponent<SaveManager>();
		}
	}

	void Update()
	{
		if(player == null)
		{
			player = FindObjectOfType<Player>().gameObject;
		}
	}

	public void ClearedRoom(int i)
	{
		if(!clearedRooms.Contains(i))
		{
			clearedRooms.Add(i);
		}
	}

	public void AddShrine(int i)
	{
		if(!shrineLocation.Contains(i))
		{
			shrineLocation.Add(i);
		}
	}

	public void AddSeenRoom(int i)
	{
		if(!seenRooms.Contains(i))
		{
			seenRooms.Add(i);
		}
	}

	public void RefreshMinimap()
	{
		for (int i = 0; i < 15 * 15; i++)
		{
			PlaceMinimapRoom(i, Color.white, Color.white, RoomType.EMPTY);
		}
		//Place left room if it exists
		if (currentRoom % 15 != 0)
		{
			if (dungeonLayout.rooms[currentRoom - 1] != null)
				AddSeenRoom(currentRoom - 1);
		}
		//Place right room if it exists
		if(currentRoom != ((15 * 15) - 1) && (currentRoom + 1) % 15 != 0)
		{
			if (dungeonLayout.rooms[currentRoom + 1] != null)
				AddSeenRoom(currentRoom + 1);
		}
		//top
		if(currentRoom >= 15)
		{
			if (dungeonLayout.rooms[currentRoom - 15] != null)
				AddSeenRoom(currentRoom - 15);
		}
		//bottom
		if(currentRoom <= ((15 * 15) - 1) - 15)
		{
			if (dungeonLayout.rooms[currentRoom + 15] != null)
				AddSeenRoom(currentRoom + 15);
		}

		for (int k = 0; k < seenRooms.Count; k++)
		{
			PlaceMinimapRoom(seenRooms[k], Color.white, Color.black, RoomType.SEEN);
		}

		for (int i = 0; i < clearedRooms.Count; i++)
		{
			PlaceMinimapRoom(clearedRooms[i], Color.gray, Color.black, RoomType.CLEARED);
		}

		for (int j = 0; j < shrineLocation.Count; j++)
		{
			PlaceMinimapRoom(shrineLocation[j], Color.yellow, Color.black, RoomType.START);
		}
		PlaceMinimapRoom(endLocation, Color.red, Color.black, RoomType.BOSS);
		PlaceMinimapRoom(startLocation, Color.green, Color.black, RoomType.START);
		PlaceMinimapRoom(currentRoom, Color.blue, Color.black, RoomType.CURRENT);
		CreateVisibleMap(currentRoom);
		minimapTex.Apply();
		minimap.GetComponent<SpriteRenderer>().sprite = Sprite.Create(minimapTex, new Rect(0, 0, minimapTex.width, minimapTex.height), new Vector2(0, 0));
	}

	public void PlaceMinimapRoom(int i, Color solid, Color border, RoomType type)
	{
		PlaceMinimapRoom(minimapTex, i, solid, border, type);
	}

	public void PlaceMinimapRoom(Texture2D t, int i, Color solid, Color border, RoomType type)
	{
		Color showCol = solid;
		for (int y = 0; y < 8 * 5; y++)
		{
			for (int x = 0; x < 16 * 5; x++)
			{
				Color col = new Color();
				switch(type)
				{
					case RoomType.START:
						col = room_starting.GetPixel(x, y);
						break;
					case RoomType.SEEN:
						col = room_notComplete.GetPixel(x, y);
						break;
					case RoomType.BOSS:
						col = room_boss.GetPixel(x, y);
						break;
					case RoomType.CURRENT:
						col = room_current.GetPixel(x, y);
						break;
					case RoomType.CLEARED:
						col = room_complete.GetPixel(x, y);
						break;
					case RoomType.EMPTY:
						col = room_empty.GetPixel(x, y);
						break;
				}
				t.SetPixel(((i % 15) * 80) + x, ((t.height - 1 - (int)i / 15) * 40) + y, col);
			}
		}

		t.Apply();
	}

	public void CreateVisibleMap(int i)
	{
		UnityEngine.UI.Image img = visibleMap.GetComponent<UnityEngine.UI.Image>();

		int xOffset = i % 15;
		int yOffset = i / 15;

		print("xOffset: " + xOffset);
		print("YOffset: " + yOffset);

		img.sprite = Sprite.Create(minimapTex, new Rect(Vector2.zero, new Vector2(minimapTex.width, minimapTex.height)), Vector2.zero);
		print("Moving to: " + new Vector2(minimapStartPos.x - (xOffset * (720 / 15)), minimapStartPos.y + (yOffset * (720 / 15))));
		visibleMap.GetComponent<RectTransform>().localPosition = new Vector2(minimapStartPos.x - (xOffset * (720 / 15) + (720 / 15)), minimapStartPos.y + (yOffset * (720 / 15)));

	}

	public bool CheckForHPDrop()
	{
		float increment = (1.0f - hpStartChance) / maxEnemiesBeforeHPDrop;
		if(UnityEngine.Random.Range(0.0f, 1.0f) <= hpDropChance)
		{
			hpDropChance = hpStartChance;
			return true;
		}

		hpDropChance += increment;

		return false;
	}

}
