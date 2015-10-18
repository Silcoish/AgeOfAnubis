/*using UnityEngine;
using System.Collections;

public class DungeonGeneration {

	DungeonSets.DungeonPieces set;

	[System.Flags]
	enum Options
	{
		UP		= (1 << 0),
		RIGHT	= (1 << 1),
		DOWN	= (1 << 2),
		LEFT	= (1 << 3)
	}

	public enum Direction
	{
		UP,
		DOWN,
		LEFT,
		RIGHT
	}

	public void LoadSet(DungeonSets.DungeonPieces s)
	{
		set = s;
	}

	public Dungeon.Room CreateRoom(bool startRoom)
	{
		return new Dungeon.Room(set, startRoom);
	}

	/*public void CreateDoor(Dungeon.Room room, Options options)
	{
		if ((options & Options.UP) != 0)
			Debug.Log("UP");
		if ((options & Options.RIGHT) != 0)
			Debug.Log("RIGHT");
		if (options & Options.DOWN)
			Debug.Log("DOWN");
		if (options & Options.LEFT)
			Debug.Log("LEFT");
	}

}
*/