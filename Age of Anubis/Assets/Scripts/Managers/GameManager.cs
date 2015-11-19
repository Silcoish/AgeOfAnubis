using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager inst;
	public GameObject player;
	public SaveManager m_saveManager;
    public GameObject coinPrefab;
    public GameObject healthPotionPrefab;
    public float hpDropChance = 0.2F;
	public GameObject minimap;
	public Texture2D minimapTex;
	public GameObject visibleMap;
	public Texture2D visibleMapTex;

	public bool destroyNextScene = false;

	
	[HideInInspector] public DungeonLayoutLoader dungeonLayout;
	[HideInInspector] public int currentRoom;
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
		print("Start GM");
		try
		{
			player = FindObjectOfType<Player>().gameObject;
		}
		catch(Exception e)
		{
			print(e.Message);
		}

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

	public void RefreshMinimap()
	{
		for (int i = 0; i < clearedRooms.Count; i++)
		{
			PlaceMinimapRoom(clearedRooms[i], Color.gray, Color.black);
		}
		for (int j = 0; j < shrineLocation.Count; j++)
		{
			PlaceMinimapRoom(shrineLocation[j], Color.yellow, Color.black);
		}
		PlaceMinimapRoom(endLocation, Color.red, Color.black);
		PlaceMinimapRoom(startLocation, Color.green, Color.black);
		PlaceMinimapRoom(currentRoom, Color.blue, Color.black);
		CreateVisibleMap(currentRoom);
		minimapTex.Apply();
		minimap.GetComponent<SpriteRenderer>().sprite = Sprite.Create(minimapTex, new Rect(0, 0, minimapTex.width, minimapTex.height), new Vector2(0, 0));
	}

	public void PlaceMinimapRoom(int i, Color solid, Color border)
	{
		PlaceMinimapRoom(minimapTex, i, solid, border);
	}

	public void PlaceMinimapRoom(Texture2D t, int i, Color solid, Color border)
	{
		Color showCol = solid;
		for (int y = 0; y < 8; y++)
		{
			for (int x = 0; x < 16; x++)
			{
				showCol = solid;
				if (y == 0 || y == 7 || x == 0 || x == 15)
				{
					showCol = border;
				}
				//if(((y == 0 || y == 7) && (x == 7 || x == 8)) || ((x == 0 || x == 15) && (y == 3 || y == 4))) 
				//{
					//showCol = Color.magenta;
				//}

				t.SetPixel(((i % 15) * 16) + x, ((t.height - 1 - (int)i / 15) * 8) + y, showCol);
			}
		}

		t.Apply();

		//CreateVisibleMap();
	}

	public void CreateVisibleMap(int i)
	{
		SpriteRenderer sr = visibleMap.GetComponent<SpriteRenderer>();

		Texture2D tex = new Texture2D(16 * 7, 8 * 9);
		tex.filterMode = FilterMode.Point;

		for (int y = 0; y < tex.height; y++)
		{
			for (int x = 0; x < tex.width; x++)
			{
				tex.SetPixel(x, tex.height - y, minimapTex.GetPixel(x + ((int)(i % 15) * 16) - (3 * 16), minimapTex.height - y - ((int)(i / 15)* 8) + (4 * 8)));
			}
		}

		tex.Apply();

		sr.sprite = Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), Vector2.zero);
	}

}
