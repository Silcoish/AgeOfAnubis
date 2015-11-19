/* Copyright (c) Handsome Dragon Games
*  http://www.handsomedragongames.com
*  Script Created by:
*  Corey Underdown
*/
 
using UnityEngine;
 
public class StartingSnapshot : MonoBehaviour
{
	public UnityEngine.Audio.AudioMixerSnapshot startingSnapshot;

	void Start()
	{
		AudioManager.Inst.FadeMusic(startingSnapshot);
	}
}