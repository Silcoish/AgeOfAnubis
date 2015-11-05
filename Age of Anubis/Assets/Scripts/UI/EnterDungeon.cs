using UnityEngine;
using System.Collections;

public class EnterDungeon : ActivateArea 
{
	float m_timerColour = 0;
	public override void OnActivate()
	{
		base.OnActivate();


		//TODO Load Dungeon
		GameManager.inst.m_saveManager.Save();
		LoadingManager.Inst.LoadLevel("DungeonGenTest",true);
		
	}


}
