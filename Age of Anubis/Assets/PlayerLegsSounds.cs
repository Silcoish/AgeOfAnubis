/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class PlayerLegsSounds : MonoBehaviour
{
    public void PlayFootstep()
	{
		AudioManager.Inst.PlaySFX(AudioManager.Inst.a_player_step);
	}
}