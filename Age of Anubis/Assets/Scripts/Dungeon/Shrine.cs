using UnityEngine;
 
public class Shrine : ActivateArea 
{
	public override void OnActivate()
	{
		base.OnActivate();

		if (LastRunStats.inst != null)
		{
			LastRunStats.inst.endGold = PlayerInventory.Inst.m_currentGold;
			LastRunStats.inst.died = false;
		}

		if (GameManager.inst.m_saveManager != null)
			GameManager.inst.m_saveManager.Save();

		GameManager.inst.player.GetComponent<Player>().LeaveDungeon();		
	}
    
}