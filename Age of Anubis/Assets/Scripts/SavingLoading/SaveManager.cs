/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class SaveManager : MonoBehaviour
{
	public int m_currentLevel;
	public float m_exp;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.L))
			SaveLoad.Load();
		if (Input.GetKeyDown(KeyCode.O))
			SaveLoad.Save();
	}
}