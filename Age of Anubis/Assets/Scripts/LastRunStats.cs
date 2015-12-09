/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class LastRunStats : MonoBehaviour
{

	public static LastRunStats inst;

	public bool m_isEnded = false;

	public int startGold = 0;
	public int endGold = 0;
	public int enemiesKilled = 0;
	public bool died = false;
	public int hpPickups = 0;
	public int roomsCleared = 0;

	void Awake()
	{
		if(LastRunStats.inst == null)
		{
			LastRunStats.inst = this;
		}
		else
		{
			Destroy(gameObject);
		}

		DontDestroyOnLoad(gameObject);
	}
}