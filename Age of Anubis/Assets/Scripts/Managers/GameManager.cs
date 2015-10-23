using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager inst;
	public GameObject player;
	public SaveManager m_saveManager;
    public GameObject coinPrefab;
	public GameObject minimap;
	public Texture2D minimapTex;

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

	public void RefreshMinimap()
	{
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
				if(((y == 0 || y == 7) && (x == 7 || x == 8)) || ((x == 0 || x == 15) && (y == 3 || y == 4))) 
				{
					showCol = Color.magenta;
				}

				t.SetPixel(((i % 15) * 16) + x, ((t.height - 1 - (int)i / 15) * 8) + y, showCol);
			}
		}

		t.Apply();
	}

}
