using UnityEngine;
using System.Collections;

public class EnterDungeon : ActivateArea 
{
	float m_timerColour = 0;
	public override void OnActivate()
	{
		base.OnActivate();

        Shop.Inst.ForceProgressShop();
        Shop.Inst.FlagToPrograssItems();

		//TODO Load Dungeon
		GameManager.inst.m_saveManager.Save();
		LoadingManager.Inst.LoadLevel("DungeonGenTest",true);

		GameManager.inst.player.GetComponent<Player>().m_isShopOpen = true;
		
	}


}
