using UnityEngine;
using System.Collections;

public class EnterDungeon : ActivateArea 
{
	float m_timerColour = 0;
	public override void OnActivate()
	{
		base.OnActivate();


		//TODO Load Dungeon
		LoadingManager.Inst.LoadLevel("Room Prefab Scene");
		
	}


}
