/* Copyright (c) Dungeon Crawlers
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class Shrine : ActivateArea 
{
	public override void OnActivate()
	{
		base.OnActivate();

		GameManager.inst.player.GetComponent<Player>().LeaveDungeon();		
	}
    
}