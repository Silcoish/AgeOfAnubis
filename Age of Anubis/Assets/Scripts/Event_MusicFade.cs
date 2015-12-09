/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class Event_MusicFade : MonoBehaviour
{
    
	void OnTriggerEnter2D(Collider2D col)
	{
		if(col.tag == "Player")
		{
			AudioManager.Inst.FadeMusic(AudioManager.Inst.s_temple, 2.0f);
			Destroy(gameObject);
		}
	}
}